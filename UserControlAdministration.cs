using System;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlAdministration : UserControl
  {
    private Schueler schueler;

    public UserControlAdministration()
    {
      InitializeComponent();
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

    private void btnFrm1_Click(object sender, EventArgs e)
    {
      Form1 frm = new Form1();
      frm.Show();
    }

    private void btnAbiergebnisse_Click(object sender, EventArgs e)
    {
      // Elternreihenfolge: usercontrol -> Tabpage -> pageControl -> Form Klassenansicht
      var obj = ((Klassenansicht)(Parent.Parent.Parent)).SelectedObjects();
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
      var obj = ((Klassenansicht)(Parent.Parent.Parent)).SelectedObjects();
      new ReportNotendruck(obj,"diNo.rptNotenmitteilungA5.rdlc").Show();
    }

    private void btnBerechtigungen_Click(object sender, EventArgs e)
    {
      new AdminBerechtigungenForm().ShowDialog();
    }
  }
}
