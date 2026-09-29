using CommunityToolkit.Mvvm.ComponentModel;

namespace diNo.ViewModels
{
  // Hilfsklasse für die Berechtigungs-Checkliste in LehrerWindow (Ersatz für WinForms CheckedListBox).
  public partial class RolleCheckItem : ObservableObject
  {
    public diNoDataSet.RolleRow Row { get; }
    public string Bezeichnung => Row.Bezeichnung;

    [ObservableProperty]
    private bool isChecked;

    public RolleCheckItem(diNoDataSet.RolleRow row, bool isChecked)
    {
      Row = row;
      this.isChecked = isChecked;
    }
  }
}
