using diNo;
using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using System.Windows.Forms;

namespace diNoVerwaltung
{
  public partial class KurszuordnungenForm : Form
  {
    private Schueler schueler;

    public KurszuordnungenForm(Schueler schueler)
    {
      InitializeComponent();
      this.schueler = schueler;
      this.lblSchuelername.Text = schueler.NameVorname + " (Id="+schueler.Id+")";
      InitKurse();
    }

    private void InitKurse()
    {
      List<Kurs> aktuelleKurse = new List<Kurs>();
      foreach (var aKurs in schueler.Kurse)
      {
        aktuelleKurse.Add(new Kurs(aKurs));
      }

      this.objectListView1.SetObjects(aktuelleKurse);

      var kurseDerKlasse = new KlasseKursTableAdapter().GetDataByKlasse(schueler.getKlasse.Data.Id);
      IList<Kurs> moeglicheNeueKurse = new List<Kurs>();
      foreach (var aKurs in kurseDerKlasse)
      {
        if (!aktuelleKurse.Exists(x => x.Id == aKurs.KursId))
        {
          moeglicheNeueKurse.Add(new Kurs(aKurs.KursId));
        }
      }

      this.objectListView2.SetObjects(moeglicheNeueKurse);
    }

    private void objectListView1_DoubleClick(object sender, System.EventArgs e)
    {
      var kurs = this.objectListView1.SelectedObject as Kurs;
      if (kurs != null)
      {
        this.schueler.MeldeAb(kurs);
        InitKurse();
      }
    }

    private void objectListView2_DoubleClick(object sender, System.EventArgs e)
    {
      var kurs = this.objectListView2.SelectedObject as Kurs;
      if (kurs != null)
      {
        this.schueler.MeldeAn(kurs);
        InitKurse();
      }
    }
  }
}
