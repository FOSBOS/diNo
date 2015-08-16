using System;
using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;

namespace diNo
{
	public enum Jahrgangsstufe
	{
		None = 0,
		Vorklasse = 1,
		Elf = 2,
		Zwoelf = 3,
		Dreizehn = 4,
        Vorkurs = 5,
    ALLE = 15
	}

	public enum Schulart
	{
 		None = 0,
		FOS = 1,
		BOS = 2,
    ALLE = 15
	}

	public enum Zweig
	{
		None = 0,
		Sozial = 1,
		Technik = 2,
		Wirtschaft = 3,
    Agrar = 4,
    ALLE = 15
	}

    public class Klasse
    {
        private diNoDataSet.KlasseRow data;

        public Klasse(int id)
        {
            var rst = new KlasseTableAdapter().GetDataById(id);
            if (rst.Count == 1)
            {
                this.data = rst[0];
            }
            else
            {
                throw new InvalidOperationException("Konstruktor Klasse: Ungültige ID.");
            }

        }

        public diNoDataSet.KlasseRow Data
        {
            get { return this.data; }
        }

        public Jahrgangsstufe Jahrgangsstufe
        {
            get
            {
                string klasse = data.Bezeichnung;
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

                throw new InvalidOperationException("Jahrgangsstufe nicht gefunden: " + klasse);
            }
        }

        public Schulart Schulart
        {
            get
            {
                string klasse = data.Bezeichnung;
                if (klasse.StartsWith("F") || klasse.StartsWith("f"))
                {
                    return Schulart.FOS;
                }

                if (klasse.StartsWith("B") || klasse.StartsWith("b"))
                {
                    return Schulart.BOS;
                }

                throw new InvalidOperationException("Schulart nicht gefunden: " + klasse);
            }
        }


        public Zweig Zweig
        {
            get
            {
                string klasse = data.Bezeichnung;
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
        }
    }

   
    /// <summary>
    /// Ein Kurs.
    /// </summary>
    public class Kurs
  {
    private diNoDataSet.KursRow data;
    private diNoDataSet.SchuelerDataTable schueler;
    private diNoDataSet.FachRow fach;
    private diNoDataSet.LehrerRow lehrer;


        public Kurs(int id)
        {
            this.Id = id;
            var rst = new KursTableAdapter().GetDataById(id);
            if (rst.Count == 1)
            {
                this.data = rst[0];
            }
            else
            {
                throw new InvalidOperationException("Konstruktor Kurs: Ungültige ID.");
            }
        }
    /// <summary>
    /// Id des Kurses in der Datenbank.
    /// </summary>
    public int Id
    {
      get;
      private set;
    }

    public diNoDataSet.KursRow Data
        {
            get { return data; }           
        }

    /// <summary>
    /// Die Liste der Schüler dieser Kurses (sortiert via SQL)
    /// </summary>
    public diNoDataSet.SchuelerDataTable getSchueler
    {
      get   {
                if (schueler == null)
                {
                    SchuelerTableAdapter sa = new SchuelerTableAdapter();
                    schueler = sa.GetDataByKursId(Id);
                }
                return schueler;
            }
    }

    public diNoDataSet.FachRow getFach
        {
            get
            {
                if (fach == null)
                {
                    fach = new FachTableAdapter().GetDataById(data.FachId)[0];
                }
                return fach;

                // return data.FachRow; so sollte es eigentlich gehen
            }
        }

        public diNoDataSet.LehrerRow getLehrer
        {
            get
            {
                if (lehrer == null)
                {
                    lehrer = new LehrerTableAdapter().GetDataById(data.LehrerId)[0];
                }
                return lehrer;

                // return data.LehrerRow; so sollte es eigentlich gehen
            }
        }

        public string FachBezeichnung
        {
            get { return this.getFach.Bezeichnung; }        
        }

        // Ermittelt die SA-Wertung für diesen Kurs
        // todo: strings durch Schlüssel ersetzen
        public Schulaufgabenwertung GetSchulaufgabenwertung(Klasse klasse)
        {            
            Jahrgangsstufe jahrgang = klasse.Jahrgangsstufe;
            //Zweig zweig = klasse.Zweig;
            //Schulart schulart = klasse.Schulart;
            
            // Prüfungsfächer haben immer SA (Vorklasse und 12. 3, sonst 2)
            if (fach.IstSAP)
            {
                if (klasse.Jahrgangsstufe == Jahrgangsstufe.Vorklasse || klasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
                    return Schulaufgabenwertung.ZweiZuEins;
                else
                    return Schulaufgabenwertung.EinsZuEins;
            }
            else
            {
                if (fach.Kuerzel == "TeIn" || fach.Kuerzel == "VWL" || fach.Kuerzel == "B" )
                    return Schulaufgabenwertung.EinsZuEins;
                else
                    return Schulaufgabenwertung.KurzarbeitenUndExen;
            }

            /* oder via SA-Zahl,  Problem ist ToString geht grad nicht; ToDo: Strings durch Ids ersetzen
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
            */
        }
    }
}
