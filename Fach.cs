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
    }


    ///  <summary>
    /// Klasse Fächerkanon dient zur Abfrage der Modalitäten (1:1, 2:1-Fach oder KA/Ex)
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
