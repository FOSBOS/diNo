using diNo.diNoDataSetTableAdapters;
using System;

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
    protected string toText(int z, string adjektiv = "", string substantiv = "", Halbjahr hj = Halbjahr.Ohne)
    {
      string res = (hj == Halbjahr.Zweites) ? "Im 2. Halbjahr" : "Es";

      if (adjektiv != "")
      {
        if (z == 0) adjektiv += "n "; // mündlichen
        else adjektiv += " ";      // mündliche
      }
      if (substantiv != "")
      {
        if (z == 1) substantiv += " "; // Note
        else substantiv += "n ";    // Noten
      }
      if (z == 0) res += " sind keine " + adjektiv + substantiv;
      else if (z == 1) res += " ist nur eine " + adjektiv + substantiv;
      else res += " sind nur " + z + " " + adjektiv + substantiv;

      res += "vorhanden.";
      return res;
    }
  }

  /// <summary>
  /// Klasse zur Prüfung, ob alle Schüler einer Klasse ein Fachreferat haben.
  /// </summary>
  public class FachreferatChecker : NotenCheck
  {
    public FachreferatChecker(NotenCheckController contr) : base(contr)
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
        contr.Add(null, "Es ist keine Note für das Fachreferat vorhanden.");
      else if (anz > 1)
      {
        string s = "";
        foreach (var hj in schueler.Fachreferat)
          s += hj.getFach.Kuerzel + " ";

        contr.Add(null, "Es sind " + anz + " Noten für das Fachreferat vorhanden; Fächer: " + s);
      }
    }
  }

  /// <summary>
  /// Prüft, ob die fachpraktische Ausbildung mit Erfolg durchlaufen wurde.
  /// </summary>
  public class FpABestandenChecker : NotenCheck
  {
    public FpABestandenChecker(NotenCheckController contr) : base(contr)
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
        else if (fpa1.Gesamt < 4 || fpa2.Gesamt < 4 || fpa1.Gesamt + fpa2.Gesamt < 10)
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
  public class SeminarfachChecker : NotenCheck
  {
    public SeminarfachChecker(NotenCheckController contr) : base(contr)
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
        contr.Add(null, "Es liegt keine Seminarfachnote vor.");
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
    public NotenanzahlChecker(NotenCheckController contr) : base(contr)
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
          }
          if (fachNoten.getFach.Kuerzel == "E" && fachNoten.getNotenanzahl(Notentyp.APMuendlich) == 0)
          {
            contr.Add(kurs, "Es liegt keine Note in der Gruppenprüfung vor.");
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
        int muendlicheCount = fachNoten.getNotenanzahl(hj, Notentyp.Ex) + fachNoten.getNotenanzahl(hj, Notentyp.EchteMuendliche);
        int schulaufgabenCount = fachNoten.getNotenanzahl(hj, Notentyp.Schulaufgabe);

        // wenn gar nichts da ist...
        if (kurzarbeitenCount == 0 && muendlicheCount == 0 && schulaufgabenCount == 0)
        {
          contr.Add(kurs, toText(0, "", "Note", hj));
          continue;
        }
        if (fachNoten.NoteUngueltig)
        {
          contr.Add(kurs, "Die Gesamtnote ist ungültig.");
          continue;
        }
        else if (fachNoten.BerechnungFehlt)
        {
          contr.FehlendeBerechnung++;
        }

        if (contr.zeitpunkt == Zeitpunkt.ProbezeitBOS)
        {
          // Prüfungsfächer BOS12 und und SA-Fächer Vorklassen: zur Probezeit muss eine SA vorliegen und eine weitere Note
          if ((istSAPFach && noetigeAnzahlSchulaufgaben == 1 && schueler.getKlasse.Jahrgangsstufe < Jahrgangsstufe.Dreizehn) || noetigeAnzahlSchulaufgaben > 1)
          {
            if (schulaufgabenCount == 0)
              contr.Add(kurs, toText(schulaufgabenCount, "", "Schulaufgabe", hj));
            if (kurzarbeitenCount == 0 && muendlicheCount == 0)
              contr.Add(kurs, "Es liegt keine sonstige Leistung vor.");
          }
          else if (schulaufgabenCount == 0 && kurzarbeitenCount == 0 && muendlicheCount < 2)
          {
            contr.Add(kurs, "Es liegen nicht genügend sonstige Leistungen vor.");
          }

          // zusätzliche mdl. Note, falls kritisch (<4 P. im Hj)
          else if (muendlicheCount == 0)
          {
            byte punktePZ = fachNoten.getHjLeistung(HjArt.Hj1).Punkte;
            if (punktePZ < 4)
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

          if (kurs.schreibtKA && kurzarbeitenCount == 0)
          {
            contr.Add(kurs, toText(kurzarbeitenCount, "", "Kurzarbeite"));
          }

          if ((!kurs.schreibtKA && muendlicheCount < (kurs.getFach.NichtNC ? 2 : 3)) || muendlicheCount == 0)
          {
            contr.Add(kurs, toText(muendlicheCount, "mündliche", "Note"));
          }
        }
      }
    }
  }

  public class UnterpunktungChecker : NotenCheck
  {
    public UnterpunktungChecker(NotenCheckController contr) : base(contr)
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
        else if (n.anz4P > 1 || n.AnzahlNoten(5) > 0 || n.AnzahlNoten(6) > 0)
        {
          contr.Add(Vorkommnisart.BeiWeiteremAbsinken, n.Unterpunktungen, true);
        }
      }

      else if (contr.zeitpunkt == Zeitpunkt.ErstePA)
      {
        // nur falls ein Schüler bereits zuviele schlechte Noten in Nichtprüfungsfächer hat
        if (n.AnzahlNoten(6, false) * 2 + n.AnzahlNoten(5, false) > 2)
        {
          contr.Add(Vorkommnisart.NichtZurPruefungZugelassen, n.Unterpunktungen, true);
        }
      }

      else if (contr.zeitpunkt == Zeitpunkt.ZweitePA)
      {
        bool nb = false, nz = false;
        if (n.HatAbiNichtBestanden())
        {
          contr.Add(Vorkommnisart.PruefungNichtBestanden, n.UnterpunktungenAbi);
          nz = n.WegenAbiNichtZurMAPZugelassen();
        }
        if (n.HatNichtBestanden())
        {
          nb = true;
          nz = nz || !n.MAPmoeglich();
        }
        if (nz)
          contr.Add(Vorkommnisart.nichtBestandenMAPnichtZugelassen, n.Unterpunktungen, true);
        else if (nb)
          contr.Add(Vorkommnisart.bisherNichtBestandenMAPmoeglich, n.Unterpunktungen, true);

        return;
      }

      else if (contr.zeitpunkt == Zeitpunkt.DrittePA)
      {
        bool nb = false;
        if (n.HatAbiNichtBestanden())
        {
          nb = true;
          contr.Add(Vorkommnisart.PruefungNichtBestanden, n.UnterpunktungenAbi);
        }
        if (n.HatNichtBestanden())
        {
          nb = true;
          contr.Add(Vorkommnisart.NichtBestanden, n.Unterpunktungen, true);
        }
        if (nb && n.DarfInBOS13())
          contr.Add(Vorkommnisart.VorrueckenBOS13moeglich, "");
      }
      else // Jahresende
      {
        if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
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
          else if (schueler.Data.Schulart == "B" && Zugriff.Instance.globaleKonstanten.Schuljahr != 2019)
            contr.Add(Vorkommnisart.MittlereReife, "");

          if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Vorklasse && n.HatIn12KeinePZ())
            contr.Add(Vorkommnisart.KeineProbezeitNaechstesSJ, "");
        }
      }
    }
  }

  // Prüft, ob die Abiergebnisse zum Bestehen ausreichen
  /*
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
  */

  // Gibt nur die Ergebnisse der MAP für die 3. PA aus
  public class MAPChecker : NotenCheck
  {
    public MAPChecker(NotenCheckController contr) : base(contr)
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
        if (fach.getFach.Kuerzel != "E" && fach.getNotenanzahl(Halbjahr.Zweites, Notentyp.APMuendlich) > 0)
        {
          contr.Add(null, "MAP in " + fach.getFach.Kuerzel + " mit " + fach.getNoten(Halbjahr.Zweites, Notentyp.APMuendlich)[0] + " P.");
        }
      }
    }
  }


  // Gibt eine Meldung aus, wenn Legasthenie angehakt
  public class LRSChecker : NotenCheck
  {
    public LRSChecker(NotenCheckController contr) : base(contr)
    { }

    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return true;
    }

    public override void Check(Schueler schueler)
    {
      base.Check(schueler);

      if (schueler.IsLegastheniker)
      {
        contr.Add(null, "Die bisherigen Regelungen zum Nachteilsausgleich bleiben bestehen.");
      }
    }
  }

  // Prüft, ob die richtige Anzahl an HjLeistungen eingebracht wurden
  public class EinbringungsChecker : NotenCheck
  {
    public EinbringungsChecker(NotenCheckController contr) : base(contr)
    { }

    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return true;
    }

    public override void Check(Schueler schueler)
    {
      if (schueler.Data.Berechungsstatus == (byte)Berechnungsstatus.Unberechnet)
      {
        if (contr.zeitpunkt > Zeitpunkt.ErstePA)
          contr.Add(null, "Die Gesamtergebnisse sind noch nicht berechnet.");
        return;
      }
      int notw = schueler.GetAnzahlEinbringung();
      int eing = schueler.punktesumme.Anzahl(PunktesummeArt.HjLeistungen);
      if (eing > 0 && notw != eing)
      {
        contr.Add(null, "Es wurden " + eing + " statt " + notw + " Halbjahresleistungen eingebracht.");
        return;
      }
      if (schueler.Data.Berechungsstatus == (byte)Berechnungsstatus.ZuWenigeHjLeistungen)
      {
        contr.Add(null, "Es wurden zu wenige Halbjahresleistungen eingebracht.");
        return;
      }

      // Kontrolle, ob unterwegs nichts verloren gegangen ist:
      if (contr.zeitpunkt > Zeitpunkt.ErstePA)
      {
        notw = schueler.hatVorHj ? 40 : 26;
        eing = schueler.punktesumme.Anzahl(PunktesummeArt.Gesamt);
        if (eing != notw)
          contr.Add(null, "Einbringungsfaktor " + eing + " statt " + notw);
      }
    }
  }

  // Ermittelt bei 2./3.PA, ob ein Schüler für die Eliteförderung in Frage kommt.
  public class EliteChecker : NotenCheck
  {
    public EliteChecker(NotenCheckController contr) : base(contr)
    { }

    public override bool CheckIsNecessary(Jahrgangsstufe jahrgangsstufe, Schulart schulart)
    {
      return true;
    }

    public override void Check(Schueler schueler)
    {
      int sumSAP = 0;
      int noteSAP;
      base.Check(schueler);
      decimal DNote;
      if (!schueler.Data.IsDNoteNull())
        DNote = schueler.Data.DNote;
      else return;

      if (Math.Floor(10 * DNote) > 13) // Schnitt mindestens 1.3
        return;

      foreach (var fach in noten.alleKurse)
      {
        if (!fach.getFach.IstSAPFach(schueler.Zweig)) continue;
        if (fach.getNotenanzahl(Halbjahr.Zweites, Notentyp.APSchriftlich) == 0) return; // Note fehlt
        noteSAP = fach.getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich)[0];
        if (noteSAP < 10) return; // keine SAP-Note darf einstellig sein.
        sumSAP += noteSAP;
      }
      if (sumSAP >= 50) // Schnitt aller SAP muss mindestens 12,5 Punkte sein
      {
        decimal erg = (17 - (decimal)schueler.punktesumme.Summe(PunktesummeArt.Gesamt) / schueler.punktesumme.Anzahl(PunktesummeArt.Gesamt)) / 3;
        contr.Add(null, "V" +
          "orschlag für Eliteförderung, Durchschnitt: " + String.Format("{0:0.0000}", erg));
      }
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
