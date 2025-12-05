using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static diNo.MailTools;

namespace diNo
{
  public partial class MailDialog : Form
  {
    private List<Schueler> sList;
    StreamWriter err;
    private Task sendTask;

    public MailDialog(List<Schueler> selectedObjects)
    {
      sList = selectedObjects;
      InitializeComponent();
      lbAnzahl.Text = sList.Count + " Schüler ausgewählt.";
      
      //log = new StreamWriter(new FileStream(Path.Combine(verzeichnis, "Mail_log.txt"), FileMode.Create, FileAccess.ReadWrite));
      
    }

    private void opAnhangAbsenzen_Click(object sender, EventArgs e)
    {
      opReplyKL.Checked = true;
      opToEltern.Checked = true;
    }

    private async void btnSend_Click(object sender, EventArgs e)
    {
      if (sendTask != null && !sendTask.IsCompleted)
      {
        MessageBox.Show("Ein Versand läuft bereits.");
        return;
      }

      btnSend.Enabled = false;

      try
      {
        var subject = txtSubject.Text;
        var withAbsenzen = opAnhangAbsenzen.Checked;
        var isTest = chkTest.Checked;
        var toEltern = opToEltern.Checked;

        var replyTyp =
          opSekretariat.Checked ? ReplyTyp.Sekretariat :
          (opReplyKL.Checked ? ReplyTyp.Klassenleiter : ReplyTyp.dino);

        string bodyText = null;
        if (!withAbsenzen)
        {
          if (chkReadBodyText.Checked)
          {
            using (var dia = new OpenFileDialog
            {
              Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
              RestoreDirectory = true,
              Title = "Textdatei mit Bodytext des Mails auswählen"
            })
            {
              if (dia.ShowDialog() != DialogResult.OK)
                return;
              bodyText = File.ReadAllText(dia.FileName);
            }
          }
          else
          {
            bodyText = txtBody.Text;
          }
        }
        else
        {
          if (!Zugriff.Instance.AbsenzenEingelesen)
            ImportCSV();
        }

        string attachmentPath = null;
        if (opAnhangPDF.Checked)
        {
          using (var dia = new OpenFileDialog
          {
            Filter = "PDF (*.pdf)|*.pdf",
            RestoreDirectory = true,
            Title = "PDF-Datei als Anhang auswählen"
          })
          {
            if (dia.ShowDialog() != DialogResult.OK)
              return;
            attachmentPath = dia.FileName;
          }
        }

        var list = sList.ToList(); // Snapshot

        sendTask = Task.Run(() =>
        {
          using (var mail = new MailTools()) // parameterlos – lädt Settings intern
          {
            mail.Betreff = subject;

            if (!withAbsenzen)
              mail.BodyText = bodyText ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(attachmentPath))
              mail.DateiAnhang = attachmentPath;

            foreach (var s in list)
            {
              if (withAbsenzen)
              {
                if (s.absenzen != null && s.absenzen.Count > 0)
                  mail.SendAbsenzen(s, isTest);
              }
              else
              {
                mail.SendMail(s, toEltern, replyTyp, isTest);
                if (isTest) break; // nur eine Test-Mail
              }
            }
          }
        });

        await sendTask;
        MessageBox.Show("Versand abgeschlossen.");
      }
      catch (Exception ex)
      {
        MessageBox.Show("Fehler beim Versand: " + ex.Message);
      }
      finally
      {
        btnSend.Enabled = true;
      }
    }

    // Datei aus WebUntis einlesen und beim Schüler speichern (Liste absenzen)
    public void ImportCSV()
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "CSV-Datei aus WebUntis (Klassenbuch/Abwesenheiten/Berichte als csv) mit allen Absenzen dieses Monats wählen.";
      if (dia.ShowDialog() != DialogResult.OK)
        return;
      string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string verzeichnis = Path.Combine(userProfilePath, "Downloads");
      err = new StreamWriter(new FileStream(Path.Combine(verzeichnis, "Absenzen_Reader_err.txt"), FileMode.Create, FileAccess.ReadWrite));

      using (FileStream stream = new FileStream(dia.FileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(stream))
      {
        reader.ReadLine(); // erste Zeile enthält die Feldnamen
        while (!reader.EndOfStream)
        {
          string original = reader.ReadLine();
          string[] line = original.Split(new string[] { "\t" }, StringSplitOptions.None);

          if (line.Length != 15) // Format prüfen
          {
            err.WriteLine("FORMAT! " + original);
            continue;
          }
          try
          {
            int schuelerId = int.Parse(line[2]);
            Schueler s = Zugriff.Instance.SchuelerRep.Find(schuelerId);
            if (s.Name != line[0]) // falscher Name?
              err.WriteLine("NAME! " + original);
           
            string a = "";
            string grund = line[9];
            string text = line[10];
            string status = line[12];
            if (status != "") status = ", " + status;
            if (grund == "krank")
              a = VonBis(line[4], line[6]) + " krank" + status + " " + text;
            else if (grund == "Befreiung" || grund == "krank (Unt)" || grund == "Verspätung")
              a = VonBis(line[4], line[6], line[5], line[7]) + " " + grund + " " + text;
            else if (grund == "unentschuldigt")
              a = VonBis(line[4], line[6]) + " unentschuldigt " + text;
            else err.WriteLine("GRUND! " + original);

            if (a != "")
            {
              s.absenzen.Add(a);
            }            
          }
          catch
          {
            err.WriteLine("EXCEPTION! " + original);
          }
        }        
      }
      Zugriff.Instance.AbsenzenEingelesen = true;
    }


    private string VonBis(string von, string bis)
    {
      if (von == bis)
        return von;
      else return von + " bis " + bis;
    }

    private string VonBis(string von, string bis, string zeitVon, string zeitBis)
    {
      if (von == bis)
      {
        if (zeitVon != "07:40" && zeitVon == "16:30") von += " ab " + zeitVon;
        else if (zeitVon != "07:40") von += " von " + zeitVon + " bis " + zeitBis;
        else if (zeitBis != "16:30") von += " bis " + zeitBis;
        return von;
      }
      else return von + " bis " + bis;
    }


  }

}

