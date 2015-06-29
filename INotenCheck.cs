using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
  /// <summary>
  /// Interface für Notenprüfungsklassen.
  /// </summary>
  public interface INotenCheck
  {
    /// <summary>
    /// Der Anzeigename des Checkers für den Bildschirm.
    /// </summary>
    string Anzeigename
    {
      get;
    }

    /// <summary>
    /// Die Fehlermeldung, die ausgegeben werden soll, wenn der Notencheck schief geht.
    /// </summary>
    string Fehlermeldung
    {
      get;
    }

    /// <summary>
    /// Prüft die Noten und liefert eine Liste der Schüler-Ids, bei denen ein Problem aufgetreten ist.
    /// </summary>
    /// <param name="schueler">Die Schüler der Klasse.</param>
    /// <returns>Liste der Schüler-Ids, bei denen ein Problem aufgetreten ist.</returns>
    IList<int> GetFehler(IList<int> schueler);
  }

  /// <summary>
  /// Abstrakte Basis für Notenchecker. Regelt Fehlermeldung und Anzeigename.
  /// </summary>
  public abstract class AbstractNotenCheck: INotenCheck
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="anzeigename">Der Anzeigename.</param>
    /// <param name="fehlermeldung">Die Fehlermeldung (wenn der Notencheck schief geht).</param>
    public AbstractNotenCheck(string anzeigename, string fehlermeldung)
    {
      this.Anzeigename = anzeigename;
      this.Fehlermeldung = fehlermeldung;
    }

    /// <summary>
    /// Der Anzeigename des Checkers für den Bildschirm.
    /// </summary>
    public string Anzeigename
    {
      get;
      private set;
    }

    /// <summary>
    /// Die Fehlermeldung, die ausgegeben werden soll, wenn der Notencheck schief geht.
    /// </summary>
    public string Fehlermeldung
    {
      get;
      private set;
    }

    /// <summary>
    /// Prüft die Noten und liefert eine Liste der Schüler-Ids, bei denen ein Problem aufgetreten ist.
    /// </summary>
    /// <param name="schueler">Die Schüler der Klasse.</param>
    /// <returns>Liste der Schüler-Ids, bei denen ein Problem aufgetreten ist.</returns>
    public abstract IList<int> GetFehler(IList<int> schueler);
  }

  /// <summary>
  /// Klasse zur Prüfung, ob alle Schüler einer Klasse ein Fachreferat haben.
  /// </summary>
  public class FachreferatChecker : AbstractNotenCheck
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    public FachreferatChecker(): base("Fachreferat vorhanden", "Es liegt kein Fachreferat vor.")
    {
    }

    /// <summary>
    /// Prüft die Noten und liefert eine Liste der Schüler-Ids, bei denen ein Problem aufgetreten ist.
    /// </summary>
    /// <param name="schueler">Die Schüler der Klasse.</param>
    /// <returns>Liste der Schüler-Ids, bei denen ein Problem aufgetreten ist.</returns>
    public override IList<int> GetFehler(IList<int> schueler)
    {
      throw new NotImplementedException();
    }
  }
}
