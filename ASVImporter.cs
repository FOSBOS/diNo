using diNo.diNoDataSetTableAdapters;
using log4net;
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
  /// Importiert Schülerstammdaten aus einer ASV-Export-XML-Datei (bayerisches Schulverwaltungssystem).
  /// Ersetzt den früheren WinSVSchuelerReader: die Schülertabelle wird zu Schuljahresbeginn geleert,
  /// deshalb wird hier jeder Schüler als neuer Datensatz angelegt (keine Suche nach vorhandenen Schülern
  /// über Name/Geburtsdatum mehr nötig). Als externer Schlüssel dient das lokale Differenzierungsmerkmal (asv_id).
  /// schullaufbahn und besuchte_faecher werden bewusst nicht importiert.
  /// </summary>
  public class ASVImporter
  {
    private static readonly XNamespace ns = "http://www.asv.bayern.de/import";
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private StringBuilder fehlerProtokoll = new StringBuilder();
    private StringBuilder erfolgsProtokoll = new StringBuilder();
    private int anzahlGesamt;
    private int anzahlErfolgreich;
    private int nextId; // Schueler.Id ist kein Autoincrement, daher wird die nächste freie Id selbst ermittelt

    /// <summary>
    /// Importiert die ASV-Daten aus der angegebenen XML-Datei. Legt für jeden gefundenen Schüler einen neuen
    /// diNo-Datensatz an (inkl. Anschriften und ggf. einer Wiederholung) und weist ihn seiner Klasse zu.
    /// </summary>
    /// <returns>Anzahl der erfolgreich importierten Schüler</returns>
    public int ImportiereASVDaten(string xmlDateiPfad)
    {
      try
      {
        XDocument doc = XDocument.Load(xmlDateiPfad);
        nextId = (new SchuelerTableAdapter().GetMaxId() ?? 0) + 1;

        foreach (var schuleElement in doc.Descendants(ns + "schule"))
        {
          string schuljahr = El(schuleElement, "schuljahr");
          var klassenElement = schuleElement.Element(ns + "klassen");
          if (klassenElement == null) continue;

          foreach (var klasseElement in klassenElement.Elements(ns + "klasse"))
          {
            string klassenname = El(klasseElement, "klassenname");
            var klassengruppenElement = klasseElement.Element(ns + "klassengruppen");
            if (klassengruppenElement == null) continue;

            foreach (var gruppeElement in klassengruppenElement.Elements(ns + "klassengruppe"))
            {
              string kennung = El(gruppeElement, "kennung");
              string jahrgangsstufe = El(gruppeElement, "jahrgangsstufe");
              var schuelerlisteElement = gruppeElement.Element(ns + "schuelerliste");
              if (schuelerlisteElement == null) continue;

              foreach (var schuelerElement in schuelerlisteElement.Elements(ns + "schuelerin"))
              {
                anzahlGesamt++;
                VerarbeiteSchueler(schuelerElement, klassenname, kennung, jahrgangsstufe, schuljahr);
              }
            }
          }
        }

        return anzahlErfolgreich;
      }
      catch (Exception ex)
      {
        fehlerProtokoll.AppendLine($"KRITISCHER FEHLER beim Laden der XML-Datei: {ex.Message}");
        return anzahlErfolgreich;
      }
    }

    private static string El(XElement parent, string name)
    {
      return parent?.Element(ns + name)?.Value?.Trim();
    }

    private void VerarbeiteSchueler(XElement schuelerElement, string klassenname, string kennung, string jahrgangsstufe, string schuljahr)
    {
      string asvId = El(schuelerElement, "lokales_differenzierungsmerkmal");
      string familienname = El(schuelerElement, "familienname");
      string vornamen = El(schuelerElement, "vornamen");

      try
      {
        if (string.IsNullOrEmpty(asvId) || string.IsNullOrEmpty(familienname) || string.IsNullOrEmpty(vornamen))
        {
          fehlerProtokoll.AppendLine($"FEHLER: Unvollständige Stammdaten (ASV-ID: {asvId ?? "leer"}, Name: {familienname} {vornamen})");
          return;
        }

        var klasse = ErmittleKlasse(klassenname);
        if (klasse == null)
        {
          fehlerProtokoll.AppendLine($"FEHLER: Klasse '{klassenname}' konnte nicht ermittelt/angelegt werden - {familienname}, {vornamen} (ASV-ID: {asvId}) übersprungen.");
          return;
        }

        var dt = new diNoDataSet.SchuelerDataTable();
        diNoDataSet.SchuelerRow row = dt.NewSchuelerRow();
        row.Id = nextId;
        FuelleRow(row, schuelerElement, klasse, asvId, kennung, klassenname);
        dt.AddSchuelerRow(row);
        new SchuelerTableAdapter().Update(row);
        nextId++;

        var schueler = new Schueler(row);
        ImportiereAnschriften(schueler, schuelerElement);
        ImportiereWiederholung(schueler, schuelerElement, jahrgangsstufe, schuljahr);

        schueler.WechsleKlasse(new Klasse(klasse.Id));

        anzahlErfolgreich++;
        erfolgsProtokoll.AppendLine($"ERFOLG: {familienname}, {vornamen} - ID: {row.Id} - ASV-ID: {asvId}");
      }
      catch (Exception ex)
      {
        fehlerProtokoll.AppendLine($"FEHLER bei Schüler {familienname}, {vornamen} (ASV-ID: {asvId}): {ex.Message}");
        log.Error("Fehler beim ASV-Import eines Schülers", ex);
      }
    }

    private void FuelleRow(diNoDataSet.SchuelerRow row, XElement schuelerElement, diNoDataSet.KlasseRow klasse, string asvId, string kennung, string klassenname)
    {
      row.Name = El(schuelerElement, "familienname");
      row.Vorname = El(schuelerElement, "vornamen");
      row.KlasseId = klasse.Id;
      row.Schulart = klasse.Bezeichnung.StartsWith("B") ? "B" : "F";

      string rufname = El(schuelerElement, "rufname");
      row.Rufname = string.IsNullOrEmpty(rufname) ? row.Vorname : rufname;

      row.Geschlecht = DecodeGeschlecht(El(schuelerElement, "geschlecht"));

      DateTime? geburtsdatum = ParseAsvDatum(El(schuelerElement, "geburtsdatum"));
      if (geburtsdatum == null) row.SetGeburtsdatumNull();
      else row.Geburtsdatum = geburtsdatum.Value;

      row.Geburtsort = El(schuelerElement, "geburtsort") ?? "";
      row.Ausbildungsrichtung = ErmittleAusbildungsrichtung(kennung, klassenname);

      DateTime? eintritt = ParseAsvDatum(El(schuelerElement, "eintrittsdatum"));
      if (eintritt == null) row.SetEintrittAmNull();
      else row.EintrittAm = eintritt.Value;

      row.Status = 0; // aktiv
      row.LRSStoerung = false; // wird erst nach Ausstellung des Bescheids gesetzt, s. altes WinSVSchuelerReader-Verhalten
      row.LRSZuschlagMin = 0;
      row.LRSZuschlagMax = 0;
      row.Berechungsstatus = (int)Berechnungsstatus.Unberechnet;
      row.AndereFremdspr2Art = 0;
      row.asv_id = asvId;
    }

    private void ImportiereAnschriften(Schueler schueler, XElement schuelerElement)
    {
      var listeElement = schuelerElement.Element(ns + "schueleranschriften");
      if (listeElement == null) return;

      foreach (var a in listeElement.Elements(ns + "schueleranschrift"))
      {
        string wessen = El(a, "anschrift_wessen");
        if (string.IsNullOrEmpty(wessen)) continue;

        string anschriftstyp = El(a, "anschriftstyp");

        string nachname = null, vorname = null, anrede = null, verwandtschaft = null;
        var personElement = a.Element(ns + "person");
        if (personElement != null)
        {
          nachname = El(personElement, "familienname");
          vorname = El(personElement, "vornamen");
          anrede = DecodeAnrede(El(personElement, "anrede"));
          verwandtschaft = DecodePersonentyp(El(personElement, "personentyp"));
        }

        string strasse = null, hausnummer = null, plz = null, ort = null;
        var anschriftElement = a.Element(ns + "anschrift");
        if (anschriftElement != null)
        {
          strasse = El(anschriftElement, "strasse");
          hausnummer = El(anschriftElement, "nummer");
          plz = El(anschriftElement, "postleitzahl");
          ort = El(anschriftElement, "ortsbezeichnung");
        }

        string telefon = null, mobil = null, email = null;
        var kommunikationElement = a.Element(ns + "kommunikationsdaten");
        if (kommunikationElement != null)
        {
          foreach (var k in kommunikationElement.Elements(ns + "kommunikation"))
          {
            string wert = El(k, "nummer_adresse");
            if (string.IsNullOrEmpty(wert)) continue;
            switch (KommunikationsKategorie(El(k, "typ")))
            {
              case "tel": telefon = wert; break;
              case "mobil": mobil = wert; break;
              case "email": email = wert; break;
            }
          }
        }

        bool? hauptAnsprechpartner = ParseBool(El(a, "hauptansprechpartner"));
        bool? auskunftsberechtigt = ParseBool(El(a, "auskunftsberechtigt"));

        schueler.AddAnschrift(wessen, anschriftstyp, nachname, vorname, anrede, verwandtschaft,
          strasse, hausnummer, plz, ort, telefon, mobil, email, hauptAnsprechpartner, auskunftsberechtigt);
      }
    }

    private void ImportiereWiederholung(Schueler schueler, XElement schuelerElement, string jahrgangsstufe, string schuljahr)
    {
      // "Grund" wird als ASV-Rohcode übernommen (Werteliste Wiederholungsart_2140 ist schulartabhängig kodiert)
      string wiederholungsart = El(schuelerElement, "wiederholungsart");
      if (string.IsNullOrEmpty(wiederholungsart)) return;

      schueler.AddWiederholung(schuljahr, jahrgangsstufe, wiederholungsart);
    }

    private diNoDataSet.KlasseRow ErmittleKlasse(string klassenname)
    {
      if (string.IsNullOrEmpty(klassenname)) return null;

      var ta = new KlasseTableAdapter();
      var result = ta.GetDataByBezeichnung(klassenname);
      if (result.Count == 1) return result[0];

      Klasse.Insert(klassenname);
      var neu = ta.GetDataByBezeichnung(klassenname);
      return neu.Count == 1 ? neu[0] : null;
    }

    /// <summary>
    /// Ermittelt den Zweig (S/T/W/U) zunächst aus der Kennung der Klassengruppe (bei gemischten Klassen,
    /// z. B. F13TU, steht dort der Zweig der jeweiligen Untergruppe), sonst aus dem Klassennamen.
    /// </summary>
    private static string ErmittleAusbildungsrichtung(string kennung, string klassenname)
    {
      string ausKennung = ExtrahiereZweig(kennung);
      if (!string.IsNullOrEmpty(ausKennung)) return ausKennung;

      string ausName = ExtrahiereZweig(klassenname);
      return string.IsNullOrEmpty(ausName) ? "V" : ausName; // "V" = (Integrations-)Vorklasse ohne eigenen Zweig
    }

    private static string ExtrahiereZweig(string s)
    {
      if (string.IsNullOrEmpty(s)) return "";
      foreach (char c in "STUW")
        if (s.IndexOf(c) >= 0) return c.ToString();
      return "";
    }

    private static string DecodeGeschlecht(string code)
    {
      switch (code)
      {
        case "1": return "M";
        case "2": return "W";
        case "3": return "D";
        default: throw new InvalidOperationException("Unbekannter/uneindeutiger ASV-Geschlecht-Code: " + code);
      }
    }

    // Werteliste Anrede_2008: 1=Frau, 2=Herr, 3=keine
    private static string DecodeAnrede(string code)
    {
      switch (code)
      {
        case "1": return "F";
        case "2": return "H";
        default: return "";
      }
    }

    // Werteliste Personentyp_2091 (unbekannte/seltene Codes werden als Rohcode übernommen)
    private static string DecodePersonentyp(string code)
    {
      switch (code)
      {
        case "002": return "Vater";
        case "003": return "Mutter";
        case "004": return "Vormund";
        case "005": return "Verwandter";
        case "006": return "Pflegeeltern";
        case "014": return "Rechtliche Betreuung";
        case "015": return "Schulbegleiter";
        case "016": return "Ehepartner";
        default: return code;
      }
    }

    // Werteliste Kommunikationsanschluss_2009: Telefon (01/06/07), Mobil (02/08/09), E-Mail (04/10/11); Fax/Homepage werden ignoriert
    private static string KommunikationsKategorie(string typCode)
    {
      switch (typCode)
      {
        case "01":
        case "06":
        case "07":
          return "tel";
        case "02":
        case "08":
        case "09":
          return "mobil";
        case "04":
        case "10":
        case "11":
          return "email";
        default:
          return null;
      }
    }

    private static bool? ParseBool(string s)
    {
      bool b;
      return !string.IsNullOrEmpty(s) && bool.TryParse(s, out b) ? (bool?)b : null;
    }

    private static DateTime? ParseAsvDatum(string datum)
    {
      DateTime result;
      if (!string.IsNullOrEmpty(datum) && DateTime.TryParseExact(datum, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        return result;
      return null;
    }

    /// <summary>
    /// Gibt das komplette Protokoll (Zusammenfassung + Fehler + Erfolge) zurück.
    /// </summary>
    public string GetKompletteProtokoll()
    {
      StringBuilder komplett = new StringBuilder();
      komplett.AppendLine("=== ZUSAMMENFASSUNG ===");
      komplett.AppendLine($"Gesamt in XML: {anzahlGesamt}");
      komplett.AppendLine($"Erfolgreich importiert: {anzahlErfolgreich}");
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

    public string GetFehlerProtokoll()
    {
      StringBuilder result = new StringBuilder();
      result.AppendLine("=== FEHLERPROTOKOLL ===");
      result.AppendLine();
      result.Append(fehlerProtokoll.ToString());
      return result.ToString();
    }

    public string GetErfolgsProtokoll()
    {
      StringBuilder result = new StringBuilder();
      result.AppendLine("=== ERFOLGSPROTOKOLL ===");
      result.AppendLine($"Erfolgreich importiert: {anzahlErfolgreich}");
      result.AppendLine();
      result.Append(erfolgsProtokoll.ToString());
      return result.ToString();
    }

    public void SpeichereKomplettesProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetKompletteProtokoll(), Encoding.UTF8);
    }

    public void SpeichereFehlerProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetFehlerProtokoll(), Encoding.UTF8);
    }

    public void SpeichereErfolgsProtokoll(string dateiPfad)
    {
      File.WriteAllText(dateiPfad, GetErfolgsProtokoll(), Encoding.UTF8);
    }
  }
}
