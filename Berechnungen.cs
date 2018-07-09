using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  /// <summary>
  /// Verwaltet durchzuführenden Berechnung am übergebenen Zeitpunkt
  /// </summary>
  public class Berechnungen
  {
    private Zeitpunkt zeitpunkt;
    private Schueler schueler;
    public Berechnungen(Zeitpunkt azeitpunkt)
    {
      zeitpunkt = azeitpunkt;
    }

    public void BerechneSchueler(Schueler s)
    { 
      schueler = s;
      if ((zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA) && s.getKlasse.Jahrgangsstufe > Jahrgangsstufe.Elf)
      {
        s.berechneDNote(false);
        if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          s.berechneDNote(true); // DNote für allg. HSR

        s.Save();
      }
    }

    /// <summary>
    /// Methode macht einen Vorschlag zur Einbringung der Halbjahresleistungen eines Schülers.
    /// </summary>
    /// <param name="s">Der Schueler.</param>
    public static void BerechneEinbringung(Schueler s)
    {
      var sowiesoPflicht = new List<HjLeistung>();
      var einbringen = new List<HjLeistung>();
      var streichen = new List<HjLeistung>();

      foreach (var fachNoten in s.getNoten.alleFaecher)
      {
        if (!fachNoten.getFach.NichtNC) 
        {
          var hjLeistungen = new List<HjLeistung>();
          foreach (HjArt art in Enum.GetValues(typeof(HjArt)))
          {
            var hjLeistung = fachNoten.getHjLeistung(art);
            if (hjLeistung == null)
              continue;

            if (hjLeistung.Art == HjArt.AP || hjLeistung.Art == HjArt.FR /*TODO: Oder Seminarfach oder FpA*/)
            {
              sowiesoPflicht.Add(hjLeistung);
            }
            else
            {
              hjLeistungen.Add(hjLeistung);
            }
          }

          if (hjLeistungen.Count == 4)
          {
            // werfe 11/1 weg
            hjLeistungen.RemoveAll(x=> x.JgStufe == Jahrgangsstufe.Elf && x.Art == HjArt.Hj1);
          }

          // jetzt stehen alle "normalen" Halbjahresleistungen in hjLeistungen.
          // Sortieren, nur eine davon kann gestrichen werden
          hjLeistungen.Sort((x, y) => x.Punkte.CompareTo(y.Punkte));
          einbringen.AddRange(hjLeistungen.GetRange(0, hjLeistungen.Count - 1));
          streichen.Add(hjLeistungen[hjLeistungen.Count - 1]);
        }
      }

      int fehlend = GetNoetigeAnzahl(s) - einbringen.Count;
      streichen.Sort((x, y) => x.Punkte.CompareTo(y.Punkte));
      einbringen.AddRange(streichen.GetRange(0, fehlend));
      streichen.RemoveRange(0, fehlend);

      foreach (var hjLeistung in sowiesoPflicht.Union(einbringen))
      {
        hjLeistung.Einbringen = true;
        hjLeistung.WriteToDB();
      }
      foreach (var hjLeistung in streichen)
      {
        hjLeistung.Einbringen = false;
        hjLeistung.WriteToDB();
      }
    }

    private static int GetNoetigeAnzahl(Schueler s)
    {
      if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        return 16;
      }
      if (s.getKlasse.Schulart == Schulart.BOS)
      {
        return 17;
      }

      return 25; //FOS11


    }
  }
}
