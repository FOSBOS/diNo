using diNo.diNoDataSetTableAdapters;
using diNo.Xml.Mbstatistik;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
      dia.Title = "CSV-Datei aus WebUntis mit allen Absenzen dieses Monats wählen";
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
      string mailTo;

      int abID = Int32.Parse(InputBox.Show("Ab welcher Schüler-ID soll gemailt werden (0 für alle)?", "0"));

      using (StreamWriter writer = new StreamWriter(new FileStream(fileName + "_send.txt", FileMode.Create, FileAccess.ReadWrite)))
      {
        foreach (int i in sList) // Liste ist nach ID sortiert
        {
          if (i < abID) continue;
          Schueler s = Zugriff.Instance.SchuelerRep.Find(i);
          bool isBOS = s.Data.Schulart == "B";
          if (isBOS)
            mailTo = s.Data.MailSchule;
          else if (s.Data.IsNotfalltelefonnummerNull() || s.Data.Notfalltelefonnummer == "")
          {
            err.WriteLine("MAILADRESSE fehlt bei " + s.VornameName);
            continue;
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
            err.WriteLine("MAILADRESSE " + mailTo + " ungültig bei " + s.VornameName);
            continue;
          }
          writer.WriteLine("Mail an " + mailTo);

          string body = s.ErzeugeAnrede(!isBOS);
          body += "im Folgenden dürfen wir Sie über die Absenzen ";
          if (!isBOS)
            body +=  (s.Data.Geschlecht == "M" ? "Ihres Sohnes " : "Ihrer Tochter ") + s.Data.Rufname + " ";
          body += "im letzten Monat an der FOSBOS Kempten informieren.\n\n";

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
          s.absenzen.Clear();

          // Versendeprozess:
          if (!isTest || erstesMal) // bei Test nur ein Mail schicken, aber ganze Datei erzeugen
          {
            try
            {
              erstesMal = false;
              var msg = new MimeKit.MimeMessage()
              {
                Sender = new MimeKit.MailboxAddress("FOSBOS Kempten", mail.MailFrom),
                Subject = "Absenzenübersicht " + s.VornameName
              };

              msg.From.Add(new MimeKit.MailboxAddress("FOSBOS Kempten", mail.MailFrom));
              if (isTest)
                msg.To.Add(new MimeKit.MailboxAddress("Claus Konrad", "claus.konrad@fosbos-kempten.de"));
              else 
                msg.To.Add(new MimeKit.MailboxAddress(mailTo, mailTo));
              if (!isBOS)
              {
                msg.Cc.Add(new MimeKit.MailboxAddress(s.Data.MailSchule, s.Data.MailSchule));
              }

              msg.ReplyTo.Add(new MimeKit.MailboxAddress(kl.VornameName, kl.Data.EMail));

              var builder = new MimeKit.BodyBuilder();
              builder.TextBody = body;
              msg.Body = builder.ToMessageBody();

              //mailServer.Timeout = 1000;
              mail.mailServer.Send(msg);
              writer.WriteLine("Mail versendet für Schüler " + s.VornameName);
            }
            catch (Exception ex)
            {
              err.WriteLine("Fehler bei Schüler " + s.VornameName + " mit ID=" + s.Id);
              if (MessageBox.Show(s.VornameName + "\n" + ex.Message, "diNo", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                throw;
            }
          }
          writer.WriteLine("------------------ " + s.Id);
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
          sList.Sort();
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
