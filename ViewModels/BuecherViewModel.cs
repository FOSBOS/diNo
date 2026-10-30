using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;

namespace diNo.ViewModels
{
  // Stammdatenverwaltung für Bücher (Bibliotheksmodul), Muster wie LehrerViewModel.
  public partial class BuecherViewModel : ObservableObject
  {
    private readonly BuecherTableAdapter ta = new BuecherTableAdapter();
    private List<Buch> t;

    public List<Buch> BuchListe => t;

    [ObservableProperty] private Buch selectedBuch;
    [ObservableProperty] private string fach = "";
    [ObservableProperty] private string titel = "";
    [ObservableProperty] private string isbnText = "";
    [ObservableProperty] private string autor = "";
    [ObservableProperty] private string verlag = "";
    [ObservableProperty] private string preisText = "";
    [ObservableProperty] private string bestandText = "0";
    [ObservableProperty] private bool aktiv = true;

    public BuecherViewModel()
    {
      Init();
    }

    private void Init()
    {
      Buecher.Instance.Refresh();
      t = Buecher.Instance.Liste;
      OnPropertyChanged(nameof(BuchListe));
      SelectedBuch = t.Count > 0 ? t[0] : null;
    }

    partial void OnSelectedBuchChanged(Buch value)
    {
      if (value == null) return;
      Fach = value.Fach;
      Titel = value.Titel;
      IsbnText = value.ISBN;
      Autor = value.Autor;
      Verlag = value.Verlag;
      PreisText = value.Preis?.ToString() ?? "";
      BestandText = value.Bestand.ToString();
      Aktiv = value.Aktiv;
    }

    [RelayCommand]
    private void Add()
    {
      SelectedBuch = null;
      Fach = "";
      Titel = "";
      IsbnText = "";
      Autor = "";
      Verlag = "";
      PreisText = "";
      BestandText = "0";
      Aktiv = true;
    }

    [RelayCommand]
    private void Delete()
    {
      if (SelectedBuch == null) return;
      if (MessageBox.Show("Soll das Buch \"" + SelectedBuch.Anzeige + "\" gelöscht werden?", "Löschen?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        try
        {
          ta.Delete(SelectedBuch.Id);
          Init();
        }
        catch
        {
          MessageBox.Show("Dieses Buch konnte nicht gelöscht werden, weil noch Ausleihen oder Standardsätze darauf verweisen. Setzen Sie es statt dessen auf \"inaktiv\".", "diNo", MessageBoxButton.OK);
        }
      }
    }

    [RelayCommand]
    private void Save()
    {
      if (string.IsNullOrWhiteSpace(Titel))
      {
        MessageBox.Show("Bitte einen Titel angeben.", "diNo", MessageBoxButton.OK);
        return;
      }
      decimal? preis = null;
      if (!string.IsNullOrWhiteSpace(PreisText))
      {
        if (!decimal.TryParse(PreisText, out var p))
        {
          MessageBox.Show("Ungültiger Preis.", "diNo", MessageBoxButton.OK);
          return;
        }
        preis = p;
      }
      if (!int.TryParse(BestandText, out var bestand)) bestand = 0;

      if (SelectedBuch != null)
      {
        var b = SelectedBuch;
        b.Fach = Fach;
        b.Titel = Titel;
        b.ISBN = IsbnText;
        b.Autor = Autor;
        b.Verlag = Verlag;
        b.Preis = preis;
        b.Bestand = bestand;
        b.Aktiv = Aktiv;
        b.Save();
        Init();
      }
      else
      {
        Buch.Insert(Fach, Titel, IsbnText, Autor, Verlag, preis, bestand);
        Init();
      }
    }
  }
}
