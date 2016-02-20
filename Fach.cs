using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
    public class Fach
    {
        private diNoDataSet.FachRow data;

        public Fach(int id)
        {
            var rst = new FachTableAdapter().GetDataById(id);
            if (rst.Count == 1)
            {
                this.data = rst[0];
            }
            else
            {
                throw new InvalidOperationException("Konstruktor Kurs: Ungültige ID.");
            }
        }

        public Fach(diNoDataSet.FachRow f)
        {
          data = f;
        }

        public int Id
        {
            get { return this.data.Id; }
        }

        public string Bezeichnung
        {
            get { return this.data.Bezeichnung; }
        }

        public string Kuerzel
        {
            get { return this.data.Kuerzel; }
        }

        public int AnzahlSA(Zweig zweig, Jahrgangsstufe jg)
        {
          int z=0;
          if (jg == Jahrgangsstufe.Vorklasse)
          {
             // in D,E,M je 3 SA, in 2 Fächern des Profilbereichs je 2 SA
             // (T-Zweig: Ph,C;  W: BwR,Te;  S/A: B,C)
             if (Kuerzel == "D" || Kuerzel == "E" || Kuerzel == "M") z=3;
             else if (Kuerzel == "Ph" || Kuerzel == "B" || Kuerzel == "C"|| Kuerzel == "Te"|| Kuerzel == "BwR") z=2;
          }
          else if (jg == Jahrgangsstufe.Elf || jg == Jahrgangsstufe.Vorkurs)
          {
            if (IstSAPFach(zweig)) z=2;
          }
          else // 12./13. Klasse
          {
            if (IstSAPFach(zweig) || Kuerzel=="F" /*|| Kuerzel=="F-Wi"*/) z = (jg == Jahrgangsstufe.Zwoelf) ? 3 : 2;
            else if (Kuerzel == "TeIn" || Kuerzel == "B" || Kuerzel == "VWL" ||
              (Kuerzel == "C" && zweig==Zweig.Agrar)) z=2;
          }
          return z;
        }
        public bool IstSAFach(Zweig zweig, Jahrgangsstufe jg)
        {
            return AnzahlSA(zweig,jg)>0;
        }

        public int Sortierung
        {
            get { return this.data.Sortierung; }
        }


        public bool IstSAPFach(Zweig zweig)
        {
          // TODO: für Agrarzweig brauchen wir Bio als SAP-Fach, für den Sozialzweig als SA-Fach; vorläufige Lösung:
          if (zweig==Zweig.Agrar && Kuerzel == "B") return true;
          else return this.data.IstSAP;
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
        case "V": return Zweig.None; //Vorklasse FOS ohne Zweigzuordnung
        case "A": return Zweig.Agrar;
        default: throw new InvalidOperationException("unbekannter Zweig " + zweig);
      }
    }

    public static Jahrgangsstufe GetJahrgangsstufe(string jahrgangsstufe)
    {
      switch (jahrgangsstufe)
      {
        case "10": return Jahrgangsstufe.Vorklasse; // FOS Vorklasse
        case "11": return Jahrgangsstufe.Elf;
        case "12": return Jahrgangsstufe.Zwoelf;
        case "13": return Jahrgangsstufe.Dreizehn;
        default: throw new InvalidOperationException("unbekannte Jahrgangsstufe " + jahrgangsstufe);
      }
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
        case Zweig.Agrar: return "A";
        case Zweig.ALLE: return "ALLE";
        default: throw new InvalidOperationException("Unbekannter Zweig : " + zweig);
      }
    }

	}
}
