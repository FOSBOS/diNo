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
    /// Format: ID;Name;Fachkürzel;Lehrerkürzel;Zeugnisnote
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ExportiereNoten(string fileName)
    {
      using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(stream))
      {
        var schuelerAdapter = new SchuelerTableAdapter();
        foreach (var dbSchueler in schuelerAdapter.GetData())
        {
          Schueler schueler = new Schueler(dbSchueler);
          foreach (var fachNote in schueler.getNoten.alleKurse)
          {
            var fachKuerzel = fachNote.getFach.Kuerzel;

            if (IsExportNecessary(fachKuerzel, schueler))
            {
              var lehrer = Zugriff.Instance.KursRep.Find(fachNote.kursId).getLehrer;
              string lehrerKuerzel = lehrer == null ? "" : lehrer.Kuerzel;
              var noteImFach = schueler.getNoten.getFach(fachNote.kursId);
              var note = noteImFach.getRelevanteNote(Zeitpunkt.Jahresende);
              writer.WriteLine(schueler.Id +""+ Separator + schueler.Name + Separator + noteImFach.getFach.Kuerzel + Separator + lehrerKuerzel + Separator + note);

            }
          }

          if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && schueler.getKlasse.Schulart == Schulart.FOS)
          {
            var fpa = schueler.FPANoten;
            if (!fpa.IsErfolgNull())
            {
              writer.WriteLine(schueler.Id + Separator + schueler.Name + Separator + fpa.Erfolg);
            }
          }
        }
      }
    }

    /// <summary>
    /// Methode prüft ob ein Export nötig ist.
    /// </summary>
    /// <param name="fachKuerzel">Fachkürzel.</param>
    /// <param name="schueler">Der Schüler.</param>
    /// <returns>Ob für diesen Schüler die Note des genannten Faches exportiert werden soll.</returns>
    private static bool IsExportNecessary(string fachKuerzel, Schueler schueler)
    {
      // Momentan exportieren wir nur die Noten der FOS in den Fächern, die dort abgelegt werden.
      if (schueler.getKlasse.Schulart == Schulart.FOS)
      {
        return fachKuerzel == "G" ||
          (schueler.Data.Ausbildungsrichtung == "W" && fachKuerzel == "Rl") ||
          (schueler.Data.Ausbildungsrichtung == "S" && fachKuerzel == "C") ||
          (schueler.Data.Ausbildungsrichtung == "T" && fachKuerzel == "TZ");
      }
      else return false;
    }

    /// <summary>
    /// Methode importiert die Zeugnisnoten des Vorjahres.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ImportiereNoten(string fileName)
    {
      using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(stream))
      {
        var schuelerAdapter = new SchuelerTableAdapter();
        var fpaAdapter = new FpANotenTableAdapter();
        Dictionary<string, Kurs> kurse = GetKursverzeichnis();
        kurse.Add("G", GetGeschichteKurs());

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
            if (line.Length == 5)
            {
              //notenzeile
              string nachname = line[1];
              string fachKuerzel = line[2];
              string lehrerKuerzel = line[3];

              if (string.IsNullOrEmpty(line[4])) // was das heißt ist aber auch fraglich. keine Note?
              {
                continue;
              }

              byte zeugnisnote = byte.Parse(line[4]);

              if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
              {
                schueler.MeldeAn(kurse[fachKuerzel.ToUpper()]);
                BerechneteNote bnote = new BerechneteNote(kurse[fachKuerzel.ToUpper()].Id, schueler.Id);
                bnote.ErstesHalbjahr = false;
                bnote.JahresfortgangGanzzahlig = zeugnisnote;
                bnote.Abschlusszeugnis = zeugnisnote;
                bnote.writeToDB();
              }
            }
            else if (line.Length == 3)
            {
              //FpA-Zeile
              string nachname = line[1];
              int gesamterfolg = int.Parse(line[2]);
              fpaAdapter.Insert(schuelerId, "", null, null, null, null, gesamterfolg, null, null);
            }
            else
              throw new InvalidOperationException("Diese Zeile hat " + line.Length + " Spalten. Das ist mir unbekannt");
          }
        }
      }

      TrageFehlendeSchülerInDummykurseEin();
    }

    /// <summary>
    /// Geschichte wird etwas anders gehandhabt, weil jeder Geschichte ablegt - deshalb außerhalb des normalen Kursverzeichnisses.
    /// </summary>
    /// <returns>Der Dummy-Geschichtekurs.</returns>
    private static Kurs GetGeschichteKurs()
    {
      return FindOrCreateDummyKurs("Geschichte aus elfter Jahrgangsstufe", "G");
    }

    private static Dictionary<string, Kurs> GetKursverzeichnis()
    {
      Dictionary<string, Kurs> kurse = new Dictionary<string, Kurs>();
      kurse.Add("RL", FindOrCreateDummyKurs("Rechtslehre aus elfter Jahrgangsstufe", "Rl"));
      kurse.Add("C", FindOrCreateDummyKurs("Chemie aus elfter Jahrgangsstufe", "C"));
      kurse.Add("TZ", FindOrCreateDummyKurs("TZ aus elfter Jahrgangsstufe", "TZ"));
      // laut Stundentafel legt der Agrarzweig außer Geschichte nichts ab.
      return kurse;
    }

    public static void TrageFehlendeSchülerInDummykurseEin()
    {
      var kurse = GetKursverzeichnis();
      foreach (var dbSchueler in new SchuelerTableAdapter().GetData())
      {
        Schueler schueler = new Schueler(dbSchueler);
        if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf && schueler.getKlasse.Schulart == Schulart.FOS)
        {
          schueler.MeldeAn(GetGeschichteKurs());
          switch (schueler.Zweig)
          {
            case Zweig.Agrar: break;
            case Zweig.Sozial: schueler.MeldeAn(kurse["C"]); break;
            case Zweig.Technik: schueler.MeldeAn(kurse["TZ"]); break;
            case Zweig.Wirtschaft: schueler.MeldeAn(kurse["RL"]); break;
          }
        }
      }
    }

    /// <summary>
    /// Methode importiert die Zeugnisnoten des Vorjahres.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ImportiereNotenAusWinSD(string fileName)
    {
      using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(stream))
      {
        var schuelerAdapter = new SchuelerTableAdapter();
        var fpaAdapter = new FpANotenTableAdapter();
        Kurs geschichte = GetGeschichteKurs();
        Dictionary<string, Kurs> kurse = new Dictionary<string, Kurs>();
        kurse.Add("W11", FindOrCreateDummyKurs("Rechtslehre aus elfter Jahrgangsstufe", "Rl"));
        kurse.Add("S11", FindOrCreateDummyKurs("Chemie aus elfter Jahrgangsstufe", "C"));
        kurse.Add("T11", FindOrCreateDummyKurs("TZ aus elfter Jahrgangsstufe", "TZ"));
        kurse.Add("A11", null); // laut Stundentafel legt der Agrarzweig außer Geschichte nichts ab.

        while (!reader.EndOfStream)
        {
          string[] line = reader.ReadLine().Split(SeparatorChar);
          if (line.Length == 5)
          {
            //notenzeile
            int schuelerId = int.Parse(line[0]);
            string faecherspiegel = line[1];
            fpaNote fpaNote = OmnisDB.Konstanten.GetFpaNoteFromString(line[2]);
            byte? geschichteNote = string.IsNullOrEmpty(line[3]) ? (byte?)null : byte.Parse(line[3]);
            byte? zweitesAbgelegtesFachNote = string.IsNullOrEmpty(line[4]) ? (byte?)null : byte.Parse(line[4]);

            var schuelerGefunden = schuelerAdapter.GetDataById(schuelerId);
            if (schuelerGefunden == null || schuelerGefunden.Count == 0)
            {
              continue;
            }

            var schueler = new Schueler(schuelerGefunden[0]);

            if (geschichteNote != null)
            {
              TrageNoteEin(geschichte, (byte)geschichteNote, schueler);
            }
            Kurs zweitesFach = kurse[faecherspiegel];
            if (zweitesFach != null && zweitesAbgelegtesFachNote != null)
            {
              TrageNoteEin(zweitesFach, (byte)zweitesAbgelegtesFachNote, schueler);
            }


            fpaAdapter.Insert(schuelerId, "", null, null, null, null, (int)fpaNote,null,null);
          }
          else
            throw new InvalidOperationException("Diese Zeile hat " + line.Length + " Spalten. Das ist mir unbekannt");
        }
      }

      TrageFehlendeSchülerInDummykurseEin();
    }

    private static void TrageNoteEin(Kurs kurs, byte note, Schueler schueler)
    {
      schueler.MeldeAn(kurs);
      BerechneteNote bnote = new BerechneteNote(kurs.Id, schueler.Id);
      bnote.ErstesHalbjahr = false;
      bnote.JahresfortgangGanzzahlig = note;
      bnote.Abschlusszeugnis = note;
      bnote.writeToDB();
    }

    private static Kurs FindOrCreateDummyKurs(string bezeichnung, string fachKuerzel)
    {
      var kursAdapter = new KursTableAdapter();
      var fachAdapter = new FachTableAdapter();

      var kurse = kursAdapter.GetDataByBezeichnung(bezeichnung);
      if (kurse == null || kurse.Count == 0)
      {
        kursAdapter.Insert(bezeichnung, null, fachAdapter.GetDataByKuerzel(fachKuerzel)[0].Id, null);
        kurse = kursAdapter.GetDataByBezeichnung(bezeichnung);
      }
      if (kurse == null || kurse.Count == 0)
      {
        throw new InvalidOperationException("Dummykurs "+ bezeichnung + " konnte nicht angelegt werden.");
      }

      return new Kurs(kurse[0]);
    }
  }
}
