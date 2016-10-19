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
      if (!Zugriff.Instance.HatRolle(Rolle.Admin))
      {
        groupBoxBerechtigungen.Visible = false;
        groupBoxImport.Visible = false;
        groupBoxExport.Visible = false;
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
      return  ((Klassenansicht)(Parent.Parent.Parent)).SelectedObjects();
    }

    private void btnFrm1_Click(object sender, EventArgs e)
    {
      Form1 frm = new Form1();
      frm.Show();
    }

    private void btnAbiergebnisse_Click(object sender, EventArgs e)
    {
      new ReportNotendruck(getSelectedObjects(),"diNo.rptAbiergebnisse.rdlc").Show();
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
      new ReportNotendruck(getSelectedObjects(),"diNo.rptNotenmitteilungA5.rdlc").Show();
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
        var b = new BriefDaten(s);
        b.Betreff = "Attestpflicht";
        b.Inhalt = "da sich Ihre Versäumnisse häufen, müssen Sie ab sofort jeden Fehltag mit einem ärztlichen Attest belegen.";
        var KL = s.getKlasse.Klassenleiter;
        b.Unterschrift = KL.Vorname + " " + KL.Nachname + ", " + KL.Dienstbezeichnung;
        new ReportBrief(b).Show();

        s.AddVorkommnis(Vorkommnisart.Attestpflicht,"", false);
      }
    }
  }
}
