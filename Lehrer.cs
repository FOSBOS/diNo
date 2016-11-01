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
    public Klasse KlassenleiterVon=null;
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
      get { return this.data.Nachname + " " + this.Data.Vorname; }
    }

    private void Init(diNoDataSet.LehrerRow row)
    {
      this.data = row;
      SetKlassenleiter();
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

    private void SetKlassenleiter()
    {
      var rst = new KlasseTableAdapter().GetDataByKlassenleiterId(Id);
      if (rst.Count>0) KlassenleiterVon = Zugriff.Instance.KlassenRep.Find(rst[0].Id);
    }

    public string KompletterName
    {
      get { return this.Data.Nachname + ", " + this.Data.Vorname; }
    }
  }
}
