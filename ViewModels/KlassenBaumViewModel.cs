using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace diNo.ViewModels
{
  // Flach dargestellter Klassen-/Schülerbaum für KlassenansichtWindow, Ersatz für die
  // hierarchische WinForms-TreeListView (BrightIdeasSoftware). Auf-/Zuklapp-Zustand wird separat
  // von KlassenansichtViewModel gehalten, da Letzteres bewusst unverändert bleiben soll (dessen
  // API arbeitet nur gegen IList von Modellobjekten, unabhängig vom UI-Steuerelement).
  public class KlassenBaumViewModel
  {
    private readonly HashSet<int> aufgeklappteKlassenIds = new HashSet<int>();
    private List<Klasse> klassen = new List<Klasse>();

    public ObservableCollection<TreeRow> Rows { get; } = new ObservableCollection<TreeRow>();

    public void SetKlassen(IEnumerable<Klasse> neueKlassen)
    {
      klassen = neueKlassen?.ToList() ?? new List<Klasse>();
      Rebuild();
    }

    private void Rebuild()
    {
      Rows.Clear();
      foreach (var k in klassen)
      {
        bool aufgeklappt = aufgeklappteKlassenIds.Contains(k.Data.Id);
        Rows.Add(new TreeRow(k, aufgeklappt, Toggle));
        if (aufgeklappt)
          foreach (var s in k.Schueler)
            Rows.Add(new TreeRow(s));
      }
    }

    private void Toggle(Klasse klasse)
    {
      if (!aufgeklappteKlassenIds.Remove(klasse.Data.Id))
        aufgeklappteKlassenIds.Add(klasse.Data.Id);
      Rebuild();
    }
  }
}
