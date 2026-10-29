using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace diNo.ViewModels
{
  // Hilfsklasse für die Liste in BibliothekMahnuebersichtWindow (Grid-Zeile): ein Schüler mit
  // mindestens einer offenen (nicht zurückgegebenen) Buchausleihe.
  public partial class SchuelerMahnungRow : ObservableObject
  {
    public Schueler Schueler { get; }

    public string NameVorname => Schueler.NameVorname;
    public string KlassenBezeichnung => Schueler.KlassenBezeichnung;
    public int AnzahlOffen => Schueler.AktiveAusleihen.Count();
    public string OffeneBuecherText => string.Join(", ", Schueler.AktiveAusleihen.Select(a => a.Buch?.Anzeige));
    public DateTime AeltesteAusgabeAm => Schueler.AktiveAusleihen.Min(a => a.AusgegebenAm);

    [ObservableProperty] private bool isSelected;

    // Absichtlich kein Auto-Refresh der Liste bei Änderung (würde die ObservableCollection
    // mitten aus dem ComboBox-Bindungsupdate heraus verändern) - Button "Aktualisieren" in der
    // View übernimmt das Neuladen nach manuellen Mahnstufe-Änderungen.
    public int MahnstufeValue
    {
      get => (int)Schueler.Mahnstufe;
      set
      {
        if (value == (int)Schueler.Mahnstufe) return;
        Schueler.Mahnstufe = (Mahnstufe)value;
        Schueler.Save();
      }
    }

    public SchuelerMahnungRow(Schueler schueler)
    {
      Schueler = schueler;
    }
  }
}
