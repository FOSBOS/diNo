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
        private IList<INotenCheck> alleNotenchecks = new List<INotenCheck>();
        private Zeitpunkt zeitpunkt;
        private bool nurEigeneNoten;

        public NotenCheckController(Zeitpunkt azeitpunkt, bool aNurEigeneNoten)
        {
            zeitpunkt = azeitpunkt;
            nurEigeneNoten = aNurEigeneNoten;            
            alleNotenchecks.Add(new NotenanzahlChecker());
            alleNotenchecks.Add(new UnterpunktungChecker());            
            if (azeitpunkt == Zeitpunkt.ErstePA) // nur dort FR prüfen
                alleNotenchecks.Add(new FachreferatChecker());
        }

/*
        public NotenCheckResults CheckAll()
        {
            var klassen = KlassenController.AlleKlassen();
            foreach (var klasse in klassen)
            {
                CheckKlasse(klasse);
            }
            return res;
        }

    public NotenCheckResults CheckKlassen(IList<Klasse> klassen)
    {
      IList<INotenCheck> notwendigeNotenchecks = new List<INotenCheck>();
      foreach (var klasse in klassen)
      {
        CheckKlasse(klasse);
      }

      return res;
    }

    public NotenCheckResults CheckKlasse(Klasse klasse)
        {
            IList<INotenCheck> notwendigeNotenchecks = new List<INotenCheck>();
            foreach (var ch in alleNotenchecks)
            {
                if (ch.CheckIsNecessary(klasse.Jahrgangsstufe, klasse.Schulart, zeitpunkt))
                    notwendigeNotenchecks.Add(ch);
            }
            foreach (var schueler in klasse.getSchueler)
            {
                // TODO: Probezeit-check nur für Schüler mit Probezeit. Achtung: teilweise stehen noch alte Datümer in der DB.
                // können wir diesen Wert beim Import gleich löschen, wenn aus dem alten Schuljahr stammend?
                CheckSchueler(new Schueler(schueler),notwendigeNotenchecks);
            }
            return res;
        }
*/
        public void CheckSchueler(Schueler s)
        {            
            Klasse klasse;
            klasse = s.getKlasse;
            
            // muss dieser Schüler überhaupt geprüft werden?

            // S ohne Probezeit oder späterer Probezeit
            var aktJahr = (DateTime.Today).Year;                       
            if (zeitpunkt == Zeitpunkt.ProbezeitBOS && 
                    (s.Data.IsProbezeitBisNull() 
                    || s.Data.ProbezeitBis < DateTime.Parse("15.09." +  aktJahr)
                    || s.Data.ProbezeitBis > DateTime.Parse("15.12." +  aktJahr)))
                return ;
                
            // 11. Klassen

            // Prüfungsklassen

            foreach (var ch in alleNotenchecks)
            {
                if (ch.CheckIsNecessary(klasse.Jahrgangsstufe, klasse.Schulart, zeitpunkt))
                    ch.Check(s, zeitpunkt,res);
            }
        }

        /// <summary>
        /// Liefert die Prüfungsergebnisse in druckbarer Form
        /// </summary>
        public List<string> PrintResults()
        {
            List<string> s = new List<string>();
            foreach (var r in res.list)
            {
                s.Add(r.ToString());
            }
            return s;
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

        public void Add(Schueler s,Kurs k,string m)
        {
            list.Add(new NotenCheckResult(s,k,m));
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
            lehrer = k!=null ? k.getLehrer.Kuerzel : "";
            fach =   k!=null ? k.getFach.Kuerzel : "";
            meldung = m;
        }

        public override string ToString()
        {
             return klasse + ", " + schueler + ", " +
                    (lehrer=="" ? fach + " (" + lehrer + "): " : "")  + meldung;            
        }
    }
}
