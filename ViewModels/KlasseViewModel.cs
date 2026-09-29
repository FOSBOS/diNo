using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von KlasseForm.cs (WinForms). btnAdd ist wie im Original
  // funktionslos/deaktiviert (siehe Kommentar dort: nie fertig verdrahtet).
  public partial class KlasseViewModel : ObservableObject
  {
    private List<Klasse> t;

    public List<Klasse> Klassen => t;
    public List<Lehrer> AlleLehrer { get; private set; }
    public List<string> SchulartOptions { get; } = new List<string> { "", "FOS", "BOS" };

    [ObservableProperty] private Klasse selectedKlasse;
    [ObservableProperty] private string bezeichnung = "";
    [ObservableProperty] private string jgStufe = "";
    [ObservableProperty] private string zweig = "";
    [ObservableProperty] private int schulartIndex;
    [ObservableProperty] private int klassenleiterId;

    public KlasseViewModel()
    {
      Init();
    }

    private void Init()
    {
      t = Zugriff.Instance.KlassenRep.getList();
      t.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
      OnPropertyChanged(nameof(Klassen));
      SelectedKlasse = t.Count > 0 ? t[0] : null; // WinForms ListBox.DataSource wählt automatisch das 1. Element

      AlleLehrer = Zugriff.Instance.LehrerRep.getList();
      AlleLehrer.Sort((x, y) => x.KompletterName.CompareTo(y.KompletterName));
      OnPropertyChanged(nameof(AlleLehrer));
    }

    partial void OnSelectedKlasseChanged(Klasse value)
    {
      if (value == null) return;
      Bezeichnung = value.Bezeichnung;
      JgStufe = ((byte)value.Jahrgangsstufe).ToString();
      Zweig = value.Data.IsZweigNull() ? "" : value.Data.Zweig;
      SchulartIndex = (int)value.Schulart;
      KlassenleiterId = value.KlassenleiterId;
    }

    [RelayCommand]
    private void Add()
    {
      SelectedKlasse = null;
      Bezeichnung = "";
      Zweig = "";
      JgStufe = "";
      SchulartIndex = 1;
      KlassenleiterId = 0;
    }

    [RelayCommand]
    private void Delete()
    {
      if (SelectedKlasse == null) return;
      if (MessageBox.Show("Soll die Klasse " + SelectedKlasse.Data.Bezeichnung + " gelöscht werden?", "Löschen?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        try
        {
          Zugriff.Instance.KlassenRep.Remove(SelectedKlasse.Data.Id);
          var ta = new KlasseTableAdapter();
          ta.DeleteById(SelectedKlasse.Data.Id);
          Init();
        }
        catch
        {
          MessageBox.Show("Diese Klasse konnte nicht gelöscht werden, weil sie Beziehungen zu Klassen, Lehrern oder Schülern besitzt.", "diNo", MessageBoxButton.OK);
        }
      }
    }

    [RelayCommand]
    private void Save()
    {
      if (SelectedKlasse == null) return;
      SelectedKlasse.Data.Bezeichnung = Bezeichnung;
      SelectedKlasse.Data.JgStufe = byte.Parse(JgStufe);
      SelectedKlasse.Data.Schulart = (byte)SchulartIndex;
      SelectedKlasse.Data.Zweig = Zweig;
      SelectedKlasse.Data.KlassenleiterId = KlassenleiterId;
      SelectedKlasse.Save();
    }
  }
}
