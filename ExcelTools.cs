using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace diNo
{
  /// <summary>
  /// Klasse verwaltet die Schnittstelle zur Exceldatei
  /// </summary>
  public class OpenExcel : IDisposable
  {

        /// <summary>
        /// Konstruktor; öffnet die Exceldatei und stellt einen Verweis auf Notenbogen, AP und ID-Daten bereit
        /// </summary>
        /// <param name="fileName">Dateiname.</param>
        public OpenExcel(string fileName)
        {
            try
            {
                this.excelApp = new Excel.Application();
                this.FileName = fileName;

                this.workbook = excelApp.Workbooks.Open(this.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                excelApp.Visible = true;

                sheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("Notenbogen") select sh).FirstOrDefault();
                if (sheet == null)
                {
                    throw new InvalidOperationException("kein Sheet mit dem Namen \"Notenbogen\" gefunden");
                }

                sheetAP = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("AP") select sh).FirstOrDefault();
                if (sheet == null)
                {
                    throw new InvalidOperationException("kein Sheet mit dem Namen \"AP\" gefunden");
                }

                sidsheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("diNo") select sh).FirstOrDefault();
                if (sidsheet == null)
                {
                    throw new InvalidOperationException("kein Sheet mit dem Namen \"diNo\" gefunden");
                }

            }
            catch (Exception exp)
            {
                log.Fatal("Fehler beim Schreiben der Excel-Datei " + fileName, exp);
            }
        }

        // Destruktor
        ~OpenExcel()
        {
            if (UnsavedChanges) workbook.Save();
            workbook.Close(false, FileName, Type.Missing);
            Marshal.ReleaseComObject(workbook);
            excelApp.Quit();
        }

    /// <summary>
    /// Die Maximalanzahl von Schülern in einem ExcelSheet.
    /// </summary>
    private const int MaxAnzahlSchueler = 35;

    public enum Notensheets
    {
        Notenbogen, AP, sid
    }

    /// <summary>
    /// Der Dateiname.
    /// </summary>
    public string FileName
    {
        get;
        private set;
    }

    /// <summary>
    /// Referenz auf die Excel Application.
    /// </summary>
    private Excel.Application excelApp;

    /// <summary>
    /// Das Excel Workbook.
    /// </summary>
    public Excel.Workbook workbook;

    /// <summary>
    /// Das Sheet Notenbogen
    /// </summary>
    private Worksheet sheet;

    /// <summary>
    /// Das Sheet Abschlussprüfung
    /// </summary>
    private Worksheet sheetAP;

    /// <summary>
    /// Das Sheet mit den ID-Daten der Schüler.
    /// </summary>
    private Worksheet sidsheet;

    /// <summary>
    /// Liefert das passende Sheet der Exceldatei
    /// </summary>
    public Worksheet getSheet(Notensheets s)
    {
        switch (s)
        {           
            case Notensheets.AP: return sheetAP;
            case Notensheets.sid: return sidsheet;
            default: return sheet; // Notenbogen
        }
    }

    public Worksheet getSheet(string sheetName)
    {
        var pruefungssheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals(sheetName) select sh).FirstOrDefault();
        if (pruefungssheet == null)
        {
            throw new InvalidOperationException("kein Sheet mit dem Namen " + sheetName + " gefunden");
        }
        return pruefungssheet;
    }

        /// <summary>
        /// Der Logger.
        /// </summary>
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Reflection Missing object. Wird gebraucht, um Daten einzulesen.
    /// </summary>
    private static object missing = System.Reflection.Missing.Value;

 
     /// <summary>
    /// Hängt einen neuen Schüler unten an die Datei an.
    /// </summary>
    /// <param name="aSchueler">Der Schüler.</param>
    public void AppendSchueler(Schueler aSchueler)
    {
      this.UnsavedChanges = true;

            // TODO: Methode ungetestet
            // muss von unten her gesucht werden, da in der DB dieser Schüler schon weg sein kann.
            int zeile = 69;  // CellConstant.ZeileErsterSchueler + this.Schueler.Count * 2 + 1;
            int zeileFuerSId = 34;//  CellConstant.zeileSIdErsterSchueler + this.Schueler.Count + 1;
      WriteValue(sheet, CellConstant.Nachname + zeile, aSchueler.Data.Name);
      WriteValue(sheet, CellConstant.Vorname + (zeile + 1), "   " + aSchueler.Data.Vorname);
      WriteValue(sidsheet, CellConstant.SId + zeileFuerSId, aSchueler.Id.ToString());
      if (aSchueler.IsLegastheniker)
      {
        WriteValue(sheet, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
      }

      this.workbook.Save();
      this.UnsavedChanges = false;
    }

    /// <summary>
    /// Schreibt einen Wert in die Zelle des gegebenen Excel-Sheets
    /// </summary>
    /// <param name="sheet">Das Excel Sheet.</param>
    /// <param name="zelle">Die Zelle, z. B. A2.</param>
    /// <param name="value">Den Wert der Zelle als String.</param>
    public void WriteValue(Notensheets sheet, string zelle, string value)
    {
      Excel.Range r = getSheet(sheet).get_Range(zelle, missing);
      r.Value2 = value;
    }

    public void WriteValue(Excel.Worksheet sheet, string zelle, string value)
    {
        Excel.Range r = sheet.get_Range(zelle, missing);
        r.Value2 = value;
    }

    public bool UnsavedChanges
    {
      get;
      private set;
    }

    #region IDisposable Member

    /// <summary>
    /// Disposes.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
    }

    /// <summary>
    /// Disposes.
    /// </summary>
    /// <param name="disposing">If true, free native resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.workbook != null)
        {
          this.workbook.Close(this.UnsavedChanges, this.FileName, Type.Missing);
          Marshal.ReleaseComObject(this.workbook);
          this.workbook = null;
        }

        this.excelApp = null;
      }

      GC.SuppressFinalize(this);
    }

    #endregion
  }
}
