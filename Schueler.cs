
using System.Collections.Generic;
using System.Drawing;

namespace diNo
{
  /// <summary>
  /// Ein Schüler.
  /// </summary>
  public class Schueler
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="id">Die Id des Schülers in der Datenbank.</param>
    /// <param name="vorname">Vorname des Schülers.</param>
    /// <param name="nachname">Nachname des Schülers.</param>
    /// <param name="isLegastheniker">Ob der Schüler einen Legasthenie-Vermerk hat.</param>
    public Schueler(int id, string vorname, string nachname, bool isLegastheniker)
    {
      this.Einzelnoten = new List<Note>();
      this.BerechneteNoten = new BerechneteNote();
      this.BerechneteNotenErstesHalbjahr = new BerechneteNote();
      this.Id = id;
      this.Vorname = vorname;
      this.Nachname = nachname;
      this.IsLegastheniker = isLegastheniker;
    }

    /// <summary>
    /// Die Id des Schülers in der Datenbank.
    /// </summary>
    public int Id
    {
      get;
      internal set;
    }

    /// <summary>
    /// Name und Vorname des Schülers, durch ", " getrennt.
    /// </summary>
    public string Name
    {
      get
      {
        return this.Nachname + ", " + this.Vorname;
      }
    }

    /// <summary>
    /// Vorname des Schülers.
    /// </summary>
    public string Vorname
    {
      get;
      private set;
    }

    /// <summary>
    /// Nachnname des Schülers.
    /// </summary>
    public string Nachname
    {
      get;
      private set;
    }

    /// <summary>
    /// Die Noten des Schülers.
    /// </summary>
    public IList<Note> Einzelnoten
    {
      get;
      private set;
    }

    /// <summary>
    /// Die berechneten Noten des Schülers.
    /// </summary>
    public BerechneteNote BerechneteNoten
    {
      get;
      private set;
    }

    /// <summary>
    /// Die berechneten Noten des Schülers.
    /// </summary>
    public BerechneteNote BerechneteNotenErstesHalbjahr
    {
      get;
      private set;
    }

    /// <summary>
    /// Ob der Schüler Legastheniker ist.
    /// </summary>
    public bool IsLegastheniker
    {
      get;
      set;
    }

    /// <summary>
    /// Toes the string.
    /// </summary>
    /// <returns>String represantation of this.</returns>
    public override string ToString()
    {
      return this.Name;
    }
  }
}
