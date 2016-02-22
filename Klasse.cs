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
    Agrar = 4,
    ALLE = 15
  }

  /// <summary>
  /// FactoryClass, die eine Menge von Klassen je nach Auswahlkriterien liefert
  /// </summary>
  public static class KlassenController
  {
    private static IList<Klasse> Fill(diNoDataSet.KlasseDataTable klassen)
    {
      IList<Klasse> res = new List<Klasse>();
      foreach (var klasse in klassen)
      {
        res.Add(new Klasse(klasse));
      }
      return res;
    }

    public static IList<Klasse> AlleKlassen()
    {
      return Fill(new KlasseTableAdapter().GetData());

    }

    public static IList<Klasse> KlassenProLehrer(int lehrerId)
    {
      return Fill(new KlasseTableAdapter().GetDataByLehrerId(lehrerId));
    }

  }

  public class Klasse
  {
    private diNoDataSet.KlasseRow data;
    private diNoDataSet.SchuelerDataTable schueler;
    private diNoDataSet.LehrerRow klassenleiter;
    public List<Schueler> eigeneSchueler;

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

    public Klasse(diNoDataSet.KlasseRow klasseR)
    {
      eigeneSchueler = new List<Schueler>();
      data = klasseR;
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
        string bez = this.Bezeichnung.ToUpper();

        if ((bez.Contains("W") && !bez.Contains("S") && !bez.Contains("A") && !bez.Contains("T")) || bez.EndsWith("_W"))
        {
          return Zweig.Wirtschaft;
        }

        if ((bez.Contains("S") && !bez.Contains("W") && !bez.Contains("A") && !bez.Contains("T")) || bez.EndsWith("_S"))
        {
          return Zweig.Sozial;
        }

        if ((bez.Contains("T") && !bez.Contains("S") && !bez.Contains("A") && !bez.Contains("W")) || bez.EndsWith("_T"))
        {
          return Zweig.Technik;
        }

        if ((bez.Contains("A") && !bez.Contains("S") && !bez.Contains("T") && !bez.Contains("W")) || bez.EndsWith("_A"))
        {
          return Zweig.Agrar;
        }

        //hier stehen nur noch die Klassen, deren Zweig nicht eindeutig ist, z. B. Vorkurs BOS oder Mischklassen
        return Zweig.None;
      }
    }

    public diNoDataSet.LehrerRow Klassenleiter
    {
      get
      {
        if (this.klassenleiter == null)
        {
          var lehrer = new LehrerTableAdapter().GetDataById(this.Data.KlassenleiterId);
          this.klassenleiter = lehrer.Count == 1 ? lehrer[0] : null;
        }

        return this.klassenleiter;
      }
    }

    public IList<Kurs> FindeAlleMöglichenKurse(Zweig zweig)
    {
      var result = new List<Kurs>(this.Kurse);
      foreach (var teilKlasse in this.GetTeilKlassen())
      {
        if (teilKlasse.Zweig == zweig || teilKlasse.Zweig == Zweig.None) // None steht auch bei Mischklassen. Dann gehen alle Schüler in diese Kurse.
        {
          foreach (var neuerKurs in teilKlasse.Kurse)
          {
            var kursSchonDrin = result.Find(x => x.Id == neuerKurs.Id);
            if (kursSchonDrin == null)
            {
              result.Add(neuerKurs);
            }
          }
        }
      }

      return result;
    }

    public IList<Kurs> Kurse
    {
      get
      {
        var ada = new KlasseKursTableAdapter();
        List<Kurs> result = new List<Kurs>();
        foreach (var klasseZukursRow in ada.GetDataByKlasse(this.Data.Id))
        {
          // TODO: Wie kann es sein, dass in der BVkST_S manche Kurse doppelt existieren? 
          //       Gibt es hier ein Problem beim Import, z. B. wegen der Lehrertandems?
          //       Und wieso lässt die Datenbank dies überhaupt zu?
          var found = result.Find(
            x => x.Id == klasseZukursRow.KursId
            );
          if (found == null)
          {
            result.Add(new Kurs(klasseZukursRow.KursId));
          }
        }

        return result;
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

    public static Klasse FindKlasse(string bezeichnung)
    {
      foreach (var klasseRow in new KlasseTableAdapter().GetDataByBezeichnung(bezeichnung))
      {
        return new Klasse(klasseRow);
      }

      return null;
    }

    public IList<Klasse> GetTeilKlassen()
    {
      var children = new KlasseTableAdapter().GetDataByVaterklasse(this.data.Id);
      var result = new List<Klasse>();
      if (children != null && children.Count > 0)
      {
        foreach (var child in children)
        {
          result.Add(new Klasse(child));
        }
      }

      return result;
    }
  }

  /// <summary>
  /// Ein Kurs.
  /// </summary>
  public class Kurs
  {
    private diNoDataSet.KursRow data;
    private diNoDataSet.SchuelerDataTable schueler;
    private Fach fach;
    private diNoDataSet.LehrerRow lehrer;
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

    /// <summary>
    /// Id des Kurses in der Datenbank.
    /// </summary>
    public int Id
    {
      get;
      private set;
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
          fach = new Fach(data.FachId);
        }
        return fach;
      }
    }

    public diNoDataSet.LehrerRow getLehrer
    {
      get
      {
        if (lehrer == null && !data.IsLehrerIdNull())
        {
          lehrer = new LehrerTableAdapter().GetDataById(data.LehrerId)[0];
        }
        return lehrer;

        // return data.LehrerRow; so sollte es eigentlich gehen
      }
    }

    public string FachBezeichnung
    {
      get { return this.getFach.Bezeichnung; }
    }

    private void setSchreibtKA()
    {
      var rst = new NoteTableAdapter().GetKAByKursId(Id);
      schreibtKA = rst.Count > 0;
    }


  }
}
