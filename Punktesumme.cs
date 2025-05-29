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

    public void BerechneGesamtpunktzahl(int deltaP)
    {
      if (!schueler.Seminarfachnote.IsGesamtnoteNull())
      {
        Add(PunktesummeArt.Seminar,(byte)schueler.Seminarfachnote.Gesamtnote,2);
      }

      int anz = 0, sum = 0;
      for (int i = 0; i < 5; i++) // bis Seminar
      {
        if (anzahl[i] > 0)
        {
          anz += anzahl[i];
          sum += summe[i];          
        }
      }
      
      // für Ergänzungsprüfung oder F-f in 12. Klasse:
      if (anzahl[(int)PunktesummeArt.FremdspracheErgPr] > 0 || anzahl[(int)PunktesummeArt.FremdspracheAus12] > 0)
      {
        anzahl[(int)PunktesummeArt.GesamtFachgebHSR] = anz;
        summe[(int)PunktesummeArt.GesamtFachgebHSR] = sum;

        anz += anzahl[(int)PunktesummeArt.FremdspracheErgPr] + anzahl[(int)PunktesummeArt.FremdspracheAus12];
        sum += summe[(int)PunktesummeArt.FremdspracheErgPr] + summe[(int)PunktesummeArt.FremdspracheAus12];
      }

      anzahl[(int)PunktesummeArt.Gesamt] = anz;
      summe[(int)PunktesummeArt.Gesamt] = sum;

      if (deltaP > 0) // schließt hoffentlich den obigen Fall mit Fremdsprache > 0 aus!
      {
        anzahl[(int)PunktesummeArt.GesamtFachgebHSR] = anz;
        summe[(int)PunktesummeArt.GesamtFachgebHSR] = sum + deltaP;
      }
    }

    // deltaP gibt an, um wieviel höher die Punktesumme bei der fachgeb. HSR liegt, wenn eine alternative HjLeistung eingebracht wird
    public void WriteToDB(int deltaP=0)
    {
      BerechneGesamtpunktzahl(deltaP);
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
    public void Add(PunktesummeArt art, int punkte, int faktor=1)
    {
      anzahl[(int)art] +=faktor;
      summe[(int)art] += punkte * faktor;
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
        case PunktesummeArt.Seminar: return "Seminar";
        case PunktesummeArt.FremdspracheErgPr: return "Ergänzungsprüfung";
        case PunktesummeArt.FremdspracheAus12:return "Fremdsprache der 12. Klasse";
        case PunktesummeArt.Gesamt: return "Summe";
        case PunktesummeArt.GesamtFachgebHSR: return "Summe (fachgeb. HSR)";
        default: return "";
      }
    }
  }

  public class Fachsumme
  {
    public int anz, sum;

    public void SaveGesErg(HjLeistung gesErg)
    {
      if (anz == 0) // z.B. weil schon beide HjLeistungen ungültig waren
      {
        gesErg.SetStatus(HjStatus.Ungueltig);
        return;
      }

      gesErg.Punkte2Dez = sum / (decimal)anz;
      gesErg.Punkte = GesErg();
      gesErg.Status = HjStatus.None;
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
    FremdspracheErgPr,
    FremdspracheAus12,
    Gesamt,
    GesamtFachgebHSR
  }


}
