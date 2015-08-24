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
                if (kurs.Id>824 && kurs.Id<840)
                //if (kurs.Id==851)
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
    private OpenNotendatei xls;
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

//           try
            {
                CopyExcelFile();
                xls = new OpenNotendatei(fileName);
                
                FillExcelFile();
                SwitchNotenschluessel();

                // speichere und schließe Datei
                xls.workbook.Save();
                xls.Dispose(); // Destruktor aufrufen
            }
            /*catch (Exception exp)
            {
                log.Fatal("Fehler beim Schreiben der Excel-Datei " + fileName, exp);
            }
            */
        }

        /// <summary>
        /// Legt eine neue Exceldatei für diesen Kurs aus der Vorlage an
        /// </summary>
        private void CopyExcelFile()
        {
            string directoryName = Konstanten.ExcelPfad + "\\" + kurs.getLehrer.Kuerzel;
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
            if (kurs.FachBezeichnung == "Englisch")
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
            Schulaufgabenwertung wertung = kurs.getFach.GetSchulaufgabenwertung(new Klasse(ersterSchueler.KlasseId));

            // schreibe Notenbogen - Kopf
            xls.WriteValue(xls.notenbogen, CellConstant.Wertungsart, GetWertungsString(wertung));
            xls.WriteValue(xls.notenbogen, CellConstant.Fachbezeichnung, kurs.getFach.Bezeichnung);
            xls.WriteValue(xls.notenbogen, CellConstant.Lehrer, kurs.getLehrer.Name);
            xls.WriteValue(xls.notenbogen, CellConstant.Schuljahr, Konstanten.Schuljahr);
            xls.WriteValue(xls.sid, CellConstant.KursId, kurs.Id.ToString());

            int zeile = 5;
            int zeileFuerSId = CellConstant.zeileSIdErsterSchueler;

            foreach (var s in alleSchueler)
            {
                Schueler schueler = new Schueler(s.Id);
                
                
                if (!klassen.Contains(schueler.getKlasse.Data.Bezeichnung))
                {
                    klassen.Add(schueler.getKlasse.Data.Bezeichnung);
                }

                // Schüler in die Exceldatei schreiben
                xls.WriteValue(xls.notenbogen, CellConstant.Nachname + zeile, schueler.Data.Name);
                xls.WriteValue(xls.notenbogen, CellConstant.Vorname + (zeile + 1), "   " + schueler.benutzterVorname);
                xls.WriteValue(xls.sid, CellConstant.SId + zeileFuerSId, schueler.Id.ToString());
                if (schueler.IsLegastheniker)
                {
                    xls.WriteValue(xls.notenbogen, CellConstant.LegasthenieVermerk + zeile, CellConstant.LegasthenieEintragung);
                }

                zeile += 2;
                zeileFuerSId++;
            }

            // Klassenbezeichnung wird aus allen Schülern gesammelt
            xls.WriteValue(xls.notenbogen, CellConstant.Klassenbezeichnung, klassen.Aggregate((x, y) => x + ", " + y));
        }

    /// <summary>
    /// Trägt die korrekten Einstellungen für den Notenschlüssel eines Faches als Vorbelegung ins Excel-Sheet ein.
    /// </summary>
    private void SwitchNotenschluessel()
    {
            string schluessel, ug, og;
            switch (kurs.getFach.Kuerzel)
            {
                case "E":
                    schluessel = "E";
                    ug = "34";
                    og = "49";
                    break;
                case "BwR":
                case "WIn":
                case "VWL":
                case "Wl":
                case "Rl":
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

    }
}

