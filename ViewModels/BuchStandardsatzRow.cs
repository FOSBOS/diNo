namespace diNo.ViewModels
{
  // Hilfsklasse für die Liste in BuchStandardsatzWindow (Grid-Zeile).
  public class BuchStandardsatzRow
  {
    public BuchStandardsatz Satz { get; }
    public string JahrgangsstufeText => ((int)Satz.Jahrgangsstufe).ToString();
    public string SchulartText => Satz.Schulart.HasValue ? Satz.Schulart.Value.ToString() : "alle";
    public string ZweigText => Satz.Zweig.HasValue ? Satz.Zweig.Value.ToString() : "alle";
    public string BuchText => Satz.Buch?.Anzeige ?? "";

    public BuchStandardsatzRow(BuchStandardsatz satz)
    {
      Satz = satz;
    }
  }
}
