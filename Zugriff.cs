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
    public Lehrer lehrer = null;
    public List<Klasse> Klassen { get; private set; }
    public int AnzahlSchueler { get; private set; }
    public List<Fach> eigeneFaecher { get; private set; }
    public Dictionary<int, string> Lehrerliste;
    private diNoDataSet.GlobaleKonstantenRow globaleKonstanten;
    public int Schuljahr { get { return globaleKonstanten.Schuljahr; } }
    public Sperrtyp Sperre { get { return (Sperrtyp)globaleKonstanten.Sperre; } }
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

      //Username = "FOSBOS\\ckonrad";
      //Username = "VW\\gmerk";
      Username = Username.Replace("FOSBOS\\", "");
      var lehrerResult = new LehrerTableAdapter().GetDataByWindowsname(Username);
      if (lehrerResult.Count > 0) lehrer = new Lehrer(lehrerResult[0]);
      else
      {
        throw new InvalidOperationException("Keine Zugriffsberechtigung!");
      }

      LoadSchueler();
      LoadFaecher();
      LoadLehrer();
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

    private void LoadSchueler()
    {
      List<int> klassenIds = new List<int>(); // für schnelles Auffinden
      Klassen = new List<Klasse>();

      var ta = new SchuelerTableAdapter();

      Dictionary<int, Schueler> schueler = new Dictionary<int, Schueler>();
      foreach (var aSchueler in ta.GetDataByLehrerId(lehrer.Id))
      {
        // erstmal alle eigenen Schueler einladen, damit die sicher sichtbar sind.
        schueler.Add(aSchueler.Id, new Schueler(aSchueler));
      }

      if (!this.IstNurNormalerLehrer)
      {
        // danach schauen, ob aufgrund einer besonderen Rolle des Lehrers noch andere Schueler angezeigt werden müssen
        foreach (var aSchueler in ta.GetData())
        {
          Schueler derSchueler = new Schueler(aSchueler);
          if (!schueler.ContainsKey(aSchueler.Id) && SollSichtbarSein(derSchueler))
            schueler.Add(aSchueler.Id, derSchueler);
        }
      }

      AnzahlSchueler = schueler.Count;
      foreach (var sRow in schueler.Values)
      {
        int index = klassenIds.IndexOf(sRow.Data.KlasseId);
        if (index < 0)
        {
          klassenIds.Add(sRow.Data.KlasseId);
          Klassen.Add(sRow.getKlasse);
          index = klassenIds.Count - 1;
        }

        Klassen[index].eigeneSchueler.Add(sRow); // dieser Klassen den Schüler hinzufügen
      }

      // alles sortieren
      Klassen.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
      foreach (var klasse in Klassen)
        klasse.eigeneSchueler.Sort((x, y) => x.NameVorname.CompareTo(y.NameVorname));
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
      Lehrerliste = new Dictionary<int, string>();
      var ta = new LehrerTableAdapter();
      dt = ta.GetData();

      foreach (var r in dt)
      {
        Lehrerliste.Add(r.Id, r.Name + " (" + r.Kuerzel + ")");
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

    private bool SollSichtbarSein(Schueler schueler)
    {
      if (this.lehrer.HatRolle(Rolle.Admin))
      {
        return true;
      }
      if (this.lehrer.HatRolle(Rolle.FpAAgrar) && (schueler.Zweig == Zweig.Agrar && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf))
      {
        return true;
      }
      if (this.lehrer.HatRolle(Rolle.FpASozial) && (schueler.Zweig == Zweig.Sozial && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf))
      {
        return true;
      }
      if (this.lehrer.HatRolle(Rolle.FpATechnik) && (schueler.Zweig == Zweig.Technik && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf))
      {
        return true;
      }
      if (this.lehrer.HatRolle(Rolle.FpAWirtschaft) && (schueler.Zweig == Zweig.Wirtschaft && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf))
      {
        return true;
      }
      if (this.lehrer.HatRolle(Rolle.Seminarfach) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        return true;
      }

      return false;
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
