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
      EnableFPA();
      FillFPA();

      pnlSeminar.Enabled = schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn &&
        (Zugriff.Instance.lehrer.HatRolle(Rolle.Seminarfach) || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin));
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        var sem = schueler.Seminarfachnote;
        numSeminarpunkte.Value = sem.IsGesamtnoteNull() ? null : (decimal?)sem.Gesamtnote;
        textBoxSeminarfachthema.Text = sem.IsThemaNull() ? "" : sem.Thema;
      }
      else
      {
        numSeminarpunkte.Value = null;
        textBoxSeminarfachthema.Text = "";        
      }
    }


    private void FillFPA()
    {
      var fpANoten = schueler.FPANoten;
      FillFPAHj(fpANoten[0], numBetrieb1, numAnleitung1, numVertiefung11, numVertiefung21, numVertiefung1, numGesamt1, edStelle1, edBemerkung1);
      FillFPAHj(fpANoten[1], numBetrieb2, numAnleitung2, numVertiefung12, numVertiefung22, numVertiefung2, numGesamt2, edStelle2, edBemerkung2);
      jahrespunkte.Value = fpANoten[1].IsJahrespunkteNull() ? null : (byte?)fpANoten[1].Jahrespunkte;
    }

    private void FillFPAHj(diNoDataSet.FpaRow r, NumericUpDownNullable betrieb, NumericUpDownNullable anleitung,
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

    private void EnableFPA()
    {
      if (pnlFPA.Enabled)
      {
        EnableFPAHj(numVertiefung11, numVertiefung21, numVertiefung1, lbVertiefung11, lbVertiefung21, lbVertiefung1);
        EnableFPAHj(numVertiefung12, numVertiefung22, numVertiefung2, lbVertiefung12, lbVertiefung22, lbVertiefung2);
      }
    }

    private void EnableFPAHj(NumericUpDownNullable vertiefung1, NumericUpDownNullable vertiefung2,
          NumericUpDownNullable vertiefung, Label lbV1, Label lbV2, Label lbV)
    {
      // S, U mit 2 Vertiefungsfächern
      bool v12 = schueler.Zweig == Zweig.Sozial; ; // || schueler.Zweig == Zweig.Umwelt; Anforderung Go/Hä: ABU kann auch nicht immer 50/50 gewertet werden. Deshalb eingeben der Gesamtnote (Gewichtung uns dann egal)

      vertiefung1.Enabled = v12;
      vertiefung2.Enabled = v12;
      vertiefung.Enabled = !v12;
      if (schueler.Zweig == Zweig.Sozial)
      {
        lbV1.Text = "Kunst (2/3)";
        lbV2.Text = "Methoden (1/3)";
        lbV.Text = "Vertiefung gesamt (25%)";
      }
      else if (schueler.Zweig == Zweig.Umwelt)
      {
        lbV1.Text = "Boden (1/2)";
        lbV2.Text = "Ernährung (1/2)";
        lbV.Text = "Vertiefung gesamt (25%)";
      }
      else
      {
        lbV1.Text = "Vertiefung 1";
        lbV2.Text = "Vertiefung 2";
        if (schueler.Zweig == Zweig.Technik)
          lbV.Text = "Technisches Zeichnen (25%)";
        else
          lbV.Text = "Wirtschaftsinformatik (25%)";
      }
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
      FPA.Save(schueler.FPANoten, schueler.Zweig);
      schueler.Save();
      FillFPA();
    }

    private void btnSaveSeminar_Click(object sender, System.EventArgs e)
    {
      var sem = schueler.Seminarfachnote;
      if (numSeminarpunkte.Value == null) sem.SetGesamtnoteNull(); else sem.Gesamtnote = (int)numSeminarpunkte.Value;
      if (textBoxSeminarfachthema.Text == "") sem.SetThemaNull(); else sem.Thema = textBoxSeminarfachthema.Text;
      schueler.Save();
    }
  }
}