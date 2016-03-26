using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public class Lehrer
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

    private void Init(diNoDataSet.LehrerRow row)
    {
      this.data = row;
      SetKlassenleiter();
      SetRollen();
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
      if (rst.Count>0) KlassenleiterVon = new Klasse(rst[0]);
    }

  }
}
