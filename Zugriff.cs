using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public class Zugriff
  {
    private static Zugriff _Instance = null;

    public string Username { get; private set; }
    public Lehrer lehrer = null; // angemeldeter Lehrer
    public List<Klasse> Klassen { get; private set; }
    public List<Fach> eigeneFaecher { get; private set; }
    public Klasse eigeneKlasse { get; private set; } // Verweis auf die Klasse, in der der User Klassenleiter ist
    public int AnzahlSchueler { get; private set; }

    // folgende Nachschlagelisten dienen v.a. der Performance, damit die Objekte insgesamt nur 1x im Speicher angelegt werden müssen!
    public Repository<Schueler> SchuelerListe = new Repository<Schueler>(Schueler.CreateSchueler);
    public Dictionary<int, Kurs> KursListe = new Dictionary<int, Kurs>();
    public Dictionary<int, string> LehrerListe = new Dictionary<int, string>(); // aktuell nicht verwendet


    private diNoDataSet.GlobaleKonstantenRow globaleKonstanten;
    public int Schuljahr { get { return globaleKonstanten.Schuljahr; } }
    public Sperrtyp Sperre { get { return (Sperrtyp)globaleKonstanten.Sperre; } }
    public bool SiehtAlles{ get; private set; }
    public bool HatVerwaltungsrechte{ get; private set; }

    public int aktZeitpunkt
    {
      get { return globaleKonstanten.aktZeitpunkt; }
      set { globaleKonstanten.aktZeitpunkt = value; SaveGlobaleKonstanten(); }
    }

    private Zugriff()
    {
      Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
      // TODO: Username ToUpper verwenden, dann muss aber die DB passen
      if (Username == "Markus-PC\\Markus")
      {
        Username = "FOSBOS\\msiegel";
      }
      if (Username == "ClausPC\\Claus")
      {
        Username = "FOSBOS\\ckonrad";
      }
      
      Username = Username.Replace("FOSBOS\\", "");
      Username = Username.Replace("VW\\", "");
      var lehrerResult = new LehrerTableAdapter().GetDataByWindowsname(Username);
      if (lehrerResult.Count > 0) lehrer = new Lehrer(lehrerResult[0]);
      else
      {
        throw new InvalidOperationException("Keine Zugriffsberechtigung!");
      }
      SiehtAlles = (this.lehrer.HatRolle(Rolle.Admin) || this.lehrer.HatRolle(Rolle.Sekretariat) || this.lehrer.HatRolle(Rolle.Schulleitung));
      HatVerwaltungsrechte = lehrer.HatRolle(Rolle.Admin) || lehrer.HatRolle(Rolle.Sekretariat);      
    
   

      // LoadSchueler(); erst in Klassenansicht, wegen Parameter nurAktive
      LoadFaecher();
      // LoadLehrer(); wird aktuell nicht benötigt
      LoadGlobaleKonstanten();
    }

    public static Zugriff Instance
    {
      get
      {
        if (_Instance == null)
        {
          _Instance = new Zugriff();
        }
        return _Instance;
      }
    }

    public bool IstNurNormalerLehrer
    {
      get
      {
        return this.lehrer.rollen.Count == 0;
      }
    }
   
    


    public void LoadSchueler(bool nurAktive=true)
    {
      List<int> klassenIds = new List<int>(); // für schnelles Auffinden
      Klassen = new List<Klasse>();
      diNoDataSet.SchuelerDataTable sListe;
      int NotStatus = nurAktive?1:255; // Status=1 bedeutet abgemeldet,

      var ta = new SchuelerTableAdapter();
      if (SiehtAlles)
        sListe = ta.GetDataByStatus(NotStatus); // alle Schüler reinladen
      else if (IstNurNormalerLehrer)
        sListe = ta.GetDataByLehrerId(NotStatus,lehrer.Id); //  nur eigene Schüler            
      else
        sListe = ta.GetDataByLehrerIdFPASem(NotStatus,lehrer.Id); // Lehrer mit erweiterten Rollen

      AnzahlSchueler = sListe.Count;
      foreach (var sRow in sListe)
      {
        Schueler s = new Schueler(sRow);
        int index = klassenIds.IndexOf(sRow.KlasseId);
        if (index < 0) // Klasse kam bis jetzt nicht vor, also anlegen
        {
          klassenIds.Add(sRow.KlasseId);
          Klassen.Add(s.getKlasse);
          index = klassenIds.Count - 1;
        }        
        s.getKlasse = Klassen[index]; // dem Schüler die Klasseninstanz zuweisen, damit die nicht jedesmal neu erzeugt werden muss!
        Klassen[index].eigeneSchueler.Add(s); // und umgekehrt dieser Klasse den Schüler hinzufügen
        SchuelerListe.Add(s); // Schüler ins Repository aufnehmen
      }

      // alles sortieren
      Klassen.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
      foreach (var klasse in Klassen)
      {
        klasse.eigeneSchueler.Sort((x, y) => x.NameVorname.CompareTo(y.NameVorname));
        if (klasse.KlassenleiterId == lehrer.Id) eigeneKlasse = klasse;
      }
    }

    private void LoadFaecher()
    {
      eigeneFaecher = new List<Fach>();
      var ta = new FachTableAdapter();
      diNoDataSet.FachDataTable dtFach = this.lehrer.HatRolle(Rolle.Admin) ? ta.GetData() : ta.GetDataByLehrerId(lehrer.Id);
      foreach (var fRow in dtFach)
      {
        Fach f = new Fach(fRow);
        eigeneFaecher.Add(f);
      }
    }

    private void LoadLehrer()
    {
      diNoDataSet.LehrerDataTable dt;      
      var ta = new LehrerTableAdapter();
      dt = ta.GetData();

      foreach (var r in dt)
      {
        LehrerListe.Add(r.Id, r.Name + " (" + r.Kuerzel + ")");
      }
    }

    private void LoadGlobaleKonstanten()
    {
      globaleKonstanten = new GlobaleKonstantenTableAdapter().GetData()[0];
    }

    private void SaveGlobaleKonstanten()
    {
      new GlobaleKonstantenTableAdapter().Update(globaleKonstanten);
    }

    // Lädt die Schüler in den Speicher
    public static void Refresh()
    {
      Instance.Klassen = null; // Garbage-Collector 
      Instance.LoadSchueler();
    }
    
    public bool HatRolle(Rolle typ)
    {
      return lehrer.HatRolle(typ);
    }
  }

  public enum Sperrtyp
  {
    Keine = 0,
    Notenschluss = 1
  }

  public enum Rolle
  {
    Lehrer = 0,
    Admin = 1,
    Schulleitung = 2,
    Sekretariat = 3,
    Seminarfach = 4,
    FpAWirtschaft = 5,
    FpASozial = 6,
    FpATechnik = 7,
    FpAAgrar = 8
  }


}
