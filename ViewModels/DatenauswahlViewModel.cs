using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  public partial class DatenauswahlViewModel : ObservableObject
  {
    public List<string> AuswahlOptions { get; } = new List<string>
    {
      "mit Vorkommnis",
      "von Zubringerschule",
      "die diese Jahrgangsstufe wiederholen",
      "mit Probezeit",
      "mit 2. Fremdsprache"
    };

    public List<KeyValuePair<Vorkommnisart, string>> VorkommnisOptions { get; } =
      Vorkommnisse.Instance.Liste.ToList();

    public List<string> ZubringerschuleOptions { get; } = new List<string>
    {
      "RS", "RS1", "RS2", "RS3a", "RS3b", "GY", "GY0", "GY1", "WS", "F10"
    };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsVorkommnisEnabled))]
    [NotifyPropertyChangedFor(nameof(IsZubringerschuleEnabled))]
    private int selectedAuswahlIndex;

    public bool IsVorkommnisEnabled => SelectedAuswahlIndex == 0;
    public bool IsZubringerschuleEnabled => SelectedAuswahlIndex == 1;

    [ObservableProperty]
    private Vorkommnisart selectedVorkommnisart;

    [ObservableProperty]
    private string zubringerschuleText = "";

    public event EventHandler RequestClose;

    public DatenauswahlViewModel()
    {
      if (VorkommnisOptions.Count > 0)
        SelectedVorkommnisart = VorkommnisOptions[0].Key;
    }

    [RelayCommand]
    private void Ok()
    {
      var erg = Zugriff.Instance.markierteSchueler;
      var liste = new List<Schueler>();

      var auswahlart = (Auswahlart)SelectedAuswahlIndex;
      Zugriff.Instance.selectedAuswahlart = auswahlart;
      Zugriff.Instance.selectedVorkommnisart = SelectedVorkommnisart;
      erg.Clear();
      foreach (var k in Zugriff.Instance.Klassen)
        foreach (var s in k.Schueler)
        {
          if (auswahlart == Auswahlart.Vorkommnis && s.hatVorkommnis(SelectedVorkommnisart) ||
              auswahlart == Auswahlart.Zubringerschule && s.Data.SchulischeVorbildung.StartsWith(ZubringerschuleText) ||
              auswahlart == Auswahlart.Probezeit && !s.Data.IsProbezeitBisNull() ||
              auswahlart == Auswahlart.Wiederholer && s.Wiederholt() ||
              auswahlart == Auswahlart.Fremdsprache2 && !s.Data.IsAndereFremdspr2FachNull())
          {
            erg.Add(s.Id, s);
            liste.Add(s); // je nach Verwendungszweck andere Liste
          }
        }

      RequestClose?.Invoke(this, EventArgs.Empty);

      if (MessageBox.Show(
            "Es wurden " + erg.Count + " Schüler ausgewählt.\nSoll die Standardübersicht ausgegeben werden? \nKlicken Sie nein für einen individuellen Druckvorgang.",
            "dino", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        new ReportSchuelerdruck(liste, Bericht.Auswahlliste).Show();
    }
  }
}
