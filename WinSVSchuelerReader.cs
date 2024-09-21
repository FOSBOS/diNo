using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{
  /// <summary>
  /// Liest die Schülerdatei aus der WinSV ein.
  /// </summary>
  public class WinSVSchuelerReader
  {
    # region Spaltenindex-Konstanten
    public const int schuelerIdSpalte = 2;
    public const int nachnameSpalte = 3;
    public const int vornameSpalte = 6;
    public const int rufnameSpalte = 7;
    public const int geschlechtSpalte = 8;
    public const int geburtsdatumSpalte = 10;
    public const int geburtsortSpalte = 12;
    public const int bekenntnisSpalte = 14;
    public const int nachnameEltern1Spalte = 15;
    public const int vornameEltern1Spalte = 16;
    public const int anredeEltern1Spalte = 17;
    public const int verwandtschaftsbezeichnungEltern1Spalte = 18;
    public const int nachnameEltern2Spalte = 20;
    public const int vornameEltern2Spalte = 21;
    public const int anredeEltern2Spalte = 22;
    public const int verwandtschaftsbezeichnungEltern2Spalte = 23;
    public const int anschr1PlzSpalte = 25;
    public const int anschr1OrtSpalte = 26;
    public const int anschr1StrasseSpalte = 27;
    public const int anschr1TelefonSpalte = 28;
    public const int klasseSpalte = 52;
    public const int jahrgangsstufeSpalte = 53;
    public const int ausbildungsrichtungSpalte = 58;
    public const int fremdsprache2Spalte = 60;
    public const int reliOderEthikSpalte = 63;
    public const int wahlpflichtfachSpalte = 64;
    public const int wahlfach1Spalte = 73;
    public const int wahlfach2Spalte = 74;
    public const int wahlfach3Spalte = 75;
    public const int wahlfach4Spalte = 76;
    public const int wdh1JahrgangsstufeSpalte = 86;
    public const int wdh2JahrgangsstufeSpalte = 87;
    public const int wdh1GrundSpalte = 91;
    public const int wdh2GrundSpalte = 92;
    public const int probezeitBisSpalte = 98;
    public const int eintrittDatumSpalte = 115;
    public const int eintrittJgstSpalte = 117;
    public const int eintrittVonSchulnummerSpalte = 125;
    public const int austrittsdatumSpalte = 122;
    public const int schulischeVorbildungSpalte = 128;
    public const int beruflicheVorbildungSpalte = 129;
    public const int lrsStoerungSpalte = 215;
    public const int lrsSchwaecheSpalte = 216;
    public const int lrsBisDatumSpalte = 254;
    public const int emailSpalte = 269;
    public const int notfallrufnummerSpalte = 270;

    #endregion
    /// <summary>
    /// Der log4net-Logger.
    /// </summary>
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Die Methode zum Einlesen der Daten.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ReadSchueler(string fileName)
    {
      int anzSpalten=0;
      int zeile = 0;
      int anzS = 0;
      using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("iso-8859-1")))
      using (SchuelerTableAdapter tableAdapter = new SchuelerTableAdapter())
      using (KlasseTableAdapter klasseTableAdapter = new KlasseTableAdapter())
      {
        while (!reader.EndOfStream)
        {
          string line = reader.ReadLine();
          if (string.IsNullOrEmpty(line))
          {
            log.Debug("Ignoriere Leerzeile");
            continue;
          }

          string[] array = line.Split(new string[] { "\t" }, StringSplitOptions.None);
          zeile++;
          if (anzSpalten == 0) anzSpalten = array.Length;
          else if (anzSpalten != array.Length)
          {
            if (MessageBox.Show("Ungültige Spaltenzahl in Zeile " + zeile, "diNo", MessageBoxButtons.RetryCancel)==DialogResult.Cancel) 
              return;
          }
          string[] cleanArray = array.Select(aString => aString.Trim(new char[] { '\"', ' ', '\n' })).ToArray();

          var klasse = GetKlasseId(klasseTableAdapter, cleanArray[klasseSpalte].Trim());
          if (klasse == null)
          {
            log.Debug("Ignoriere einen Schüler ohne richtige Klasse. Übergebene Klasse war " + cleanArray[klasseSpalte]);
            continue;
          }

          // wenn der Schüler noch nicht vorhanden ist
          var table = tableAdapter.GetDataById(int.Parse(cleanArray[schuelerIdSpalte]));
          diNoDataSet.SchuelerRow row = (table.Count == 0) ? table.NewSchuelerRow() : table[0];
          FillRow(cleanArray, klasse, row);
          if (table.Count == 0)
          {
            table.AddSchuelerRow(row);
            anzS++;
          }

          tableAdapter.Update(row);

          // Diese Zeile meldet den Schüler bei allen notwendigen Kursen seiner Klasse an
          new Schueler(row).WechsleKlasse(new Klasse(klasse));
        }
      }
      MessageBox.Show(zeile + " Zeilen mit " + anzS + " Schülern importiert.", "diNo", MessageBoxButtons.OK);
    }

    /// <summary>
    /// Füllt die SchuelerRow mit ihren Daten aus WinSV
    /// </summary>
    /// <param name="cleanArray">Das Array mit Daten.</param>
    /// <param name="klasse">Die Klasse in welche der Schüler gehen soll.</param>
    /// <param name="row">Die SchuelerRow.</param>
    private static void FillRow(string[] cleanArray, diNoDataSet.KlasseRow klasse, diNoDataSet.SchuelerRow row)
    {
      row.Id = int.Parse(cleanArray[schuelerIdSpalte]);
      row.Name = cleanArray[nachnameSpalte];
      row.Vorname = cleanArray[vornameSpalte];
      row.KlasseId = klasse.Id;
      row.Schulart = klasse.Bezeichnung.StartsWith("B") ? "B" : "F";
      row.Rufname = cleanArray[rufnameSpalte];
      row.Geschlecht = cleanArray[geschlechtSpalte];
      if (row.Geschlecht != "M" && row.Geschlecht != "W")
        throw new InvalidDataException("Geschlecht unbekannt");

      DateTime? geburtsdatum = ParseDate(cleanArray[geburtsdatumSpalte]);
      if (geburtsdatum == null)
      {
        row.SetGeburtsdatumNull();
      }
      else
      {
        row.Geburtsdatum = (DateTime)geburtsdatum;
      }

      row.Geburtsort = cleanArray[geburtsortSpalte];
      row.Bekenntnis = cleanArray[bekenntnisSpalte];
      row.AnschriftPLZ = cleanArray[anschr1PlzSpalte];
      row.AnschriftOrt = cleanArray[anschr1OrtSpalte];
      row.AnschriftStrasse = cleanArray[anschr1StrasseSpalte];
      row.AnschriftTelefonnummer = cleanArray[anschr1TelefonSpalte];
      row.Ausbildungsrichtung = ChangeAusbildungsrichtung(cleanArray[ausbildungsrichtungSpalte]);
      row.ReligionOderEthik = cleanArray[reliOderEthikSpalte];

      row.Wiederholung1Jahrgangsstufe = cleanArray[wdh1JahrgangsstufeSpalte];
      row.Wiederholung2Jahrgangsstufe = cleanArray[wdh2JahrgangsstufeSpalte];
      row.Wiederholung1Grund = cleanArray[wdh1GrundSpalte];
      row.Wiederholung2Grund = cleanArray[wdh2GrundSpalte];
      
      DateTime? probezeit = ParseDate(cleanArray[probezeitBisSpalte]);
      if (probezeit == null)
      {
        row.SetProbezeitBisNull();
      }
      else
      {
        row.ProbezeitBis = (DateTime)probezeit;
      }
      
      DateTime? austrittsdatum = ParseDate(cleanArray[austrittsdatumSpalte]);
      if (austrittsdatum == null)
      {
        row.SetAustrittsdatumNull();
        row.Status = 0;
      }
      else
      {
        row.Austrittsdatum = (DateTime)austrittsdatum;
        row.Status = austrittsdatum < DateTime.Now ? 1 : 0;
      }

      row.SchulischeVorbildung = cleanArray[schulischeVorbildungSpalte];
      row.BeruflicheVorbildung = cleanArray[beruflicheVorbildungSpalte];
      row.LRSStoerung = cleanArray[lrsStoerungSpalte] == "1";
      row.VerwandtschaftsbezeichnungEltern1 = cleanArray[verwandtschaftsbezeichnungEltern1Spalte];
      row.NachnameEltern1 = cleanArray[nachnameEltern1Spalte];
      row.VornameEltern1 = cleanArray[vornameEltern1Spalte];
      row.AnredeEltern1 = cleanArray[anredeEltern1Spalte];
      row.NachnameEltern2 = cleanArray[nachnameEltern2Spalte];
      row.VornameEltern2 = cleanArray[vornameEltern2Spalte];
      row.AnredeEltern2 = cleanArray[anredeEltern2Spalte];
      row.VerwandtschaftsbezeichnungEltern2 = cleanArray[verwandtschaftsbezeichnungEltern2Spalte];
      row.EintrittJahrgangsstufe = cleanArray[eintrittJgstSpalte];
      row.LRSZuschlagMin = 0;
      row.LRSZuschlagMax = 0;

      DateTime? eintrittDatum = ParseDate(cleanArray[eintrittDatumSpalte]);
      if (eintrittDatum == null)
      {
        row.SetEintrittAmNull();
      }
      else
      {
        row.EintrittAm = (DateTime)eintrittDatum;
      }

      row.EintrittAusSchulnummer = !string.IsNullOrEmpty(cleanArray[eintrittVonSchulnummerSpalte]) ? int.Parse(cleanArray[eintrittVonSchulnummerSpalte]) : -1;
      row.Email = cleanArray[emailSpalte];
      row.Notfalltelefonnummer = cleanArray[notfallrufnummerSpalte];

      row.Berechungsstatus = (int)Berechnungsstatus.Unberechnet;
      row.AndereFremdspr2Art = 0;
    }

    /// <summary>
    /// Sucht die ID der Klasse in der Datenbank. Versucht auch zu beurteilen, ob es sich überhaupt um eine echte Klasse handelt.
    /// Legt auch Klassen ggf. selbstständig in der Datenbank an.
    /// </summary>
    /// <param name="klasseTableAdapter">Der Table Adapter für Klassen.</param>
    /// <param name="klasse">Die Klassenbezeichnung.</param>
    /// <returns>Die Id der Klasse oder -1 falls die Klasse ungültig ist.</returns>
    private static diNoDataSet.KlasseRow GetKlasseId(KlasseTableAdapter klasseTableAdapter, string klasse)
    {
      var klasseDBresult = klasseTableAdapter.GetDataByBezeichnung(klasse);
      if (klasseDBresult.Count == 1)
      {
        return klasseDBresult[0];
      }
      else
      {
        // -N : Klassen für kommendes Jahr
        // AHR, FHR: Klassen des vergangenen Jahres
        // Abm: Abmeldungen
        // Ex, Import: ?
        if (klasse.EndsWith("-N") || klasse.Contains("Rest") || klasse.Contains("AHR") || klasse.Contains("FHR") || klasse.Contains("Abm") || klasse.Equals("Ex") || klasse.Equals("Import"))
        {
          return null;
        }
        else
        {
          Klasse.Insert(klasse);
          var neueKlasse = klasseTableAdapter.GetDataByBezeichnung(klasse);
          return neueKlasse[0];
        }
      }
    }

    /// <summary>
    /// Macht aus einem Datummstring ein DateTime oder null wenn das Datum leer ist.
    /// </summary>
    /// <param name="date">Der Datumsstring.</param>
    /// <returns>Ein DateTime oder null.</returns>
    private static DateTime? ParseDate(string date)
    {
      if (string.IsNullOrEmpty(date))
        return null;

      return DateTime.Parse(date, CultureInfo.CurrentCulture);
    }

    /// <summary>
    ///  Hauptzweck der Methode: W statt WVR im Wirtschaftszweig verwenden.
    /// </summary>
    /// <param name="ausbildungsrichtung">Die Ausbildungsrichtung aus der Schüler SV. z. B. WVR für Wirtschaft.</param>
    /// <returns>Ein-Buchstabige Ausbildungsrichtung, z. B. W für Wirtschaft.</returns>
    private static string ChangeAusbildungsrichtung(string ausbildungsrichtung)
    {
      switch (ausbildungsrichtung)
      {
        case "S": return "S";
        case "T": return "T";
        case "WVR": return "W";
        case "W": return "W"; // manchmal steht W auch schon drin
        case "V": return "V"; // Vorklasse FOS hat noch keine Ausbildungsrichtung
        case "": return "V"; // Integrationsvorklasse hat auch noch keine Richtung
        case "A": return "U"; // Agrar- Bio und Umwelttechnologie.
        case "U": return "U"; // Agrar- Bio und Umwelttechnologie.
        default:
          throw new InvalidOperationException("Unbekannte Ausbildungsrichtung " + ausbildungsrichtung);
      }
    }
  }
}
