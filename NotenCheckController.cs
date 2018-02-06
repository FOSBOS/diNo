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
        public List<Klasse> zuPruefendeKlassen = new List<Klasse>();
        public int AnzahlSchueler=0;
        private List<KeyValuePair<string,NotenCheckContainer>> chkContainer;
        private Dictionary<string,NotenCheckCounter> chkCounter;
        private int aktKlassenId=0;
        private Schueler aktSchueler,vorigerSchueler;
        private bool UnterpunktungGedruckt;
        
        public NotenCheckController(Zeitpunkt azeitpunkt, NotenCheckModus amodus)
        {
            zeitpunkt = azeitpunkt;            
            modus = amodus; 
            Zugriff.Instance.markierteSchueler.Clear();

      // je nach Modus und Zeitpunkt werden nur bestimmte Klassen ausgewählt
      if (modus == NotenCheckModus.EigeneKlasse)
            {
              KlasseInNotenpruefungAufnehmen(Zugriff.Instance.eigeneKlasse);
            }
            else if (zeitpunkt==Zeitpunkt.ProbezeitBOS || zeitpunkt==Zeitpunkt.HalbjahrUndProbezeitFOS)
            {
              zuPruefendeKlassen = Zugriff.Instance.Klassen;
              AnzahlSchueler = Zugriff.Instance.AnzahlSchueler;
            }
            else
            {
              foreach (var k in Zugriff.Instance.Klassen)
                KlasseInNotenpruefungAufnehmen(k);
            }

            if (modus!=NotenCheckModus.BerechnungenSpeichern && modus != NotenCheckModus.VorkommnisseErzeugen
              && azeitpunkt !=Zeitpunkt.DrittePA)           
              alleNotenchecks.Add(new NotenanzahlChecker(this));
            if (modus!=NotenCheckModus.EigeneNotenVollstaendigkeit)
              alleNotenchecks.Add(new UnterpunktungChecker(this));            
            if (azeitpunkt == Zeitpunkt.ErstePA && (modus==NotenCheckModus.Gesamtpruefung || modus == NotenCheckModus.EigeneKlasse)) // nur dort FR prüfen
            {
              alleNotenchecks.Add(new FachreferatChecker(this));
              alleNotenchecks.Add(new SeminarfachChecker(this));
            }
            if ((azeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS || azeitpunkt == Zeitpunkt.Jahresende)
              && (modus == NotenCheckModus.Gesamtpruefung || modus == NotenCheckModus.EigeneKlasse))
              alleNotenchecks.Add(new FpABestandenChecker(this));
            if ((azeitpunkt == Zeitpunkt.ZweitePA || azeitpunkt == Zeitpunkt.DrittePA)  && 
              (modus==NotenCheckModus.Gesamtpruefung || modus == NotenCheckModus.EigeneKlasse || modus == NotenCheckModus.VorkommnisseErzeugen))
            {
              alleNotenchecks.Add(new AbiergebnisChecker(this));
              alleNotenchecks.Add(new EliteChecker(this));
            } 
            if (azeitpunkt == Zeitpunkt.DrittePA  && (modus==NotenCheckModus.Gesamtpruefung || modus == NotenCheckModus.EigeneKlasse))
              alleNotenchecks.Add(new MAPChecker(this));

            // Folgende Vorkommnisse ggf. löschen, bzw. neu erzeugen bei 2./3.PA
            if ((azeitpunkt == Zeitpunkt.ZweitePA || azeitpunkt == Zeitpunkt.DrittePA)  && (modus==NotenCheckModus.VorkommnisseErzeugen))
            {
              VorkommnisTableAdapter ta = new VorkommnisTableAdapter();
              ta.DeleteVorkommnis((int)Vorkommnisart.bisherNichtBestandenMAPmoeglich);
              ta.DeleteVorkommnis((int)Vorkommnisart.PruefungNichtBestanden);
            }
        }
      
        // Klassenweise Vorauswahl, damit weniger Schüler einzeln erst analysiert werden müssen.
        private void KlasseInNotenpruefungAufnehmen(Klasse k)
        {
            if (zeitpunkt==Zeitpunkt.HalbjahrUndProbezeitFOS || 
                zeitpunkt==Zeitpunkt.ProbezeitBOS && k.Jahrgangsstufe == Jahrgangsstufe.Vorklasse ||
                k.Jahrgangsstufe>=Jahrgangsstufe.Zwoelf && zeitpunkt<=Zeitpunkt.DrittePA || 
                k.Jahrgangsstufe<Jahrgangsstufe.Zwoelf && zeitpunkt==Zeitpunkt.Jahresende)
            {
              zuPruefendeKlassen.Add(k);
              AnzahlSchueler += k.eigeneSchueler.Count;
            }  
        }

        public void CheckSchueler(Schueler s)
        { 
          aktSchueler = s;
          if ((modus==NotenCheckModus.EigeneKlasse && s.getKlasse.KlassenleiterId != Zugriff.Instance.lehrer.Id) ||
               s.Status==Schuelerstatus.Abgemeldet) return;
              
          // auch abgebrochene bekommen ein Jahreszeugnis
          if (modus!=NotenCheckModus.EigeneNotenVollstaendigkeit && (zeitpunkt >= Zeitpunkt.DrittePA || zeitpunkt==Zeitpunkt.HalbjahrUndProbezeitFOS))
            ZeugnisVorkommnisAnlegen(s);

          //if (s.Status == Schuelerstatus.SAPabgebrochen && zeitpunkt == Zeitpunkt.ZweitePA)
          //  Add(null, "Prüfung abgebrochen"); // TODO: dazu Vorkommnis anlegen
          
           if ((s.Status == Schuelerstatus.SAPabgebrochen|| s.Status==Schuelerstatus.NichtZurSAPZugelassen) && zeitpunkt > Zeitpunkt.ErstePA ) // nicht zugelassene raus
            return;

          Klasse klasse;
          klasse = s.getKlasse;          
          UnterpunktungGedruckt=false;
        
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
                || zeitpunkt == Zeitpunkt.ZweitePA
                || zeitpunkt == Zeitpunkt.DrittePA)
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
             
               
            foreach (var ch in alleNotenchecks)
            {
                if (ch.CheckIsNecessary(klasse.Jahrgangsstufe, klasse.Schulart))
                {
                    ch.Check(s);
                }
            }
            // Kontrollmöglichkeit: alle weiteren Unterpunktungen werden gedruckt
            if (s.getNoten.Unterpunktungen!="" && !UnterpunktungGedruckt && zeitpunkt!=Zeitpunkt.HalbjahrUndProbezeitFOS
              && (modus==NotenCheckModus.Gesamtpruefung || modus==NotenCheckModus.EigeneKlasse))
              Add(null, "Unterpunktet in " + s.getNoten.Unterpunktungen 
                + (zeitpunkt != Zeitpunkt.ProbezeitBOS && s.AlteFOBOSO() ? "" : " Schnitt: " + String.Format("{0:0.00}", s.getNoten.Punkteschnitt)));

            vorigerSchueler = s; // CreateResults braucht noch den
          }
    }   
    
    public void ZeugnisVorkommnisAnlegen(Schueler s)
    {
      Vorkommnisart v = s.Zeugnisart(zeitpunkt);
      if (v==Vorkommnisart.NotSet) return;

      if (modus==NotenCheckModus.VorkommnisseErzeugen)        
      {
        s.AddVorkommnis(v,Zugriff.Instance.Zeugnisdatum,""); // Zeugnis als Vorkommnis anlegen
        if (s.getNoten.ErhaeltMittlereReife() && zeitpunkt == Zeitpunkt.Jahresende)
          s.AddVorkommnis(Vorkommnisart.MittlereReife, Zugriff.Instance.Zeugnisdatum, "");
      }
      else
      {
        if (v==Vorkommnisart.allgemeineHochschulreife)
          Add(v,""); // zusätzliche Ausgabe für die Meldungsliste        
      }
    }

    // fügt eine Meldung/Vorkommnis hinzu, und erzeugt ggf. abhängige Vorkommnisse
    public void Add(Vorkommnisart art, string meldung,bool aUnterpunktungGedruckt=false)
    {     
      if (aUnterpunktungGedruckt) UnterpunktungGedruckt=aUnterpunktungGedruckt;
      AddVorkommnis(art, meldung);
     
      // bei Wiederholungsschülern wird bei bestimmten Ereignissen automatisch Gefahr d. Abw. oder d.n.w erzeugt
      if (aktSchueler.Wiederholt())
      {
        if (art==Vorkommnisart.NichtBestanden || art==Vorkommnisart.nichtBestandenMAPnichtZugelassen ||
          art==Vorkommnisart.NichtZurPruefungZugelassen || art==Vorkommnisart.KeineVorrueckungserlaubnis)           
            AddVorkommnis(Vorkommnisart.DarfNichtMehrWiederholen,"");

        if (art==Vorkommnisart.Gefaehrdungsmitteilung || art==Vorkommnisart.starkeGefaehrdungsmitteilung || art==Vorkommnisart.BeiWeiteremAbsinken)           
            AddVorkommnis(Vorkommnisart.GefahrDerAbweisung,"");
      }
    }

    private void AddVorkommnis(Vorkommnisart art, string meldung)
    {
      if (modus==NotenCheckModus.VorkommnisseErzeugen)
      {
        aktSchueler.AddVorkommnis(art,meldung);
      }
      else
      {
        Add(null, Vorkommnisse.Instance.VorkommnisText(art) + " " + meldung);
      }
    }

    public void Add(Kurs k,string m,bool aUnterpunktungGedruckt=false)
    {
      if (aUnterpunktungGedruckt) UnterpunktungGedruckt=aUnterpunktungGedruckt;
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

      if (!Zugriff.Instance.markierteSchueler.ContainsKey(aktSchueler.Id))
        Zugriff.Instance.markierteSchueler.Add(aktSchueler.Id,aktSchueler);
    }

    // am Ende einer Klasse muss die Druckliste erneuert werden, das merkt man erst, wenn einer neuer Schüler aus einer anderen Klasse kommt
    public void CreateResults()
    {      
      if (aktKlassenId==0) return; // noch keine Klasse erzeugt
      NotenCheckCounter cnt;
      int maxAnzahl = 5; // ab dieser Zahl wird kumuliert

      // kumulierte Meldungen für viele Schüler
      foreach (var c in chkCounter)
      {
        if (c.Value.count>maxAnzahl) 
           res.list.Add(new NotenCheckResult(vorigerSchueler.getKlasse,c.Value.kurs,c.Value.meldung + " ("+c.Value.count+"x)"));
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
