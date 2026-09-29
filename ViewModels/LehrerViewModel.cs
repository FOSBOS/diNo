using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von LehrerForm.cs (WinForms).
  public partial class LehrerViewModel : ObservableObject
  {
    private readonly LehrerTableAdapter ta = new LehrerTableAdapter();
    private readonly List<diNoDataSet.RolleRow> rollen = new List<diNoDataSet.RolleRow>();
    private List<Lehrer> t;

    public List<Lehrer> Lehrer => t;
    public ObservableCollection<RolleCheckItem> RollenItems { get; } = new ObservableCollection<RolleCheckItem>();

    [ObservableProperty] private diNo.Lehrer selectedLehrer;
    [ObservableProperty] private string nachname = "";
    [ObservableProperty] private string vorname = "";
    [ObservableProperty] private string kuerzel = "";
    [ObservableProperty] private string dienstbez = "";
    [ObservableProperty] private string windowsname = "";
    [ObservableProperty] private string mail = "";
    [ObservableProperty] private bool isMaennlich = true;
    [ObservableProperty] private bool isWeiblich;
    [ObservableProperty] private bool isRollenListEnabled = true;

    public LehrerViewModel()
    {
      Init();
    }

    private void Init()
    {
      rollen.Clear();
      var rta = new RolleTableAdapter();
      foreach (var row in rta.GetData())
        rollen.Add(row);

      RollenItems.Clear();
      foreach (var rolle in rollen)
        RollenItems.Add(new RolleCheckItem(rolle, false));

      t = Zugriff.Instance.LehrerRep.getList();
      t.Sort((x, y) => x.KompletterName.CompareTo(y.KompletterName));
      OnPropertyChanged(nameof(Lehrer));

      SelectedLehrer = t.Count > 0 ? t[0] : null; // WinForms ListBox.DataSource wählt automatisch das 1. Element
    }

    partial void OnSelectedLehrerChanged(diNo.Lehrer value)
    {
      if (value == null) return;
      Nachname = value.Data.Nachname;
      Vorname = value.Data.Vorname;
      Kuerzel = value.Data.Kuerzel;
      Dienstbez = value.Data.Dienstbezeichnung;
      Windowsname = value.Data.Windowsname;
      Mail = value.Data.IsEMailNull() ? "" : value.Data.EMail;
      IsMaennlich = value.Data.Geschlecht == "M";
      IsWeiblich = value.Data.Geschlecht == "W";

      foreach (var item in RollenItems)
        item.IsChecked = value.HatRolle(item.Row.Id);
    }

    public void VornameLostFocus()
    {
      if (!Zugriff.Instance.IsFBKempten) return;
      if (string.IsNullOrEmpty(Mail))
        Mail = Vorname.ToLower() + "." + Nachname.ToLower() + "@fosbos-kempten.de";
      if (string.IsNullOrEmpty(Windowsname) && Vorname.Length > 0)
        Windowsname = Vorname.ToLower()[0] + Nachname.ToLower();
    }

    private static string F(string s) => s == "" ? null : s;

    private void SetBerechtigung(int rolle, bool newValue)
    {
      if (SelectedLehrer == null) return;
      if (newValue && !SelectedLehrer.HatRolle(rolle))
        SelectedLehrer.AddRolle(rolle);
      if (!newValue && SelectedLehrer.HatRolle(rolle))
        SelectedLehrer.RemoveRolle(rolle);
    }

    [RelayCommand]
    private void Save()
    {
      if (SelectedLehrer != null)
      {
        var q = SelectedLehrer;
        q.Data.Nachname = Nachname;
        q.Data.Vorname = Vorname;
        q.Data.Kuerzel = Kuerzel;
        q.Data.Dienstbezeichnung = Dienstbez;
        q.Data.Windowsname = Windowsname;
        if (Mail == "") q.Data.SetEMailNull(); else q.Data.EMail = Mail;
        q.Data.Geschlecht = IsMaennlich ? "M" : "W";
        ta.Update(q.Data);

        foreach (var item in RollenItems)
          SetBerechtigung(item.Row.Id, item.IsChecked);
      }
      else
      {
        try
        {
          ta.Insert(F(Kuerzel), F(Dienstbez), F(Mail), F(Windowsname), F(Vorname), F(Nachname), IsMaennlich ? "M" : "W");
          // TODO: ta. sollte irgendwie verraten, welche Id er in der DB vergeben hat, dann nur den neu laden
          Zugriff.Instance.LehrerRep.Clear();
          Zugriff.Instance.LoadLehrer();
          Init();
        }
        catch
        {
          MessageBox.Show("Dieser Lehrer konnte nicht eingefügt werden, weil nicht alle Pflichtfelder ausgefüllt wurden.", "diNo", MessageBoxButton.OK);
        }

        IsRollenListEnabled = true;
      }
    }

    [RelayCommand]
    private void Delete()
    {
      if (SelectedLehrer == null) return;
      var q = SelectedLehrer;
      if (MessageBox.Show("Soll der Lehrer " + q.KompletterName + " gelöscht werden?", "Löschen?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        try
        {
          Zugriff.Instance.LehrerRep.Remove(q.Data.Id);
          ta.DeleteById(q.Data.Id);
          Init();
        }
        catch
        {
          MessageBox.Show("Dieser Lehrer konnte nicht gelöscht werden, weil er Beziehungen zu Klassen oder Kursen besitzt.", "diNo", MessageBoxButton.OK);
        }
      }
    }

    [RelayCommand]
    private void Add()
    {
      SelectedLehrer = null;
      Nachname = "";
      Vorname = "";
      Kuerzel = "";
      Dienstbez = "";
      Windowsname = "";
      Mail = "";
      IsMaennlich = true;
      IsWeiblich = false;

      foreach (var item in RollenItems) item.IsChecked = false;
      IsRollenListEnabled = false;
    }
  }
}
