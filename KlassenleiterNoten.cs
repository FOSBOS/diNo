﻿using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenZip;
using System.Net.Mail;
using System.Windows.Forms;

namespace diNo
{
  public class KlassenleiterNoten
  {
    string passwort = "FB8home";
    string pfad = @"C:\tmp\";
    public KlassenleiterNoten(List<Klasse> klassen)
    {
      string datei;
      foreach (Klasse k in klassen)
      {
        datei = pfad + "Notenübersicht " + k.Bezeichnung + ".pdf";
        CreatePdf(k, datei);
        Zip(datei);
        Send(k, datei + ".zip");
      }
    }

    // erzeugt eine Notenmitteilung in PDF-Form
    public void CreatePdf(Klasse k, string targetFile)
    {
      List<SchuelerDruck> liste = new List<SchuelerDruck>();

      //ReportViewer im Hintergrund erstellen, um PDF zu drucken, und Report zuweisen.
      ReportViewer rpt = new ReportViewer();
      ReportDataSource dataSource = new ReportDataSource();
      dataSource.Name = "DataSet1";

      //Report als Embedded Resource mit dem Namen "Report.rdlc" ... entsprechend anpassen
      rpt.LocalReport.ReportEmbeddedResource = "diNo.rptNotenmitteilung.rdlc";

      // Unterberichte einbinden
      rpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(subrptEventHandler);

      //Report mit Daten befüllen
      foreach (Schueler s in k.eigeneSchueler)
      {
        liste.Add(SchuelerDruck.CreateSchuelerDruck(s, Bericht.Notenmitteilung, UnterschriftZeugnis.SL));
      }
      dataSource.Value = liste; // ob das geht???
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
      // ACHTUNG: Der Parameter muss im Haupt- und im Unterbericht definiert werden (mit gleichem Namen)
      // string subrpt = e.ReportPath; // jeder Unterbericht ruft diesen EventHandler auf; hier steht drin welcher es ist.
      int schuelerId;
      int.TryParse(e.Parameters[0].Values[0], out schuelerId);
      if (schuelerId > 0)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
        IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenDruck(Bericht.Notenmitteilung);
        e.DataSources.Add(new ReportDataSource("DataSet1", noten));
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


    void Send(Klasse k, string datei)
    {
      string bodyText = "";
      MailAddress from = new MailAddress(Zugriff.Instance.getString(GlobaleStrings.SendExcelViaMail), "Digitale Notenverwaltung");
      SmtpClient mailServer;

      string infoFile = pfad + "Mail.txt";
//    if (MessageBox.Show("Mailservereinstellungen müssen unter globale Texte angegeben werden.\nEin in der Mail zu versendender Infotext kann in der Datei " + infoFile + " abgelegt werden.", "Notendateien versenden", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
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

      if (File.Exists(infoFile))
      {
        bodyText = File.ReadAllText(infoFile);
      }

      string dienstlicheMailAdresse = k.Klassenleiter.Data.EMail; // "claus.konrad@fosbos-kempten.de";
      if (!string.IsNullOrEmpty(dienstlicheMailAdresse))
      {
        try
        {
          MailMessage msg = new MailMessage();
          msg.From = from;
          msg.To.Add(new MailAddress(dienstlicheMailAdresse));
          msg.Subject = "Notenübersicht " + k.Bezeichnung;
          msg.Body = "Hallo " + k.Klassenleiter.Data.Vorname + "," + bodyText;
          msg.Attachments.Add(new Attachment(datei));

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
