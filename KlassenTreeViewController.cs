using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;

namespace diNo
{
  public class KlassenTreeViewController
  {
    public static IList<Klasse> GetSortedKlassenList(bool nurEigeneKlassen)
    {
      string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

      // wenn die Software in der Schule läuft, enthalten die Nutzer den Domänennamen
      if (nurEigeneKlassen && username.Contains("FOSBOS"))
      {
        username = username.Replace("FOSBOS\\", "");
        var lehrerResult = new LehrerTableAdapter().GetDataByWindowsname(username);
        var angemeldeterLehrer = (lehrerResult == null || lehrerResult.Count == 0) ? null : lehrerResult[0];
        if (angemeldeterLehrer == null && nurEigeneKlassen)
        {
          return new List<Klasse>();
        }

        List<Klasse> klassen = new List<Klasse>();
        foreach (var klasse in (new KlasseTableAdapter().GetData()))
        {
          Klasse dieKlasse = new Klasse(klasse);
          // Dies filtert wenigstens ein Paar Dummy- und Spassklassen heraus
          if (dieKlasse.getSchueler.Count > 0 && LehrerUnterrichtetKlasse(angemeldeterLehrer.Id, dieKlasse))
          {
            klassen.Add(dieKlasse);
          }
        }

        klassen.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
        return klassen;
      }
      else
      {
        return GetSortedKlassenList();
      }
    }

    public static bool LehrerUnterrichtetKlasse(int lehrerId, Klasse klasse)
    {
      foreach (Klasse teilKlasse in Klasse.GetTeilKlassen(klasse.Bezeichnung))
      {
        foreach (var kurs in teilKlasse.Kurse)
        {
          if (kurs.getLehrer.Id == lehrerId)
          {
            return true;
          }
        }
      }

      return false;
    }

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
