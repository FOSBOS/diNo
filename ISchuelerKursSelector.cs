using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;

namespace diNo
{
  /// <summary>
  /// Interface für Klassen, die die Auswahl von Schülern für bestimmte Kurse übernehmen können.
  /// </summary>
  public interface ISchuelerKursSelector
  {
    /// <summary>
    /// Prüft, ob ein Schueler in einem Kurs ist. Voreinstellung: Der Schueler ist drin, solange kein Selector ihn rausnimmt.
    /// </summary>
    /// <param name="schueler">Der Schueler.</param>
    /// <param name="kurs">Der Kurs.</param>
    /// <returns>False wenn es einen Grund gibt, weswegen der Schueler in diesen Kurs nicht gehen darf. Sonst true.</returns>
    bool IsInKurs(diNo.diNoDataSet.SchuelerRow schueler, diNo.diNoDataSet.KursRow kurs);

    /// <summary>
    /// Muss true ausgeben, wenn sich der Selector für ein Fach interessiert.
    /// </summary>
    /// <param name="fach">Das Fach.</param>
    /// <returns>true wenn interessiert.</returns>
    bool IsInterestedInFach(diNo.diNoDataSet.FachRow fach);
  }

  /// <summary>
  /// Diese Klasse nimmt mehrere solche Selektoren auf und leitet intern die Anfragen weiter.
  /// </summary>
  public class SchuelerKursSelectorHolder : ISchuelerKursSelector
  {
    /// <summary>
    /// Die Selektoren.
    /// </summary>
    private IList<ISchuelerKursSelector> allSelectors;

    /// <summary>
    /// Der Konstruktor.
    /// </summary>
    public SchuelerKursSelectorHolder()
    {
      this.allSelectors = new List<ISchuelerKursSelector>();
    }

    /// <summary>
    /// Methode zum Anmelden eines weiteren Selektors.
    /// </summary>
    /// <param name="selector">Der Selektor.</param>
    public void AddSelector(ISchuelerKursSelector selector)
    {
      this.allSelectors.Add(selector);
    }

    /// <summary>
    /// Prüft, ob ein Schueler in einem Kurs ist. Voreinstellung: Der Schueler ist drin, solange kein Selector ihn rausnimmt.
    /// </summary>
    /// <param name="schueler">Der Schueler.</param>
    /// <param name="kurs">Der Kurs.</param>
    /// <returns>False wenn es einen Grund gibt, weswegen der Schueler in diesen Kurs nicht gehen darf. Sonst true.</returns>
    public bool IsInKurs(diNoDataSet.SchuelerRow schueler, diNoDataSet.KursRow kurs)
    {
      var fach = new FachTableAdapter().GetDataById(kurs.FachId)[0];
 	    foreach (ISchuelerKursSelector selector in this.allSelectors)
      {
        if (selector.IsInterestedInFach(fach) && !selector.IsInKurs(schueler, kurs))
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Muss true ausgeben, wenn sich der Selector für ein Fach interessiert.
    /// </summary>
    /// <param name="fach">Das Fach.</param>
    /// <returns>true wenn interessiert.</returns>
    public bool IsInterestedInFach(diNo.diNoDataSet.FachRow fach)
    {
      foreach (ISchuelerKursSelector selector in this.allSelectors)
      {
        if (selector.IsInterestedInFach(fach))
        {
          return true;
        }
      }

      return false;
    }
  }

  /// <summary>
  /// Prüft, ob der Schüler den übergebenen Kurs als Fremdsprache gewählt hat.
  /// </summary>
  public class FremdspracheSelector :ISchuelerKursSelector
  {
    /// <summary>
    /// Prüft, ob ein Schueler in einem Kurs ist. Voreinstellung: Der Schueler ist drin, solange kein Selector ihn rausnimmt.
    /// Der Fremdsprachenselektor prüft, ob Französisch als zweite Fremdsprache eingetragen ist.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="kurs">Der Kurs (momentan nur französisch).</param>
    /// <returns>true wenn der Schüler französisch gewählt hat.</returns>
    public bool IsInKurs(diNoDataSet.SchuelerRow schueler, diNoDataSet.KursRow kurs)
    {
      var fach = new FachTableAdapter().GetDataById(kurs.FachId)[0];
      if (fach.Kuerzel.Equals("F", StringComparison.OrdinalIgnoreCase))
      {
        // F3 steht für französisch fortgeführt. Diese Leute gehören nicht in den "normalen" Französischkurs
        return (fach.Kuerzel.Equals(schueler.Fremdsprache2, StringComparison.OrdinalIgnoreCase)) && !(schueler.Wahlpflichtfach.Equals("F3", StringComparison.OrdinalIgnoreCase));
      }

      return true;
    }

    /// <summary>
    /// Muss true ausgeben, wenn sich der Selector für ein Fach interessiert.
    /// Momentan interessiert sich der Fremdsprachenselektor nur für Französisch.
    /// </summary>
    /// <param name="fach">Das Fach.</param>
    /// <returns>true wenn interessiert.</returns>
    public bool IsInterestedInFach(diNo.diNoDataSet.FachRow fach)
    {
      return fach.Kuerzel.Equals("F", StringComparison.OrdinalIgnoreCase);
    }
  }

  /// <summary>
  /// Prüft, ob der Schüler in RK, EV oder Eth geht.
  /// </summary>
  public class ReliOderEthikSelector : ISchuelerKursSelector
  {
    /// <summary>
    /// Prüft, ob ein Schueler in einem Kurs ist.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="kurs">Der Kurs.</param>
    /// <returns>true, wenn der Schüler in diesen Kurs gehen soll.</returns>
    public bool IsInKurs(diNoDataSet.SchuelerRow schueler, diNoDataSet.KursRow kurs)
    {
      var fach = new FachTableAdapter().GetDataById(kurs.FachId)[0];

      // wenn eine Wahl vorliegt, zuerst danach richten
      bool kursGewaehlt = !string.IsNullOrEmpty(schueler.ReligionOderEthik);
      if (kursGewaehlt)
      {
        bool katholisch = fach.Kuerzel.Equals("K", StringComparison.OrdinalIgnoreCase) && schueler.ReligionOderEthik.Equals("RK", StringComparison.OrdinalIgnoreCase);
        bool evangelisch = fach.Kuerzel.Equals("Ev", StringComparison.OrdinalIgnoreCase) && schueler.ReligionOderEthik.Equals("EV", StringComparison.OrdinalIgnoreCase);
        bool ethik = fach.Kuerzel.Equals("Eth", StringComparison.OrdinalIgnoreCase) && schueler.ReligionOderEthik.Equals("Eth", StringComparison.OrdinalIgnoreCase);
        
        return katholisch || evangelisch || ethik;
      }
      else 
      {
        // wenn keine Wahl vorliegt, dann muss das Bekenntnis passen
        // wenn das auch nicht passt, dann stecken wir den Schueler in Ethik
        bool bekenntnisStimmt = IsEqualBekenntnis(schueler.Bekenntnis, fach.Kuerzel);
        return bekenntnisStimmt || fach.Kuerzel.Equals("Eth", StringComparison.OrdinalIgnoreCase);
      }
    }

    public bool IsEqualBekenntnis(string bekenntnis, string anderes)
    {
      if (bekenntnis.Equals(anderes, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      if ((bekenntnis.Equals("K", StringComparison.OrdinalIgnoreCase) && anderes.Equals("RK", StringComparison.OrdinalIgnoreCase)) || (bekenntnis.Equals("RK", StringComparison.OrdinalIgnoreCase) && anderes.Equals("K", StringComparison.OrdinalIgnoreCase)))
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Interessiert sich momentan für die Fächer K, Ev und Eth.
    /// </summary>
    /// <param name="fach">Das Fach</param>
    /// <returns>true falls eines der Reli-Fächer übergeben wurde. Sonst false.</returns>
    public bool IsInterestedInFach(diNo.diNoDataSet.FachRow fach)
    {
      return fach.Kuerzel.Equals("K", StringComparison.OrdinalIgnoreCase)
        || fach.Kuerzel.Equals("Ev", StringComparison.OrdinalIgnoreCase)
        || fach.Kuerzel.Equals("Eth", StringComparison.OrdinalIgnoreCase);
    }
  }

  public class WahlpflichtfachSelector : ISchuelerKursSelector
  {
    /// <summary>
    /// Prüft, ob ein Schueler in einem Kurs ist.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="kurs">Der Kurs.</param>
    /// <returns>true, wenn der Schüler in diesen Kurs gehen soll.</returns>
    public bool IsInKurs(diNoDataSet.SchuelerRow schueler, diNoDataSet.KursRow kurs)
    {
      var fach = new FachTableAdapter().GetDataById(kurs.FachId)[0];
      Schueler derSchueler = new Schueler(schueler);

      if (derSchueler.Zweig == Zweig.Wirtschaft)
      {
        if (derSchueler.getKlasse.Schulart == Schulart.FOS)
        {
          // Wirtschafts-FOSler müssen zwischen WIn und Französisch (fortgeführt) wählen
          // wenn der Schüler dieses Fach extra gewählt hat soll er natürlich reingehen
          if (fach.Kuerzel.Equals("F-Wi", StringComparison.OrdinalIgnoreCase) &&
            schueler.Wahlpflichtfach.Equals("F3", StringComparison.OrdinalIgnoreCase))
          {
            return true;
          }

          if (fach.Kuerzel.Equals("WIn", StringComparison.OrdinalIgnoreCase) &&
            schueler.Wahlpflichtfach.Equals("WIn", StringComparison.OrdinalIgnoreCase))
          {
            return true;
          }

          return false;
        }
        else
        {
          // Wirtschafts-BOSler gehen immer in WIn, aber nie in Französisch (fortgeführt) oder Kunst
          return fach.Kuerzel.Equals("WIn", StringComparison.OrdinalIgnoreCase);
        }
      }

      // Für Soziale gilt: Wer Kunst als Wahlpflichtfach gewählt hat geht in Kunst
      // wer Französisch als Wahlpflichtfach gewählt hat, geht in Französisch (das macht aber der
      // Fremdsprachenselector, weil dann Französisch als Sprache2 eingetragen wird)
      if (derSchueler.Zweig == Zweig.Sozial)
      {
        if (fach.Kuerzel.Equals("Ku", StringComparison.OrdinalIgnoreCase) &&
            schueler.Wahlpflichtfach.Equals("Ku", StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Interessiert sich momentan für die Fächer F-Wi, Ku und WIn.
    /// </summary>
    /// <param name="fach">Das Fach</param>
    /// <returns>true falls eines der Reli-Fächer übergeben wurde. Sonst false.</returns>
    public bool IsInterestedInFach(diNoDataSet.FachRow fach)
    {
      return fach.Kuerzel.Equals("F-Wi", StringComparison.OrdinalIgnoreCase)
        || fach.Kuerzel.Equals("WIn", StringComparison.OrdinalIgnoreCase)
        || fach.Kuerzel.Equals("Ku", StringComparison.OrdinalIgnoreCase);
    }
  }
}
