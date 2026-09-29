using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von Klassenansicht.cs (WinForms), Phase 2 der WPF-Umstellung.
  // treeListView1 und die 7 UserControls bleiben vorerst unveränderte WinForms-Controls,
  // die in KlassenansichtWindow per WindowsFormsHost eingebunden sind (siehe Analyse in
  // Claude\UmstellungWPF.md.txt). Dieses ViewModel bekommt daher für die wenigen Stellen,
  // die echten Zugriff auf diese WinForms-Controls brauchen, Callbacks vom Code-Behind
  // übergeben (gleiches Muster wie BriefViewModel/NotenCheckViewModel), der Rest ist normales MVVM.
  public partial class KlassenansichtViewModel : ObservableObject
  {
    private readonly Action<IEnumerable<Klasse>> setTreeRoots;
    private readonly Func<IList> getTreeSelectedObjects;
    private readonly Action<Schueler> pushSchuelerToUserControls;
    private readonly Action refreshVorkommnisseTab;

    private Schueler schueler;
    private List<Schueler> suchListe;
    private int suchIndex = -1;
    private diNo.Views.BriefWindow frmBrief;

    public ObservableCollection<KursDruckItem> KursDruckItems { get; } = new ObservableCollection<KursDruckItem>();

    public bool HatVerwaltungsrechte => Zugriff.Instance.HatVerwaltungsrechte;
    public bool SiehtAlles => Zugriff.Instance.SiehtAlles;
    public bool IsTestDB => Zugriff.Instance.IsTestDB;

    [ObservableProperty] private string nameText = "";
    [ObservableProperty] private string klasseText = "";
    [ObservableProperty] private string hinweiseText = "";
    [ObservableProperty] private BitmapImage bildQuelle;
    [ObservableProperty] private bool briefEnabled;
    [ObservableProperty] private bool notenabgebenEnabled;
    [ObservableProperty] private string statusText = "";
    [ObservableProperty] private string suchText = "";
    [ObservableProperty] private bool suchNichtGefunden;
    [ObservableProperty] private bool nurAktive = true;

    public KlassenansichtViewModel(
      Action<IEnumerable<Klasse>> setTreeRoots,
      Func<IList> getTreeSelectedObjects,
      Action<Schueler> pushSchuelerToUserControls,
      Action refreshVorkommnisseTab)
    {
      this.setTreeRoots = setTreeRoots;
      this.getTreeSelectedObjects = getTreeSelectedObjects;
      this.pushSchuelerToUserControls = pushSchuelerToUserControls;
      this.refreshVorkommnisseTab = refreshVorkommnisseTab;

      foreach (var k in Zugriff.Instance.eigeneKurse)
      {
        Fach f = k.getFach;
        if (f.Typ == FachTyp.WPF || f.Kuerzel == "K" || f.Kuerzel == "Ev" || f.Kuerzel == "Eth")
          KursDruckItems.Add(new KursDruckItem(k));
      }
    }

    public void OnLoaded()
    {
      NotenabgebenEnabled = Zugriff.Instance.Sperre != Sperrtyp.Notenschluss || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin);
      RefreshTreeView();
    }

    private void RefreshTreeView()
    {
      Zugriff.Instance.Refresh(NurAktive);
      setTreeRoots(Zugriff.Instance.Klassen);
      StatusText = "";
    }

    public void OnTreeSelectionChanged(object selectedModel)
    {
      Zugriff.Instance.markierteSchueler.Clear();
      if (!(selectedModel is Schueler s)) return;

      if (schueler == null || schueler.Id != s.Id) // nur Id auslesen, falls Refresh das Objekt verändert hat
      {
        schueler = Zugriff.Instance.SchuelerRep.Find(s.Id);
        UpdateSchuelerAnzeige();
      }
    }

    private void UpdateSchuelerAnzeige()
    {
      if (schueler == null) return;

      NameText = schueler.NameVorname;
      KlasseText = schueler.KlassenBezeichnung;
      BildQuelle = new BitmapImage(new Uri(schueler.Data.Geschlecht == "W"
        ? "pack://application:,,,/Resources/avatarFrau.png"
        : "pack://application:,,,/Resources/avatarMann.png"));
      BriefEnabled = true;
      HinweiseText = schueler.getNTAText;

      pushSchuelerToUserControls(schueler);
    }

    // Entspricht Klassenansicht.SetSchueler() - wird vom Administration-Tab nach Änderungen am
    // aktuellen Schüler aufgerufen, um die Anzeige/anderen Tabs zu aktualisieren (SchuelerChangedNotifier).
    public void RefreshSchuelerAnzeige() => UpdateSchuelerAnzeige();

    // Entspricht Klassenansicht.SelectedKlassen() (nur für Administration/NotenCheck relevant).
    public List<Klasse> SelectedKlassen()
    {
      var res = new List<Klasse>();
      var obj = getTreeSelectedObjects();
      if (obj.Count > 0 && obj[0] is Klasse)
        foreach (Klasse k in obj)
          res.Add(k);
      return res;
    }

    // Entspricht Klassenansicht.SelectedObjects().
    public List<Schueler> SelectedObjects()
    {
      var res = new List<Schueler>();
      var obj = getTreeSelectedObjects();

      if (HatVerwaltungsrechte && Zugriff.Instance.markierteSchueler.Count > 0)
      {
        foreach (Schueler s in Zugriff.Instance.markierteSchueler.Values)
          res.Add(s);
      }
      else if (obj.Count > 0 && obj[0] is Klasse)
      {
        foreach (Klasse k in obj)
          foreach (Schueler s in k.Schueler)
            res.Add(s);
      }
      else if (obj.Count > 0 && obj[0] is Schueler)
      {
        foreach (Schueler s in obj)
          res.Add(s);
      }
      else if (schueler != null)
        res.Add(schueler);

      return res;
    }

    [RelayCommand]
    private void Refresh() => RefreshTreeView();

    partial void OnNurAktiveChanged(bool value) => RefreshTreeView();

    [RelayCommand]
    private void Suchen()
    {
      string cmp = SuchText;
      int c = 0;
      if (suchListe == null) suchListe = Zugriff.Instance.SchuelerRep.getList();
      int max = suchListe.Count;
      SuchNichtGefunden = false;

      do
      {
        suchIndex++;
        if (suchIndex >= max) suchIndex = 0; // wieder von vorn anfangen
        c++; // Endlosschleife verhindern

        if (suchListe[suchIndex].Name.StartsWith(cmp, StringComparison.OrdinalIgnoreCase))
        {
          schueler = suchListe[suchIndex];
          UpdateSchuelerAnzeige();
          break;
        }
      }
      while (c < max);

      if (c >= max) SuchNichtGefunden = true;
    }

    [RelayCommand]
    private void Check()
    {
      var obj = getTreeSelectedObjects();
      var selKlassen = new List<Klasse>();
      if (HatVerwaltungsrechte && obj.Count > 0 && obj[0] is Klasse)
        foreach (Klasse k in obj) selKlassen.Add(k);

      new diNo.Views.NotenCheckWindow(selKlassen).Show();
    }

    [RelayCommand]
    private void Brief()
    {
      if (schueler == null) return;
      if (frmBrief == null) frmBrief = new diNo.Views.BriefWindow(refreshVorkommnisseTab);
      frmBrief.Anzeigen(schueler);
    }

    [RelayCommand]
    private void Notenabgeben()
    {
      var fileDialog = new Microsoft.Win32.OpenFileDialog { Filter = "Excel Files|*.xls*", Multiselect = true };
      if (fileDialog.ShowDialog() != true) return;

      System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
      try
      {
        foreach (string fileName in fileDialog.FileNames)
          new LeseNotenAusExcel(fileName, NotenReaderOnStatusChange);

        Zugriff.Instance.Refresh();
        if (schueler != null)
        {
          schueler = Zugriff.Instance.SchuelerRep.Find(schueler.Id); // neues Objekt setzen
          UpdateSchuelerAnzeige();
        }
        MessageBox.Show("Die Notendateien wurden übertragen.", "diNo", MessageBoxButton.OK, MessageBoxImage.Information);
        StatusText = "";
      }
      finally
      {
        System.Windows.Input.Mouse.OverrideCursor = null;
      }
    }

    private void NotenReaderOnStatusChange(object sender, StatusChangedEventArgs e)
    {
      try { StatusText = e.Meldung; } catch { }
    }

    [RelayCommand]
    private void LnwAbgeben() => new diNo.Views.CopyLNWWindow().ShowDialog();

    [RelayCommand]
    private void Mail()
    {
      var obj = getTreeSelectedObjects();
      if (obj == null || obj.Count == 0)
        MessageBox.Show("Bitte einen Schüler oder eine Klasse auswählen.", "diNo", MessageBoxButton.OK, MessageBoxImage.Exclamation);
      else
        OpenOutlook.BetroffeneLehrer(obj[0]);
    }

    [RelayCommand]
    private void DruKlassenliste()
    {
      var obj = getTreeSelectedObjects();
      if (obj.Count > 0 && obj[0] is Klasse k)
        new ReportSchuelerdruck(k.Schueler, Bericht.Klassenliste).Show();
      else
        MessageBox.Show("Bitte erst eine Klasse auswählen.", "diNo", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void DruNotenbogen()
    {
      var obj = SelectedObjects();
      if (obj.Count == 0 || (!HatVerwaltungsrechte && obj.Count > 1))
      {
        MessageBox.Show("Bitte erst einen Schüler auswählen.", "diNo", MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }
      new ReportSchuelerdruck(obj, Bericht.Notenbogen).Show();
    }

    [RelayCommand]
    private void DruLegastheniker()
    {
      var lst = Zugriff.Instance.SchuelerRep.getList().Where(s => s.HatNachteilsausgleich).ToList();
      new ReportSchuelerdruck(lst, Bericht.Legastheniker).Show();
    }

    [RelayCommand]
    private void DruKursliste(KursDruckItem item)
    {
      if (item != null) new ReportSchuelerdruck(item.Kurs.Schueler, Bericht.Klassenliste).Show();
    }
  }
}
