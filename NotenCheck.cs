using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
   
  /// <summary>
  /// Abstrakte Basis für Notenchecker. Regelt Fehlermeldungen und Controller
  /// </summary>
  public abstract class NotenCheck 
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

    // erzeugt einen grammatikalisch korrekten Satz, je nach Anzahl der LNWs
    // Leerzeichen werden vorne und hinten angefügt.
    protected string toText(int z, string adjektiv="", string substantiv="", Halbjahr hj=Halbjahr.Ohne)
    {   
        string res = (hj==Halbjahr.Zweites)  ? "Im 2. Halbjahr" : "Es";
        
        if (adjektiv!="")
        {
          if (z==0) adjektiv+="n "; // mündlichen
          else adjektiv +=" ";      // mündliche
        }
        if (substantiv!="")
        {
          if (z==1) substantiv+=" "; // Note
          else substantiv +="n ";    // Noten
        }
        if (z==0) res += " sind keine "+adjektiv+substantiv;
        else if (z==1) res += " ist nur eine "+adjektiv+substantiv;
        else res += " sind nur " + z + " " +adjektiv+substantiv;  
        
        res += "vorhanden.";
        return res;
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
                contr.Add(null,"Es ist keine Note für das Fachreferat vorhanden.");
            else if (sum>1)
                contr.Add( null, "Es sind " + sum + " Noten für das Fachreferat vorhanden.");
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
        if (contr.modus == NotenCheckModus.EigeneNotenVollstaendigkeit && schueler.BetreuerId != Zugriff.Instance.lehrer.Id)
          return;

        var fpANoten = schueler.FPANoten;
        if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
            if (fpANoten.IsErfolg1HjNull() || fpANoten.IsPunkte1HjNull()) contr.Add(null, "Es liegt keine FpA-Note vor.");
            else if (fpANoten.Erfolg1Hj == 4)
            {
                contr.Add(null, "Die fachpraktische Ausbildung wurde bisher ohne Erfolg durchlaufen.");
            }
        }            
        else if (contr.zeitpunkt == Zeitpunkt.Jahresende)
        {
            if (fpANoten.IsPunkte2HjNull() || fpANoten.IsErfolgNull() || fpANoten.IsPunkteNull()) contr.Add(null, "Es liegt keine FpA-Note vor.");
            else if (fpANoten.Erfolg == 4)
            {
                contr.Add(null, "Die fachpraktische Ausbildung wurde ohne Erfolg durchlaufen.");
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
            contr.Add(null,"Es liegt keine Seminarfachnote vor.");
      }
      else
      {
        var note = seminarfachnoten[0].Gesamtnote;

        if (note < 4)
        {
            contr.Add(null, "Im Seminarfach wurden " +note+" Punkte erzielt.");
        }

        if (string.IsNullOrEmpty(seminarfachnoten[0].ThemaLang) && string.IsNullOrEmpty(seminarfachnoten[0].ThemaKurz))
        {
            contr.Add(null, "Es liegt kein Seminarfachthema vor.");
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
      return true; // bei der 3. PA wird nur noch auf Bestehen geprüft --> s. Konstruktor NotenCheckController
    }

    /// <summary>
    /// Führt den Check durch.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>    
    public override void Check(Schueler schueler)

    {
      base.Check(schueler);
      //List<string> faecherOhneNoten = new List<string>(); 
      foreach (var fachNoten in noten.alleFaecher)
      {
        Kurs kurs = new Kurs(fachNoten.kursId);
        if (contr.modus == NotenCheckModus.EigeneNotenVollstaendigkeit && (kurs.getLehrer == null || Zugriff.Instance.lehrer.Id != kurs.getLehrer.Id))
          continue;

        // Zweite PA: nur Vorliegen der Prüfungsnoten prüfen
        // -------------------------------------------------
        if (contr.zeitpunkt == Zeitpunkt.ZweitePA && fachNoten.getFach.IstSAPFach(schueler.Zweig))
        {
          if (fachNoten.getNotenanzahl(Notentyp.APSchriftlich) == 0)
          {
            contr.Add( kurs, "Es liegt keine Note in der schriftlichen Abschlussprüfung vor.");
          }
          continue;
        }

        // Grunddaten dieses Fachs
        // -----------------------
        int noetigeAnzahlSchulaufgaben = fachNoten.getFach.AnzahlSA(schueler.Zweig,schueler.getKlasse.Jahrgangsstufe);
        bool istSAFach = noetigeAnzahlSchulaufgaben>0;
        bool einstuendig = fachNoten.getFach.IstEinstuendig(schueler.getKlasse.Jahrgangsstufe,schueler.getKlasse.Schulart);
        int noetigeAnzahlEchteMdl = (einstuendig ? 1 : 2);
        bool meldungKA=false, meldungMdl=false, meldungSA=false;
        Kurs derKurs = new Kurs(fachNoten.kursId);
        if (derKurs.getLehrer == null)
        {
          // vermutlich ein Dummy-Kurs, der aus der elften Klasse übernommen wurde. Prüfe nur Jahresfortgang und Zeugnisnote
          byte? relevanteNote = fachNoten.getRelevanteNote(contr.zeitpunkt);
          if (relevanteNote == null)
          {
            contr.Add(kurs, "Es liegt keine Note aus der 11ten Klasse vor.");
          }

          continue; // in diesen Kursen nicht weiter nach Notenanzahl prüfen
        }


        // Halbjahresprüfung
        // -----------------
        Halbjahr hj = Halbjahr.Zweites;
        if (contr.zeitpunkt==Zeitpunkt.ProbezeitBOS || contr.zeitpunkt==Zeitpunkt.HalbjahrUndProbezeitFOS)
          hj = Halbjahr.Erstes;

        // die Prüfung unterscheidet wie der bisherige Notenbogen nicht, ob die Note aus einer Ex oder echt mündlich ist - das verantwortet der Lehrer
        int kurzarbeitenCount = fachNoten.getNotenanzahl(hj,Notentyp.Kurzarbeit);
        int muendlicheCount = fachNoten.getNotenanzahl(hj,Notentyp.Ex) + fachNoten.getNotenanzahl(hj,Notentyp.EchteMuendliche) + fachNoten.getNotenanzahl(Notentyp.Fachreferat);
        int schulaufgabenCount = fachNoten.getNotenanzahl(hj,Notentyp.Schulaufgabe);
        bool hatErsatzpruefung = fachNoten.getNotenanzahl(hj,Notentyp.Ersatzprüfung)>0;

        // wenn gar nichts da ist...
        if (kurzarbeitenCount == 0 && muendlicheCount == 0 && schulaufgabenCount == 0 && !hatErsatzpruefung)
        {
          contr.Add(kurs, toText(0,"","Note",hj));
          continue;
        }

        // zur Probezeit BOS muss noch keine SA vorliegen, wenn nur pro HJ eine geschrieben wird        
        if (istSAFach && schulaufgabenCount == 0)      
          if (!(contr.zeitpunkt == Zeitpunkt.ProbezeitBOS && noetigeAnzahlSchulaufgaben <= 2))
          {
            contr.Add(kurs, toText(schulaufgabenCount,"","Schulaufgabe",hj));              
            meldungSA=true;
          }

        // Ist eine Ersatzprüfung da, erübrigen sich KA, mdl. Noten
        if (!hatErsatzpruefung)
        {
          if (contr.zeitpunkt == Zeitpunkt.ProbezeitBOS)
          {
            if (!AnzahlMuendlicheNotenOKProbezeitBOS(schulaufgabenCount,kurzarbeitenCount,muendlicheCount,fachNoten))
            {
              contr.Add( kurs, toText(muendlicheCount,"mündliche","Note"));
            }
            continue;
          }

          if (kurs.schreibtKA && kurzarbeitenCount == 0)
          {
            contr.Add( kurs, toText(kurzarbeitenCount,"","Kurzarbeite",hj));        
            meldungKA=true;
          }

          // mündliche Noten (bei einstündigen Fächern reicht 1 Note im Schuljahr (also hier nicht prüfen)
          if (!einstuendig && (kurs.schreibtKA && muendlicheCount == 0 || !kurs.schreibtKA && muendlicheCount < 2))
          {
            contr.Add( kurs, toText(muendlicheCount,"mündliche","Note",hj));
            meldungMdl=true;
          }
        }

        if (hj == Halbjahr.Erstes) continue; // Gesamtjahr nur prüfen, wenn auch das zweite vorliegt

        // Gesamtjahresprüfung
        // -------------------
        kurzarbeitenCount = fachNoten.getNotenanzahl(Notentyp.Kurzarbeit);
        muendlicheCount = fachNoten.getNotenanzahl(Notentyp.Ex) + fachNoten.getNotenanzahl(Notentyp.EchteMuendliche);
        schulaufgabenCount = fachNoten.getNotenanzahl(Notentyp.Schulaufgabe);
        hatErsatzpruefung = fachNoten.getNotenanzahl(Notentyp.Ersatzprüfung)>0;
       
                         
        if (schulaufgabenCount < noetigeAnzahlSchulaufgaben && !meldungSA)
        {
          contr.Add(kurs, toText(schulaufgabenCount,"","Schulaufgabe"));
        }

        if (kurs.schreibtKA && kurzarbeitenCount < 2 && !meldungKA)
        {
          contr.Add( kurs, toText(kurzarbeitenCount,"","Kurzarbeit"));
        }

        // wenn Exen geschrieben werden, reichen 2 Exen + 2 mdl. pro Schüler (weil ja eine nicht mitgeschrieben werden muss)
        if (((!kurs.schreibtKA && muendlicheCount < 2+noetigeAnzahlEchteMdl) || muendlicheCount < noetigeAnzahlEchteMdl)  && !meldungMdl)
        {
          contr.Add( kurs,toText(muendlicheCount,"mündliche","Note"));
        }

      }      
    }

    private bool AnzahlMuendlicheNotenOKProbezeitBOS(int schulaufgabenCount, int kurzarbeitenCount, int muendlicheCount, FachSchuelerNoten noten)
    {
      // TODO: Wenn 1 SA und 1 mdl. vorliegt, muss das doch auch reichen?
      if (kurzarbeitenCount > 1 || noten.getNotenanzahl(Halbjahr.Erstes,Notentyp.Ersatzprüfung)>0)
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
          return (kurzarbeitNote >= 6);
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
        var einzigeNote =  exen.Count > 0 ? exen[0] : echteMuendliche[0];
        return (einzigeNote >= 6);
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
        string kuerzel="";
      
        foreach (var fachNoten in noten.alleFaecher)
        {
            kuerzel = fachNoten.getFach.Kuerzel;
            if (kuerzel == "F" || kuerzel == "Smw") continue;  // keine Vorrückungsfächer (Ku?)
            byte? relevanteNote = fachNoten.getRelevanteNote(contr.zeitpunkt);                    
            if (relevanteNote != null) // null muss der notenanzahl-Checker als Problem erkennen
            {                         
                if (relevanteNote == 0) anz6++;                    
                else if (relevanteNote < 4) anz5++;
                else if (relevanteNote == 4) anz4P++;
                else if (relevanteNote >=13) anz1++;
                else if (relevanteNote >= 10) anz2++;
                //else if (relevanteNote >= 7 && fachNoten.getFach.IstSAPFach()) anz3++;

                if (relevanteNote <4 || relevanteNote == 4 && contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
                    m = m + fachNoten.getFach.Kuerzel + "(" + relevanteNote +") ";
            }
        }

        if (contr.zeitpunkt == Zeitpunkt.ErstePA)
        {
          if (anz6 > 1 || (anz6 + anz5) > 3)
          {
            contr.Add(Vorkommnisart.NichtZurPruefungZugelassen,m);            
          }          
          return;
        }
        else if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
        {
          if (anz6>0 || anz5 > 1)
          {
            contr.Add(Vorkommnisart.starkeGefaehrdungsmitteilung,m);
          }
          //else if (anz5 > 0) contr.Add( null, "Gefährdet: " + m); 
          else if (anz4P > 1 || anz5 > 0)
          { 
            contr.Add(Vorkommnisart.BeiWeiteremAbsinken,m);
          }
          contr.ErzeugeZeugnisVorkommnis();

          if (schueler.Data.IsProbezeitBisNull() || !(schueler.Data.ProbezeitBis > DateTime.Parse("01.02." +  (DateTime.Today).Year)))
            return; // bei Schülern ohne PZ geht es zum Halbjahr nur um Gefährdungen
        }
        else if (contr.zeitpunkt == Zeitpunkt.ZweitePA)
        {
          if (anz6 > 0 || anz5 > 1)
          {         
            if (anz6 < 2 && anz6 + anz5 < 3)
              contr.Add(Vorkommnisart.bisherNichtBestandenMAPmoeglich,m);
            else 
              contr.Add(Vorkommnisart.nichtBestandenMAPnichtZugelassen,m);
          }          
          return;        
        }
        
        {
          // TODO: Notenausgleich sauber implementieren
          if (anz6 > 0 || anz5 > 1)
          {
            if (anz2 > 1 || anz1 > 0) contr.Add( null, "Nicht bestanden, Notenausgleich prüfen: " + m);
            else
            {
              if (contr.zeitpunkt == Zeitpunkt.DrittePA)
                contr.Add(Vorkommnisart.endgueltigNichtBestanden,m);
              else if (contr.zeitpunkt == Zeitpunkt.Jahresende)
                contr.Add(Vorkommnisart.KeineVorrueckungserlaubnis,m);
              else 
                contr.Add( null, "Nicht bestanden, kein Notenausgleich möglich: " + m); 


            }
          }
          else
            contr.ErzeugeZeugnisVorkommnis();
        }
    }
  }
}
