using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von KurseForm.cs (WinForms).
  public partial class KurseViewModel : ObservableObject
  {
    private readonly KursTableAdapter ta = new KursTableAdapter();
    private List<Kurs> t;

    public List<Kurs> Kurse => t;
    public List<Lehrer> AlleLehrer { get; private set; }
    public List<Fach> AlleFaecher { get; private set; }
    public ObservableCollection<KlasseCheckItem> KlassenItems { get; } = new ObservableCollection<KlasseCheckItem>();

    [ObservableProperty] private Kurs selectedKurs;
    [ObservableProperty] private string bezeichnung = "";
    [ObservableProperty] private string kurzbez = "";
    [ObservableProperty] private string idText = "";
    [ObservableProperty] private string zweig = "";
    [ObservableProperty] private bool isUndef = true;
    [ObservableProperty] private bool isMaennlich;
    [ObservableProperty] private bool isWeiblich;
    [ObservableProperty] private int lehrerId;
    [ObservableProperty] private int fachId;
    [ObservableProperty] private bool isSchuelerButtonsEnabled;
    [ObservableProperty] private List<Schueler> kursSchueler = new List<Schueler>();
    [ObservableProperty] private string anzSchuelerText = "";

    public KurseViewModel()
    {
      Init();
    }

    private void Init()
    {
      t = new List<Kurs>();
      var dt = ta.GetData();
      foreach (var d in dt)
        t.Add(new Kurs(d));
      t.Sort((x, y) => x.Kursbezeichnung.CompareTo(y.Kursbezeichnung));
      OnPropertyChanged(nameof(Kurse));

      AlleLehrer = Zugriff.Instance.LehrerRep.getList();
      AlleLehrer.Sort((x, y) => x.KompletterName.CompareTo(y.KompletterName));
      OnPropertyChanged(nameof(AlleLehrer));

      AlleFaecher = Zugriff.Instance.FachRep.getList();
      AlleFaecher.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
      OnPropertyChanged(nameof(AlleFaecher));

      KlassenItems.Clear();
      foreach (Klasse k in Zugriff.Instance.Klassen)
        KlassenItems.Add(new KlasseCheckItem(k, false));

      SelectedKurs = t.Count > 0 ? t[0] : null; // WinForms ListBox.DataSource wählt automatisch das 1. Element
    }

    partial void OnSelectedKursChanged(Kurs value)
    {
      if (value == null) return;
      Bezeichnung = value.Data.Bezeichnung;
      Kurzbez = value.Data.IsKurzbezNull() ? "" : value.Data.Kurzbez;
      IdText = value.Data.Id.ToString();
      Zweig = value.Data.IsZweigNull() ? "" : value.Data.Zweig;
      IsUndef = value.Data.IsGeschlechtNull();
      IsMaennlich = !value.Data.IsGeschlechtNull() && value.Data.Geschlecht == "M";
      IsWeiblich = !value.Data.IsGeschlechtNull() && value.Data.Geschlecht == "W";

      LehrerId = value.getLehrer.Id; // in der ComboBox muss als ValueMember Id stehen!!
      FachId = value.getFach.Id;

      bool isWPF = value.getFach.Typ == FachTyp.WPF;
      IsSchuelerButtonsEnabled = !isWPF;

      foreach (var item in KlassenItems)
        item.IsChecked = value.Klassen.Contains(item.Klasse);

      InitSchueler();
    }

    private void InitSchueler()
    {
      if (SelectedKurs == null) return;
      KursSchueler = SelectedKurs.Schueler.ToList();
      AnzSchuelerText = SelectedKurs.Schueler.Count + " Schüler im Kurs";
    }

    private static string F(string s) => s == "" ? null : s;

    [RelayCommand]
    private void Save()
    {
      if (SelectedKurs != null)
      {
        var q = SelectedKurs;
        q.Data.Bezeichnung = Bezeichnung;
        q.Data.Kurzbez = Kurzbez;
        if (Zweig == "") q.Data.SetZweigNull(); else q.Data.Zweig = Zweig;
        if (IsUndef) q.Data.SetGeschlechtNull();
        else if (IsMaennlich) q.Data.Geschlecht = "M";
        else q.Data.Geschlecht = "W";

        q.Data.Id = int.Parse(IdText);
        q.Data.LehrerId = LehrerId;
        q.Data.FachId = FachId;
        q.SetLehrerNull();
        q.SetFachNull();
        ta.Update(q.Data);

        q.Klassen.Clear();
        foreach (var item in KlassenItems)
          if (item.IsChecked) q.Klassen.Add(item.Klasse);
        q.SaveKlassenzuordnung();
      }
      else
      {
        try
        {
          ta.Insert(int.Parse(IdText), F(Bezeichnung), LehrerId, FachId, F(Zweig),
            IsUndef ? null : (IsMaennlich ? "M" : "W"), Kurzbez == "" ? null : Kurzbez);
          Init();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "diNo", MessageBoxButton.OK);
        }
      }
    }

    [RelayCommand]
    private void Delete()
    {
      if (SelectedKurs == null) return;
      var q = SelectedKurs;
      if (MessageBox.Show("Soll der Kurs " + q.Data.Bezeichnung + " gelöscht werden?", "Löschen?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        try
        {
          Zugriff.Instance.KursRep.Remove(q.Data.Id);
          ta.DeleteById(q.Data.Id);
          Init();
        }
        catch
        {
          MessageBox.Show("Dieser Kurs konnte nicht gelöscht werden, weil er Beziehungen zu Klassen, Lehrern oder Schülern besitzt.", "diNo", MessageBoxButton.OK);
        }
      }
    }

    [RelayCommand]
    private void Add()
    {
      SelectedKurs = null;
      Bezeichnung = "";
      Kurzbez = "";
      Zweig = "";
      IsUndef = true;
      IsMaennlich = false;
      IsWeiblich = false;
      IdText = "";
      foreach (var item in KlassenItems) item.IsChecked = false;
    }

    [RelayCommand]
    private void ErzeugeExcel()
    {
      if (SelectedKurs == null) return;
      using (var datei = new ErzeugeExcelDateien())
      {
        datei.ErzeugeNeueExcelDatei(SelectedKurs.Data);
      }
    }

    [RelayCommand]
    private void DeleteSchueler()
    {
      if (SelectedKurs == null) return;
      foreach (Schueler s in SelectedKurs.Schueler)
        s.MeldeAb(SelectedKurs);
      SelectedKurs.ResetSchueler();
      InitSchueler();
    }

    [RelayCommand]
    private void SchuelerZuteilen()
    {
      if (SelectedKurs == null) return;
      foreach (Klasse k in SelectedKurs.Klassen)
        foreach (Schueler s in k.Schueler)
          if (s.KursPasstZumSchueler(SelectedKurs))
            s.MeldeAn(SelectedKurs);
      SelectedKurs.ResetSchueler();
      InitSchueler();
    }
  }
}
