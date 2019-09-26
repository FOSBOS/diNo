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
      OpenExcel.excelApp.DisplayAlerts = false; // unterdrückt Meldungen, die von Excel kommen
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
          try
          {
            // wir speichern jetzt generell ohne Speichern, weil das jetzt von diNo direkt erzwungen wird.
            this.workbook.Close(false /*this.UnsavedChanges*/, this.FileName, Type.Missing);
          }
          catch (COMException exp)
          {
            // dummerweise wird eine COMException ausgelöst, wenn jemand beim Speichern auf "Nein" klickt
            // schließe in diesem Fall ohne die Änderungen zu speichern
            log.Debug(exp);
            //this.workbook.Close(false, this.FileName, Type.Missing);
          }

          Marshal.ReleaseComObject(this.workbook);
          this.workbook = null;
        }

        excelApp.Quit();
      }

      GC.SuppressFinalize(this);
    }
    #endregion
  }

  public abstract class BasisNotendatei: OpenExcel
  {
    /// <summary>
    /// Die Maximalanzahl von Schülern in einem ExcelSheet.
    /// </summary>
    public const int MaxAnzahlSchueler = 35;

    /// <summary>
    /// Das Sheet Notenbogen
    /// </summary>
    public Worksheet notenbogen;

    /// <summary>
    /// Das Sheet mit den Daten des zweiten Halbjahres
    /// </summary>
    public Worksheet notenbogen2;

    /// <summary>
    /// Das Sheet Abschlussprüfung
    /// </summary>
    public Worksheet AP;

    /// <summary>
    /// Das Sheet mit den ID-Daten der Schüler.
    /// </summary>
    public Worksheet sid;

    public BasisNotendatei(string filename): base(filename)
    {
      notenbogen = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("1. Halbjahr") select sh).FirstOrDefault();
      if (notenbogen == null)
      {
        notenbogen = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("Notenbogen") select sh).FirstOrDefault(); // für Dateien nach alter SchO
        if (notenbogen == null)
        {
          throw new InvalidOperationException("kein Sheet mit dem Namen \"Notenbogen\" oder \"1. Halbjahr\" gefunden");
        }
      }

      notenbogen2 = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("2. Halbjahr") select sh).FirstOrDefault();
      // hier dieses Jahr noch keine Exception werfen, denn nach der alten SchO gibt es dieses Sheet wirklich nicht

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

    /// <summary>
    /// Sucht einen Schüler und liefert die Zeile, in welcher seine SchülerId steht.
    /// </summary>
    /// <param name="schuelerId">Die Id des Schülers.</param>
    /// <returns>Die Zeile der SchuelerId in der Excel-Datei.</returns>
    public int GetSidZeileForSchueler(int schuelerId)
    {
      for (int i = CellConstant.zeileSIdErsterSchueler; i <= MaxAnzahlSchueler + CellConstant.zeileSIdErsterSchueler; i++)
      {
        string aValue = ReadValue(sid, CellConstant.SId + i);
        if (aValue != null)
        {
          int intValue = int.Parse(aValue);
          if (intValue.Equals(schuelerId))
          {
            return i;
          }
        }
      }

      throw new InvalidOperationException("Schüler nicht gefunden. Id " + schuelerId);
    }

    /// <summary>
    /// Liest eine ganzzahlige Note aus der angegebenen Zelle des Sheets
    /// </summary>
    /// <param name="zelle">Die Zelle.</param>
    /// <param name="sheet">Das Worksheet.</param>
    /// <returns></returns>
    public byte? ReadNote(string zelle, Worksheet sheet)
    {
      string v = ReadValue(sheet, zelle);
      return !string.IsNullOrEmpty(v) ? Convert.ToByte(v, CultureInfo.CurrentUICulture) : (byte?)null;
    }

    /// <summary>
    /// Liest eine Kommanote aus der angegebenen Zelle des Sheets
    /// </summary>
    /// <param name="zelle">Die Zelle.</param>
    /// <param name="sheet">Das Worksheet.</param>
    /// <returns></returns>
    public decimal? ReadKommanote(string zelle, Worksheet sheet)
    {
      string v = ReadValue(sheet, zelle);
      return !string.IsNullOrEmpty(v) ? Convert.ToDecimal(v, CultureInfo.CurrentUICulture) : (decimal?)null;
    }

    /// <summary>
    /// Prüft, ob die Datei nach der alten Schulordnung ausgelesen werden muss.
    /// </summary>
    /// <returns>true wenn für diese Datei die alte Schulordnung gelten muss.</returns>
    public bool IsAlteSchulordnung()
    {
      return ReadValue(sid, "F1") != "Neue";
    }

    protected int GetErsteFreieZeile(Excel.Worksheet sheet)
    {
      int zeile = 38;
      while (string.IsNullOrEmpty(ReadValue(sheet, CellConstant.SId + zeile)) && zeile >= 4)
      {
        zeile = zeile - 1;
      }

      return zeile + 1;
    }

    /// <summary>
    /// Blendet das Tabellenblatt mit dem übergebenen Namen aus.
    /// </summary>
    /// <param name="name">Name des Tabellenblattes.</param>
    public void HideWorksheet(string name)
    {
      Worksheet sheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals(name) select sh).FirstOrDefault();
      if (sheet != null)
      {
        sheet.Visible = XlSheetVisibility.xlSheetHidden;
      }
    }
  }

  public class OpenNotendatei: BasisNotendatei
  {
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="filename"></param>
    public OpenNotendatei(string filename) : base(filename)
    {
    }

    /// <summary>
    /// Hängt einen neuen Schüler unten an die Datei an.
    /// </summary>
    /// <param name="aSchueler">Der Schüler.</param>
    /// <param name="setzeLegasthenie">Ob der Legasthenievermerk geprüft werden soll.</param>
    public void AppendSchueler(diNoDataSet.SchuelerRow aSchueler, bool setzeLegasthenie)
    {
      UnsavedChanges = true;

      int zeile = GetErsteFreieZeile(sid); //gilt in Notenbogen und auf dem diNo-sid-Reiter
      WriteValueProtectedCell(notenbogen, "B" + zeile, aSchueler.Name + ", "+aSchueler.Rufname);
      WriteValueProtectedCell(sid, CellConstant.SId + zeile, aSchueler.Id.ToString());
      if (setzeLegasthenie && aSchueler.LRSStoerung)
      {
        // TODO: Legastenie: wie geht das neuerdings?
        //WriteValue(notenbogen, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
      }
    }

    /// <summary>
    /// Entfernt einen Schüler aus der Datei (nur Name). Lässt seine Noten aber stehen.
    /// </summary>
    /// <param name="schuelerId">die Id des Schülers.</param>
    public bool RemoveSchueler(int schuelerId)
    {
      int zeile = GetSidZeileForSchueler(schuelerId); //gilt für sId und Name
      string name = ReadValue(notenbogen, CellConstant.Nachname + zeile);
      if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(name.Trim()))
      {
        // Der Schüler ist bereits aus der Datei entfernt. Keine Aktion nötig.
        return false;
      }

      UnsavedChanges = true;
      WriteValueProtectedCell(notenbogen, CellConstant.Nachname + zeile, "");
      return true;
    }
  }

  /// <summary>
  /// Klasse, die eine Notendatei öffnet und Verweise auf die wichtigsten Sheets liefert
  /// </summary>
  public class OpenAlteNotendatei : BasisNotendatei
  {
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="filename"></param>
    public OpenAlteNotendatei(string filename) : base(filename)
    {
    }

    /// <summary>
    /// Hängt einen neuen Schüler unten an die Datei an.
    /// </summary>
    /// <param name="aSchueler">Der Schüler.</param>
    /// <param name="setzeLegasthenie">Ob der Legasthenievermerk geprüft werden soll.</param>
    public void AppendSchueler(diNoDataSet.SchuelerRow aSchueler, bool setzeLegasthenie)
    {
      UnsavedChanges = true;

      int zeileFuerSId = GetErsteFreieZeile(sid);
      int zeile = GetNotenbogenZeileForSidZeile(zeileFuerSId);

      WriteValueProtectedCell(notenbogen, CellConstant.Nachname + zeile, aSchueler.Name);
      WriteValueProtectedCell(notenbogen, CellConstant.Vorname + (zeile + 1), "   " + aSchueler.Rufname);
      WriteValueProtectedCell(sid, CellConstant.SId + zeileFuerSId, aSchueler.Id.ToString());
      if (setzeLegasthenie && aSchueler.LRSStoerung)
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
      RemoveSchueler(aSchueler.Id);
    }

    /// <summary>
    /// Entfernt einen Schüler aus der Datei (nur Name). Lässt seine Noten aber stehen.
    /// </summary>
    /// <param name="schuelerId">die Id des Schülers.</param>
    public bool RemoveSchueler(int schuelerId)
    {
      int zeileSId = GetSidZeileForSchueler(schuelerId);
      int zeileNachname = GetNotenbogenZeileForSidZeile(zeileSId);
      string name = ReadValue(notenbogen, CellConstant.Nachname + zeileNachname) + ReadValue(notenbogen, CellConstant.Vorname + (zeileNachname + 1));
      if (string.IsNullOrEmpty(name.Trim()))
      {
        // Der Schüler ist bereits aus der Datei entfernt. Keine Aktion nötig.
        return false;
      }

      UnsavedChanges = true;
      WriteValueProtectedCell(notenbogen, CellConstant.Nachname + zeileNachname, "");
      WriteValueProtectedCell(notenbogen, CellConstant.Vorname + (zeileNachname + 1), "");
      return true;
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

    private int GetNotenbogenZeileForSidZeile(int sidZeile)
    {
      int indexSchueler = sidZeile - 4; // Beginnend mit dem Nullten
      return 2 * indexSchueler + 5;
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
