using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von UserControlKurszuordnungen.cs (WinForms).
  public partial class KurszuordnungenViewModel : ObservableObject
  {
    private Schueler schueler;

    public ObservableCollection<Kurs> AktuelleKurse { get; } = new ObservableCollection<Kurs>();
    public ObservableCollection<Kurs> MoeglicheKurse { get; } = new ObservableCollection<Kurs>();

    [ObservableProperty] private string reliOderEthik = "";

    public void SetSchueler(Schueler value)
    {
      schueler = value;
      InitKurse();
    }

    private void InitKurse()
    {
      AktuelleKurse.Clear();
      MoeglicheKurse.Clear();

      if (schueler == null || schueler.Status == Schuelerstatus.Abgemeldet)
        return;

      foreach (var k in schueler.Kurse)
        AktuelleKurse.Add(k);

      var kurseDerKlasse = schueler.AlleMoeglichenKurse();
      foreach (var aKurs in kurseDerKlasse)
      {
        if (!schueler.Kurse.Exists(x => x.Id == aKurs.Id))
          MoeglicheKurse.Add(new Kurs(aKurs.Id));
      }

      ReliOderEthik = schueler.Data.IsReligionOderEthikNull() ? "" : schueler.Data.ReligionOderEthik;
    }

    [RelayCommand]
    private void MeldeAb(Kurs kurs)
    {
      if (kurs == null) return;
      schueler.MeldeAb(kurs);
      InitKurse();
    }

    [RelayCommand]
    private void MeldeAn(Kurs kurs)
    {
      if (kurs == null) return;
      schueler.MeldeAn(kurs);
      schueler.PasseWahlfachschluesselAn(kurs);
      schueler.Save();
      InitKurse();
    }
  }
}
