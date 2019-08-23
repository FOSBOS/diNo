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
        Fachreferat = 5, // TODO: 5-7 raus, wenn AlteFOBOSO raus
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

  /// <summary>
  /// Eine Note.
  /// </summary>
  public class Note
  {
    private int kursid, schuelerid;
    private diNoDataSet.NoteRow row;

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

      row = nr;
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

    // schreibt ein Notenobjekt in die DB
    public void writeToDB()
    {
      NoteTableAdapter na = new NoteTableAdapter();
      if (row == null)
      {
        int noteid;
        na.Insert((int)Typ, Punktwert, DateTime.Now.Date, Zelle, (byte)Halbjahr, schuelerid, kursid, out noteid);

        row = na.GetDataById(noteid)[0]; // todo: testen ob das so auch wirklich geht, d.h. steht in noteId tatsächlich die ID der Note nach dem Insert?
      }
      else
      {
        row.Datum = this.Datum;
        row.Halbjahr = (byte)this.Halbjahr;
        row.KursId = this.kursid;
        row.Notenart = (int)this.Typ;
        row.Punktwert = this.Punktwert;
        row.SchuelerId = this.schuelerid;
        na.Update(row);
      }
    }
  }
     

  public static class Notentools
  {
    public static decimal Aufrunden2NK(decimal schnitt)
    {
      return Math.Ceiling(schnitt*100)/100;
    }
    
    public static byte RundeJF(decimal schnitt)
    {
      if (schnitt < 1)
          return 0;
      else 
          return (byte) Math.Round((double)schnitt,0,MidpointRounding.AwayFromZero);
    }

    public static byte BerechneZeugnisnote(decimal? jf,decimal? sap, decimal? map)
    {
      decimal abi;
      
      // bei externen ist jf==null
      if (jf!=null && map==null && sap==null) return RundeJF(jf.GetValueOrDefault());

      if (map==null) abi = sap.GetValueOrDefault();
      else if (sap==null) abi = map.GetValueOrDefault();
      else abi = Aufrunden2NK((2*sap.GetValueOrDefault()+map.GetValueOrDefault())/3);

      if (jf==null) return RundeJF(abi);
      else return RundeJF(Aufrunden2NK((jf.GetValueOrDefault()+abi)/2));
    }

    public static byte BerechneAbiGes(int sap, int map)
    {
      return RundeJF((2 * sap+ map) / (decimal)3);
    }
  }
}
