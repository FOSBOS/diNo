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
                 
/*       this.Einzelnoten = new List<Note>();
         this.BerechneteNoten = new BerechneteNote();
         this.BerechneteNotenErstesHalbjahr = new BerechneteNote();
*/
    }

    /// <summary>
    /// Die Id des Schülers in der Datenbank.
    /// </summary>
    public int Id
    {
      get;
      internal set;
    }

    /// <summary>
    /// Name und Rufname des Schülers, durch ", " getrennt.
    /// </summary>
    public string NameVorname
    {
      get
      {
        return this.Data.Name + ", " + this.Data.Rufname;
      }
    }

    /// <summary>
    /// Die Noten des Schülers.
    /// </summary>
    public IList<Note> Einzelnoten
    {
      get;
      private set;
    }

    /// <summary>
    /// Die berechneten Noten des Schülers.
    /// </summary>
    public BerechneteNote BerechneteNoten
    {
      get;
      private set;
    }

    /// <summary>
    /// Die berechneten Noten des Schülers.
    /// </summary>
    public BerechneteNote BerechneteNotenErstesHalbjahr
    {
      get;
      private set;
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
    public string Klasse
    {
      get
            {
                if (klasse == null)
                {
                    klasse = new Klasse(this.data.KlasseId);
                }
                return klasse.Data.Bezeichnung;
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
    
    public NotenProKurs NotenImKurs(int kursid)
        {
            return new NotenProKurs(this.Id, kursid);
        }


    /// <summary>
    /// Toes the string.
    /// </summary>
    /// <returns>String represantation of this.</returns>
    public override string ToString()
    {
      return this.Name;
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
                        summe -= 1; // Punktwert 0 wird als -1 gezählt
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

