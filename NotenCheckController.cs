﻿using System;
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
        private NotenCheckResults res = new NotenCheckResults();
        private IList<INotenCheck> alleNotenchecks = new List<INotenCheck>();
        private Zeitpunkt zeitpunkt;

        public NotenCheckController(Zeitpunkt azeitpunkt)
        {
            zeitpunkt = azeitpunkt;
            alleNotenchecks.Add(new FachreferatChecker());
            alleNotenchecks.Add(new NotenanzahlChecker());
            alleNotenchecks.Add(new UnterpunktungChecker());

            /*
            int[] klassenIDs = new[] { 15, 20, 34 };
            foreach (int klassenId in klassenIDs)
            {

            }
            */
        }

        public NotenCheckResults CheckAll()
        {
            var klassen = KlassenController.AlleKlassen();
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
                CheckSchueler(new Schueler(schueler),notwendigeNotenchecks);
            }
            return res;
        }

        public NotenCheckResults CheckSchueler(Schueler s, IList<INotenCheck> notwendigeNotenchecks)
        {            
            foreach (var ch in notwendigeNotenchecks)
            {
                ch.Check(s, zeitpunkt,res);
            }

            return res;
        }

        /// <summary>
        /// Liefert die Prüfungsergebnisse in druckbarer Form
        /// </summary>
        public List<string> PrintResults()
        {
            List<string> s = new List<string>();
            foreach (var r in res.list)
            {
                s.Add(r.schueler.getKlasse.Data.Bezeichnung + ", " + r.schueler.NameVorname + ", " +
                    (r.kurs!=null ? r.kurs.getFach.Kuerzel + " (" + r.kurs.getLehrer.Kuerzel +"): " : "")  + r.meldung);
            }
            return s;
        }
    }

    /// <summary>
    /// Verwaltet die Notenprüfung für einen Schüler
    /// </summary>
    public class NotenCheckSchueler
    {
        public NotenCheckSchueler(Schueler s, ref NotenCheckResults res)
        {

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
        public Schueler schueler  { get; set; } // betroffener Schüler
        public Kurs kurs { get; set; } // Referenz zum betroffenen Kurs (und damit Lehrer)
        public string meldung { get; set; }

        public NotenCheckResult(Schueler s,Kurs k,string m)
        {
            schueler = s;
            kurs = k;
            meldung = m;
        }
    }
}
