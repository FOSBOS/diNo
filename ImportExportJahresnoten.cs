using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.IO;

namespace diNo
{

  /// <summary>
  /// Exportiert die Halbjahresnoten aus der elften Klasse ins nächste Jahr.
  /// </summary>
  public class ImportExportJahresnoten
  {
    /// <summary>
    /// Trennzeichen für csv-Export als char und string
    /// string benötigt, da sonst glatt Id 55+';'=114 ausgerechnet wird...
    /// </summary>
    private static char SeparatorChar = ';';
    private static string Separator = "" + SeparatorChar;
    private static string FpAKennzeichen = "FpA";

    /// <summary>
    /// Methode exportiert alle Noten in eine csv-Datei
    /// Format: ID;Name;Fachkürzel;HJPunkte;HJPunkte2Dez;HJSchnittMdl;HJArt
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ExportiereHjLeistungen(string fileName)
    {
      using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(stream))
      {
        var schuelerAdapter = new SchuelerTableAdapter();
        foreach (var dbSchueler in schuelerAdapter.GetData())
        {
          Schueler schueler = new Schueler(dbSchueler);
          foreach (var fachNoten in schueler.getNoten.alleFaecher)
          {
            // Exportiert werden erst mal alle Halbjahres-Leistungen aller Schüler
            foreach (HjArt hjArt in Enum.GetValues(typeof(HjArt)))
            {
              HjLeistung note = fachNoten.getHjLeistung(hjArt);
              if (note != null)
              {
                // Erzeugt eine Zeile mit 9 durch ; getrennten Werten
                writer.WriteLine(schueler.Id + Separator + schueler.NameVorname + Separator + note.getFach.Kuerzel + Separator + (int)note.JgStufe + Separator + note.Punkte + Separator + note.Punkte2Dez + Separator + note.SchnittMdl + Separator + (byte)note.Art + Separator + (byte)note.Status);
              }
            }

            // und dann noch die fpA
            if (schueler.FPANoten != null && schueler.FPANoten.Count > 0)
            {
              foreach (var fpaZeile in schueler.FPANoten)
              {
                if (fpaZeile.IsGesamtNull())
                {
                  continue;
                }
                // jahrespunkte sind null bei den fpaNoten aus dem ersten Halbjahr
                // vertiefung1 und vertiefung2 sind nur bei den Sozialen gefüllt
                // bemerkung und stelle sind für uns auch nicht so wichtig und dürften null sein
                byte? jahresPunkte = fpaZeile.IsJahrespunkteNull() ? (byte?)null : fpaZeile.Jahrespunkte;
                string bemerkung = fpaZeile.IsBemerkungNull() ? null : fpaZeile.Bemerkung;
                byte? vertiefung1 = fpaZeile.IsVertiefung1Null() ? (byte?)null : fpaZeile.Vertiefung1;
                byte? vertiefung2 = fpaZeile.IsVertiefung2Null() ? (byte?)null : fpaZeile.Vertiefung2;
                string stelle = fpaZeile.IsStelleNull() ? null : fpaZeile.Stelle;
                {
                  // Erzeugt eine Zeile mit mind. 12 durch ; getrennten Werten (kann mehr sein, falls in der Bemerkung auch ;e enthalten sind)
                  writer.WriteLine(schueler.Id + Separator + schueler.NameVorname + Separator + FpAKennzeichen + Separator + fpaZeile.Gesamt + Separator + fpaZeile.Halbjahr + Separator + jahresPunkte + Separator + fpaZeile.Vertiefung + Separator + vertiefung1 + Separator + vertiefung2 + Separator + fpaZeile.Anleitung + Separator + fpaZeile.Betrieb + Separator + stelle + Separator + bemerkung);
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Importiert die Halbjahresleistungen der Schüler.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ImportierteHJLeistungen(string fileName)
    {
      HjLeistungTableAdapter ada = new HjLeistungTableAdapter();
      FachTableAdapter fta = new FachTableAdapter();
      FpaTableAdapter fpata = new FpaTableAdapter();
      using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(stream))
      {
        var schuelerAdapter = new SchuelerTableAdapter();
        while (!reader.EndOfStream)
        {
          string[] line = reader.ReadLine().Split(SeparatorChar);
          int schuelerId = int.Parse(line[0]);
          var schuelerGefunden = schuelerAdapter.GetDataById(schuelerId);
          if (schuelerGefunden == null || schuelerGefunden.Count == 0)
          {
            continue; // das ist normal. Schließlich sind auch die Halbjahresleistungen der letzten Absolventen noch mit in der Datei
          }

          var schueler = new Schueler(schuelerGefunden[0]);
          if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          {
            // nur bei Schülern in der zwölften Klasse wird irgendetwas importiert
            if (line.Length == 9)
            {
              // Zeile mit normaler Halbjahresleistung
              string nameVorname = line[1]; // nur zu Kontrollzwecken vorhanden
              int fachId = GetFachId(fta, line[2]);
              Jahrgangsstufe jgstufe = (Jahrgangsstufe)int.Parse(line[3]);
              byte note = byte.Parse(line[4]);
              decimal note2Dez = decimal.Parse(line[5]);
              decimal schnittMdl = decimal.Parse(line[6]);
              HjArt notenArt = (HjArt)byte.Parse(line[7]);
              HjStatus status = (HjStatus)byte.Parse(line[8]);

              // Importiere nur die Noten der elften Klasse
              if (jgstufe == Jahrgangsstufe.Elf)
              {
                ada.Insert(schueler.Id, fachId, (byte)notenArt, note, note2Dez, schnittMdl, (int)jgstufe, (byte)status);
              }
            }

            if (line.Length >= 13)
            {
              // FpaZeile
              string nameVorname = line[1]; // nur zu Kontrollzwecken vorhanden
              string fach = line[2];
              if (fach != FpAKennzeichen)
              {
                throw new InvalidDataException("FpA-Zeile zu SchülerId " + schuelerId + " nicht korrekt. Erwartet: " + FpAKennzeichen + " enthielt: " + fach);
              }

              byte gesamt = byte.Parse(line[3]);
              byte halbjahr = byte.Parse(line[4]);
              byte? jahrespunkte = line[5] == "null" ? (byte?)null : byte.Parse(line[5]); // Jahrespunkte können null sein
              byte vertiefung = byte.Parse(line[6]);
              byte vertiefung1 = byte.Parse(line[7]);
              byte vertiefung2 = byte.Parse(line[8]);
              byte anleitung = byte.Parse(line[9]);
              byte betrieb = byte.Parse(line[10]);
              string stelle = line[11] == "null" ? null : line[11];

              string bemerkung = null;
              if (line[12] != "null")
              {
                List<string> bemerkungen = new List<string>();
                for (int i = 12; i < line.Length; i++) // im Normalfall nur eine Bemerkung. Aber falls jemand einen ; in der Bemerkung hatte...
                {
                  bemerkungen.Add(line[i]);
                }

                bemerkung = string.Join(Separator, bemerkungen);
              }

              fpata.Insert(schuelerId, halbjahr, betrieb, anleitung, vertiefung, vertiefung1, vertiefung2, gesamt, stelle, bemerkung, jahrespunkte);
            }
          }
        }
      }
    }

    
    /// <summary>
    /// Sucht nach dem Fach in der Datenbank und liefert die Id zurück
    /// Wirft eine Exception wenn das Fach nicht gefunden werden kann
    /// </summary>
    /// <param name="fta">Fach Table Adapter.</param>
    /// <param name="fachKuerzel">Das eingelesene Fächerkürzel.</param>
    /// <returns>Die Id des Faches.</returns>
    private static int GetFachId(FachTableAdapter fta, string fachKuerzel)
    {
      var ergebnisFachSuche = fta.GetDataByKuerzel(fachKuerzel);
      if (ergebnisFachSuche == null || ergebnisFachSuche.Count == 0)
      {
        throw new InvalidDataException("ungültiges Fachkürzel in Halbjahresleistung " + fachKuerzel);
      }

      int fachId = ergebnisFachSuche[0].Id;
      return fachId;
    }
  }
}
