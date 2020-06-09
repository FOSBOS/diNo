namespace diNo
{
  public static class Fremdsprachen
  {
    public static string NiveauText(Sprachniveau n)
    {
      switch ((Sprachniveau)n)
      {
        case Sprachniveau.A2: return "A2";
        case Sprachniveau.B1: return "B1";
        case Sprachniveau.B1p: return "B1+";
        case Sprachniveau.B2: return "B2";
        case Sprachniveau.B2p: return "B2+";
        default: return "";
      }
    }

    public static Sprachniveau GetSprachniveau(Kursniveau k, Jahrgangsstufe jg)
    {
      if (jg != Jahrgangsstufe.Dreizehn)
        switch (k)
        {
          case Kursniveau.Anfaenger: return Sprachniveau.A2;
          case Kursniveau.Fortg: return Sprachniveau.B1;
          case Kursniveau.FortgIW: return Sprachniveau.B1p;
          case Kursniveau.Englisch: return Sprachniveau.B2;
          default: return Sprachniveau.None;
        }
      else
        switch (k)
        {
          case Kursniveau.Anfaenger: return Sprachniveau.B1;
          case Kursniveau.Fortg: return Sprachniveau.B1p;
          case Kursniveau.FortgIW: return Sprachniveau.B2;
          case Kursniveau.Englisch: return Sprachniveau.B2p;
          default: return Sprachniveau.None;
        }
    }

    public static Sprachniveau HjToSprachniveau(FachSchuelerNoten f)
    {
      HjLeistung hj = f.getHjLeistung(HjArt.Sprachenniveau);
      if (hj == null) return Sprachniveau.None;
      return (Sprachniveau)hj.Punkte;
    }
  }

  public enum Kursniveau
  {
    None = 0,
    Englisch = 1, // als Prüfungsfach
    FortgIW = 2, // Fortgeschrittene Internat. Wirtschaft
    Fortg = 3, // fortgeführtes WPF aus Realschule
    Anfaenger = 4
  }

  public enum Sprachniveau
  {
    None = 0,
    A2 = 1,
    B1 = 2,
    B1p = 3,
    B2 = 4,
    B2p = 5,
  }

  public enum ZweiteFSArt
  {
    RS = 0,
    ErgPr = 1,
    FFAlt = 2
  }
}
