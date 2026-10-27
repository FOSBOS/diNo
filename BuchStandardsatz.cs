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

    public bool PasstZu(Jahrgangsstufe jg, Schulart schulart, Zweig zweig)
    {
      if (Jahrgangsstufe != jg) return false;
      if (Schulart.HasValue && Schulart.Value != schulart) return false;
      if (Zweig.HasValue && Zweig.Value != zweig) return false;
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
    /// Alle Standardbuchungen, die zu dieser Schülerkonstellation passen.
    /// </summary>
    public List<BuchStandardsatz> GetFuer(Jahrgangsstufe jg, Schulart schulart, Zweig zweig)
    {
      return Liste.Where(s => s.PasstZu(jg, schulart, zweig)).ToList();
    }
  }
}
