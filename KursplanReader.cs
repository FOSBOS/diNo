using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

namespace diNo
{
  /// <summary>
  /// Eine Zeile in der Kursplandatei
  /// </summary>
  public class KursplanZeile
  {
    public string Klasse { get; set; }
    public string FachKurzbez { get; set; }
    public string LehrerKuerzel { get; set; }
    public string KursBezeichnung { get; set; }
    public string Jahrgangsstufe { get; set; }
    public string Schulart { get; set; }
  }

  /// <summary>
  /// Hilfsklasse zum Vergleichen zweier Kursplanzeilen
  /// </summary>
  public class KursplanZeileEqualityComparer : EqualityComparer<KursplanZeile>
  {
    /// <summary>
    /// Die eigentliche Vergleichsroutine.
    /// </summary>
    private Func<KursplanZeile, KursplanZeile, bool> compareFunc;

    /// <summary>
    /// Funktion, die einen Hashwert für die zu vergleichenden Zeilen liefert.
    /// Achtung diese Funktion muss zur compare-Funktion passen, d. h. wenn zwei Objekte gleich sind muss auch derselbe Hash rauskommen.
    /// </summary>
    private Func<KursplanZeile, int> hashcodeFunc;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="compareFunc">Die eigentliche Vergleichsroutine.</param>
    /// <param name="hashcodeFunc">Funktion, die einen Hashwert für die zu vergleichenden Zeilen liefert.</param>
    public KursplanZeileEqualityComparer(Func<KursplanZeile, KursplanZeile, bool> compareFunc, Func<KursplanZeile, int> hashcodeFunc)
    {
      this.compareFunc = compareFunc;
      this.hashcodeFunc = hashcodeFunc;
    }

    /// <summary>
    /// Prüft zwei Kursplanzeilen auf Gleichheit.
    /// </summary>
    /// <param name="x">erste Zeile.</param>
    /// <param name="y">zweite Zeile.</param>
    /// <returns>true wenn die Zeilen gleich sind, sonst false.</returns>
    public override bool Equals(KursplanZeile x, KursplanZeile y)
    {
      return compareFunc(x, y);
    }

    /// <summary>
    /// Liefert einen HashCode für ein Kursplanzeile.
    /// </summary>
    /// <param name="obj">Die Kursplanzeile.</param>
    /// <returns>Der HashCode.</returns>
    public override int GetHashCode(KursplanZeile obj)
    {
      return hashcodeFunc(obj);
    }
  }

  /// <summary>
  /// Klasse zum Einlesen der Kursplandatei.
  /// </summary>
  public class KursplanReader
  {
    private const int klasseSpalte = 1;
    private const int fachSpalte = 2;
    private const int lehrerkuerzelSpalte = 3;
    private const int kursSpalte = 5;
    private const int jahrgangsstufeSpalte = 8;
    private const int schulartSpalte = 10;

    /// <summary>
    /// Der Logger.
    /// </summary>
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Methode zum Einlesen des Kursplans aus einer Datei.
    /// </summary>
    /// <param name="fileName">Der Dateiname samt Pfad.</param>
    /// <returns>Liste der eingelesenen Kursplanzeilen.</returns>
    public static IList<KursplanZeile> Read(string fileName)
    {
      IList<KursplanZeile> result = new List<KursplanZeile>();


      log.Debug("öffne datei " + fileName);
      using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("iso-8859-1")))
      {
        while (!reader.EndOfStream)
        {
          string line = reader.ReadLine();
          if (string.IsNullOrEmpty(line))
          {
            continue;
          }

          //ToDo: Es gibt hier Einträge ohne Lehrer: Was bedeuten diese und was machen wir damit?
          string[] array = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);

          //säubere die Einzeleinträge (Anführungsstriche, Leerzeichen etc. entfernen)
          string[] cleanArray = array.Select(aString => aString.Trim(new char[] { '\"', ' ', '\n' })).ToArray();
          result.Add(new KursplanZeile() {
            Klasse = cleanArray[klasseSpalte],
            FachKurzbez = cleanArray[fachSpalte],
            LehrerKuerzel = cleanArray[lehrerkuerzelSpalte],
            KursBezeichnung = cleanArray[kursSpalte],
            Jahrgangsstufe = cleanArray[jahrgangsstufeSpalte],
            Schulart = cleanArray[schulartSpalte]
          });
        }
      }

      log.Debug(result.Count + " Einträge gelesen");
      return result;
    }
  }
}
