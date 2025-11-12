using diNo.Xml.Mbstatistik;
using MailKit.Net;
using Microsoft.Office.Interop.Excel;
using Microsoft.Reporting.WinForms;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


  public enum ReplyTyp
  {
    dino = 0,
    Sekretariat = 1,
    Klassenleiter = 2
  }

  public class MailTools
  {
    public string Betreff = "";
    Bericht rptTyp = Bericht.Notenmitteilung;  // Bericht.Einbringung; // ggf. ändern
    public string Pfad = Zugriff.Instance.getString(GlobaleStrings.VerzeichnisExceldateien); // @"C:\tmp\"; // Pfad, in dem die temporären Dateien abgelegt werden
    public StreamWriter log;

    string smtp = Zugriff.Instance.getString(GlobaleStrings.SMTP);
    int port = int.Parse(Zugriff.Instance.getString(GlobaleStrings.Port));
    public string MailFrom = Zugriff.Instance.getString(GlobaleStrings.SendExcelViaMail);
    string MailPwd = Zugriff.Instance.getString(GlobaleStrings.MailPasswort);
    public MailKit.Net.Smtp.SmtpClient mailServer;    
    public string BodyText;
    public string DateiAnhang="";
    string MailTo;
    string MailToVorname;
    string MailToNachname;
    bool erstesMal = true;
   

    public MailTools() : 
    {
      try
      {
        string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string verzeichnis = Path.Combine(userProfilePath, "Downloads");
        log = new StreamWriter(new FileStream(Path.Combine(verzeichnis, "Mail_log.txt"), FileMode.Create, FileAccess.ReadWrite));

        mailServer = new MailKit.Net.Smtp.SmtpClient();
        mailServer.Connect(smtp, port, MailKit.Security.SecureSocketOptions.StartTls);
        mailServer.Authenticate(MailFrom, MailPwd);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "diNo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
    }

    // Versendet ein Mail an diesen Schüler (ggf. an die Elternadresse)
    public void SendMail(Schueler s, bool anEltern,ReplyTyp replyTyp, bool isTest)
    {
      try
      {
        var msg = new MimeKit.MimeMessage()
        {
          Sender = new MimeKit.MailboxAddress("FOSBOS", MailFrom),
          Subject = Betreff
        };

        msg.From.Add(new MimeKit.MailboxAddress(Zugriff.Instance.getString(GlobaleStrings.SchulName), MailFrom));
        if (isTest)
          MailTo = Zugriff.Instance.lehrer.Data.EMail;
        else if (anEltern)
          MailTo = s.Data.Notfalltelefonnummer.Split(new string[] { ",", ";", " " }, StringSplitOptions.RemoveEmptyEntries).First();
        else
          MailTo = s.Data.MailSchule;

        msg.To.Add(new MimeKit.MailboxAddress(MailTo, MailTo));

        var builder = new MimeKit.BodyBuilder();
        builder.TextBody = (s.ErzeugeAnrede(anEltern) + BodyText).Replace("<br>", "\n");

        if (DateiAnhang != "")
          builder.Attachments.Add(DateiAnhang);
        msg.Body = builder.ToMessageBody();

        if (replyTyp == ReplyTyp.Klassenleiter)
        {
          Lehrer kl = s.getKlasse.Klassenleiter;
          msg.ReplyTo.Add(new MimeKit.MailboxAddress(kl.VornameName, kl.Data.EMail));
        }
        else if (replyTyp == ReplyTyp.Sekretariat)
        {
          msg.ReplyTo.Add(new MimeKit.MailboxAddress(Zugriff.Instance.getString(GlobaleStrings.SchulName), Zugriff.Instance.getString(GlobaleStrings.SchulMail)));
        }

        mailServer.Send(msg);
        log.WriteLine(s.NameVorname + ", Mail versendet an " + MailTo);
        log.Flush();
      }
      catch (Exception ex)
      {
        log.WriteLine("FEHLER bei Schüler " + s.NameVorname + ": " + ex.Message);
        log.Flush();
        if (MessageBox.Show(ex.Message, "diNo", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
          throw;
      }
    }

    public void SendAbsenzen(Schueler s, bool isTest)
    {
      string mailTo;

      bool isBOS = s.Data.Schulart == "B";
      if (isBOS)
        mailTo = s.Data.MailSchule;
      else if (s.Data.IsNotfalltelefonnummerNull() || s.Data.Notfalltelefonnummer == "")
      {
        log.WriteLine("MAILADRESSE fehlt bei " + s.VornameName);
        return;
      }
      else
      {
        mailTo = s.Data.Notfalltelefonnummer.Split(new string[] { ",", ";", " " }, StringSplitOptions.RemoveEmptyEntries).First();
      }

      try
      {
        MailAddress address = new MailAddress(mailTo);
      }
      catch (FormatException)
      {
        log.WriteLine("MAILADRESSE " + mailTo + " ungültig bei " + s.VornameName);
        return;
      }
      log.WriteLine("Mail an " + mailTo);

      string body = s.ErzeugeAnrede(!isBOS);
      body += "im Folgenden dürfen wir Sie über die Absenzen ";
      if (!isBOS)
        body += (s.Data.Geschlecht == "M" ? "Ihres Sohnes " : "Ihrer Tochter ") + s.Data.Rufname + " ";
      body += "im letzten Monat an der FOSBOS Kempten informieren.\n\n";

      foreach (string a in s.absenzen)
      {
        body += a + "\n";
      }

      Lehrer kl = s.getKlasse.Klassenleiter;
      body += "\nBei Unstimmigkeiten wenden Sie sich bitte per Mail an mich:"
        + "\n" + kl.Data.EMail;

      body += "\n\nMit freundlichen Grüßen\n" + kl.NameDienstbezeichnung;
      body += "\n" + kl.KLString;
      body = body.Replace("<br>", "\n");
      log.WriteLine(body);
      s.absenzen.Clear();

      // Versendeprozess:
      if (!isTest || erstesMal) // bei Test nur ein Mail schicken, aber ganze Datei erzeugen
      {
        try
        {
          erstesMal = false;
          var msg = new MimeKit.MimeMessage()
          {
            Sender = new MimeKit.MailboxAddress("FOSBOS Kempten", MailFrom),
            Subject = "Absenzenübersicht " + s.VornameName
          };

          msg.From.Add(new MimeKit.MailboxAddress("FOSBOS Kempten", MailFrom));
          if (isTest)
            msg.To.Add(new MimeKit.MailboxAddress("Claus Konrad", "claus.konrad@fosbos-kempten.de"));
          else
            msg.To.Add(new MimeKit.MailboxAddress(mailTo, mailTo));
          if (!isBOS && !isTest)
          {
            msg.Cc.Add(new MimeKit.MailboxAddress(s.Data.MailSchule, s.Data.MailSchule));
          }

          msg.ReplyTo.Add(new MimeKit.MailboxAddress(kl.VornameName, kl.Data.EMail));

          var builder = new MimeKit.BodyBuilder();
          builder.TextBody = body;
          msg.Body = builder.ToMessageBody();

          mailServer.Send(msg);
          log.WriteLine("Mail versendet für Schüler " + s.VornameName);
        }
        catch (Exception ex)
        {
          log.WriteLine("FEHLER bei Schüler " + s.VornameName + " mit ID=" + s.Id + "\n" + ex.Message);
        }
      }
      log.WriteLine("------------------ " + s.Id);        
    }


    // Älterer Quellcode - noch nicht überarbeitet.
    public void Zip(Lehrer l, string inFile)
    {
      Zip(inFile, "1949");
    }

    public void Zip(Schueler s, string inFile)
    {
      Zip(inFile, "FB-" + s.Data.Geburtsdatum.ToString("yyyyMMdd"));
    }
    
    private void Zip(string inFile, string passwort)
    {
      // Es muss das NuGet-Package SevenZipSharp installiert sein. Diesem muss man den DLL-Pfad der 7z.dll mitgeben: 

      string outFile = inFile + ".zip";
      string dll = @"F:\diNo\packages\SevenZipSharp.Net45.1.0.19\lib\net45\7z.dll";
      SevenZipCompressor.SetLibraryPath(dll);

      SevenZipCompressor szc = new SevenZipCompressor
      {
        CompressionMethod = CompressionMethod.Deflate,
        CompressionLevel = CompressionLevel.Normal,
        CompressionMode = CompressionMode.Create,
        DirectoryStructure = true,
        PreserveDirectoryRoot = false,
        ArchiveFormat = OutArchiveFormat.Zip
      };

      szc.CompressFilesEncrypted(outFile, passwort, new string[] { inFile });
    }

    // erzeugt eine Notenmitteilung in PDF-Form (einzelner Schüler oder eine ganze Klasse)
    public void CreatePdf(Schueler s, string targetFile)
    {
      List<SchuelerDruck> liste = new List<SchuelerDruck>();
      liste.Add(SchuelerDruck.CreateSchuelerDruck(s, Bericht.Notenmitteilung, UnterschriftZeugnis.SL));
      CreatePdf(liste, targetFile);
    }

    public void CreatePdf(Klasse k, string targetFile)
    {
      List<SchuelerDruck> liste = new List<SchuelerDruck>();
      foreach (Schueler s in k.Schueler)
        liste.Add(SchuelerDruck.CreateSchuelerDruck(s, Bericht.Notenmitteilung, UnterschriftZeugnis.SL));
      CreatePdf(liste, targetFile);
    }

    private void CreatePdf(List<SchuelerDruck> liste, string targetFile)
    {
      //ReportViewer im Hintergrund erstellen, um PDF zu drucken, und Report zuweisen.
      ReportViewer rpt = new ReportViewer();
      ReportDataSource dataSource = new ReportDataSource();
      dataSource.Name = "DataSet1";

      //Report als Embedded Resource mit dem Namen "Report.rdlc" ... entsprechend anpassen
      rpt.LocalReport.ReportEmbeddedResource = "diNo." + SchuelerDruck.GetBerichtsname(rptTyp) + ".rdlc"; //"diNo.rptNotenmitteilung.rdlc";

      // Unterberichte einbinden
      rpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(subrptEventHandler);

      //Report mit Daten befüllen

      dataSource.Value = liste;
      rpt.LocalReport.DataSources.Add(dataSource);

      //Report als PDF in Datei speichern
      byte[] PDF = rpt.LocalReport.Render("PDF");
      FileStream fsReport = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None);
      fsReport.Write(PDF, 0, PDF.Length);
      fsReport.Close();

      rpt.Dispose();
    }

    void subrptEventHandler(object sender, SubreportProcessingEventArgs e)
    {
      string subrpt = e.ReportPath; // jeder Unterbericht ruft diesen EventHandler auf; hier steht drin welcher es ist.
      int schuelerId;
      int.TryParse(e.Parameters[0].Values[0], out schuelerId);
      if (schuelerId > 0)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
        if (subrpt == "subrptPunktesumme" || subrpt == "subrptPunktesummeNB")
        {
          e.DataSources.Add(new ReportDataSource("DataSet1", PunkteSummeDruck.Create(schueler, rptTyp)));
        }
        else
        {
          IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenDruck(Bericht.Notenmitteilung);
          e.DataSources.Add(new ReportDataSource("DataSet1", noten));
        }
      }
    }


    public void MailToKlassenleiter(List<Klasse> klassen)
    {
      string datei;
      foreach (Klasse k in klassen)
      {
        datei = Pfad + k.Bezeichnung + ".pdf";
        CreatePdf(k, datei);
        Zip(k.Klassenleiter, datei);
        Send(k.Klassenleiter, datei + ".zip");
      }
    }

    public void MailToSchueler(List<Schueler> schueler)
    {
      foreach (Schueler s in schueler)
      {
        string datei = Pfad + s.getKlasse.Bezeichnung + "_" + Tools.ErsetzeUmlaute(s.Name + s.benutzterVorname) + ".pdf";
        CreatePdf(s, datei);
        Zip(s, datei);
        Send(s, datei + ".zip");
      }
    }

    public void Send(Lehrer l, string datei)
    {
      MailTo = l.Data.EMail;
      MailToNachname = l.Name;
      MailToVorname = l.Data.Vorname;
      Send(new string[] { datei });
    }

    public void SendNotendateien(Lehrer l, string[] dateien)
    {
      MailTo = l.Data.EMail;
      MailToNachname = l.Name;
      MailToVorname = l.Data.Vorname;
      Send(dateien);
    }

    private void Send(Schueler s, string datei)
    {
      MailTo = s.Data.MailSchule;
      MailToNachname = s.Name;
      MailToVorname = s.benutzterVorname;
      Send(new string[] { datei });
    }

    private void Send(string[] dateien)
    {
      // Test
      // MailTo = "claus.konrad@fosbos-kempten.de";

      if (!string.IsNullOrEmpty(MailTo))
      {
        try
        {
          var msg = new MimeKit.MimeMessage()
          {
            Sender = new MimeKit.MailboxAddress("Digitale Notenverwaltung", MailFrom),
            Subject = Betreff
          };

          msg.From.Add(new MimeKit.MailboxAddress("Digitale Notenverwaltung", MailFrom));
          msg.To.Add(new MimeKit.MailboxAddress(MailToVorname + " " + MailToNachname, MailTo));

          var builder = new MimeKit.BodyBuilder();
          builder.TextBody = "Hallo " + MailToVorname + "," + BodyText;
          foreach (var d in dateien)
            builder.Attachments.Add(d);
          msg.Body = builder.ToMessageBody();

          //mailServer.Timeout = 1000;
          mailServer.Send(msg);
        }
        catch (Exception ex)
        {
          if (MessageBox.Show(ex.Message, "diNo", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
            throw;
        }
      }
    }


  }
