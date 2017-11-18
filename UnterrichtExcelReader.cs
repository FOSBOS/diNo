using log4net;
using System;
using System.Linq;

using Excel = Microsoft.Office.Interop.Excel;
using diNo.diNoDataSetTableAdapters;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections.Generic;

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
    public static void ReadUnterricht(string fileName)
    {
      var excelApp = new Microsoft.Office.Interop.Excel.Application();
      var workbook = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
      var sheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("Tabelle1") select sh).FirstOrDefault();
      if (sheet == null)
      {
        throw new InvalidOperationException("kein Sheet mit dem Namen \"Tabelle1\" gefunden");
      }

      int lastRow = sheet.get_Range("A" + sheet.Rows.Count, "B" + sheet.Rows.Count).get_End(Excel.XlDirection.xlUp).Row;
      for (int zeile = 5; zeile <= lastRow; zeile++)
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
        if ((new string[] { "SSl", "SNT", "SWi", "FPU", "FPA", "FPB", "TZ-Fö", "GK_BF", "M-Fö", "E-Fö", "Ph-Fö", "AWU", "Me", "SL", "SF" }).Contains(fach))
        {
          log.Debug("Ignoriere Förderunterricht, Ergänzungsunterricht, Seminarfach und diversen anderen Unfug - kein selbstständiger Unterricht");
          continue;
        }

        if (fach.ToUpper().Contains("FPV"))
        {
          // ignoriere Fachpraktische Vertiefung, die läuft anders
          continue;
        }

        if (string.IsNullOrEmpty(klassenString))
        {
          log.Debug("Unterricht Ohne Klassen wird ignoriert in Zeile " + zeile);
          continue;
        }

        var dbFach = FindOrCreateFach(fach);
        if (string.IsNullOrEmpty(dbFach.Bezeichnung.Trim()))
        {
          log.Debug("Ignoriere Ignoriere Fach ohne Namen : Kürzel " + dbFach.Kuerzel);
          continue;
        }

        var dblehrer = FindLehrer(lehrer);
        if (dblehrer == null)
        {
          log.Error("Ignoriere Kurse des unbekannten Lehrers " + lehrer);
          continue;
        }

        var klassen = klassenString.Split(',');
        var klasseKursAdapter = new KlasseKursTableAdapter();
        Dictionary<string, IList<string>> unterschiedlicheKlassen = GetKlassenTeile(klassen);
        string zweig = GetZweig(unterschiedlicheKlassen);
        var kurs = FindOrCreateKurs(dbFach.Bezeichnung.Trim() + " " + klassenString, dblehrer.Id, fach, zweig);

        foreach (var klasseKvp in unterschiedlicheKlassen)
        {
          // nur die eigentliche Klasse als Klasse erzeugen, nicht die Klassenteile
          var dbKlasse = FindOrCreateKlasse(klasseKvp.Key, true);

          if (klasseKursAdapter.ScalarQueryCountByKlasseAndKurs(dbKlasse.Id, kurs.Id) == 0)
          {
            klasseKursAdapter.Insert(dbKlasse.Id, kurs.Id);
          }

          // AddSchuelerToKurs(kurs, dbKlasse, kursSelector); Das machen wir beim Einlesen der Schüler
        }
      }

      workbook.Close(false, fileName, Type.Missing);
      Marshal.ReleaseComObject(workbook);
      excelApp.Quit();
    }

    /// <summary>
    /// Sucht den Klassenteil, für welchen der Kurs gilt.
    /// </summary>
    /// <param name="unterschiedlicheKlassen">Alle unterschiedlichen Klassen, die in den Kurs sollen samt deren Klassenteilen.</param>
    private static string GetZweig(Dictionary<string, IList<string>> unterschiedlicheKlassen)
    {
      // sind keine Klassenteile vorhanden oder mehrere Teilklassen in dem Kurs, dann trage die ganze Klasse ohne Zweigangabe ein
      // ansonsten (d.h. wenn nur ein Klassenteil in den Kurs gehen soll) trage im Kurs den Zweig ein.
      string teilklasse = null;
      foreach (var klasseKvp in unterschiedlicheKlassen)
      {
        if (klasseKvp.Value.Count == 1)
        {
          teilklasse = klasseKvp.Value[0];
        }
      }

      return teilklasse;
    }

    /// <summary>
    /// Teilt die Angabe aus Untis in Klassen und eine Liste deren Teilklassen auf (sofern vorhanden).
    /// </summary>
    /// <param name="klassen">Die Klassen wie sie aus Untis kommen.</param>
    /// <returns>Klassen und deren Teile.</returns>
    private static Dictionary<string, IList<string>> GetKlassenTeile(string[] klassen)
    {
      Dictionary<string, IList<string>> unterschiedlicheKlassen = new Dictionary<string, IList<string>>();
      foreach (var klasse in klassen)
      {
        string eigentlicheKlasse = klasse;
        string klassenteil = "";
        if (klasse.Contains("_"))
        {
          string[] teilstrings = klasse.Split('_');
          eigentlicheKlasse = teilstrings[0];
          klassenteil = teilstrings[1].Trim();
        }

        if (!unterschiedlicheKlassen.ContainsKey(eigentlicheKlasse))
        {
          unterschiedlicheKlassen.Add(eigentlicheKlasse, new List<string>());
        }

        if (!string.IsNullOrEmpty(klassenteil))
        {
          unterschiedlicheKlassen[eigentlicheKlasse].Add(klassenteil);
        }
      }

      return unterschiedlicheKlassen;
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
    /// <param name="aZweig">Der Zweig, für welchen der Kurs gilt.</param>
    /// <returns>Die Zurszeile in der Datenbank.</returns>
    public static diNoDataSet.KursRow FindOrCreateKurs(string aKursBezeichung, int aLehrerId, string aFach, string aZweig)
    {
      using (var kursAdapter = new KursTableAdapter())
      {
        // suche den Kurs in der Datenbank. Wenn neu => anlegen
        var kurse = kursAdapter.GetDataByBezeichnung(aKursBezeichung);
        if (kurse.Count == 0)
        {
          // suche Fach in der Datenbank
          var fach = FindOrCreateFach(aFach);
          string geschlecht = null;
          if (fach.Kuerzel == "Sw") geschlecht = "W";
          if (fach.Kuerzel == "Sm") geschlecht = "M";
          kursAdapter.Insert(aKursBezeichung, aLehrerId, fach.Id, aZweig, geschlecht);
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
            new KlasseTableAdapter().Insert(aklasse, null);
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
          new FachTableAdapter().Insert("", aFach,false,999);
          faecher = new FachTableAdapter().GetDataByKuerzel(aFach);
        }

        return faecher[0];
      }
    }
  }
}
