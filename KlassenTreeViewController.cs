using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;

namespace diNo
{
  public class KlassenTreeViewController
  {
    public static IList<Klasse> GetSortedKlassenList(bool nurEigeneKlassen)
    {
        return Zugriff.Instance.Klassen;
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
