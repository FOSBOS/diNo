using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
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

    private static readonly Logger log = Logger.Instance;

    // ---------------------------------------------------------------
    // Öffentliche Methode
    // ---------------------------------------------------------------

    /// <summary>
    /// Verarbeitet die XML-Datei und speichert die IDs direkt an den Kursen.
    /// </summary>
    public void VerarbeiteXml(string xmlDateiPfad)
    {
      log.Info($"=== AsvXmlKursMapper gestartet ===");
      log.Info($"XML-Datei: {xmlDateiPfad}");

      XDocument doc;
      try
      {
        doc = XDocument.Load(xmlDateiPfad);
      }
      catch (Exception ex)
      {
        log.Error($"FEHLER beim Laden der XML-Datei: {ex.Message}");
        return;
      }

      // Genau eine Schule
      var schule = Desc(doc.Root, "schule").FirstOrDefault();
      if (schule == null)
      {
        log.Error("FEHLER: Kein <schule>-Element in der XML-Datei gefunden.");
        return;
      }

      log.Info($"Schule gefunden: {(string)El(schule, "schulnummer") ?? "unbekannt"}");

      // Lookup-Tabellen aufbauen
      var faecher = LeseFaecher(schule);
      var ues = LeseUnterrichtselemente(schule);

      log.Info($"Fächer:              {faecher.Count}");
      log.Info($"Unterrichtselemente: {ues.Count}");

      var fachById = faecher.ToDictionary(f => f.XmlId);

      var fachliste = Zugriff.Instance.FachRep.getList();
      if (fachliste == null || !fachliste.Any())
      {
        log.Warn("WARNUNG: Keine Fächer in der Notenverwaltung gefunden.");
        return;
      }

      log.Info($"Fächer in Notenverwaltung: {fachliste.Count()}");
      log.Info(new string('-', 60));


      int gefunden = 0, nichtGefunden = 0, geaendert = 0;
      FachTableAdapter ta = new FachTableAdapter();

      foreach (Fach f in fachliste)
      {
        string fachKuerzel = f.Kuerzel;

        var ue = SucheUnterrichtselement(
            f, ues, fachById,
            out string hinweis);

        if (ue != null)
        {
          gefunden++;
          string aktuelleSchuelerfachId = f.Data.Isschuelerfach_idNull() ? "" : f.Data.schuelerfach_id;
          string aktuelleSchuleFachId = f.Data.Isschule_fach_idNull() ? "" : f.Data.schule_fach_id;

          if (aktuelleSchuelerfachId == ue.SchuelerfachId && aktuelleSchuleFachId == ue.SchuleFachId)
          {
            log.Info($"UNVERÄNDERT {f.Kuerzel} → schuelerfach_id={ue.SchuelerfachId}, " +
                $"schule_fach_id={ue.SchuleFachId} | {hinweis}");
          }
          else
          {
            log.Info($"GEÄNDERT {f.Kuerzel} → schuelerfach_id: '{aktuelleSchuelerfachId}' -> '{ue.SchuelerfachId}', " +
                $"schule_fach_id: '{aktuelleSchuleFachId}' -> '{ue.SchuleFachId}' | {hinweis}");
            f.Data.schuelerfach_id = ue.SchuelerfachId;
            f.Data.schule_fach_id = ue.SchuleFachId;
            ta.Update(f.Data); // Save() - nur bei tatsächlicher Änderung
            geaendert++;
          }
        }
        else
        {
          log.Error($"FEHLER  {f.Kuerzel} → nicht gefunden | {hinweis}");
          nichtGefunden++;
        }
      }

      log.Info(new string('-', 60));
      log.Info($"Ergebnis: {gefunden} gefunden ({geaendert} geändert), {nichtGefunden} nicht gefunden.");
      log.Info("=== AsvXmlKursMapper beendet ===");
    }

    // ---------------------------------------------------------------
    // Kern-Suchmethode
    // ---------------------------------------------------------------

    private XmlUnterrichtselement SucheUnterrichtselement(
    Fach f,
    List<XmlUnterrichtselement> alleUes,
    Dictionary<int, XmlFach> fachById,
    out string hinweis)
    {
      hinweis = null;

      // --- Alle Unterrichtselemente mit diesem Fach filtern ---
      var kandidaten = alleUes.Where(ue =>
      {
        if (!fachById.TryGetValue(ue.FachId, out var fach)) return false;
        return string.Equals(fach.Kurzform, f.Kuerzel,
                             StringComparison.OrdinalIgnoreCase);
      }).ToList();

      if (!kandidaten.Any())
      {
        hinweis = "Kein Unterrichtselement gefunden für Fach " + f.Bezeichnung + " " + f.Kuerzel;
        return null;
      }
      // --- Gruppieren nach SchuelerfachId und Häufigkeit zählen ---
      var gruppen = kandidaten
          .GroupBy(ue => ue.SchuelerfachId)
          .Select(g => new { SchuelerfachId = g.Key, Anzahl = g.Count(), Element = g.First() })
          .OrderByDescending(g => g.Anzahl)
          .ToList();

      // --- Zur Kontrolle: auch bei schulefachid ---
      var gruppen2 = kandidaten
          .GroupBy(ue => ue.SchuleFachId)
          .Select(g => new { SchuleFachId = g.Key, Anzahl = g.Count(), Element = g.First() })
          .OrderByDescending(g => g.Anzahl)
          .ToList();

      // --- Eindeutig: alle haben dieselbe SchuelerfachId ---
      if (gruppen.Count == 1 && gruppen2.Count == 1)
        return gruppen[0].Element;

      // --- Mehrdeutig: häufigstes Element zurückgeben ---
      var haeufigste = gruppen[0];
      hinweis = $"Mehrdeutige SchuelerfachId für Fach '{f.Kuerzel}': " +
                $"'{haeufigste.SchuelerfachId}' ({haeufigste.Anzahl}x) wurde gewählt. " +
                $"Weitere: {string.Join(", ", gruppen.Skip(1).Select(g => $"'{g.SchuelerfachId}' ({g.Anzahl}x)"))}" +
                $"schulefachid: {string.Join(", ", gruppen2.Select(g => $"'{g.SchuleFachId}' ({g.Anzahl}x)"))}";

      return haeufigste.Element;
    }


    // ---------------------------------------------------------------
    // XML-Lese-Hilfsmethoden
    // ---------------------------------------------------------------

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

    // ---------------------------------------------------------------
    // Parse-Hilfsmethoden
    // ---------------------------------------------------------------

    private static int ParseInt(XElement el)
        => el != null && int.TryParse((string)el, out int v) ? v : 0;

    private static int? ParseIntNullable(XElement el)
        => el != null && int.TryParse((string)el, out int v) ? v : (int?)null;
  }
}