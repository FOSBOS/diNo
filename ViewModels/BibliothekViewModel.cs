using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // Pro-Schüler-Ansicht der Bibliotheksausleihen: manuelles Einbuchen/Zurückgeben einzelner Bücher,
  // Standardsatz-Einbuchung und die Rückgabefunktion für alle Bücher dieses Schülers.
  public partial class BibliothekViewModel : ObservableObject
  {
    private Schueler schueler;

    public List<Buch> BuchOptions => Buecher.Instance.Aktive;
    public ObservableCollection<BuchAusleiheRow> Eintraege { get; } = new ObservableCollection<BuchAusleiheRow>();

    [ObservableProperty] private Buch selectedBuch;
    [ObservableProperty] private DateTime datum = DateTime.Today;

    public void SetSchueler(Schueler value)
    {
      schueler = value;
      if (schueler != null)
        RefreshAusleihen();
    }

    public void RefreshAusleihen()
    {
      Eintraege.Clear();
      if (schueler == null) return;
      foreach (var a in schueler.Buchausleihen.OrderByDescending(x => x.AusgegebenAm))
        Eintraege.Add(new BuchAusleiheRow(a));
    }

    [RelayCommand]
    private void Einbuchen()
    {
      if (schueler == null) return;
      if (SelectedBuch == null)
      {
        MessageBox.Show("Sie müssen erst ein Buch auswählen.");
        return;
      }
      schueler.Ausleihen(SelectedBuch, Datum);
      RefreshAusleihen();
    }

    [RelayCommand]
    private void Zurueckgeben(BuchAusleiheRow row)
    {
      if (row == null || schueler == null) return;
      schueler.BuchZurueckgeben(row.Ausleihe.Id);
      RefreshAusleihen();
    }

    [RelayCommand]
    private void ZurueckgebenRueckgaengig(BuchAusleiheRow row)
    {
      if (row == null || schueler == null) return;
      schueler.BuchZurueckgebenRueckgaengig(row.Ausleihe.Id);
      RefreshAusleihen();
    }

    [RelayCommand]
    private void AlleZurueckgeben()
    {
      if (schueler == null) return;
      if (!schueler.AktiveAusleihen.Any())
      {
        MessageBox.Show("Es sind derzeit keine Bücher aktiv ausgeliehen.");
        return;
      }
      schueler.AlleBuecherZurueckgeben();
      RefreshAusleihen();
    }

    [RelayCommand]
    private void StandardsatzEinbuchen()
    {
      if (schueler == null) return;
      var neu = schueler.EinbuchenStandardsatz();
      RefreshAusleihen();
      if (neu.Count == 0)
        MessageBox.Show("Es gibt keinen hinterlegten Standardsatz für diese Klasse, oder alle Standardbücher sind bereits eingebucht.");
      else
        MessageBox.Show(neu.Count + " Buch/Bücher gemäß Standardsatz eingebucht.");
    }
  }
}
