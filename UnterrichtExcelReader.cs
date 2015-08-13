using log4net;
using System;
using System.Linq;

using Excel = Microsoft.Office.Interop.Excel;
using diNo.diNoDataSetTableAdapters;
using System.Runtime.InteropServices;
using System.Data;

namespace diNo
{
  /// <summary>
  /// Liest die Unterrichtsplanung aus Stanis Excel-Datei.
  /// </summary>
  public class UnterrichtExcelReader
  {
    /// <summary>
    /// Reflection Missing object. Wird gebraucht, um Daten einzulesen.
    /// </summary>
    private static object missing = System.Reflection.Missing.Value;

    /// <summary>
    /// log 4 net logger.
    /// </summary>
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Die eigentliche Lese-Methode.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    /// <param name="kursSelector">Ein Selektor zur Prüfung, welche Schüler in welchen Kurs auch wirklich müssen.</param>
    public static void ReadUnterricht(string fileName, ISchuelerKursSelector kursSelector)
    {
      var excelApp = new Microsoft.Office.Interop.Excel.Application();
      var workbook = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
      var sheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("Tabelle1") select sh).FirstOrDefault();
      if (sheet == null)
      {
        throw new InvalidOperationException("kein Sheet mit dem Namen \"Tabelle1\" gefunden");
      }

      int rowCount = sheet.get_Range("A" + sheet.Rows.Count, "B" + sheet.Rows.Count).get_End(Excel.XlDirection.xlUp).Row;
      for (int zeile = 5; zeile < rowCount; zeile++)
      {
        string lehrer = ReadValue(sheet, "E" + zeile);
        string fach = ReadValue(sheet, "F" + zeile);
        string klassenString = ReadValue(sheet, "G" + zeile);

        if (string.IsNullOrEmpty(lehrer))
        {
          log.Debug("Unterricht Ohne Lehrer wird ignoriert in Zeile " + zeile);
          continue;
        }
        if (string.IsNullOrEmpty(fach))
        {
          log.Debug("Unterricht Ohne Fach wird ignoriert in Zeile " + zeile);
          continue;
        }
        if ((new string[] { "SSl", "SNT", "SWi", "FPU", "FPA", "FPB", "F-Wi", "TZ-Fö", "GK_BF", "M-Fö", "E-Fö", "Ph-Fö", "AWU", "Me", "SL", "SF" }).Contains(fach))
        {
          log.Debug("Ignoriere Förderunterricht, Ergänzungsunterricht, Seminarfach und diversen anderen Unfug - kein selbstständiger Unterricht");
          continue;
        }
        if (string.IsNullOrEmpty(klassenString))
        {
          log.Debug("Unterricht Ohne Klassen wird ignoriert in Zeile " + zeile);
          continue;
        }

        var dbFach = FindOrCreateFach(fach);
        var dblehrer = FindLehrer(lehrer);
        if (dblehrer == null)
        {
          log.Error("Ignoriere Kurse des unbekannten Lehrers " + lehrer);
          continue;
        }

        var kurs = FindOrCreateKurs(dbFach.Bezeichnung.Trim() + " " + klassenString, dblehrer.Id, fach);
        
        var klassen = klassenString.Split(',');
        foreach (var klasse in klassen)
        {
          var dbKlasse = FindOrCreateKlasse(klasse, true);
          new KlasseKursTableAdapter().Insert(dbKlasse.Id, kurs.Id);
          AddSchuelerToKurs(kurs, dbKlasse, kursSelector);
        }
      }

      workbook.Close(false, fileName, Type.Missing);
      Marshal.ReleaseComObject(workbook);
      excelApp.Quit();
    }

    /// <summary>
    /// Trägt einen Schüler in einen Kurs in der Datenbank ein.
    /// </summary>
    /// <param name="kurs">Der Kurs.</param>
    /// <param name="dbKlasse">Die Klasse.</param>
    /// <param name="kursSelector">Ein Selektor zur Prüfung, welche Schüler in welchen Kurs auch wirklich müssen.</param>
    private static void AddSchuelerToKurs(diNoDataSet.KursRow kurs, diNoDataSet.KlasseRow dbKlasse, ISchuelerKursSelector kursSelector)
    {
      using (SchuelerKursTableAdapter skursAdapter = new SchuelerKursTableAdapter())
      using (SchuelerTableAdapter sAdapter = new SchuelerTableAdapter())
      {
        sAdapter.ClearBeforeFill = true;
        var schuelerDerKlasse = sAdapter.GetDataByKlasse(dbKlasse.Id);
        if (schuelerDerKlasse.Count == 0)
        {
          log.Debug("Klasse " + dbKlasse.Bezeichnung + " ist leer");
          if (dbKlasse.Bezeichnung.StartsWith("FB") && dbKlasse.Bezeichnung.EndsWith("F"))
          {
            // z.B. FB13T_F meint die FOSler der Mischklasse FB13T. Evtl. sind die als eigene Klasse F13T in der DB
            string modifizierteKlasse = dbKlasse.Bezeichnung.Replace("FB", "F");
            modifizierteKlasse = modifizierteKlasse.Replace("_F", string.Empty);
            dbKlasse = FindOrCreateKlasse(modifizierteKlasse, false);
            if (dbKlasse != null)
            {
              schuelerDerKlasse = sAdapter.GetDataByKlasse(dbKlasse.Id);
            }
          }

          if (dbKlasse.Bezeichnung.StartsWith("FB") && dbKlasse.Bezeichnung.EndsWith("B"))
          {
            // z.B. FB13T_B meint die BOSler der Mischklasse FB13T. Evtl. sind die als eigene Klasse B13T in der DB
            string modifizierteKlasse = dbKlasse.Bezeichnung.Replace("FB", "B");
            modifizierteKlasse = modifizierteKlasse.Replace("_B", string.Empty);
            dbKlasse = FindOrCreateKlasse(modifizierteKlasse, false);
            if (dbKlasse != null)
            {
              schuelerDerKlasse = sAdapter.GetDataByKlasse(dbKlasse.Id);
            }
          }

          if (dbKlasse.Bezeichnung.EndsWith("_W") && dbKlasse.Bezeichnung.Contains("SW"))
          {
            // z.B. B13SW_W meint die Wirtschaftler der Mischklasse B13SW. Evtl. sind die nur als Mischklasse in der DB
            string modifizierteKlasse = dbKlasse.Bezeichnung.Replace("_W", string.Empty);
            dbKlasse = FindOrCreateKlasse(modifizierteKlasse, false);
            if (dbKlasse != null)
            {
              schuelerDerKlasse = sAdapter.GetDataByKlasseAndZweig(dbKlasse.Id, "W");
              if (schuelerDerKlasse.Count == 0)
              {
                schuelerDerKlasse = sAdapter.GetDataByKlasseAndZweig(dbKlasse.Id, "WVR");
              }
            }
          }

          if (dbKlasse.Bezeichnung.EndsWith("_S") && dbKlasse.Bezeichnung.Contains("SW"))
          {
            // z.B. B13SW_S meint die Wirtschaftler der Mischklasse B13SW. Evtl. sind die nur als Mischklasse in der DB
            string modifizierteKlasse = dbKlasse.Bezeichnung.Replace("_S", string.Empty);
            dbKlasse = FindOrCreateKlasse(modifizierteKlasse, false);
            if (dbKlasse != null)
            {
              schuelerDerKlasse = sAdapter.GetDataByKlasseAndZweig(dbKlasse.Id, "S");
            }
          }
        }

        foreach (var schueler in schuelerDerKlasse)
        {
          if (kursSelector.IsInKurs(schueler, kurs) && skursAdapter.GetCountBySchuelerAndKurs(schueler.Id, kurs.Id) == 0)
          {
            skursAdapter.Insert(schueler.Id, kurs.Id);
          }
        }
      }
    }

    /// <summary>
    /// Liest einen Wert aus einer Zelle des gegebenen ExcelSheets.
    /// </summary>
    /// <param name="sheet">Das Excel Sheet.</param>
    /// <param name="zelle">Die Zelle, z. B. A2.</param>
    /// <returns>Den Wert der Zelle als String.</returns>
    private static string ReadValue(Excel.Worksheet sheet, string zelle)
    {
      Microsoft.Office.Interop.Excel.Range r = sheet.get_Range(zelle, missing);
      return r.Value2 == null ? null : Convert.ToString(r.Value2).Trim();
    }

    /// <summary>
    /// Sucht den Kurs in der Datenbank. Falls nicht vorhanden, wird er neu angelegt.
    /// </summary>
    /// <param name="aKursBezeichung">Die Bezeichnung des Kurses.</param>
    /// <param name="aLehrerId">Die Id des Lehrers.</param>
    /// <param name="aFach">Das Fach.</param>
    /// <returns>Die Zurszeile in der Datenbank.</returns>
    public static diNoDataSet.KursRow FindOrCreateKurs(string aKursBezeichung, int aLehrerId, string aFach)
    {
      using (var kursAdapter = new KursTableAdapter())
      {
        // suche den Kurs in der Datenbank. Wenn neu => anlegen
        var kurse = kursAdapter.GetDataByBezeichnung(aKursBezeichung);
        if (kurse.Count == 0)
        {
          // suche Fach in der Datenbank
          var fach = FindOrCreateFach(aFach);
          kursAdapter.Insert(aKursBezeichung, aLehrerId, fach.Id);
        }

        kurse = kursAdapter.GetDataByBezeichnung(aKursBezeichung);
        return kurse[0];
      }
    }

    /// <summary>
    /// Sucht den Lehrer in der Datenbank.
    /// </summary>
    /// <param name="aKuerzel">Das Kürzel des Lehrers.<\param>
    /// <returns>Die Zeile des Lehrers in der Datenbank.</returns>
    public static diNoDataSet.LehrerRow FindLehrer(string aKuerzel)
    {
      using (var ltAdapter = new LehrerTableAdapter())
      {
        var lehrer = ltAdapter.GetDataByKuerzel(aKuerzel);
        if (lehrer.Count == 0)
        {
          return null;
        }

        return lehrer[0];
      }
    }

    /// <summary>
    /// Sucht eine Klasse in der Datenbank. Wenn nicht gefunden, kann sie neu angelegt werden.
    /// </summary>
    /// <param name="aklasse">Die Klassenbezeichnung.</param>
    /// <param name="createIfNotFound">True wenn die Klasse ggf. auch neu angelegt werden darf.</param>
    /// <returns>Die Zeile der Klasse in der Datenbank oder null wenn nicht gefunden.</returns>
    public static diNoDataSet.KlasseRow FindOrCreateKlasse(string aklasse, bool createIfNotFound)
    {
      // suche die Klasse in der Datenbank. Wenn neu => anlegen
      using (var klAdapter = new KlasseTableAdapter())
      {
        var klasse = klAdapter.GetDataByBezeichnung(aklasse);
        if (klasse.Count == 0)
        {
          if (createIfNotFound)
          {
            new KlasseTableAdapter().Insert(aklasse);
          }
          else
          {
            return null;
          }
        }

        return new KlasseTableAdapter().GetDataByBezeichnung(aklasse)[0];
      }
    }

    /// <summary>
    /// Sucht oder erzeugt ein Fach in der Datenbank.
    /// </summary>
    /// <param name="aFach">Das Fachkürzel.</param>
    /// <returns>Die Zeile des Faches in der Datenbank.</returns>
    public static diNoDataSet.FachRow FindOrCreateFach(string aFach)
    {
      using (var fachAdapter = new FachTableAdapter())
      {
        var faecher = fachAdapter.GetDataByKuerzel(aFach);
        if (faecher.Count == 0)
        {
          // Fach voller Name muss in der Datenbank angepasst werden
          new FachTableAdapter().Insert("", aFach);
          faecher = new FachTableAdapter().GetDataByKuerzel(aFach);
        }

        return faecher[0];
      }
    }
  }
}
