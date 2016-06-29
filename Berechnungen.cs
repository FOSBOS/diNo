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
  }
}
