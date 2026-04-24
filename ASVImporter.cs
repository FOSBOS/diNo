using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace diNo
{
  /// <summary>
  /// Importiert ASV-Daten aus einer XML-Datei und ordnet jedem Schüler die ASV-ID zu.
  /// Matching erfolgt über: Familienname, Vornamen und Geburtsdatum
  /// </summary>
  public class ASVImporter
  {
    private List<string> fehlerProtokoll;
    private List<string> erfolgsProtokoll;
    private int anzahlGesamt;
    private int anzahlErfolgreich;
    private int anzahlNichtGefunden;
    private int anzahlMehrfach;

    public ASVImporter()
    {
      fehlerProtokoll = new List<string>();
      erfolgsProtokoll = new List<string>();
      anzahlGesamt = 0;
      anzahlErfolgreich = 0;
      anzahlNichtGefunden = 0;
      anzahlMehrfach = 0;
    }

    /// <summary>
    /// Importiert die ASV-Daten aus der XML-Datei und ordnet jedem Schüler die ASV-ID zu
    /// </summary>
    /// <param name="xmlDateiPfad">Pfad zur ASV-XML-Datei</param>
    /// <returns>Anzahl der erfolgreich zugeordneten Schüler</returns>
    public int ImportiereASVDaten(string xmlDateiPfad)
    {
      // Protokolle zurücksetzen
      fehlerProtokoll.Clear();
      erfolgsProtokoll.Clear();
      anzahlGesamt = 0;
      anzahlErfolgreich = 0;
      anzahlNichtGefunden = 0;
      anzahlMehrfach = 0;

      try
      {
        // XML-Datei laden
        XDocument doc = XDocument.Load(xmlDateiPfad);
        XNamespace ns = "http://www.asv.bayern.de/import";

        // Alle Schülerinnen und Schüler aus der XML-Datei extrahieren
        var schuelerElemente = doc.Descendants(ns + "schuelerin");
        anzahlGesamt = schuelerElemente.Count();

        foreach (var schuelerElement in schuelerElemente)
        {
          VerarbeiteSchueler(schuelerElement, ns);
        }
      }
      catch (Exception ex)
      {
        fehlerProtokoll.Add($"KRITISCHER FEHLER beim Laden der XML-Datei: {ex.Message}");
      }

      return anzahlErfolgreich;
    }

    /// <summary>
    /// Verarbeitet einen einzelnen Schüler aus der XML-Datei
    /// </summary>
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
        if (string.IsNullOrEmpty(asvId))
        {
          fehlerProtokoll.Add($"FEHLER: Schüler ohne ASV-ID gefunden (Name: {familienname} {vornamen})");
          return;
        }

        if (string.IsNullOrEmpty(familienname) || string.IsNullOrEmpty(vornamen))
        {
          fehlerProtokoll.Add($"FEHLER: Schüler mit unvollständigen Daten - ASV-ID: {asvId}");
          return;
        }

        // Geburtsdatum parsen (Format: dd.MM.yyyy, z.B. 21.09.2009)
        DateTime geburtsdatum;
        if (!DateTime.TryParseExact(geburtsdatumStr, "dd.MM.yyyy",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out geburtsdatum))
        {
          fehlerProtokoll.Add($"FEHLER: Ungültiges Geburtsdatum '{geburtsdatumStr}' - {familienname} {vornamen} - ASV-ID: {asvId}");
          return;
        }

        // Schüler im Repository suchen
        var gefundeneSchueler = FindeSchueler(familienname, vornamen, geburtsdatum);

        if (gefundeneSchueler.Count == 0)
        {
          // Kein Schüler gefunden
          anzahlNichtGefunden++;
          fehlerProtokoll.Add($"FEHLER: Schüler nicht gefunden - {familienname} {vornamen} (Geb: {geburtsdatum:dd.MM.yyyy}) - ASV-ID: {asvId}");
        }
        else if (gefundeneSchueler.Count == 1)
        {
          // Genau ein Schüler gefunden - ASV-ID zuweisen
          Schueler schueler = gefundeneSchueler[0];
          schueler.Data.asv_id = asvId;
          schueler.Save();
          anzahlErfolgreich++;
          erfolgsProtokoll.Add($"ERFOLG: {familienname} {vornamen} (Geb: {geburtsdatum:dd.MM.yyyy}) - ID: {schueler.Id} - ASV-ID: {asvId}");
        }
        else
        {
          // Mehrere Schüler gefunden
          anzahlMehrfach++;
          string ids = string.Join(", ", gefundeneSchueler.Select(s => s.Id));
          fehlerProtokoll.Add($"FEHLER: Mehrere Schüler gefunden ({gefundeneSchueler.Count}x) - {familienname} {vornamen} (Geb: {geburtsdatum:dd.MM.yyyy}) - ASV-ID: {asvId}");
          fehlerProtokoll.Add($"  IDs: {ids}");
        }
      }
      catch (Exception ex)
      {
        fehlerProtokoll.Add($"FEHLER bei der Verarbeitung eines Schülers: {ex.Message}");
      }
    }

    /// <summary>
    /// Sucht Schüler im Repository anhand von Familienname, Vornamen und Geburtsdatum
    /// </summary>
    private List<Schueler> FindeSchueler(string familienname, string vornamen, DateTime geburtsdatum)
    {
      var gefundeneSchueler = new List<Schueler>();

      // Alle Schüler aus dem Repository durchsuchen
      var alleSchueler = Zugriff.Instance.SchuelerRep.getList();

      foreach (var schueler in alleSchueler)
      {
        // Vergleich: Familienname, Vornamen und Geburtsdatum (nur Datum, ohne Uhrzeit)
        bool namePasst = string.Equals(schueler.Name, familienname, StringComparison.OrdinalIgnoreCase);
        bool vornamePasst = string.Equals(schueler.Vorname, vornamen, StringComparison.OrdinalIgnoreCase);
        bool geburtsdatumPasst = schueler.Data.Geburtsdatum.Date == geburtsdatum.Date;

        if (namePasst && vornamePasst && geburtsdatumPasst)
        {
          gefundeneSchueler.Add(schueler);
        }
      }

      return gefundeneSchueler;
    }

    /// <summary>
    /// Gibt das komplette Protokoll (Zusammenfassung + Fehler + Erfolge) zurück
    /// </summary>
    public string GetKompletteProtokoll()
    {
      StringBuilder sb = new StringBuilder();

      // Zusammenfassung
      sb.AppendLine("=== ZUSAMMENFASSUNG ===");
      sb.AppendLine($"Gesamt in XML: {anzahlGesamt}");
      sb.AppendLine($"Erfolgreich zugeordnet: {anzahlErfolgreich}");
      sb.AppendLine($"Nicht gefunden: {anzahlNichtGefunden}");
      sb.AppendLine($"Mehrfach gefunden: {anzahlMehrfach}");
      sb.AppendLine();

      // Fehlerprotokoll
      if (fehlerProtokoll.Count > 0)
      {
        sb.AppendLine("=== FEHLERPROTOKOLL ===");
        foreach (var fehler in fehlerProtokoll)
        {
          sb.AppendLine(fehler);
        }
        sb.AppendLine();
      }

      // Erfolgsprotokoll
      if (erfolgsProtokoll.Count > 0)
      {
        sb.AppendLine("=== ERFOLGSPROTOKOLL ===");
        foreach (var erfolg in erfolgsProtokoll)
        {
          sb.AppendLine(erfolg);
        }
      }

      return sb.ToString();
    }

    /// <summary>
    /// Gibt nur das Fehlerprotokoll zurück
    /// </summary>
    public string GetFehlerProtokoll()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("=== ZUSAMMENFASSUNG ===");
      sb.AppendLine($"Gesamt in XML: {anzahlGesamt}");
      sb.AppendLine($"Erfolgreich zugeordnet: {anzahlErfolgreich}");
      sb.AppendLine($"Nicht gefunden: {anzahlNichtGefunden}");
      sb.AppendLine($"Mehrfach gefunden: {anzahlMehrfach}");
      sb.AppendLine();

      if (fehlerProtokoll.Count > 0)
      {
        sb.AppendLine("=== FEHLER ===");
        foreach (var fehler in fehlerProtokoll)
        {
          sb.AppendLine(fehler);
        }
      }
      else
      {
        sb.AppendLine("Keine Fehler aufgetreten.");
      }

      return sb.ToString();
    }

    /// <summary>
    /// Gibt nur das Erfolgsprotokoll zurück
    /// </summary>
    public string GetErfolgsProtokoll()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"=== ERFOLGREICH ZUGEORDNET ({anzahlErfolgreich}) ===");

      if (erfolgsProtokoll.Count > 0)
      {
        foreach (var erfolg in erfolgsProtokoll)
        {
          sb.AppendLine(erfolg);
        }
      }
      else
      {
        sb.AppendLine("Keine erfolgreichen Zuordnungen.");
      }

      return sb.ToString();
    }

    /// <summary>
    /// Speichert das komplette Protokoll in eine Datei
    /// </summary>
    public void SpeichereKomplettesProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetKompletteProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Speichert nur das Fehlerprotokoll in eine Datei
    /// </summary>
    public void SpeichereFehlerProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetFehlerProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Speichert nur das Erfolgsprotokoll in eine Datei
    /// </summary>
    public void SpeichereErfolgsProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetErfolgsProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Gibt die Anzahl der erfolgreich zugeordneten Schüler zurück
    /// </summary>
    public int AnzahlErfolgreich => anzahlErfolgreich;

    /// <summary>
    /// Gibt die Anzahl der nicht gefundenen Schüler zurück
    /// </summary>
    public int AnzahlNichtGefunden => anzahlNichtGefunden;

    /// <summary>
    /// Gibt die Anzahl der mehrfach gefundenen Schüler zurück
    /// </summary>
    public int AnzahlMehrfach => anzahlMehrfach;

    /// <summary>
    /// Gibt die Gesamtanzahl der Schüler in der XML-Datei zurück
    /// </summary>
    public int AnzahlGesamt => anzahlGesamt;
  }
}
