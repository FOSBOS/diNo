using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
    /// <summary>
    /// Typ einer Note.
    /// </summary>
    public enum Notentyp
    {
        /// <summary>
        /// Default
        /// </summary>
        NONE = 0,
        /// <summary>
        /// Schulaufgabe.
        /// </summary>
        Schulaufgabe = 1,
        /// <summary>
        /// Kurzarbeit
        /// </summary>
        Kurzarbeit = 2,
        /// <summary>
        /// Ex.
        /// </summary>
        Ex = 3,
        /// <summary>
        /// Echte mündliche Note.
        /// </summary>
        EchteMuendliche = 4,
        /// <summary>
        /// Fachreferat.
        /// </summary>
        Fachreferat = 5,
        /// <summary>
        /// Seminararfach.
        /// </summary>
        Seminarfach = 6,
        /// <summary>
        /// Ersatzprüfung.
        /// </summary>
        Ersatzprüfung = 7,
        /// <summary>
        /// Schriftliche Abschlussprüfung.
        /// </summary>
        APSchriftlich = 8,
        /// <summary>
        /// Mündliche Abschlussprüfung.
        /// </summary>
        APMuendlich = 9
    }

    public enum BerechneteNotentyp
    {
        /// <summary>
        /// Durchschnittsnote der Schulaufgaben.
        /// </summary>
        SchnittSA = 1,
        /// <summary>
        /// Durchschnittsnote aller mündlichen und Exen.
        /// </summary>
        Schnittmuendlich = 2,
        /// <summary>
        /// Jahresfortgang mit 2 Nachkommastellen.
        /// </summary>
        JahresfortgangMitNKS = 3,
        /// <summary>
        /// Jahresfortgang (ganzzahlig).
        /// </summary>
        Jahresfortgang = 4,
        /// <summary>
        /// Gesamtnote Abschlussprüfung.
        /// </summary>
        APGesamt = 5,
        /// <summary>
        /// Endnote (Jahresfortgang und Abschlussprüfung) mit 2 Nachkommastellen.
        /// </summary>
        EndnoteMitNKS = 6,
        /// <summary>
        /// Note im Abschlusszeugnis (ganzzahlig).
        /// </summary>
        Abschlusszeugnis = 7
    }

    /// <summary>
    /// Enumeration fuer Halbjahre. Bitte Nummern nicht ändern (die werden so in die Datenbank als int gecasted).
    /// </summary>
    public enum Halbjahr
    {

        /// <summary>
        /// Keine Zuordnung zu einem Halbjahr möglich.
        /// </summary>
        Ohne = 0,

        /// <summary>
        /// Erstes Halbjahr.
        /// </summary>
        Erstes = 1,
        /// <summary>
        /// Zweites Halbjahr.
        /// </summary>
        Zweites = 2
    }

    /// <summary>
    /// Enumeration für Zeitpunkte zur Standspeicherung und Überprüfung der Notenkonsistenzen
    /// Bitte Nummern nicht ändern (die werden so in die Datenbank als int gecasted).
    /// </summary>
    public enum Zeitpunkt
    {
        None = 0,
        ProbezeitBOS = 1,
        HalbjahrUndProbezeitFOS = 2,
        ErstePA = 3,
        ZweitePA = 4,
        DrittePA = 5,
        Jahresende = 6
    }

    public class BerechneteNote
    {
        public decimal? SchnittMuendlich
        {
            get;
            set;
        }

        public decimal? SchnittSchulaufgaben
        {
            get;
            set;
        }

        public byte? JahresfortgangGanzzahlig
        {
            get;
            set;
        }

        public decimal? JahresfortgangMitKomma
        {
            get;
            set;
        }

        public decimal? PruefungGesamt
        {
            get;
            set;
        }

        public decimal? SchnittFortgangUndPruefung
        {
            get;
            set;
        }

        public byte? Abschlusszeugnis
        {
            get;
            set;
        }
        public bool ErstesHalbjahr { get; set; }

        public Zeitpunkt StandNr
        {
            get;
            private set;
        }

        private int kursid, schuelerid;

        public BerechneteNote(int aKursId, int aSchuelerId)
        {
            kursid = aKursId;
            schuelerid = aSchuelerId;
            StandNr = Zeitpunkt.None;
        }

        public BerechneteNote(int aKursId, int aSchuelerId, diNoDataSet.BerechneteNoteRow d)
        {
            kursid = aKursId;
            schuelerid = aSchuelerId;
            
            SchnittSchulaufgaben = d.SchnittSchulaufgaben;
            SchnittMuendlich = d.SchnittMuendlich;
            JahresfortgangMitKomma = d.JahresfortgangMitKomma;
            JahresfortgangGanzzahlig = d.JahresfortgangGanzzahlig;
            PruefungGesamt = d.PruefungGesamt;
            SchnittFortgangUndPruefung = d.SchnittFortgangUndPruefung;
            Abschlusszeugnis = d.Abschlusszeugnis;
            StandNr = (Zeitpunkt)d.StandNr;
            ErstesHalbjahr = d.ErstesHalbjahr;
        }

        public void writeToDB()
        {
            BerechneteNoteTableAdapter na = new BerechneteNoteTableAdapter();
            na.Insert(SchnittMuendlich, SchnittSchulaufgaben, JahresfortgangMitKomma, JahresfortgangGanzzahlig,
                    PruefungGesamt, SchnittFortgangUndPruefung, Abschlusszeugnis, 0, false, schuelerid, kursid, ErstesHalbjahr);

        }        
    }
        /// <summary>
        /// Eine Note.
        /// </summary>
        public class Note
        {
            private int kursid, schuelerid;

            // baut Notenobjekt aus einer DB-Zeile auf
            public Note(diNoDataSet.NoteRow nr)
            {
                kursid = nr.KursId;
                schuelerid = nr.SchuelerId;
                Typ = (Notentyp)nr.Notenart;
                Punktwert = nr.Punktwert;
                Datum = nr.Datum;
                Zelle = nr.Zelle;
                Halbjahr = (Halbjahr)nr.Halbjahr;
            }

            // Note von Hand eingeben (z.B. aus Excel)
            public Note(int aKursId, int aSchuelerId)
            {
                this.kursid = aKursId;
                this.schuelerid = aSchuelerId;
            }

            /// <summary>
            /// Der Typ der Note, z. B. Schulaufgabe oder Ex.
            /// </summary>
            public Notentyp Typ
            {
                get;
                set;
            }

            /// <summary>
            /// Der Punktwert der Note (0-15).
            /// </summary>
            public byte Punktwert
            {
                get;
                set;
            }

            /// <summary>
            /// Das Datum der Note.
            /// </summary>
            public DateTime Datum
            {
                get;
                set;
            }

            /// <summary>
            /// In welcher Zelle diese Note steht.
            /// </summary>
            public string Zelle
            {
                get;
                set;
            }

            /// <summary>
            /// Das Halbjahr, welchem die Note zuzuordnen ist.
            /// </summary>
            public Halbjahr Halbjahr
            {
                get;
                set;
            }

            // schreibt ein Notenobjekt in die DB (keine Aktualisierung, d.h. alte Note muss vorher gelöscht sein)
            public void writeToDB()
            {
                int noteid;
                NoteTableAdapter na = new NoteTableAdapter();
                na.Insert((int)Typ, Punktwert, DateTime.Now.Date, Zelle, (byte)Halbjahr, schuelerid, kursid, out noteid);
            }
        }
     }