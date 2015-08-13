using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{

  public class ErzeugeAlleExcelDateien
    {
        public ErzeugeAlleExcelDateien()
        {
            KursTableAdapter ta = new KursTableAdapter();
            var kurse = ta.GetData();
            foreach (var kurs in kurse)
            {
                new ErzeugeExcelDatei(kurs);
            }
        }        
    }

  /// <summary>
  /// Legt eine neue Exceldatei zum übergebenen Kurs an.
  /// </summary>
  public class ErzeugeExcelDatei
  {
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private OpenExcel xls;
    private Kurs kurs;
    private string fileName;
    private diNoDataSet.SchuelerDataTable alleSchueler;

        /// <summary>
        /// Aus dem übergebenen Kurs wird eine Exceldatei mit allen Schülerdaten generiert
        /// </summary>
        public ErzeugeExcelDatei(diNoDataSet.KursRow aKurs)
        {
            kurs = new Kurs(aKurs.Id);
            alleSchueler = kurs.getSchueler; // sind bereits via SQL nach Klasse und Namen sortiert

            if (alleSchueler.Count == 0)
            {
                log.WarnFormat("Der Kurs {0} hat keine Schueler ", kurs.Data.Bezeichnung);
                return;
            }

            if (string.IsNullOrEmpty(kurs.FachBezeichnung))
            {
                // ignoriere FPA, Seminare und ähnliche Platzhalter
                log.Debug("Erzeuge keine Datei für das Fach " + kurs.getFach.Kuerzel);
                return;
            }

            try
            {
                CopyExcelFile();
                xls = new OpenExcel(fileName);
                FillExcelFile();
                SwitchNotenschluessel();

                // speichere und schließe Datei
                xls.workbook.Save();
                xls.Dispose(); // Destruktor aufrufen
            }
            catch (Exception exp)
            {
                log.Fatal("Fehler beim Schreiben der Excel-Datei " + FileName, exp);
            }
        }

        /// <summary>
        /// Legt eine neue Exceldatei für diesen Kurs aus der Vorlage an
        /// </summary>
        private void CopyExcelFile()
        {
            string directoryName = Konstanten.ExcelPfad + "\\" + kurs.Data.LehrerRow.Kuerzel;
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }            
            fileName = directoryName + "\\" + kurs.Data.Bezeichnung.Replace('/', ' ') + ".xlsx";
            if (File.Exists(fileName))
            {
                File.Delete(fileName); // bisherige Datei löschen
            }

            // kopiere Vorlage
            if (kurs.Data.FachRow.Bezeichnung == "Englisch")
            {
                File.Copy(Konstanten.ExcelPfad + "\\Vorlage Englisch.xlsx", fileName);
            }
            else
            {
                File.Copy(Konstanten.ExcelPfad + "\\Vorlage.xlsx", fileName);
            }
        }

        /// <summary>
        /// Füllt die Daten des Kurses (Schülernamen, Klasse,...) in die Exceldatei
        /// </summary>
        private void FillExcelFile()
        {
            var klassen = new List<string>(); // sammelt alle Klassennamen dieses Kurses (z.B. für Ethik spannend)
            // Schulart, SA-Wertung wird dem ersten Schüler entnommen
            diNoDataSet.SchuelerRow ersterSchueler = alleSchueler[0]; // muss existieren, da nur Kurse mit Schülern erzeugt werden
            Schulart schulart = ersterSchueler.KlasseWinSV.StartsWith("B") ? Schulart.BOS : Schulart.FOS;
            Schulaufgabenwertung wertung = Faecherkanon.GetSchulaufgabenwertung(kurs.getFach, Faecherkanon.GetJahrgangsstufe(ersterSchueler.Jahrgangsstufe), Faecherkanon.GetZweig(ersterSchueler.Ausbildungsrichtung), schulart);

            // schreibe Notenbogen - Kopf
            xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.Wertungsart, GetWertungsString(wertung));
            xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.Fachbezeichnung, kurs.getFach.Bezeichnung);
            xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.Lehrer, kurs.Data.LehrerRow.Name);
            xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.Schuljahr, Konstanten.Schuljahr);
            xls.WriteValue(OpenExcel.Notensheets.sid, CellConstant.KursId, kurs.Id.ToString());

            int zeile = 5;
            int zeileFuerSId = CellConstant.zeileSIdErsterSchueler;

            foreach (var schueler in alleSchueler)
            {               
                string benutzterVorname = string.IsNullOrEmpty(schueler.Rufname) ? schueler.Vorname : schueler.Rufname;
                bool isLegastheniker = schueler.LRSStoerung || schueler.LRSSchwaeche;
                if (!klassen.Contains(schueler.KlasseWinSV))
                {
                    klassen.Add(schueler.KlasseWinSV);
                }

                // Schüler in die Exceldatei schreiben
                xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.Nachname + zeile, schueler.Name);
                xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.Vorname + (zeile + 1), "   " + benutzterVorname);
                xls.WriteValue(OpenExcel.Notensheets.sid, CellConstant.SId + zeileFuerSId, schueler.Id.ToString());
                if (schueler.LRSStoerung || schueler.LRSSchwaeche)
                {
                    xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
                }

                zeile += 2;
                zeileFuerSId++;
            }

            // Klassenbezeichnung wird aus allen Schülern gesammelt
            xls.WriteValue(OpenExcel.Notensheets.Notenbogen, CellConstant.Klassenbezeichnung, klassen.Aggregate((x, y) => x + y));
        }

    /// <summary>
    /// Trägt die korrekten Einstellungen für den Notenschlüssel eines Faches als Vorbelegung ins Excel-Sheet ein.
    /// </summary>
    private void SwitchNotenschluessel()
    {
            string schluessel, ug, og;
            switch (kurs.FachBezeichnung)
            {
                case "Englisch":
                    schluessel = "E";
                    ug = "34";
                    og = "49";
                    break;
                case "Betriebswirtschaftslehre":
                case "Wirtschaftsinformatik":
                case "Volkswirtschaftslehre":
                case "Wirtschaftslehre":
                case "Rechtslehre":
                    schluessel = "M";
                    ug = "30";
                    og = "44";
                    break;
                default:
                    schluessel = "M";
                    ug = "20";
                    og = "40";
                    break;
            }
            
            foreach (string sheetName in new[] { "I1SA", "I2SA", "I3SA", "I1Ext", "I2Ext", "I3Ext", "I4Ext", "II1SA", "II2SA", "II3SA", "II1Ext", "II2Ext", "II3Ext", "II4Ext" })
            {
                // Trage schon mal den zum Fach passenden Notenschlüssel und die Prozente ein
                // TODO: Abschlussprüfung auch noch!
                var pruefungssheet = xls.getSheet(sheetName);
                xls.WriteValue(pruefungssheet, CellConstant.SchluesselArt, schluessel);
                xls.WriteValue(pruefungssheet, CellConstant.ProzentFuenfUntergrenze, ug);
                xls.WriteValue(pruefungssheet, CellConstant.ProzentFuenfObergrenze, og);

            }
        }

    /// <summary>
    /// Liefert die möglichen Einstellungen zur Schulaufgabenwertung.
    /// </summary>
    /// <param name="wertung">Die geswünschte Wertung.</param>
    /// <returns>Der Text, den man in der Excel-Datei einstellen muss.</returns>
    public string GetWertungsString(Schulaufgabenwertung wertung)
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
    /// Der Dateiname.
    /// </summary>
    public string FileName
    {
      get;
      private set;
    }

        /*
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
    */
    }
}

