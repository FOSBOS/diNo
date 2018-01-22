using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public class Lehrer : IRepositoryObject
  {
    private diNoDataSet.LehrerRow data;
    private bool KlassenleiterAbfragen=true;
    private Klasse klassenleiterVon=null;
    public IList<Rolle> rollen;

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
    { get {return data.Kuerzel; } }

    public string Name
    {
      get { return this.data.Nachname + ", " + this.Data.Vorname; }
    }

    public string NameMitAbkVorname
    {
      get { return Data.Vorname.Substring(0, 1) + ". " + Data.Nachname; }
    }

    public string VornameName
    {
      get { return Data.Vorname + " " + Data.Nachname; }
    }

    public string NameDienstbezeichnung
    {
      get { return Data.Vorname + " " + Data.Nachname + ", " + Data.Dienstbezeichnung; }
    }

    public string NameMitAbkVornameDienstbezeichnung
    {
      get { return Data.Vorname.Substring(0, 1) + ". " + Data.Nachname + ", " + Data.Dienstbezeichnung; }
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

    public void AddRolle(Rolle aRolle)
    {
      LehrerRolleTableAdapter ada = new LehrerRolleTableAdapter();
      if (!this.HatRolle(aRolle))
      {
        ada.Insert(this.Id, (int)aRolle);
        this.rollen.Add(aRolle);
      }
    }

    public void RemoveRolle(Rolle aRolle)
    {
      LehrerRolleTableAdapter ada = new LehrerRolleTableAdapter();
      if (this.HatRolle(aRolle))
      {
        ada.Delete(this.Id, (int)aRolle);
        this.rollen.Remove(aRolle);
      }
    }

    private void SetRollen()
    {
      rollen = new List<Rolle>();
      var lehrerRolleTA = new LehrerRolleTableAdapter();
      foreach (var rolle in lehrerRolleTA.GetDataByLehrerId(this.Id))
      {
        this.rollen.Add((Rolle)rolle.RolleId);
      }
    }

    public bool HatRolle(Rolle rolle)
    {
      if (rolle == Rolle.Lehrer)
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
    { get { // übliche Technik funktioniert nicht ganz, weil KL auch null sein kann.
        if (KlassenleiterAbfragen)          
        {
          var rst = new KlasseTableAdapter().GetDataByKlassenleiterId(Id);
          if (rst.Count>0) klassenleiterVon = Zugriff.Instance.KlassenRep.Find(rst[0].Id);
          KlassenleiterAbfragen = false;
        }
        return klassenleiterVon;
    } }

    public string KompletterName
    {
      get { return this.Data.Nachname + ", " + this.Data.Vorname; }
    }
  }
}
