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
        /// Konstruktor; öffnet die Exceldatei und verwaltet den Freigabemechanismus
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
            }
            catch (Exception exp)
            {
                log.Fatal("Fehler beim Öffnen der Excel-Datei " + fileName, exp);
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

    public Worksheet getSheet(string sheetName)
    {
        var sheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals(sheetName) select sh).FirstOrDefault();
        if (sheet == null)
        {
            throw new InvalidOperationException("Kein Sheet mit dem Namen " + sheetName + " gefunden");
        }
        return sheet;
    }

    /// <summary>
    /// Der Logger.
    /// </summary>
    protected static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Reflection Missing object. Wird gebraucht, um Daten einzulesen.
    /// </summary>
    protected static object missing = System.Reflection.Missing.Value;

    public bool UnsavedChanges
    {
        get;
        protected set;
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

    /// <summary>
    /// Klasse, die eine Notendatei öffnet und Verweise auf die wichtigsten Sheets liefert
    /// </summary>
    public class OpenNotendatei : OpenExcel
    {
        /// <summary>
        /// Das Sheet Notenbogen
        /// </summary>
        public Worksheet notenbogen;

        /// <summary>
        /// Das Sheet Abschlussprüfung
        /// </summary>
        public Worksheet AP;

        /// <summary>
        /// Das Sheet mit den ID-Daten der Schüler.
        /// </summary>
        public Worksheet sid;


        public OpenNotendatei(string filename) : base(filename)
        {
            notenbogen = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("Notenbogen") select sh).FirstOrDefault();
            if (notenbogen == null)
            {
                throw new InvalidOperationException("kein Sheet mit dem Namen \"Notenbogen\" gefunden");
            }

            AP = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("AP") select sh).FirstOrDefault();
            if (AP == null)
            {
                throw new InvalidOperationException("kein Sheet mit dem Namen \"AP\" gefunden");
            }

            sid = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("diNo") select sh).FirstOrDefault();
            if (sid == null)
            {
                throw new InvalidOperationException("kein Sheet mit dem Namen \"diNo\" gefunden");
            }
        }

 
        /// <summary>
        /// Die Maximalanzahl von Schülern in einem ExcelSheet.
        /// </summary>
        public const int MaxAnzahlSchueler = 35;
        /// <summary>
        /// Hängt einen neuen Schüler unten an die Datei an.
        /// </summary>
        /// <param name="aSchueler">Der Schüler.</param>
        public void AppendSchueler(diNoDataSet.SchuelerRow aSchueler)
        {
            UnsavedChanges = true;

            // TODO: Methode ungetestet
            // muss von unten her gesucht werden, da in der DB dieser Schüler schon weg sein kann.
            int zeile = 69;  // CellConstant.ZeileErsterSchueler + this.Schueler.Count * 2 + 1;
            int zeileFuerSId = 34;//  CellConstant.zeileSIdErsterSchueler + this.Schueler.Count + 1;
            WriteValue(notenbogen, CellConstant.Nachname + zeile, aSchueler.Name);
            WriteValue(AP, CellConstant.Vorname + (zeile + 1), "   " + aSchueler.Vorname);
            WriteValue(sid, CellConstant.SId + zeileFuerSId, aSchueler.Id.ToString());
            if (aSchueler.LRSStoerung || aSchueler.LRSSchwaeche)
            {
                WriteValue(notenbogen, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
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
        public void WriteValue(Excel.Worksheet sheet, string zelle, string value)
        {
            Excel.Range r = sheet.get_Range(zelle, missing);
            r.Value2 = value;
        }

        /// <summary>
        /// Liest einen Wert aus einer Zelle des gegebenen ExcelSheets.
        /// </summary>
        /// <param name="sheet">Das Excel Sheet.</param>
        /// <param name="zelle">Die Zelle, z. B. A2.</param>
        /// <returns>Den Wert der Zelle als String.</returns>
        public string ReadValue(Excel.Worksheet sheet, string zelle)
        {
            Excel.Range r = sheet.get_Range(zelle, missing);
            return r.Value2 == null ? null : Convert.ToString(r.Value2).Trim();
        }
    }

}
