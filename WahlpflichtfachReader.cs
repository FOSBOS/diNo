using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            continue;
          }

          string[] array = line.Split(new string[] { ";" }, StringSplitOptions.None);

          if (array.Count() == 0 || string.IsNullOrEmpty(array[0]))
          {
            log.Debug("Ignoriere unvollständige Zeile");
            continue;
          }

          string asvid = array[0].Trim(trimchar);
          int kursId = 0;
          try
          {
            kursId = int.Parse(array[1]); // Untis-KursId.
          }
          catch
          {
            log.Warn("Kurs-ID " + array[1] + " bei Schüler " + asvid + " konnte nicht konvertiert werden.");
            continue;
          }
          
          Schueler schueler = null;
          try
          {
            schueler = Zugriff.Instance.SchuelerRep.FindBy(x => x.AsvId == asvid);
          }
          catch
          {
            log.Error("Schüler mit ID=" + asvid + " nicht in der Datenbank gefunden.");
            continue;
          }
          try
          {
            Kurs kurs = Zugriff.Instance.KursRep.Find(kursId);
            schueler.MeldeAn(kurs);
          }
          catch
          {
            log.Error("Schüler mit ID=" + asvid + " konnte nicht im Kurs " + kursId + " angemeldet werden.");
          }
        }
      }
    }
  }
}
