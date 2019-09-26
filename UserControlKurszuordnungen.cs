using System.Collections.Generic;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlKurszuordnungen : UserControl
  {
    private Schueler schueler;

    public UserControlKurszuordnungen()
    {
      InitializeComponent();
    }

    public UserControlKurszuordnungen(Schueler schueler) : this()
    {
      this.schueler = schueler;
    }

    public Schueler Schueler
    {
      get
      {
        return this.schueler;
      }
      set
      {
        this.schueler = value;
        InitKurse();
      }
    }

    public void InitKurse()
    {
      if (this.schueler == null || this.Schueler.Status == Schuelerstatus.Abgemeldet)
      {
        this.objectListView1.SetObjects(new List<Kurs>());
        this.objectListView2.SetObjects(new List<Kurs>());
        return;
      }
      /*
      List<Kurs> aktuelleKurse = new List<Kurs>();
      foreach (var aKurs in schueler.Kurse)
      {
        aktuelleKurse.Add(aKurs);
      }
      this.objectListView1.SetObjects(aktuelleKurse);
      */
      objectListView1.SetObjects(schueler.Kurse);

      var kurseDerKlasse = schueler.AlleMoeglichenKurse();
      IList<Kurs> moeglicheNeueKurse = new List<Kurs>();
      foreach (var aKurs in kurseDerKlasse)
      {
        if (!schueler.Kurse.Exists(x => x.Id == aKurs.Id))
        {
          moeglicheNeueKurse.Add(new Kurs(aKurs.Id));
        }
      }

      this.objectListView2.SetObjects(moeglicheNeueKurse);     
      
      // Kontrollfelder aktualisieren
      textBoxWahlpflichtfach.Text = schueler.Data.IsWahlpflichtfachNull() ? "" : schueler.Data.Wahlpflichtfach; 
      textBoxFremdsprache2.Text = schueler.Data.IsFremdsprache2Null() ? "" : schueler.Data.Fremdsprache2; 
      textBoxReliOderEthik.Text = schueler.Data.IsReligionOderEthikNull() ? "" : schueler.Data.ReligionOderEthik; 
    }

    private void objectListView1_DoubleClick(object sender, System.EventArgs e)
    {
      var kurs = this.objectListView1.SelectedObject as Kurs;
      if (kurs != null)
      {
        this.schueler.MeldeAb(kurs);
        if(kurs.getFach.Kuerzel=="F") // Abmeldung aus Französisch löscht auch den Fremdsprachenschlüssel
        { 
          schueler.Data.SetFremdsprache2Null();
          schueler.Save();
        }
        InitKurse();
      }
    }

    private void objectListView2_DoubleClick(object sender, System.EventArgs e)
    {
      var kurs = this.objectListView2.SelectedObject as Kurs;
      if (kurs != null)
      {
        this.schueler.MeldeAn(kurs);
        schueler.PasseWahlfachschluesselAn(kurs);
        schueler.Save();
        InitKurse();
      }
    }
  }
}
