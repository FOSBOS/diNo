﻿using diNo.diNoDataSetTableAdapters;
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
      // TODO: Notbehelf, weil die IDs nicht in Untis gespeichert sind      
      IDictionary<string, int> anmeldenameZuID = new Dictionary<string, int>();

      string directory = Path.GetDirectoryName(fileName);
      try
      {
        using (StreamReader reader = new StreamReader(directory + "\\ZuordnungSchueler.txt", Encoding.GetEncoding("utf-8")))
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
              log.Debug("Zuordnungsdatei: Ignoriere unvollständige Zeile");
              continue;
            }

            int id = int.Parse(array[0]);
            string anmeldename = array[4];
            anmeldenameZuID.Add(anmeldename, id);
          }
        }
      }
      catch
      {
        log.Debug("Keine Zuordnungsdatei gefunden.");
      }

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

          string schuelerRef = array[0].Trim(trimchar);
          int kursId = 0;
          try
          {
            kursId = int.Parse(array[1]); // Untis-KursId.
          }
          catch
          {
            log.Warn("Kurs-ID " + array[1] + " bei Schüler " + schuelerRef + " konnte nicht konvertiert werden.");
            continue;
          }

          int schuelerId = 0;
          try
          {
            schuelerId = int.Parse(schuelerRef);
          }
          catch
          {
            ;
            // nichts tun --> hoffentlich klappt es mit der Zuordnungstabelle.
          }

          Schueler schueler = null;
          if (schuelerId == 0) // externe Id konnte nicht geladen werden ==> Zuordnungstabelle verwenden
          {
            try
            {
              schuelerId = anmeldenameZuID[schuelerRef]; // wirft Exception wenn nicht vorhanden. Das ist gut so.
            }
            catch
            {
              log.Error("Schüler " + schuelerRef + " in der Zuordnungstabelle nicht gefunden.");
              continue;
            }
          }
          try
          {
            schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
          }
          catch
          {
            log.Error("Schüler mit ID=" + schuelerId + " nicht in der Datenbank gefunden.");
            continue;
          }
          try
          {
            Kurs kurs = Zugriff.Instance.KursRep.Find(kursId);
            schueler.MeldeAn(kurs);
          }
          catch
          {
            log.Error("Schüler mit ID=" + schuelerId + " konnte nicht im Kurs " + kursId + " angemeldet werden.");
          }
        }
      }
    }
  }
}
