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
      var schueler = (fKSchuelerToKlasseBindingSource.Current as DataRowView).Row as diNoDataSet.SchuelerRow;
      var kurs = (kursBindingSource.Current as DataRowView).Row as diNoDataSet.KursRow;

      var schuelerZuKurs = new SchuelerKursTableAdapter().GetDataBySchuelerAndKurs(schueler.Id, kurs.Id)[0];
      var notenZuordnungen = new NoteSchuelerKursTableAdapter().GetDataBySchuelerKursId(schuelerZuKurs.Id);
      BindingList<diNoDataSet.NoteRow> noten = new BindingList<diNoDataSet.NoteRow>();
      foreach (var notenZuordnung in notenZuordnungen)
      {
        noten.Add(new NoteTableAdapter().GetDataById(notenZuordnung.NoteId)[0]);
      }

      this.noteBindingSource.DataSource = noten;
    }
  }
}
