using diNo.diNoDataSetTableAdapters;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace diNo
{
  public partial class Klassenansicht : Form
  {
    private bool aenderungenVorhanden = false;
    private string aktuelleKlasse = "";

    public Klassenansicht()
    {
      InitializeComponent();
    }

    private void Klassenansicht_Load(object sender, EventArgs e)
    {
      // TODO: This line of code loads data into the 'diNoDataSet.SchuelerKurs' table. You can move, or remove it, as needed.
      this.schuelerKursTableAdapter.Fill(this.diNoDataSet.SchuelerKurs);
      // TODO: This line of code loads data into the 'diNoDataSet.Kurs' table. You can move, or remove it, as needed.
      this.kursTableAdapter.Fill(this.diNoDataSet.Kurs);
      this.klasseTableAdapter.Fill(this.diNoDataSet.Klasse);
      this.fKSchuelerToKlasseBindingSource.CurrentChanged += fKSchuelerToKlasseBindingSource_CurrentChanged;
    }

    void fKSchuelerToKlasseBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      var schueler = (fKSchuelerToKlasseBindingSource.Current as DataRowView).Row as diNoDataSet.SchuelerRow;
      BindingList<diNoDataSet.KursRow> list = new BindingList<diNoDataSet.KursRow>();
      foreach (var kurs in new SchuelerKursTableAdapter().GetDataBySchuelerId(schueler.Id))
      {
        list.Add(new KursTableAdapter().GetDataById(kurs.KursId)[0]);
      }

      this.kursBindingSource.DataSource = list;
    }

    private void diNoDataSetBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      var alteListe = this.fKSchuelerToKlasseBindingSource.DataSource as diNo.diNoDataSet.SchuelerDataTable;
      if (aenderungenVorhanden)
      {
        var result = MessageBox.Show("Sie hatten in der Klasse " + aktuelleKlasse + " Änderungen. Wollen Sie speichern?", "Frage", MessageBoxButtons.YesNo);
        if (result == System.Windows.Forms.DialogResult.Yes)
        {
          new SchuelerTableAdapter().Update(alteListe);
        }
      }

      var neueKlasse = ((this.klasseBindingSource.Current as DataRowView).Row as diNoDataSet.KlasseRow);
      var schuelerDerKlasse = new SchuelerTableAdapter().GetDataByKlasse(neueKlasse.Id);
      this.fKSchuelerToKlasseBindingSource.DataSource = schuelerDerKlasse;
      schuelerDerKlasse.RowChanged += schuelerDerKlasse_RowChanged;
      aktuelleKlasse = neueKlasse.Bezeichnung;
      aenderungenVorhanden = false;
    }

    void schuelerDerKlasse_RowChanged(object sender, DataRowChangeEventArgs e)
    {
      aenderungenVorhanden = true;
    }

    private void kursBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      if (fKSchuelerToKlasseBindingSource.Current == null || kursBindingSource.Current == null)
      {
        return;
      }

      var schueler = (fKSchuelerToKlasseBindingSource.Current as DataRowView).Row as diNoDataSet.SchuelerRow;
      var kurs = kursBindingSource.Current as diNoDataSet.KursRow;
      if (kurs == null)
      {
        kurs = (kursBindingSource.Current as DataRowView).Row as diNoDataSet.KursRow;
      }

      BindingList<diNoDataSet.NoteRow> noten = new BindingList<diNoDataSet.NoteRow>();
      foreach (var note in new NoteTableAdapter().GetDataByKursId(kurs.Id))
      {
        noten.Add(note);
      }

      this.noteBindingSource.DataSource = noten;
    }

    private void btnNotenbogenZeigen_Click(object sender, EventArgs e)
    {
      var schueler = (fKSchuelerToKlasseBindingSource.Current as DataRowView).Row as diNoDataSet.SchuelerRow;
      if (schueler != null)
      {
        new Notenbogen(schueler.Id).Show();
      }
    }
  }
}
