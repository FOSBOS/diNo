using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Globalization;
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
        if (OpenExcel.excelApp == null)
          OpenExcel.excelApp = new Excel.Application();
        this.FileName = fileName;

        this.workbook = excelApp.Workbooks.Open(this.FileName, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        excelApp.Visible = false;
      }
      catch (Exception exp)
      {
        log.Fatal("Fehler beim Öffnen der Excel-Datei " + fileName, exp);
      }
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
    private static Excel.Application excelApp;

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

        excelApp.Quit();
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
      int zeile = GetErsteFreieZeile(notenbogen);
      int zeileFuerSId = GetSidZeileForNotenbogenZeile(zeile);
      WriteValue(notenbogen, CellConstant.Nachname + zeile, aSchueler.Name);
      WriteValue(notenbogen, CellConstant.Vorname + (zeile + 1), "   " + aSchueler.Rufname);
      WriteValueProtectedCell(sid, CellConstant.SId + zeileFuerSId, aSchueler.Id.ToString());
      if (aSchueler.LRSStoerung || aSchueler.LRSSchwaeche)
      {
        WriteValue(notenbogen, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
      }
    }

    /// <summary>
    /// Entfernt einen Schüler aus der Datei (Name und Id). Lässt seine Noten aber stehen.
    /// </summary>
    /// <param name="aSchueler">der Schüler.</param>
    public void RemoveSchueler(diNoDataSet.SchuelerRow aSchueler)
    {
      UnsavedChanges = true;
      RemoveSchueler(aSchueler.Id);
    }

    /// <summary>
    /// Entfernt einen Schüler aus der Datei (Name und Id). Lässt seine Noten aber stehen.
    /// </summary>
    /// <param name="schuelerId">die Id des Schülers.</param>
    public void RemoveSchueler(int schuelerId)
    {
      UnsavedChanges = true;
      int zeileSId = GetSidZeileForSchueler(schuelerId);
      int zeileNachname = GetNotenbogenZeileForSidZeile(zeileSId);
      WriteValue(notenbogen, CellConstant.Nachname + zeileNachname, "");
      WriteValue(notenbogen, CellConstant.Vorname + (zeileNachname + 1), "");
    }

    /// <summary>
    /// Ruft den Legasthenievermerk für den Schüler ab.
    /// </summary>
    /// <param name="schuelerId">ID des Schülers.</param>
    /// <returns>Ob der Schüler Legastheniker ist oder nicht.</returns>
    public bool GetLegasthenievermerk(int schuelerId)
    {
      int zeileSId = GetSidZeileForSchueler(schuelerId);
      int zeileNachname = GetNotenbogenZeileForSidZeile(zeileSId);
      string value = ReadValue(notenbogen, CellConstant.LegasthenieVermerk + zeileNachname);
      return CellConstant.LegasthenieEintragung.Equals(value);
    }

    /// <summary>
    /// Setzt den Legasthenievermerk für  den Schüler.
    /// </summary>
    /// <param name="schuelerId">ID des Schülers.</param>
    /// <param name="isLegastheniker">Ob der Schüler Legastheniker ist oder nicht.</param>
    public void SetLegasthenievermerk(int schuelerId, bool isLegastheniker)
    {
      UnsavedChanges = true;
      int zeileSId = GetSidZeileForSchueler(schuelerId);
      int zeileNachname = GetNotenbogenZeileForSidZeile(zeileSId);
      SetLegasthenievermerkByZeile(zeileNachname, isLegastheniker);
    }

    /// <summary>
    /// Setzt den Legasthenievermerk für einen Schüler anhand der Zeile.
    /// </summary>
    /// <param name="zeile">Die Zeile, in der der Schueler steht.</param>
    /// <param name="isLegastheniker">Ob der Schüler Legastheniker ist oder nicht.</param>
    public void SetLegasthenievermerkByZeile(int zeile, bool isLegastheniker)
    {
      UnsavedChanges = true;
      string value = isLegastheniker ? CellConstant.LegasthenieEintragung : string.Empty;
      WriteValue(notenbogen, CellConstant.LegasthenieVermerk + zeile, value);
    }

    public int GetSidZeileForSchueler(int schuelerId)
    {
      for (int i = CellConstant.zeileSIdErsterSchueler; i <= MaxAnzahlSchueler + CellConstant.zeileSIdErsterSchueler ; i++)
      {
        string aValue = ReadValue(sid, CellConstant.SId + i);
        int intValue = int.Parse(aValue);
        if (intValue.Equals(schuelerId))
        {
          return i;
        }
      }

      throw new InvalidOperationException("Schüler nicht gefunden. Id "+schuelerId);
    }

    private int GetNotenbogenZeileForSidZeile(int sidZeile)
    {
      int indexSchueler = sidZeile - 4; // Beginnend mit dem Nullten
      return 2 * indexSchueler + 5;
    }

    private int GetSidZeileForNotenbogenZeile(int notenbogenZeile)
    {
      int indexSchueler = (notenbogenZeile - 5) / 2; // Beginnend mit dem Nullten
      return indexSchueler + 4;
    }

    private int GetErsteFreieZeile(Excel.Worksheet sheet)
    {
      int zeile = 73;
      while (string.IsNullOrEmpty(ReadValue(sheet, "B"+zeile)) && zeile >= 5)
      {
        zeile = zeile - 2;
      }

      return zeile + 2;
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
    /// Schreibt einen Wert in die Zelle des gegebenen Excel-Sheets
    /// </summary>
    /// <param name="sheet">Das Excel Sheet.</param>
    /// <param name="zelle">Die Zelle, z. B. A2.</param>
    /// <param name="value">Den Wert der Zelle als String.</param>
    public void WriteValueProtectedCell(Excel.Worksheet sheet, string zelle, string value)
    {
      sheet.Unprotect("1111");
      Excel.Range r = sheet.get_Range(zelle, missing);
      r.Value2 = value;
      sheet.Protect("1111", false, true);
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

    public byte? ReadNote(Notentyp typ, string zelle)
    {
      string v;
      if (typ == Notentyp.APMuendlich || typ == Notentyp.APSchriftlich)
        v = ReadValue(AP, zelle);
      else
        v = ReadValue(notenbogen, zelle);

      return !string.IsNullOrEmpty(v) ? Convert.ToByte(v, CultureInfo.CurrentUICulture) : (byte?)null;
    }

    public decimal? ReadSchnitt(BerechneteNotentyp typ, Halbjahr hj, int zeile)
    {
      string zelle = CellConstant.getSchnittZelle(typ, hj, zeile);
      if (zelle == null)
        return null;

      string v;
      if (typ == BerechneteNotentyp.APGesamt)
        v = ReadValue(AP, zelle);
      else
        v = ReadValue(notenbogen, zelle);

      return !string.IsNullOrEmpty(v) ? Convert.ToDecimal(v, CultureInfo.CurrentUICulture) : (decimal?)null;
    }

    public byte? ReadSchnittGanzzahlig(BerechneteNotentyp typ, Halbjahr hj, int zeile)
    {
      string zelle = CellConstant.getSchnittZelle(typ, hj, zeile);
      if (zelle == null)
        return null;

      string v;
      if (typ == BerechneteNotentyp.Abschlusszeugnis)
        v = ReadValue(AP, zelle);
      else
        v = ReadValue(notenbogen, zelle);

      return !string.IsNullOrEmpty(v) ? Convert.ToByte(v, CultureInfo.CurrentUICulture) : (byte?)null;
    }
  }

  /// <summary>
  /// Statusmeldungen als Event Args.
  /// </summary>
  public class StatusChangedEventArgs : EventArgs
  {
    public string Meldung
    {
      get;
      set;
    }
  }
}
