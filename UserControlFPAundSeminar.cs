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

      return (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAUmwelt) && schueler.Zweig == Zweig.Umwelt) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpASozial) && schueler.Zweig == Zweig.Sozial) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpATechnik) && schueler.Zweig == Zweig.Technik) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAWirtschaft) && schueler.Zweig == Zweig.Wirtschaft);
    }

    public void Init()
    {
      pnlFPA.Enabled = IstFpaAenderbar();
//      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf) immer füllen, ggf. leer
      {        
        var fpANoten = schueler.FPANoten;
        FillFPAHj(fpANoten[0], numBetrieb1, numAnleitung1, numVertiefung11, numVertiefung21, numVertiefung1, numGesamt1, edStelle1, edBemerkung1);
        FillFPAHj(fpANoten[1], numBetrieb2, numAnleitung2, numVertiefung12, numVertiefung22, numVertiefung2, numGesamt2, edStelle2, edBemerkung2);

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
   
    private void FillFPAHj(diNoDataSet.FpaRow r,NumericUpDownNullable betrieb, NumericUpDownNullable anleitung,
      NumericUpDownNullable vertiefung1, NumericUpDownNullable vertiefung2, NumericUpDownNullable vertiefung, NumericUpDownNullable gesamt,
      TextBox stelle, TextBox bemerkung)
    {
      betrieb.Value = r.IsBetriebNull() ? null : (byte?)r.Betrieb;
      anleitung.Value = r.IsAnleitungNull() ? null : (byte?)r.Anleitung;
      vertiefung.Value = r.IsVertiefungNull() ? null : (byte?)r.Vertiefung;
      vertiefung1.Value = r.IsVertiefung1Null() ? null : (byte?)r.Vertiefung1;
      vertiefung2.Value = r.IsVertiefung2Null() ? null : (byte?)r.Vertiefung2;
      gesamt.Value = r.IsGesamtNull() ? null : (byte?)r.Gesamt;
      stelle.Text = r.IsStelleNull() ? "" : r.Stelle;
      bemerkung.Text = r.IsBemerkungNull() ? "" : r.Bemerkung;
    }

    /*
    private void FPAGesamtnoteErmitteln()
    {
      if (numBetrieb1.Value!=null && numPunkte2Hj.Value!=null)
      {
        numPunkte.Value = (decimal)System.Math.Ceiling(((double)(numBetrieb1.Value + numPunkte2Hj.Value))/2);
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
      cbFPAErfolg1Hj.SelectedIndex = FPAErfolgErmitteln(numBetrieb1.Value);
      FPAGesamtnoteErmitteln();
    }

    private void numPunkte2Hj_Leave(object sender, System.EventArgs e)
    {
      FPAGesamtnoteErmitteln();
    }
    */

    private void SaveFPAHj(diNoDataSet.FpaRow r, NumericUpDownNullable betrieb, NumericUpDownNullable anleitung,
      NumericUpDownNullable vertiefung1, NumericUpDownNullable vertiefung2, NumericUpDownNullable vertiefung, NumericUpDownNullable gesamt,
      TextBox stelle, TextBox bemerkung)
    {
      if (betrieb.Value == null) r.SetBetriebNull(); else r.Betrieb = (byte)betrieb.Value;
      if (anleitung.Value == null) r.SetAnleitungNull(); else r.Anleitung = (byte)anleitung.Value;
      if (vertiefung.Value == null) r.SetVertiefungNull(); else r.Vertiefung = (byte)vertiefung.Value;
      if (vertiefung1.Value == null) r.SetVertiefung1Null(); else r.Vertiefung1 = (byte)vertiefung1.Value;
      if (vertiefung2.Value == null) r.SetVertiefung2Null(); else r.Vertiefung2 = (byte)vertiefung2.Value;
      if (gesamt.Value == null) r.SetGesamtNull(); else r.Gesamt = (byte)gesamt.Value;
      if (stelle.Text == "") r.SetStelleNull(); else r.Stelle = stelle.Text;
      if (bemerkung.Text == "") r.SetBemerkungNull(); else r.Bemerkung = bemerkung.Text;      
    }

    private void btnSaveFPA_Click(object sender, System.EventArgs e)
    {
      var fpANoten = schueler.FPANoten;
      SaveFPAHj(fpANoten[0], numBetrieb1, numAnleitung1, numVertiefung11, numVertiefung21, numVertiefung1, numGesamt1, edStelle1, edBemerkung1);
      SaveFPAHj(fpANoten[1], numBetrieb2, numAnleitung2, numVertiefung12, numVertiefung22, numVertiefung2, numGesamt2, edStelle2, edBemerkung2);
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