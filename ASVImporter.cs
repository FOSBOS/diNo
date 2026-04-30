using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Net;

namespace diNo
{
  /// <summary>
  /// Importiert ASV-Daten aus einer XML-Datei und weist jedem Schüler die ASV-ID zu.
  /// Matching erfolgt über Familienname, Vornamen und Geburtsdatum.
  /// Namen und Vornamen werden als wechselseitige Teilstrings geprüft.
  /// </summary>
  public class ASVImporter
  {
    private StringBuilder fehlerProtokoll;
    private StringBuilder erfolgsProtokoll;
    private int anzahlGesamt;
    private int anzahlErfolgreich;
    private int anzahlNichtGefunden;
    //private int anzahlMehrfach;
    private List<Schueler> alleSchueler;

    public ASVImporter()
    {
      fehlerProtokoll = new StringBuilder();
      erfolgsProtokoll = new StringBuilder();
      anzahlGesamt = 0;
      anzahlErfolgreich = 0;
      anzahlNichtGefunden = 0;
      //anzahlMehrfach = 0;
      alleSchueler = Zugriff.Instance.SchuelerRep.getList();
    }

    /// <summary>
    /// Importiert die ASV-Daten aus der angegebenen XML-Datei.
    /// </summary>
    /// <param name="xmlDateiPfad">Pfad zur ASV-XML-Datei</param>
    /// <returns>Anzahl der erfolgreich zugeordneten Schüler</returns>
    public int ImportiereASVDaten(string xmlDateiPfad)
    {
      try
      {
        // XML-Datei laden
        XDocument doc = XDocument.Load(xmlDateiPfad);
        XNamespace ns = "http://www.asv.bayern.de/import";

        // Alle Schülerinnen-Elemente finden
        var schuelerElemente = doc.Descendants(ns + "schuelerin");
        anzahlGesamt = schuelerElemente.Count();

        foreach (var schuelerElement in schuelerElemente)
        {
          VerarbeiteSchueler(schuelerElement, ns);
        }

        return anzahlErfolgreich;
      }
      catch (Exception ex)
      {
        fehlerProtokoll.AppendLine($"KRITISCHER FEHLER beim Laden der XML-Datei: {ex.Message}");
        return 0;
      }
    }

    private void VerarbeiteSchueler(XElement schuelerElement, XNamespace ns)
    {
      try
      {
        // ASV-Daten extrahieren
        string asvId = schuelerElement.Element(ns + "lokales_differenzierungsmerkmal")?.Value?.Trim();
        string familienname = schuelerElement.Element(ns + "familienname")?.Value?.Trim();
        string vornamen = schuelerElement.Element(ns + "vornamen")?.Value?.Trim();
        string geburtsdatumStr = schuelerElement.Element(ns + "geburtsdatum")?.Value?.Trim();

        // Validierung
        if (string.IsNullOrEmpty(asvId) || string.IsNullOrEmpty(familienname) ||
            string.IsNullOrEmpty(vornamen) || string.IsNullOrEmpty(geburtsdatumStr))
        {
          fehlerProtokoll.AppendLine($"FEHLER: Unvollständige Daten in XML - ASV-ID: {asvId ?? "leer"}");
          anzahlNichtGefunden++;
          return;
        }

        // Geburtsdatum parsen (Format: dd.MM.yyyy)
        DateTime geburtsdatum;
        if (!DateTime.TryParseExact(geburtsdatumStr, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out geburtsdatum))
        {
          fehlerProtokoll.AppendLine($"FEHLER: Ungültiges Geburtsdatum '{geburtsdatumStr}' - {familienname}, {vornamen} - ASV-ID: {asvId}");
          anzahlNichtGefunden++;
          return;
        }

        // Schüler im Repository suchen
        
        var gefundeneSchueler = new List<Schueler>();
        var ersatzSchueler = new List<Schueler>();

        foreach (var s in alleSchueler)
        {
          // Name: Wechselseitige Teilstring-Prüfung (beide Richtungen)
          bool namePasst = familienname.Contains(s.Name) || s.Name.Contains(familienname);

          // Vorname: Wechselseitige Teilstring-Prüfung (beide Richtungen)
          bool vornamePasst = vornamen.Contains(s.Vorname) || s.Vorname.Contains(vornamen);

          // Geburtsdatum: Exakte Übereinstimmung (nur Datum, ohne Uhrzeit)
          bool geburtsdatumPasst = s.Data.Geburtsdatum.Date == geburtsdatum.Date;

          if (namePasst && vornamePasst && geburtsdatumPasst)
          { 
            gefundeneSchueler.Add(s);
          }
          else if ((namePasst || vornamePasst) && geburtsdatumPasst)
          {
            ersatzSchueler.Add(s);
          }
        }

        // Auswertung
        Schueler schueler;
        if (gefundeneSchueler.Count == 1)
          schueler = gefundeneSchueler[0];
        else if (ersatzSchueler.Count == 1)
        {
          schueler = ersatzSchueler[0];
        }
        else
        {
          // Kein Schüler gefunden
          fehlerProtokoll.AppendLine($"FEHLER: Schüler nicht gefunden (Anzahl: {gefundeneSchueler.Count}/{ersatzSchueler.Count}) - {familienname}, {vornamen} (Geb: {geburtsdatumStr}) - ASV-ID: {asvId}");
          anzahlNichtGefunden++;
          return;
        }        
        
        // Genau ein Schüler gefunden - ASV-ID zuweisen           
        schueler.Data.asv_id = asvId;
        schueler.Save();
        anzahlErfolgreich++;

        // Erfolgsprotokoll mit Abweichungshinweisen
        StringBuilder erfolgsMeldung = new StringBuilder();
        erfolgsMeldung.AppendLine($"ERFOLG: {familienname}, {vornamen} (Geb: {geburtsdatumStr}) - ID: {schueler.Id} - ASV-ID: {asvId}");

        // Prüfen auf Namensabweichung
        if (!string.Equals(schueler.Name, familienname, StringComparison.OrdinalIgnoreCase))
        {
          erfolgsMeldung.AppendLine($"  HINWEIS: Namensabweichung - ASV: '{familienname}' vs. diNo: '{schueler.Name}'");
        }

        // Prüfen auf Vornamenabweichung
        if (!string.Equals(schueler.Vorname, vornamen, StringComparison.OrdinalIgnoreCase))
        {
          erfolgsMeldung.AppendLine($"  HINWEIS: Vornamenabweichung - ASV: '{vornamen}' vs. diNo: '{schueler.Vorname}'");
        }

        erfolgsProtokoll.Append(erfolgsMeldung.ToString());
        
        /*
        else
        {
          // Mehrere Schüler gefunden
          var ids = string.Join(", ", gefundeneSchueler.Select(s => s.Id));
          fehlerProtokoll.AppendLine($"FEHLER: Mehrere Schüler gefunden ({gefundeneSchueler.Count}x) - {familienname}, {vornamen} (Geb: {geburtsdatumStr}) - ASV-ID: {asvId}");
          fehlerProtokoll.AppendLine($"  IDs: {ids}");
          anzahlMehrfach++;
        }*/
      }
      catch (Exception ex)
      {
        fehlerProtokoll.AppendLine($"FEHLER bei Verarbeitung eines Schülers: {ex.Message}");
        anzahlNichtGefunden++;
      }
    }

    /// <summary>
    /// Gibt das komplette Protokoll (Zusammenfassung + Fehler + Erfolge) zurück.
    /// </summary>
    public string GetKompletteProtokoll()
    {
      StringBuilder komplett = new StringBuilder();

      komplett.AppendLine("=== ZUSAMMENFASSUNG ===");
      komplett.AppendLine($"Gesamt in XML: {anzahlGesamt}");
      komplett.AppendLine($"Erfolgreich zugeordnet: {anzahlErfolgreich}");
      komplett.AppendLine($"Nicht gefunden: {anzahlNichtGefunden}");
      //komplett.AppendLine($"Mehrfach gefunden: {anzahlMehrfach}");
      komplett.AppendLine();

      if (fehlerProtokoll.Length > 0)
      {
        komplett.AppendLine("=== FEHLERPROTOKOLL ===");
        komplett.Append(fehlerProtokoll.ToString());
        komplett.AppendLine();
      }

      if (erfolgsProtokoll.Length > 0)
      {
        komplett.AppendLine("=== ERFOLGSPROTOKOLL ===");
        komplett.Append(erfolgsProtokoll.ToString());
      }

      return komplett.ToString();
    }

    /// <summary>
    /// Gibt nur das Fehlerprotokoll zurück.
    /// </summary>
    public string GetFehlerProtokoll()
    {
      StringBuilder result = new StringBuilder();
      result.AppendLine("=== FEHLERPROTOKOLL ===");
      result.AppendLine($"Nicht gefunden: {anzahlNichtGefunden}");
      //result.AppendLine($"Mehrfach gefunden: {anzahlMehrfach}");
      result.AppendLine();
      result.Append(fehlerProtokoll.ToString());
      return result.ToString();
    }

    /// <summary>
    /// Gibt nur das Erfolgsprotokoll zurück.
    /// </summary>
    public string GetErfolgsProtokoll()
    {
      StringBuilder result = new StringBuilder();
      result.AppendLine("=== ERFOLGSPROTOKOLL ===");
      result.AppendLine($"Erfolgreich zugeordnet: {anzahlErfolgreich}");
      result.AppendLine();
      result.Append(erfolgsProtokoll.ToString());
      return result.ToString();
    }

    /// <summary>
    /// Speichert das komplette Protokoll in eine Datei.
    /// </summary>
    public void SpeichereKomplettesProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetKompletteProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Speichert nur das Fehlerprotokoll in eine Datei.
    /// </summary>
    public void SpeichereFehlerProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetFehlerProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Speichert nur das Erfolgsprotokoll in eine Datei.
    /// </summary>
    public void SpeichereErfolgsProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetErfolgsProtokoll(), Encoding.UTF8);
    }
  }
}
