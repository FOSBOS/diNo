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
        public bool nurEigeneNoten;
        public bool erzeugeVorkommnisse;

        public NotenCheckController(Zeitpunkt azeitpunkt, bool aNurEigeneNoten, bool aerzeugeVorkommnisse)
        {
            zeitpunkt = azeitpunkt;
            nurEigeneNoten = aNurEigeneNoten;            
            erzeugeVorkommnisse = aerzeugeVorkommnisse;
            alleNotenchecks.Add(new NotenanzahlChecker(this));
            alleNotenchecks.Add(new UnterpunktungChecker(this));            
            if (azeitpunkt == Zeitpunkt.ErstePA) // nur dort FR prüfen
                alleNotenchecks.Add(new FachreferatChecker(this));
            // TODO: erst, wenn FPA eingebunden
            //if (azeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS || azeitpunkt == Zeitpunkt.Jahresende)
            //  alleNotenchecks.Add(new FpABestandenChecker(this));
        }


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
                
            foreach (var ch in alleNotenchecks)
            {
                if (ch.CheckIsNecessary(klasse.Jahrgangsstufe, klasse.Schulart))
                    ch.Check(s);
            }
        }
        /*
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
        */
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
