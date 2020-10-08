using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks.Parallel;
using System.Windows.Forms;

namespace diNo
{
  public partial class NotenCheckForm : BasisForm
  {
    private Dictionary<NotenCheckModus, string> NotenCheckModusDict;
    private List<Klasse> SelObj;
    public NotenCheckForm(List<Klasse> obj)
    {
      InitializeComponent();
      SelObj = obj;
      lbStatus.Text = "";
      NotenCheckModusDict = new Dictionary<NotenCheckModus, string>();
      NotenCheckModusDict.Add(NotenCheckModus.EigeneNotenVollstaendigkeit, "eigene Noten vollständig?");
      if (Zugriff.Instance.lehrer.KlassenleiterVon != null)
        NotenCheckModusDict.Add(NotenCheckModus.EigeneKlasse, "eigene Klasse prüfen");

      NotenCheckModusDict.Add(NotenCheckModus.Gesamtpruefung, "Gesamtprüfung");
      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin))
      {
        NotenCheckModusDict.Add(NotenCheckModus.KonferenzVorbereiten, "Konferenz vorbereiten");
        NotenCheckModusDict.Add(NotenCheckModus.Protokolle, "Protokolle Klassenkonferenz");
      }
      comboBoxCheckModus.BeginUpdate();
      comboBoxCheckModus.DataSource = NotenCheckModusDict.ToList();
      comboBoxCheckModus.DisplayMember = "Value";
      comboBoxCheckModus.ValueMember = "Key";
      comboBoxCheckModus.EndUpdate();

      comboBoxZeitpunkt.SelectedIndex = Zugriff.Instance.aktZeitpunkt - 1;

      comboBoxZeitpunkt.Enabled = Zugriff.Instance.HatVerwaltungsrechte || Zugriff.Instance.HatRolle(Rolle.Schulleitung);
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      StarteNotenCheck();
    }

    /*
    private void StarteBerechnungen()
    {
      var contr = new Berechnungen(GetZeitpunkt());
      progressBarChecks.Maximum = Zugriff.Instance.AnzahlSchueler;
      foreach (var k in Zugriff.Instance.Klassen)
      {
        //if (k.Data.Id!=62) continue; // zum Test

        lbStatus.Text = "Berechne Klasse " + k.Bezeichnung;
        Refresh(); // Formular aktualisieren
        foreach (var s in k.eigeneSchueler)
        {
          contr.BerechneSchueler(s);
          progressBarChecks.Increment(1);          
        }
      }                  
      Close();      
    }
    */

    private void StarteNotenCheck()
    {
      var contr = new NotenCheckController(GetZeitpunkt(), (NotenCheckModus)comboBoxCheckModus.SelectedValue, progressBarChecks, SelObj);
      progressBarChecks.Maximum = contr.AnzahlSchueler;
      if (contr.zuPruefendeKlassen.Count == 0)
      {
        MessageBox.Show("Diese Klasse muss zu diesem Zeitpunkt nicht geprüft werden.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      // Check für alle eigenen Schüler durchführen       
      /*Partitioner.Create(0, contr.zuPruefendeKlassen.Count)
      Parallel.ForEach(contr.zuPruefendeKlassen, k => 
       {
         contr.CheckKlasse(k);
       }
       );
       */

      foreach (var k in contr.zuPruefendeKlassen)
      {
        lbStatus.Text = "Prüfe Klasse " + k.Bezeichnung;
        Refresh(); // Formular aktualisieren
        contr.CheckKlasse(k);
      }

      Close();
      contr.ShowResults();


      if ((NotenCheckModus)comboBoxCheckModus.SelectedValue == NotenCheckModus.KonferenzVorbereiten)
        Zugriff.Instance.SchuelerRep.Clear(); // Berechnungsdaten neu laden
    }

    private Zeitpunkt GetZeitpunkt()
    {
      return (Zeitpunkt)(comboBoxZeitpunkt.SelectedIndex + 1);
    }
  }
}
