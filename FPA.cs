using diNo.diNoDataSetTableAdapters;
using System;

namespace diNo
{
  public static class FPA
  {
    // Berechnet die FPA-Gesamtpunktzahl aus den Teilnoten und speichert
    public static void Save(diNoDataSet.FpaDataTable FPANoten, Zweig zweig)
    {
      byte gespunkte = 0;
      byte gesanzahl = 0;
      foreach (diNoDataSet.FpaRow fpa in FPANoten)
      {
        // Vertiefungsnote berechnen
        if (!fpa.IsVertiefung1Null() && !fpa.IsVertiefung2Null())
        {
          if (zweig == Zweig.Sozial)
          {
            fpa.Vertiefung = (byte)Math.Round((2 * fpa.Vertiefung1 + fpa.Vertiefung2) / 3.0, MidpointRounding.AwayFromZero);
          }
          /*
          else if (Zweig == Zweig.Umwelt)
          {
            fpa.Vertiefung = (byte)Math.Round((fpa.Vertiefung1 + fpa.Vertiefung2) / 2.0, MidpointRounding.AwayFromZero);
          }*/
        }
        else if (zweig == Zweig.Sozial /*||Zweig == Zweig.Umwelt*/)
        {
          fpa.SetVertiefungNull();
        }

        // beim Betrieb werden nur mittlere Punktwerte vergeben:

        if (!fpa.IsBetriebNull() && fpa.Betrieb > 0 && (fpa.Betrieb + 1) % 3 != 0)
        {
          if ((fpa.Betrieb + 1) % 3 == 1)
            fpa.Betrieb--;
          else
            fpa.Betrieb++;
        }
        if (!fpa.IsBetriebNull() && !fpa.IsAnleitungNull() && !fpa.IsVertiefungNull())
        {
          if (fpa.Betrieb == 0 || fpa.Anleitung == 0 || fpa.Vertiefung == 0)
            fpa.Gesamt = 0;
          else
            fpa.Gesamt = (byte)Math.Round((2 * fpa.Betrieb + fpa.Anleitung + fpa.Vertiefung) / 4.0, MidpointRounding.AwayFromZero);

          gespunkte += fpa.Gesamt;
          gesanzahl++;

          // Jahrespunkte
          if (gesanzahl == 2) fpa.Jahrespunkte = (byte)Math.Round(gespunkte / 2.0, MidpointRounding.AwayFromZero);
        }
        else
        {
          fpa.SetGesamtNull();
        }
      }

      (new FpaTableAdapter()).Update(FPANoten);
    }
  }
}
