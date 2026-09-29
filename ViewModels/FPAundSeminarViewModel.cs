using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von UserControlFPAundSeminar.cs (WinForms). Die Notenpunkte-Felder (bisher
  // NumericUpDownNullable) sind jetzt normale TextBoxen mit NullableIntConverter: leer bleibt
  // "kein Wert" (NULL in der DB), eine ungültige Eingabe wird beim Verlassen des Felds auf 0
  // zurückgesetzt (Entscheidung des Nutzers, um auf das Custom-Control verzichten zu können).
  public partial class FPAundSeminarViewModel : ObservableObject
  {
    private Schueler schueler;

    [ObservableProperty] private bool fpaEnabled;
    [ObservableProperty] private bool seminarEnabled;
    [ObservableProperty] private bool istVertiefung12;
    [ObservableProperty] private string vertiefung1Label = "Vertiefung 1";
    [ObservableProperty] private string vertiefung2Label = "Vertiefung 2";
    [ObservableProperty] private string vertiefungLabel = "Vertiefung gesamt (25%)";

    [ObservableProperty] private int? betrieb1;
    [ObservableProperty] private int? anleitung1;
    [ObservableProperty] private int? vertiefung11;
    [ObservableProperty] private int? vertiefung21;
    [ObservableProperty] private int? vertiefung1;
    [ObservableProperty] private int? gesamt1;
    [ObservableProperty] private string stelle1 = "";
    [ObservableProperty] private string bemerkung1 = "";

    [ObservableProperty] private int? betrieb2;
    [ObservableProperty] private int? anleitung2;
    [ObservableProperty] private int? vertiefung12;
    [ObservableProperty] private int? vertiefung22;
    [ObservableProperty] private int? vertiefung2;
    [ObservableProperty] private int? gesamt2;
    [ObservableProperty] private string stelle2 = "";
    [ObservableProperty] private string bemerkung2 = "";

    [ObservableProperty] private int? jahrespunkte;

    [ObservableProperty] private int? seminarpunkte;
    [ObservableProperty] private string seminarThema = "";

    public void SetSchueler(Schueler value)
    {
      schueler = value;
      if (schueler != null) Init();
    }

    private void Init()
    {
      FpaEnabled = IstFpaAenderbar();
      IstVertiefung12 = schueler.Zweig == Zweig.Sozial; // S mit 2 Vertiefungsfächern; U, T, W je nur 1
      if (schueler.Zweig == Zweig.Sozial)
      {
        Vertiefung1Label = "Kunst (2/3)";
        Vertiefung2Label = "Methoden (1/3)";
        VertiefungLabel = "Vertiefung gesamt (25%)";
      }
      else if (schueler.Zweig == Zweig.Umwelt)
      {
        Vertiefung1Label = "Boden (1/2)";
        Vertiefung2Label = "Ernährung (1/2)";
        VertiefungLabel = "Vertiefung gesamt (25%)";
      }
      else
      {
        Vertiefung1Label = "Vertiefung 1";
        Vertiefung2Label = "Vertiefung 2";
        VertiefungLabel = schueler.Zweig == Zweig.Technik ? "Technisches Zeichnen (25%)" : "Wirtschaftsinformatik (25%)";
      }

      var fpaNoten = schueler.FPANoten;
      FillHj(fpaNoten[0], 1);
      FillHj(fpaNoten[1], 2);
      Jahrespunkte = fpaNoten[1].IsJahrespunkteNull() ? (int?)null : fpaNoten[1].Jahrespunkte;

      SeminarEnabled = schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn &&
        (Zugriff.Instance.lehrer.HatRolle(Rolle.Seminarfach) || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin));
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        var sem = schueler.Seminarfachnote;
        Seminarpunkte = sem.IsGesamtnoteNull() ? (int?)null : sem.Gesamtnote;
        SeminarThema = sem.IsThemaNull() ? "" : sem.Thema;
      }
      else
      {
        Seminarpunkte = null;
        SeminarThema = "";
      }
    }

    private bool IstFpaAenderbar()
    {
      if (schueler.getKlasse.Jahrgangsstufe != Jahrgangsstufe.Elf) return false;
      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin)) return true;

      return (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAUmwelt) && schueler.Zweig == Zweig.Umwelt) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpASozial) && schueler.Zweig == Zweig.Sozial) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpATechnik) && schueler.Zweig == Zweig.Technik) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAWirtschaft) && schueler.Zweig == Zweig.Wirtschaft);
    }

    private void FillHj(diNoDataSet.FpaRow r, int hj)
    {
      int? betrieb = r.IsBetriebNull() ? (int?)null : r.Betrieb;
      int? anleitung = r.IsAnleitungNull() ? (int?)null : r.Anleitung;
      int? vertiefung1 = r.IsVertiefung1Null() ? (int?)null : r.Vertiefung1;
      int? vertiefung2 = r.IsVertiefung2Null() ? (int?)null : r.Vertiefung2;
      int? vertiefung = r.IsVertiefungNull() ? (int?)null : r.Vertiefung;
      int? gesamt = r.IsGesamtNull() ? (int?)null : r.Gesamt;
      string stelle = r.IsStelleNull() ? "" : r.Stelle;
      string bemerkung = r.IsBemerkungNull() ? "" : r.Bemerkung;

      if (hj == 1)
      {
        Betrieb1 = betrieb; Anleitung1 = anleitung; Vertiefung11 = vertiefung1; Vertiefung21 = vertiefung2;
        Vertiefung1 = vertiefung; Gesamt1 = gesamt; Stelle1 = stelle; Bemerkung1 = bemerkung;
      }
      else
      {
        Betrieb2 = betrieb; Anleitung2 = anleitung; Vertiefung12 = vertiefung1; Vertiefung22 = vertiefung2;
        Vertiefung2 = vertiefung; Gesamt2 = gesamt; Stelle2 = stelle; Bemerkung2 = bemerkung;
      }
    }

    private static void SaveHj(diNoDataSet.FpaRow r, int? betrieb, int? anleitung, int? vertiefung1, int? vertiefung2,
      int? vertiefung, string stelle, string bemerkung)
    {
      if (betrieb == null) r.SetBetriebNull(); else r.Betrieb = (byte)betrieb;
      if (anleitung == null) r.SetAnleitungNull(); else r.Anleitung = (byte)anleitung;
      if (vertiefung == null) r.SetVertiefungNull(); else r.Vertiefung = (byte)vertiefung;
      if (vertiefung1 == null) r.SetVertiefung1Null(); else r.Vertiefung1 = (byte)vertiefung1;
      if (vertiefung2 == null) r.SetVertiefung2Null(); else r.Vertiefung2 = (byte)vertiefung2;
      if (stelle == "") r.SetStelleNull(); else r.Stelle = stelle;
      if (bemerkung == "") r.SetBemerkungNull(); else r.Bemerkung = bemerkung;
    }

    [RelayCommand]
    private void SaveFPA()
    {
      var fpaNoten = schueler.FPANoten;
      SaveHj(fpaNoten[0], Betrieb1, Anleitung1, Vertiefung11, Vertiefung21, Vertiefung1, Stelle1, Bemerkung1);
      SaveHj(fpaNoten[1], Betrieb2, Anleitung2, Vertiefung12, Vertiefung22, Vertiefung2, Stelle2, Bemerkung2);
      FPA.Save(schueler.FPANoten, schueler.Zweig);
      schueler.Save();
      Init();
    }

    [RelayCommand]
    private void SaveSeminar()
    {
      var sem = schueler.Seminarfachnote;
      if (Seminarpunkte == null) sem.SetGesamtnoteNull(); else sem.Gesamtnote = Seminarpunkte.Value;
      if (SeminarThema == "") sem.SetThemaNull(); else sem.Thema = SeminarThema;
      schueler.Save();
    }
  }
}
