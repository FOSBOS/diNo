using System.IO;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{

  /// <summary>
  /// Exportiert die Jahresnoten aus der elften Klasse ins nächste Jahr. Exportiert wird jeweils Geschichte und
  /// T: Technisches Zeichnen
  /// W: Rechtslehre
  /// S: Chemie
  /// </summary>
  public class ImportExportJahresnoten
  {
    /// <summary>
    /// Trennzeichen für csv-Export
    /// </summary>
    private static char Separator = ';';

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
        foreach (var dbkurs in new KursTableAdapter().GetData())
        {
          Kurs kurs = new Kurs(dbkurs);
          var lehrer = kurs.getLehrer;
          foreach (var dbSchueler in new SchuelerTableAdapter().GetDataByKursId(kurs.Id))
          {
            Schueler schueler = new Schueler(dbSchueler);
            if (IsExportNecessary(kurs.getFach.Kuerzel, schueler))
            {
              var fach = schueler.getNoten.getFach(kurs.Id);
              var note = fach.getRelevanteNote(Zeitpunkt.Jahresende);
              writer.WriteLine(schueler.Id + Separator + schueler.Name + Separator + fach.getFach.Kuerzel + Separator + lehrer.Kuerzel + Separator + note);
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
      // Momentan exportieren wir nur die Noten der FOS11 in den Fächern, die dort abgelegt werden.
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && schueler.getKlasse.Schulart == Schulart.FOS)
      {
        return fachKuerzel == "G" ||
          (schueler.Data.Ausbildungsrichtung == "W" && fachKuerzel == "Rl") ||
          (schueler.Data.Ausbildungsrichtung == "S" && fachKuerzel == "Ch") ||
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
        string[] line = reader.ReadLine().Split(Separator);
        int schuelerId = int.Parse(line[0]);
        string nachname = line[1];
        string fachKuerzel = line[2];
        string lehrerKuerzel = line[3];
        byte zeugnisnote = byte.Parse(line[4]);

        // TODO: Künstlichen Kurs anlegen für Vorjahresnoten. Aber: Nur einen für alle Schüler / pro Lehrer / ganz ohne Lehrer
        //       wie ist gewährleistet, dass dieser Kurs anders gehandhabt wird, z. B. bei den Noten-Checks (keine SA usw.)
      }
    }
  }
}
