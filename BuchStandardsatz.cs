using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using System.Linq;

namespace diNo
{
  /// <summary>
  /// Eine Standardbuchung: welches Buch (und wie viele Exemplare) Schüler einer bestimmten
  /// Jahrgangsstufe/Schulart/Zweig-Konstellation normalerweise erhalten. Schulart/Zweig NULL
  /// bedeutet "für alle Schularten/Zweige dieser Jahrgangsstufe gültig".
  /// </summary>
  public class BuchStandardsatz
  {
    private diNoDataSet.BuchStandardsatzRow data;

    public BuchStandardsatz(diNoDataSet.BuchStandardsatzRow data)
    {
      this.data = data;
    }

    public int Id => data.Id;

    public Jahrgangsstufe Jahrgangsstufe => (Jahrgangsstufe)data.Jahrgangsstufe;

    public Schulart? Schulart => data.IsSchulartNull() ? (Schulart?)null : (Schulart)data.Schulart;

    public Zweig? Zweig => data.IsZweigNull() ? (Zweig?)null : CharToZweig(data.Zweig);

    public int BuchId => data.BuchId;

    public Buch Buch => Buecher.Instance.Find(data.BuchId);

    // Fachkürzel, mit denen Französisch- bzw. Spanisch-Kurse im Fach/Kurs-Datenmodell geführt werden
    // (siehe Schueler.cs MeldeAn/MeldeAb, ErzeugeExcelDateien.cs). "F-Wi"/"Ffg" sind die
    // fortgeführt-Varianten von Französisch; für Spanisch gibt es keine entsprechende Variante.
    private static readonly string[] franzoesischKuerzel = { "F", "F-Wi", "Ffg" };
    private static readonly string[] spanischKuerzel = { "Sp" };

    private static Zweig CharToZweig(string c)
    {
      switch (c)
      {
        case "S": return diNo.Zweig.Sozial;
        case "T": return diNo.Zweig.Technik;
        case "W": return diNo.Zweig.Wirtschaft;
        case "U": return diNo.Zweig.Umwelt;
        default: return diNo.Zweig.None;
      }
    }

    /// <summary>
    /// Ob dieser Standardsatz auf den übergebenen Schüler zutrifft. Bei Schulart Französisch/Spanisch
    /// wird statt der Klassen-Schulart geprüft, ob der Schüler aktuell einen entsprechenden
    /// Fremdsprachenkurs belegt (unabhängig von Zweig/Schulart der Klasse).
    /// </summary>
    public bool PasstZu(Schueler schueler)
    {
      var klasse = schueler.getKlasse;
      if (Jahrgangsstufe != klasse.Jahrgangsstufe) return false;

      if (Schulart == diNo.Schulart.Franzoesisch)
        return schueler.Kurse.Any(k => franzoesischKuerzel.Contains(k.getFach.Kuerzel));
      if (Schulart == diNo.Schulart.Spanisch)
        return schueler.Kurse.Any(k => spanischKuerzel.Contains(k.getFach.Kuerzel));

      if (Schulart.HasValue && Schulart.Value != klasse.Schulart) return false;
      if (Zweig.HasValue && Zweig.Value != klasse.Zweig) return false;
      return true;
    }
  }

  /// <summary>
  /// Cache aller Standardbuchungen (Konstellation Jahrgangsstufe/Schulart/Zweig -> Bücherliste).
  /// </summary>
  public class BuchStandardsaetze
  {
    private static BuchStandardsaetze _Instance = null;

    public List<BuchStandardsatz> Liste { get; private set; }

    public static BuchStandardsaetze Instance
    {
      get
      {
        if (_Instance == null) _Instance = new BuchStandardsaetze();
        return _Instance;
      }
    }

    private BuchStandardsaetze()
    {
      Refresh();
    }

    public void Refresh()
    {
      Liste = new List<BuchStandardsatz>();
      foreach (var row in new BuchStandardsatzTableAdapter().GetData())
        Liste.Add(new BuchStandardsatz(row));
    }

    /// <summary>
    /// Alle Standardbuchungen, die auf diesen Schüler zutreffen.
    /// </summary>
    public List<BuchStandardsatz> GetFuer(Schueler schueler)
    {
      return Liste.Where(s => s.PasstZu(schueler)).ToList();
    }
  }
}
