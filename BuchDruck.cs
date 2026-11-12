using System.Collections.Generic;
using System.Linq;

namespace diNo
{
  // Eine Zeile im Bücherliste-Subreport (Mahnung/Rechnung/Bücherinfo): immer nur aktuell
  // offene (nicht zurückgegebene) Ausleihen, siehe Schueler.AktiveAusleihen.
  public class BuchDruck
  {
    public string Titel { get; private set; }
    public string Verlag { get; private set; }
    public string Autor { get; private set; }
    public string Preis { get; private set; }

    public BuchDruck(BuchAusleihe a)
    {
      Titel = a.Buch.Anzeige;
      Verlag = a.Buch.Verlag;
      Autor = a.Buch.Autor;
      Preis = a.Buch.Preis.HasValue ? string.Format("{0:0.00} €", a.Buch.Preis.Value) : "";
    }

    public static List<BuchDruck> Create(Schueler s)
    {
      return s.AktiveAusleihen.OrderBy(a => a.AusgegebenAm).Select(a => new BuchDruck(a)).ToList();
    }

    public static decimal Gesamtbetrag(Schueler s)
    {
      return s.AktiveAusleihen.Sum(a => a.Buch.Preis ?? 0);
    }
  }

  // Kopfdaten für rptBuecherinfo.rdlc (ein Schüler pro Zeile, bewusst kein Schueler direkt
  // gebunden, wie überall sonst im Projekt).
  public class BuecherinfoDruck
  {
    public int Id { get; private set; }
    public string VornameName { get; private set; }
    public string Klasse { get; private set; }

    public BuecherinfoDruck(Schueler s)
    {
      Id = s.Id;
      VornameName = s.VornameName;
      Klasse = s.getKlasse.Bezeichnung;
    }

    public static List<BuecherinfoDruck> Create(List<Schueler> liste)
    {
      return liste.Select(s => new BuecherinfoDruck(s)).ToList();
    }
  }
}
