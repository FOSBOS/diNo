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
          // TODO: die Daten direkt in ein SchuelerRow schreiben, und dann den Insert auf dieses Objekt machen
          if (tableAdapter.GetDataById(int.Parse(cleanArray[schuelerIdSpalte])).Count == 0)
          {/*
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
              ); */
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
          klasseTableAdapter.Insert(klasse, null, null);
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
