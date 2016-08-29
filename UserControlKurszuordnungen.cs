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

      List<Kurs> aktuelleKurse = new List<Kurs>();
      foreach (var aKurs in schueler.Kurse)
      {
        aktuelleKurse.Add(new Kurs(aKurs));
      }

      this.objectListView1.SetObjects(aktuelleKurse);
      this.objectListView1.Enabled = Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || Zugriff.Instance.lehrer.HatRolle(Rolle.Sekretariat);

      var kurseDerKlasse = schueler.AlleMoeglichenKurse();
      IList<Kurs> moeglicheNeueKurse = new List<Kurs>();
      foreach (var aKurs in kurseDerKlasse)
      {
        if (!aktuelleKurse.Exists(x => x.Id == aKurs.Id))
        {
          moeglicheNeueKurse.Add(new Kurs(aKurs.Id));
        }
      }

      this.objectListView2.SetObjects(moeglicheNeueKurse);
      this.objectListView2.Enabled = Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || Zugriff.Instance.lehrer.HatRolle(Rolle.Sekretariat);
    }

    private void objectListView1_DoubleClick(object sender, System.EventArgs e)
    {
      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || Zugriff.Instance.lehrer.HatRolle(Rolle.Sekretariat))
      {
        var kurs = this.objectListView1.SelectedObject as Kurs;
        if (kurs != null)
        {
          this.schueler.MeldeAb(kurs);
          InitKurse();
        }
      }
    }

    private void objectListView2_DoubleClick(object sender, System.EventArgs e)
    {
      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || Zugriff.Instance.lehrer.HatRolle(Rolle.Sekretariat))
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
}
