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
    

    public MailDialog(List<Schueler> selectedObjects)
    {
      sList = selectedObjects;
      InitializeComponent();
      lbAnzahl.Text = sList.Count + " Schüler ausgewählt.";
    }

    private void opAnhangAbsenzen_Click(object sender, EventArgs e)
    {
      opReplyKL.Checked = true;
      opToEltern.Checked = true;
    }

    private void btnSend_Click(object sender, EventArgs e)
    {      
    
      MailTools mail = new MailTools();
      ReplyTyp repltyp = ReplyTyp.dino;
      mail.Betreff = txtSubject.Text;

      if (!opAnhangAbsenzen.Checked)
      {
        if (chkReadBodyText.Checked)
        {
          OpenFileDialog dia = new OpenFileDialog();
          dia.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
          dia.RestoreDirectory = true;
          dia.Title = "Textdatei mit Bodytext des Mails auswählen";
          if (dia.ShowDialog() != DialogResult.OK)
            return;
          mail.BodyText = File.ReadAllText(dia.FileName);
        }
        else mail.BodyText = txtBody.Text;
      }
      
      if (opAnhangPDF.Checked)
      {
        OpenFileDialog dia = new OpenFileDialog();
        dia.Filter = "PDF (*.pdf)|*.pdf";
        dia.RestoreDirectory = true;
        dia.Title = "PDF-Datei als Anhang auswählen";
        if (dia.ShowDialog() != DialogResult.OK)
          return;
        mail.DateiAnhang = dia.FileName;        
      }

      if (opSekretariat.Checked)
        repltyp = ReplyTyp.Sekretariat;
      else if (opReplyKL.Checked)
        repltyp = ReplyTyp.Klassenleiter;

      foreach (Schueler s in sList)
      {
        mail.SendMail(s,opToEltern.Checked, repltyp,chkTest.Checked);
        if (chkTest.Checked) // Schleife beenden
          break;
      }

      Close();
    }
  }
}
