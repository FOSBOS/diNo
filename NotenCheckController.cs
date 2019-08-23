using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace diNo
{
  /// <summary>
  /// Verwaltet die Notenprüfung
  /// </summary>
  public class NotenCheckController
  {
    public NotenCheckResults res = new NotenCheckResults();
    private IList<NotenCheck> alleNotenchecks = new List<NotenCheck>();
    public delegate void Aufgabe(Schueler s);
    public List<Aufgabe> aufgaben; // speichert alle zu erledigenden Berechnungsaufgaben + Notenchecks eines Schülers
    public Zeitpunkt zeitpunkt;
    public NotenCheckModus modus;
    public List<Klasse> zuPruefendeKlassen = new List<Klasse>();
    public int AnzahlSchueler = 0;
    private List<KeyValuePair<string, NotenCheckContainer>> chkContainer;
    private Dictionary<string, NotenCheckCounter> chkCounter;

    private Schueler aktSchueler;
    private bool UnterpunktungGedruckt;
    private ProgressBar progressBar;
    private Berechnungen berechnungen = null;
    public int FehlendeBerechnung = 0; // gibt einen Überblick, ob Gesamtergebnisse schon bestimmt sind.

    public NotenCheckController(Zeitpunkt azeitpunkt, NotenCheckModus amodus, ProgressBar aprogressBar)
    {
      zeitpunkt = azeitpunkt;
      modus = amodus;
      progressBar = aprogressBar;
      Zugriff.Instance.markierteSchueler.Clear();

      // je nach Modus und Zeitpunkt werden nur bestimmte Klassen ausgewählt
      if (modus == NotenCheckModus.EigeneKlasse)
      {
        KlasseInNotenpruefungAufnehmen(Zugriff.Instance.eigeneKlasse);
      }
      else if (zeitpunkt == Zeitpunkt.ProbezeitBOS || zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        zuPruefendeKlassen = Zugriff.Instance.Klassen;
        AnzahlSchueler = Zugriff.Instance.AnzahlSchueler;
      }
      else
      {
        foreach (var k in Zugriff.Instance.Klassen)
          KlasseInNotenpruefungAufnehmen(k);
      }

      // Nötige Berechnungen, bevor geprüft wird
      if (modus == NotenCheckModus.KonferenzVorbereiten)
      {
        berechnungen = new Berechnungen(azeitpunkt);
      }

      // Durchzuführende Prüfungen
      if (azeitpunkt != Zeitpunkt.DrittePA)
        alleNotenchecks.Add(new NotenanzahlChecker(this));
      if (azeitpunkt == Zeitpunkt.ErstePA && modus != NotenCheckModus.EigeneNotenVollstaendigkeit) // nur dort FR prüfen
      {
        alleNotenchecks.Add(new FachreferatChecker(this));
        alleNotenchecks.Add(new SeminarfachChecker(this));
      }
      if ((azeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS || azeitpunkt == Zeitpunkt.Jahresende)
        && modus != NotenCheckModus.EigeneNotenVollstaendigkeit)
        alleNotenchecks.Add(new FpABestandenChecker(this));
      if ((azeitpunkt == Zeitpunkt.ZweitePA || azeitpunkt == Zeitpunkt.DrittePA) && modus != NotenCheckModus.EigeneNotenVollstaendigkeit)
      {
        //alleNotenchecks.Add(new AbiergebnisChecker(this));
        alleNotenchecks.Add(new EliteChecker(this));
      }
      if ((azeitpunkt == Zeitpunkt.ErstePA || azeitpunkt == Zeitpunkt.ZweitePA || azeitpunkt == Zeitpunkt.DrittePA) && modus != NotenCheckModus.EigeneNotenVollstaendigkeit)
      {
        alleNotenchecks.Add(new EinbringungsChecker(this));
      }
      if (modus != NotenCheckModus.EigeneNotenVollstaendigkeit)
        alleNotenchecks.Add(new UnterpunktungChecker(this));
      if (azeitpunkt == Zeitpunkt.DrittePA && modus != NotenCheckModus.EigeneNotenVollstaendigkeit)
        alleNotenchecks.Add(new MAPChecker(this));

      // Folgende Vorkommnisse ggf. löschen, bzw. neu erzeugen bei 2./3.PA
      if ((azeitpunkt == Zeitpunkt.ZweitePA || azeitpunkt == Zeitpunkt.DrittePA) && modus == NotenCheckModus.KonferenzVorbereiten)
      {
        VorkommnisTableAdapter ta = new VorkommnisTableAdapter();
        ta.DeleteVorkommnis((int)Vorkommnisart.bisherNichtBestandenMAPmoeglich);
        ta.DeleteVorkommnis((int)Vorkommnisart.PruefungNichtBestanden);
      }

      if ((zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS || zeitpunkt >= Zeitpunkt.DrittePA) && modus == NotenCheckModus.KonferenzVorbereiten)
        alleNotenchecks.Add(new ZeugnisVorkommnisAnlegen(this));
    }

    // Klassenweise Vorauswahl, damit weniger Schüler einzeln erst analysiert werden müssen.
    private void KlasseInNotenpruefungAufnehmen(Klasse k)
    {
      if (zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS ||
        zeitpunkt == Zeitpunkt.ProbezeitBOS ||
        k.Jahrgangsstufe >= Jahrgangsstufe.Zwoelf && zeitpunkt <= Zeitpunkt.DrittePA ||
        k.Jahrgangsstufe < Jahrgangsstufe.Zwoelf && zeitpunkt == Zeitpunkt.Jahresende)
      {
        zuPruefendeKlassen.Add(k);
        AnzahlSchueler += k.eigeneSchueler.Count;
      }
    }

    public void CheckKlasse(Klasse k)
    {
      // je Klasse wird die akkumulierte Liste neu gefüllt
      chkContainer = new List<KeyValuePair<string, NotenCheckContainer>>();
      chkCounter = new Dictionary<string, NotenCheckCounter>();
      foreach (Schueler s in k.eigeneSchueler)
      {
        CheckSchueler(s);
        progressBar.Increment(1);
      }

      /* Parallele Threads wären schick, dazu müssten aber die Container klassenweise getrennt instanziiert werden!
      Parallel.ForEach(k.eigeneSchueler, s =>
      {
        CheckSchueler(s);
      }
       );
       */
      CreateResults();
    }
  
    public void CheckSchueler(Schueler s)
    {  
      aktSchueler = s;
      if (s.Status==Schuelerstatus.Abgemeldet) return;
              
      Klasse klasse = s.getKlasse;          
      UnterpunktungGedruckt=false;
        
      // muss dieser Schüler überhaupt geprüft werden?
          // S ohne Probezeit oder späterer Probezeit                             
      if (zeitpunkt == Zeitpunkt.ProbezeitBOS && s.HatProbezeitBis()==Zeitpunkt.ProbezeitBOS ||
          // fast alle zum Halbjahr
          zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS /*&& klasse.Jahrgangsstufe != Jahrgangsstufe.Vorkurs */||
          // Jahresende nur für Vorklasse und 11. 
          zeitpunkt == Zeitpunkt.Jahresende && klasse.Jahrgangsstufe <= Jahrgangsstufe.Elf ||
          // 1.-3. PA nur für 12./13.
          klasse.Jahrgangsstufe >= Jahrgangsstufe.Zwoelf &&
            (zeitpunkt == Zeitpunkt.ErstePA 
            || zeitpunkt == Zeitpunkt.ZweitePA
            || zeitpunkt == Zeitpunkt.DrittePA)
          )
      {
        // Erst werden alle Berechnungen durchgeführt (Gesamtergebnisse, DNote,...)
        if (berechnungen!=null)
          berechnungen.BerechneSchueler(s);

        // In memoriam: Schüler, die draußen sind
        if (zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA)
        {
          bool weg = false;
          if (s.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen))
          {
            weg = true;
            Add(null, "War nicht zur Prüfung zugelassen.");            
          }
          if (s.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
          {
            weg = true;
            Add(null, "Hat die Prüfung abgebrochen.");
          }
          if (weg)
          { 
            if (modus == NotenCheckModus.KonferenzVorbereiten && zeitpunkt == Zeitpunkt.DrittePA && !s.hatVorkommnis(Vorkommnisart.keinJahreszeugnis))
              s.AddVorkommnis(Vorkommnisart.Jahreszeugnis, Zugriff.Instance.Zeugnisdatum, "");
            return;
          }          
        }

        foreach (var ch in alleNotenchecks)
        {
          if (ch.CheckIsNecessary(klasse.Jahrgangsstufe, klasse.Schulart))
          {
            ch.Check(s);
          }
        }

        // Kontrollmöglichkeit: alle weiteren Unterpunktungen werden gedruckt
        if (s.getNoten.Unterpunktungen != "" && !UnterpunktungGedruckt && zeitpunkt != Zeitpunkt.HalbjahrUndProbezeitFOS
          && modus != NotenCheckModus.EigeneNotenVollstaendigkeit)
        {
          Add(null, "Unterpunktet in " + s.getNoten.Unterpunktungen);
        }        
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
      if (modus==NotenCheckModus.KonferenzVorbereiten && zeitpunkt>Zeitpunkt.ProbezeitBOS)
      {
        aktSchueler.AddVorkommnis(art,meldung);
      }

      Add(null, Vorkommnisse.Instance.VorkommnisText(art) + " " + meldung);
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
    
    public void CreateResults()
    {
      NotenCheckCounter cnt;
      int maxAnzahl = 5; // ab dieser Zahl wird kumuliert

      // kumulierte Meldungen für viele Schüler
      foreach (var c in chkCounter)
      {
        if (c.Value.count > maxAnzahl)
          res.list.Add(new NotenCheckResult(aktSchueler.getKlasse, c.Value.kurs, c.Value.meldung + " (" + c.Value.count + "x)"));
      }

      // einzelne Meldungen
      foreach (var r in chkContainer)
      {
        if (!chkCounter.TryGetValue(r.Key, out cnt) || cnt.count <= maxAnzahl)
        {
          res.list.Add(new NotenCheckResult(r.Value.schueler, r.Value.kurs, r.Value.meldung));
        }
      }
    }

    public void ShowResults()
    {
      if (FehlendeBerechnung > 0)
      {
        string s = (Zugriff.Instance.HatVerwaltungsrechte ? " (" + FehlendeBerechnung + " HjL) " : "");
        MessageBox.Show("Die Gesamtergebnisse " + s + "sind noch nicht bei allen Schülern berechnet.\nEine aussagekräftige Notenprüfung kann erst durchgeführt werden, wenn die Einbringung vorliegt.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      if (res.list.Count == 0)
        MessageBox.Show("Es traten keine Fehler auf.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      else
        new ReportNotencheck(res).Show();
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
      KonferenzVorbereiten  // nur Admin
     }
  }
