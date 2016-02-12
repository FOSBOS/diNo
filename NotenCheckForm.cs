using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using diNo;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
    public partial class NotenCheckForm : Form
    {
        private bool abbrechen = false;
        private Dictionary<NotenCheckModus,string> NotenCheckModusDict;

        public NotenCheckForm()
        {
            InitializeComponent();
            lbStatus.Text = "";
            NotenCheckModusDict = new Dictionary<NotenCheckModus,string>();
            if (!Zugriff.Instance.IsAdmin)
            {
              NotenCheckModusDict.Add(NotenCheckModus.EigeneNotenVollstaendigkeit,"eigene Noten vollständig?");
              if (Zugriff.Instance.lehrer.KlassenleiterVon!=null)
                NotenCheckModusDict.Add(NotenCheckModus.EigeneKlasse,"eigene Klasse prüfen");
            }
            NotenCheckModusDict.Add(NotenCheckModus.Gesamtpruefung,"Gesamtprüfung");
            if (Zugriff.Instance.IsAdmin)
            {
              NotenCheckModusDict.Add(NotenCheckModus.VorkommnisseErzeugen,"Vorkommnisse erzeugen");
              NotenCheckModusDict.Add(NotenCheckModus.BerechnungenSpeichern,"Berechnungen speichern");
              btnSetVorbelegung.Visible = true;
            }   
            comboBoxCheckModus.BeginUpdate();
            comboBoxCheckModus.DataSource = NotenCheckModusDict.ToList();
            comboBoxCheckModus.DisplayMember = "Value";
            comboBoxCheckModus.ValueMember = "Key";
            comboBoxCheckModus.EndUpdate();        

            comboBoxZeitpunkt.SelectedIndex = Zugriff.Instance.aktZeitpunkt-1;            
        }

        private void btnUnterpunktungen_Click(object sender, EventArgs e)
        {            
            var contr = new NotenCheckController(GetZeitpunkt(),(NotenCheckModus)comboBoxCheckModus.SelectedValue);
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
            Close();
            if (contr.res.list.Count==0)
              MessageBox.Show("Es traten keine Fehler auf.","diNo",MessageBoxButtons.OK,MessageBoxIcon.Information);
            else
              new ReportNotencheck(contr.res);                                    
        }

        private Zeitpunkt GetZeitpunkt()
        {
          return (Zeitpunkt)(comboBoxZeitpunkt.SelectedIndex+1);
        }

    private void btnSetVorbelegung_Click(object sender, EventArgs e)
    {
      Zugriff.Instance.aktZeitpunkt = (int) GetZeitpunkt();   
    }
  }
}
