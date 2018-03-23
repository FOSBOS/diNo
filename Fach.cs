using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
  public class Fach :IRepositoryObject
  {
    private diNoDataSet.FachRow data;
    private int[] sort = new int[5]; // Profilfachnummer je nach Ausbildungsrichtung (Index vgl. enum Zweig)

    public Fach(int id)
    {
      var rst = new FachTableAdapter().GetDataById(id);
      if (rst.Count == 1)
      {
          this.data = rst[0];
      }
      else
      {
          throw new InvalidOperationException("Konstruktor Kurs: Ungültige ID=" + id);
      }
    }

    public Fach(diNoDataSet.FachRow f)
    {
      data = f;
      InitSortierung();
    }

    private void InitSortierung()
    { if (Typ == FachTyp.Profilfach)
      {
        var ta = new FachSortierungTableAdapter();
        var d = ta.GetDataByFachId(Id);
        if (d.Count==0)
          throw new InvalidOperationException("Im Fach " + Bezeichnung + " (Id=" + Id + ") liegt keine Sortierung vor.");
        foreach (var r in d)
        {
          byte i = (byte)Faecherkanon.GetZweig(r.Zweig);
          sort[i] = r.Sortierung;
        }

      }
    }


    public static Fach CreateFach(int id)
    {
      return new Fach(id);
    }

    public int Id
    {
        get { return this.data.Id; }
    }

    public int GetId()
    {
      return Id;
    }
    public string Bezeichnung
    {
        get { return this.data.Bezeichnung; }
    }

        public string Kuerzel
        {
            get { return this.data.Kuerzel; }
        }

        // ab neuer FOBOSO: SA pro Halbjahr
        public int AnzahlSA(Zweig zweig, Jahrgangsstufe jg)
        {
          int z=0;
          if (jg == Jahrgangsstufe.Vorklasse)
          {
             // in D,E,M je 2 SA
             if (Kuerzel == "D" || Kuerzel == "DAZ" || Kuerzel == "E" || Kuerzel == "M") z=2;             
          }
          else if (jg == Jahrgangsstufe.Elf)
          {
            if (IstSAPFach(zweig)) z=1;            
          }
          else // 12./13. Klasse
          {
            if (IstSAPFach(zweig) || Kuerzel=="F" /*|| Kuerzel=="F-Wi"*/) z = (jg == Jahrgangsstufe.Zwoelf) ? 3 : 2;
            else if (Kuerzel == "TeIn" || Kuerzel == "B" || Kuerzel == "VWL" ||
              (Kuerzel == "C" && zweig==Zweig.Umwelt)) z=2;
          }
          return z;
        }
        public bool IstSAFach(Zweig zweig, Jahrgangsstufe jg)
        {
            return AnzahlSA(zweig,jg)>0;
        }

        public int Sortierung(Zweig zweig)
        {
          if (zweig != Zweig.None && Typ == FachTyp.Profilfach)
          {            
            return 100+sort[(byte)zweig];
          }
          else
          {
            return this.data.Sortierung;
          }          
        }

        public FachTyp Typ
        {
          get { return (FachTyp)data.Typ; }
        }

        public string BezZeugnis
        {
          get { return (data.IsBezZeugnisNull() ? data.Bezeichnung : data.BezZeugnis); }
        }
   
    public bool IstSAPFach(Zweig zweig)
    {

      if (Kuerzel == "D" || Kuerzel == "E" || Kuerzel == "M") return true;
      else if (Typ == FachTyp.Profilfach)
      {
        return sort[(int)zweig] == 1; // nur das 1. Profilfach ist SAP-Fach
      }
      else return false;
    }

        // Ermittelt die SA-Wertung für diesen Kurs
        public Schulaufgabenwertung GetSchulaufgabenwertung(Zweig zweig,Jahrgangsstufe jg)
        {
          int z=0;
          z = AnzahlSA(zweig,jg);
          if (z==0) return Schulaufgabenwertung.KurzarbeitenUndExen;
          else if (z<=2) return Schulaufgabenwertung.EinsZuEins;
          else return Schulaufgabenwertung.ZweiZuEins;
        }

        public bool IstEinstuendig(Jahrgangsstufe jg,Schulart sa)
        {
          return ((sa==Schulart.BOS || jg==Jahrgangsstufe.Dreizehn) && (Kuerzel=="K" || Kuerzel=="Ev" || Kuerzel=="Eth"));
            // || ggf. Kunst im S-Zweig, und Wl in A-Zweig in der 11. Klasse
        }
      }
    


///  <summary>
/// Klasse Fächerkanon dient der Konvertierung von Strings in Enumerables
/// </summary>
public static class Faecherkanon
	{
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static Zweig GetZweig(string zweig)
    {
      switch (zweig)
      {
        case "S": return Zweig.Sozial;
        case "T": return Zweig.Technik;
        case "WVR":
        case "W":
          return Zweig.Wirtschaft;        
        case "U": return Zweig.Umwelt;
        default: return Zweig.None; //Vorklasse FOS ohne Zweigzuordnung
      }
    }

    public static string GetZweigText(Schueler s)
    {
      switch (s.Data.Ausbildungsrichtung)
      {
        case "S": return "Sozialwesen";
        case "T": return "Technik";
        case "WVR":
        case "W":
          return "Wirtschaft und Verwaltung";
        case "U": return "Agrarwirtschaft, Bio- und Umwelttechnologie";
        default: return "";
      }
    }

    public static Jahrgangsstufe GetJahrgangsstufe(string jahrgangsstufe)
    {
      if (jahrgangsstufe.ToUpper().Contains("VS"))
      {
        return Jahrgangsstufe.Vorkurs;
      }

      if (jahrgangsstufe=="IV" || jahrgangsstufe.ToUpper().Contains("VK"))
      {
        return Jahrgangsstufe.Vorklasse;
      }

      if (jahrgangsstufe.Contains("11"))
      {
        return Jahrgangsstufe.Elf;
      }

      if (jahrgangsstufe.Contains("12"))
      {
        return Jahrgangsstufe.Zwoelf;
      }

      if (jahrgangsstufe.Contains("13"))
      {
        return Jahrgangsstufe.Dreizehn;
      }

      return Jahrgangsstufe.None;
    }

    public static string GetJahrgangsstufe(Jahrgangsstufe jahrgangsstufe)
    {
      switch (jahrgangsstufe)
      {
        case Jahrgangsstufe.Vorklasse: return "10";  // FOS Vorklasse
        case Jahrgangsstufe.Elf: return "11";
        case Jahrgangsstufe.Zwoelf: return "12" ;
        case Jahrgangsstufe.Dreizehn: return "13" ;
        default: throw new InvalidOperationException("unbekannte Jahrgangsstufe " + jahrgangsstufe);
      }
    }

    public static string GetSchulartString(Schulart schulart)
    {
      switch (schulart)
      {
        case Schulart.None: return "None";
        case Schulart.FOS: return "FOS";
        case Schulart.BOS: return "BOS";
        case Schulart.ALLE: return "ALLE";
        default: throw new InvalidOperationException("Unbekannte Schulart : "+schulart);
      }
    }

    public static string GetZweigString(Zweig zweig)
    {
      switch (zweig)
      {
        case Zweig.None: return "None";
        case Zweig.Sozial: return "S";
        case Zweig.Wirtschaft: return "W";
        case Zweig.Technik: return "T";
        case Zweig.Umwelt: return "U";        
        default: throw new InvalidOperationException("Unbekannter Zweig : " + zweig);
      }
    }

	}

  public enum FachTyp
  {
    Allgemein = 0,
    Profilfach = 1,
    WPF = 2,
    OhneNoten = 9
  }
}
