 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace diNo
{
  /// <summary>
  /// Importiert ASV-Exportdaten und ordnet Schülern ihre ASV-IDs zu
  /// </summary>
  public class ASVImporter
  {
    private List<string> fehlerProtokoll;
    private List<string> erfolgsProtokoll;
    private int anzahlGesamt;
    private int anzahlErfolgreich;
    private int anzahlFehler;

    public ASVImporter()
    {
      fehlerProtokoll = new List<string>();
      erfolgsProtokoll = new List<string>();
      anzahlGesamt = 0;
      anzahlErfolgreich = 0;
      anzahlFehler = 0;
    }

    /// <summary>
    /// Importiert die ASV-XML-Datei und ordnet jedem Schüler die ASV-ID zu
    /// </summary>
    /// <param name="xmlDateiPfad">Pfad zur ASV-Export-XML-Datei</param>
    /// <returns>Anzahl der erfolgreich zugeordneten Schüler</returns>
    public int ImportiereASVDaten(string xmlDateiPfad)
    {
      fehlerProtokoll.Clear();
      erfolgsProtokoll.Clear();
      anzahlGesamt = 0;
      anzahlErfolgreich = 0;
      anzahlFehler = 0;

      try
      {
        // XML-Datei laden
        XDocument doc = XDocument.Load(xmlDateiPfad);
        XNamespace ns = "http://www.asv.bayern.de/import";

        // Alle Schüler aus der XML-Datei extrahieren
        var schuelerElemente = doc.Descendants(ns + "schuelerin").ToList();
        anzahlGesamt = schuelerElemente.Count;

        foreach (var schuelerElement in schuelerElemente)
        {
          VerarbeiteSchueler(schuelerElement, ns);
        }

        // Zusammenfassung erstellen
        ErstelleZusammenfassung();

        return anzahlErfolgreich;
      }
      catch (Exception ex)
      {
        fehlerProtokoll.Insert(0, $"KRITISCHER FEHLER beim Laden der XML-Datei: {ex.Message}");
        return 0;
      }
    }

    /// <summary>
    /// Verarbeitet einen einzelnen Schüler aus der XML
    /// </summary>
    private void VerarbeiteSchueler(XElement schuelerElement, XNamespace ns)
    {
      try
      {
        // ASV-Daten extrahieren
        string asvId = schuelerElement.Element(ns + "lokales_differenzierungsmerkmal")?.Value?.Trim();
        string vorname = schuelerElement.Element(ns + "rufname")?.Value?.Trim();
        string nachname = schuelerElement.Element(ns + "familienname")?.Value?.Trim();

        // Klassenbezeichnung extrahieren (kann an verschiedenen Stellen stehen)
        string klassenbezeichnung = ExtrahiereKlassenbezeichnung(schuelerElement, ns);

        // Validierung
        if (string.IsNullOrEmpty(asvId))
        {
          fehlerProtokoll.Add($"FEHLER: Schüler ohne ASV-ID gefunden (Name: {vorname} {nachname})");
          anzahlFehler++;
          return;
        }

        if (string.IsNullOrEmpty(vorname) || string.IsNullOrEmpty(nachname))
        {
          fehlerProtokoll.Add($"FEHLER: Schüler mit unvollständigen Daten - ASV-ID: {asvId}");
          anzahlFehler++;
          return;
        }

        // Schüler im Repository suchen
        Schueler gefundenerSchueler = FindeSchueler(vorname, nachname, klassenbezeichnung);

        if (gefundenerSchueler != null)
        {
          // ASV-ID zuweisen
          gefundenerSchueler.AsvId = asvId;
          anzahlErfolgreich++;
          erfolgsProtokoll.Add($"OK: {vorname} {nachname} (Klasse: {klassenbezeichnung}) → ASV-ID: {asvId}");
        }
        else
        {
          fehlerProtokoll.Add($"FEHLER: Schüler nicht gefunden - {vorname} {nachname} (Klasse: {klassenbezeichnung}) - ASV-ID: {asvId}");
          anzahlFehler++;
        }
      }
      catch (Exception ex)
      {
        fehlerProtokoll.Add($"FEHLER beim Verarbeiten eines Schülers: {ex.Message}");
        anzahlFehler++;
      }
    }

    /// <summary>
    /// Extrahiert die Klassenbezeichnung aus verschiedenen möglichen XML-Strukturen
    /// </summary>
    private string ExtrahiereKlassenbezeichnung(XElement schuelerElement, XNamespace ns)
    {
      // Versuche verschiedene Pfade zur Klassenbezeichnung

      // 1. Direkt unter schuelerin
      var klasse = schuelerElement.Element(ns + "klasse");
      if (klasse != null)
      {
        var bezeichnung = klasse.Element(ns + "klassenbezeichnung")?.Value?.Trim();
        if (!string.IsNullOrEmpty(bezeichnung))
          return bezeichnung;
      }

      // 2. Über klassengruppe
      var klassengruppe = schuelerElement.Element(ns + "klassengruppe");
      if (klassengruppe != null)
      {
        klasse = klassengruppe.Element(ns + "klasse");
        if (klasse != null)
        {
          var bezeichnung = klasse.Element(ns + "klassenbezeichnung")?.Value?.Trim();
          if (!string.IsNullOrEmpty(bezeichnung))
            return bezeichnung;
        }
      }

      // 3. Direkt klassenbezeichnung
      var direktBezeichnung = schuelerElement.Element(ns + "klassenbezeichnung")?.Value?.Trim();
      if (!string.IsNullOrEmpty(direktBezeichnung))
        return direktBezeichnung;

      return string.Empty;
    }

    /// <summary>
    /// Findet einen Schüler im Repository anhand von Name und Klasse
    /// </summary>
    private Schueler FindeSchueler(string vorname, string nachname, string klassenbezeichnung)
    {
      var alleSchueler = Zugriff.Instance.SchuelerRep.getList();

      // Normalisierung für Vergleich
      string vornameNorm = vorname.ToLower().Trim();
      string nachnameNorm = nachname.ToLower().Trim();
      string klasseNorm = klassenbezeichnung?.ToLower().Trim() ?? "";

      // 1. Versuch: Exakte Übereinstimmung mit Vorname, Nachname UND Klasse
      if (!string.IsNullOrEmpty(klasseNorm))
      {
        var exakterTreffer = alleSchueler.FirstOrDefault(s =>
       s.Vorname?.ToLower().Trim() == vornameNorm &&
          s.Name?.ToLower().Trim() == nachnameNorm &&
          s.getKlasse?.Bezeichnung?.ToLower().Trim() == klasseNorm
        );

        if (exakterTreffer != null)
          return exakterTreffer;
      }

      // 2. Versuch: Nur Vorname und Nachname (wenn genau ein Treffer)
      var nameTreffer = alleSchueler.Where(s =>
        s.Vorname?.ToLower().Trim() == vornameNorm &&
      s.Name?.ToLower().Trim() == nachnameNorm
      ).ToList();

      if (nameTreffer.Count == 1)
      {
        var schueler = nameTreffer[0];
        string schuelerKlasse = schueler.getKlasse?.Bezeichnung ?? "unbekannt";

        if (!string.IsNullOrEmpty(klasseNorm) && schuelerKlasse.ToLower().Trim() != klasseNorm)
        {
          fehlerProtokoll.Add($"WARNUNG: Schüler {vorname} {nachname} nur über Name gefunden (Klasse unterschiedlich: ASV={klassenbezeichnung}, diNo={schuelerKlasse})");
        }

        return schueler;
      }
      else if (nameTreffer.Count > 1)
      {
        fehlerProtokoll.Add($"WARNUNG: Mehrere Schüler mit Namen {vorname} {nachname} gefunden - keine eindeutige Zuordnung möglich");
        return null;
      }

      // Kein Treffer gefunden
      return null;
    }

    /// <summary>
    /// Erstellt eine Zusammenfassung am Anfang des Fehlerprotokolls
    /// </summary>
    private void ErstelleZusammenfassung()
    {
      var zusammenfassung = new List<string>
      {
        "=== ZUSAMMENFASSUNG ASV-IMPORT ===",
  $"Gesamt in XML: {anzahlGesamt}",
     $"Erfolgreich zugeordnet: {anzahlErfolgreich}",
   $"Fehler: {anzahlFehler}",
        ""
      };

      fehlerProtokoll.InsertRange(0, zusammenfassung);
    }

    /// <summary>
    /// Gibt das komplette Protokoll (Erfolge und Fehler) zurück
    /// </summary>
    public string GetKompletteProtokoll()
    {
      var sb = new StringBuilder();

      sb.AppendLine("=== FEHLER UND WARNUNGEN ===");
      foreach (var zeile in fehlerProtokoll)
      {
        sb.AppendLine(zeile);
      }

      sb.AppendLine();
      sb.AppendLine("=== ERFOLGREICHE ZUORDNUNGEN ===");
      foreach (var zeile in erfolgsProtokoll)
      {
        sb.AppendLine(zeile);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Gibt nur das Fehlerprotokoll zurück
    /// </summary>
    public string GetFehlerProtokoll()
    {
      return string.Join(Environment.NewLine, fehlerProtokoll);
    }

    /// <summary>
    /// Gibt nur das Erfolgsprotokoll zurück
    /// </summary>
    public string GetErfolgsProtokoll()
    {
      return string.Join(Environment.NewLine, erfolgsProtokoll);
    }

    /// <summary>
    /// Speichert das Fehlerprotokoll in eine Datei
    /// </summary>
    public void SpeichereFehlerProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetFehlerProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Speichert das Erfolgsprotokoll in eine Datei
    /// </summary>
    public void SpeichereErfolgsProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetErfolgsProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Speichert das komplette Protokoll in eine Datei
    /// </summary>
    public void SpeichereKomplettesProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetKompletteProtokoll(), Encoding.UTF8);
    }

    /// <summary>
    /// Gibt Statistiken zum Import zurück
    /// </summary>
    public (int gesamt, int erfolgreich, int fehler) GetStatistik()
    {
      return (anzahlGesamt, anzahlErfolgreich, anzahlFehler);
    }
  }
}
