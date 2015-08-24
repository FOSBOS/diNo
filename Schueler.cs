using System;
using System.Collections.Generic;
using System.Drawing;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  /// <summary>
  /// Ein Schüler.
  /// </summary>
  public class Schueler
  {
      
        private diNoDataSet.SchuelerRow data;   // nimmt SchülerRecordset auf
        private Klasse klasse;                  // Objektverweis zur Klasse dieses Schülers
        private diNoDataSet.KursDataTable kurse; // Recordset-Menge aller Kurse dieses Schülers
        private SchuelerNoten noten;            // verwaltet alle Noten dieses Schülers

    public Schueler(int id)
    {         
         this.Id = id; 
         var rst = new SchuelerTableAdapter().GetDataById(id);
         if (rst.Count == 1)
            {
                this.data = rst[0];
            }
         else
            {
                throw new InvalidOperationException("Konstruktor Schueler: Ungültige ID.");
            }       
    }

    public Schueler(diNoDataSet.SchuelerRow s)
    {
        this.Id = s.Id;
        this.data = s;
    }

        /// <summary>
        /// Die Id des Schülers in der Datenbank.
        /// </summary>
        public int Id
    {
      get;
      internal set;
    }

     public string benutzterVorname
        {
            get { return string.IsNullOrEmpty(data.Rufname) ? data.Vorname : data.Rufname; }
        }

    /// <summary>
    /// Name und Rufname des Schülers, durch ", " getrennt.
    /// </summary>
    public string NameVorname
    {
      get
      {
        return this.Data.Name + ", " + benutzterVorname;
      }
    }

     /// <summary>
    /// Ob der Schüler Legastheniker ist.
    /// </summary>
    public bool IsLegastheniker
    {
      get { return this.data.LRSStoerung || this.data.LRSSchwaeche;  }     
    }

    /// <summary>
    /// Die Klassenbezeichnung 
    /// </summary>
    public Klasse getKlasse
    {
      get
            {
                if (klasse == null)
                {
                    klasse = new Klasse(this.data.KlasseId);
                }
                return klasse;
            }
      
    }

    /// <summary>
    /// Alle Noten (je Fach/Kurs) dieses Schülers
    /// </summary>
    public SchuelerNoten getNoten
    {
        get
        {
            if (noten == null)
            {
                noten = new SchuelerNoten(this);
            }
            return noten;
        }
    }

    public diNoDataSet.SchuelerRow Data
    {            
            get { return this.data; }
    }

    public diNoDataSet.KursDataTable Kurse
        {
            get
            {
                if (kurse == null)
                {
                    kurse = new KursTableAdapter().GetDataBySchulerId(this.Id);
                }
                return kurse;
            }
        }
    
 
    public double berechneDNote()
        {
            int summe=0, anz=0;
            double erg;
            var faecher = new BerechneteNoteTableAdapter().GetDataBySchueler4DNote(this.Id);
            foreach (var fach in faecher)
            {
                if ( true /*!fach.KursRow.FachRow.Kuerzel in ['F','Ku','Sp']*/)
                {
                    if (fach.Abschlusszeugnis == 0)
                    {
                        summe--; // Punktwert 0 wird als -1 gezählt
                    }
                    else
                    {
                        summe += fach.Abschlusszeugnis;
                    }
                    
                    anz++;
                }                
            }
            if (anz > 0)
                {
                    erg = (17 - summe / anz) / 3;
                    if (erg<1)
                    {
                        erg = 1;
                    }
                    else
                    {
                        erg = Math.Floor(erg * 10) / 10; // auf 1 NK abrunden
                    }
                }
            else
            {
                erg = 0;
            }

            return erg;
        }
  }
}

