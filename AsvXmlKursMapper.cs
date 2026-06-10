using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace diNo
{
  /// <summary>
  /// Liest aus einer ASV-Export-XML-Datei für alle bekannten Kurse die
  /// schuelerfach_id und schule_fach_id und speichert sie direkt am Kursobjekt.
  ///
  /// Matching-Strategie (in dieser Reihenfolge):
  /// 1. Lehrkraft-Kürzel (k.getLehrer.Kuerzel == lehrkraft->namenskuerzel)
  /// 2. Fach-Kürzel (k.getFach.Kuerzel == fach->kurzform)
  /// 3. Falls noch mehrdeutig: Klassen-Abgleich über schuelerin-Daten
  /// → das unterrichtselement mit den meisten Schüler-Treffern gewinnt
  /// </summary>
  public class AsvXmlKursMapper
  {
    // ---------------------------------------------------------------
    // Interne Hilfsklassen
    // ---------------------------------------------------------------


    private class XmlLehrkraft
    {
      public int XmlId { get; set; }
      public string Namenskuerzel { get; set; }
    }

    private class XmlFach
    {
      public int XmlId { get; set; }
      public string Kurzform { get; set; }
    }

    private class XmlUnterrichtselement
    {
      public int XmlId { get; set; }
      public int? LehrkraftId { get; set; }
      public int FachId { get; set; }
      public string SchuelerfachId { get; set; }
      public string SchuleFachId { get; set; }
    }

    private class XmlSchuelerinInfo
    {
      public int XmlId { get; set; }
      public string Klassenname { get; set; }
      public List<int> UnterrichtselementIds { get; set; } = new List<int>();
    }

    // ---------------------------------------------------------------
    // Hilfsmethode: Elemente ohne Namespace-Präfix ansprechen
    // ---------------------------------------------------------------

    /// <summary>
    /// Gibt das erste Kind-Element mit dem angegebenen lokalen Namen zurück,
    /// unabhängig vom XML-Namespace.
    /// </summary>
    private static XElement El(XElement parent, string localName)
        => parent?.Elements().FirstOrDefault(e => e.Name.LocalName == localName);

    /// <summary>
    /// Gibt alle Kind-Elemente mit dem angegebenen lokalen Namen zurück.
    /// </summary>
    private static IEnumerable<XElement> Els(XElement parent, string localName)
        => parent?.Elements().Where(e => e.Name.LocalName == localName)
           ?? Enumerable.Empty<XElement>();

    /// <summary>
    /// Gibt alle Nachfahren-Elemente mit dem angegebenen lokalen Namen zurück.
    /// </summary>
    private static IEnumerable<XElement> Desc(XElement parent, string localName)
        => parent?.Descendants().Where(e => e.Name.LocalName == localName)
           ?? Enumerable.Empty<XElement>();

    // ---------------------------------------------------------------
    // Logging
    // ---------------------------------------------------------------

    private StreamWriter _log;

    private void Log(string message)
    {
      string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
      _log.WriteLine(line);
      _log.Flush();
    }

    // ---------------------------------------------------------------
    // Öffentliche Methode
    // ---------------------------------------------------------------

    /// <summary>
    /// Verarbeitet die XML-Datei, speichert die IDs direkt an den Kursen
    /// und schreibt alle Meldungen nach C:\tmp\AsvXmlKursMapper_[Datum].log
    /// </summary>
    public void VerarbeiteXml(string xmlDateiPfad)
    {
      Directory.CreateDirectory(@"C:\tmp");
      string logPfad = $@"C:\tmp\AsvXmlKursMapper_{DateTime.Now:yyyyMMdd_HHmmss}.log";

      using (_log = new StreamWriter(logPfad, append: false, encoding: System.Text.Encoding.UTF8))
      {
        Log($"=== AsvXmlKursMapper gestartet ===");
        Log($"XML-Datei: {xmlDateiPfad}");

        XDocument doc;
        try
        {
          doc = XDocument.Load(xmlDateiPfad);
        }
        catch (Exception ex)
        {
          Log($"FEHLER beim Laden der XML-Datei: {ex.Message}");
          return;
        }

        // Genau eine Schule
        var schule = Desc(doc.Root, "schule").FirstOrDefault();
        if (schule == null)
        {
          Log("FEHLER: Kein <schule>-Element in der XML-Datei gefunden.");
          return;
        }

        Log($"Schule gefunden: {(string)El(schule, "schulnummer") ?? "unbekannt"}");

        // Lookup-Tabellen aufbauen
        var lehrkraefte = LeseLehrkraefte(schule);
        var faecher = LeseFaecher(schule);
        var ues = LeseUnterrichtselemente(schule);
        var schuelerinnen = LeseSchuelerinnen(schule);

        Log($"Lehrkräfte:          {lehrkraefte.Count}");
        Log($"Fächer:              {faecher.Count}");
        Log($"Unterrichtselemente: {ues.Count}");
        Log($"Schülerinnen:        {schuelerinnen.Count}");

        var lehrkraftById = lehrkraefte.ToDictionary(l => l.XmlId);
        var fachById = faecher.ToDictionary(f => f.XmlId);

        // Kurse holen
        var kurse = Zugriff.Instance.KursRep.getList();
        if (kurse == null || !kurse.Any())
        {
          Log("WARNUNG: Keine Kurse in der Notenverwaltung gefunden.");
          return;
        }

        Log($"Kurse in Notenverwaltung: {kurse.Count()}");
        Log(new string('-', 60));

        int gefunden = 0, nichtGefunden = 0;
         KursTableAdapter ta = new KursTableAdapter();

        foreach (var k in kurse)
        {
          string lehrerKuerzel = k.getLehrer?.Kuerzel ?? string.Empty;
          string fachKuerzel = k.getFach?.Kuerzel ?? string.Empty;
          string kursInfo = $"[{lehrerKuerzel} / {fachKuerzel}]";

          var ue = SucheUnterrichtselement(
              k, ues, lehrkraftById, fachById, schuelerinnen,
              kursInfo, out string hinweis);

          if (ue != null)
          {
            k.Data.schuelerfach_id = ue.SchuelerfachId;
            k.Data.schule_fach_id = ue.SchuleFachId;            
            ta.Update(k.Data); // Save()

            Log($"OK      {kursInfo} → schuelerfach_id={ue.SchuelerfachId}, " +
                $"schule_fach_id={ue.SchuleFachId} | {hinweis}");
            gefunden++;
          }
          else
          {
            Log($"FEHLER  {kursInfo} → nicht gefunden | {hinweis}");
            nichtGefunden++;
          }
        }

        Log(new string('-', 60));
        Log($"Ergebnis: {gefunden} gefunden, {nichtGefunden} nicht gefunden.");
        Log($"Log-Datei: {logPfad}");
        Log("=== AsvXmlKursMapper beendet ===");
      }
    }

    // ---------------------------------------------------------------
    // Kern-Suchmethode
    // ---------------------------------------------------------------

    private XmlUnterrichtselement SucheUnterrichtselement(
        dynamic kurs,
        List<XmlUnterrichtselement> alleUes,
        Dictionary<int, XmlLehrkraft> lehrkraftById,
        Dictionary<int, XmlFach> fachById,
        List<XmlSchuelerinInfo> schuelerinnen,
        string kursInfo,
        out string hinweis)
    {
      string lehrerKuerzel = kurs.getLehrer?.Kuerzel as string ?? string.Empty;
      string fachKuerzel = kurs.getFach?.Kuerzel as string ?? string.Empty;

      // --- Schritt 1: Nach Lehrkraft filtern ---
      var kandidaten = alleUes.Where(ue =>
      {
        if (!ue.LehrkraftId.HasValue) return false;
        if (!lehrkraftById.TryGetValue(ue.LehrkraftId.Value, out var lk)) return false;
        return string.Equals(lk.Namenskuerzel, lehrerKuerzel,
                             StringComparison.OrdinalIgnoreCase);
      }).ToList();

      // --- Schritt 2: Nach Fach filtern ---
      kandidaten = kandidaten.Where(ue =>
      {
        if (!fachById.TryGetValue(ue.FachId, out var fach)) return false;
        return string.Equals(fach.Kurzform, fachKuerzel,
                             StringComparison.OrdinalIgnoreCase);
      }).ToList();

      if (kandidaten.Count == 0)
      {
        hinweis = $"Kein Unterrichtselement für Lehrer '{lehrerKuerzel}' / Fach '{fachKuerzel}' gefunden.";
        return null;
      }

      if (kandidaten.Count == 1)
      {
        hinweis = "Eindeutig über Lehrer + Fach gefunden.";
        return kandidaten[0];
      }

      // --- Schritt 3: Mehrdeutig → Klassen-Abgleich über Schülerinnen ---
      var kursKlassen = new List<string>();
      try
      {
        foreach (var klasse in kurs.Klassen)
          kursKlassen.Add(klasse.Bezeichnung as string ?? string.Empty);
      }
      catch { /* Klassen nicht verfügbar */ }

      if (!kursKlassen.Any())
      {
        hinweis = $"Mehrdeutig ({kandidaten.Count} Treffer), keine Klasse verfügbar – ersten Kandidaten gewählt.";
        return kandidaten[0];
      }

      // Für jeden Kandidaten zählen, wie viele Schülerinnen
      // (a) dieses Unterrichtselement besuchen UND (b) in einer Kursklasse sitzen
      var gewertet = kandidaten
          .Select(ue =>
          {
            int treffer = schuelerinnen.Count(s =>
                  s.UnterrichtselementIds.Contains(ue.XmlId) &&
                  kursKlassen.Any(kk =>
                      string.Equals(kk, s.Klassenname, StringComparison.OrdinalIgnoreCase)));
            return (Ue: ue, Treffer: treffer);
          })
          .OrderByDescending(x => x.Treffer)
          .ToList();

      var bester = gewertet[0];

      if (bester.Treffer == 0)
      {
        hinweis = $"Mehrdeutig ({kandidaten.Count} Treffer), kein Schüler-Klassen-Treffer – ersten Kandidaten gewählt.";
        return bester.Ue;
      }

      bool eindeutig = gewertet.Count < 2 || gewertet[0].Treffer > gewertet[1].Treffer;
      hinweis = eindeutig
          ? $"Über Schüler-Klassen-Abgleich eindeutig bestimmt ({bester.Treffer} Treffer)."
          : $"Bester Kandidat mit {bester.Treffer} Treffern gewählt (Gleichstand möglich, Klassen: {string.Join(", ", kursKlassen)}).";

      return bester.Ue;
    }

    // ---------------------------------------------------------------
    // XML-Lese-Hilfsmethoden
    // ---------------------------------------------------------------

    private List<XmlLehrkraft> LeseLehrkraefte(XElement schule)
    {
      return Els(El(schule, "lehrkraefte"), "lehrkraft")
          .Select(lk => new XmlLehrkraft
          {
            XmlId = ParseInt(El(lk, "xml_id")),
            Namenskuerzel = (string)El(lk, "namenskuerzel") ?? string.Empty
          })
          .ToList();
    }

    private List<XmlFach> LeseFaecher(XElement schule)
    {
      return Els(El(schule, "faecher"), "fach")
          .Select(f => new XmlFach
          {
            XmlId = ParseInt(El(f, "xml_id")),
            Kurzform = (string)El(f, "kurzform") ?? string.Empty
          })
          .ToList();
    }

    private List<XmlUnterrichtselement> LeseUnterrichtselemente(XElement schule)
    {
      return Els(El(schule, "unterrichtselemente"), "unterrichtselement")
          .Select(ue => new XmlUnterrichtselement
          {
            XmlId = ParseInt(El(ue, "xml_id")),
            LehrkraftId = ParseIntNullable(El(ue, "lehrkraft_id")),
            FachId = ParseInt(El(ue, "fach_id")),
            SchuelerfachId = (string)El(ue, "schuelerfach_id") ?? string.Empty,
            SchuleFachId = (string)El(ue, "schule_fach_id") ?? string.Empty
          })
          .ToList();
    }

    private List<XmlSchuelerinInfo> LeseSchuelerinnen(XElement schule)
    {
      var result = new List<XmlSchuelerinInfo>();

      foreach (var s in Desc(schule, "schuelerin"))
      {
        var info = new XmlSchuelerinInfo
        {
          XmlId = ParseInt(El(s, "xml_id"))
        };

        // Aktuellsten Klassennamen aus schullaufbahnliste holen
        var letzterEintrag = Els(El(s, "schullaufbahnliste"), "schullaufbahn").LastOrDefault();
        if (letzterEintrag != null)
          info.Klassenname = (string)El(letzterEintrag, "klassenname") ?? string.Empty;

        // Alle besuchten unterrichtselement_ids sammeln
        foreach (var ueId in Desc(s, "unterrichtselement_id"))
        {
          if (int.TryParse((string)ueId, out int id))
            info.UnterrichtselementIds.Add(id);
        }

        result.Add(info);
      }

      return result;
    }

    // ---------------------------------------------------------------
    // Parse-Hilfsmethoden
    // ---------------------------------------------------------------

    private static int ParseInt(XElement el)
        => el != null && int.TryParse((string)el, out int v) ? v : 0;

    private static int? ParseIntNullable(XElement el)
        => el != null && int.TryParse((string)el, out int v) ? v : (int?)null;
  }
}