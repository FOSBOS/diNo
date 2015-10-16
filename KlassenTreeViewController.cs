using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
  public class KlassenTreeViewController
  {


    public static IList<Klasse> GetSortedKlassenList()
    {
      List<Klasse> klassen = new List<Klasse>();
      foreach (var klasse in (new KlasseTableAdapter().GetData()))
      {
        Klasse dieKlasse = new Klasse(klasse);
        if (dieKlasse.getSchueler.Count > 0)
        {
          // Dies filtert wenigstens ein Paar Dummy- und Spassklassen heraus
          klassen.Add(dieKlasse);
        }
      }

      klassen.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
      return klassen;
    }

    public static List<Schueler> GetSortedSchuelerList(Klasse klasse)
    {
      List<Schueler> schueler = new List<Schueler>();
      foreach (var einSchueler in klasse.getSchueler)
      {
        schueler.Add(new Schueler(einSchueler));
      }

      schueler.Sort((x, y) => x.NameVorname.CompareTo(y.NameVorname));
      return schueler;
    }

    public static object SelectValueCol1(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return ((Klasse)rowObject).Bezeichnung;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).NameVorname;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }
  }
}
