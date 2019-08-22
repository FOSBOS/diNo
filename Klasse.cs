using System;
using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;

namespace diNo
{
  public enum Jahrgangsstufe
  {
    None = 0,
    Vorkurs = 8, // funktioniert noch nicht
    IntVk = 9,
    Vorklasse = 10,    
    Elf = 11,
    Zwoelf = 12,
    Dreizehn = 13,
  }

  public enum Schulart
  {
    None = 0,
    FOS = 1,
    BOS = 2,
    ALLE = 15
  }

  public enum Zweig
  {
    None = 0,
    Sozial = 1,
    Technik = 2,
    Wirtschaft = 3,
    Umwelt = 4    
  }

  public class Klasse :  IRepositoryObject
  {
    private diNoDataSet.KlasseRow data;
    private diNoDataSet.SchuelerDataTable schueler;
    private Lehrer klassenleiter;
    private Jahrgangsstufe jg;
    private Zweig zweig;
    private Schulart schulart;
    public List<Schueler> eigeneSchueler;
    private List<Kurs> kurse = null;

    public Klasse(int id)
    {   
      var rst = new KlasseTableAdapter().GetDataById(id);
      if (rst.Count == 1)
      {
        data = rst[0];
        Init();
      }
      else
      {
        throw new InvalidOperationException("Konstruktor Klasse: Ungültige ID=" + id);
      }
    }
    
    public Klasse(diNoDataSet.KlasseRow klasseR)
    {      
      data = klasseR;
      Init();
    }

    private void Init()
    {
      eigeneSchueler = new List<Schueler>();
      jg = Faecherkanon.GetJahrgangsstufe(data.Bezeichnung);
      
      // Zweig der Klasse (None für Mischklassen)
      string bez = this.Bezeichnung;
      if (bez.Contains("_"))
        zweig = CharToZweig(bez.Substring(bez.Length - 1, 1)); // Teilklasse      
      else if      (bez.Contains("W") && !bez.Contains("S") && !bez.Contains("U") && !bez.Contains("T"))
        zweig = Zweig.Wirtschaft;
      else if (bez.Contains("S") && !bez.Contains("W") && !bez.Contains("U") && !bez.Contains("T"))
        zweig = Zweig.Sozial;
      else if (bez.Contains("T") && !bez.Contains("S") && !bez.Contains("U") && !bez.Contains("W"))
        zweig = Zweig.Technik;
      else if (bez.Contains("U") && !bez.Contains("S") && !bez.Contains("T") && !bez.Contains("W"))
        zweig = Zweig.Umwelt;
      else //hier stehen nur noch die Klassen, deren Zweig nicht eindeutig ist, z. B. Vorkurs BOS oder Mischklassen      
        zweig = Zweig.None;
      
      if (bez.StartsWith("B") || bez.StartsWith("b"))
        schulart = Schulart.BOS;
      else
        schulart = Schulart.FOS;
    }

    public static Klasse CreateKlasse(int id)
    {
      return new Klasse(id);
    }

    public int GetId()
    {
      return data.Id;
    }

    public diNoDataSet.KlasseRow Data
    {
      get { return this.data; }
    }

    public string Bezeichnung
    {
      get
      {
        return data.Bezeichnung;
      }
    }

    public Jahrgangsstufe Jahrgangsstufe
    {
      get { return jg; }
    }

    public Zweig Zweig
    {
      get { return zweig; }
    }

    public string JahrgangsstufeZeugnis
    {
      get
      {
        if (Jahrgangsstufe == Jahrgangsstufe.IntVk) return "Integrationsvorklasse";
        if (Jahrgangsstufe == Jahrgangsstufe.Vorklasse) return "Vorklasse";
        return "Jahrgangsstufe " + ((int)Jahrgangsstufe);
      }
    }    
    
    public Schulart Schulart
    {
      get { return schulart; }
    }

    private Zweig CharToZweig(string c)
    {
      switch (c)
      {
        case "S" : return Zweig.Sozial;
        case "T" : return Zweig.Technik;
        case "W" : return Zweig.Wirtschaft;
        case "A" : return Zweig.Umwelt;
        default: return Zweig.None; 
      }
    }
           
    public int KlassenleiterId
    {
      get { return Data.IsKlassenleiterIdNull() ? 0 : Data.KlassenleiterId;  }
    }


    public Lehrer Klassenleiter
    {
      get
      {
        if (klassenleiter == null)
        {
          klassenleiter = Zugriff.Instance.LehrerRep.Find(Data.KlassenleiterId);          
        }

        return klassenleiter;
      }
    }

    public override string ToString()
    {
      return this.Bezeichnung;
    }

    // alle Kurse, die in dieser Klasse angeboten werden
    public IList<Kurs> Kurse
    {
      get
      {
        if (kurse==null)
        {
          var ta = new KursTableAdapter();
          kurse = new List<Kurs>();
          var dt = ta.GetDataByKlasseId(Data.Id);
          foreach (var kursRow in dt)
          {
            Kurs k = new Kurs(kursRow);
            kurse.Add(k);
            // Kurse werden normalerweise nur in einer Klasse angeboten, daher hier ins Rep aufnehmen
            Zugriff.Instance.KursRep.Add(k); 
          }
        }
        return kurse;
      }
    }

    public diNoDataSet.SchuelerDataTable getSchueler
    {
      get
      {
        if (schueler == null)
        {
          SchuelerTableAdapter sa = new SchuelerTableAdapter();
          schueler = sa.GetDataByKlasse(data.Id);
        }
        return schueler;
      }
    }
    
  }

  /// <summary>
  /// Ein Kurs.
  /// </summary>
  public class Kurs : IRepositoryObject
  {
    private diNoDataSet.KursRow data;
    private diNoDataSet.SchuelerDataTable schueler;
    private diNoDataSet.KlasseKursDataTable klassenZuordnungen;
    private Fach fach;
    private Lehrer lehrer;
    public Jahrgangsstufe JgStufe;
    public bool schreibtKA;

    public Kurs(int id)
    {
      this.Id = id;
      var rst = new KursTableAdapter().GetDataById(id);
      if (rst.Count == 1)
      {
        this.data = rst[0];
        Init();
      }
      else
      {
        throw new InvalidOperationException("Konstruktor Kurs: Ungültige ID.");
      }
    }

    public Kurs(diNoDataSet.KursRow data)
    {
      this.Id = data.Id;
      this.data = data;
      Init();
    }

    public static Kurs CreateKurs(int id)
    {
      return new Kurs(id);
    }

    /// <summary>
    /// Id des Kurses in der Datenbank.
    /// </summary>
    public int Id
    {
      get;
      private set;
    }

    public int GetId()
    {
      return Id;
    }

    public diNoDataSet.KursRow Data
    {
      get { return data; }
    }

    /// <summary>
    /// Die Liste der Schüler dieser Kurses (sortiert via SQL)
    /// </summary>
    public diNoDataSet.SchuelerDataTable Schueler
    {
      get
      {
        if (schueler == null)
        {
          SchuelerTableAdapter sa = new SchuelerTableAdapter();
          schueler = sa.GetDataByKursId(Id);
        }

        return schueler;
      }
    }

    /// <summary>
    /// Die Liste der Klassen dieser Kurses
    /// </summary>
    public diNoDataSet.KlasseKursDataTable KlassenZuordnungen
    {
      get
      {
        if (klassenZuordnungen == null)
        {
          KlasseKursTableAdapter kka = new KlasseKursTableAdapter();
          klassenZuordnungen = kka.GetDataByKursId(this.Id);
        }

        return klassenZuordnungen;
      }
    }

    public IList<Klasse> Klassen
    {
      get
      {
        IList<Klasse> result = new List<Klasse>();
        foreach (var klasse in KlassenZuordnungen)
        {
          result.Add(Zugriff.Instance.KlassenRep.Find(klasse.KlasseId));
        }
        return result;
      }
    }

    /// <summary>
    /// Speichert die aktuelle Klassenzuordnung in die Datenbank.
    /// </summary>
    public void SaveKlassenzuordnung()
    {
      KlasseKursTableAdapter kka = new KlasseKursTableAdapter();
      kka.Update(this.KlassenZuordnungen);
    }

    /// <summary>
    /// Die Liste der Schüler dieser Kurses (sortiert via SQL).
    /// </summary>
    /// <param name="excludeAusgetretene">Ob Ausgetretene ausgeschlossen werden sollen.</param>
    /// <returns>Liste mit den SchuelerRows.</returns>
    public IList<diNoDataSet.SchuelerRow> getSchueler(bool excludeAusgetretene)
    {
      if (excludeAusgetretene)
      {
        IList<diNoDataSet.SchuelerRow> result = new List<diNoDataSet.SchuelerRow>();
        foreach (var schueler in this.Schueler)
        {
          if (schueler.IsAustrittsdatumNull())
          {
            result.Add(schueler);
          }
        }

        return result;
      }
      else
      {
        return new List<diNoDataSet.SchuelerRow>(this.Schueler);
      }
    }

    /// <summary>
    /// Methode ermittelt anhand des ersten Schülers ob 
    /// </summary>
    /// <returns></returns>
    public bool IstSAPKurs
    {
      get; private set;
    }

    public Fach getFach
    {
      get
      {
        if (fach == null)
        {
          fach = Zugriff.Instance.FachRep.Find(data.FachId);
        }
        return fach;
      }
    }

    public void SetFachNull() // erzwingt ein neues Laden
    {
      fach = null;
    }

    public Lehrer getLehrer
    {
      get
      {
        if (lehrer == null && !data.IsLehrerIdNull())
        {
          lehrer = Zugriff.Instance.LehrerRep.Find(data.LehrerId);
        }
        return lehrer;        
      }
    }

    public void SetLehrerNull() // erzwingt ein neues Laden
    {
      lehrer = null;
    }

    public string Kurzbez // TODO: Kurzbez aus Datenbank
    {
      get { return getFach.Kuerzel + " " + Id; }
      //get { return Data.Kurzbez; }
    }
    
    public string Kursbezeichnung
    {
      get { return Data.Bezeichnung; }
    }

    public string FachBezeichnung
    {
      get { return getFach.Bezeichnung; }
    }

    public string Geschlecht
    {
      get
      {
        if (Data.IsGeschlechtNull()) return null;
        else return Data.Geschlecht;
      }
    }

    private void Init()
    {
      var rst = new NoteTableAdapter().GetKAByKursId(Id,(byte)Zugriff.Instance.aktHalbjahr);
      schreibtKA = rst.Count > 0;

      var ta = new KlasseTableAdapter();
      var dt = ta.GetDataByKursId(Id);
      JgStufe = Jahrgangsstufe.None;
      IstSAPKurs = false;
      foreach (var d in dt)
      {
        Klasse k = Zugriff.Instance.KlassenRep.Find(d.Id); // normalerweise wird ein Kurs nur in einer Jgstufe angeboten (Problem WPF--> s. LeseNotenausExcel)
        if (k.Jahrgangsstufe > JgStufe)
        {
          JgStufe = k.Jahrgangsstufe;
          IstSAPKurs = (JgStufe == Jahrgangsstufe.Zwoelf || JgStufe == Jahrgangsstufe.Dreizehn) && getFach.IstSAPFach(k.Zweig);
        }
      }      
    }

    public void SetzeNeuenLehrer(Lehrer lehrer)
    {
      data.LehrerId = lehrer.Id;
      (new KursTableAdapter()).Update(this.data);
      lehrer = Zugriff.Instance.LehrerRep.Find(lehrer.Id);
    }
  }
}
