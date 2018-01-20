using System;
using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;

namespace diNo
{
  public enum Jahrgangsstufe
  {
    None = 0,
    Vorkurs = 9,
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
    Umwelt = 4,
    ALLE = 15
  }

  public class Klasse :  IRepositoryObject
  {
    private diNoDataSet.KlasseRow data;
    private diNoDataSet.SchuelerDataTable schueler;
    private Lehrer klassenleiter;
    public List<Schueler> eigeneSchueler;
    private List<Kurs> kurse = null;

    public Klasse(int id)
    {
      eigeneSchueler = new List<Schueler>();
      var rst = new KlasseTableAdapter().GetDataById(id);
      if (rst.Count == 1)
      {
        this.data = rst[0];
      }
      else
      {
        throw new InvalidOperationException("Konstruktor Klasse: Ungültige ID.");
      }
    }

    public int GetId()
    {
      return data.Id;
    }

    public Klasse(diNoDataSet.KlasseRow klasseR)
    {
      eigeneSchueler = new List<Schueler>();
      data = klasseR;
    }

    public static Klasse CreateKlasse(int id)
    {
      return new Klasse(id);
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
      get
      {
        return Faecherkanon.GetJahrgangsstufe(data.Bezeichnung);
      }
    }

    public Schulart Schulart
    {
      get
      {
        string klasse = data.Bezeichnung;
        if (klasse.StartsWith("B") || klasse.StartsWith("b"))
        {
          return Schulart.BOS;
        }
        else
        {
          return Schulart.FOS;
        }
      }
    }

    public Zweig Zweig
    {
      get
      {
        string bez = this.Bezeichnung;//.ToUpper();
        string letztesZeichen = bez.Substring(bez.Length-1,1);
        if (bez.Contains("_")) return CharToZweig(letztesZeichen); // Teilklasse

        // TODO: eleganter über Regex lösen...
        if ((bez.Contains("W") && !bez.Contains("S") && !bez.Contains("U") && !bez.Contains("T")) || bez.EndsWith("_W"))
        {
          return Zweig.Wirtschaft;
        }

        if ((bez.Contains("S") && !bez.Contains("W") && !bez.Contains("U") && !bez.Contains("T")) || bez.EndsWith("_S"))
        {
          return Zweig.Sozial;
        }

        if ((bez.Contains("T") && !bez.Contains("S") && !bez.Contains("U") && !bez.Contains("W")) || bez.EndsWith("_T"))
        {
          return Zweig.Technik;
        }

        if ((bez.Contains("U") && !bez.Contains("S") && !bez.Contains("T") && !bez.Contains("W")) || bez.EndsWith("_U"))
        {
          return Zweig.Umwelt;
        }

        //hier stehen nur noch die Klassen, deren Zweig nicht eindeutig ist, z. B. Vorkurs BOS oder Mischklassen
        return Zweig.None;
      }
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
           
    public bool AlteFOBOSO()
    {
        return (Jahrgangsstufe >= Jahrgangsstufe.Zwoelf);
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
    private Fach fach;
    private Lehrer lehrer;
    public bool schreibtKA;


    public Kurs(int id)
    {
      this.Id = id;
      var rst = new KursTableAdapter().GetDataById(id);
      if (rst.Count == 1)
      {
        this.data = rst[0];
        setSchreibtKA();
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
      setSchreibtKA();
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

    private void setSchreibtKA()
    {
      var rst = new NoteTableAdapter().GetKAByKursId(Id);
      schreibtKA = rst.Count > 0;
    }

    public void SetzeNeuenLehrer(Lehrer lehrer)
    {
      data.LehrerId = lehrer.Id;
      (new KursTableAdapter()).Update(this.data);
      lehrer = Zugriff.Instance.LehrerRep.Find(lehrer.Id);
    }
  }
}
