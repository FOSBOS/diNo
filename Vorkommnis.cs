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
    KeineVorrueckungserlaubnis = 5,
    NichtZurPruefungZugelassen = 6,
    bisherNichtBestandenMAPmoeglich = 7,
    nichtBestandenMAPnichtZugelassen = 8,
    Verweis = 9,
    SonstigeOrdnungsmaßnahme = 10,    
    Gefaehrdungsmitteilung = 11,
    starkeGefaehrdungsmitteilung = 12,
    BeiWeiteremAbsinken = 13,
    GefahrDerAbweisung = 14,
    DarfNichtMehrWiederholen = 15,
    Zwischenzeugnis = 16,
    keinZwischenzeugnis = 17,
    Jahreszeugnis = 18,
    keinJahreszeugnis = 19,
    Fachabiturzeugnis = 20,
    fachgebundeneHochschulreife = 21,
    allgemeineHochschulreife = 22,
    EnglischNiveauB2 = 23,
    Bemerkung = 24,
    FranzNurBisSAP = 25,
    endgueltigNichtBestanden = 26,
    nichtZurMAPangetreten = 27,
    VorrueckenBOS13moeglich = 28
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
      Liste.Add(Vorkommnisart.NotSet,"");
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
