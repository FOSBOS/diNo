using System.IO;
using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using diNo.OmnisDB;

namespace diNo
{

  /// <summary>
  /// Exportiert die Jahresnoten aus der elften Klasse ins nächste Jahr. Exportiert wird jeweils Geschichte und
  /// T: Technisches Zeichnen
  /// W: Rechtslehre
  /// S: Chemie
  /// A: ---
  /// </summary>
  public class ImportExportJahresnoten
  {
    /// <summary>
    /// Trennzeichen für csv-Export als char und string
    /// string benötigt, da sonst glatt Id 55+';'=114 ausgerechnet wird...
    /// </summary>
    private static char SeparatorChar = ';';
    private static string Separator = "" + SeparatorChar;

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
                writer.WriteLine(schueler.Id + Separator + schueler.NameVorname + Separator + note.getFach.Kuerzel + Separator + note.Punkte + Separator + note.Punkte2Dez + Separator + note.SchnittMdl + Separator + note.Art);
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
            continue;
          }

          var schueler = new Schueler(schuelerGefunden[0]);
          if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          {
            // nur bei Schülern in der zwölften Klasse wird irgendetwas importiert
            if (line.Length == 7)
            {
              string nameVorname = line[1];
              int fachId = GetFachId(fta, line[2]);

              byte note = byte.Parse(line[3]);
              decimal note2Dez = decimal.Parse(line[4]);
              decimal schnittMdl = decimal.Parse(line[5]);
              HjArt notenArt = (HjArt)byte.Parse(line[6]);

              // Importiere nur die Noten der elften Klasse (ggf. bei Wiederholern ein Fachreferat)
              /*
              if (schueler.Wiederholt()) //  FR darf nicht übernommen werden!
              {
                if (notenArt == HjArt.FR || notenArt == HjArt.VorHj1 || notenArt == HjArt.VorHj1)
                {
                  // Bei Wiederholern stehen diese bereits als Vorjahresleistungen (HJArt.VorHj1 = 5 bzw. HJArt.VorHj2 = 6) drin. Importiere nur diese und Fachreferat.
                  ada.Insert(schueler.Id, fachId, (byte)notenArt, note, false, note2Dez, schnittMdl);
                }
              }
              else */
              {
                // Bei normal aufgerückten Schülern stehen sie als aktuelle Leistung (Hj1 = 0, Hj2 = 1) in der Datei, müssen aber zum VorHJ gemacht werden
                if (notenArt == HjArt.Hj1 || notenArt == HjArt.Hj2)
                {
                  ada.Insert(schueler.Id, fachId, (byte)ConvertHjArt(notenArt), note, false, note2Dez, schnittMdl,11);
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Macht aus einem aktuellen Halbjahr die passende Vorjahresnote
    /// </summary>
    /// <param name="art">Art der Note (vorher: aktuelles Halbjahr)</param>
    /// <returns>Art der Note (nachher: Vor-Halbjahr)</returns>
    private static HjArt ConvertHjArt(HjArt art)
    {
      switch (art)
      {
        case HjArt.Hj1: return HjArt.VorHj1;
        case HjArt.Hj2: return HjArt.VorHj2;
        default: throw new InvalidOperationException("Halbjahres-Noten vom Typ "+art+" können nicht ins nächste Jahr übertragen werden");
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
