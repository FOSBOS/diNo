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
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>    
    /// <returns>true wenn check nötig.</returns>
    bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart);

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    void Check(Schueler schueler);
  }


  /// <summary>
  /// Abstrakte Basis für Notenchecker. Regelt Fehlermeldungen und Controller
  /// </summary>
  public abstract class NotenCheck : INotenCheck 
  {
    protected SchuelerNoten noten;
    protected NotenCheckController contr;

    /// <summary>
    /// Konstruktor.
    /// </summary>    
    public NotenCheck(NotenCheckController acontr)
    {
      contr = acontr;
    }

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>    
    /// <returns>true wenn check nötig.</returns>
    public abstract bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart);

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>    
    public virtual void Check(Schueler schueler)
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
    public FachreferatChecker(NotenCheckController contr) :base (contr)
        { }
        

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return jahrgangsstufe == Jahrgangsstufe.Zwoelf && contr.zeitpunkt == Zeitpunkt.ErstePA;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>    
    public override void Check(Schueler schueler)
    {
            base.Check(schueler);
            int sum=0;
            foreach (var fach in noten.alleFaecher)
                sum += fach.getNotenanzahl(Notentyp.Fachreferat);
            
            if (sum == 0)
                contr.res.Add(schueler,null,"Der Schüler hat kein Fachreferat.");           
            else if (sum>1)
                contr.res.Add(schueler, null, "Der Schüler hat " + sum + " Fachreferate.");
    }
  }

  /// <summary>
  /// Prüft, ob die fachpraktische Ausbildung mit Erfolg durchlaufen wurde.
  /// </summary>
  public class FpABestandenChecker: NotenCheck
  {
    public FpABestandenChecker(NotenCheckController contr) :base (contr)
        { }

    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return jahrgangsstufe == Jahrgangsstufe.Elf && schulart == Schulart.FOS;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    public override void Check(Schueler schueler)
    {        
        var fpANoten = schueler.FPANoten;
        if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
            if (fpANoten.IsErfolg1HjNull() || fpANoten.IsPunkte1HjNull()) contr.res.Add(schueler, null, "Es liegt keine FpA-Note vor.");
            else if (fpANoten.Erfolg1Hj == 4)
            {
                contr.res.Add(schueler, null, "Die fachpraktische Ausbildung wurde bisher ohne Erfolg durchlaufen.");
            }
        }            
        else if (contr.zeitpunkt == Zeitpunkt.Jahresende)
        {
            if (fpANoten.IsPunkte2HjNull() || fpANoten.IsErfolgNull() || fpANoten.IsPunkteNull()) contr.res.Add(schueler, null, "Es liegt keine FpA-Note vor.");
            else if (fpANoten.Erfolg == 4)
            {
                contr.res.Add(schueler, null, "Die fachpraktische Ausbildung wurde ohne Erfolg durchlaufen.");
            }
        }           
    }
  }

  /// <summary>
  /// Prüft, ob eine Seminarfachnote vorhanden ist und ein Thema eingetragen wurde.
  /// </summary>
  public class SeminarfachChecker: NotenCheck
  {
    public SeminarfachChecker(NotenCheckController contr) :base (contr)
        { }
    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return jahrgangsstufe == Jahrgangsstufe.Dreizehn;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    public override void Check(Schueler schueler)
    {
      SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
      var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
      if (seminarfachnoten.Count == 0)
      {
            contr.res.Add(schueler, null,"Es liegt keine Seminarfachnote vor.");
      }
      else
      {
        var note = seminarfachnoten[0].Gesamtnote;
        var thema = seminarfachnoten[0].ThemaLang;

        if (note < 4)
        {
            contr.res.Add(schueler, null, "Im Seminarfach wurden " +note+" Punkte erzielt.");
        }

        if (string.IsNullOrEmpty(thema))
        {
            contr.res.Add(schueler, null, "Es liegt kein Seminarfachthema vor.");
        }
    }
  }
}

  /// <summary>
  /// Prüft die Anzahl der Noten
  /// </summary>
  public class NotenanzahlChecker : NotenCheck
  {
    public NotenanzahlChecker(NotenCheckController contr) :base (contr)
        { }
    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="contr.zeitpunkt">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      // Diese Prüfung kann immer durchgeführt werden
      return true;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>    
    public override void Check(Schueler schueler)

    {
      base.Check(schueler);
      List<string> faecherOhneNoten = new List<string>();
      foreach (var fachNoten in noten.alleFaecher)
      {
        Kurs kurs = new Kurs(fachNoten.kursId);
        if (contr.nurEigeneNoten && (Zugriff.Instance.Lehrer.Id != kurs.getLehrer.Id))
          continue;

        // TODO: Anzahl SA direkt aus DB lesen!           
        Schulaufgabenwertung wertung = fachNoten.getFach.GetSchulaufgabenwertung(schueler.getKlasse);

        // die Prüfung unterscheidet wie der bisherige Notenbogen nicht, ob die Note aus einer Ex oder echt mündlich ist - das verantwortet der Lehrer
        int kurzarbeitenCount = fachNoten.getNotenanzahl(Notentyp.Kurzarbeit);
        int muendlicheCount = fachNoten.getNotenanzahl(Notentyp.Ex) + fachNoten.getNotenanzahl(Notentyp.EchteMuendliche);
        int schulaufgabenCount = fachNoten.getNotenanzahl(Notentyp.Schulaufgabe);

        if (kurzarbeitenCount == 0 && muendlicheCount == 0 && schulaufgabenCount == 0)
        {
          faecherOhneNoten.Add(kurs.getFach.Kuerzel);
          continue; // eine Meldung pro Fach und Schüler reicht
        }

        //es müssen 2 oder 3 Schulaufgaben zum Ende des Jahcontr.res vorliegen - zum Halbjahr min. eine                                
        int noetigeAnzahlSchulaufgaben = GetAnzahlSchulaufgaben(wertung);
        if (noetigeAnzahlSchulaufgaben > 0)
        {
          if (contr.zeitpunkt == Zeitpunkt.ProbezeitBOS)
          {
            if (noetigeAnzahlSchulaufgaben <= 2)
              noetigeAnzahlSchulaufgaben = 0; // zur Probezeit BOS muss noch keine SA vorliegen, wenn nur pro HJ eine geschrieben wird
            else
              noetigeAnzahlSchulaufgaben = 1; // wenn mehr als 2 SA geschrieben werden, muss auch zur Probezeit BOS schon eine vorliegen
          }
          if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
          {
            noetigeAnzahlSchulaufgaben = 1;
          }
          
          if (schulaufgabenCount < noetigeAnzahlSchulaufgaben)
          {
            contr.res.Add(schueler, kurs, "Es " + toText(schulaufgabenCount) + " SA vorhanden.");
            continue; // eine Meldung pro Fach und Schüler reicht
          }
        }

        // egal, bei welcher Entscheidung: Es müssen im ersten Halbjahr min. 2 mündliche Noten vorliegen
        // am Jahcontr.resende bzw. zur PA-Sitzung müssen es entweder 2 Kurzarbeiten/Exen und 2 echte mündliche


        if (contr.zeitpunkt == Zeitpunkt.ProbezeitBOS)
        {
          if (!AnzahlMuendlicheNotenOKProbezeitBOS(schulaufgabenCount, kurzarbeitenCount, muendlicheCount, fachNoten))
          {
            contr.res.Add(schueler, kurs, "Es " + toText(muendlicheCount) + " mündlichen Noten vorhanden.");
          }
        }
        else if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
          {
            if ((kurzarbeitenCount == 0 && muendlicheCount < 2) || muendlicheCount == 0)
            {
              contr.res.Add(schueler, kurs,
                  "Es " + toText(muendlicheCount) + " mündlichen Noten vorhanden.");
            }
          }
        }
        else if (contr.zeitpunkt == Zeitpunkt.ErstePA || contr.zeitpunkt == Zeitpunkt.Jahresende)
        {
          if (kurzarbeitenCount == 1)
          {
            contr.res.Add(schueler, kurs,
                "Es " + toText(kurzarbeitenCount) + " Kurzarbeit vorhanden.");
            continue;
          }
          if ((kurzarbeitenCount == 0 && muendlicheCount < 4) || muendlicheCount < 2)
          {
            contr.res.Add(schueler, kurs,
                "Es " + toText(muendlicheCount) + " mündliche Noten vorhanden.");
          }
        }

        // Zweite PA: nur Vorliegen der Prüfungsnoten prüfen
        else if (contr.zeitpunkt == Zeitpunkt.ZweitePA && fachNoten.getFach.IstSAPFach())
        {
          if (fachNoten.getNotenanzahl(Notentyp.APSchriftlich) == 0)
          {
            contr.res.Add(schueler, kurs, "Es liegt keine Note in der schriftlichen Abschlussprüfung vor.");
          }
        }
      }

      if (faecherOhneNoten.Count > 0)
      {
        contr.res.Add(schueler, null, "Es sind keine Noten vorhanden in:" + string.Join(", ", faecherOhneNoten));
      }
    }

    private bool AnzahlMuendlicheNotenOKProbezeitBOS(int schulaufgabenCount, int kurzarbeitenCount, int muendlicheCount, FachSchuelerNoten noten)
    {
      if (kurzarbeitenCount > 1)
      {
        return true; // mehr als 1 Kurzabeit oder min. 2 mündliche Noten sind auf jeden Fall OK
      }

      if (kurzarbeitenCount == 1)
      {
        if (muendlicheCount > 0 || schulaufgabenCount > 0)
        {
          return true; // wenn nur 1 Kurzarbeit vorliegt, braucht man im Normalfall noch eine andere Note. Dann ist es OK.
        }
        else
        {
          // wir akzeptieren eine Kurzarbeit als einzelne Note, wenn sie bei min. 6 Punkten liegt (somit kann der Schüler in diesem Fach nicht mehr unterpunkten)
          var kurzarbeitNote = noten.getNoten(Halbjahr.Erstes, Notentyp.Kurzarbeit)[0];
          return (kurzarbeitNote >= 4);
        }
      }

      //ansonsten ist keine Kurzarbeit vorhanden. 
      if (muendlicheCount >= 2)
      {
        return true; // wenn min. 2 mündliche vorliegen, ist das ok.
      }
      
      if (muendlicheCount == 1)
      {
        // wir akzeptieren eine Kurzarbeit als einzelne Note, wenn sie bei min. 6 Punkten liegt (somit kann der Schüler in diesem Fach nicht mehr unterpunkten)
        var exen = noten.getNoten(Halbjahr.Erstes, Notentyp.Ex);
        var echteMuendliche = noten.getNoten(Halbjahr.Erstes, Notentyp.EchteMuendliche);
        var einzigeNote = exen.Count > 0 ? exen[0] : echteMuendliche[0];
        return (einzigeNote >= 4);
      }

      //in allen anderen Fällen sind es zu wenig Noten
      return false;
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
    public UnterpunktungChecker(NotenCheckController contr) :base (contr)
        { }
    /// <summary>
    /// Ob die implementierte Prüfung überhaupt sinnvoll ist.
    /// </summary>
    /// <param name="jahrgangsstufe">Die Jahrgangsstufe.</param>
    /// <param name="schulart">Die Schulart (FOS oder BOS)</param>
    /// <param name="contr.zeitpunkt">Die Art der Prüfung.</param>
    /// <returns>true wenn check nötig.</returns>
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return true;
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="contr.zeitpunkt">Die Art der Prüfung.</param>
    /// <returns>Array mit Fehler- oder Problemmeldungen. Kann auch leer sein.</returns>
    public override void Check(Schueler schueler)
    {
        base.Check(schueler);
        int anz5=0,anz6=0,anz4P=0,anz2=0,anz1=0;
        string m="";


        foreach (var fachNoten in noten.alleFaecher)
        {            
            byte? relevanteNote = fachNoten.getRelevanteNote(contr.zeitpunkt);                    
            if (relevanteNote == null)
            {
                    ; // Das stellt der Unterpunktungschecker fest.
                // contr.res.Add(schueler,new Kurs(fachNoten.kursId) ,"Es konnte keine Note gebildet werden.");
            }
            else
            {                         
                if (relevanteNote == 0) anz6++;                    
                else if (relevanteNote < 4) anz5++;
                else if (relevanteNote == 4) anz4P++;
                else if (relevanteNote >=13) anz1++;
                else if (relevanteNote >= 10) anz2++;

                if (relevanteNote <4 || relevanteNote == 4 && contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
                    m = m + fachNoten.getFach.Kuerzel + "(" + relevanteNote +") ";
            }
        }

        if (contr.zeitpunkt == Zeitpunkt.ErstePA)
        {
            if (anz6 > 1 || (anz6 + anz5) > 3) contr.res.Add(schueler, null, "Zum Abitur nicht zugelassen: " + m);                
        }
        else if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
            if (anz6>0 || anz5 > 1) contr.res.Add(schueler, null, "Stark gefährdet: " + m); 
            else if (anz5 > 0) contr.res.Add(schueler, null, "Gefährdet: " + m); 
            else if (anz4P > 1) contr.res.Add(schueler, null, "Bei weiterem Absinken: " + m); 
        }
        else
        {
            // TODO: Notenausgleich sauber implementieren
            if (anz6 > 0 || anz5 > 1)
            {
                if (anz2 < 2 || anz1 == 0) contr.res.Add(schueler, null, "Nicht bestanden, kein Notenausgleich möglich: " + m); 
                else contr.res.Add(schueler, null, "Nicht bestanden, Notenausgleich prüfen: " + m); 
            }                    
        }
    }
  }
}
