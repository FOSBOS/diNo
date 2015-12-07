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
    void Check(Schueler schueler, Zeitpunkt reason, NotenCheckResults res);
  }


  /// <summary>
  /// Abstrakte Basis für Notenchecker. Regelt Fehlermeldung und Anzeigename.
  /// </summary>
  public abstract class NotenCheck : INotenCheck 
  {
        protected SchuelerNoten noten;
    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="anzeigename">Der Anzeigename.</param>
    public NotenCheck(string anzeigename)
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
    public virtual void Check(Schueler schueler, Zeitpunkt reason,NotenCheckResults res)
    {
        noten = schueler.getNoten;
    }

    protected string toText(int z)
    {
        if (z==0) return " sind keine";
        if (z==1) return " ist nur eine";
        return " sind nur " + z;
    }
  }

  /// <summary>
  /// Klasse zur Prüfung, ob alle Schüler einer Klasse ein Fachreferat haben.
  /// </summary>
  public class FachreferatChecker : NotenCheck
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
    public override void Check(Schueler schueler, Zeitpunkt reason, NotenCheckResults res)
    {
            base.Check(schueler, reason, res);
            int sum=0;
            foreach (var fach in noten.alleFaecher)
                sum += fach.getNotenanzahl(Notentyp.Fachreferat);
            
            if (sum == 0)
                res.Add(schueler,null,"Der Schüler hat kein Fachreferat.");           
            else if (sum>1)
                res.Add(schueler, null, "Der Schüler hat " + sum + " Fachreferate.");
    }
  }

  /// <summary>
  /// Prüft, ob die fachpraktische Ausbildung mit Erfolg durchlaufen wurde.
  /// </summary>
  public class FpABestandenChecker: NotenCheck
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
    public override void Check(Schueler schueler, Zeitpunkt reason, NotenCheckResults res)
    {
      FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
      var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
      if (fpANoten.Count == 0)
      {
            res.Add(schueler, null, "Es liegt keine FpA-Note vor.");
      }
      else
      {
            var note = fpANoten[0].Note;
            if (note == 3)
            {
                res.Add(schueler, null, "Die fachpraktische Ausbildung wurde ohne Erfolg durchlaufen.");
            }
        }
    }

  }

  /// <summary>
  /// Prüft, ob eine Seminarfachnote vorhanden ist und ein Thema eingetragen wurde.
  /// </summary>
  public class SeminarfachChecker: NotenCheck
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
    public override void Check(Schueler schueler, Zeitpunkt reason, NotenCheckResults res)
    {
      SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
      var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
      if (seminarfachnoten.Count == 0)
      {
            res.Add(schueler, null,"Es liegt keine Seminarfachnote vor.");
      }
      else
      {
        var note = seminarfachnoten[0].Gesamtnote;
        var thema = seminarfachnoten[0].ThemaLang;

        if (note < 4)
        {
            res.Add(schueler, null, "Im Seminarfach wurden " +note+" Punkte erzielt.");
        }

        if (string.IsNullOrEmpty(thema))
        {
            res.Add(schueler, null, "Es liegt kein Seminarfachthema vor.");
        }
    }
  }
}

  /// <summary>
  /// Prüft die Anzahl der Noten
  /// </summary>
  public class NotenanzahlChecker : NotenCheck
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
    public override void Check(Schueler schueler, Zeitpunkt reason, NotenCheckResults res)
    {
        base.Check(schueler, reason, res);
        int anz=0;
        foreach (var fachNoten in noten.alleFaecher)
        {            
            // TODO: Anzahl SA direkt aus DB lesen!
            Schulaufgabenwertung wertung = fachNoten.getFach.GetSchulaufgabenwertung(schueler.getKlasse);
            Kurs kurs = new Kurs(fachNoten.kursId);

            //es müssen 2 oder 3 Schulaufgaben zum Ende des Jahres vorliegen - zum Halbjahr min. eine                                
            int noetigeAnzahlSchulaufgaben = GetAnzahlSchulaufgaben(wertung);
            if (noetigeAnzahlSchulaufgaben > 0)
            {
                if (reason == Zeitpunkt.HalbjahrUndProbezeitFOS || reason == Zeitpunkt.ProbezeitBOS)
                    noetigeAnzahlSchulaufgaben = 1;
                anz = fachNoten.getNotenanzahl(Notentyp.Schulaufgabe);
                if (anz < noetigeAnzahlSchulaufgaben)
                {
                    res.Add(schueler, kurs, 
                        "Es " + toText(anz) + " SA vorhanden.");
                    continue; // eine Meldung pro Fach und Schüler reicht
                }
            }

            // egal, bei welcher Entscheidung: Es müssen im ersten Halbjahr min. 2 mündliche Noten vorliegen
            // am Jahresende bzw. zur PA-Sitzung müssen es entweder 2 Kurzarbeiten/Exen und 2 echte mündliche

            // die Prüfung unterscheidet wie der bisherige Notenbogen nicht, ob die Note aus einer Ex oder echt mündlich ist - das verantwortet der Lehrer
            int kurzarbeitenCount = fachNoten.getNotenanzahl(Notentyp.Kurzarbeit);
            int muendlicheCount = fachNoten.getNotenanzahl(Notentyp.Ex) + fachNoten.getNotenanzahl(Notentyp.EchteMuendliche);

            if (reason == Zeitpunkt.ProbezeitBOS || reason == Zeitpunkt.HalbjahrUndProbezeitFOS)
            {
                if ((kurzarbeitenCount == 0 && muendlicheCount < 2) || muendlicheCount == 0)
                {
                    res.Add(schueler, kurs,
                        "Es " + toText(muendlicheCount) + " mündlichen Noten vorhanden.");
                }
            }
            else if (reason == Zeitpunkt.ErstePA || reason == Zeitpunkt.Jahresende)
            {
                if (kurzarbeitenCount == 1)
                {
                    res.Add(schueler, kurs,
                        "Es " + toText(kurzarbeitenCount) + " Kurzarbeit vorhanden.");
                    continue;
                }
                if ((kurzarbeitenCount == 0 && muendlicheCount < 4) || muendlicheCount < 2)
                {
                    res.Add(schueler, kurs,
                        "Es " + toText(muendlicheCount) + " mündliche Noten vorhanden.");
                }
            }

            // Zweite PA: nur Vorliegen der Prüfungsnoten prüfen
            else if (reason == Zeitpunkt.ZweitePA && fachNoten.getFach.IstSAPFach())
            {
                if (fachNoten.getNotenanzahl(Notentyp.APSchriftlich) == 0)
                {
                    res.Add(schueler, kurs,"Es liegt keine Note in der schriftlichen Abschlussprüfung vor.");
                }
            }
        }
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

  public class UnterpunktungChecker : NotenCheck
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
    public override void Check(Schueler schueler, Zeitpunkt reason, NotenCheckResults res)
    {
        base.Check(schueler, reason, res);
        int anz5=0,anz6=0,anz4P=0,anz2=0,anz1=0;
        string m="";


        foreach (var fachNoten in noten.alleFaecher)
        {            
            byte? relevanteNote = fachNoten.getRelevanteNote(reason);                    
            if (relevanteNote == null)
            {
                    ; // Das stellt der Unterpunktungschecker fest.
                // res.Add(schueler,new Kurs(fachNoten.kursId) ,"Es konnte keine Note gebildet werden.");
            }
            else
            {                         
                if (relevanteNote == 0) anz6++;                    
                else if (relevanteNote < 4) anz5++;
                else if (relevanteNote == 4) anz4P++;
                else if (relevanteNote >=13) anz1++;
                else if (relevanteNote >= 10) anz2++;

                if (relevanteNote <4 || relevanteNote == 4 && reason == Zeitpunkt.HalbjahrUndProbezeitFOS)
                    m = m + fachNoten.getFach.Kuerzel + "(" + relevanteNote +") ";
            }
        }

        if (reason == Zeitpunkt.ErstePA)
        {
            if (anz6 > 1 || (anz6 + anz5) > 3) res.Add(schueler, null, "Zum Abitur nicht zugelassen: " + m);                
        }
        else if (reason == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
            if (anz6>0 || anz5 > 1) res.Add(schueler, null, "Stark gefährdet: " + m); 
            else if (anz5 > 0) res.Add(schueler, null, "Gefährdet: " + m); 
            else if (anz4P > 1) res.Add(schueler, null, "Bei weiterem Absinken: " + m); 
        }
        else
        {
            // TODO: Notenausgleich sauber implementieren
            if (anz6 > 0 || anz5 > 1)
            {
                if (anz2 < 2 || anz1 == 0) res.Add(schueler, null, "Nicht bestanden, kein Notenausgleich möglich: " + m); 
                else res.Add(schueler, null, "Nicht bestanden, Notenausgleich prüfen: " + m); 
            }                    
        }
    }
  }
}
