using System.Collections.Generic;

namespace diNo
{
	public enum Jahrgangsstufe
	{
		None = 0,
		Vorklasse = 1,
		Elf = 2,
		Zwoelf = 3,
		Dreizehn = 4,
    Vorkurs = 5,
    ALLE = 15
	}

	public enum Schulart
	{
 		None = 0,
		FOS = 1,
		BOS = 2,
    ALLE = 15
	}

	public enum Zweig
	{
		None = 0,
		Sozial = 1,
		Technik = 2,
		Wirtschaft = 3,
    Agrar = 4,
    ALLE = 15
	}

  /// <summary>
  /// Ein Kurs.
  /// </summary>
  public class Kurs
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="id">Id des Kurses in der Datenbank.</param>
    /// <param name="name"> Name der Klasse bzw. des Kurses, z. B. Mathematik F11Wa.</param>
    /// <param name="fach">Das Fach.</param>
    public Kurs(int id, string name, string fach)
    {
      this.Schueler = new List<Schueler>();
      this.Id = id;
      this.Name = name;
      this.Fach = fach;
    }

    /// <summary>
    /// Id des Kurses in der Datenbank.
    /// </summary>
    public int Id
    {
      get;
      set;
    }

    /// <summary>
    /// Name der Klasse, z. B. Mathematik F11Wa.
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Die Liste der Schüler dieser Klasse.
    /// </summary>
    public IList<Schueler> Schueler
    {
      get;
      private set;
    }

    /// <summary>
    /// Das Fach, z. B. Englisch.
    /// </summary>
    public string Fach
    {
      get;
      private set;
    }
  }
}
