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

    public Lehrer(int id)
    {
        var rst = new LehrerTableAdapter().GetDataById(id);
        if (rst.Count == 1)
        {
          data = rst[0];
          SetKlassenleiter();
        }
        else
        {
            throw new InvalidOperationException("Konstruktor Lehrer: Ungültige ID.");
        }
    }

    public Lehrer(diNoDataSet.LehrerRow r)
    {
      data = r;
      SetKlassenleiter();
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
