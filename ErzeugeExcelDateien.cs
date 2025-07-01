using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace diNo
{
  /// <summary>
  /// Delegat für Statusänderungsmeldungen.
  /// </summary>
  /// <param name="sender">Der Sender.</param>
  /// <param name="eventArgs">Die Event Args (hauptsächlich die Meldung).</param>
  public delegate void StatusChanged(object sender, StatusChangedEventArgs eventArgs);

  /// <summary>
  /// Klasse zum Erzeugen der Excel-Dateien.
  /// </summary>
  public class ErzeugeAlleExcelDateien
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="statusChangedHandler">Handler für Statusmeldungen. Kann auch null sein.</param>
    public ErzeugeAlleExcelDateien(StatusChanged statusChangedHandler)
    {
      KursTableAdapter ta = new KursTableAdapter();
      var kurse = ta.GetData();
      int count = 0;

      foreach (var kurs in kurse)
      {
        Kurs derKurs = new Kurs(kurs);

        if (!kurs.IsLehrerIdNull())
        {
          statusChangedHandler?.Invoke(this, new StatusChangedEventArgs() { Meldung = "Erzeuge Datei " + count + " von " + kurse.Count });
          new ErzeugeNeueExcelDatei(derKurs.Data);
          count++;
        }
      }

      statusChangedHandler?.Invoke(this, new StatusChangedEventArgs() { Meldung = count + " Dateien erfolgreich erzeugt" });
    }
  }
  
  /// <summary>
  /// Legt eine neue Exceldatei zum übergebenen Kurs an.
  /// </summary>
  public class ErzeugeNeueExcelDatei : IDisposable
  {
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private OpenNotendatei xls;
    private Kurs kurs;
    private string fileName;
    private List<Schueler> alleSchueler;

    /// <summary>
    /// Aus dem übergebenen Kurs wird eine Exceldatei mit allen Schülerdaten generiert
    /// </summary>
    public ErzeugeNeueExcelDatei(diNoDataSet.KursRow aKurs)
    {
      kurs = new Kurs(aKurs);

      if (kurs.getLehrer == null)
      {
        return; // es gibt auch Kurse ohne Lehrer, z. B. übernommene Noten aus 11ter Klasse
      }

      alleSchueler = kurs.Schueler;
      alleSchueler.Sort((x, y) => (x.Name + x.Vorname).CompareTo(y.Name + y.Vorname));

      if (alleSchueler.Count == 0)
      {
        log.WarnFormat("Der Kurs {0} hat keine Schueler ", kurs.Data.Bezeichnung);
        return;
      }

      if (alleSchueler.Count > BasisNotendatei.MaxAnzahlSchueler)
      {
        throw new InvalidOperationException("zu viele Schüler " + alleSchueler.Count);
      }

      if (string.IsNullOrEmpty(kurs.FachBezeichnung) || kurs.getFach.Typ == FachTyp.OhneNoten)
      {
        // ignoriere FPA, Seminare und ähnliche Platzhalter
        log.Debug("Erzeuge keine Datei für das Fach " + kurs.getFach.Kuerzel);
        return;
      }

      CopyExcelFile();

      xls = new OpenNotendatei(fileName);

      FillExcelFile();
      SwitchNotenschluessel();

      // speichere und schließe Datei
      xls.workbook.Save();
      xls.Dispose(); // Destruktor aufrufen
      xls = null;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (xls != null)
        {
          xls.Dispose();
          xls = null;
        }
      }
    }

    /// <summary>
    /// Legt eine neue Exceldatei für diesen Kurs aus der Vorlage an
    /// </summary>
    private void CopyExcelFile()
    {
      string directoryName = Zugriff.Instance.getString(GlobaleStrings.VerzeichnisExceldateien) + kurs.getLehrer.Kuerzel;
      if (!Directory.Exists(directoryName))
      {
        Directory.CreateDirectory(directoryName);
      }
      fileName = directoryName + "\\" + kurs.Data.Bezeichnung.Replace("/", "") + ".xlsx";
      if (File.Exists(fileName))
      {
        File.Delete(fileName); // bisherige Datei löschen
      }

      File.Copy(Zugriff.Instance.getString(GlobaleStrings.VerzeichnisExceldateien) + "Vorlage.xlsx", fileName);
    }

    /// <summary>
    /// Füllt die Daten des Kurses (Schülernamen, Klasse,...) in die Exceldatei
    /// </summary>
    private void FillExcelFile()
    {
      var klassen = new List<string>(); // sammelt alle Klassennamen dieses Kurses (z.B. für Ethik spannend)
                                        // Schulart, SA-Wertung wird dem ersten Schüler entnommen
     
      // schreibe Notenbogen - Kopf
      xls.WriteValue(xls.notenbogen, "E1", kurs.getFach.Bezeichnung);
      xls.WriteValueProtectedCell(xls.notenbogen, "I1", GetLehrerOderLehrerin(kurs));
      xls.WriteValue(xls.notenbogen, "K1", kurs.getLehrer.Name);
      xls.WriteValueProtectedCell(xls.AP, "B1", "Abschlussprüfung " + (Zugriff.Instance.Schuljahr + 1));
      xls.WriteValueProtectedCell(xls.sid, "F2", kurs.Id.ToString());

      int zeile = 4;
      int zeileFuerSId = CellConstant.zeileSIdErsterSchueler;

      foreach (var schueler in alleSchueler)
      {        
        if (!klassen.Contains(schueler.getKlasse.Data.Bezeichnung))
        {
          klassen.Add(schueler.getKlasse.Data.Bezeichnung);
        }

        // Schüler in die Exceldatei schreiben
        xls.WriteValueProtectedCell(xls.notenbogen, CellConstant.Nachname + zeile, schueler.Data.Name + ", " + schueler.benutzterVorname);
        xls.WriteValueProtectedCell(xls.sid, CellConstant.SId + zeileFuerSId, schueler.Id.ToString());

        zeile++;
        zeileFuerSId++;
      }

      // Klassenbezeichnung wird aus allen Schülern gesammelt
      xls.WriteValue(xls.notenbogen, "B1", klassen.Aggregate((x, y) => x + ", " + y));

      if (kurs.getFach.Kuerzel == "E")
      {
        xls.HideWorksheet("APRohpunkte");
      }
      else
      {
        xls.HideWorksheet("Eingabe Abitur");
        xls.HideWorksheet("Ausdruck MAP");
      }
    }

    /// <summary>
    /// Methode dient zur Zufriedenstellung der Frauenbauftragten :-)
    /// </summary>
    /// <param name="kurs">Der Kurs.</param>
    /// <returns>Den Text Lehrer oder Lehrerin.</returns>
    private string GetLehrerOderLehrerin(Kurs kurs)
    {
      if (kurs.getLehrer != null)
      {
        if (kurs.getLehrer.Data.Geschlecht == "W")
          return "Lehrerin:";
      }
      return "Lehrer:";
    }

    /// <summary>
    /// Trägt die korrekten Einstellungen für den Notenschlüssel eines Faches als Vorbelegung ins Excel-Sheet ein.
    /// </summary>
    private void SwitchNotenschluessel()
    {
      string schluessel, ug, og, eingabe = "BE";

      switch (kurs.getFach.Kuerzel)
      {
        case "E":
        case "EBC": //English Book Club
        case "Sp":
        case "F":
        case "F-f":
          schluessel = "E";
          ug = "34";
          og = "49";
          break;
        case "RSw": //Sozialwirtschaft und Recht
        case "BwR":
        case "WIn":
        case "VWL":
        case "Wl":
        case "Rl":
        case "Inf": //Informatik im Wirtschaftszweig
        case "InfW_SU": // Informatik Wahlfach für ABU, Soziale (für InfW_T gilt der Matheschlüssel)
        case "WAk": // Wirtschaft aktuell
        case "WR": // Wirtschaft und Recht
        case "IBS": // International Business Studies
          schluessel = "M";
          ug = "30";
          og = "44";
          break;
        case "D":
        case "PP":
          schluessel = "M";
          ug = "20";
          og = "40";
          eingabe = "Punkte";
          xls.WriteValue(xls.AP, "E42", eingabe); // Sonderfall AP in D und PP
          break;
        default:
          schluessel = "M";
          ug = "20";
          og = "40";
          break;
      }

      foreach (string sheetName in new[] { "I1SA", "I2SA", "I1KA", "I2KA", "I1Ext", "I2Ext", "I3Ext", "II1SA", "II2SA", "II1KA", "II2KA", "II1Ext", "II2Ext", "II3Ext" })
      {
        // Trage schon mal den zum Fach passenden Notenschlüssel und die Prozente ein
        var pruefungssheet = xls.getSheet(sheetName);
        xls.WriteValue(pruefungssheet, CellConstant.SchluesselArt, schluessel);
        xls.WriteValue(pruefungssheet, CellConstant.ProzentFuenfUntergrenze, ug);
        xls.WriteValue(pruefungssheet, CellConstant.ProzentFuenfObergrenze, og);
        xls.WriteValue(pruefungssheet, CellConstant.EingabeUeber, eingabe);
      }

        try
        {
            if (kurs.Klassen[0].Jahrgangsstufe == Jahrgangsstufe.IntVk)
            {
                xls.WriteValue(xls.notenbogen2, "M39", "2");  // 2. SA im 2. Hj zählt doppelt
            }
        }
        catch
        { }
    }
  }
}

