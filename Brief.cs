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
        public Brief(Schueler schueler)
        {
            s = schueler;
            InitializeComponent();
            this.AcceptButton = btnOK;
            this.CancelButton = btnEsc;
        }

        private void btnEsc_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {                        
            BriefDaten b = new BriefDaten(s);
            if (opVerweis.Checked) b.Verweistext(edBetreff.Text);
            else
            {
                b.Betreff = edBetreff.Text;
                b.Inhalt = edInhalt.Text;
            }
            new ReportBrief(b); 
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
            Unterschrift = Zugriff.Instance.Lehrer.Name + ", "+ Zugriff.Instance.Lehrer.Dienstbezeichnung;
        }

        public void Verweistext(string Grund)
        {
            Betreff = "Verweis";
            Inhalt = "Hiermit wird " + Anrede + " " + VornameName + " aus der Klasse " + Klasse + " gemäß Art. 86(2) BayEUG ein Verweis erteilt.\n";
            Inhalt += "Begründung der Ordnungsmaßnahme: " + Grund;
        }
    }


}
