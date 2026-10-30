using diNo.diNoDataSetTableAdapters;
using System;

namespace diNo
{
  /// <summary>
  /// Eine Ausleihe (Buchentleihung) eines Schülers: Verknüpfung Schüler/Buch mit Ausgabe- und
  /// ggf. Rückgabedatum. RueckgabeAm == null bedeutet: das Buch ist noch nicht zurückgegeben.
  /// </summary>
  public class BuchAusleihe
  {
    private diNoDataSet.SchuelerBuecherRow data;

    public BuchAusleihe(diNoDataSet.SchuelerBuecherRow data)
    {
      this.data = data;
    }

    public int Id => data.Id;

    public int SchuelerId => data.SchuelerId;

    public int BuchId => data.BuchId;

    public Buch Buch => Buecher.Instance.Find(data.BuchId);

    public DateTime AusgegebenAm => data.AusgegebenAm;

    public DateTime? RueckgabeAm => data.IsRueckgabeAmNull() ? (DateTime?)null : data.RueckgabeAm;

    public bool IstAktiv => data.IsRueckgabeAmNull();

    /// <summary>
    /// Markiert diese einzelne Ausleihe als zurückgegeben.
    /// </summary>
    public void Zurueckgeben(DateTime? datum = null)
    {
      data.RueckgabeAm = datum ?? DateTime.Today;
      new SchuelerBuecherTableAdapter().Update(data);
    }

    /// <summary>
    /// Macht eine Rückgabe rückgängig: die Ausleihe gilt wieder als aktiv.
    /// </summary>
    public void RueckgabeRueckgaengig()
    {
      data.SetRueckgabeAmNull();
      new SchuelerBuecherTableAdapter().Update(data);
    }
  }
}
