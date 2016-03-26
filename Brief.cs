using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{
    public partial class Brief : Form
    {
        private Schueler s;
        private Klassenansicht frmKlasse;
        private BriefDaten b;
        public Brief(Klassenansicht aufrufendesFormular)
        {            
            frmKlasse = aufrufendesFormular;
            InitializeComponent();
            this.AcceptButton = btnOK;
            this.CancelButton = btnEsc;
            radioButton_CheckedChanged(this,null);
            datVersaeumtAm.Value = DateTime.Today;
            datTermin.Value = DateTime.Today;
            foreach (var f in Zugriff.Instance.eigeneFaecher)
            {
              cbFach.Items.Add(f.Bezeichnung);
            }
            cbFach.SelectedIndex = 0;
        }

        public void Anzeigen(Schueler schueler)
        {
          s = schueler;
          Show();
        }
        
        private void btnEsc_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {                        
            b = new BriefDaten(s);
            if (opVerweis.Checked) VerweisText();
            else if (opSA.Checked || opKA.Checked) NachterminText();
            else if (opSEP.Checked || opMEP.Checked) ErsatzprText();
            else NacharbeitText();

            Hide();
            new ReportBrief(b).Show();
            
            if (opVerweis.Checked && MessageBox.Show("Soll der Verweis auch in den Notenbogen eingetragen werden?","diNo",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {             
              s.AddVorkommnis(Vorkommnisart.Verweis, DateTime.Today, edInhalt.Text);
              frmKlasse.RefreshVorkommnisse();
            }
        }


    private void radioButton_CheckedChanged(object sender, EventArgs e)
    {
        pnlVersaeumtAm.Enabled = opSA.Checked || opKA.Checked;
        pnlNachterminAm.Enabled = !opVerweis.Checked;
        pnlInhalt.Enabled = opVerweis.Checked || opNacharbeit.Checked || opSEP.Checked || opMEP.Checked;
        labelInhalt.Text = (opSEP.Checked || opMEP.Checked) ? "Prüfungsstoff" : "Grund";
    }

    public string erzeugeAnrede()
    {
      if (s.Data.Geschlecht == "M")
          return "Sehr geehrter Herr " + s.Data.Name +",\n\n";
      else
          return "Sehr geehrte Frau " + s.Data.Name +",\n\n";
    }

    public string erzeugeRaum()
    {
      if (edRaum.Text=="") return "";
      else return " in Raum " + edRaum.Text;
    }

    public void VerweisText()
    {
        b.Betreff = "Verweis";
        b.Inhalt = "Hiermit wird " + b.Anrede + " " + b.VornameName + " gemäß Art. 86 (2) BayEUG ein Verweis erteilt.\n\n";
        b.Inhalt += "Begründung der Ordnungsmaßnahme:\n" + edInhalt.Text + "\n\n";            
    }

    public void NachterminText()
    {
        string lnwart = opSA.Checked ? "Schulaufgabe" : "Kurzarbeit";
        b.Betreff = "Versäumnis einer "+lnwart;
        b.Inhalt = erzeugeAnrede();
        b.Inhalt += "Sie haben die " + lnwart + " im Fach " + cbFach.Text + " am " + datVersaeumtAm.Text + " versäumt.\n";
        
        b.Inhalt += "Nach § 50 (1) FOBOSO wird Ihnen ein Nachtermin eingeräumt.\n\n";           
        b.Inhalt += "Der Nachtermin findet statt am " + datTermin.Text + " um " + datZeit.Text + " Uhr" + erzeugeRaum() + ".\n\n";           
        b.Inhalt += "Wird dieser Termin ohne ausreichende Entschuldigung versäumt, wird die Note 6 (0 Punkte) erteilt.\n\n";
        b.Inhalt += "Freundliche Grüße";
    }
    public void ErsatzprText()
    {
        string lnwart = opSEP.Checked ? "schriftliche" : "mündliche";
        b.Betreff = "Nachholung von Leistungsnachweisen";
        b.Inhalt = erzeugeAnrede();
        b.Inhalt += "Sie konnten in diesem Schuljahr im Fach " + cbFach.Text + " wegen Ihrer Versäumnisse nicht hinreichend geprüft werden.\n\n"; 
        b.Inhalt += "Gemäß § 50 (2) FOBOSO wird hiermit eine " +lnwart+" Ersatzprüfung angesetzt.\n\n";
        b.Inhalt += "Prüfungsstoff wird sein: \n" + edInhalt.Text + "\n\n";
        b.Inhalt += "Die " +lnwart+" Ersatzprüfung findet statt am " + datTermin.Text + " um " + datZeit.Text + " Uhr" + erzeugeRaum() + ".\n\n";           
        b.Inhalt += "Wird an der Ersatzprüfung wegen Erkrankung nicht teilgenommen, so muss die Erkrankung durch amtsärztliches Attest nachgewiesen werden; ohne ausreichende Entschuldigung wird die Note 6 (0 Punkte) erteilt.\n\n";
        b.Inhalt += "Freundliche Grüße";
    }

    public void NacharbeitText()
    {
      b.Betreff = "Nacharbeit";
      b.Inhalt = erzeugeAnrede();
      b.Inhalt += "hiermit werden Sie gemäß § 35 (4) FOBOSO zur Nacharbeit verpflichtet.\n\n";
      b.Inhalt += "Begründung:\n" + edInhalt.Text;
      b.Inhalt += "\n\nDie Nacharbeit findet statt am " + datTermin.Text + " um " + datZeit.Text + " Uhr" + erzeugeRaum() + ".\n\n";           
      b.Inhalt += "Wird die Nacharbeit wegen Erkrankung nicht ausgeführt, so muss die Erkrankung durch ärztliches Attest nachgewiesen werden.\n\n";
      b.Inhalt += "Freundliche Grüße";
    }
  }

  public class BriefDaten
    {
        public string Anrede{ get; set; }
        public string Name { get; set; }
        public string VornameName { get; set; }
        public string Strasse { get; set; }
        public string Ort { get; set; }
        public string Klasse { get; set; }
        public string Betreff { get; set; }
        public string Inhalt { get; set; }
        public string Unterschrift { get; set; }

        public BriefDaten(Schueler s)
        {
            Anrede = s.Data.Geschlecht == "M" ? "Herrn" : "Frau";
            VornameName = s.Data.Rufname + " " + s.Data.Name;
            Name = s.Name;
            Strasse = s.Data.AnschriftStrasse;
            Ort = s.Data.AnschriftPLZ + " " +  s.Data.AnschriftOrt;
            Klasse = s.getKlasse.Bezeichnung;
            if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin))
              Unterschrift = "(Systemadministration FOS/BOS Kempten)";
            else
              Unterschrift = Zugriff.Instance.lehrer.Data.Name + ", "+ Zugriff.Instance.lehrer.Data.Dienstbezeichnung;
        }
   }



}
