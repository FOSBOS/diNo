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
        private diNoDataSet.KursDataTable kurse;
        public IList<FachSchuelerNoten> alleFaecher;

        public SchuelerNoten(Schueler s)
        {
            schueler = s;
            kurse = schueler.Kurse; // ermittle alle Kurse, die der S besucht
            alleFaecher = new List<FachSchuelerNoten>();
            foreach (var kurs in kurse)
            {
                alleFaecher.Add(new FachSchuelerNoten(schueler.Id, kurs.Id));
            }
        }

        public FachSchuelerNoten getFach(int kursid)
        {
            foreach (FachSchuelerNoten f in alleFaecher)
            {
                if (f.kursId == kursid) return f;
            }
            throw new IndexOutOfRangeException("FachSchuelerNoten.getFach: falsche kursid");            
        }
        
        /// <summary>
        /// Liefert eine Liste in der je Fach alle Noten in druckbarer Form vorliegen.
        /// </summary>
        public IList<FachSchuelerNotenDruck> SchuelerNotenDruck()
        {
            IList<FachSchuelerNotenDruck> liste = new List<FachSchuelerNotenDruck>();
            foreach (FachSchuelerNoten f in alleFaecher)
            {
                // für SA-Fächer werden zwei Datensätze erzeugt, immer der sL-Eintrag 
                if (f.getFach.IstSAFach())
                    liste.Add(new FachSchuelerNotenDruck(f, true)); // SA

                liste.Add(new FachSchuelerNotenDruck(f, false)); // sonstige Leistungen
            }
            return liste;
        }
    }

    /// <summary>
    /// Verwaltet alle Noten eines Schülers in einem Fach (=Kurs)
    /// </summary>
    public class FachSchuelerNoten
    {
        public int schuelerId;
        private Fach fach=null;
        public int kursId
        {
            get;
            private set;
        }


        // das Array wird über das Halbjahr und den Notentyp indiziert, 
        // jedes Arrayelement enthält eine Liste mit Noten dieses Typs.
        private IList<int>[,] noten = new List<int>[Enum.GetValues(typeof(Halbjahr)).Length,Enum.GetValues(typeof(Notentyp)).Length];
        private BerechneteNote[] schnitte = new BerechneteNote[Enum.GetValues(typeof(Halbjahr)).Length];

        public FachSchuelerNoten(int aschuelerid, int akursid)
        {
            kursId = akursid;
            schuelerId = aschuelerid;
            foreach (Halbjahr hj in Enum.GetValues(typeof(Halbjahr)))
            {

                // erstmal leere Notenlisten anlegen
                foreach (Notentyp typ in Enum.GetValues(typeof(Notentyp)))
                    noten[(int)hj,(int)typ] = new List<int>();
            }

            // Notenlisten füllen je Typ und Halbjahr
            LeseNotenAusDB();
        }

        private void LeseNotenAusDB()
        {
            diNoDataSet.NoteDataTable notenDT;
            notenDT = new NoteTableAdapter().GetDataBySchuelerAndKurs(schuelerId, kursId);
            foreach (var noteR in notenDT)
            {
                // Note note = new Note(noteR);
                noten[noteR.Halbjahr,noteR.Notenart].Add(noteR.Punktwert);
            }

            // Schnitte werden direkt gelesen
            diNoDataSet.BerechneteNoteDataTable bnotenDT;
            // liefert max. 2 Datensätze (einen für 1. und 2. Hj.), historische Stände werden nicht geliefert
            bnotenDT = new BerechneteNoteTableAdapter().GetDataBySchuelerAndKurs(kursId, schuelerId);
            foreach (var bnoteR in bnotenDT)
            {                
                schnitte[(int)(bnoteR.ErstesHalbjahr ? Halbjahr.Erstes : Halbjahr.Zweites)] = 
                        new BerechneteNote(kursId, schuelerId, bnoteR);
            }
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
            if (s==null) return new BerechneteNote(kursId,schuelerId); // gibt leere Berechnungstabelle zurück
            return s;
        }

        /*
        public IList<int> getNoten(Notentyp typ)
        {
            IList<int> res = new List<int>(noten[(int)Halbjahr.Erstes, (int)typ]);
            noten[(int)Halbjahr.Zweites, (int)typ].CopyTo(res);
            return res;
        }
        */

        /// <summary>
        /// Liefert die zur Zeit z (z.B. Probezeit BOS) relevante Note (hier Jahresfortgang Ganzz. 1. Hj.)
        /// </summary>
        public byte? getRelevanteNote(Zeitpunkt z)
        {
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
                    Kurs k = new Kurs(kursId);
                    fach = new Fach(k.Data.FachId);
                }
                return fach;
            }
        }

        /// <summary>
        /// Liefert eine druckbare Liste für die SA
        /// </summary>
        public IList<string> SA(Halbjahr hj)
        {
            IList<string> liste = new List<string>();            
            InsertNoten(liste, getNoten(hj, Notentyp.Schulaufgabe), "");
            return liste;           
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
                    liste.Add(note + (bez=="" ? "" : " " + bez));                
            }        
        }
        
    }

    public class FachSchuelerNotenDruck
    {
        FachSchuelerNoten schueler;
        
        // Arrays können in Bericht leider nicht gedruckt werden, daher einzeln:
        // für SA / sL wird je ein Datensatz erzeugt
        // JF und DGes wird nur bei sL mitgeschickt
        public string fachBez { get; private set; }
        public string Art { get; private set; } // gibt den Text SA oder sL aus
        public string N11 { get; private set; } 
        public string N12 { get; private set; } // 2. Note im 1. Hj.
        public string N13 { get; private set; }
        public string N14 { get; private set; }
        public string N15 { get; private set; }
        public string N16 { get; private set; }
        public string N21 { get; private set; }
        public string N22 { get; private set; }
        public string N23 { get; private set; }
        public string N24 { get; private set; }
        public string N25 { get; private set; }
        public string N26 { get; private set; }
        public string D1 { get; private set; } // Durchschnitt 1. Hj.
        public string DGes1 { get; private set; } // Schnitt Gesamt im 1. Hj.
        public string JF1 { get; private set; }
        public string D2 { get; private set; }
        public string DGes2 { get; private set; }
        public string JF2 { get; private set; }
        
        public FachSchuelerNotenDruck(FachSchuelerNoten s, bool evalSA)
        {
            schueler = s;
            fachBez = s.getFach.Bezeichnung;

            var d1 = s.getSchnitt(Halbjahr.Erstes);
            var d2 = s.getSchnitt(Halbjahr.Zweites);

            IList<string> n1,n2;
            if (evalSA)
            {
                Art = "SA";
                n1 = s.SA(Halbjahr.Erstes);
                n2 = s.SA(Halbjahr.Zweites);
                D1 = String.Format("{0:f2}", d1.SchnittSchulaufgaben);
                D2 = String.Format("{0:f2}", d2.SchnittSchulaufgaben);                              

            }
            else
            {
                Art = "sL";
                n1 = s.sonstigeLeistungen(Halbjahr.Erstes);
                n2 = s.sonstigeLeistungen(Halbjahr.Zweites);
                D1 = String.Format("{0:f2}", d1.SchnittMuendlich);
                D2 = String.Format("{0:f2}", d2.SchnittMuendlich);
                DGes1 = String.Format("{0:f2}", d1.JahresfortgangMitKomma);
                JF1 = d1.JahresfortgangGanzzahlig.ToString();
                DGes2 = String.Format("{0:f2}", d2.JahresfortgangMitKomma);
                JF2 = d2.JahresfortgangGanzzahlig.ToString();
            }                      
            checkLen(n1,6);
            N11 = put(n1,0);
            N12 = put(n1,1);
            N13 = put(n1,2);
            N14 = put(n1,3);
            N15 = put(n1,4);
            N16 = put(n1,5);
            
            checkLen(n2,6);
            N21 = put(n2,0);
            N22 = put(n2,1);
            N23 = put(n2,2);
            N24 = put(n2,3);
            N25 = put(n2,4);
            N26 = put(n2,5);
        }

        private string put(IList<string> n, int index)
        {
            if (index < n.Count)
                return n[index];
            else
                return "";
        }

        private void checkLen(IList<string> n, int maxindex)
        {
            if (n.Count>=maxindex)
            {
                throw new IndexOutOfRangeException("Notenbogendruck: Zuviele Noten im Fach " + schueler.getFach.Bezeichnung + " bei Schüler " + schueler.schuelerId);            
            }
        }
    }
    
    /*
    public class FachSchuelerNotenDruck
    {
        FachSchuelerNoten schueler;
        
        // Arrays können in Bericht leider nicht gedruckt werden, daher einzeln:
        public string fachBez { get; private set; }        
        public string SA11 { get; private set; }
        public string SA12 { get; private set; }
        public string SA21 { get; private set; }
        public string SA22 { get; private set; }
        public string sL11 { get; private set; }
        public string sL12 { get; private set; }
        public string sL13 { get; private set; }
        public string sL14 { get; private set; }
        public string sL15 { get; private set; }
        public string sL16 { get; private set; }
        public string sL21 { get; private set; }
        public string sL22 { get; private set; }
        public string sL23 { get; private set; }
        public string sL24 { get; private set; }
        public string sL25 { get; private set; }
        public string sL26 { get; private set; }
        public string DSA1 { get; private set; }
        public string DsL1 { get; private set; }
        public string DGes1 { get; private set; }
        public string JF1 { get; private set; }
        public string DSA2 { get; private set; }
        public string DsL2 { get; private set; }
        public string DGes2 { get; private set; }
        public string JF2 { get; private set; }
        
        public FachSchuelerNotenDruck(FachSchuelerNoten s)
        {
            schueler = s;
            fachBez = s.getFach.Bezeichnung;
            IList<string> n;
            n = s.SA(Halbjahr.Erstes);
            checkLen(n,2);
            SA11 = put(n,0);
            SA12 = put(n,1);
            n = s.SA(Halbjahr.Zweites);
            checkLen(n,2);
            SA21 = put(n,0);
            SA22 = put(n,1);

            n = s.sonstigeLeistungen(Halbjahr.Erstes);
            checkLen(n,6);
            sL11 = put(n,0);
            sL12 = put(n,1);
            sL13 = put(n,2);
            sL14 = put(n,3);
            sL15 = put(n,4);
            sL16 = put(n,5);
            n = s.sonstigeLeistungen(Halbjahr.Zweites);
            checkLen(n,6);
            sL21 = put(n,0);
            sL22 = put(n,1);
            sL23 = put(n,2);
            sL24 = put(n,3);
            sL25 = put(n,4);
            sL26 = put(n,5);

            var d = s.getSchnitt(Halbjahr.Erstes);
            DSA1 = String.Format("{0:f2}", d.SchnittSchulaufgaben);
            DsL1 = String.Format("{0:f2}", d.SchnittMuendlich);
            DGes1 = String.Format("{0:f2}", d.JahresfortgangMitKomma);
            JF1 = d.JahresfortgangGanzzahlig.ToString();

            d = s.getSchnitt(Halbjahr.Zweites);
            DSA2 = String.Format("{0:f2}", d.SchnittSchulaufgaben);
            DsL2 = String.Format("{0:f2}", d.SchnittMuendlich);
            DGes2 = String.Format("{0:f2}", d.JahresfortgangMitKomma);
            JF2 = d.JahresfortgangGanzzahlig.ToString();
        }

        private string put(IList<string> n, int index)
        {
            if (index < n.Count)
                return n[index];
            else
                return "";
        }

        private void checkLen(IList<string> n, int maxindex)
        {
            if (n.Count>=maxindex)
            {
                throw new IndexOutOfRangeException("Notenbogendruck: Zuviele Noten im Fach " + schueler.getFach.Bezeichnung + " bei Schüler " + schueler.schuelerId);            
            }
        }
    }
    */
}
