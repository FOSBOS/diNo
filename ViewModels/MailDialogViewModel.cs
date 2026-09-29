using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using static diNo.MailTools;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von MailDialog.cs (WinForms). Versandlogik unverändert
  // übernommen; die Mehrfachversand-Sperre übernimmt automatisch der generierte
  // AsyncRelayCommand (AllowConcurrentExecutions=false), zusätzlich CanSend für
  // die Button-Optik.
  public partial class MailDialogViewModel : ObservableObject
  {
    private readonly List<Schueler> sList;
    private StreamWriter err;

    public string AnzahlText { get; }

    [ObservableProperty] private bool isToSchueler = true;
    [ObservableProperty] private bool isToEltern;

    [ObservableProperty] private bool isReplyDino = true;
    [ObservableProperty] private bool isReplySekretariat;
    [ObservableProperty] private bool isReplyKL;

    [ObservableProperty] private bool isAnhangKeiner = true;
    [ObservableProperty] private bool isAnhangNoten;
    [ObservableProperty] private bool isAnhangPDF;
    [ObservableProperty] private bool isAnhangAbsenzen;

    [ObservableProperty] private bool isTest = true;
    [ObservableProperty] private bool isZip;
    [ObservableProperty] private bool isReadBodyText;

    [ObservableProperty] private string subject = "";
    [ObservableProperty] private string body = "";

    [ObservableProperty] private bool canSend = true;

    public MailDialogViewModel(List<Schueler> selectedObjects)
    {
      sList = selectedObjects;
      AnzahlText = sList.Count + " Schüler ausgewählt.";
    }

    partial void OnIsAnhangAbsenzenChanged(bool value)
    {
      if (value)
      {
        IsReplyKL = true;
        IsToEltern = true;
      }
    }

    [RelayCommand]
    private async Task Send()
    {
      CanSend = false;
      try
      {
        string subjectToSend = IsAnhangNoten ? "Notenmitteilung" : Subject;
        bool isTestToSend = IsTest;

        var replyTyp =
          IsReplySekretariat ? ReplyTyp.Sekretariat :
          (IsReplyKL ? ReplyTyp.Klassenleiter : ReplyTyp.dino);

        AnhangTyp anhangTyp = IsAnhangNoten ? AnhangTyp.Noten :
            (IsAnhangPDF ? AnhangTyp.PDF :
            (IsAnhangAbsenzen ? AnhangTyp.Absenzen : AnhangTyp.Keiner));
        string bodyText = null;

        if (!IsAnhangAbsenzen)
        {
          if (IsReadBodyText)
          {
            var dia = new OpenFileDialog
            {
              Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
              RestoreDirectory = true,
              Title = "Textdatei mit Bodytext des Mails auswählen"
            };
            if (dia.ShowDialog() != true)
              return;
            bodyText = File.ReadAllText(dia.FileName);
          }
          else
          {
            bodyText = Body;
          }
        }
        else
        {
          if (!Zugriff.Instance.AbsenzenEingelesen)
            ImportCSV();
        }

        string attachmentPath = null;
        if (IsAnhangPDF)
        {
          var dia = new OpenFileDialog
          {
            Filter = "PDF (*.pdf)|*.pdf",
            RestoreDirectory = true,
            Title = "PDF-Datei als Anhang auswählen"
          };
          if (dia.ShowDialog() != true)
            return;
          attachmentPath = dia.FileName;
        }

        var list = sList.ToList(); // Snapshot

        await Task.Run(() =>
        {
          using (var mail = new MailTools()) // parameterlos – lädt Settings intern
          {
            mail.Betreff = subjectToSend;
            mail.anhangTyp = anhangTyp;
            mail.replyTyp = replyTyp;
            mail.anEltern = IsToEltern;
            mail.isTest = isTestToSend;

            if (anhangTyp != AnhangTyp.Absenzen) // dort automatisch generiert
              mail.BodyText = bodyText ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(attachmentPath))
              mail.DateiAnhang = attachmentPath;

            foreach (var s in list)
            {
              if (anhangTyp == AnhangTyp.Absenzen)
              {
                if (s.absenzen != null && s.absenzen.Count > 0)
                  mail.SendAbsenzen(s);
              }
              else
              {
                mail.SendMail(s);
                if (isTestToSend) break; // nur eine Test-Mail
              }
            }
          }
        });

        MessageBox.Show("Versand abgeschlossen.");
      }
      catch (Exception ex)
      {
        MessageBox.Show("Fehler beim Versand: " + ex.Message);
      }
      finally
      {
        CanSend = true;
      }
    }

    // Datei aus WebUntis einlesen und beim Schüler speichern (Liste absenzen)
    public void ImportCSV()
    {
      var dia = new OpenFileDialog
      {
        Title = "CSV-Datei aus WebUntis (Klassenbuch/Abwesenheiten/Berichte als csv) mit allen Absenzen dieses Monats wählen."
      };
      if (dia.ShowDialog() != true)
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
