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

        public Klasse (int id)
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
    /// Die Liste der Schüler dieser Klasse (sortiert via SQL)
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
  }
}
