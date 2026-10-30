namespace diNo.ViewModels
{
  // Hilfsklasse für die Ausleihen-Liste in BibliothekView (Grid-Zeile), analog zu VorkommnisRow.
  public class BuchAusleiheRow
  {
    public BuchAusleihe Ausleihe { get; }
    public string Titel => Ausleihe.Buch?.Anzeige ?? "";
    public System.DateTime AusgegebenAm => Ausleihe.AusgegebenAm;
    public System.DateTime? RueckgabeAm => Ausleihe.RueckgabeAm;
    public bool IstAktiv => Ausleihe.IstAktiv;

    public BuchAusleiheRow(BuchAusleihe ausleihe)
    {
      Ausleihe = ausleihe;
    }
  }
}
