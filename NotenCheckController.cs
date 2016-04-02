using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
    /// <summary>
    /// Verwaltet die Notenprüfung
    /// </summary>
    public class NotenCheckController
    {
        public NotenCheckResults res = new NotenCheckResults();
        private IList<NotenCheck> alleNotenchecks = new List<NotenCheck>();
        public Zeitpunkt zeitpunkt;
        public NotenCheckModus modus;        
        private List<KeyValuePair<string,NotenCheckContainer>> chkContainer;
        private Dictionary<string,NotenCheckCounter> chkCounter;
        private int aktKlassenId=0;
        private Schueler aktSchueler;

        public NotenCheckController(Zeitpunkt azeitpunkt, NotenCheckModus amodus)
        {
            zeitpunkt = azeitpunkt;
            modus = amodus; 
            if (modus!=NotenCheckModus.BerechnungenSpeichern && modus != NotenCheckModus.VorkommnisseErzeugen)           
              alleNotenchecks.Add(new NotenanzahlChecker(this));
            if (modus!=NotenCheckModus.EigeneNotenVollstaendigkeit)
              alleNotenchecks.Add(new UnterpunktungChecker(this));            
            if (azeitpunkt == Zeitpunkt.ErstePA && (modus==NotenCheckModus.Gesamtpruefung || modus == NotenCheckModus.EigeneKlasse)) // nur dort FR prüfen
              alleNotenchecks.Add(new FachreferatChecker(this));            
            if (azeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS || azeitpunkt == Zeitpunkt.Jahresende)
              alleNotenchecks.Add(new FpABestandenChecker(this));
        }


        public void CheckSchueler(Schueler s)
        { 
          if (modus==NotenCheckModus.EigeneKlasse && s.getKlasse.Klassenleiter.Id != Zugriff.Instance.lehrer.Id)           
            return;

          Klasse klasse;
          klasse = s.getKlasse;          
    
          // muss dieser Schüler überhaupt geprüft werden?
              // S ohne Probezeit oder späterer Probezeit                             
          if (zeitpunkt == Zeitpunkt.ProbezeitBOS && s.HatProbezeitBis()==Zeitpunkt.ProbezeitBOS ||
              // fast alle zum Halbjahr
              zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS && klasse.Jahrgangsstufe != Jahrgangsstufe.Vorkurs ||
              // Jahresende nur für Vorklasse und 11. 
              zeitpunkt == Zeitpunkt.Jahresende && klasse.Jahrgangsstufe <= Jahrgangsstufe.Elf ||
              // 1.-3. PA nur für 12./13.
              klasse.Jahrgangsstufe >= Jahrgangsstufe.Zwoelf &&
                (zeitpunkt == Zeitpunkt.ErstePA 
                || zeitpunkt == Zeitpunkt.ZweitePA /* && zugelassen zur SAP  s.Vorkommnisse.Contains(Vorkommnisart.NichtZurPruefungZugelassen)*/
                || zeitpunkt == Zeitpunkt.DrittePA /* && zugelassen zur SAP  */ )
              )
          {
            // je Klasse wird die akkumlierte Liste neu erstellt
            if (klasse.Data.Id != aktKlassenId)
            {
              CreateResults();
              aktKlassenId = klasse.Data.Id;
              chkContainer = new List<KeyValuePair<string, NotenCheckContainer>>();
              chkCounter = new Dictionary<string, NotenCheckCounter>();
            }
                
            aktSchueler = s; // erst hier: für CreateResults muss noch der alte drinstehen
            foreach (var ch in alleNotenchecks)
            {
                if (ch.CheckIsNecessary(klasse.Jahrgangsstufe, klasse.Schulart))
                    ch.Check(s);
            }
          }
    }   
    
    public void ErzeugeZeugnisVorkommnis()
    {
      if (modus!=NotenCheckModus.VorkommnisseErzeugen) return;

      if (zeitpunkt==Zeitpunkt.HalbjahrUndProbezeitFOS)
        aktSchueler.AddVorkommnis(Vorkommnisart.Zwischenzeugnis, "");

      else if (zeitpunkt==Zeitpunkt.DrittePA)
      {
        if (aktSchueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          aktSchueler.AddVorkommnis(Vorkommnisart.Fachabiturzeugnis,"");
        else if (aktSchueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
        {
          var f = aktSchueler.getNoten.FindeFach("F",false);
          if (aktSchueler.Wahlpflichtfach=="F3" || // fortgef. F
            f != null && f.getSchnitt(Halbjahr.Zweites).Abschlusszeugnis > 3) // TODO: es fehlen S die Franz. in der RS hatten
            aktSchueler.AddVorkommnis(Vorkommnisart.allgemeineHochschulreife,"");
          else 
            aktSchueler.AddVorkommnis(Vorkommnisart.fachgebundeneHochschulreife,"");          
        }
      }
      else if (zeitpunkt==Zeitpunkt.Jahresende && aktSchueler.getKlasse.Jahrgangsstufe < Jahrgangsstufe.Zwoelf)
        aktSchueler.AddVorkommnis(Vorkommnisart.Jahreszeugnis,"");
    }

    public void Add(Vorkommnisart nr, string meldung)
    {
      if (modus==NotenCheckModus.VorkommnisseErzeugen)
      {
        aktSchueler.AddVorkommnis(nr,meldung);
      }
      else
      {
        Add(null, Vorkommnisse.Instance.VorkommnisText(nr) + " " + meldung);
      }
    }

    public void Add(Kurs k,string m)
    {
      NotenCheckCounter c;      
      if (k!=null)
      {
        chkContainer.Add(new KeyValuePair<string, NotenCheckContainer>(k.Id + "_" +m,new NotenCheckContainer(k,aktSchueler,m)));
        if (chkCounter.TryGetValue(k.Id + "_" +m,out c))
        {
          c.count++; // so eine ähnliche Meldung gab es schon mal
        }
        else
        {
          chkCounter.Add(k.Id + "_" +m,new NotenCheckCounter(k,m));
        }
      }
      else
      {
        chkContainer.Add(new KeyValuePair<string, NotenCheckContainer>("",new NotenCheckContainer(k,aktSchueler,m)));
      }
    }

    // am Ende einer Klasse muss die Druckliste erneuert werden
    public void CreateResults()
    {      
      if (aktKlassenId==0) return; // noch keine Klasse erzeugt
      NotenCheckCounter cnt;
      int maxAnzahl = 5; // ab dieser Zahl wird kumuliert

      // kumulierte Meldungen für viele Schüler
      foreach (var c in chkCounter)
      {
        if (c.Value.count>maxAnzahl) 
           res.list.Add(new NotenCheckResult(aktSchueler.getKlasse,c.Value.kurs,c.Value.meldung + " ("+c.Value.count+"x)"));
      }

      // einzelne Meldungen
      foreach (var r in chkContainer)
      {
        if (!chkCounter.TryGetValue(r.Key, out cnt) || cnt.count<=maxAnzahl)
        {         
            res.list.Add(new NotenCheckResult(r.Value.schueler,r.Value.kurs,r.Value.meldung));
        }
      }
    }

  }

   
    /// <summary>
    /// Verwaltet die Fehlermeldungen
    /// </summary>
    public class NotenCheckResults
    {
        public IList<NotenCheckResult> list;
        public NotenCheckResults()
        {
            list = new List<NotenCheckResult>();
        }
    }

    /// <summary>
    /// Verwaltet eine Fehlermeldung für einen Schüler
    /// </summary>
    public class NotenCheckResult
    {
        public string schueler  { get; private set; }
        public string klasse { get; private set; } 
        public string lehrer { get; private set; } 
        public string fach { get; private set; } 
        public string meldung { get; set; }

        public NotenCheckResult(Schueler s,Kurs k,string m)
        {
            schueler = s.NameVorname;
            klasse = s.getKlasse.Data.Bezeichnung;
            lehrer = k!=null && k.getLehrer != null ? k.getLehrer.Kuerzel : "";
            fach =   k!=null ? k.getFach.Kuerzel : "";
            meldung = m;
        }

        public NotenCheckResult(Klasse kl,Kurs k,string m)
        {
            schueler = "...mehrmals...";
            klasse = kl.Data.Bezeichnung;
            lehrer = k!=null && k.getLehrer != null ? k.getLehrer.Kuerzel : "";
            fach =   k!=null ? k.getFach.Kuerzel : "";
            meldung = m;
        }

        public override string ToString()
        {
             return klasse + ", " + schueler + ", " +
                    (lehrer=="" ? fach + " (" + lehrer + "): " : "")  + meldung;            
        }
    }

    // nimmt eine Fehlermeldung zur Zwischenspeicherung auf
    public class NotenCheckContainer
    {     
      public Kurs kurs;
      public Schueler schueler;
      public string meldung;

      public NotenCheckContainer(Kurs k,Schueler s, string m)
      {
        kurs = k;
        schueler = s;
        meldung = m;
      }

    }

    // verwaltet die Anzahl ähnlicher Fehlermeldungen je Klasse
    public class NotenCheckCounter
    {      
      public int count;
      public Kurs kurs;     
      public string meldung;

      public NotenCheckCounter(Kurs k, string m)
      {
        count =1;
        kurs = k;        
        meldung = m;
      }
    }

    public enum NotenCheckModus
    {
      EigeneNotenVollstaendigkeit,
      EigeneKlasse, // nur für Klassenleiter
      Gesamtpruefung,
      VorkommnisseErzeugen, // nur Admin
      BerechnungenSpeichern // nur Admin
     }
  }
