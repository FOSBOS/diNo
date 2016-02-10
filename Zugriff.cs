﻿using System;
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
      public int AnzahlSchueler { get; private set;}
      public List<Fach> eigeneFaecher { get; private set;}
      public Dictionary<int, string> Lehrerliste;
      public int Schuljahr { get; private set;}
      public int aktZeitpunkt;
      
      private Zugriff()
      {
            Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //Username = "FOSBOS\\ckonrad";
            IsAdmin = (!Username.Contains("FOSBOS") || Username.ToUpper().Equals("FOSBOS\\ADMINISTRATOR"));
            Username = Username.Replace("FOSBOS\\", "");
            var lehrerResult = new LehrerTableAdapter().GetDataByWindowsname(Username);
            Lehrer = (lehrerResult == null || lehrerResult.Count == 0) ? null : lehrerResult[0];

            if (!IsAdmin && Lehrer == null)
            {
                throw new InvalidOperationException("Keine Zugriffsberechtigung!");
            }
            LoadSchueler();
            LoadFaecher();
            LoadLehrer();
            LoadGlobaleKonstanten();
     }

    

     public static Zugriff Instance {
        get {
          if (_Instance == null) {
            _Instance = new Zugriff();
          }
          return _Instance;
        }
      }
    
      private void LoadSchueler() {
            diNoDataSet.SchuelerDataTable dtSchueler;
            List<int> klassenIds = new List<int>(); // für schnelles Auffinden
            int index;

            Klassen = new List<Klasse>();

            var ta = new SchuelerTableAdapter();
            if (IsAdmin) dtSchueler = ta.GetData();
            else dtSchueler = ta.GetDataByLehrerId(Lehrer.Id);
            AnzahlSchueler = dtSchueler.Count;

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
      
        private void LoadFaecher()
        {
          diNoDataSet.FachDataTable dtFach;
          eigeneFaecher = new List<Fach>();
          var ta = new FachTableAdapter();
          if (IsAdmin) dtFach = ta.GetData();
          else dtFach = ta.GetDataByLehrerId(Lehrer.Id);          

          foreach (var fRow in dtFach)
          {
            Fach f = new Fach(fRow);
            eigeneFaecher.Add(f);
          }
        }

        private void LoadLehrer()
        {
          diNoDataSet.LehrerDataTable dt;
          Lehrerliste = new Dictionary<int, string>();
          var ta = new LehrerTableAdapter();
          dt = ta.GetData();
                
          foreach (var r in dt)
          {
            Lehrerliste.Add(r.Id,r.Name + " ("+r.Kuerzel + ")");
          }
        }  

        public void LoadGlobaleKonstanten()
        {
          diNoDataSet.GlobaleKonstantenDataTable dt;
          var ta = new GlobaleKonstantenTableAdapter();
          dt = ta.GetData();
          aktZeitpunkt = dt[0].aktZeitpunkt;
          Schuljahr = dt[0].Schuljahr; 
        }

        // Lädt die Schüler in den Speicher
        public static void Refresh()
        {
             Instance.Klassen = null; // Garbage-Collector 
             Instance.LoadSchueler();
        }
  }

}
