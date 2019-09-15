using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diNo
{
  /// <summary>
  /// Liest die Wahlpflichtfächer aus dem Untis-Export
  /// </summary>
  public class WahlpflichtfachReader
  {
    /// <summary>
    /// Der log4net-Logger.
    /// </summary>
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private static char[] trimchar = new char[] { '"' };

  /// <summary>
  /// Die Methode zum Einlesen der Daten.
  /// </summary>
  /// <param name="fileName">Der Dateiname.</param>
  public static void Read(string fileName)
    {
      using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("iso-8859-1")))
      using (KursTableAdapter kursTableAdapter = new KursTableAdapter())
      {
        while (!reader.EndOfStream)
        {
          string line = reader.ReadLine();
          if (string.IsNullOrEmpty(line))
          {
            log.Debug("Ignoriere Leerzeile");
            continue;
          }

          string[] array = line.Split(new string[] { ";" }, StringSplitOptions.None);

          if (array.Count() == 0 || string.IsNullOrEmpty(array[0]))
          {
            log.Debug("Ignoriere unvollständige Zeile");
            continue;
          }

          string nameVorname = array[0].Trim(trimchar); // nur zur Kontrolle
          int kursId = int.Parse(array[1]); // Untis-KursId. Identisch zu diNo da IDs bereits belegt.
          string kursKuerzel = array[2].Trim(trimchar); // Untis-Kursname. Der ist identisch zu diNo.
          // was in array[3] steht weiß ich nicht - es scheint immer leer zu sein
          string klasse = array[4].Trim(trimchar); // nur zur Kontrolle
          // was in array[5] steht weiß ich nicht - es scheint immer leer zu sein
          int schuelerId = int.Parse(array[6].Trim(trimchar));
          // weiter hinten kommen noch Infos zu Parallelklassen o. Ä.

          Schueler schueler = new Schueler(schuelerId); // wirft Exception wenn nicht vorhanden. Das ist gut so.
          var kurse = kursTableAdapter.GetDataById(kursId);
          if (kurse.Count != 1)
          {
            throw new InvalidOperationException("Kurs " + kursKuerzel + " nicht gefunden oder nicht eindeutig!");
          }

          Kurs kurs = new Kurs(kurse[0]);
          schueler.MeldeAn(kurs);
        }
      }
    }
  }
}
