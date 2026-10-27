using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using System.Linq;

namespace diNo
{
  /// <summary>
  /// Kapselt ein Buch (Stammdaten) der Schulbibliothek.
  /// </summary>
  public class Buch
  {
    private diNoDataSet.BuecherRow data;

    public Buch(diNoDataSet.BuecherRow data)
    {
      this.data = data;
    }

    public int Id => data.Id;

    public string Fach
    {
      get => data.IsFachNull() ? "" : data.Fach;
      set { if (string.IsNullOrEmpty(value)) data.SetFachNull(); else data.Fach = value; }
    }

    public string Titel
    {
      get => data.Titel;
      set => data.Titel = value;
    }

    public string ISBN
    {
      get => data.IsISBNNull() ? "" : data.ISBN;
      set { if (string.IsNullOrEmpty(value)) data.SetISBNNull(); else data.ISBN = value; }
    }

    public string Autor
    {
      get => data.IsAutorNull() ? "" : data.Autor;
      set { if (string.IsNullOrEmpty(value)) data.SetAutorNull(); else data.Autor = value; }
    }

    public string Verlag
    {
      get => data.IsVerlagNull() ? "" : data.Verlag;
      set { if (string.IsNullOrEmpty(value)) data.SetVerlagNull(); else data.Verlag = value; }
    }

    public decimal? Preis
    {
      get => data.IsPreisNull() ? (decimal?)null : data.Preis;
      set { if (value == null) data.SetPreisNull(); else data.Preis = value.Value; }
    }

    public int Bestand
    {
      get => data.Bestand;
      set => data.Bestand = value;
    }

    public bool Aktiv
    {
      get => data.Aktiv;
      set => data.Aktiv = value;
    }

    /// <summary>
    /// Zeile für Auswahllisten (ComboBox), z.B. "E4 Freeway (11/12 FOS/BOS)".
    /// </summary>
    public string Anzeige => string.IsNullOrEmpty(Fach) ? Titel : Fach + " " + Titel;

    public int AusgeliehenAnzahl => Buecher.Instance.AusgeliehenAnzahl(this);

    public void Save()
    {
      new BuecherTableAdapter().Update(data);
    }

    public static void Insert(string fach, string titel, string isbn, string autor, string verlag, decimal? preis, int bestand)
    {
      new BuecherTableAdapter().Insert(
        string.IsNullOrEmpty(fach) ? null : fach,
        titel,
        string.IsNullOrEmpty(isbn) ? null : isbn,
        string.IsNullOrEmpty(autor) ? null : autor,
        string.IsNullOrEmpty(verlag) ? null : verlag,
        preis,
        bestand,
        true);
      Buecher.Instance.Refresh();
    }
  }

  /// <summary>
  /// Cache aller Bücher (Stammdaten) der Schulbibliothek, analog zum Muster von Vorkommnisse.Instance.
  /// </summary>
  public class Buecher
  {
    private static Buecher _Instance = null;

    public List<Buch> Liste { get; private set; }

    public static Buecher Instance
    {
      get
      {
        if (_Instance == null) _Instance = new Buecher();
        return _Instance;
      }
    }

    private Buecher()
    {
      Refresh();
    }

    public void Refresh()
    {
      Liste = new List<Buch>();
      foreach (var row in new BuecherTableAdapter().GetData())
        Liste.Add(new Buch(row));
      Liste.Sort((x, y) => x.Anzeige.CompareTo(y.Anzeige));
    }

    /// <summary>
    /// Alle aktiven (nicht ausgemusterten) Bücher, z.B. für Auswahllisten.
    /// </summary>
    public List<Buch> Aktive => Liste.Where(b => b.Aktiv).ToList();

    public Buch Find(int id) => Liste.FirstOrDefault(b => b.Id == id);

    /// <summary>
    /// Wie viele Exemplare dieses Buches aktuell ausgeliehen (nicht zurückgegeben) sind - rein
    /// informativ, keine Bestandsprüfung/-blockade beim Einbuchen.
    /// </summary>
    public int AusgeliehenAnzahl(Buch buch)
    {
      return new SchuelerBuecherTableAdapter().GetAusgeliehenAnzahlByBuchId(buch.Id) ?? 0;
    }
  }
}
