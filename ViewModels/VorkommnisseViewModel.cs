using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von UserControlVorkommnisse.cs (WinForms). Die "Fake-Button"-Löschspalte
  // (bisher über CellEditStarting-Interception auf der ObjectListView simuliert) wird durch
  // einen echten Button in einer DataGridTemplateColumn ersetzt (siehe VorkommnisseView.xaml).
  public partial class VorkommnisseViewModel : ObservableObject
  {
    private Schueler schueler;

    private static readonly Vorkommnisart[] notenRelevanteVorkommnisse =
    {
      Vorkommnisart.Gefaehrdungsmitteilung, Vorkommnisart.starkeGefaehrdungsmitteilung,
      Vorkommnisart.BeiWeiteremAbsinken, Vorkommnisart.KeineVorrueckungserlaubnis,
      Vorkommnisart.NichtZurPruefungZugelassen, Vorkommnisart.Notenausgleich,
      Vorkommnisart.ProbezeitNichtBestanden, Vorkommnisart.NichtBestanden,
      Vorkommnisart.nichtBestandenMAPnichtZugelassen, Vorkommnisart.VorrueckenAufProbe
    };

    public List<KeyValuePair<Vorkommnisart, string>> ArtOptions { get; } = diNo.Vorkommnisse.Instance.Liste.ToList();
    public ObservableCollection<VorkommnisRow> Eintraege { get; } = new ObservableCollection<VorkommnisRow>();

    [ObservableProperty] private Vorkommnisart selectedArt = Vorkommnisart.NotSet;
    [ObservableProperty] private DateTime datum = DateTime.Today;
    [ObservableProperty] private string bemerkung = "";

    public void SetSchueler(Schueler value)
    {
      schueler = value;
      if (schueler != null)
      {
        RefreshVorkommnisse();
        VorbelegungVorkommnistext(); // auch beim Schülerwechsel vorbelegen
      }
    }

    public void RefreshVorkommnisse()
    {
      Eintraege.Clear();
      foreach (var v in schueler.Vorkommnisse)
        Eintraege.Add(new VorkommnisRow(v));
    }

    partial void OnSelectedArtChanged(Vorkommnisart value)
    {
      if (schueler == null) return;
      VorbelegungVorkommnistext();
    }

    private void VorbelegungVorkommnistext()
    {
      Bemerkung = notenRelevanteVorkommnisse.Contains(SelectedArt) ? schueler.getNoten.Unterpunktungen : "";
    }

    [RelayCommand]
    private void AddVorkommnis()
    {
      if (SelectedArt == Vorkommnisart.NotSet)
      {
        MessageBox.Show("Sie müssen erst auswählen, von welcher Art das Vorkommnis ist");
        return;
      }
      if (schueler == null) return;

      schueler.AddVorkommnis(SelectedArt, Datum, Bemerkung, true);
      // ist doch praktisch, weil meistens gleich mehrere ähnliche Vorkommnisse auftreten
      RefreshVorkommnisse();
    }

    [RelayCommand]
    private void DeleteVorkommnis(VorkommnisRow row)
    {
      if (row == null || schueler == null) return;
      if (MessageBox.Show("Soll das Vorkommnis gelöscht werden?", "Löschen?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        schueler.RemoveVorkommnis(row.Vorkommnis.Id);
        Eintraege.Remove(row);
      }
    }
  }
}
