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

    // Probezeit und Gefährdungen
    ProbezeitNichtBestanden = 11,
    BeiWeiteremAbsinken = 12,
    Gefaehrdungsmitteilung = 13,
    starkeGefaehrdungsmitteilung = 14,
    GefahrDerAbweisung = 15,
    RuecktrittVorklasse = 16,
    ProbezeitVerlaengert = 17,
    KeineProbezeitNaechstesSJ = 18,

    // 1.-3. PA
    NichtZurPruefungZugelassen = 21,
    bisherNichtBestandenMAPmoeglich = 22,
    nichtBestandenMAPnichtZugelassen = 23,
    nichtZurMAPangetreten = 24,
    VorrueckenBOS13moeglich = 25,
    PruefungNichtBestanden = 26,
    PruefungAbgebrochen = 27,
    ErhaeltNachtermin = 28,

    // Jahresende
    NichtBestanden = 31,
    KeineVorrueckungserlaubnis = 32,
    DarfNichtMehrWiederholen = 33,
    Notenausgleich = 34,
    VorrueckenAufProbe = 35,

    // Zeugnisse
    Zwischenzeugnis = 41,
    keinZwischenzeugnis = 42,
    Jahreszeugnis = 43,
    keinJahreszeugnis = 44,
    Fachabiturzeugnis = 45,
    fachgebundeneHochschulreife = 46,
    allgemeineHochschulreife = 47,
    EnglischNiveauB2 = 48,
    MittlereReife = 49,

    // Ordnungsmaßnahmen
    Verweis = 51,
    verschaerfterVerweis = 52,
    Nacharbeit = 53,
    SonstigeOrdnungsmaßnahme = 59,

    // Sonstiges
    Bemerkung = 61,
    FranzNurBisSAP = 62,
    Sportbefreiung = 63,
    Attestpflicht = 64
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
