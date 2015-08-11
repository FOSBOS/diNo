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

		private class Jahrgang
		{
			public Jahrgang()
			{
				this.Faecher = new List<KeyValuePair<diNo.diNoDataSet.FachRow, Schulaufgabenwertung>>();
			}

			public Zweig Zweig
			{
				get;
				set;
			}

			public Jahrgangsstufe Jahrgangsstufe
			{
				get;
				set;
			}

			public Schulart Schulart
			{
				get;
				set;
			}

      public IList<KeyValuePair<diNo.diNoDataSet.FachRow, Schulaufgabenwertung>> Faecher
			{
				get;
				private set;
			}
		}

    public static Schulaufgabenwertung GetSchulaufgabenwertung(diNo.diNoDataSet.FachRow fach, Jahrgangsstufe jahrgang, Zweig zweig, Schulart schulart)
		{
      using (SchulaufgabenfachTableAdapter ada = new SchulaufgabenfachTableAdapter())
      {
        int schulaufgabenzahl = 0;

        var wertung = ada.GetDataByAllInfos(schulart.ToString(), jahrgang.ToString(), zweig.ToString(), fach.Id);
        if (wertung.Count > 0)
        {
          schulaufgabenzahl = wertung[0].AnzahlSA;
        }

        wertung = ada.GetDataByAllInfos("ALLE", jahrgang.ToString(), zweig.ToString(), fach.Id);
        if (wertung.Count > 0)
        {
          schulaufgabenzahl = wertung[0].AnzahlSA;
        }

        wertung = ada.GetDataByAllInfos("ALLE", jahrgang.ToString(), "ALLE", fach.Id);
        if (wertung.Count > 0)
        {
          schulaufgabenzahl = wertung[0].AnzahlSA;
        }

        wertung = ada.GetDataByAllInfos(schulart.ToString(), jahrgang.ToString(), "ALLE", fach.Id);
        if (wertung.Count > 0)
        {
          schulaufgabenzahl = wertung[0].AnzahlSA;
        }

        wertung = ada.GetDataByAllInfos("ALLE", "ALLE", zweig.ToString(), fach.Id);
        if (wertung.Count > 0)
        {
          schulaufgabenzahl = wertung[0].AnzahlSA;
        }

        wertung = ada.GetDataByAllInfos("ALLE", "ALLE", "ALLE", fach.Id);
        if (wertung.Count > 0)
        {
          schulaufgabenzahl = wertung[0].AnzahlSA;
        }

        if (schulaufgabenzahl == 1 || schulaufgabenzahl == 2)
        {
          return Schulaufgabenwertung.EinsZuEins;
        }
        else if (schulaufgabenzahl > 2)
        {
          return Schulaufgabenwertung.ZweiZuEins;
        }
        else
        {
          log.InfoFormat("keine Schulaufgabenwertung gefunden für: Schulart={0}, Jahrgang={1}, Zweig={2}, Fach={3}. Gehe von Kurzarbeiten und Exen aus.", schulart.ToString(), jahrgang.ToString(), zweig.ToString(), fach.Kuerzel);
          return Schulaufgabenwertung.KurzarbeitenUndExen;
        }
      }
		}

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

    public static Schulaufgabenwertung GetSchulaufgabenwertung(string fachKuerzel, string klasse)
    {
      diNo.diNoDataSet.FachRow fach = FindFachByKuerzel(fachKuerzel);
      Jahrgangsstufe jahrgangsstufe = GetJahrgangsstufeFromKlasse(klasse);
      Zweig zweig = GetZweigFromKlasse(klasse);
      Schulart schulart = GetSchulartFromKlasse(klasse);
      return GetSchulaufgabenwertung(fach, jahrgangsstufe, zweig, schulart);
    }

    public static Jahrgangsstufe GetJahrgangsstufeFromKlasse(string klasse)
    {


      if (klasse.Contains("Vk") || klasse.Contains("vk") || klasse.Contains("Vs"))
      {
        return Jahrgangsstufe.Vorklasse;
      }

      if (klasse.Contains("11"))
      {
        return Jahrgangsstufe.Elf;
      }

      if (klasse.Contains("12"))
      {
        return Jahrgangsstufe.Zwoelf;
      }

      if (klasse.Contains("13"))
      {
        return Jahrgangsstufe.Dreizehn;
      }

      throw new InvalidOperationException("Jahrgangsstufe nicht gefunden: "+klasse);
    }

    public static Zweig GetZweigFromKlasse(string klasse)
    {
      if (klasse.Contains("W") || klasse.Contains("w"))
      {
        return Zweig.Wirtschaft;
      }

      if (klasse.Contains("S") || klasse.Contains("s"))
      {
        return Zweig.Sozial;
      }

      if (klasse.Contains("T") || klasse.Contains("t"))
      {
        return Zweig.Technik;
      }

      throw new InvalidOperationException("Zweig nicht gefunden: " + klasse);
    }

    public static Schulart GetSchulartFromKlasse(string klasse)
    {
      if (klasse.StartsWith("F") || klasse.StartsWith("f"))
      {
        return Schulart.FOS;
      }

      if (klasse.StartsWith("B") || klasse.StartsWith("b"))
      {
        return Schulart.BOS;
      }

      throw new InvalidOperationException("Schulart nicht gefunden: "+klasse);
    }

    public static diNo.diNoDataSet.FachRow FindFachByKuerzel(string kuerzel)
    {
      var result = new FachTableAdapter().GetDataByKuerzel(kuerzel);
      if (result.Count == 0)
      {
        throw new InvalidOperationException("fach nicht gefunden: "+kuerzel);
      }

      return result[0];
    }
	}
}
