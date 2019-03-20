using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  /// <summary>
  /// Verwaltet alle Noten eines Schülers
  /// </summary>
  public class SchuelerNoten
  {
    private Schueler schueler;
    public List<FachSchuelerNoten> alleKurse; // enthält nur die aktuell besuchten Kurse
    public List<FachSchuelerNoten> alleFaecher; // enthält alle Fächer, die der S jemals belegt hat
    public List<FachSchuelerNoten> alleSprachen; // für die Fremdsprachenliste
    public List<FachSchuelerNoten> FaecherOhneKurse; // enthält alle Fächer, die der S aus vorigen JgStufen mitbringt

    // die folgendes Array verwaltet die Anzahl der Einser, Zweier, usw., getrennt nach SAP-Fach und Nebenfach
    // anzahlNoten[6,1] ergibt z.B. die Anzahl der Sechser in SAP-Fächern, anzahlNoten[5,0] die Anzahl der Fünfer in Nebenfächern
    private int[,] anzahlNoten;
    private int abi5er=0,abi6er=0; // Anzahl 5er und 6 im Abi
    private Zeitpunkt zeitpunkt = (Zeitpunkt)Zugriff.Instance.aktZeitpunkt;
    public string Unterpunktungen, UnterpunktungenAbi;
    public bool hatDeutsch6 = false; // kann nicht ausgeglichen werden
    public int anz4P=0;
    public double Punkteschnitt = 0;
    public List<HjLeistung> Fachreferat = new List<HjLeistung>(); // sollte i.d.R. nur einelementig sein, aber wegen irrtümlicher Doppelvergabe

    public SchuelerNoten(Schueler s)
    {
      schueler = s;
      diNoDataSet.KursDataTable kurse = schueler.Kurse; // ermittle alle Kurse, die der S besucht
      alleFaecher = new List<FachSchuelerNoten>();
      alleKurse = new List<FachSchuelerNoten>();
      alleSprachen = new List<FachSchuelerNoten>();
      FaecherOhneKurse = new List<FachSchuelerNoten>();

      foreach (var kurs in kurse)
      {
        var fsn = new FachSchuelerNoten(schueler, kurs.Id, this);
        alleFaecher.Add(fsn);
        alleKurse.Add(fsn);
        if (fsn.getFach.getKursniveau() != Kursniveau.None) alleSprachen.Add(fsn);
      }

      // alle Fächer des Schülers ohne Kurs finden und diese HjLeistungen laden 
      diNoDataSet.FachDataTable fDT = (new FachTableAdapter()).GetFaecherOhneKurseBySchuelerId(schueler.Id,(int)schueler.getKlasse.Jahrgangsstufe);
      foreach (var fachR in fDT)
      {
        Fach fach = Zugriff.Instance.FachRep.Find(fachR.Id);
        var fsn = new FachSchuelerNoten(schueler, fach, this);
        alleFaecher.Add(fsn);
        if (fsn.getFach.getKursniveau() != Kursniveau.None) alleSprachen.Add(fsn);
        FaecherOhneKurse.Add(fsn);
      }

      // Fachreferat als eigenes Fach führen --> macht leider auch viele Probleme, deshalb erst mal so lassen
      // (Sollte es mehrere FR geben, bleibt aber nur das letzte übrig, weil alle in denselben Index geschrieben werden).
      /*
      if (schueler.Fachreferat.Count>0)
      {
        alleFaecher.Add(new  FachSchuelerNoten(schueler, schueler.Fachreferat));  
      }
      */

      Zweig z = (schueler.AlteFOBOSO() ? Zweig.None : schueler.Zweig); // Profilfächer haben neue Sortierung
      alleFaecher.Sort((x,y) => x.getFach.Sortierung(z).CompareTo(y.getFach.Sortierung(z)));
      alleKurse.Sort((x,y) => x.getFach.Sortierung(z).CompareTo(y.getFach.Sortierung(z)));

      // Schnitt, Unterpunktungen etc. berechnen
      anzahlNoten = new int[7, 2];
      InitAnzahlNoten();
    }

    public FachSchuelerNoten getFach(int kursid)
    {
      foreach (FachSchuelerNoten f in alleFaecher)
      {
        if (f.kursId == kursid) return f;
      }
      return null;
      //throw new IndexOutOfRangeException("FachSchuelerNoten.getFach: falsche kursid");            
  }

    /// <summary>
    /// Liefert die Noten des Schülers im übergebenen Fach.
    /// </summary>
    /// <param name="fachKuerzel">Das Fachkürzel.</param>
    /// <returns>Die FachNoten oder null, wenn der Schüler das fach nicht belegt.</returns>
    public FachSchuelerNoten FindeFach(string fachKuerzel, bool throwExceptionIfNotFound)
    {
      foreach (FachSchuelerNoten f in alleFaecher)
      {
        if (f.getFach.Kuerzel.Equals(fachKuerzel, StringComparison.OrdinalIgnoreCase)) return f;
      }
       
      if (throwExceptionIfNotFound)
      {
        throw new InvalidOperationException("Der Schüler belegt Fach " + fachKuerzel + " gar nicht.");
      }

      return null;
    }

    public IList<FachSchuelerNotenDruckAlt> SchuelerNotenDruckAlt(Bericht rptName)
    {
      IList<FachSchuelerNotenDruckAlt> liste = new List<FachSchuelerNotenDruckAlt>();
      foreach (FachSchuelerNoten f in alleKurse)
      {                
        liste.Add(new FachSchuelerNotenDruckAlt(f, f.getFach.IstSAFach(schueler.Zweig, schueler.getKlasse.Jahrgangsstufe), rptName));
      }
      
      if (schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Dreizehn)
      {
        if (!schueler.Data.IsAndereFremdspr2NoteNull())
        {
          liste.Add(new FachSchuelerNotenDruckAlt(
            (schueler.Data.IsAndereFremdspr2TextNull() ? "Andere 2. Fremdsprache" :  schueler.Data.AndereFremdspr2Text ),
            schueler.Data.AndereFremdspr2Note));
        }
        liste.Add(new FachSchuelerNotenDruckAlt(schueler.Seminarfachnote));
      }

      return liste;
    }


    /// <summary>
    /// Liefert eine Liste in der je Fach alle Noten in druckbarer Form vorliegen.
    /// </summary>
    public IList<NotenDruck> SchuelerNotenDruck(Bericht rptName)
    {
      IList<NotenDruck> liste = new List<NotenDruck>();      
      foreach (FachSchuelerNoten f in alleFaecher)
      {               
        liste.Add(NotenDruck.CreateNotenDruck(f,rptName));
      }
      foreach (var f in Fachreferat)
      {
        if (rptName==Bericht.Abiergebnisse)
          liste.Add(new NotenAbiDruck(f));
        else
          liste.Add(new NotenHjDruck(f));
      }

      /*
      if (schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Dreizehn)
      {
        if (!schueler.Data.IsAndereFremdspr2NoteNull())
        {
          liste.Add(new NotenSjDruck(
            (schueler.Data.IsAndereFremdspr2TextNull() ? "Andere 2. Fremdsprache" :  schueler.Data.AndereFremdspr2Text ),
            schueler.Data.AndereFremdspr2Note));
        }
        liste.Add(new FachSchuelerNotenDruckKurz(schueler.Seminarfachnote));
      }
      */
      // FPA ausgeben für Notenmitteilung (im Notenbogen als Bemerkung)
      /*
      if (schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Elf)
      {
        if (rptName == Bericht.Notenmitteilung) liste.Add(new NotenHjDruck(schueler.FPANoten));
        else liste.Add(new NotenSjDruck(schueler.FPANoten));
      }*/
      return liste;      
    }

    public IList<NotenDruck> SchuelerNotenZeugnisDruck(Bericht rptName)
    {
      IList<NotenDruck> liste = new List<NotenDruck>(); 
      if (rptName == Bericht.Abiturzeugnis)
      {
        foreach (FachSchuelerNoten f in alleFaecher)
        {
          liste.Add(new NotenZeugnisDruck(f, rptName));
        }
      }
      else
      foreach (FachSchuelerNoten f in alleKurse)
      {
        if (rptName!=Bericht.Gefaehrdung || f.getRelevanteNote(zeitpunkt)<=4)
          liste.Add(new NotenZeugnisDruck(f, rptName));
      }

      foreach (var f in Fachreferat)
      {
        liste.Add(new NotenZeugnisDruck(f, "Fachreferat in " + f.getFach.BezZeugnis));
      }
      if (rptName != Bericht.Gefaehrdung && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
      {        
        NotenZeugnisDruck f = new NotenZeugnisDruck(schueler.FPANoten);          
        liste.Add(f);
      }
      
      return liste;
    }
     
    // Zu diesem Zeitpunkt werden die Notenanzahlen gebildet,
    // wird dieser geändert, muss neu gerechnet werden
    public void SetZeitpunkt(Zeitpunkt z)
    {
      if (zeitpunkt!=z)
      {
        anzahlNoten = null;
        anzahlNoten = new int[7, 2];
        zeitpunkt = z;
        InitAnzahlNoten();        
      }
    }

    public int AnzahlProbleme()
    {
      return AnzahlNoten(5) + 2*AnzahlNoten(6);
    }
    
    // liefert Anzahl der Sechser, Fünfer, ..., egal ob Prüfungsfach oder nicht
    public int AnzahlNoten(int note)
    {
      return AnzahlNoten(note,true) + AnzahlNoten(note, false);      
    }

    // Doku s. private Variable
    public int AnzahlNoten(int note, bool SAP)
    {
      if (SAP) return anzahlNoten[note,1];
      else return anzahlNoten[note,0];
    }

    private void NimmNote(byte relevanteNote, int istSAP, string kuerzel)
    {
      if (relevanteNote == 0)
      {
        anzahlNoten[6, istSAP]++;
        if (kuerzel == "D") hatDeutsch6 = true; // nur AlteFOBOSO
      }
      else if (relevanteNote < 4) anzahlNoten[5, istSAP]++;
      else if (relevanteNote >= 13) anzahlNoten[1, istSAP]++;
      else if (relevanteNote >= 10) anzahlNoten[2, istSAP]++;
      else if (relevanteNote >= 7) anzahlNoten[3, istSAP]++;
      else anzahlNoten[4, istSAP]++;

      if (relevanteNote == 4) anz4P++; // nur 4P für Gefährdungsmitteilung

      if (relevanteNote < 4 || relevanteNote == 4 && zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
        Unterpunktungen += kuerzel + "(" + relevanteNote + ") ";
    }

    private void InitAnzahlNoten()
    {      
      string kuerzel;
      int Punktesumme = 0;
      int AnzahlFaecher = 0;
      Unterpunktungen = "";
      UnterpunktungenAbi = "";

      List<FachSchuelerNoten> zuAnalysierendeNoten; // zum Abi werden alle Fächer betrachtet
      zuAnalysierendeNoten = (zeitpunkt >= Zeitpunkt.ErstePA && zeitpunkt <= Zeitpunkt.DrittePA ? alleFaecher : alleKurse);

      foreach (var fachNoten in zuAnalysierendeNoten)
      {
        kuerzel = fachNoten.getFach.Kuerzel;
        if (schueler.AlteFOBOSO() &&  (kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Sw" || kuerzel == "Sm" || kuerzel == "Ku")) continue;  // keine Vorrückungsfächer
        if (!schueler.AlteFOBOSO() && fachNoten.getFach.NichtNC) continue;  // Nicht-NC-Fächer zählen gar nicht
        byte? relevanteNote = fachNoten.getRelevanteNote(zeitpunkt);
        int istSAP = fachNoten.getFach.IstSAPFach(schueler.Zweig, schueler.getKlasse.Jahrgangsstufe <= Jahrgangsstufe.Vorklasse) ? 1:0;
        if (relevanteNote != null)
        {
          Punktesumme += relevanteNote.GetValueOrDefault();
          AnzahlFaecher++;
          NimmNote(relevanteNote.GetValueOrDefault(), istSAP, kuerzel);
        }
        if (istSAP==1 && (zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA))
        {
          InitAbiNoten(fachNoten);
        }
      }

      foreach (var f in Fachreferat)
      {
        NimmNote(f.Punkte, 0, "FR"); 
      }

      // TODO: in alleFaecher integrieren
      if (!schueler.Seminarfachnote.IsGesamtnoteNull())
      {
        byte relevanteNote = (byte)schueler.Seminarfachnote.Gesamtnote;
        Punktesumme += relevanteNote;
        AnzahlFaecher++;
        NimmNote(relevanteNote, 0, "Sem");
      }

      // Punkteschnitt berechnen
      if (!schueler.AlteFOBOSO() && zeitpunkt>=Zeitpunkt.ErstePA && zeitpunkt<=Zeitpunkt.DrittePA)
      {
        Punktesumme = schueler.punktesumme.Summe(PunktesummeArt.Gesamt);
        AnzahlFaecher = schueler.punktesumme.Anzahl(PunktesummeArt.Gesamt);
      }
      if (AnzahlFaecher > 0)
      {
        Punkteschnitt = Math.Round((double)Punktesumme / AnzahlFaecher, 2, MidpointRounding.AwayFromZero);
        if (Unterpunktungen != "" && !schueler.AlteFOBOSO() && !(zeitpunkt == Zeitpunkt.Jahresende && schueler.getKlasse.Jahrgangsstufe <= Jahrgangsstufe.Vorklasse))
        { 
          Unterpunktungen += " Schnitt: " + String.Format("{0:0.00}", Punkteschnitt);
          if (zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA)
            Unterpunktungen += "; " + Punktesumme + " P.";
        }
      }
    }

    private void InitAbiNoten(FachSchuelerNoten f)
    {      
      decimal? apg=null;
      if (f.schueler.AlteFOBOSO()) // wenn raus: dann apg ggf. auf Ganzzahl umstellen
      {
        apg = f.getSchnitt(Halbjahr.Zweites).PruefungGesamt;
      }
      else
      {
        HjLeistung hj = f.getHjLeistung(HjArt.AP);
        if (hj!=null)
          apg = hj.Punkte;
      }

      if (apg != null && apg < (decimal)3.5)
      {
        UnterpunktungenAbi += f.getFach.Kuerzel + "-Abi(" + apg + ") ";
        if (apg < (decimal)1.0) abi6er++; else abi5er++;
          
      }
    }

    public bool HatAbiNichtBestanden()
    {
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
        return (abi6er > 0 || abi5er > 2);
      else
        return (2 * abi6er + abi5er > 2);

    }

    public bool HatNichtBestanden()
    {
      if (schueler.AlteFOBOSO() && zeitpunkt!=Zeitpunkt.ProbezeitBOS)        
        return AnzahlNoten(6) > 0 || AnzahlNoten(5) > 1;

      // Achtung: Vorklasse hat am Jahresende eine besondere Bestanden-Regelung
      else if (zeitpunkt == Zeitpunkt.Jahresende && schueler.getKlasse.Jahrgangsstufe <= Jahrgangsstufe.Vorklasse)
      {
        if (AnzahlNoten(6) == 0 && AnzahlNoten(5) == 0) return false;  // bestanden
        if (AnzahlNoten(6) > 0 || AnzahlNoten(5) > 1) return true;  // da geht nichts mehr
        if (AnzahlNoten(5, true) > 0) // eine 5 in D,E oder M kann nur dadurch ausgeglichen werden
          return !(AnzahlNoten(1, true) + AnzahlNoten(2, true) > 0 || AnzahlNoten(3, true) > 1);
        else
          return !(AnzahlNoten(1) + AnzahlNoten(2) > 0 || AnzahlNoten(3) > 1);
      }
      else 
        return !(AnzahlNoten(6) == 0 && AnzahlNoten(5) == 0 ||
          AnzahlNoten(6) == 0 && AnzahlNoten(5) == 1 && Punkteschnitt >= 5 ||
          (AnzahlNoten(6) * 2 + AnzahlNoten(5)) == 2 && Punkteschnitt >= 6);
    }

    public bool KannAusgleichen()
    {
      // geht nur, wenn 1x6 und keine 5 oder 2x5 und keine 6 vorliegt.
      if (!schueler.AlteFOBOSO() || hatDeutsch6 || 2*AnzahlNoten(6) + AnzahlNoten(5) >2 || schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Vorklasse)
        return false;

      if (AnzahlNoten(3,true)>=3) return true; // Ausgleich mit 3x3 in Prüfungsfächern
      if (AnzahlNoten(1)>=1 || AnzahlNoten(2)>=2) // Ausgleich mit 1x1 oder 2x2
      {
        // Prüfungsfächer müssen mit Prüfungsfächern ausgeglichen werden:
        if (2*AnzahlNoten(6,true) + AnzahlNoten(5, true) <= 2*AnzahlNoten(1,true) + AnzahlNoten(2,true))
          return true;
      }
      return false;
    }

    public bool HatIn12KeinePZ()
    { // überall mindestens eine 3
      return AnzahlNoten(6) == 0 && AnzahlNoten(5) == 0 && AnzahlNoten(4) == 0;
    }

    public bool MAPmoeglich()
    {
      if (schueler.AlteFOBOSO()) return MAPmoeglichAlt();

      GEVariante[] gev = new GEVariante[3]; // aus 3 Fächern kann die MAP ausgewählt werden
      byte index = 0; 

      foreach (var f in alleKurse)
      {
        string kuerzel = f.getFach.Kuerzel;       

        HjLeistung hj = f.getHjLeistung(HjArt.GesErg);
        if (hj == null) continue; // sollte natürlich nicht passieren!
        
        // nur in Abifächern außer E kann man MAP machen
        if (f.getFach.IstSAPFach(schueler.Zweig) && kuerzel != "E")
        {        
          int? sap = f.getNote(Halbjahr.Zweites, Notentyp.APSchriftlich);
          if (sap == null) sap = f.getHjLeistung(HjArt.AP).Punkte; // für Testzwecke Meldung über Notenanzahlchecker
          gev[index] = new GEVariante();

          Fachsumme fs = f.SummeHalbjahre();
          gev[index].AP = Notentools.BerechneAbiGes(sap.GetValueOrDefault(), 15); // AP-Gesamtergebnis bei MAP=15
          fs.Add(gev[index].AP, schueler.APFaktor);
          gev[index].GE = fs.GesErg(); //GE in diesem Fach, wenn optimaler MAP
          gev[index].CalcDiff(sap.GetValueOrDefault(), hj.Punkte, schueler.APFaktor); // berechnen, wie sich die Gesamtsumme dadurch verbessert
          index++;
        }
      }
      if (index != 3) throw new Exception(schueler.benutzterVorname + " kann nicht aus 3 Fächern die MAP auswählen.");

      return CheckVariante(gev[0], gev[1]) || CheckVariante(gev[0], gev[2]) || CheckVariante(gev[1], gev[2]);
    }
  
    // ergibt true, wenn er bei dieser Variante besteht
    private bool CheckVariante(GEVariante v1, GEVariante v2)
    {
      Punktesumme ps = schueler.punktesumme;
      int probl = AnzahlProbleme() + v1.ProblemDiff + v2.ProblemDiff; // ProblemDiff ist idR negativ
      int sum = ps.Summe(PunktesummeArt.Gesamt) + v1.PunkteDiff + v2.PunkteDiff;
      bool erg = (probl <= 0 ||
          probl == 1 && sum >= 5 * ps.Anzahl(PunktesummeArt.Gesamt) ||
          probl == 2 && sum >= 6 * ps.Anzahl(PunktesummeArt.Gesamt));
      return erg;
    }

    public bool MAPmoeglichAlt()
    {
      // Anzahl 5er und 6er nach einen MAP mit "bestmöglichem" Ergebnis
      int anz5=0; 
      int anz6=0;
      bool MAPinSAPFach=false; // je eine MAP möglich
      bool MAPinNebenFach=false;
      bool istSAPFach;
      decimal? jf=null;
      decimal? sap=null;
      
      // Seminarfachnote ist gesetzt
      if (!schueler.Seminarfachnote.IsGesamtnoteNull() && schueler.Seminarfachnote.Gesamtnote<4) anz5++;

      foreach (var f in alleKurse)
      {
        string kuerzel = f.getFach.Kuerzel;
        if (kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Sw" || kuerzel == "Sm" || kuerzel == "Ku") continue;  // keine Vorrückungsfächer

        byte? note = f.getRelevanteNote(Zeitpunkt.ZweitePA);
        if (note<4)
        {          
          jf = f.getSchnitt(Halbjahr.Zweites).JahresfortgangMitKomma;
          istSAPFach = f.getFach.IstSAPFach(schueler.Zweig);

          // Nur Fächer in denen eine MAP möglich ist (nicht Englisch und Fächer aus der 11. Klasse)
          if (kuerzel!="E" && jf!=null)
          {
            // 1x MAP im Prüfungsfach und 1x im Nebenfach möglich:
            if (istSAPFach && !MAPinSAPFach) 
            {
              if (f.getNotenanzahl(Halbjahr.Zweites,Notentyp.APSchriftlich)==0) // Note liegt ggf. noch nicht vor.
                return true;                                                    // Meldung über NotenanzahlChecker
              
              sap = f.getNoten(Halbjahr.Zweites,Notentyp.APSchriftlich)[0];
              byte noteMoegl = Notentools.BerechneZeugnisnote(jf,sap,15);
              if (noteMoegl>=4 || note==0) // wenn er auf dem 5er bleibt, soll er lieber ein anderes Fach nehmen
              {
                note = noteMoegl;
                MAPinSAPFach = true;
              }
            }
            if (!istSAPFach && !MAPinNebenFach)
            {
              note = 5; // im Nebenfach nie ein Problem (wenn er sich anstrengt)
              MAPinNebenFach = true;
            }
          }
          if (note==0) anz6++;
          else if (note<4) anz5++;
        }
      }    

      return (anz6==0 && anz5<=1); // dann hätte er bestanden
    }

    public bool DarfInBOS13()
    {
      if (schueler.Data.Schulart != "B" || schueler.getKlasse.Jahrgangsstufe != Jahrgangsstufe.Zwoelf) return false;

      foreach (var f in alleKurse) // auch Nicht-NC-Fächer
      {
        var jn = f.getHjLeistung(HjArt.JN);
        if (jn ==null || jn.Punkte < 4)
          return false;
      }
      return true;
    }
  }

  /// <summary>
  /// Verwaltet alle Noten eines Schülers in einem Fach (=Kurs)
  /// </summary>
  public class FachSchuelerNoten
    {
        public Schueler schueler;
        private SchuelerNoten schuelernoten;
        private Fach fach=null;
        public bool NoteUngueltig = false;
        public int kursId
        {
            get;
            private set;
        }


        // das Array wird über das Halbjahr und den Notentyp indiziert, 
        // jedes Arrayelement enthält eine Liste mit Noten dieses Typs.
        private IList<int>[,] noten = new List<int>[Enum.GetValues(typeof(Halbjahr)).Length,Enum.GetValues(typeof(Notentyp)).Length];
        private BerechneteNote[] schnitte = new BerechneteNote[Enum.GetValues(typeof(Halbjahr)).Length];
        private HjLeistung[] hjLeistung = new HjLeistung[Enum.GetValues(typeof(HjArt)).Length];
        private HjLeistung[] vorHjLeistung = new HjLeistung[2]; // enthält nur 11/1 und 11/2

        public FachSchuelerNoten(Schueler aschueler, int akursid, SchuelerNoten aschuelernoten)
        {
            kursId = akursid;        
            schueler = aschueler;
            schuelernoten = aschuelernoten;
      
            foreach (Halbjahr hj in Enum.GetValues(typeof(Halbjahr)))
            {
                // erstmal leere Notenlisten anlegen
                foreach (Notentyp typ in Enum.GetValues(typeof(Notentyp)))
                    noten[(int)hj,(int)typ] = new List<int>();
            }

            // Notenlisten füllen je Typ und Halbjahr
            LeseNotenAusDB();
        }

        // Kursunabhängige HjLeistungen (z.B. abgelegte Fächer oder Seminararbeit) können hier erzeugt werden
        public FachSchuelerNoten(Schueler aschueler, Fach aFach, SchuelerNoten aschuelernoten)
        {
          fach = aFach;
          kursId = 0;
          schueler = aschueler;
          schuelernoten = aschuelernoten;

          LeseNotenAusDB();
        }

    // HjLeistungen, die als eigenes Fach behandelt werden, aber aus andere Quelle kommen (FR, FpA, Seminar)
    public FachSchuelerNoten(Schueler aschueler, List<HjLeistung> hjList, SchuelerNoten aschuelernoten)
    {
      fach = hjList[0].getFach;
      schuelernoten = aschuelernoten;
      foreach (var hj in hjList)
      {
        hjLeistung[(int)(hj.Art)] = hj;
      }        
    }

    private void LeseNotenAusDB()
    {
      if (kursId>0)
      { 
        diNoDataSet.NoteDataTable notenDT;
        notenDT = new NoteTableAdapter().GetDataBySchuelerAndKurs(schueler.Id, kursId);
        foreach (var noteR in notenDT)
        {
          noten[noteR.Halbjahr,noteR.Notenart].Add(noteR.Punktwert);
        }

        // Schnitte werden direkt gelesen
        diNoDataSet.BerechneteNoteDataTable bnotenDT;
        // liefert max. 2 Datensätze (einen für 1. und 2. Hj.), historische Stände werden nicht geliefert
        bnotenDT = new BerechneteNoteTableAdapter().GetDataBySchuelerAndKurs(kursId, schueler.Id);
        foreach (var bnoteR in bnotenDT)
        {                
          schnitte[(int)(bnoteR.ErstesHalbjahr ? Halbjahr.Erstes : Halbjahr.Zweites)] = 
                    new BerechneteNote(kursId, schueler.Id, bnoteR);
        }
      }
      // HjLeistungen          
      // nur die HjLeistungen holen, die der aktuellen JgStufe entsprechen
      int jg = (int)schueler.getKlasse.Jahrgangsstufe;
      var hjDT = new HjLeistungTableAdapter().GetDataBySchuelerAndFach(schueler.Id,getFach.Id).Where(x => x.JgStufe == jg);
      foreach (var hjR in hjDT)
      {                
        hjLeistung[(int)(hjR.Art)] = new HjLeistung(hjR);
        if ((HjArt)hjR.Art == HjArt.FR)
        {
          schuelernoten.Fachreferat.Add(hjLeistung[(int)(hjR.Art)]);              
        }        
      }

      if (schueler.hatVorHj) // suche 11. Klassnoten
      {
        hjDT = new HjLeistungTableAdapter().GetDataBySchuelerAndFach(schueler.Id, getFach.Id).Where(x => x.JgStufe == 11 && x.Art < 2);
        foreach (var hjR in hjDT)
        {
          vorHjLeistung[(int)(hjR.Art)] = new HjLeistung(hjR);
        }
      }
    }

    /// <summary>
    /// Liefert die erste Note eines Schülers in einem Fach von diesem Typ
    /// </summary>
    public int? getNote(Halbjahr hj, Notentyp typ)
    {
      var n = noten[(int)hj, (int)typ];
      return n.Count == 0 ? null : (int?)n.First();
    }

    /// <summary>
    /// Liefert alle Noten eines Schülers in einem Fach von diesem Typ
    /// </summary>
    public IList<int> getNoten(Halbjahr hj,Notentyp typ)
        {          
            return noten[(int)hj,(int)typ]; // klappt der Cast immer???
        }

        /// <summary>
        /// Liefert die Notenschnitte
        /// </summary>
        public BerechneteNote getSchnitt(Halbjahr hj)
        {
            var s = schnitte[(int)hj];
            if (s==null) return new BerechneteNote(kursId,schueler.Id); // gibt leere Berechnungstabelle zurück
            return s;
        }

        /// <summary>
        /// Liefert die Halbjahresleistungen
        /// </summary>
        public HjLeistung getHjLeistung(HjArt art)
        {
          return hjLeistung[(int)art];
        }

    public HjLeistung getVorHjLeistung(HjArt art)
    {
      return vorHjLeistung[(int)art];
    }

    public void NimmHj(Fachsumme fs, HjArt a, bool vorJahr, bool ignoreEinbringung=false)
    {
      int faktor = 1;
      HjLeistung hj;
      if (vorJahr) hj = getVorHjLeistung(a);
      else hj = getHjLeistung(a);

      if (hj!=null && (hj.Status!=HjStatus.NichtEinbringen || ignoreEinbringung))
      {
        if (a==HjArt.AP)
        {
          faktor = schueler.APFaktor;
        }
        fs.anz += faktor;
        fs.sum += hj.Punkte * faktor;
      }
    }

    /// <summary>
    /// Bestimmt die Punktesumme der eingebrachten Halbjahre dieses Faches 
    /// </summary>
    public Fachsumme SummeHalbjahre(bool ignoreEinbringung = false)
    {
      Fachsumme fs = new Fachsumme();
      NimmHj(fs, HjArt.Hj1, true, false);
      NimmHj(fs, HjArt.Hj2, true, ignoreEinbringung);
      NimmHj(fs, HjArt.Hj1, false, ignoreEinbringung); // NichtNC-Fächer werden zwar nicht eingebracht, GesErg wird aber über alle Hj gebildet
      NimmHj(fs, HjArt.Hj2, false, ignoreEinbringung);
      return fs;
    }

    /// <summary>
    /// Berechnet das Gesamtergebnis für dieses Fach aufgrund der vorliegenden HjLeistungen neu (nur Abiturklassen)
    /// </summary>
    public void BerechneGesErg(Punktesumme p)
    {
      Fachsumme fs;
      Fachsumme fsprache; // Fremdsprache getrennt
      Fachsumme ap = new Fachsumme(); // Abiergebnis
      
      bool istFS = fach.getKursniveau() != Kursniveau.None; // Fremdsprache
      HjLeistung gesErg = getHjLeistung(HjArt.GesErg);
      if (gesErg == null) gesErg = new HjLeistung(schueler.Id,fach,HjArt.GesErg,schueler.getKlasse.Jahrgangsstufe);

      // 4 mögliche Halbjahre:
      fs = SummeHalbjahre(fach.NichtNC); // Nicht-NC-Fächer ausrechnen, aber nicht verbuchen

      // Verbuchen auf die richtige Gesamt-Punktesumme (über alle Fächer)
      if (fach.Kuerzel=="FpA") p.Add(PunktesummeArt.FPA, fs);
      else if (!fach.NichtNC) p.Add(PunktesummeArt.HjLeistungen, fs);

      // AP
      NimmHj(ap, HjArt.AP, false); 
      p.Add(PunktesummeArt.AP, ap);

      // Gesamtergebnis berechnen (dazu das AP-Ergebnis mitnehmen)
      fs.Add(ap);
      fs.SaveGesErg(gesErg);

      // extra Rechnung für Sprachniveau (unabhängig von Einbringung)
      if (istFS) 
      {
        fsprache = SummeHalbjahre(true);
        fsprache.Add(ap);
        gesErg = getHjLeistung(HjArt.GesErgSprache);
        if (gesErg == null) gesErg = new HjLeistung(schueler.Id, fach, HjArt.GesErgSprache, schueler.getKlasse.Jahrgangsstufe);
        fsprache.SaveGesErg(gesErg);
      }
    }

    /// <summary>
    /// Liefert die zur Zeit z (z.B. Probezeit BOS) relevante Note (hier Jahresfortgang Ganzz. 1. Hj.)
    /// </summary>
    public byte? getRelevanteNote(Zeitpunkt z)
    {
      if (!schueler.AlteFOBOSO()) // neue FOBOSO:
      {
        HjLeistung hj;
        if (z <= Zeitpunkt.HalbjahrUndProbezeitFOS) hj = getHjLeistung(HjArt.Hj1);
        else if ((byte)schueler.getKlasse.Jahrgangsstufe < 12)
        {
          hj = getHjLeistung(HjArt.JN);
          if (hj == null)
          {
            hj = getHjLeistung(HjArt.Hj2); // Behelfskonstruktion, falls im 1. Hj keine Note gebildet wurde (z.B. Rücktritt in Fvk)
            NoteUngueltig = true;
          }
        }
        else
        {
          hj = getHjLeistung(HjArt.GesErg);
          if (hj == null)
          {
            hj = getHjLeistung(HjArt.JN); // behelfsweise, damit man grob den Leistungsstand überprüfen kann
            NoteUngueltig = true;
          }
        }

        if (hj == null) return null;
        else
        {
          if (hj.Status == HjStatus.Ungueltig)
            NoteUngueltig = true;
          return hj.Punkte;
        }
      }

      // Alte FOBOSO:
      if (Zugriff.Instance.KursRep.Find(kursId).getLehrer == null)
      {
        // wenn der Kurs keinen Lehrer hat, handelt es sich vermutlich um eine Note aus der 11ten Klasse
        return getSchnitt(Halbjahr.Zweites).Abschlusszeugnis;
      }

      if (z == Zeitpunkt.ProbezeitBOS || z == Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        return getSchnitt(Halbjahr.Erstes).JahresfortgangGanzzahlig;
      }
      else if (z == Zeitpunkt.ErstePA || z == Zeitpunkt.Jahresende)
      {
        return getSchnitt(Halbjahr.Zweites).JahresfortgangGanzzahlig;
      }
      else // 2./3.PA
      {
        return getSchnitt(Halbjahr.Zweites).Abschlusszeugnis;
      }
    }

        public int getNotenanzahl(Halbjahr hj, Notentyp typ)
        {
            return noten[(int)hj, (int)typ].Count;
        }

        public int getNotenanzahl(Notentyp typ)
        {
            return noten[(int)Halbjahr.Erstes, (int)typ].Count + noten[(int)Halbjahr.Zweites, (int)typ].Count;
        }

        public Fach getFach
        {
            get
            {
                if (fach == null)
                {
                    Kurs k = Zugriff.Instance.KursRep.Find(kursId);
                    fach = Zugriff.Instance.FachRep.Find(k.Data.FachId);
                }
                return fach;
            }
        }

    private string NotenString(IList<int>noten, string bez="")
    {
      if (noten == null) return ""; // tritt bei Fach ohne Kurs auf (z.B. Fpa)
      string s="";
      if (noten != null)
      {
        foreach (var note in noten)
        {
          s += note + bez + "  ";
        }
      }
      return s;
    }

    /// <summary>
    /// Liefert alle SA eines Faches als Text
    /// </summary>
    public string SA(Halbjahr hj)
    {
      return (NotenString(getNoten(hj, Notentyp.Schulaufgabe), "")).TrimEnd();
    }

    public string ToString(Halbjahr hj, Notentyp typ)
    {
      return (NotenString(getNoten(hj, typ), "")).TrimEnd();
    }

    /// <summary>
    /// Liefert alle sonstige Leistungen eines Faches als Text
    /// </summary>
    public string sL(Halbjahr hj)
    {
      string s;
      s=NotenString(getNoten(hj, Notentyp.Kurzarbeit), "K");
      s+=NotenString(getNoten(hj, Notentyp.Ex), "");
      s+=NotenString(getNoten(hj, Notentyp.EchteMuendliche), "");
      s+=NotenString(getNoten(hj, Notentyp.Fachreferat), "F"); // TODO: bald eine Hj-Leistung!
      s+=NotenString(getNoten(hj, Notentyp.Ersatzprüfung), "E"); 
      return s.TrimEnd();
    }

    /// <summary>
    /// Liefert eine druckbare Liste für alle sonstigen Leistungen
    /// </summary>
    public IList<string> sonstigeLeistungen(Halbjahr hj)
    {
      IList<string> liste = new List<string>();
      InsertNoten(liste, getNoten(hj, Notentyp.Kurzarbeit), "K");
      InsertNoten(liste, getNoten(hj, Notentyp.Ex), "");
      InsertNoten(liste, getNoten(hj, Notentyp.EchteMuendliche), "");
      InsertNoten(liste, getNoten(hj, Notentyp.Fachreferat), "F");
      InsertNoten(liste, getNoten(hj, Notentyp.Ersatzprüfung), "E"); 
      return liste;           
    }

    private void InsertNoten(IList<string> liste, IList<int>noten, string bez="")
    {
      foreach (var note in noten)
      {
        liste.Add(note + (bez=="" ? "" : /*" " + */ bez));                
      }        
    }

    public string NotwendigeNoteInMAP(double Zielpunkte)
    {
      if (getFach.Kuerzel == "E" || !getFach.IstSAPFach(schueler.Zweig)) return ""; // in Nebenfächern gibt es keine MAP
      if (getHjLeistung(HjArt.GesErg) == null) return "";
      int zeugnis = getHjLeistung(HjArt.GesErg).Punkte;
      if (zeugnis >= Zielpunkte) return ""; // Ziel schon erreicht
      
      Fachsumme fs = SummeHalbjahre();
      int faktor = schueler.APFaktor;
      int apg = (int)Math.Ceiling((Zielpunkte*(fs.anz+faktor)-fs.sum)/(double)faktor); // diese Note müsste im Abigesamt stehen
      var sapL = getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich);
      int sap = sapL.Count == 0 ? 0 : sapL[0]; // sollte nur im Test passieren.

      sap = getHjLeistung(HjArt.AP).Punkte; // raus: nur zum Test!

      int map = 3 * apg - 2 * sap - 1;

      if (map>15) return "nicht möglich";
      else return map.ToString();
    }

    public string NotwendigeNoteInMAPalt(int Zielpunkte)
    {
      int? zeugnis = getSchnitt(Halbjahr.Zweites).Abschlusszeugnis;
      if (!zeugnis.HasValue || zeugnis >= Zielpunkte) return ""; // Ziel schon erreicht

      string kuerzel = getFach.Kuerzel;
      decimal? jf = getSchnitt(Halbjahr.Zweites).JahresfortgangMitKomma;
      if (!jf.HasValue || kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Sw" || kuerzel == "Sm" || kuerzel == "Ku") return ""; // in Nebenfächern wie G,TZ,... gibt es keine MAP

      int map;
      if (!getFach.IstSAPFach(schueler.Zweig)) // Nebenfächer
      {
        map = (int)Math.Ceiling((decimal)(6.99) - jf.GetValueOrDefault());
        return map.ToString();
      }

      // Prüfungsfächer
      if (getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich).Count == 0) return "";
      int sap = getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich)[0];
      map = sap + 1; // Mündliche muss größer als schriftliche sein, sonst bringt es nichts.
      while (map <= 15 && Notentools.BerechneZeugnisnote(jf, sap, map) < Zielpunkte)
        map++;

      if (map > 15) return "nicht möglich";
      else return map.ToString();
    }
  }
  
  // Verwaltet die Gesamtergebnis-Varianten, um zu überprüfen, ob ein Schüler noch in die MAP darf
  public class GEVariante
  {
    public int AP,GE,PunkteDiff,ProblemDiff;
    
    public void CalcDiff(int APalt, int GEalt, int faktor)
    {
      PunkteDiff = (AP - APalt) * faktor;
      ProblemDiff = ProblemLevel(GE) - ProblemLevel(GEalt);
    }

    private int ProblemLevel(int ge)
    {
      if (ge == 0) return 2;
      else if (ge < 4) return 1;
      else return 0;
    }
  }
}
