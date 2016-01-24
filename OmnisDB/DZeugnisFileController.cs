using System.IO;

namespace diNo.OmnisDB
{
  public class DZeugnisFileController
  {
    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="sourceFileName">Der Dateiname des Exportfiles der leeren DZeugnis-Tabelle.</param>
    /// <param name="targetFileName">Der Dateiname des zu erstellenden ImportFiles der DZeugnis-Tabelle.</param>
    public DZeugnisFileController(string sourceFileName, string targetFileName)
    {
      Faecherspiegel faecher = new Faecherspiegel();

      using (FileStream inStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(inStream))
      using (FileStream outStream = new FileStream(targetFileName, FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(outStream))
      {
        while (!reader.EndOfStream)
        {
          var zeile = new VerwalteZeile(reader.ReadLine());
          int schuelerId = int.Parse(zeile[Konstanten.schuelerIdCol]);
          Schueler schueler = new Schueler(schuelerId);

          //TODO: Hier müssen die allgemeinen Infos geprüft / ausgefüllt werden, z. B. Gefährdungsmitteilung etc.
          string faecherspiegel = zeile[Konstanten.faecherspiegelCol];
          for (int i = 0; i < 30; i++)
          {
            var fach = faecher.GetFach(faecherspiegel, i, schueler.getKlasse.Schulart);
            if (fach == null)
            {
              break; // wenn kein Fach mehr gefunden wird, brich die Noten-Schleife ab
            }

            var noten = schueler.getNoten.FindeFach(fach, false);
            if (noten == null)
            {
              // TODO: Was mach'mer denn dann? Nicht jeder Schüler ist katholisch...
            }

            byte? note = noten.getRelevanteNote(Zeitpunkt.HalbjahrUndProbezeitFOS); //TODO: Nicht nur Halbjahr
            zeile[Konstanten.notePflichtfach1Col + i] = note == null ? "-" : note.ToString();
          }

        }
      }
    }

    /// <summary>
    /// Klasse zum vereinfachten Verwalten der Einträge einer Zeile 
    /// </summary>
    private class VerwalteZeile
    {
      private string[] eintraege;

      public VerwalteZeile(string line)
      {
        eintraege = line.Split('\t');
        if (eintraege.Length != 231)
        {
          throw new InvalidDataException("ungültige zeile für DZeugnis hat nur "+eintraege.Length + "Einträge statt 231");
        }
      }
 
      /// <summary>
      /// Zugriff auf internes array wird über Indexer nach außen freigegeben.
      /// </summary>
      /// <param name="i">Die Spaltennummer.</param>
      /// <returns>Den Eintrag der betreffenden Spalte.</returns>
      public string this[int i]
      {
        get
        {
          return eintraege[i];
        }
        set
        {
          eintraege[i] = value;
        }
      }

      /// <summary>
      /// Setzt die Einträge hintereinander (mit Tabulatortrennung).
      /// </summary>
      /// <returns>Die komplette Zeile als einzelnen String.</returns>
      public override string ToString()
      {
        return string.Join("\t", eintraege);
      }
    }
  }
}
