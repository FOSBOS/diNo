using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace diNo
{
  /// <summary>
  /// Liest die Schülerdatei aus der WinSV ein.
  /// </summary>
  public class WinSVSchuelerReader
  {
    # region Spaltenindex-Konstanten
    private const int schuelerIdSpalte = 2;
    private const int nachnameSpalte = 3;
    private const int vornameSpalte = 6;
    private const int rufnameSpalte = 7;
    private const int geschlechtSpalte = 8;
    private const int geburtsdatumSpalte = 10;
    private const int geburtsortSpalte = 12;
    private const int bekenntnisSpalte = 14;
    private const int nachnameEltern1Spalte = 15;
    private const int vornameEltern1Spalte = 16;
    private const int anredeEltern1Spalte = 17;
    private const int verwandtschaftsbezeichnungEltern1Spalte = 18;
    private const int nachnameEltern2Spalte = 20;
    private const int vornameEltern2Spalte = 21;
    private const int anredeEltern2Spalte = 22;
    private const int verwandtschaftsbezeichnungEltern2Spalte = 23;
    private const int anschr1PlzSpalte = 25;
    private const int anschr1OrtSpalte = 26;
    private const int anschr1StrasseSpalte = 27;
    private const int anschr1TelefonSpalte = 28;
    private const int klasseSpalte = 52;
    private const int jahrgangsstufeSpalte = 53;
    private const int ausbildungsrichtungSpalte = 58;
    private const int fremdsprache2Spalte = 60;
    private const int reliOderEthikSpalte = 63;
    private const int wahlpflichtfachSpalte = 64;
    private const int wahlfach1Spalte = 73;
    private const int wahlfach2Spalte = 74;
    private const int wahlfach3Spalte = 75;
    private const int wahlfach4Spalte = 76;
    private const int wdh1JahrgangsstufeSpalte = 86;
    private const int wdh2JahrgangsstufeSpalte = 87;
    private const int wdh1GrundSpalte = 91;
    private const int wdh2GrundSpalte = 92;
    private const int probezeitBisSpalte = 98;
    private const int eintrittDatumSpalte = 115;
    private const int eintrittJgstSpalte = 117;
    private const int eintrittVonSchulnummerSpalte = 125;
    private const int austrittsdatumSpalte = 122;
    private const int schulischeVorbildungSpalte = 128;
    private const int beruflicheVorbildungSpalte = 129;
    private const int lrsStoerungSpalte = 215;
    private const int lrsSchwaecheSpalte = 216;
    private const int lrsBisDatumSpalte = 254;
    private const int emailSpalte = 269;
    private const int notfallrufnummerSpalte = 270;

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
          string[] cleanArray = array.Select(aString => aString.Trim(new char[] { '\"', ' ', '\n' })).ToArray();

          int klasseId = GetKlasseId(klasseTableAdapter, cleanArray[klasseSpalte].Trim());
          if (klasseId == -1)
          {
            log.Debug("Ignoriere einen Schüler ohne richtige Klasse. Übergebene Klasse war " + cleanArray[klasseSpalte]);
            continue;
          }

          // wenn der Schüler noch nicht vorhanden ist
          if (tableAdapter.GetDataById(int.Parse(cleanArray[schuelerIdSpalte])).Count == 0)
          {
            tableAdapter.Insert(
              int.Parse(cleanArray[schuelerIdSpalte]),
              cleanArray[nachnameSpalte],
              cleanArray[vornameSpalte],
              klasseId,
              cleanArray[rufnameSpalte],
              cleanArray[geschlechtSpalte],
              ParseDate(cleanArray[geburtsdatumSpalte]),
              cleanArray[geburtsortSpalte],
              cleanArray[bekenntnisSpalte],
              cleanArray[anschr1PlzSpalte],
              cleanArray[anschr1OrtSpalte],
              cleanArray[anschr1StrasseSpalte],
              cleanArray[anschr1TelefonSpalte],
              ChangeAusbildungsrichtung(cleanArray[ausbildungsrichtungSpalte]),
              cleanArray[fremdsprache2Spalte],
              cleanArray[reliOderEthikSpalte],
              cleanArray[wahlpflichtfachSpalte],
              cleanArray[wahlfach1Spalte],
              cleanArray[wahlfach2Spalte],
              cleanArray[wahlfach3Spalte],
              cleanArray[wahlfach4Spalte],
              cleanArray[wdh1JahrgangsstufeSpalte],
              cleanArray[wdh2JahrgangsstufeSpalte],
              cleanArray[wdh1GrundSpalte],
              cleanArray[wdh2GrundSpalte],
              ParseDate(cleanArray[probezeitBisSpalte]),
              ParseDate(cleanArray[austrittsdatumSpalte]),
              cleanArray[schulischeVorbildungSpalte],
              cleanArray[beruflicheVorbildungSpalte],
              cleanArray[lrsStoerungSpalte] == "1",
              cleanArray[lrsSchwaecheSpalte] == "1",
              ParseDate(cleanArray[lrsBisDatumSpalte]),
              cleanArray[verwandtschaftsbezeichnungEltern1Spalte],
              cleanArray[nachnameEltern1Spalte],
              cleanArray[vornameEltern1Spalte],
              cleanArray[anredeEltern1Spalte],
              cleanArray[nachnameEltern2Spalte],
              cleanArray[vornameEltern2Spalte],
              cleanArray[anredeEltern2Spalte],
              cleanArray[verwandtschaftsbezeichnungEltern2Spalte],
              cleanArray[eintrittJgstSpalte],
              ParseDate(cleanArray[eintrittDatumSpalte]),
              !string.IsNullOrEmpty(cleanArray[eintrittVonSchulnummerSpalte]) ? int.Parse(cleanArray[eintrittVonSchulnummerSpalte]) : -1,
              cleanArray[emailSpalte],
              cleanArray[notfallrufnummerSpalte]
              );
          }
          else
          {
            tableAdapter.Update(
              cleanArray[nachnameSpalte],
              cleanArray[vornameSpalte],
              klasseId,
              cleanArray[rufnameSpalte],
              cleanArray[geschlechtSpalte],
              ParseDate(cleanArray[geburtsdatumSpalte]),
              cleanArray[geburtsortSpalte],
              cleanArray[bekenntnisSpalte],
              cleanArray[anschr1PlzSpalte],
              cleanArray[anschr1OrtSpalte],
              cleanArray[anschr1StrasseSpalte],
              cleanArray[anschr1TelefonSpalte],
              ChangeAusbildungsrichtung(cleanArray[ausbildungsrichtungSpalte]),
              cleanArray[fremdsprache2Spalte],
              cleanArray[reliOderEthikSpalte],
              cleanArray[wahlpflichtfachSpalte],
              cleanArray[wahlfach1Spalte],
              cleanArray[wahlfach2Spalte],
              cleanArray[wahlfach3Spalte],
              cleanArray[wahlfach4Spalte],
              cleanArray[wdh1JahrgangsstufeSpalte],
              cleanArray[wdh2JahrgangsstufeSpalte],
              cleanArray[wdh1GrundSpalte],
              cleanArray[wdh2GrundSpalte],
              ParseDate(cleanArray[probezeitBisSpalte]),
              ParseDate(cleanArray[austrittsdatumSpalte]),
              cleanArray[schulischeVorbildungSpalte],
              cleanArray[beruflicheVorbildungSpalte],
              cleanArray[lrsStoerungSpalte] == "1",
              cleanArray[lrsSchwaecheSpalte] == "1",
              ParseDate(cleanArray[lrsBisDatumSpalte]),
              cleanArray[verwandtschaftsbezeichnungEltern1Spalte],
              cleanArray[nachnameEltern1Spalte],
              cleanArray[vornameEltern1Spalte],
              cleanArray[anredeEltern1Spalte],
              cleanArray[nachnameEltern2Spalte],
              cleanArray[vornameEltern2Spalte],
              cleanArray[anredeEltern2Spalte],
              cleanArray[verwandtschaftsbezeichnungEltern2Spalte],
              cleanArray[eintrittJgstSpalte],
              ParseDate(cleanArray[eintrittDatumSpalte]),
              !string.IsNullOrEmpty(cleanArray[eintrittVonSchulnummerSpalte]) ? int.Parse(cleanArray[eintrittVonSchulnummerSpalte]) : -1,
              cleanArray[emailSpalte],
              cleanArray[notfallrufnummerSpalte],
              int.Parse(cleanArray[schuelerIdSpalte])
              );
          }
        }
      }
    }

    /// <summary>
    /// Sucht die ID der Klasse in der Datenbank. Versucht auch zu beurteilen, ob es sich überhaupt um eine echte Klasse handelt.
    /// Legt auch Klassen ggf. selbstständig in der Datenbank an.
    /// </summary>
    /// <param name="klasseTableAdapter">Der Table Adapter für Klassen.</param>
    /// <param name="klasse">Die Klassenbezeichnung.</param>
    /// <returns>Die Id der Klasse oder -1 falls die Klasse ungültig ist.</returns>
    private static int GetKlasseId(KlasseTableAdapter klasseTableAdapter, string klasse)
    {
      var klasseDBresult = klasseTableAdapter.GetDataByBezeichnung(klasse);
      if (klasseDBresult.Count == 1)
      {
        return klasseDBresult[0].Id;
      }
      else
      {
        // -N : Klassen für kommendes Jahr
        // AHR, FHR: Klassen des vergangenen Jahres
        // Abm: Abmeldungen
        // Ex, Import: ?
        if (klasse.EndsWith("-N") || klasse.Contains("AHR") || klasse.Contains("FHR") || klasse.Contains("Abm") || klasse.Equals("Ex") || klasse.Equals("Import"))
        {
          return -1;
        }
        else
        {
          klasseTableAdapter.Insert(klasse, null);
          var neueKlasse = klasseTableAdapter.GetDataByBezeichnung(klasse);
          return neueKlasse[0].Id;
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
        default: throw new InvalidOperationException("Unbekannte Ausbildungsrichtung " + ausbildungsrichtung);
      }
    }
  }
}
