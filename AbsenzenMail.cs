using diNo.diNoDataSetTableAdapters;
using diNo.Xml.Mbstatistik;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diNo
{
  /*
   Aus WebUntis die Übersicht Klassenbuch->Abwesenheiten aufrufen, unten Bericht->csv herunterladen 
   ergibt eine UTF-8 Datei mit den Spalten
   Langname	Vorname	ID	Klasse	Beginndatum	Beginnzeit	Enddatum	Endzeit	Unterbrechungen	Abwesenheitsgrund	Text/Grund	Entschuldigungsnummer	Status	Entschuldigungstext	gemeldet von Schüler
   Trennzeichen sind Tabs
  */
  public class AbsenzenMail
  {
    private List<int> sList = new List<int>();
    StreamWriter err;

    public AbsenzenMail(){

      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "CSV-Datei aus WebUntis wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ImportCSV(dia.FileName);
        SendAbsenzen(dia.FileName,
          MessageBox.Show("Bitte die Fehlerdatei prüfen.\nSollen die Absenzen jetzt gemailt werden (Nein für Test)?", "diNo", MessageBoxButtons.YesNo, MessageBoxIcon.Question)!=DialogResult.Yes);
      }
    }

    private void SendAbsenzen(string fileName, bool isTest)
    {
      MailTools mail = new MailTools();
      bool erstesMal = true;

      using (StreamWriter writer = new StreamWriter(new FileStream(fileName + "_send.txt", FileMode.Create, FileAccess.ReadWrite)))
      {
        foreach (int i in sList)
        {
          Schueler s = Zugriff.Instance.SchuelerRep.Find(i);
          if (s.Data.IsNotfalltelefonnummerNull() || s.Data.Notfalltelefonnummer=="")
          {
            err.WriteLine("MAILADRESSE fehlt bei " + s.VornameName);
            continue;
          }
          writer.WriteLine("Mail an " + s.Data.Notfalltelefonnummer);

          string body = s.ErzeugeAnrede(true);
          body += "im Folgenden dürfen wir Sie über die aktuellen Absenzen " 
            + (s.Data.Geschlecht=="M" ? "Ihres Sohnes " : "Ihrer Tochter ")
            + s.Data.Rufname + " an der FOS Kempten informieren.\n\n";

          foreach (string a in s.absenzen)
          {
            body += a + "\n";
          }

          Lehrer kl = s.getKlasse.Klassenleiter;
          body += "\nBei Unstimmigkeiten wenden Sie sich bitte per Mail an mich:"             
            +  "\n" + kl.Data.EMail;

          body += "\n\nMit freundlichen Grüßen\n" + kl.NameDienstbezeichnung;
          body += "\n" + kl.KLString;
          body = body.Replace("<br>", "\n");
          writer.WriteLine(body);
          writer.WriteLine("------------------");
          s.absenzen.Clear();

          // Versendeprozess:
          if (!isTest || erstesMal) // bei Test nur ein Mail schicken, aber ganze Datei erzeugen
          {
            try
            {
              erstesMal = false;
              var msg = new MimeKit.MimeMessage()
              {
                Sender = new MimeKit.MailboxAddress("FOS Kempten", mail.MailFrom),
                Subject = "Absenzenübersicht " + s.VornameName
              };

              msg.From.Add(new MimeKit.MailboxAddress("FOS Kempten", mail.MailFrom));
              if (isTest)
                msg.To.Add(new MimeKit.MailboxAddress("Claus Konrad", "claus.konrad@fosbos-kempten.de"));
              else
                msg.To.Add(new MimeKit.MailboxAddress(s.Data.Notfalltelefonnummer, s.Data.Notfalltelefonnummer));

              msg.ReplyTo.Add(new MimeKit.MailboxAddress(kl.VornameName, kl.Data.EMail));

              var builder = new MimeKit.BodyBuilder();
              builder.TextBody = body;
              msg.Body = builder.ToMessageBody();

              //mailServer.Timeout = 1000;
              mail.mailServer.Send(msg);
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

    private void ImportCSV(string fileName)
    {
      err = new StreamWriter(new FileStream(fileName + "_err.txt", FileMode.Create, FileAccess.ReadWrite));
      
      using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
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

            if (s.Data.Schulart == "B")  // BOSler kriegen keine Aufstellung
              continue;

            /*
            Datenstruktur für Absenzeneintrag oder gleich eine Textzeile?         
            */
            string a = "";
            string grund = line[9];
            if (grund == "krank")
              a = VonBis(line[4], line[6]) + " krank";
            else if (grund == "Befreiung" || grund == "krank (Unt)" || grund == "Verspätung")
              a = VonBis(line[4], line[6], line[5], line[7]) + " " + grund + " " + line[10];
            else if (grund == "unentschuldigt")
              a = VonBis(line[4], line[6]) + " UNENTSCHULDIGT";
            else err.WriteLine("GRUND! " + original);

            if (a != "")
            {
              if (s.absenzen.Count == 0)
                sList.Add(schuelerId); // jeden Schüler nur einmal aufnehmen
              s.absenzen.Add(a);
            }
          }
          catch {
            err.WriteLine("EXCEPTION! " + original);
          }
        }
      }
    }


    private string VonBis(string von, string bis)
    {
      if (von == bis)
        return von;
      else return von + " bis " + bis;
    }
    
    private string VonBis(string von, string bis, string zeitVon, string zeitBis){
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
