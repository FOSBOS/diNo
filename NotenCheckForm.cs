﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using diNo;

namespace diNo
{
    public partial class NotenCheckForm : Form
    {
        private bool abbrechen = false;

        public NotenCheckForm()
        {
            InitializeComponent();
            lbStatus.Text = "";
            if (Zugriff.Instance.IsAdmin)
            {
                chkEigeneNoten.Checked = false;
                chkEigeneNoten.Enabled = false;
                chkErzeugeVorkommnisse.Visible = true;
            }
            comboBoxZeitpunkt.SelectedIndex = Zugriff.Instance.aktZeitpunkt-1;
        }

        private void btnUnterpunktungen_Click(object sender, EventArgs e)
        {            
            var contr = new NotenCheckController(GetZeitpunkt(),chkEigeneNoten.Checked,chkErzeugeVorkommnisse.Checked);
            // Check für alle eigenen Schüler durchführen
            progressBarChecks.Maximum = Zugriff.Instance.AnzahlSchueler;
            foreach (var k in Zugriff.Instance.Klassen)
            {
                lbStatus.Text = "Prüfe Klasse " + k.Bezeichnung;
                Refresh(); // Formular aktualisieren
                foreach (var s in k.eigeneSchueler)
                {
                    contr.CheckSchueler(s);
                    progressBarChecks.Increment(1);
                    if (abbrechen) break;
                }
            }            
            contr.CreateResults();
            var r = new ReportNotencheck(contr.res);            
            Close();            
        }

        private Zeitpunkt GetZeitpunkt()
        {
          return (Zeitpunkt)(comboBoxZeitpunkt.SelectedIndex+1);
        }
    }
}
