using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{

  public enum ReplyTyp
  {
    dino = 0,
    Sekretariat = 1,
    Klassenleiter = 2
  }

  public class MailTools : IDisposable
  {
    private readonly SmtpClient mailServer;
    private readonly StreamWriter log;
    private readonly object logLock = new object();
    private readonly object mailLock = new object();
    private bool disposed = false;
    private bool erstesMal = true;

    // SMTP-Konfigurationsfelder
    private readonly string smtpHost;
    private readonly int smtpPort;
    private readonly string mailFrom;
    private readonly string mailPwd;

    // Öffentliche Einstellungsfelder
    public string Betreff { get; set; } = "";
    public string DateiAnhang { get; set; } = "";
    public string BodyText { get; set; } = "";

    // Parameterloser Konstruktor: lädt Settings aus Zugriff.Instance
    public MailTools(string logDirectory = null)
    {
      // SMTP-Settings aus eurer Konfiguration (konkret aus dem Projekt)    
      smtpHost = Zugriff.Instance.getString(GlobaleStrings.SMTP);
      smtpPort = int.Parse(Zugriff.Instance.getString(GlobaleStrings.Port));
      mailFrom = Zugriff.Instance.getString(GlobaleStrings.SendExcelViaMail);
      mailPwd = Zugriff.Instance.getString(GlobaleStrings.MailPasswort);

      if (string.IsNullOrWhiteSpace(smtpHost) || smtpPort <= 0 ||
          string.IsNullOrWhiteSpace(mailFrom) || string.IsNullOrWhiteSpace(mailPwd))
        throw new InvalidOperationException("SMTP-Settings unvollständig (SmtpHost/Port, SchulMail, SmtpPwd).");

      // Log-Datei (wie gehabt)
      try
      {
        if (string.IsNullOrEmpty(logDirectory))
        {
          string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
          logDirectory = Path.Combine(userProfilePath, "Downloads");
        }
        Directory.CreateDirectory(logDirectory);
        string logPath = Path.Combine(logDirectory, "Mail_log.txt");
        log = new StreamWriter(new FileStream(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
        {
          AutoFlush = true
        };
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Konnte Log-Datei nicht anlegen: " + ex.Message, ex);
      }

      mailServer = new SmtpClient
      {
        Timeout = 60000 // optionaler Timeout
      };

      // Wichtig: Kein Connect/Authenticate hier, um den Konstruktor leicht zu halten.
      WriteLog("mailTools initialisiert (ohne Verbindung).");
    }

    private void WriteLog(string text)
    {
      lock (logLock)
      {
        try { var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {text}"; log.WriteLine(entry); }
        catch { }
      }
    }

    private void EnsureConnected()
    {
      lock (mailLock)
      {
        if (disposed) throw new ObjectDisposedException(nameof(MailTools));
        if (mailServer == null) throw new InvalidOperationException("SMTP-Client nicht initialisiert.");

        if (!mailServer.IsConnected)
        {
          try
          {
            mailServer.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            mailServer.Authenticate(mailFrom, mailPwd);
            WriteLog("SMTP verbunden und authentifiziert.");
          }
          catch (Exception ex)
          {
            WriteLog("FEHLER beim Verbinden/Authentifizieren: " + ex.Message);
            throw;
          }
        }
      }
    }
    // Versendet ein Mail an diesen Schüler (ggf. an die Elternadresse)
    public void SendMail(Schueler s, bool anEltern, ReplyTyp replyTyp, bool isTest)
    {
      string mailTo;
      try
      {
        var msg = new MimeKit.MimeMessage()
        {
          Sender = new MimeKit.MailboxAddress("FOSBOS", mailFrom),
          Subject = Betreff
        };

        msg.From.Add(new MimeKit.MailboxAddress(Zugriff.Instance.getString(GlobaleStrings.SchulName), mailFrom));
        if (isTest)
          mailTo = Zugriff.Instance.lehrer.Data.EMail;
        else if (anEltern)
          mailTo = s.Data.Notfalltelefonnummer.Split(new string[] { ",", ";", " " }, StringSplitOptions.RemoveEmptyEntries).First();
        else
          mailTo = s.Data.MailSchule;

        msg.To.Add(new MimeKit.MailboxAddress(mailTo, mailTo));

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
        EnsureConnected();
        mailServer.Send(msg);
        log.WriteLine(s.NameVorname + ", Mail versendet an " + mailTo);
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

      if (!MimeKit.MailboxAddress.TryParse(mailTo, out _))
      {
        WriteLog($"MAILADRESSE ungültig ({mailTo}) bei {s.VornameName}");
        return;
      }
      log.WriteLine("Mail an " + mailTo);

      string body = s.ErzeugeAnrede(!isBOS);
      body += "im Folgenden dürfen wir Sie über die Absenzen ";
      if (!isBOS)
        body += s.getIhrSohn(2) + s.Data.Rufname + " ";
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
      if (!isTest)
        s.absenzen.Clear(); // dieser Schüler ist erledigt

      // Versendeprozess:
      if (!isTest || erstesMal) // bei Test nur ein Mail schicken, aber ganze Datei erzeugen
      {
        try
        {
          erstesMal = false;
          var msg = new MimeKit.MimeMessage()
          {
            Sender = new MimeKit.MailboxAddress("FOSBOS Kempten", mailFrom),
            Subject = "Absenzenübersicht " + s.VornameName
          };

          msg.From.Add(new MimeKit.MailboxAddress("FOSBOS Kempten", mailFrom));
          if (isTest)
            msg.To.Add(new MimeKit.MailboxAddress("Testadresse", Zugriff.Instance.getString(GlobaleStrings.MailAdresseTest)));
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

          EnsureConnected();
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
   

    public void Dispose()
    {
      if (disposed) return; disposed = true;

      try
      {
        lock (mailLock)
        {
          if (mailServer != null)
          {
            try { if (mailServer.IsConnected) mailServer.Disconnect(true); } catch { }
            try { mailServer.Dispose(); } catch { }
          }
        }
      }
      catch { }

      try
      {
        lock (logLock)
        {
          try { log?.Flush(); } catch { }
          try { log?.Close(); } catch { }
          try { log?.Dispose(); } catch { }
        }
      }
      catch { }
    }
  }
}