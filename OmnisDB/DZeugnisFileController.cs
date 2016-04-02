using diNo.diNoDataSetTableAdapters;
using log4net;
using System.IO;
using System.Text;

namespace diNo.OmnisDB
{
  public class DZeugnisFileController
  {
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="sourceFileName">Der Dateiname des Exportfiles der leeren DZeugnis-Tabelle.</param>
    /// <param name="targetFileName">Der Dateiname des zu erstellenden ImportFiles der DZeugnis-Tabelle.</param>
    public DZeugnisFileController(string sourceFileName, string targetFileName, Zeitpunkt zeitpunkt)
    {
      Faecherspiegel faecher = new Faecherspiegel();
      SchuelerTableAdapter ada = new SchuelerTableAdapter();

      using (FileStream inStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(inStream, Encoding.GetEncoding("iso-8859-1")))
      using (FileStream outStream = new FileStream(targetFileName, FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(outStream, Encoding.GetEncoding("iso-8859-1")))
      {
        while (!reader.EndOfStream)
        {
          var zeile = new VerwalteZeile(reader.ReadLine());
          int schuelerId = int.Parse(zeile[Konstanten.schuelerIdCol]);

          // Prüfe vorher, ob der Schüler existiert (hier kommen tausend Schüler aus den Vorjahren)
          if (ada.GetDataById(schuelerId).Count == 0)
          {
            continue;
          }

          Schueler schueler = new Schueler(schuelerId);
          zeile[Konstanten.fpaCol] = Konstanten.GetFpaString(GetFpaNote(zeitpunkt, schueler));
          zeile[Konstanten.klassenzielOderGefaehrdungCol] = Konstanten.GetKlassenzielOderGefaehrdungString(GetZielerreichung(zeitpunkt, schueler));
          zeile[Konstanten.abweisungCol] = Konstanten.GetAbweisungString(schueler.GefahrDerAbweisung);

          string faecherspiegel = zeile[Konstanten.faecherspiegelCol];
          if (string.IsNullOrEmpty(faecherspiegel))
          {
            log.Warn("Für den Schüler " + schueler.NameVorname + " gibt es keinen passenden Fächerspiegel!");
            continue;
          }
          for (int i = 0; i < 30; i++)
          {
            zeile[Konstanten.notePflichtfach1Col + i] = faecher.GetFachNoteString(faecherspiegel, i, schueler.getKlasse.Schulart, schueler, zeitpunkt);
          }

          SucheWahlpflichtfach(zeitpunkt, faecher, zeile, schueler, Konstanten.weiteresFach1BezeichnungCol, Konstanten.weiteresFach1NoteCol);
          SucheWahlpflichtfach(zeitpunkt, faecher, zeile, schueler, Konstanten.weiteresFach2BezeichnungCol, Konstanten.weiteresFach2NoteCol);
          SucheWahlpflichtfach(zeitpunkt, faecher, zeile, schueler, Konstanten.weiteresFach3BezeichnungCol, Konstanten.weiteresFach3NoteCol);

          writer.WriteLine(zeile.ToString());

        }
      }
    }

    private static void SucheWahlpflichtfach(Zeitpunkt zeitpunkt, Faecherspiegel faecher, VerwalteZeile zeile, Schueler schueler, int bezeichnungCol, int noteCol)
    {
      if (string.IsNullOrEmpty(zeile[bezeichnungCol]))
      {
        return;
      }

      string fach = zeile[bezeichnungCol];
      if (fach =="F3")
      {
        fach = "F-Wi";
      }

      var wahlpflichtfach = schueler.getNoten.FindeFach(fach, false);
      if (wahlpflichtfach != null)
      {
        zeile[noteCol] = faecher.GetNotenString(wahlpflichtfach, zeitpunkt);
      }
      else
      {
        log.Warn("Für den Schüler "+schueler.NameVorname+" konnte das Wahlpflichtfach "+fach+" nicht gefunden werden.");
      }
    }

    private static KlassenzielOderGefaehrdung GetZielerreichung(Zeitpunkt zeitpunkt, Schueler schueler)
    {
      KlassenzielOderGefaehrdung ziel = zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS ? KlassenzielOderGefaehrdung.NichtGefaehrdet : KlassenzielOderGefaehrdung.VorrueckenOK;
      foreach (var vorkommnis in schueler.Vorkommnisse)
      {
        switch (vorkommnis.Art)
        {
          case Vorkommnisart.starkeGefaehrdungsmitteilung: ziel = KlassenzielOderGefaehrdung.SehrGefaehrdet; break;
          case Vorkommnisart.Gefaehrdungsmitteilung: ziel = KlassenzielOderGefaehrdung.Gefaehrdet; break;
          case Vorkommnisart.BeiWeiteremAbsinken: ziel = KlassenzielOderGefaehrdung.BeiWeiteremAbsinkenGefaehrdet; break;
          case Vorkommnisart.NichtZurPruefungZugelassen: ziel = KlassenzielOderGefaehrdung.AbschlusspruefungOhneErfolg; break;
          case Vorkommnisart.Notenausgleich: ziel = KlassenzielOderGefaehrdung.NotenausgleichGewaehrt; break;
          case Vorkommnisart.endgueltigNichtBestanden: ziel = KlassenzielOderGefaehrdung.AbschlusspruefungOhneErfolg; break;
            // TODO: weitere nicht bestehen-Gründe aufnehmen
        }
      }

      return ziel;
    }

    private static fpaNote GetFpaNote(Zeitpunkt zeitpunkt, Schueler schueler)
    {
      if (zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS && !schueler.FPANoten.IsErfolg1HjNull())
      {
        //das ist dieselbe Zahlencodierung ist wie in der DB (0=nicht gesetzt, 1 = sehr gut, ... 4 = ohne Erfolg)
        return (fpaNote)schueler.FPANoten.Erfolg1Hj;
      }
      else if (!schueler.FPANoten.IsErfolgNull())
      {
        return (fpaNote)schueler.FPANoten.Erfolg;
      }

      return fpaNote.Entfaellt;
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
        if (eintraege.Length != 233)
        {
          throw new InvalidDataException("ungültige zeile für DZeugnis hat "+eintraege.Length + "Einträge statt 233");
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
          return eintraege[i].Trim('\"');
        }
        set
        {
          eintraege[i] = "\"" + value + "\"";
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
