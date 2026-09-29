using CommunityToolkit.Mvvm.ComponentModel;

namespace diNo.ViewModels
{
  // Hilfsklasse für die Klassen-Checkliste in KurseWindow (Ersatz für WinForms CheckedListBox).
  public partial class KlasseCheckItem : ObservableObject
  {
    public Klasse Klasse { get; }
    public string Bezeichnung => Klasse.Bezeichnung;

    [ObservableProperty]
    private bool isChecked;

    public KlasseCheckItem(Klasse klasse, bool isChecked)
    {
      Klasse = klasse;
      this.isChecked = isChecked;
    }
  }
}
