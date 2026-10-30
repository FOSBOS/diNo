using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // Schulweite Sammel-Einbuchung der Standardsätze: erst Vorschau berechnen (welche Schüler
  // welche Standardbücher noch nicht haben), dann erst auf Bestätigung tatsächlich einbuchen.
  public partial class BuchSammelEinbuchenViewModel : ObservableObject
  {
    private List<(Schueler schueler, List<BuchStandardsatz> fehlend)> vorschau = new List<(Schueler schueler, List<BuchStandardsatz> fehlend)>();

    public ObservableCollection<string> VorschauZeilen { get; } = new ObservableCollection<string>();

    [ObservableProperty] private string statusText = "Noch keine Vorschau berechnet.";
    [ObservableProperty] private bool kannEinbuchen;

    [RelayCommand]
    private void VorschauBerechnen()
    {
      vorschau.Clear();
      VorschauZeilen.Clear();
      int anzahlBuecher = 0;

      foreach (var schueler in Zugriff.Instance.SchuelerRep.getList().Where(s => s.Status == Schuelerstatus.Aktiv))
      {
        var fehlend = BuchStandardsaetze.Instance.GetFuer(schueler)
          .Where(s => !schueler.HatAktiveAusleihe(s.BuchId))
          .ToList();
        if (fehlend.Count == 0) continue;

        vorschau.Add((schueler, fehlend));
        anzahlBuecher += fehlend.Count;
        VorschauZeilen.Add(schueler.NameVorname + " (" + schueler.KlassenBezeichnung + "): " + string.Join(", ", fehlend.Select(f => f.Buch?.Anzeige)));
      }

      StatusText = vorschau.Count == 0
        ? "Keine fehlenden Standardbücher gefunden - entweder sind alle Schüler schon versorgt, oder es ist kein Standardsatz hinterlegt."
        : vorschau.Count + " Schüler, " + anzahlBuecher + " neue Ausleihen insgesamt.";
      KannEinbuchen = vorschau.Count > 0;
    }

    [RelayCommand]
    private void JetztEinbuchen()
    {
      if (vorschau.Count == 0) return;
      int anzahlBuecher = vorschau.Sum(v => v.fehlend.Count);
      if (MessageBox.Show("Sollen jetzt " + anzahlBuecher + " Bücher bei " + vorschau.Count + " Schülern eingebucht werden?",
        "Sammel-Einbuchung", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

      Mouse.OverrideCursor = Cursors.Wait;
      try
      {
        foreach (var eintrag in vorschau)
          eintrag.schueler.EinbuchenStandardsatz();
      }
      finally
      {
        Mouse.OverrideCursor = null;
      }

      MessageBox.Show("Sammel-Einbuchung abgeschlossen.");
      vorschau.Clear();
      VorschauZeilen.Clear();
      StatusText = "Noch keine Vorschau berechnet.";
      KannEinbuchen = false;
    }
  }
}
