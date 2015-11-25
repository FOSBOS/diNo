using System;

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
  }

  public enum Vorkommnisart
  {
    NotSet = 0,
    ProbezeitNichtBestanden = 1,
    VorrueckenAufProbe = 2,
    Notenausgleich = 3,
    Gefaehrdungsmitteilung = 4,
    JahrgangsstufeNichtBestanden = 5,
    NichtZurPruefungZugelassen = 6,
    PruefungSchriftlichNichtBestanden = 7,
    PruefungInsgesamtNichtBestanden = 8,
    Verweis = 9,
    SonstigeOrdnungsmaßnahme = 10
  }
}
