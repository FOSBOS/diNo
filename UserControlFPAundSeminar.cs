using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlFPAundSeminar : UserControl
  {
    private Schueler schueler;

    public UserControlFPAundSeminar()
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
          Init();
        }
      }
    }

    private bool IstFpaAenderbar()
    {
      if (schueler.getKlasse.Jahrgangsstufe != Jahrgangsstufe.Elf)
      {
        return false;
      }

      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin))
      {
        return true;
      }

      return (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAAgrar) && schueler.Zweig == Zweig.Agrar) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpASozial) && schueler.Zweig == Zweig.Sozial) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpATechnik) && schueler.Zweig == Zweig.Technik) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAWirtschaft) && schueler.Zweig == Zweig.Wirtschaft);
    }

    public void Init()
    {
      pnlFPA.Enabled = IstFpaAenderbar();
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
      {        
        var fpANoten = schueler.FPANoten;
        textBoxFpABemerkung.Text = fpANoten.IsBemerkungNull() ? "" : fpANoten.Bemerkung;
        cbFPAErfolg1Hj.SelectedIndex = fpANoten.IsErfolg1HjNull() ? 0 : fpANoten.Erfolg1Hj;
        cbFPAErfolg.SelectedIndex = fpANoten.IsErfolgNull() ? 0 : fpANoten.Erfolg;
        numPunkte.Value = fpANoten.IsPunkteNull() ? null : (decimal?)fpANoten.Punkte;
        numPunkte1Hj.Value = fpANoten.IsPunkte1HjNull() ? null : (decimal?)fpANoten.Punkte1Hj;
        numPunkte2Hj.Value = fpANoten.IsPunkte2HjNull() ? null : (decimal?)fpANoten.Punkte2Hj;
      }
      else
      {
        pnlFPA.Enabled = false;
        textBoxFpABemerkung.Text = "";
        cbFPAErfolg1Hj.SelectedIndex = 0;
        cbFPAErfolg.SelectedIndex = 0;
        numPunkte.Value = null;
        numPunkte1Hj.Value = null;
        numPunkte2Hj.Value = null;
      }

      pnlSeminar.Enabled = schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn && 
        (Zugriff.Instance.lehrer.HatRolle(Rolle.Seminarfach) || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin));        
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        var sem = schueler.Seminarfachnote;
        numSeminarpunkte.Value = sem.IsGesamtnoteNull() ? null : (decimal?)sem.Gesamtnote;
        textBoxSeminarfachthemaKurz.Text = sem.IsThemaKurzNull() ? "" : sem.ThemaKurz;
        textBoxSeminarfachthemaLang.Text = sem.IsThemaLangNull() ? "" : sem.ThemaLang;

      }
      else
      {
        numSeminarpunkte.Value = null;
        textBoxSeminarfachthemaKurz.Text = "";
        textBoxSeminarfachthemaLang.Text = "";
      }
    }
   
    // gibt zu den FAP-Rohpunkten die zugehörige Erfolgsnote aus
    private int FPAErfolgErmitteln(decimal? Punkte)
    {
      if (Punkte== null) return 0;
      else if (Punkte>=28) return 1; // sehr gutem Erfolg
      else if (Punkte>=23) return 2;
      else if (Punkte>=14) return 3;
      else return 4; // ohne Erfolg
    }

    private void FPAGesamtnoteErmitteln()
    {
      if (numPunkte1Hj.Value!=null && numPunkte2Hj.Value!=null)
      {
        numPunkte.Value = (decimal)System.Math.Ceiling(((double)(numPunkte1Hj.Value + numPunkte2Hj.Value))/2);
        cbFPAErfolg.SelectedIndex = FPAErfolgErmitteln(numPunkte.Value);
      }
      else
      {
        numPunkte.Value = null;
        cbFPAErfolg.SelectedIndex = 0;
      }
    }

    private void numPunkte1Hj_Leave(object sender, System.EventArgs e)
    {
      cbFPAErfolg1Hj.SelectedIndex = FPAErfolgErmitteln(numPunkte1Hj.Value);
      FPAGesamtnoteErmitteln();
    }

    private void numPunkte2Hj_Leave(object sender, System.EventArgs e)
    {
      FPAGesamtnoteErmitteln();
    }

    private void btnSaveFPA_Click(object sender, System.EventArgs e)
    {
      var fpANoten = schueler.FPANoten;
      if (textBoxFpABemerkung.Text == "") fpANoten.SetBemerkungNull(); else fpANoten.Bemerkung = textBoxFpABemerkung.Text;
      if (cbFPAErfolg1Hj.SelectedIndex == 0) fpANoten.SetErfolg1HjNull(); else fpANoten.Erfolg1Hj = cbFPAErfolg1Hj.SelectedIndex;
      if (cbFPAErfolg.SelectedIndex == 0) fpANoten.SetErfolgNull(); else fpANoten.Erfolg = cbFPAErfolg.SelectedIndex;
      if (numPunkte.Value == null) fpANoten.SetPunkteNull(); else fpANoten.Punkte = (int)numPunkte.Value;
      if (numPunkte1Hj.Value == null) fpANoten.SetPunkte1HjNull(); else fpANoten.Punkte1Hj = (int)numPunkte1Hj.Value;
      if (numPunkte2Hj.Value == null) fpANoten.SetPunkte2HjNull(); else fpANoten.Punkte2Hj = (int)numPunkte2Hj.Value;
      schueler.Save();
    }

    private void btnSaveSeminar_Click(object sender, System.EventArgs e)
    {      
      var sem = schueler.Seminarfachnote;
      if (numSeminarpunkte.Value == null) sem.SetGesamtnoteNull(); else sem.Gesamtnote = (int)numSeminarpunkte.Value;
      if (textBoxSeminarfachthemaKurz.Text == "") sem.SetThemaKurzNull(); else sem.ThemaKurz = textBoxSeminarfachthemaKurz.Text;
      if (textBoxSeminarfachthemaLang.Text == "") sem.SetThemaLangNull(); else sem.ThemaLang = textBoxSeminarfachthemaLang.Text;
      schueler.Save();
    }
  }
}