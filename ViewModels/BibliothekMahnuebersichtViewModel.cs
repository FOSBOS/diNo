using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // Übersicht aller Schüler mit offenen (nicht zurückgegebenen) Buchausleihen, filterbar nach
  // Mahnstufe - Vorstufe für die noch zu bauenden Mahn-/Rechnungsreports.
  public partial class BibliothekMahnuebersichtViewModel : ObservableObject
  {
    public List<KeyValuePair<int, string>> MahnstufeFilterOptions { get; } = new List<KeyValuePair<int, string>>
    {
      new KeyValuePair<int, string>(-1, "Alle"),
      new KeyValuePair<int, string>((int)Mahnstufe.Ungemahnt, "ungemahnt"),
      new KeyValuePair<int, string>((int)Mahnstufe.ErsteMahnung, "1. Mahnung"),
      new KeyValuePair<int, string>((int)Mahnstufe.ZweiteMahnung, "2. Mahnung"),
      new KeyValuePair<int, string>((int)Mahnstufe.Rechnung, "Rechnung"),
    };

    public List<KeyValuePair<int, string>> MahnstufeOptions { get; } = new List<KeyValuePair<int, string>>
    {
      new KeyValuePair<int, string>((int)Mahnstufe.Ungemahnt, "ungemahnt"),
      new KeyValuePair<int, string>((int)Mahnstufe.ErsteMahnung, "1. Mahnung"),
      new KeyValuePair<int, string>((int)Mahnstufe.ZweiteMahnung, "2. Mahnung"),
      new KeyValuePair<int, string>((int)Mahnstufe.Rechnung, "Rechnung"),
    };

    public ObservableCollection<SchuelerMahnungRow> Eintraege { get; } = new ObservableCollection<SchuelerMahnungRow>();

    [ObservableProperty] private int mahnstufeFilter = -1;
    [ObservableProperty] private bool alleAusgewaehlt;
    [ObservableProperty] private int neueMahnstufeFuerAuswahl;
    private bool alleAusgewaehltWirdGesetzt;

    public BibliothekMahnuebersichtViewModel()
    {
      Refresh();
    }

    partial void OnMahnstufeFilterChanged(int value) => Refresh();

    partial void OnAlleAusgewaehltChanged(bool value)
    {
      if (alleAusgewaehltWirdGesetzt) return;
      foreach (var row in Eintraege)
        row.IsSelected = value;
    }

    [RelayCommand]
    public void Refresh()
    {
      Eintraege.Clear();
      var schueler = Zugriff.Instance.SchuelerRep.getList()
        .Where(s => s.AktiveAusleihen.Any())
        .Where(s => MahnstufeFilter < 0 || (int)s.Mahnstufe == MahnstufeFilter)
        .OrderBy(s => s.KlassenBezeichnung).ThenBy(s => s.NameVorname);

      foreach (var s in schueler)
        Eintraege.Add(new SchuelerMahnungRow(s));

      alleAusgewaehltWirdGesetzt = true;
      AlleAusgewaehlt = false;
      alleAusgewaehltWirdGesetzt = false;
    }

    [RelayCommand]
    private void MahnstufeFuerAuswahlSetzen()
    {
      var ausgewaehlte = Eintraege.Where(r => r.IsSelected).ToList();
      if (ausgewaehlte.Count == 0)
      {
        MessageBox.Show("Bitte zuerst mindestens einen Schüler auswählen.");
        return;
      }

      var ausgewaehlteIds = ausgewaehlte.Select(r => r.Schueler.Id).ToHashSet();

      foreach (var row in ausgewaehlte)
      {
        row.Schueler.Mahnstufe = (Mahnstufe)NeueMahnstufeFuerAuswahl;
        row.Schueler.Save();
      }
      Refresh();

      // Auswahl nach dem Neuladen wiederherstellen (bei "Alle"-Filter bleiben die Schüler sichtbar;
      // bei einem auf eine Mahnstufe eingeschränkten Filter fallen sie erwartungsgemäß heraus).
      foreach (var row in Eintraege)
        if (ausgewaehlteIds.Contains(row.Schueler.Id)) row.IsSelected = true;
    }
  }
}
