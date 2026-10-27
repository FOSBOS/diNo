using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;

namespace diNo.ViewModels
{
  // Pflege der Standardbuchungen (Jahrgangsstufe/Schulart/Zweig -> Buch), die beim
  // Einbuchen (einzeln oder für die ganze Schule) als Vorlage dienen.
  public partial class BuchStandardsatzViewModel : ObservableObject
  {
    private readonly BuchStandardsatzTableAdapter ta = new BuchStandardsatzTableAdapter();

    public List<int> JahrgangsstufenOptions { get; } = new List<int> { 8, 9, 10, 11, 12, 13 };

    public List<KeyValuePair<int, string>> SchulartOptions { get; } = new List<KeyValuePair<int, string>>
    {
      new KeyValuePair<int, string>(0, "alle Schularten"),
      new KeyValuePair<int, string>((int)Schulart.FOS, "FOS"),
      new KeyValuePair<int, string>((int)Schulart.BOS, "BOS"),
    };

    public List<KeyValuePair<int, string>> ZweigOptions { get; } = new List<KeyValuePair<int, string>>
    {
      new KeyValuePair<int, string>(0, "alle Zweige"),
      new KeyValuePair<int, string>((int)Zweig.Sozial, "Sozial"),
      new KeyValuePair<int, string>((int)Zweig.Technik, "Technik"),
      new KeyValuePair<int, string>((int)Zweig.Wirtschaft, "Wirtschaft"),
      new KeyValuePair<int, string>((int)Zweig.Umwelt, "Umwelt"),
    };

    public List<Buch> BuchOptions => Buecher.Instance.Aktive;

    public ObservableCollection<BuchStandardsatzRow> Eintraege { get; } = new ObservableCollection<BuchStandardsatzRow>();

    [ObservableProperty] private int jahrgangsstufe = 11;
    [ObservableProperty] private int schulartAuswahl;
    [ObservableProperty] private int zweigAuswahl;
    [ObservableProperty] private Buch selectedBuch;

    public BuchStandardsatzViewModel()
    {
      Refresh();
    }

    private void Refresh()
    {
      BuchStandardsaetze.Instance.Refresh();
      Eintraege.Clear();
      foreach (var s in BuchStandardsaetze.Instance.Liste.OrderBy(x => x.Jahrgangsstufe).ThenBy(x => x.Buch?.Anzeige))
        Eintraege.Add(new BuchStandardsatzRow(s));
    }

    [RelayCommand]
    private void Add()
    {
      if (SelectedBuch == null)
      {
        MessageBox.Show("Bitte zuerst ein Buch auswählen.");
        return;
      }
      byte? schulart = SchulartAuswahl == 0 ? (byte?)null : (byte)SchulartAuswahl;
      string zweig = ZweigAuswahl == 0 ? null : CharFromZweig((Zweig)ZweigAuswahl);
      ta.Insert((byte)Jahrgangsstufe, schulart, zweig, SelectedBuch.Id);
      Refresh();
    }

    [RelayCommand]
    private void Delete(BuchStandardsatzRow row)
    {
      if (row == null) return;
      ta.Delete(row.Satz.Id);
      Refresh();
    }

    private static string CharFromZweig(Zweig z)
    {
      switch (z)
      {
        case Zweig.Sozial: return "S";
        case Zweig.Technik: return "T";
        case Zweig.Wirtschaft: return "W";
        case Zweig.Umwelt: return "U";
        default: return null;
      }
    }
  }
}
