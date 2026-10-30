namespace diNo.ViewModels
{
  // Hilfsklasse für die Liste in BuchStandardsatzWindow (Grid-Zeile).
  public class BuchStandardsatzRow
  {
    public BuchStandardsatz Satz { get; }
    public string JahrgangsstufeText => ((int)Satz.Jahrgangsstufe).ToString();
    public string SchulartText => SchulartToText(Satz.Schulart);
    public string ZweigText => Satz.Zweig.HasValue ? Satz.Zweig.Value.ToString() : "alle";
    public string BuchText => Satz.Buch?.Anzeige ?? "";

    public BuchStandardsatzRow(BuchStandardsatz satz)
    {
      Satz = satz;
    }

    private static string SchulartToText(diNo.Schulart? s)
    {
      if (!s.HasValue) return "alle";
      switch (s.Value)
      {
        case diNo.Schulart.Franzoesisch: return "Französisch";
        case diNo.Schulart.Spanisch: return "Spanisch";
        default: return s.Value.ToString();
      }
    }
  }
}
