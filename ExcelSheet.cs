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
  /// Das Excel Sheet.
  /// </summary>
  public class ExcelSheet : IDisposable
  {
    /// <summary>
    /// Die Maximalanzahl von Schülern in einem ExcelSheet.
    /// </summary>
    private const int MaxAnzahlSchueler = 35;

    /// <summary>
    /// Referenz auf die Excel Application.
    /// </summary>
    private Excel.Application excelApp;

    /// <summary>
    /// Das Excel Workbook.
    /// </summary>
    private Excel.Workbook workbook;

    /// <summary>
    /// Die Klasse, die hier dargestellt wird.
    /// </summary>
    private Kurs myKurs;

    /// <summary>
    /// Der Logger.
    /// </summary>
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Reflection Missing object. Wird gebraucht, um Daten einzulesen.
    /// </summary>
    private static object missing = System.Reflection.Missing.Value;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="fileName">Dateiname.</param>
    public ExcelSheet(string fileName)
    {
      this.excelApp = new Excel.Application();
      this.FileName = fileName;

      this.Open();
    }

    public static void WriteExcelFile(Schulaufgabenwertung wertung, string fach, string lehrer, string schuljahr, string kursbezeichnung, IList<Schueler> schueler, string destFileName, int kursId)
    {
      // kopiere Vorlage
      if (fach == "Englisch")
      {
        File.Copy("C:\\Projects\\diNo\\Vorlage Englisch.xlsx", destFileName);
      }
      else
      {
        File.Copy("C:\\Projects\\diNo\\Vorlage.xlsx", destFileName);
      }

      // öffne Datei
      try
      {
        // starte Excel
        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
        excelApp.Visible = true;

        var workbook = excelApp.Workbooks.Open(destFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        var sheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("Notenbogen") select sh).FirstOrDefault();
        if (sheet == null)
        {
          throw new InvalidOperationException("kein Sheet mit dem Namen \"Notenbogen\" gefunden");
        }

        var sidsheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("diNo") select sh).FirstOrDefault();
        if (sidsheet == null)
        {
          throw new InvalidOperationException("kein Sheet mit dem Namen \"diNo\" gefunden");
        }

        // schreibe Notenbogen - Kopf
        WriteValue(sheet, CellConstant.Klassenbezeichnung, kursbezeichnung);
        WriteValue(sheet, CellConstant.Wertungsart, GetWertungsString(wertung));
        WriteValue(sheet, CellConstant.Fachbezeichnung, fach);
        WriteValue(sheet, CellConstant.Lehrer, lehrer);
        WriteValue(sheet, CellConstant.Schuljahr, schuljahr);

        WriteValue(sidsheet, CellConstant.KursId, kursId.ToString());

        // schreibe Schülerdaten
        int zeile = 5;
        int zeileFuerSId = CellConstant.zeileSIdErsterSchueler;
        
        List<Schueler> sortedList = new List<Schueler>(schueler);
        sortedList.Sort(CompareSchuelerByName);
        foreach (Schueler aSchueler in sortedList)
        {
          WriteValue(sheet, CellConstant.Nachname + zeile, aSchueler.Nachname);
          WriteValue(sheet, CellConstant.Vorname+(zeile+1), "   " + aSchueler.Vorname);
          WriteValue(sidsheet, CellConstant.SId + zeileFuerSId, aSchueler.Id.ToString());
          if (aSchueler.IsLegastheniker)
          {
            WriteValue(sheet, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
          }

          zeile += 2;
          zeileFuerSId++;
        }

        SwitchNotenschluessel(fach, workbook);

        // speichere und schließe Datei
        workbook.Save();
        workbook.Close(false, destFileName, Type.Missing);
        Marshal.ReleaseComObject(workbook);
        excelApp.Quit();
      }
      catch (Exception exp)
      {
        log.Fatal("Fehler beim Schreiben der Excel-Datei "+destFileName, exp);
      }
    }

    /// <summary>
    /// Setzt beim übergebenen Schüler den Legasthenievermerk auf den Wert, der im entsprechenden Attribut vermerkt ist.
    /// </summary>
    /// <param name="aSchueler">Der Schüler.</param>
    public void SetLegasthenieVermerk(Schueler aSchueler)
    {
      this.UnsavedChanges = true;
      //TODO: Methode ungetestet
      var zeile = this.Schueler.IndexOf(aSchueler);
      if (zeile < 0)
      {
        throw new InvalidOperationException("Schueler zum Setzen des Legasthenievermerks nicht gefunden");
      }

      var sheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals("Notenbogen") select sh).FirstOrDefault();
      if (sheet == null)
      {
        throw new InvalidOperationException("kein Sheet mit dem Namen \"Notenbogen\" gefunden");
      }

      zeile = CellConstant.ZeileErsterSchueler + (zeile + 1) * 2;
      if (aSchueler.IsLegastheniker)
      {
        WriteValue(sheet, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
      }
      else
      {
        WriteValue(sheet, CellConstant.LegasthenieVermerk + zeile, "");
      }

      this.workbook.Save();
      this.UnsavedChanges = false;
    }

    /// <summary>
    /// Hängt einen neuen Schüler unten an die Datei an.
    /// </summary>
    /// <param name="aSchueler">Der Schüler.</param>
    public void AppendSchueler(Schueler aSchueler)
    {
      this.UnsavedChanges = true;

      // TODO: Methode ungetestet
      var sheet = (from Excel.Worksheet sh in this.workbook.Worksheets where sh.Name.Equals("Notenbogen") select sh).FirstOrDefault();
      if (sheet == null)
      {
        throw new InvalidOperationException("kein Sheet mit dem Namen \"Notenbogen\" gefunden");
      }

      var sidsheet = (from Excel.Worksheet sh in this.workbook.Worksheets where sh.Name.Equals("diNo") select sh).FirstOrDefault();
      if (sidsheet == null)
      {
        throw new InvalidOperationException("kein Sheet mit dem Namen \"diNo\" gefunden");
      }

      int zeile = CellConstant.ZeileErsterSchueler + this.Schueler.Count * 2 + 1;
      int zeileFuerSId = CellConstant.zeileSIdErsterSchueler + this.Schueler.Count + 1;
      WriteValue(sheet, CellConstant.Nachname + zeile, aSchueler.Nachname);
      WriteValue(sheet, CellConstant.Vorname + (zeile + 1), "   " + aSchueler.Vorname);
      WriteValue(sidsheet, CellConstant.SId + zeileFuerSId, aSchueler.Id.ToString());
      if (aSchueler.IsLegastheniker)
      {
        WriteValue(sheet, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
      }

      this.workbook.Save();
      this.UnsavedChanges = false;
    }

    /// <summary>
    /// Trägt die korrekten Einstellungen für den Notenschlüssel eines Faches als Vorbelegung ins Excel-Sheet ein.
    /// </summary>
    /// <param name="fach">Der Name des Faches.</param>
    /// <param name="workbook">Das Workbook (=Excel-Datei).</param>
    private static void SwitchNotenschluessel(string fach, Workbook workbook)
    {
      foreach (string sheetName in new[] { "I1SA", "I2SA", "I3SA", "I1Ext", "I2Ext", "I3Ext", "I4Ext", "II1SA", "II2SA", "II3SA", "II1Ext", "II2Ext", "II3Ext", "II4Ext" })
      {
        // Trage schon mal den zum Fach passenden Notenschlüssel und die Prozente ein
        // TODO: Abschlussprüfung auch noch!
        var pruefungssheet = (from Excel.Worksheet sh in workbook.Worksheets where sh.Name.Equals(sheetName) select sh).FirstOrDefault();
        if (pruefungssheet == null)
        {
          throw new InvalidOperationException("kein Sheet mit dem Namen " + sheetName + " gefunden");
        }

        switch (fach)
        {
          case "Englisch":
            WriteValue(pruefungssheet, CellConstant.SchluesselArt, "E");
            WriteValue(pruefungssheet, CellConstant.ProzentFuenfUntergrenze, "34");
            WriteValue(pruefungssheet, CellConstant.ProzentFuenfObergrenze, "49");
            break;
          case "Betriebswirtschaftslehre":
          case "Wirtschaftsinformatik":
          case "Volkswirtschaftslehre":
          case "Wirtschaftslehre":
          case "Rechtslehre":
            WriteValue(pruefungssheet, CellConstant.SchluesselArt, "M");
            WriteValue(pruefungssheet, CellConstant.ProzentFuenfUntergrenze, "30");
            WriteValue(pruefungssheet, CellConstant.ProzentFuenfObergrenze, "44");
            break;
          default:
            WriteValue(pruefungssheet, CellConstant.SchluesselArt, "M");
            WriteValue(pruefungssheet, CellConstant.ProzentFuenfUntergrenze, "20");
            WriteValue(pruefungssheet, CellConstant.ProzentFuenfObergrenze, "40");
            break;
        }
      }
    }

    /// <summary>
    /// Vergleicht die Vor- und Nachnamen zweier Schüler. Wird zur Sortierung genutzt.
    /// </summary>
    /// <param name="x">Schüler x.</param>
    /// <param name="y">Schüler y.</param>
    /// <returns>Ergebnis des Vergleichs.</returns>
    private static int CompareSchuelerByName(Schueler x, Schueler y)
    {
      return (x.Nachname + x.Vorname).CompareTo(y.Nachname + y.Nachname);
    }

    /// <summary>
    /// Liefert die möglichen Einstellungen zur Schulaufgabenwertung.
    /// </summary>
    /// <param name="wertung">Die geswünschte Wertung.</param>
    /// <returns>Der Text, den man in der Excel-Datei einstellen muss.</returns>
    public static string GetWertungsString(Schulaufgabenwertung wertung)
    {
      if (wertung == Schulaufgabenwertung.ZweiZuEins)
        return "2:1-Fach";
      if (wertung == Schulaufgabenwertung.EinsZuEins)
        return "1:1-Fach";
      if (wertung == Schulaufgabenwertung.KurzarbeitenUndExen)
        return "KA / mdl.";

      throw new InvalidOperationException("unbekannte Schulaufgabenwertung " + wertung);
    }

    /// <summary>
    /// Öffnet die im Konstruktor übergebene Datei und liest die Kursdaten ein.
    /// </summary>
    public void Open()
    {
      try
      {
        this.workbook = excelApp.Workbooks.Open(this.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        this.myKurs = ReadData();
      }
      catch (Exception exp)
      {
        log.Fatal("Fehler beim Einlesen der Excel-Datei ", exp);
      }
    }

    /// <summary>
    /// Methode zum Lesen der Daten.
    /// </summary>
    /// <returns>Die Daten des Kurses.</returns>
    private Kurs ReadData()
    {
      var sheet = (from Excel.Worksheet sh in this.workbook.Worksheets where sh.Name.Equals("Notenbogen") select sh).FirstOrDefault();
      if (sheet == null)
      {
        throw new InvalidOperationException("kein Sheet mit dem Namen \"Notenbogen\" gefunden");
      }

      var sidsheet = (from Excel.Worksheet sh in this.workbook.Worksheets where sh.Name.Equals("Schulverwaltung") select sh).FirstOrDefault();
      if (sidsheet == null)
      {
        log.Error("kein Sheet mit dem Namen \"Schulverwaltung\" gefunden");
      }

      var apsheet = (from Excel.Worksheet sh in this.workbook.Worksheets where sh.Name.Equals("AP") select sh).FirstOrDefault();
      if (apsheet == null)
      {
        log.Error("kein Sheet mit dem Namen \"AP\" gefunden");
      }

      var kursbezeichnung = ReadValue(sheet, CellConstant.Klassenbezeichnung);
      var fach = ReadValue(sheet, CellConstant.Fachbezeichnung);
      var datumStand = ReadValue(sheet, CellConstant.DatumStand);
      var kursId = (sidsheet != null) ? int.Parse(ReadValue(sidsheet, CellConstant.KursId)) : -1;

      Kurs klasse = new Kurs(kursId, kursbezeichnung, fach);
      int indexAP = CellConstant.APZeileErsterSchueler;
      for (int i = CellConstant.ZeileErsterSchueler; i < 2 * MaxAnzahlSchueler; i = i + 2)
      {
        var schueler = ReadSchueler(sheet, sidsheet, i);
        //Prüfungsnoten
        schueler.BerechneteNoten.PruefungGesamt = ReadDecimalNote(apsheet, CellConstant.APgesamtSpalte + indexAP);
        schueler.BerechneteNoten.Abschlusszeugnis = ReadIntegerNote(apsheet, CellConstant.APZeugnisnote + indexAP);
        AddNoteToSchueler(schueler, apsheet, CellConstant.APschriftlichSpalte + indexAP, Notentyp.APSchriftlich, Halbjahr.Ohne);
        AddNoteToSchueler(schueler, apsheet, CellConstant.APmuendlichSpalte + indexAP, Notentyp.APMuendlich, Halbjahr.Ohne);
        indexAP++;

        if (schueler != null)
        {
          klasse.Schueler.Add(schueler);
        }
      }

      return klasse;
    }

    /// <summary>
    /// Liest die Daten eines Schülers ein.
    /// </summary>
    /// <param name="sheet">Das Sheet mit den Noten ("Notenbogen").</param>
    /// <param name="sidsheet">Das Sheet mit den dino-Daten ("diNo").</param>
    /// <param name="i">Index des Schülers.</param>
    /// <returns></returns>
    private static Schueler ReadSchueler(Worksheet sheet, Worksheet sidsheet, int i)
    {
      var nachname = ReadValue(sheet, CellConstant.Nachname + i);
      var vorname = ReadValue(sheet, CellConstant.Vorname + (i + 1));
      if (string.IsNullOrEmpty(nachname) && string.IsNullOrEmpty(vorname))
      {
        return null;
      }

      var stringSid = sidsheet != null ? ReadValue(sidsheet, CellConstant.SId + (CellConstant.zeileSIdErsterSchueler + i)) : string.Empty;
      int id = int.MaxValue;
      if (string.IsNullOrEmpty(stringSid))
      {
        log.Warn("Hier ist ein Schüler ohne SId: Sein Name ist " + nachname + ", " + vorname);
      }
      else
      {
        id = Convert.ToInt32(stringSid, CultureInfo.CurrentCulture);
      }

      var legasthenieVermerk = ReadValue(sheet, CellConstant.LegasthenieVermerk + i);
      bool isLegastheniker = legasthenieVermerk == CellConstant.LegasthenieEintragung;

      Schueler schueler = new Schueler(id, vorname, nachname, isLegastheniker);

      foreach (var zelle in CellConstant.SchulaufgabenErstesHJ)
      {
        ReadSchulaufgabe(sheet, i, schueler, zelle, Halbjahr.Erstes);
      }
      foreach (var zelle in CellConstant.SchulaufgabenZweitesHJ)
      {
        ReadSchulaufgabe(sheet, i, schueler, zelle, Halbjahr.Zweites);
      }

      foreach (var zelle in CellConstant.ExenErstesHJ)
      {
        ReadEx(sheet, i, schueler, zelle, Halbjahr.Erstes);
      }
      foreach (var zelle in CellConstant.ExenZweitesHJ)
      {
        ReadEx(sheet, i, schueler, zelle, Halbjahr.Zweites);
      }

      foreach (var zelle in CellConstant.MuendlicheErstesHJ)
      {
        AddNoteToSchueler(schueler, sheet, zelle + (i + 1), Notentyp.EchteMuendliche, Halbjahr.Erstes);
      }
      foreach (var zelle in CellConstant.MuendlicheErstesHJ.Union(CellConstant.MuendlicheZweitesHJ))
      {
        AddNoteToSchueler(schueler, sheet, zelle + (i + 1), Notentyp.EchteMuendliche, Halbjahr.Zweites);
      }

      // Ersatzprüfungen und Fachreferate
      AddNoteToSchueler(schueler, sheet, CellConstant.EPErstesHJ + (i + 1), Notentyp.Ersatzprüfung, Halbjahr.Erstes);
      AddNoteToSchueler(schueler, sheet, CellConstant.EPZweitesHJ + (i + 1), Notentyp.Ersatzprüfung, Halbjahr.Zweites);
      AddNoteToSchueler(schueler, sheet, CellConstant.FachreferatErstesHJ + (i + 1), Notentyp.Fachreferat, Halbjahr.Erstes);
      AddNoteToSchueler(schueler, sheet, CellConstant.FachreferatZweitesHJ + (i + 1), Notentyp.Fachreferat, Halbjahr.Zweites);

      // Durchschnitte und Jahresfortgangsnoten
      schueler.BerechneteNotenErstesHalbjahr.SchnittSchulaufgaben = ReadDecimalNote(sheet, CellConstant.SchnittSchulaufgabenErstesHJ + (i + 1));
      schueler.BerechneteNotenErstesHalbjahr.SchnittMuendlich = ReadDecimalNote(sheet, CellConstant.SchnittMuendlicheUndExenErstesHJ + (i + 1));
      schueler.BerechneteNotenErstesHalbjahr.JahresfortgangGanzzahlig = ReadIntegerNote(sheet, CellConstant.ZeugnisnoteErstesHJ + (i));
      schueler.BerechneteNotenErstesHalbjahr.JahresfortgangMitKomma = ReadDecimalNote(sheet, CellConstant.ZeugnisnoteErstesHJ + (i + 1));

      schueler.BerechneteNoten.SchnittSchulaufgaben = ReadDecimalNote(sheet, CellConstant.SchnittSchulaufgabenZweitesHJ + (i + 1));
      schueler.BerechneteNoten.SchnittMuendlich = ReadDecimalNote(sheet, CellConstant.SchnittMuendlicheUndExenZweitesHJ + (i + 1));
      schueler.BerechneteNoten.JahresfortgangGanzzahlig = ReadIntegerNote(sheet, CellConstant.ZeugnisnoteZweitesHJ + (i));
      schueler.BerechneteNoten.JahresfortgangMitKomma = ReadDecimalNote(sheet, CellConstant.ZeugnisnoteZweitesHJ + (i + 1));

      return schueler;
    }

    /// <summary>
    /// Liest eine Ex oder Kurzarbeitsnote ein.
    /// </summary>
    /// <param name="sheet">Der Notenbogen.</param>
    /// <param name="i">Der Index des Schülers.</param>
    /// <param name="schueler">Der Schüler, zu welchem die Note gehört.</param>
    /// <param name="zelle">Der Buchstabe der Zelle, in welcher die Note steht.</param>
    /// <param name="halbjahr">Das Halbjahr, zu dem die Note gehört.</param>
    private static void ReadEx(Worksheet sheet, int i, Schueler schueler, string zelle, Halbjahr halbjahr)
    {
      var stringGewichtung = ReadValue(sheet, zelle + CellConstant.GewichteExen);
      int gewichtung = string.IsNullOrEmpty(stringGewichtung) ? 1 : Convert.ToInt32(stringGewichtung);
      if (gewichtung != 1 && gewichtung != 2)
      {
        log.Error("Fand eine Gewichtung von " + gewichtung + ". Bei Exen und Kurzarbeiten erkennen wir nur 1 oder 2 an!");
      }

      var notentyp = gewichtung == 1 ? Notentyp.Ex : Notentyp.Kurzarbeit;
      AddNoteToSchueler(schueler, sheet, zelle + (i + 1), notentyp, halbjahr);
    }

    /// <summary>
    /// Liest eine Schulaufgabennote ein.
    /// </summary>
    /// <param name="sheet">Der Notenbogen.</param>
    /// <param name="i">Der Index des Schülers.</param>
    /// <param name="schueler">Der Schüler, zu welchem die Note gehört.</param>
    /// <param name="zelle">Der Buchstabe der Zelle, in welcher die Note steht.</param>
    /// <param name="halbjahr">Das Halbjahr, zu dem die Note gehört.</param>
    private static void ReadSchulaufgabe(Worksheet sheet, int i, Schueler schueler, string zelle, Halbjahr halbjahr)
    {
      var stringGewichtung = ReadValue(sheet, zelle + CellConstant.GewichteSchulaufgaben);
      int gewichtung = string.IsNullOrEmpty(stringGewichtung) ? 1 : Convert.ToInt32(stringGewichtung);
      if (gewichtung != 1)
      {
        log.Error("Fand eine Gewichtung von " + gewichtung + ". Dies wird aktuell bei Schulaufgaben nicht unterstützt!");
      }

      AddNoteToSchueler(schueler, sheet, zelle + i, Notentyp.Schulaufgabe, halbjahr);
    }

    /// <summary>
    /// Ordnet eine Note einem Schueler zu.
    /// </summary>
    /// <param name="schueler">Der Schueler.</param>
    /// <param name="sheet">Das Worksheet, aus welchem die Note gelesen werden soll.</param>
    /// <param name="zelle">Die Zelle, in welcher die Note steht.</param>
    /// <param name="notentyp">Der Typ der Note.</param>
    /// <param name="halbjahr">Das Halbjahr.</param>
    private static void AddNoteToSchueler(Schueler schueler, Excel.Worksheet sheet, string zelle, Notentyp notentyp, Halbjahr halbjahr)
    {
      var punkte = ReadIntegerNote(sheet, zelle);
      if (punkte != null)
      {
        schueler.Einzelnoten.Add(new Note() { Punktwert = (byte)punkte, Typ = notentyp, Zelle = zelle, Halbjahr = halbjahr });
      }
    }

    private static byte? ReadIntegerNote(Excel.Worksheet sheet, string zelle)
    {
      var stringValue = ReadValue(sheet, zelle);
      return !string.IsNullOrEmpty(stringValue) ? Convert.ToByte(stringValue, CultureInfo.CurrentUICulture) : (byte?)null;
    }

    private static decimal? ReadDecimalNote(Excel.Worksheet sheet, string zelle)
    {
      var stringValue = ReadValue(sheet, zelle);
      return !string.IsNullOrEmpty(stringValue) ? Convert.ToDecimal(stringValue, CultureInfo.CurrentUICulture) : (decimal?)null;
    }

    /// <summary>
    /// Liest einen Wert aus einer Zelle des gegebenen ExcelSheets.
    /// </summary>
    /// <param name="sheet">Das Excel Sheet.</param>
    /// <param name="zelle">Die Zelle, z. B. A2.</param>
    /// <returns>Den Wert der Zelle als String.</returns>
    private static string ReadValue(Excel.Worksheet sheet, string zelle)
    {
      Excel.Range r = sheet.get_Range(zelle, missing);
      return r.Value2 == null ? null : Convert.ToString(r.Value2).Trim();
    }

    /// <summary>
    /// Schreibt einen Wert in die Zelle des gegebenen Excel-Sheets
    /// </summary>
    /// <param name="sheet">Das Excel Sheet.</param>
    /// <param name="zelle">Die Zelle, z. B. A2.</param>
    /// <param name="value">Den Wert der Zelle als String.</param>
    private static void WriteValue(Excel.Worksheet sheet, string zelle, string value)
    {
      Excel.Range r = sheet.get_Range(zelle, missing);
      r.Value2 = value;
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
    /// Die enthaltenen Schueler.
    /// </summary>
    public IList<Schueler> Schueler
    {
      get
      {
        return this.myKurs != null ? this.myKurs.Schueler : new List<Schueler>();
      }
    }

    /// <summary>
    /// Die Kursbezeichnung.
    /// </summary>
    public string Kursbezeichnung
    { 
      get
      {
        return this.myKurs != null ? this.myKurs.Name : string.Empty;
      }
    }

    /// <summary>
    /// Die Fachbezeichnung.
    /// </summary>
    public string Fachname
    {
      get
      {
        return this.myKurs != null ? this.myKurs.Fach : string.Empty;
      }
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
