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
    }

    /// <summary>
    /// Verwaltet alle Noten eines Schülers in einem Fach (=Kurs)
    /// </summary>
    public class FachSchuelerNoten
    {
        private int schuelerId;
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
            return schnitte[(int)hj];
        }

        /*
        public IList<int> getNoten(Notentyp typ)
        {
            IList<int> res = new List<int>(noten[(int)Halbjahr.Erstes, (int)typ]);
            noten[(int)Halbjahr.Zweites, (int)typ].CopyTo(res);
            return res;
        }
        */

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

    }

}
