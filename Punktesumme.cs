using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static diNo.diNoDataSet;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  // verwaltet das Datenbankobjekt, das eine Punktesumme speichert (z.B. Summe aus AP, oder aus den eingebrachten HjLeistungen)
  public class Punktesumme
  {
    Schueler schueler;
    // Anzahl und Summe der eingebrachten Punkte (aufgeteilt nach bestimmten Kategorien)
    private int[] anzahl = new int[Enum.GetValues(typeof(PunktesummeArt)).Length];
    private int[] summe = new int[Enum.GetValues(typeof(PunktesummeArt)).Length];
    private PunktesummeDataTable dt;

    public Punktesumme(Schueler s)
    {
      schueler = s;
      var ta = new PunktesummeTableAdapter();
      dt = ta.GetDataBySchuelerId(s.Id);
      foreach (var r in dt)
      {
        anzahl[r.Art] = r.Anzahl;
        summe[r.Art] = r.Punktzahl;
      }
    }

    public void Clear()
    {
      anzahl = new int[Enum.GetValues(typeof(PunktesummeArt)).Length];
      summe = new int[Enum.GetValues(typeof(PunktesummeArt)).Length];      
    }

    public void BerechneGesamtpunktzahl()
    {
      int anz = 0, sum = 0;
      for (int i = 0; i < 5; i++) // bis Seminar
      {
        if (anzahl[i] > 0)
        {
          anz += anzahl[i];
          sum += summe[i];          
        }
      }
      
      if (anzahl[(int)PunktesummeArt.Fremdsprache] > 0)
      {
        anzahl[(int)PunktesummeArt.GesamtFachgebHSR] = anz;
        summe[(int)PunktesummeArt.GesamtFachgebHSR] = sum;

        anz += anzahl[(int)PunktesummeArt.Fremdsprache];
        sum += summe[(int)PunktesummeArt.Fremdsprache];
      }

      anzahl[(int)PunktesummeArt.Gesamt] = anz;
      summe[(int)PunktesummeArt.Gesamt] = sum;
    }

    public void WriteToDB()
    {
      BerechneGesamtpunktzahl();
      var ta = new PunktesummeTableAdapter();
      ta.DeleteBySchuelerId(schueler.Id);
      for (int i=0; i < Enum.GetValues(typeof(PunktesummeArt)).Length; i++)
      {
        if (anzahl[i] > 0)
          ta.Insert(schueler.Id, i, summe[i], anzahl[i]);
      }
    }

    public void Add(PunktesummeArt art, Fachsumme fs)
    {
      anzahl[(int)art] += fs.anz;
      summe[(int)art] += fs.sum;
    }

    // Addition eines Einzelwertes
    public void Add(PunktesummeArt art, int punkte)
    {
      anzahl[(int)art] ++;
      summe[(int)art] += punkte;
    }

    public int Anzahl(PunktesummeArt art)
    {
      return anzahl[(int)art];
    }

    public int Summe(PunktesummeArt art)
    {
      return summe[(int)art];
    }

    public static string ArtToText(PunktesummeArt art)
    {
      switch (art)
      {
        case PunktesummeArt.AP: return "Abschlussprüfung";
        case PunktesummeArt.FPA: return "FpA";
        case PunktesummeArt.FR: return "Fachreferat";
        case PunktesummeArt.HjLeistungen: return "Halbjahresleistungen";
        case PunktesummeArt.Gesamt: return "Summe";
        default: return "";
      }
    }
  }

  public class Fachsumme
  {
    public int anz, sum;

    public void SaveGesErg(HjLeistung gesErg)
    {
      if (anz == 0) return; // nichts speichern

      gesErg.Punkte2Dez = sum / (decimal)anz;
      gesErg.Punkte = GesErg();
      gesErg.WriteToDB();
    }

    public byte GesErg()
    {
      decimal Punkte2Dez = sum / (decimal)anz;
      if (Punkte2Dez < (decimal)1.0) return 0;
      return (byte)Math.Round((double)Punkte2Dez, MidpointRounding.AwayFromZero);
    }

    // Addiert eine andere Fachsumme dazu
    public void Add(Fachsumme f)
    {
      anz += f.anz;
      sum += f.sum;
    }

    public void Add(int punkte, int faktor)
    {
      anz += faktor;
      sum += punkte * faktor;
    }
  }


  public enum PunktesummeArt
  {
    AP,
    HjLeistungen,
    FR,
    FPA,
    Seminar,
    Fremdsprache,
    Gesamt,
    GesamtFachgebHSR
  }


}
