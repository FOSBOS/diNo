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
            diNoDataSet.SchuelerDataTable dtSchueler;
            List<int> klassenIds = new List<int>(); // für schnelles Auffinden
            int index;

            Klassen = new List<Klasse>();

            Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //Username = "FOSBOS\\ckonrad";
            IsAdmin = (!Username.Contains("FOSBOS") || Username.Equals("FOSBOS\\Administrator"));
            Username = Username.Replace("FOSBOS\\", "");
            var lehrerResult = new LehrerTableAdapter().GetDataByWindowsname(Username);
            Lehrer = (lehrerResult == null || lehrerResult.Count == 0) ? null : lehrerResult[0];

            if (!IsAdmin && Lehrer == null)
            {
                throw new InvalidOperationException("Keine Zugriffsberechtigung!");
            }

            var ta = new SchuelerTableAdapter();
            if (IsAdmin) dtSchueler = ta.GetData();
            else dtSchueler = ta.GetDataByLehrerId(Lehrer.Id);

            foreach (var sRow in dtSchueler)
            {
                Schueler s = new Schueler(sRow);
                index = klassenIds.IndexOf(sRow.KlasseId);
                if (index<0) {
                    klassenIds.Add(sRow.KlasseId);
                    Klassen.Add(s.getKlasse);
                    index = klassenIds.Count - 1;
                }
                Klassen[index].eigeneSchueler.Add(s); // dieser Klassen den Schüler hinzufügen
            }
         
            // alles sortieren
            Klassen.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
            foreach (var klasse in Klassen)
                klasse.eigeneSchueler.Sort((x, y) => x.NameVorname.CompareTo(y.NameVorname));
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
