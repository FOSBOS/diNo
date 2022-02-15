using diNo.diNoDataSetTableAdapters;
using System;
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

  public class Klasse : IRepositoryObject
  {
    private diNoDataSet.KlasseRow data;
    private diNoDataSet.SchuelerDataTable schueler;
    private Lehrer klassenleiter;
    private Jahrgangsstufe jg;
    private Zweig zweig;
    private Schulart schulart;
    public List<Schueler> Schueler;
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
      Schueler = new List<Schueler>();
      jg = (Jahrgangsstufe)data.JgStufe;
      if (jg==Jahrgangsstufe.None) 
      {
        throw new Exception("Jahrgangstufe bei Klasse " + data.Bezeichnung + " nicht belegt.");
        //jg = Jahrgangsstufe.Dreizehn;
      }
      if (data.IsZweigNull() || data.Zweig.Length != 1) zweig = Zweig.None;
      else zweig = CharToZweig(data.Zweig);
      schulart = data.IsSchulartNull() ? Schulart.None : (Schulart)data.Schulart;
    }

    public static Klasse CreateKlasse(int id)
    {
      return new Klasse(id);
    }

    public int GetId()
    {
      return data.Id;
    }

    public string Comparer()
    {
      return Bezeichnung;
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
        case "S": return Zweig.Sozial;
        case "T": return Zweig.Technik;
        case "W": return Zweig.Wirtschaft;
        case "U": return Zweig.Umwelt;
        default: return Zweig.None;
      }
    }

    public int KlassenleiterId
    {
      get { return Data.IsKlassenleiterIdNull() ? 0 : Data.KlassenleiterId; }
    }


    public Lehrer Klassenleiter
    {
      get
      {
        if (klassenleiter == null)
        {
          if (Data.IsKlassenleiterIdNull())
            throw new Exception("Klassenleiter fehlt bei Klasse " + Bezeichnung);
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
        if (kurse == null)
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
          kurse.Sort((x, y) => (x.getFach.Sortierung(Zweig)).CompareTo(y.getFach.Sortierung(Zweig)));
        }
        return kurse;
      }
    }

    public void RefreshKurse()
    {
      Zugriff.Instance.KursRep.Clear();
      kurse = null;
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

    public static void Insert(string bez)
    {
      var ta = new KlasseTableAdapter();
      Schulart schulart = (bez.StartsWith("B") || bez.StartsWith("b")) ? schulart = Schulart.BOS : Schulart.FOS;
      Jahrgangsstufe jg = Faecherkanon.GetJahrgangsstufe(bez);
      string zweig = "";
      if (bez.Contains("S")) zweig = "S";
      if (bez.Contains("T")) zweig += "T";
      if (bez.Contains("U")) zweig += "U";
      if (bez.Contains("W")) zweig += "W";
      ta.Insert(bez, null, (byte)jg, (byte)schulart, zweig);
    }

    public void Save()
    {
      var ta = new KlasseTableAdapter();
      ta.Update(data);
      klassenleiter = null;
      Init();
    }
  }

  /// <summary>
  /// Ein Kurs.
  /// </summary>
  public class Kurs : IRepositoryObject
  {
    private diNoDataSet.KursRow data;
    private List<Schueler> schueler;
    private List<Klasse> klassen;
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

    public string Comparer()
    {
      return Kurzbez;
    }

    public int GetId()
    {
      return Id;
    }

    public diNoDataSet.KursRow Data
    {
      get { return data; }
    }

    public string Kurzbezeichnung
    {
      get { return data.Kurzbez; }
    }

   
    /// <summary>
    /// Die Liste der Klassen dieser Kurses
    /// </summary>
    public List<Klasse> Klassen
    {
      get
      {
        if (klassen == null)
        {
          KlasseKursTableAdapter kka = new KlasseKursTableAdapter();
          var klassenZuordnungen = kka.GetDataByKursId(this.Id);
          klassen = new List<Klasse>();
          foreach (var klasse in klassenZuordnungen)
          {
            klassen.Add(Zugriff.Instance.KlassenRep.Find(klasse.KlasseId));
          }
        }

        return klassen;
      }
    }

    /// <summary>
    /// Speichert die aktuelle Klassenzuordnung in die Datenbank.
    /// </summary>
    public void SaveKlassenzuordnung()
    {
      KlasseKursTableAdapter kka = new KlasseKursTableAdapter();
      kka.DeleteByKursId(Id);
      foreach (Klasse k in klassen)
      {
        kka.Insert(k.Data.Id, Id);
      }
    }

    /// <summary>
    /// Die Liste der Schüler dieser Kurses
    /// </summary>
    public List<Schueler> Schueler
    {      
      get
      {
        if (schueler == null)
        {
          schueler = new List<Schueler>();
          Schueler s;
          var ta = new SchuelerKursTableAdapter();
          var dt = ta.GetDataByKursId(this.Id);

          foreach (var d in dt)
          {
            s = Zugriff.Instance.SchuelerRep.Find(d.SchuelerId);
            if (s.Status == Schuelerstatus.Aktiv)
              schueler.Add(s);
          }
          schueler.Sort((x, y) => x.NameVorname.CompareTo(y.NameVorname));
        }

        return schueler;
      }
    }

    public void ResetSchueler()
    {
      schueler = null;
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
      var rst = new NoteTableAdapter().GetKAByKursId(Id, (byte)Zugriff.Instance.aktHalbjahr);
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
          Zweig z;
          if (Schueler.Count > 0)
            z = Faecherkanon.GetZweig(Schueler[0].Data.Ausbildungsrichtung);
          else
            z = Zweig.None;

          IstSAPKurs = (JgStufe == Jahrgangsstufe.Zwoelf || JgStufe == Jahrgangsstufe.Dreizehn) && getFach.IstSAPFach(z);
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
