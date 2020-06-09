using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;

namespace diNo
{
  public class Lehrer : IRepositoryObject
  {
    private diNoDataSet.LehrerRow data;
    private bool KlassenleiterAbfragen = true;
    private Klasse klassenleiterVon = null;
    public IList<int> rollen;

    public Lehrer(int id)
    {
      var rst = new LehrerTableAdapter().GetDataById(id);
      if (rst.Count == 1)
      {
        Init(rst[0]);
      }
      else
      {
        throw new InvalidOperationException("Konstruktor Lehrer: Ungültige ID.");
      }
    }

    public Lehrer(diNoDataSet.LehrerRow r)
    {
      Init(r);
    }

    public static Lehrer CreateLehrer(int id)
    {
      return new Lehrer(id);
    }

    public int GetId()
    {
      return data.Id;
    }

    public string Kuerzel
    { get { return data.Kuerzel; } }

    public string Name
    {
      get { return this.data.Nachname + ", " + this.Data.Vorname; }
    }

    /*  public string NameMitAbkVorname
        {
          get { return Data.Vorname.Substring(0, 1) + ". " + Data.Nachname; }
        }*/

    public string VornameName
    {
      get { return Data.Vorname + " " + Data.Nachname; }
    }

    public string NameDienstbezeichnung
    {
      get { return Data.Vorname + " " + Data.Nachname + ", " + Data.Dienstbezeichnung; }
    }

    public string KLString
    {
      get { return "Klassenleiter" + (Data.Geschlecht == "W" ? "in" : ""); }
    }

    private void Init(diNoDataSet.LehrerRow row)
    {
      this.data = row;
      SetRollen();
    }


    public void AddRolle(int aRolle)
    {
      LehrerRolleTableAdapter ada = new LehrerRolleTableAdapter();
      if (!this.HatRolle(aRolle))
      {
        ada.Insert(this.Id, (int)aRolle);
        this.rollen.Add((int)aRolle);
      }
    }

    public void AddRolle(Rolle aRolle)
    {
      AddRolle((int)aRolle);
    }

    public void RemoveRolle(Rolle aRolle)
    {
      RemoveRolle((int)aRolle);
    }

    public void RemoveRolle(int aRolle)
    {
      LehrerRolleTableAdapter ada = new LehrerRolleTableAdapter();
      if (this.HatRolle(aRolle))
      {
        ada.Delete(this.Id, aRolle);
        this.rollen.Remove(aRolle);
      }
    }

    private void SetRollen()
    {
      rollen = new List<int>();
      var lehrerRolleTA = new LehrerRolleTableAdapter();
      foreach (var rolle in lehrerRolleTA.GetDataByLehrerId(this.Id))
      {
        this.rollen.Add(rolle.RolleId);
      }
    }

    public bool HatRolle(Rolle rolle)
    {
      return HatRolle((int)rolle);
    }

    /// <summary>
    /// Prüft das Vorliegen von Berechtigungs-Rollen. Schwierig: Manche Rollen existieren im Quellcode, andere nur in der DB (dynamisch)
    /// </summary>
    /// <param name="rolle">Die Rollen-Id.</param>
    /// <returns>Ob der Lehrer diese Rolle besitzt.</returns>
    public bool HatRolle(int rolle)
    {
      if (rolle == (int)Rolle.Lehrer)
      {
        return true; // jeder Nutzer bekommt automatisch die Rolle Lehrer. Dann muss das nicht in der DB immer eingetragen werden.
      }

      return this.rollen.Contains(rolle);
    }

    public diNoDataSet.LehrerRow Data
    {
      get { return data; }
    }

    public int Id
    { get { return data.Id; } }

    public Klasse KlassenleiterVon
    {
      get
      { // übliche Technik funktioniert nicht ganz, weil KL auch null sein kann.
        if (KlassenleiterAbfragen)
        {
          var rst = new KlasseTableAdapter().GetDataByKlassenleiterId(Id);
          if (rst.Count > 0) klassenleiterVon = Zugriff.Instance.KlassenRep.Find(rst[0].Id);
          KlassenleiterAbfragen = false;
        }
        return klassenleiterVon;
      }
    }

    public string KompletterName
    {
      get { return this.Data.Nachname + ", " + this.Data.Vorname; }
    }
  }
}
