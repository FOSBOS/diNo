using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Threading.Tasks.Parallel;
using System.Windows.Forms;
using diNo;
using diNo.diNoDataSetTableAdapters;
using System.Collections.Concurrent;

namespace diNo
{
  public partial class NotenCheckForm : BasisForm
  {
    private bool abbrechen = false;
    private Dictionary<NotenCheckModus,string> NotenCheckModusDict;    

    public NotenCheckForm()
    {
      InitializeComponent();
      lbStatus.Text = "";
      NotenCheckModusDict = new Dictionary<NotenCheckModus,string>();
      NotenCheckModusDict.Add(NotenCheckModus.EigeneNotenVollstaendigkeit,"eigene Noten vollständig?");
      if (Zugriff.Instance.lehrer.KlassenleiterVon!=null)
        NotenCheckModusDict.Add(NotenCheckModus.EigeneKlasse,"eigene Klasse prüfen");

      NotenCheckModusDict.Add(NotenCheckModus.Gesamtpruefung,"Gesamtprüfung");
      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin))
      {
        NotenCheckModusDict.Add(NotenCheckModus.VorkommnisseErzeugen,"Vorkommnisse erzeugen");
        NotenCheckModusDict.Add(NotenCheckModus.BerechnungenSpeichern,"Berechnungen speichern");
      }   
      comboBoxCheckModus.BeginUpdate();
      comboBoxCheckModus.DataSource = NotenCheckModusDict.ToList();
      comboBoxCheckModus.DisplayMember = "Value";
      comboBoxCheckModus.ValueMember = "Key";
      comboBoxCheckModus.EndUpdate();        

      comboBoxZeitpunkt.SelectedIndex = Zugriff.Instance.aktZeitpunkt-1;            
    }

    private void btnStart_Click(object sender, EventArgs e)
    {      
      if ((NotenCheckModus)comboBoxCheckModus.SelectedValue == NotenCheckModus.BerechnungenSpeichern)
        StarteBerechnungen();
      else
        StarteNotenCheck();
    }

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

    private void StarteNotenCheck()
    {                        
      var contr = new NotenCheckController(GetZeitpunkt(),(NotenCheckModus)comboBoxCheckModus.SelectedValue);
      progressBarChecks.Maximum = contr.AnzahlSchueler;
      if (contr.zuPruefendeKlassen.Count == 0)
      {
        MessageBox.Show("Diese Klasse muss zu diesem Zeitpunkt nicht geprüft werden.","diNo",MessageBoxButtons.OK,MessageBoxIcon.Information);
        return;
      }

      // Check für alle eigenen Schüler durchführen       
      /*Partitioner.Create(0, contr.zuPruefendeKlassen.Count)
      Parallel.ForEach(contr.zuPruefendeKlassen, k => 
       {
         foreach (var s in k.eigeneSchueler)
         {
           contr.CheckSchueler(s);           
         }
       });
       */
      
      foreach (var k in contr.zuPruefendeKlassen)
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
        new ReportNotencheck(contr.res).Show();                                    
    }
    
    private Zeitpunkt GetZeitpunkt()
    {
      return (Zeitpunkt)(comboBoxZeitpunkt.SelectedIndex+1);
    }
  }
}
