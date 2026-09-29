using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von GlobalesForm.cs (WinForms).
  public partial class GlobalesViewModel : ObservableObject
  {
    private readonly GlobaleStringsTableAdapter ta = new GlobaleStringsTableAdapter();

    public List<diNoDataSet.GlobaleStringsRow> Rows { get; }

    [ObservableProperty]
    private diNoDataSet.GlobaleStringsRow selectedRow;

    [ObservableProperty]
    private string bezeichnung = "";

    [ObservableProperty]
    private string wert = "";

    public GlobalesViewModel()
    {
      Rows = ta.GetData().Cast<diNoDataSet.GlobaleStringsRow>().ToList();
      SelectedRow = Rows.Count > 0 ? Rows[0] : null; // WinForms ListBox.DataSource wählt automatisch das 1. Element
    }

    partial void OnSelectedRowChanged(diNoDataSet.GlobaleStringsRow value)
    {
      if (value == null) return;
      Bezeichnung = value.Bezeichnung;
      Wert = value.Wert;
    }

    [RelayCommand]
    private void Save()
    {
      if (SelectedRow == null) return;
      SelectedRow.Wert = Wert;
      ta.Update(SelectedRow);
      Zugriff.Instance.RefreshGlobalesStrings();
    }
  }
}
