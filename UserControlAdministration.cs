using System;
using System.Windows.Forms;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public partial class UserControlAdministration : UserControl
  {
    private Schueler schueler;
    private diNoDataSet.GlobaleKonstantenRow konstanten;

    public UserControlAdministration()
    {
      InitializeComponent();
      if (!Zugriff.Instance.HatRolle(Rolle.Admin))
      {
        groupBoxBerechtigungen.Visible = false;
        groupBoxImport.Visible = false;
        groupBoxExport.Visible = false;
        groupBoxEinstellungen.Visible = false;
      }
      else
      {        
        konstanten = Zugriff.Instance.globaleKonstanten;
        chkSperre.Checked = konstanten.Sperre == 1;
        edSchuljahr.Text = konstanten.Schuljahr.ToString();
        comboBoxZeitpunkt.SelectedIndex = konstanten.aktZeitpunkt-1;
        edBackupPfad.Text = konstanten.BackupPfad;
      }
    }

    public Schueler Schueler
    {
      get
      {
        return schueler;
      }
      set
      {
        this.schueler = value;
        if (this.schueler != null)
        {
        }
      }
    }

    // liefert die fürs Drucken ausgewählten Objekte (einzelne Schüler oder ein Menge von Klassen)
    private object getSelectedObjects()
    {
      // Elternreihenfolge: usercontrol -> Tabpage -> pageControl -> Form Klassenansicht
      var obj =  ((Klassenansicht)(Parent.Parent.Parent)).SelectedObjects();
      if (obj==null)
        MessageBox.Show("Bitte zuerst einen Schüler oder eine/mehrere Klassen markieren.","diNo",MessageBoxButtons.OK,MessageBoxIcon.Information);
      return obj;
    }

    private void btnFrm1_Click(object sender, EventArgs e)
    {
      Form1 frm = new Form1();
      frm.Show();
    }

    private void btnAbiergebnisse_Click(object sender, EventArgs e)
    {
      var obj = getSelectedObjects();
      if (obj!=null)
        new ReportNotendruck(obj,"diNo.rptAbiergebnisse.rdlc").Show();
    }

    private void exportNoten_Click(object sender, EventArgs e)
    {
      SaveFileDialog dia = new SaveFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ImportExportJahresnoten.ExportiereNoten(dia.FileName);
      }
    }

    private void importNoten_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ImportExportJahresnoten.ImportiereNoten(dia.FileName);
      }
    }

    private void btnImportUnterricht_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        UnterrichtExcelReader.ReadUnterricht(dia.FileName);
      }
    }

    private void btnImportSchueler_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        WinSVSchuelerReader.ReadSchueler(dia.FileName);
      }
    }

    private void btnImportKlassenleiter_Click(object sender, EventArgs e)
    {
        new ImportKlassenleiter();
    }

    private void btnKurseLehrer_Click(object sender, EventArgs e)
    {
      new AdminKursLehrerForm().ShowDialog();
    }

    private void btnNotenmitteilung_Click(object sender, EventArgs e)
    {      
      var obj = getSelectedObjects();
      if (obj!=null)
        new ReportNotendruck(obj,"diNo.rptNotenmitteilungA5.rdlc").Show();
    }

    private void btnBerechtigungen_Click(object sender, EventArgs e)
    {
      new AdminBerechtigungenForm().ShowDialog();
    }

    private void btnAttestpflicht_Click(object sender, EventArgs e)
    {
      var obj = getSelectedObjects();
      if (obj is Schueler)
      {
        Schueler s = (Schueler)obj;
        var b = new BriefDaten(s, true);
        b.Betreff = "Attestpflicht";
        b.Inhalt += "da sich im laufenden Schuljahr bei Ihnen die krankheitsbedingten Schulversäumnisse häufen, ";
        b.Inhalt += "werden Sie gemäß §35(3) FOBOSO dazu verpflichtet, künftig jede weitere krankheitsbedingte Abwesenheit ";
        b.Inhalt += "durch ein aktuelles ärztliches Zeugnis (Schulunfähigkeitsbescheinigung) zu belegen.<br><br>";
        b.Inhalt += "Wird das Zeugnis nicht unverzüglich vorgelegt, so gilt das Fernbleiben als unentschuldigt.";
        var KL = s.getKlasse.Klassenleiter;
        b.Unterschrift = KL.Vorname + " " + KL.Nachname + ", " + KL.Dienstbezeichnung;
        b.Unterschrift2 = "Helga Traut, OStDin";
        new ReportBrief(b).Show();

        s.AddVorkommnis(Vorkommnisart.Attestpflicht,"", false);
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      konstanten.Sperre = chkSperre.Checked ? 1 : 0;
      konstanten.Schuljahr = int.Parse(edSchuljahr.Text);
      konstanten.aktZeitpunkt = comboBoxZeitpunkt.SelectedIndex+1;
      konstanten.BackupPfad = edBackupPfad.Text;
      (new GlobaleKonstantenTableAdapter()).Update(konstanten);
    }    
  }
}
