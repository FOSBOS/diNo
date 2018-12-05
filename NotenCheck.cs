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
      int anz = schueler.Fachreferat.Count;
      if (anz == 0)
        contr.Add(null,"Es ist keine Note für das Fachreferat vorhanden.");
      else if (anz > 1)
      {
        string s="";
        foreach (var hj in schueler.Fachreferat)
          s += hj.getFach.Kuerzel + " ";

        contr.Add(null, "Es sind " + anz + " Noten für das Fachreferat vorhanden; Fächer: " + s);
      }        
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
      var fpa1 = fpANoten[0];
      var fpa2 = fpANoten[1];

      if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        if (fpa1.IsGesamtNull()) contr.Add(null, "Es liegt keine FpA-Note vor.");
        else if (fpa1.Gesamt < 4)
        {
            contr.Add(null, "<b>Die fachpraktische Ausbildung wurde ohne Erfolg durchlaufen.</b>");
        }
        if (fpa1.IsStelleNull())
          contr.Add(null, "Die FpA-Stelle ist nicht angegeben.");
      }            
      else if (contr.zeitpunkt == Zeitpunkt.Jahresende)
      {
        if (fpa1.IsGesamtNull() || fpa2.IsGesamtNull()) contr.Add(null, "Es liegt keine FpA-Note vor.");
        else if (fpa1.Gesamt < 4 || fpa2.Gesamt < 4 || fpa1.Gesamt+fpa2.Gesamt < 10)
        {
            contr.Add(null, "<b>Die fachpraktische Ausbildung wurde ohne Erfolg durchlaufen.</b>");
        }
        if (fpa2.IsStelleNull())
          contr.Add(null, "Die FpA-Stelle ist nicht angegeben.");

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
        if (seminarfachnoten[0].IsGesamtnoteNull())
        {
          contr.Add(null, "Es liegt keine Seminarfachnote vor.");
        }
        else
        {
          /* s. UnterpunktungsChecker
          var note = seminarfachnoten[0].Gesamtnote;

          if (note < 4)
          {
            contr.Add(null, "Im Seminarfach wurden " + note + " Punkte erzielt.");
          }
          */

          if (seminarfachnoten[0].IsThemaLangNull() && seminarfachnoten[0].IsThemaKurzNull())
          {
            contr.Add(null, "Es liegt kein Seminarfachthema vor.");
          }
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
      if (schueler.AlteFOBOSO())
      {
        CheckAlt(schueler);
        return;
      }
      
      foreach (var fachNoten in noten.alleKurse)
      {
        Kurs kurs = Zugriff.Instance.KursRep.Find(fachNoten.kursId);
        if (contr.modus == NotenCheckModus.EigeneNotenVollstaendigkeit && (kurs.getLehrer == null || Zugriff.Instance.lehrer.Id != kurs.getLehrer.Id))
          continue;

        // Zweite PA: nur Vorliegen der Prüfungsnoten prüfen
        if (contr.zeitpunkt == Zeitpunkt.ZweitePA)
        {
          if (fachNoten.getFach.IstSAPFach(schueler.Zweig) && fachNoten.getNotenanzahl(Notentyp.APSchriftlich) == 0)
          {
            contr.Add(kurs, "Es liegt keine Note in der schriftlichen Abschlussprüfung vor.");
            if (fachNoten.getFach.Kuerzel == "E" && fachNoten.getNotenanzahl(Notentyp.APMuendlich) == 0)
            {
              contr.Add(kurs, "Es liegt keine Note in der Gruppenprüfung vor.");
            }
          }
          continue;
        }

        // Grunddaten dieses Fachs
        // -----------------------
        int noetigeAnzahlSchulaufgaben = fachNoten.getFach.AnzahlSA(schueler.Zweig, schueler.getKlasse.Jahrgangsstufe);
        bool istSAFach = noetigeAnzahlSchulaufgaben > 0;
        bool istSAPFach = fachNoten.getFach.IstSAPFach(schueler.Zweig);

        // Halbjahresprüfung
        // -----------------
        Halbjahr hj;
        if (contr.zeitpunkt == Zeitpunkt.ProbezeitBOS || contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
          hj = Halbjahr.Erstes;
        else
          hj = Halbjahr.Zweites;

        // die Prüfung unterscheidet wie der bisherige Notenbogen nicht, ob die Note aus einer Ex oder echt mündlich ist - das verantwortet der Lehrer
        int kurzarbeitenCount = fachNoten.getNotenanzahl(hj, Notentyp.Kurzarbeit);
        int muendlicheCount = fachNoten.getNotenanzahl(hj, Notentyp.Ex) + fachNoten.getNotenanzahl(hj, Notentyp.EchteMuendliche) + fachNoten.getNotenanzahl(hj, Notentyp.Fachreferat);
        int schulaufgabenCount = fachNoten.getNotenanzahl(hj, Notentyp.Schulaufgabe);
        bool hatErsatzpruefung = fachNoten.getNotenanzahl(hj, Notentyp.Ersatzprüfung) > 0;

        // wenn gar nichts da ist...
        if (kurzarbeitenCount == 0 && muendlicheCount == 0 && schulaufgabenCount == 0 && !hatErsatzpruefung)
        {
          contr.Add(kurs, toText(0, "", "Note", hj));
          continue;
        }
        if (fachNoten.NoteUngueltig)
        {
          contr.Add(kurs, "Die Gesamtnote ist ungültig.");
          continue;
        }

        if (contr.zeitpunkt == Zeitpunkt.ProbezeitBOS)
        {
          // zur Probezeit BOS muss eine SA vorliegen, wenn 2 SA pro Hj (Vorklasse) oder ein SAP-Fach (mit SA, nicht Vk!) vorliegt
          if (schulaufgabenCount == 0 && ((istSAPFach && noetigeAnzahlSchulaufgaben==1) || noetigeAnzahlSchulaufgaben > 1))
          {
            contr.Add(kurs, toText(schulaufgabenCount, "", "Schulaufgabe", hj));
          }

          if (hatErsatzpruefung) continue;

          // Schreibt der Lehrer KA, dann muss sie bei allen Schülern vorliegen
          if (kurs.schreibtKA && kurzarbeitenCount==0)
          {
            contr.Add(kurs, toText(kurzarbeitenCount, "", "Kurzarbeite", hj));
          }

          // Profilfach 2: mind. 2 Noten
          if (istSAFach && !istSAPFach && schulaufgabenCount + kurzarbeitenCount + muendlicheCount <2)
          {
            contr.Add(kurs, toText(schulaufgabenCount + kurzarbeitenCount + muendlicheCount, "", "Note", hj));
            continue;
          }

          // zusätzliche mdl. Note, falls kritisch (<4 P. im Hj)
          if (kurs.schreibtKA && muendlicheCount == 0)
          {
            byte punktePZ = fachNoten.getHjLeistung(HjArt.Hj1).Punkte;
            if (punktePZ < 4)
              contr.Add(kurs, toText(muendlicheCount, "mündliche", "Note", hj));
          }
          else if (!kurs.schreibtKA && muendlicheCount < 2)
          {
            contr.Add(kurs, toText(muendlicheCount, "mündliche", "Note", hj));
          }
        }
        else // alles außer PZ-BOS:       
        {
          if (schulaufgabenCount < noetigeAnzahlSchulaufgaben)
          {
            contr.Add(kurs, toText(schulaufgabenCount, "", "Schulaufgabe"));
          }
          if (schulaufgabenCount > noetigeAnzahlSchulaufgaben)
          {
            contr.Add(kurs, "Es sind zuviele Schulaufgaben eingetragen.");
          }
          if (hatErsatzpruefung) continue;

          if (kurs.schreibtKA && kurzarbeitenCount ==0)
          {
            contr.Add(kurs, toText(kurzarbeitenCount, "", "Kurzarbeite"));
          }

          if ((!kurs.schreibtKA && muendlicheCount < 3) || muendlicheCount == 0)
          {
            contr.Add(kurs, toText(muendlicheCount, "mündliche", "Note"));
          }
        }
      }
    }



  private void CheckAlt(Schueler schueler)
    { 
      //List<string> faecherOhneNoten = new List<string>(); 
      foreach (var fachNoten in noten.alleKurse)
      {
        Kurs kurs = Zugriff.Instance.KursRep.Find(fachNoten.kursId);
        if (contr.modus == NotenCheckModus.EigeneNotenVollstaendigkeit && (kurs.getLehrer == null || Zugriff.Instance.lehrer.Id != kurs.getLehrer.Id))
          continue;

        // Zweite PA: nur Vorliegen der Prüfungsnoten prüfen
        // -------------------------------------------------
        if (contr.zeitpunkt == Zeitpunkt.ZweitePA)
        {
          if (fachNoten.getFach.IstSAPFach(schueler.Zweig) && fachNoten.getNotenanzahl(Notentyp.APSchriftlich) == 0)
          {
            contr.Add(kurs, "Es liegt keine Note in der schriftlichen Abschlussprüfung vor.");
            if (fachNoten.getFach.Kuerzel == "E" && fachNoten.getNotenanzahl(Notentyp.APMuendlich) == 0)
            {
              contr.Add(kurs, "Es liegt keine Note in der Gruppenprüfung vor.");
            }
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
        Kurs derKurs = Zugriff.Instance.KursRep.Find(fachNoten.kursId);
        if (derKurs.getLehrer == null)
        {
          // vermutlich ein Dummy-Kurs, der aus der elften Klasse übernommen wurde. Prüfe nur Jahresfortgang und Zeugnisnote
          byte? relevanteNote = fachNoten.getRelevanteNote(contr.zeitpunkt);
          if (relevanteNote == null)
          {
            contr.Add(kurs, "Es liegt keine Note aus der 11. Klasse vor.");
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
        int muendlicheCount = fachNoten.getNotenanzahl(hj,Notentyp.Ex) + fachNoten.getNotenanzahl(hj,Notentyp.EchteMuendliche) + fachNoten.getNotenanzahl(hj,Notentyp.Fachreferat);
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
        muendlicheCount = fachNoten.getNotenanzahl(Notentyp.Ex) + fachNoten.getNotenanzahl(Notentyp.EchteMuendliche) + fachNoten.getNotenanzahl(Notentyp.Fachreferat);
        schulaufgabenCount = fachNoten.getNotenanzahl(Notentyp.Schulaufgabe);
        hatErsatzpruefung = fachNoten.getNotenanzahl(Notentyp.Ersatzprüfung)>0;
       
        if (!hatErsatzpruefung)
        {                 
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
        SchuelerNoten n = schueler.getNoten;
        n.SetZeitpunkt(contr.zeitpunkt);

      // Integrationsklasse: dort gibt es kein Bestehen...
      /*if (schueler.getKlasse.Bezeichnung=="IV")
      {          
        return;
      }*/
      if (contr.zeitpunkt == Zeitpunkt.ProbezeitBOS)
      {
        if (n.HatNichtBestanden())
        {
          contr.Add(null, "<b>Probezeit nicht bestanden</b> " + n.Unterpunktungen, true);
        }
      }

      else if (contr.zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        if (n.HatNichtBestanden())
        {
          contr.Add(Vorkommnisart.starkeGefaehrdungsmitteilung, n.Unterpunktungen, true);
          if (!schueler.Data.IsProbezeitBisNull() && (schueler.Data.ProbezeitBis > DateTime.Parse("01.02." + (Zugriff.Instance.Schuljahr + 1))))
            contr.Add(null, "<b>Probezeit nicht bestanden</b> " + n.Unterpunktungen, true);
        }
        else if (n.anz4P > 1 || n.AnzahlNoten(5) > 0)
        {
          contr.Add(Vorkommnisart.BeiWeiteremAbsinken, n.Unterpunktungen, true);
        }
      }

      else if (contr.zeitpunkt == Zeitpunkt.ErstePA)
      {
        if (schueler.AlteFOBOSO() && (n.AnzahlNoten(6) > 1 || (n.AnzahlNoten(6) + n.AnzahlNoten(5)) > 3))
        {
          contr.Add(Vorkommnisart.NichtZurPruefungZugelassen, n.Unterpunktungen, true);
          return;
        }

        // nur falls ein Schüler bereits zuviele schlechte Noten in Nichtprüfungsfächer hat
        if (!schueler.AlteFOBOSO() && (n.AnzahlNoten(6, false) * 2 + n.AnzahlNoten(5, false)) > 2)
        {
          contr.Add(Vorkommnisart.NichtZurPruefungZugelassen, n.Unterpunktungen, true);
          return;
        }

        if (n.AnzahlNoten(6, false) + n.AnzahlNoten(5, false) > 0)
          contr.Add(null, "Unterpunktet in einem Nichtprüfungsfach " + n.Unterpunktungen, true);
      }

      else if (contr.zeitpunkt == Zeitpunkt.ZweitePA)
      {
        if (n.HatNichtBestanden())
        {
          if (n.MAPmoeglich())
            contr.Add(Vorkommnisart.bisherNichtBestandenMAPmoeglich, n.Unterpunktungen, true);
          else
            contr.Add(Vorkommnisart.nichtBestandenMAPnichtZugelassen, n.Unterpunktungen, true);
          if (n.KannAusgleichen())
            contr.Add(null, "Notenausgleich möglich");
        }
        return;
      }

      else if (contr.zeitpunkt == Zeitpunkt.DrittePA)
      {
        if (n.HatNichtBestanden())
        {
          if (n.KannAusgleichen()) contr.Add(null, "Nicht bestanden, Notenausgleich möglich: " + n.Unterpunktungen, true);
          else
          {
            contr.Add(Vorkommnisart.NichtBestanden, n.Unterpunktungen, true);
            if (n.DarfInBOS13())
              contr.Add(Vorkommnisart.VorrueckenBOS13moeglich, "");
          }
        }
      }
      else // Jahresende
      {
        if (schueler.getKlasse.Jahrgangsstufe <= Jahrgangsstufe.Elf)
        {
          if (n.HatNichtBestanden()) // Jgstufe 11
          {
            contr.Add(Vorkommnisart.KeineVorrueckungserlaubnis, n.Unterpunktungen, true);
          }
        }
        else // Vorklassen
        {
          if (n.HatNichtBestanden())
            contr.Add(Vorkommnisart.NichtBestanden, n.Unterpunktungen, true);

          // Schüler der BOS-Vk erhalten mittlere Reife, wenn sie bestanden haben:
          else if (schueler.Data.Schulart == "B")
            contr.Add(Vorkommnisart.MittlereReife, "");

          if (n.HatIn12KeinePZ())
            contr.Add(Vorkommnisart.KeineProbezeitNaechstesSJ, "");
        }        
      }
    }
  }

  // Prüft, ob die Abiergebnisse zum Bestehen ausreichen
  public class AbiergebnisChecker : NotenCheck
  {
    public AbiergebnisChecker(NotenCheckController contr) :base (contr)
    { }
        
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return jahrgangsstufe >= Jahrgangsstufe.Zwoelf;
    }
   
    public override void Check(Schueler schueler)
    {
      base.Check(schueler);
      var n = schueler.getNoten;
      
      if (n.HatAbiNichtBestanden())
      {        
        contr.Add(Vorkommnisart.PruefungNichtBestanden, n.UnterpunktungenAbi);
      }
    }
  }

  // Gibt nur die Ergebnisse der MAP für die 3. PA aus
  public class MAPChecker : NotenCheck
  {
    public MAPChecker(NotenCheckController contr) :base (contr)
        { }
        
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return true;
    }
   
    public override void Check(Schueler schueler)
    {
      base.Check(schueler);
          
      foreach (var fach in noten.alleKurse)
      {         
        if (fach.getFach.Kuerzel!="E" && fach.getNotenanzahl(Halbjahr.Zweites,Notentyp.APMuendlich)>0)
        {
          contr.Add(null,"MAP in " + fach.getFach.Kuerzel + " mit " + fach.getNoten(Halbjahr.Zweites,Notentyp.APMuendlich)[0]
            + " Punkten ergibt im Zeugnis: " + fach.getSchnitt(Halbjahr.Zweites).Abschlusszeugnis.GetValueOrDefault());
        }
      } 
    }
  }

  // Ermittelt bei 2./3.PA, ob ein Schüler für die Eliteförderung in Frage kommt.
  public class EliteChecker : NotenCheck
  {
    public EliteChecker(NotenCheckController contr) :base (contr)
    { }
        
    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return true;
    }
   
    public override void Check(Schueler schueler)
    {
      int sumSAP=0;
      int noteSAP;
      base.Check(schueler);
      decimal DNote;
      if (!schueler.Data.IsDNoteAllgNull())
        DNote = schueler.Data.DNoteAllg;
      else if (!schueler.Data.IsDNoteNull())
        DNote = schueler.Data.DNote;
      else return;

      if (Math.Floor(10*DNote) > 13) // Schnitt mindestens 1.3
        return;

      foreach (var fach in noten.alleKurse)
      {                       
        if (!fach.getFach.IstSAPFach(schueler.Zweig)) continue;
        if (fach.getNotenanzahl(Halbjahr.Zweites,Notentyp.APSchriftlich)==0) return; // Note fehlt
        noteSAP = fach.getNoten(Halbjahr.Zweites,Notentyp.APSchriftlich)[0];
        if (noteSAP<10) return; // keine SAP-Note darf einstellig sein.
        sumSAP += noteSAP;
      } 
      if (sumSAP>=50) // Schnitt aller SAP muss mindestens 12,5 Punkte sein
        contr.Add(null,"ggf. Vorschlag für Eliteförderung, Durchschnitt: " + DNote);
      else
        contr.Add(null, "Durchschnitt: " + DNote + ", aber Prüfung zu 'schlecht'.");
    }
  }

  // Legt das passende Zeugnisvorkommnis an
  public class ZeugnisVorkommnisAnlegen : NotenCheck
  {
    public ZeugnisVorkommnisAnlegen(NotenCheckController contr) : base(contr)
    { }

    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return true;
    }

    public override void Check(Schueler schueler)
    {
      Vorkommnisart v = schueler.Zeugnisart(contr.zeitpunkt);
      if (v == Vorkommnisart.NotSet) return;

      schueler.AddVorkommnis(v, Zugriff.Instance.Zeugnisdatum, ""); // Zeugnis als Vorkommnis anlegen
      if (v == Vorkommnisart.allgemeineHochschulreife)
        contr.Add(v, ""); // zusätzliche Ausgabe für die Meldungsliste        
    }
  }
}
