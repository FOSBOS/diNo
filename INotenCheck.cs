using diNo.diNoDataSetTableAdapters;
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
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart, Zeitpunkt reason);

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    string[] Check(diNo.diNoDataSet.SchuelerRow schueler, Zeitpunkt reason);
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
    public AbstractNotenCheck(string anzeigename)
    {
      this.Anzeigename = anzeigename;
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
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public abstract bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart, Zeitpunkt reason);

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    public abstract string[] Check(diNo.diNoDataSet.SchuelerRow schueler, Zeitpunkt reason);
  }

  /// <summary>
  /// Klasse zur Prüfung, ob alle Schüler einer Klasse ein Fachreferat haben.
  /// </summary>
  public class FachreferatChecker : AbstractNotenCheck
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    public FachreferatChecker(): base("Fachreferat vorhanden")
    {
    }

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart, Zeitpunkt reason)
    {
      return jahrgangsstufe == Jahrgangsstufe.Zwoelf && reason == Zeitpunkt.ErstePA;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    public override string[] Check(diNo.diNoDataSet.SchuelerRow schueler, Zeitpunkt reason)
    {
      IList<string> result = new List<string>();
      var noten = new NoteTableAdapter().GetDataBySchuelerId(schueler.Id).Where(x => x.Notenart == (int)Notentyp.Fachreferat);
      if (noten.Count() != 1)
      {
        result.Add("Der Schüler hat kein Fachreferat");
      }

      return result.ToArray();
    }
  }

  /// <summary>
  /// Prüft, ob die fachpraktische Ausbildung mit Erfolg durchlaufen wurde.
  /// </summary>
  public class FpABestandenChecker: AbstractNotenCheck
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    public FpABestandenChecker(): base("FpA bestanden")
    {
    }

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart, Zeitpunkt reason)
    {
      return jahrgangsstufe == Jahrgangsstufe.Elf && schulart == Schulart.FOS;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    public override string[] Check(diNoDataSet.SchuelerRow schueler, Zeitpunkt reason)
    {
      FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
      var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
      if (fpANoten.Count == 0)
      {
        return new string[] { "Es liegt keine FpA-Note vor." };
      }
      else
      {
        var note = fpANoten[0].Note;
        if (note == 3)
        {
          return new string[] { "Die fachpraktische Ausbildung wurde ohne Erfolg durchlaufen." };
        }
        else
        {
          // alles OK
          return new string[0];
        }
      }
    }
  }

  /// <summary>
  /// Prüft, ob eine Seminarfachnote vorhanden ist und ein Thema eingetragen wurde.
  /// </summary>
  public class SeminarfachChecker: AbstractNotenCheck
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    public SeminarfachChecker(): base("Seminarfachnote und -thema liegen vor und sind ausreichend")
    { }

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart, Zeitpunkt reason)
    {
      return jahrgangsstufe == Jahrgangsstufe.Dreizehn;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    public override string[] Check(diNoDataSet.SchuelerRow schueler, Zeitpunkt reason)
    {
      SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
      var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
      if (seminarfachnoten.Count == 0)
      {
        return new string[] { "Es liegt keine Seminarfachnote vor." };
      }
      else
      {
        var note = seminarfachnoten[0].Gesamtnote;
        var thema = seminarfachnoten[0].ThemaLang;

        IList<string> fehler = new List<string>();
        if (note < 4)
        {
          fehler.Add("Im Seminarfach wurden "+note+" Punkte erzielt.");
        }

        if (string.IsNullOrEmpty(thema))
        {
          fehler.Add("Es liegt kein Seminarfachthema vor.");
        }

        return fehler.ToArray();
      }
    }
  }

  /// <summary>
  /// Prüft die Anzahl der Noten
  /// </summary>
  public class NotenanzahlChecker : AbstractNotenCheck
  {
    /// <summary>
    /// Konstruktor
    /// </summary>
    public NotenanzahlChecker(): base("Notenanzahl ausreichend")
    {
    }

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart, Zeitpunkt reason)
    {
      // Diese Prüfung kann immer durchgeführt werden
      return true;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    public override string[] Check(diNo.diNoDataSet.SchuelerRow schueler, Zeitpunkt reason)
    {
      IList<string> result = new List<string>();
/*      foreach (var kursZuordnung in new SchuelerKursTableAdapter().GetDataBySchuelerId(schueler.Id))
      {
        var kurs = new KursTableAdapter().GetDataById(kursZuordnung.KursId)[0];
        var fach = new FachTableAdapter().GetDataById(kurs.FachId)[0];
        var klasse = new KlasseTableAdapter().GetDataById(schueler.KlasseId)[0];
        var noten = new NoteTableAdapter().GetDataBySchuelerAndKurs(schueler.Id, kurs.Id);

        // TODO: Nur für Test am Jahresende, da in manchen Fächern keine Datei vorlag
        if (noten.Count == 0)
        {
          continue;
        }

        Schulaufgabenwertung wertung = kurs.GetSchulaufgabenwertung(fach.Kuerzel, klasse.Bezeichnung);

        int noetigeAnzahlSchulaufgaben = GetAnzahlSchulaufgaben(wertung);
        if (noetigeAnzahlSchulaufgaben > 0)
        {
          //es müssen 2 oder 3 Schulaufgaben zum Ende des Jahres vorliegen - zum Halbjahr min. eine
          var schulaufgaben = noten.Where(x => x.Notenart == (int)Notentyp.Schulaufgabe);
          if (reason == Zeitpunkt.HalbjahrUndProbezeitFOS && schulaufgaben.Count() < 1)
          {
            result.Add("Die Anzahl der Schulaufgaben im Fach " + fach.Bezeichnung + " ist zu gering.");
          }
          if (reason == Zeitpunkt.ErstePA && schulaufgaben.Count() < noetigeAnzahlSchulaufgaben)
          {
            result.Add("Die Anzahl der Schulaufgaben im Fach " + fach.Bezeichnung + " ist zu gering.");
          }
        }

        // egal, bei welcher Entscheidung: Es müssen im ersten Halbjahr min. 2 mündliche Noten vorliegen
        // am Jahresende bzw. zur PA-Sitzung müssen es entweder 2 Kurzarbeiten und 2 echte mündliche oder 3 Exen und 2 echte mündliche sein
        var kurzarbeiten = noten.Where(x => x.Notenart == (int)Notentyp.Kurzarbeit);
        var exen = noten.Where(x => x.Notenart == (int)Notentyp.Ex);
        var echteMuendliche = noten.Where(x => x.Notenart == (int)Notentyp.EchteMuendliche);

        // die Prüfung unterscheidet wie der bisherige Notenbogen nicht, ob die Note aus einer Ex oder echt mündlich ist - das verantwortet der Lehrer
        int kurzarbeitenCount = kurzarbeiten.Count();
        int muendlicheCount = exen.Count() + echteMuendliche.Count();

        if (reason == Zeitpunkt.ProbezeitBOS || reason == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
          if (kurzarbeitenCount == 0 && muendlicheCount < 2)
          {
            result.Add("Die Anzahl der mündlichen Noten im Fach "+fach.Bezeichnung + " ist zu gering");
          }
          if (muendlicheCount == 0)
          {
            result.Add("Die Anzahl der mündlichen Noten im Fach " + fach.Bezeichnung + " ist zu gering");
          }
        }
        else if (reason == Zeitpunkt.ErstePA || reason == Zeitpunkt.Jahresende)
        {
          if (kurzarbeitenCount < 2 && muendlicheCount < 5)
          {
            result.Add("Die Anzahl der mündlichen Noten im Fach " + fach.Bezeichnung + " ist zu gering");
          }
          if (kurzarbeitenCount >= 2 && muendlicheCount < 2)
          {
            result.Add("Die Anzahl der mündlichen Noten im Fach " + fach.Bezeichnung + " ist zu gering");
          }
        }
        // Zweite PA: nur Vorliegen der Prüfungsnoten prüfen
        else if (reason == Zeitpunkt.ZweitePA)
        {
          var schriftlichePruefung = noten.Where(x => x.Notenart == (int)Notentyp.APSchriftlich);
          if (schriftlichePruefung.Count() == 0)
          {
            result.Add("Es liegt keine Note in der schriftlichen Abschlussprüfung vor");
          }
        }
      }
      */
      return result.ToArray();
    }

    private static int GetAnzahlSchulaufgaben(Schulaufgabenwertung wertung)
    {
      int noetigeAnzahlSchulaufgaben = 0;
      if (wertung == Schulaufgabenwertung.EinsZuEins)
      {
        noetigeAnzahlSchulaufgaben = 2;
      }
      if (wertung == Schulaufgabenwertung.ZweiZuEins)
      {
        noetigeAnzahlSchulaufgaben = 3;
      }

      return noetigeAnzahlSchulaufgaben;
    }
  }

  public class UnterpunktungChecker : AbstractNotenCheck
  {
    /// <summary>
    /// Konstruktor
    /// </summary>
    public UnterpunktungChecker()
      : base("Prüfe Unterpunktung")
    {
    }

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart, Zeitpunkt reason)
    {
      return true;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="reason">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    public override string[] Check(diNoDataSet.SchuelerRow schueler, Zeitpunkt reason)
    {
      IList<string> result = new List<string>();
            /*
      foreach (var kursZuordnung in new SchuelerKursTableAdapter().GetDataBySchuelerId(schueler.Id))
      {
        var kurs = new KursTableAdapter().GetDataById(kursZuordnung.KursId)[0];
        var fach = new FachTableAdapter().GetDataById(kurs.FachId)[0];
        var klasse = new KlasseTableAdapter().GetDataById(schueler.KlasseId)[0];
        var noten = new NoteTableAdapter().GetDataBySchuelerAndKurs(schueler.Id, kurs.Id);

        // TODO: Nur für Test am Jahresende, da in manchen Fächern keine Datei vorlag
        if (noten.Count == 0)
        {
          continue;
        }

        diNo.diNoDataSet.NoteRow relevanteNote = null;
        if (reason == Zeitpunkt.ProbezeitBOS || reason == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
          var halbjahresnote = noten.Where(x => x.Notenart == (int)Notentyp.Jahresfortgang && x.Halbjahr == (int)Halbjahr.Erstes);
          relevanteNote = halbjahresnote.Count() > 0 ? halbjahresnote.First() : null;
        }
        else if (reason == Zeitpunkt.ErstePA || reason == Zeitpunkt.Jahresende)
        {
          var jahresfortgang = noten.Where(x => x.Notenart == (int)Notentyp.Jahresfortgang && x.Halbjahr == (int)Halbjahr.Zweites);
          relevanteNote = jahresfortgang.Count() > 0 ? jahresfortgang.First() : null;
        }
        else if (reason == Zeitpunkt.ZweitePA || reason == Zeitpunkt.DrittePA)
        {
          var zeugnisnote = noten.Where(x => x.Notenart == (int)Notentyp.Abschlusszeugnis);
          relevanteNote = zeugnisnote.Count() > 0 ? zeugnisnote.First() : null;
        }

        if (relevanteNote == null)
        {
          result.Add("Es konnte im Fach " + fach.Bezeichnung + " keine Note gebildet werden");
        }
        else
        {
          int notenwert = (int)(relevanteNote.Punktwert);
          if (notenwert < 4)
          {
            result.Add("Unterpunktung im Fach " + fach.Bezeichnung + " mit " + notenwert + " Punkten");
          }
        }
      }
      */
      return result.ToArray();
    }
  }
}
