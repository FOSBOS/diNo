using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
  /// <summary>
  /// Typ einer Note.
  /// </summary>
  public enum Notentyp
  {
    /// <summary>
    /// Der Default Value.
    /// </summary>
    NONE = 0,
    /// <summary>
    /// Schulaufgabe.
    /// </summary>
    Schulaufgabe = 1,
    /// <summary>
    /// Kurzarbeit
    /// </summary>
    Kurzarbeit = 2,
    /// <summary>
    /// Ex.
    /// </summary>
    Ex = 3,
    /// <summary>
    /// Echte mündliche Note.
    /// </summary>
    EchteMuendliche = 4,
    /// <summary>
    /// Fachreferat.
    /// </summary>
    Fachreferat = 5,
    /// <summary>
    /// Seminararfach.
    /// </summary>
    Seminararfach = 6,
		/// <summary>
		/// Ersatzprüfung.
		/// </summary>
		Ersatzprüfung = 7,
    /// <summary>
    /// Durchschnittsnote der Schulaufgaben.
    /// </summary>
    SchnittSA = 8,
    /// <summary>
    /// Durchschnittsnote aller mündlichen und Exen.
    /// </summary>
    Schnittmuendlich = 9,
    /// <summary>
    /// Jahresfortgang mit 2 Nachkommastellen.
    /// </summary>
    JahresfortgangMitNKS = 10,
    /// <summary>
    /// Jahresfortgang (ganzzahlig).
    /// </summary>
    Jahresfortgang = 11,
    /// <summary>
    /// Schriftliche Abschlussprüfung.
    /// </summary>
    APSchriftlich = 12,
    /// <summary>
    /// Mündliche Abschlussprüfung.
    /// </summary>
    APMuendlich = 13,
    /// <summary>
    /// Gesamtnote Abschlussprüfung.
    /// </summary>
    APGesamt = 14,
    /// <summary>
    /// Endnote (Jahresfortgang und Abschlussprüfung) mit 2 Nachkommastellen.
    /// </summary>
    EndnoteMitNKS = 15,
    /// <summary>
    /// Note im Abschlusszeugnis (ganzzahlig).
    /// </summary>
    Abschlusszeugnis = 16
  }

  /// <summary>
  /// Enumeration fuer Halbjahre. Bitte Nummern nicht ändern (die werden so in die Datenbank als int gecasted).
  /// </summary>
  public enum Halbjahr
  {
    /// <summary>
    /// Keine Zuordnung zu einem Halbjahr möglich.
    /// </summary>
    Ohne = 0,
    /// <summary>
    /// Erstes Halbjahr.
    /// </summary>
    Erstes = 1,
    /// <summary>
    /// Zweites Halbjahr.
    /// </summary>
    Zweites = 2
  }

  /// <summary>
  /// Eine Note.
  /// </summary>
  public class Note
  {
    /// <summary>
    /// Der Typ der Note, z. B. Schulaufgabe oder Ex.
    /// </summary>
    public Notentyp Typ
    {
      get;
      set;
    }

    /// <summary>
    /// Der Punktwert der Note (0-15).
    /// </summary>
    public decimal Punktwert
    {
      get;
      set;
    }

    /// <summary>
    /// Das Datum der Note.
    /// </summary>
    public DateTime Datum
    {
      get;
      set;
    }

    /// <summary>
    /// In welcher Zelle diese Note steht.
    /// </summary>
    public string Zelle
    {
      get;
      set;
    }

    /// <summary>
    /// Das Halbjahr, welchem die Note zuzuordnen ist.
    /// </summary>
    public Halbjahr Halbjahr
    {
      get;
      set;
    }

		/// <summary>
		/// In welchem Fach diese Note erzielt wurde
		/// </summary>
    public diNo.diNoDataSet.FachRow Fach
		{
			get;
			set;
		}
  }
}
