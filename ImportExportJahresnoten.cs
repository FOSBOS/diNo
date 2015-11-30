using System.IO;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
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
      //TODO: Filtern wir die exportierten Noten, z. B. nur bei Schülern der Jahrgangsstufe 11 und nur in Geschichte und Sozialkunde?
      //      oder machen wir das dann beim Import im Jahr darauf

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
            var fach = schueler.getNoten.getFach(kurs.Id);
            var note = fach.getRelevanteNote(Zeitpunkt.Jahresende);
            writer.WriteLine(schueler.Id + Separator + schueler.Name + Separator + fach.getFach.Kuerzel + Separator + lehrer.Kuerzel + Separator + note);
          }
        }
      }
    }

    /// <summary>
    /// Methode importiert die Zeugnisnoten des Vorjahres.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ImportiereNoten(string fileName)
    {
      //TODO: Filtern wir die exportierten Noten, z. B. nur bei Schülern der Jahrgangsstufe 11 und nur in Geschichte und Sozialkunde?
      //      oder machen wir das dann beim Import im Jahr darauf

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
