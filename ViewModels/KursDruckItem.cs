namespace diNo.ViewModels
{
  // Hilfsklasse für die dynamischen Kursliste-Einträge im Druck-Kontextmenü von KlassenansichtWindow
  // (Ersatz für die WinForms-ToolStripMenuItem.Tag-Zuordnung in Klassenansicht.cs).
  public class KursDruckItem
  {
    public Kurs Kurs { get; }
    public string Bezeichnung => "Kursliste " + Kurs.Kursbezeichnung;

    public KursDruckItem(Kurs kurs)
    {
      Kurs = kurs;
    }
  }
}
