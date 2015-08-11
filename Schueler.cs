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
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="id">Die Id des Schülers in der Datenbank.</param>
        /// <param name="vorname">Vorname des Schülers.</param>
        /// <param name="nachname">Nachname des Schülers.</param>
        /// <param name="isLegastheniker">Ob der Schüler einen Legasthenie-Vermerk hat.</param>
        /// <param name="klasse">Die Klasse des Schülers.</param>
        /// 
        //
        private diNoDataSet.SchuelerRow data;
        private Klasse klasse;

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
                 
         this.Einzelnoten = new List<Note>();
         this.BerechneteNoten = new BerechneteNote();
         this.BerechneteNotenErstesHalbjahr = new BerechneteNote();
    }

    public Schueler(int id, string vorname, string nachname, bool isLegastheniker, string klasse) : this(id)
    {        
         this.Vorname = vorname;
         this.Nachname = nachname;
         this.IsLegastheniker = isLegastheniker;         
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
    public string Name
    {
      get
      {
        return this.Data.Name + ", " + this.Data.Rufname;
      }
    }

    /// <summary>
    /// Vorname des Schülers.
    /// </summary>
    public string Vorname
    {
      get;
      private set;
    }

    /// <summary>
    /// Nachnname des Schülers.
    /// </summary>
    public string Nachname
    {
      get;
      private set;
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
      get;
      set;
    }

    /// <summary>
    /// Die Klassenbezeichnung 
    /// </summary>
    public string Klasse
    {
      get
            {
                if (klasse == null) { klasse = new Klasse(this.data.KlasseId); }
                return klasse.Data.Bezeichnung;
            }
      
    }

    public diNoDataSet.SchuelerRow Data
    {            
            get { return this.data; }
    }
    /// <summary>
    /// Toes the string.
    /// </summary>
    /// <returns>String represantation of this.</returns>
    public override string ToString()
    {
      return this.Name;
    }
  }
}

