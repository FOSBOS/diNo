using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public class Zugriff {
      private static Zugriff _Instance = null;

      public string Username { get; private set;}
      public bool IsAdmin{ get; private set;}
      public diNoDataSet.LehrerRow Lehrer { get; private set;}
      public List<Klasse> Klassen { get; private set;}
      
      private Zugriff()
      {
            Klassen = new List<Klasse>();
            Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            // Username = "FOSBOS\\ckonrad";
            IsAdmin = (!Username.Contains("FOSBOS") || Username.Equals("FOSBOS\\Administrator"));
            Username = Username.Replace("FOSBOS\\", "");
            var lehrerResult = new LehrerTableAdapter().GetDataByWindowsname(Username);
            Lehrer = (lehrerResult == null || lehrerResult.Count == 0) ? null : lehrerResult[0];

            if (!IsAdmin && Lehrer == null)
            {
                throw new InvalidOperationException("Keine Zugriffsberechtigung!");
            }

            foreach (var klasse in (new KlasseTableAdapter().GetData()))
            {
                Klasse dieKlasse = new Klasse(klasse);
                // Dies filtert wenigstens ein Paar Dummy- und Spassklassen heraus
                if (dieKlasse.getSchueler.Count > 0)
                    // TODO: die DB soll gleich die richtigen Klassen liefern
                    if (IsAdmin || LehrerUnterrichtetKlasse(Lehrer.Id, dieKlasse))
                    {
                        Klassen.Add(dieKlasse);
                    }
            }
         
            Klassen.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
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

     public static Zugriff Instance {
        get {
          if (_Instance == null) {
            _Instance = new Zugriff();
          }
          return _Instance;
        }
      }
  }

}
