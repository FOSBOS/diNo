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
    /// Der Default Value.
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
    Seminararfach = 6,
		/// <summary>
		/// Ersatzprüfung.
		/// </summary>
		Ersatzprüfung = 7,
    /// <summary>
    /// Durchschnittsnote der Schulaufgaben.
    /// </summary>
    SchnittSA = 8,
    /// <summary>
    /// Durchschnittsnote aller mündlichen und Exen.
    /// </summary>
    Schnittmuendlich = 9,
    /// <summary>
    /// Jahresfortgang mit 2 Nachkommastellen.
    /// </summary>
    JahresfortgangMitNKS = 10,
    /// <summary>
    /// Jahresfortgang (ganzzahlig).
    /// </summary>
    Jahresfortgang = 11,
    /// <summary>
    /// Schriftliche Abschlussprüfung.
    /// </summary>
    APSchriftlich = 12,
    /// <summary>
    /// Mündliche Abschlussprüfung.
    /// </summary>
    APMuendlich = 13,
    /// <summary>
    /// Gesamtnote Abschlussprüfung.
    /// </summary>
    APGesamt = 14,
    /// <summary>
    /// Endnote (Jahresfortgang und Abschlussprüfung) mit 2 Nachkommastellen.
    /// </summary>
    EndnoteMitNKS = 15,
    /// <summary>
    /// Note im Abschlusszeugnis (ganzzahlig).
    /// </summary>
    Abschlusszeugnis = 16
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
            Typ = (Notentyp) nr.Notenart;
            Punktwert = nr.Punktwert;
            Datum = nr.Datum;
            Zelle = nr.Zelle;
            Halbjahr = (Halbjahr) nr.Halbjahr;
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

    // verwaltet alle Noten eines Schülers in einem Kurs in der Liste noten
    public class NotenProKurs
    {
        private diNoDataSet.NoteDataTable notenDT;
        public List<Note> noten;
        public NotenProKurs(int schuelerid, int kursid)
        {
            noten = new List<Note>();
            notenDT = new NoteTableAdapter().GetDataBySchuelerAndKurs(schuelerid,kursid);
            foreach (var noteR in notenDT)
            {
                noten.Add(new Note(noteR));
            }

            // berechnete Note dieses Schülers und Kurses der Eigenschaft zuweisen:
            var berechneteNoteRst = new BerechneteNoteTableAdapter().GetDataBySchuelerAndKurs(kursid, schuelerid);
            if (berechneteNoteRst.Count == 0)
            {
                throw new InvalidOperationException("Konstruktor NotenProKurs: Keine berechneten Noten vorhanden.");
            }
            berechneteNote = berechneteNoteRst[0];
        }

        public diNoDataSet.BerechneteNoteRow berechneteNote
        {
            get;
            private set;
        }
    }

}
