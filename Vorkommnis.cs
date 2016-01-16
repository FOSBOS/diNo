using System;
using System.Collections.Generic;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  /// <summary>
  /// Klasse kapselt ein Vorkommnis für einen Schüler.
  /// </summary>
  public class Vorkommnis
  {
    private diNoDataSet.VorkommnisRow data;

    public Vorkommnis(int id)
    {
      var ada = new diNoDataSetTableAdapters.VorkommnisTableAdapter();
      var result = ada.GetDataById(id);
      if (result.Count != 1)
      {
        throw new InvalidOperationException("ungültige VorkommnisId " + id);
      }

      this.data = result[0];
    }

    public Vorkommnis(diNoDataSet.VorkommnisRow data)
    {
      this.data = data;
    }

    public int Id
    {
      get { return this.data.Id; }
    }

    public DateTime Datum
    {
      get { return this.data.Datum; }
    }

    public string Bemerkung
    {
      get { return this.data.Bemerkung; }
    }

    public Vorkommnisart Art
    {
      get { return (Vorkommnisart)this.data.Art; }
    }

    public string ArtDruck
    {
      get {return Vorkommnisse.Instance.VorkommnisText((Vorkommnisart)data.Art); }
    }
  }

  public enum Vorkommnisart
  {
    NotSet = 0,
    ProbezeitNichtBestanden = 1,
    VorrueckenAufProbe = 2,
    Notenausgleich = 3,
    RuecktrittVorklasse = 4,
    JahrgangsstufeNichtBestanden = 5,
    NichtZurPruefungZugelassen = 6,
    PruefungSchriftlichNichtBestanden = 7,
    PruefungInsgesamtNichtBestanden = 8,
    Verweis = 9,
    SonstigeOrdnungsmaßnahme = 10,
    KeineVorrueckungserlaubnis = 11,
    Gefaehrdungsmitteilung = 12,
    starkeGefaehrdungsmitteilung = 13,
    BeiWeiteremAbsinken = 14,
    GefahrDerAbweisung = 15,
    DarfNichtMehrWiederholen = 16,
    Zwischenzeugnis = 17,
    keinZwischenzeugnis = 18,
    Jahreszeugnis = 19,
    keinJahreszeugnis = 20,
    Fachabiturzeugnis = 21,
    fachgebundeneHochschulreife = 22,
    allgemeineHochschulreife = 23,
    EnglischNiveauB2 = 24,
    Bemerkung = 25,
    FranzNurBisSAP = 26
  }

  public class Vorkommnisse
  {  
    private static Vorkommnisse _Instance=null;
    public Dictionary<Vorkommnisart, string> Liste;

    public static Vorkommnisse Instance {
        get {
          if (_Instance == null) {
            _Instance = new Vorkommnisse();
          }
          return _Instance;
        }
      }
    
    public Vorkommnisse()
    {      
      Liste = new Dictionary<Vorkommnisart, string>();
      var dt = (new VorkommnisartTableAdapter()).GetData();
      foreach (var v in dt)
      {        
        Liste.Add((Vorkommnisart)v.Id,v.Bezeichnung);
      }
    }

    public string VorkommnisText(Vorkommnisart v)
    {
      return Liste[v];
    }
  }

}
