using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // Eine sichtbare Zeile im flach dargestellten Klassenbaum (KlassenBaumViewModel), Ersatz für
  // die hierarchische WinForms-TreeListView (BrightIdeasSoftware) in KlassenansichtWindow.
  public class TreeRow
  {
    public Klasse Klasse { get; }
    public Schueler Schueler { get; }
    public bool IstKlasse => Klasse != null;
    public bool IstAufgeklappt { get; }
    public string ExpandGlyph => IstAufgeklappt ? "▼" : "▶";
    public object Model => IstKlasse ? (object)Klasse : Schueler;
    public string Bezeichnung => KlassenTreeViewController.SelectValueCol1(Model)?.ToString();
    public ICommand ToggleCommand { get; }

    public TreeRow(Klasse klasse, bool istAufgeklappt, Action<Klasse> toggle)
    {
      Klasse = klasse;
      IstAufgeklappt = istAufgeklappt;
      ToggleCommand = new RelayCommand(() => toggle(klasse));
    }

    public TreeRow(Schueler schueler)
    {
      Schueler = schueler;
    }
  }
}
