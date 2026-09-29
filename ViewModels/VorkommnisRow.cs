namespace diNo.ViewModels
{
  // Hilfsklasse für die Vorkommnisse-Liste in VorkommnisseView (Ersatz für die WinForms-
  // ObjectListView-AspectGetter-Spalte "Art" in UserControlVorkommnisse.cs).
  public class VorkommnisRow
  {
    public Vorkommnis Vorkommnis { get; }
    public System.DateTime Datum => Vorkommnis.Datum;
    public string ArtText => Vorkommnisse.Instance.Liste[Vorkommnis.Art];
    public string Bemerkung => Vorkommnis.Bemerkung;

    public VorkommnisRow(Vorkommnis vorkommnis)
    {
      Vorkommnis = vorkommnis;
    }
  }
}
