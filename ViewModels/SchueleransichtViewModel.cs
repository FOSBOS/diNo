using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von UserControlSchueleransicht.cs (WinForms).
  public partial class SchueleransichtViewModel : ObservableObject
  {
    private Schueler schueler;

    public bool HatVerwaltungsrechte => Zugriff.Instance.HatVerwaltungsrechte;

    [ObservableProperty] private string strasse = "";
    [ObservableProperty] private string plz = "";
    [ObservableProperty] private string ort = "";
    [ObservableProperty] private string telefonnummer = "";
    [ObservableProperty] private string eigeneMail = "";
    [ObservableProperty] private string mailSchule = "";
    [ObservableProperty] private string mailEltern = "";
    [ObservableProperty] private string geburtsdatum = "";
    [ObservableProperty] private string geburtsort = "";
    [ObservableProperty] private string wiederholungen = "";
    [ObservableProperty] private string adresseEltern = "";
    [ObservableProperty] private string bekenntnis = "";
    [ObservableProperty] private string jahrgangsstufe = "";
    [ObservableProperty] private string eintrittAm = "";
    [ObservableProperty] private int statusIndex;
    [ObservableProperty] private DateTime? probezeitBis;
    [ObservableProperty] private DateTime? austrittsdatum;

    public void SetSchueler(Schueler value)
    {
      schueler = value;
      if (schueler == null) return;

      var eigeneAnschrift = schueler.getEigeneAnschrift();
      string s = eigeneAnschrift == null || eigeneAnschrift.IsStrasseNull() ? "" : eigeneAnschrift.Strasse;
      if (eigeneAnschrift != null && !eigeneAnschrift.IsHausnummerNull() && eigeneAnschrift.Hausnummer != "")
        s += " " + eigeneAnschrift.Hausnummer;
      Strasse = s;
      Plz = eigeneAnschrift == null || eigeneAnschrift.IsPLZNull() ? "" : eigeneAnschrift.PLZ;
      Ort = eigeneAnschrift == null || eigeneAnschrift.IsOrtNull() ? "" : eigeneAnschrift.Ort;
      Telefonnummer = eigeneAnschrift == null || eigeneAnschrift.IsTelefonnummerNull() ? "" : eigeneAnschrift.Telefonnummer;
      EigeneMail = eigeneAnschrift == null || eigeneAnschrift.IsEmailNull() ? "" : eigeneAnschrift.Email;

      Geburtsdatum = schueler.Data.IsGeburtsdatumNull() ? "" : schueler.Data.Geburtsdatum.ToString("dd.MM.yyyy");
      Geburtsort = schueler.Data.Geburtsort;
      Wiederholungen = schueler.getWiederholungen();

      Jahrgangsstufe = schueler.EintrittInJahrgangsstufe;
      EintrittAm = schueler.EintrittAm == null ? "" : schueler.EintrittAm.Value.ToString("dd.MM.yyyy");

      var kontaktEltern = new System.Collections.Generic.List<string>();
      foreach (var eltern in schueler.getErziehungsberechtigte())
        kontaktEltern.Add(eltern.VornamePerson + " " + eltern.NachnamePerson);
      AdresseEltern = string.Join("\n", kontaktEltern);
      MailEltern = schueler.GetElternMail();
      Bekenntnis = schueler.Data.IsBekenntnisNull() ? "" : schueler.Data.Bekenntnis;

      ProbezeitBis = schueler.Data.IsProbezeitBisNull() ? (DateTime?)null : schueler.Data.ProbezeitBis;
      Austrittsdatum = schueler.Data.IsAustrittsdatumNull() ? (DateTime?)null : schueler.Data.Austrittsdatum;
      MailSchule = schueler.Data.IsMailSchuleNull() ? "" : schueler.Data.MailSchule;
      // erst zuletzt setzen: cbStatus_SelectedIndexChanged korrigiert ggf. Austrittsdatum, falls
      // Status/Datum inkonsistent gespeichert wurden (wie im WinForms-Original).
      StatusIndex = schueler.Data.Status;
    }

    partial void OnAustrittsdatumChanged(DateTime? value)
    {
      StatusIndex = value.HasValue ? 1 : 0;
    }

    partial void OnStatusIndexChanged(int value)
    {
      if (value == 0) Austrittsdatum = null;
      else if (value == 1 && Austrittsdatum == null) Austrittsdatum = DateTime.Today; // abgemeldet
    }

    [RelayCommand]
    private void ResetProbezeit() => ProbezeitBis = null;

    [RelayCommand]
    private void Save()
    {
      if (schueler == null) return;

      schueler.SaveEigeneAnschrift(Strasse, Plz, Ort, Telefonnummer, EigeneMail);
      schueler.Data.Bekenntnis = Bekenntnis;
      // ReliUnterricht via Kurszuordnung wird automatisch gesetzt!

      if (ProbezeitBis == null) schueler.Data.SetProbezeitBisNull();
      else schueler.Data.ProbezeitBis = ProbezeitBis.Value;
      if (Austrittsdatum == null) schueler.Data.SetAustrittsdatumNull();
      else schueler.Data.Austrittsdatum = Austrittsdatum.Value;

      schueler.Data.Geburtsort = Geburtsort;
      schueler.Data.MailSchule = MailSchule;
      schueler.Data.Status = StatusIndex;

      schueler.Save();
    }
  }
}
