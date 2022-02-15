using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenZip;
using System.Windows.Forms;
using MailKit.Net;
using System.Net.Mail;

namespace diNo
{
  public class MailSchuelerNoten
  {
    string passwort;
    string pfad = @"C:\tmp\";
    Bericht rpttyp = Bericht.Notenmitteilung; // ggf. ändern Bericht.Einbringung; // 

    public MailSchuelerNoten(List<Schueler> schueler)
    {      
      foreach (Schueler s in schueler)
      {
        SendSchuelerNoten(s);
      }
    }

    private void SendSchuelerNoten(Schueler s)
    {
      string datei;
      datei = pfad + s.getKlasse.Bezeichnung + "_" + Tools.ErsetzeUmlaute(s.Name + s.benutzterVorname) + ".pdf";
      passwort = "FB-" + s.Data.Geburtsdatum.ToString("yyyyMMdd");
      CreatePdf(s, datei);
      Zip(datei);      
      Send(s, datei + ".zip");
    }

    // erzeugt eine Notenmitteilung in PDF-Form
    public void CreatePdf(Schueler s, string targetFile)
    {
      List<SchuelerDruck> liste = new List<SchuelerDruck>();

      //ReportViewer im Hintergrund erstellen, um PDF zu drucken, und Report zuweisen.
      ReportViewer rpt = new ReportViewer();
      ReportDataSource dataSource = new ReportDataSource();
      dataSource.Name = "DataSet1";

      //Report als Embedded Resource mit dem Namen "Report.rdlc" ... entsprechend anpassen
      rpt.LocalReport.ReportEmbeddedResource = "diNo." + SchuelerDruck.GetBerichtsname(rpttyp) + ".rdlc"; //"diNo.rptNotenmitteilung.rdlc";

      // Unterberichte einbinden
      rpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(subrptEventHandler);

      //Report mit Daten befüllen
      liste.Add(SchuelerDruck.CreateSchuelerDruck(s, Bericht.Notenmitteilung, UnterschriftZeugnis.SL));
      
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
          e.DataSources.Add(new ReportDataSource("DataSet1", PunkteSummeDruck.Create(schueler, rpttyp)));
        }
        else
        {          
          IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenDruck(Bericht.Notenmitteilung);
          e.DataSources.Add(new ReportDataSource("DataSet1", noten));
        }
      }
    }

    void Zip(string inFile)
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


    void Send(Schueler s, string datei)
    {
      string bodyText = "";
      string smtp = Zugriff.Instance.getString(GlobaleStrings.SMTP);
      int port = int.Parse(Zugriff.Instance.getString(GlobaleStrings.Port));
      MailKit.Net.Smtp.SmtpClient mailServer;

      string infoFile = pfad + "Mail.txt";
      try
      {
        mailServer = new MailKit.Net.Smtp.SmtpClient();
        mailServer.Connect(smtp, port, MailKit.Security.SecureSocketOptions.StartTls);
        mailServer.Authenticate(Zugriff.Instance.getString(GlobaleStrings.SendExcelViaMail), Zugriff.Instance.getString(GlobaleStrings.MailPasswort));
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "diNo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (File.Exists(infoFile))
      {
        bodyText = File.ReadAllText(infoFile);
      }

      string MailTo = Tools.ErsetzeUmlaute(s.benutzterVorname + "." + s.Name + "@fosbos-kempten.de");
      string MailFrom = Zugriff.Instance.getString(GlobaleStrings.SendExcelViaMail);

      // Test
      //MailTo = "claus.konrad@fosbos-kempten.de";

      if (!string.IsNullOrEmpty(MailTo))
      {
        try
        {
          var msg = new MimeKit.MimeMessage()
          {
            Sender = new MimeKit.MailboxAddress("Digitale Notenverwaltung", MailFrom),
            Subject = (rpttyp == Bericht.Einbringung) ? "Einbringungsvorschlag" : "Aktuelle Notenübersicht"
          };

          msg.From.Add(new MimeKit.MailboxAddress("Digitale Notenverwaltung", MailFrom));          
          msg.To.Add(new MimeKit.MailboxAddress(s.benutzterVorname + " " + s.Name, MailTo));

          var builder = new MimeKit.BodyBuilder();
          builder.TextBody = "Hallo " + s.benutzterVorname + "," + bodyText;
          builder.Attachments.Add(datei);
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
}
