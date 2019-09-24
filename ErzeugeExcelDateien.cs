﻿using diNo.diNoDataSetTableAdapters;
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
          statusChangedHandler(this, new StatusChangedEventArgs() { Meldung = "Erzeuge Datei " + count + " von " + kurse.Count });
          count++;
          var alleSchueler = derKurs.getSchueler(true); // sind bereits via SQL nach Klasse und Namen sortiert
          Jahrgangsstufe jgStufe = Jahrgangsstufe.Elf;
          if (alleSchueler.Count > 0)
          {
            Schueler ersterSchueler = new Schueler(alleSchueler[0]);
            jgStufe = ersterSchueler.getKlasse.Jahrgangsstufe;
          }

          new ErzeugeNeueExcelDatei(kurs);
        }
      }

      statusChangedHandler(this, new StatusChangedEventArgs() { Meldung = count + " Dateien erfolgreich erzeugt" });      
    }
  }

  /// <summary>
  /// Klasse zum automatisierten verschicken der Excel-Dateien an alle Lehrer.
  /// </summary>
  public class SendExcelMails
  {
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private string bodyText;
    private MailAddress from = new MailAddress(Zugriff.Instance.getString(GlobaleStrings.SendExcelViaMail), "Digitale Notenverwaltung");
    private SmtpClient mailServer;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="statusChangedHandler">Handler für Statusmeldungen. Kann auch null sein.</param>
    public SendExcelMails(StatusChanged statusChangedHandler)
    {
      try
      {
        mailServer = new SmtpClient(Zugriff.Instance.getString(GlobaleStrings.SMTP), int.Parse(Zugriff.Instance.getString(GlobaleStrings.Port)));
        mailServer.EnableSsl = true;
        mailServer.UseDefaultCredentials = false;
        mailServer.Credentials = new System.Net.NetworkCredential(Zugriff.Instance.getString(GlobaleStrings.SendExcelViaMail), Zugriff.Instance.getString(GlobaleStrings.MailPasswort));        
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "diNo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }


      /*
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Textdatei wählen, die Nachrichtentext enthält";
      if (dia.ShowDialog() != DialogResult.OK) return;

      dia.FileName // auslesen 
      bodyText = ;
      */

      LehrerTableAdapter ada = new LehrerTableAdapter();
      var rows = ada.GetData();
      int count = 0;
      foreach (diNoDataSet.LehrerRow row in rows)
      {        
        count++;
        statusChangedHandler(this, new StatusChangedEventArgs() { Meldung = count + " von " + rows.Count + " gesendet: " + row.Kuerzel });

        string directoryName = Konstanten.ExcelPfad + row.Kuerzel;
        if (!Directory.Exists(directoryName) || Directory.GetFiles(directoryName).Count() == 0)
        {
          log.Warn("Unterrichtet der Lehrer " + row.Kuerzel + " nix ?");
          count++;
          continue;
        }

        string dienstlicheMailAdresse = row.IsEMailNull() ? "" : row.EMail;
        if (!string.IsNullOrEmpty(dienstlicheMailAdresse))
        {
          SendMail(dienstlicheMailAdresse, Directory.GetFiles(directoryName));
        }
      }
    }

    /// <summary>
    /// Schickt eine Mail mit den übergebenen Excel-Files.
    /// </summary>
    /// <param name="to">Der Empfänger.</param>
    /// <param name="fileNames">Die Dateinamen.</param>
    private void SendMail(string to, IEnumerable<string> fileNames)
    {
      try
      {
        string subject = "Notentabellen";
        string body =
@"Liebe Kolleginnen und Kollegen,
 
diese Nachricht wurde maschinell von unserer digitalen Notenverwaltung diNo erzeugt.

Im Anhang finden Sie die Excel-Notenlisten für das kommende Schuljahr. Ich habe die Dateien stichprobenhaft auf Plausibilität geprüft.
Prüfen Sie dennoch bitte 
- ob es sich um Ihre Kurse handelt und die Schülerliste vollständig ist (insbesondere bei Wahlpflichtfächern, Religionskursen und Mischklassen)
- ob die Einstellungen in der Datei korrekt sind (z. B. Lehrername, Schulaufgabenwertung und ähnliche Eintragungen)
- ob sich sonstige offensichtliche Fehler, z. B. beim Notenschlüssel eingeschlichen haben

Bei Problemen oder Fragen bitte ich um eine Nachricht.          

Bei allen Kursen, die von Tandems unterrichtet werden (v. a. Vorklassen) ist der Kurs momentan dem ersten Lehrer aus UNTIS zugeordnet. Meistens ist das auch die/derjenige mit den meisten Stunden, aber leider nicht immer.
Dieser Lehrkraft werden auch die Notendateien übersandt. Sollten Sie hierbei eine Änderung wünschen bitte ich um eine kurze Rückmeldung.

Die neuen Kolleginnen und Kollegen würde ich bitten, sich in den nächsten Tagen mal anzumelden und somit ihren Zugang zu testen.

Viele Grüße
Claus Konrad und Markus Siegel";


        MailMessage msg = new MailMessage();
        msg.From = from;
        msg.To.Add(new MailAddress(to));
        msg.Subject = subject;
        msg.Body = body;
        foreach (string fileName in fileNames)
        {
          msg.Attachments.Add(new Attachment(fileName));
        }
        
        mailServer.Send(msg);
      }
      catch (Exception ex)
      {
        if (MessageBox.Show(ex.Message, "diNo", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
          throw;
      }
    }
  }


  /// <summary>
  /// Legt eine neue Exceldatei zum übergebenen Kurs an.
  /// </summary>
  public class ErzeugeNeueExcelDatei
  {
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private OpenNotendatei xls;
    private Kurs kurs;
    private string fileName;
    private IList<diNoDataSet.SchuelerRow> alleSchueler;

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

      List<diNoDataSet.SchuelerRow> dieSchueler= new List<diNoDataSet.SchuelerRow>(kurs.getSchueler(true));
      dieSchueler.Sort((x, y) => (x.Name + x.Vorname).CompareTo(y.Name + y.Vorname));
      alleSchueler = dieSchueler;
      

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

    /// <summary>
    /// Legt eine neue Exceldatei für diesen Kurs aus der Vorlage an
    /// </summary>
    private void CopyExcelFile()
    {
      string directoryName = Konstanten.ExcelPfad + kurs.getLehrer.Kuerzel;
      if (!Directory.Exists(directoryName))
      {
        Directory.CreateDirectory(directoryName);
      }
      fileName = directoryName + "\\" + kurs.Data.Bezeichnung.Replace('/', ' ') + ".xlsx";
      if (File.Exists(fileName))
      {
        File.Delete(fileName); // bisherige Datei löschen
      }

      File.Copy(Konstanten.ExcelPfad + "\\Vorlage.xlsx", fileName);
    }

    /// <summary>
    /// Füllt die Daten des Kurses (Schülernamen, Klasse,...) in die Exceldatei
    /// </summary>
    private void FillExcelFile()
    {
      var klassen = new List<string>(); // sammelt alle Klassennamen dieses Kurses (z.B. für Ethik spannend)
                                        // Schulart, SA-Wertung wird dem ersten Schüler entnommen

      Schueler ersterSchueler = new Schueler(alleSchueler[0]); // muss existieren, da nur Kurse mit Schülern erzeugt werden

      Schulart schulart = ersterSchueler.getKlasse.Schulart;
      Schulaufgabenwertung wertung = kurs.getFach.GetSchulaufgabenwertung(ersterSchueler.Zweig, ersterSchueler.getKlasse.Jahrgangsstufe);

      // schreibe Notenbogen - Kopf
      xls.WriteValue(xls.notenbogen, "E1", kurs.getFach.Bezeichnung);
      xls.WriteValueProtectedCell(xls.notenbogen, "I1", GetLehrerOderLehrerin(kurs));
      xls.WriteValue(xls.notenbogen, "K1", kurs.getLehrer.Name);
      xls.WriteValueProtectedCell(xls.AP, "B1", "Abschlussprüfung " +  (Zugriff.Instance.Schuljahr+1));
      xls.WriteValueProtectedCell(xls.sid, "F2", kurs.Id.ToString());

      int zeile = 4;
      int zeileFuerSId = CellConstant.zeileSIdErsterSchueler;

      foreach (var s in alleSchueler)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(s.Id);

        if (!klassen.Contains(schueler.getKlasse.Data.Bezeichnung))
        {
          klassen.Add(schueler.getKlasse.Data.Bezeichnung);
        }

        // Schüler in die Exceldatei schreiben
        xls.WriteValueProtectedCell(xls.notenbogen, CellConstant.Nachname + zeile, schueler.Data.Name+", "+ schueler.benutzterVorname);
        xls.WriteValueProtectedCell(xls.sid, CellConstant.SId + zeileFuerSId, schueler.Id.ToString());
        //xls.WriteValueProtectedCell(xls.sid, CellConstant.Regelung + zeileFuerSId, schueler.hatVorHj ? "FOS" : "BOS");

        zeile ++;
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
        if (kurs.getLehrer.Data.Geschlecht=="W")
           return "Lehrerin:";
      }
      return "Lehrer:";
    }

    /// <summary>
    /// Trägt die korrekten Einstellungen für den Notenschlüssel eines Faches als Vorbelegung ins Excel-Sheet ein.
    /// </summary>
    private void SwitchNotenschluessel()
    {
      string schluessel, ug, og, eingabe="BE";

      switch (kurs.getFach.Kuerzel)
      {
        case "E":
        case "EBC": //English Book Club
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
        case "Inf": //Informatik für Sozial-13
        case "Inf_W_AS": // Informatik Wahlfach für ABU, Soziale (für Inf_T gilt der Matheschlüssel)
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
    }
  }


  /// <summary>
  /// Legt eine neue Exceldatei zum übergebenen Kurs an.
  /// </summary>
  public class ErzeugeAlteExcelDatei
  {
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private OpenAlteNotendatei xls;
    private Kurs kurs;
    private string fileName;
    private IList<diNoDataSet.SchuelerRow> alleSchueler;

    /// <summary>
    /// Aus dem übergebenen Kurs wird eine Exceldatei mit allen Schülerdaten generiert
    /// </summary>
    public ErzeugeAlteExcelDatei(diNoDataSet.KursRow aKurs)
    {
      kurs = new Kurs(aKurs);

      if (kurs.getLehrer == null)
      {
        return; // es gibt auch Kurse ohne Lehrer, z. B. übernommene Noten aus 11ter Klasse
      }

      alleSchueler = kurs.getSchueler(true); // sind bereits via SQL nach Klasse und Namen sortiert

      if (alleSchueler.Count == 0)
      {
        log.WarnFormat("Der Kurs {0} hat keine Schueler ", kurs.Data.Bezeichnung);
        return;
      }

      if (alleSchueler.Count > BasisNotendatei.MaxAnzahlSchueler)
      {
        throw new InvalidOperationException("zu viele Schüler " + alleSchueler.Count);
      }

      if (string.IsNullOrEmpty(kurs.FachBezeichnung))
      {
        // ignoriere FPA, Seminare und ähnliche Platzhalter
        log.Debug("Erzeuge keine Datei für das Fach " + kurs.getFach.Kuerzel);
        return;
      }

      CopyExcelFile();

      xls = new OpenAlteNotendatei(fileName);

      FillExcelFile();
      SwitchNotenschluessel();

      // speichere und schließe Datei
      xls.workbook.Save();
      xls.Dispose(); // Destruktor aufrufen
      xls = null;
    }

    /// <summary>
    /// Legt eine neue Exceldatei für diesen Kurs aus der Vorlage an
    /// </summary>
    private void CopyExcelFile()
    {
      string directoryName = Konstanten.ExcelPfad + kurs.getLehrer.Kuerzel;
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
        File.Copy(Konstanten.ExcelPfad + "\\Vorlage Englisch Alte Schulordnung.xlsx", fileName);
      }
      else
      {
        File.Copy(Konstanten.ExcelPfad + "\\VorlageAlteSchulordnung.xlsx", fileName);
      }
    }

    /// <summary>
    /// Füllt die Daten des Kurses (Schülernamen, Klasse,...) in die Exceldatei
    /// </summary>
    private void FillExcelFile()
    {
      var klassen = new List<string>(); // sammelt alle Klassennamen dieses Kurses (z.B. für Ethik spannend)
                                        // Schulart, SA-Wertung wird dem ersten Schüler entnommen

      Schueler ersterSchueler = new Schueler(alleSchueler[0]); // muss existieren, da nur Kurse mit Schülern erzeugt werden

      Schulart schulart = ersterSchueler.getKlasse.Schulart;
      Schulaufgabenwertung wertung = kurs.getFach.GetSchulaufgabenwertung(ersterSchueler.Zweig,ersterSchueler.getKlasse.Jahrgangsstufe);

      // schreibe Notenbogen - Kopf
      xls.WriteValue(xls.notenbogen, CellConstant.Wertungsart, GetWertungsString(wertung));
      xls.WriteValue(xls.notenbogen, CellConstant.Fachbezeichnung, kurs.getFach.Bezeichnung);
      xls.WriteValue(xls.notenbogen, CellConstant.Lehrer, kurs.getLehrer.Name);
      //xls.WriteValue(xls.notenbogen, CellConstant.Schuljahr, Konstanten.Schuljahr);
      xls.WriteValueProtectedCell(xls.sid, CellConstant.KursId, kurs.Id.ToString());

      int zeile = 5;
      int zeileFuerSId = CellConstant.zeileSIdErsterSchueler;

      foreach (var s in alleSchueler)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(s.Id);

        if (!klassen.Contains(schueler.getKlasse.Data.Bezeichnung))
        {
          klassen.Add(schueler.getKlasse.Data.Bezeichnung);
        }

        // Schüler in die Exceldatei schreiben
        xls.WriteValueProtectedCell(xls.notenbogen, CellConstant.Nachname + zeile, schueler.Data.Name);
        xls.WriteValueProtectedCell(xls.notenbogen, CellConstant.Vorname + (zeile + 1), "   " + schueler.benutzterVorname);
        xls.WriteValueProtectedCell(xls.sid, CellConstant.SId + zeileFuerSId, schueler.Id.ToString());

        if (schueler.IsLegastheniker && (kurs.getFach.Kuerzel == "E" || kurs.getFach.Kuerzel == "F"))
        {
          xls.SetLegasthenievermerkByZeile(zeile, true);
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
        case "Inf": //Informatik für Sozial-13
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

