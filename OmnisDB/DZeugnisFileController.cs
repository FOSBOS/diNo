using diNo.diNoDataSetTableAdapters;
using log4net;
using System.Globalization;
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
          
          Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
          if (BrauchtZeugnis(schueler, zeitpunkt))
          {
            if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf || (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf && (zeitpunkt == Zeitpunkt.DrittePA || zeitpunkt == Zeitpunkt.Jahresende)))
            {
              // fpA darf nur bei Elftklässlern übertragen werden oder bei Zwölftklässlern ins Abschlusszeugnis
              if (zeile[Konstanten.fpaCol] != "" && zeile[Konstanten.fpaCol] != Konstanten.GetFpaString(GetFpaNote(zeitpunkt, schueler)))
              {
                log.WarnFormat("überschreibe fpA-Note für Schüler {0} mit {1} statt {2}", schueler.Name, Konstanten.GetFpaString(GetFpaNote(zeitpunkt, schueler)), zeile[Konstanten.fpaCol]);
              }
              zeile[Konstanten.fpaCol] = Konstanten.GetFpaString(GetFpaNote(zeitpunkt, schueler));
            }

            KlassenzielOderGefaehrdung zielerreichung = GetZielerreichung(zeitpunkt, schueler);
            zeile[Konstanten.klassenzielOderGefaehrdungCol] = Konstanten.GetKlassenzielOderGefaehrdungString(zielerreichung);
            if (zeitpunkt == Zeitpunkt.ErstePA || zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA)
            {
              zeile[Konstanten.zeugnisartCol] = zielerreichung == KlassenzielOderGefaehrdung.AbschlusspruefungOhneErfolg ? "J" : "A";
              zeile[Konstanten.APBestandenCol] = Konstanten.GetBestandenString(GetBestanden(zeitpunkt, schueler));
            }
            if (zielerreichung == KlassenzielOderGefaehrdung.BeiWeiteremAbsinkenGefaehrdet || zielerreichung == KlassenzielOderGefaehrdung.Gefaehrdet || zielerreichung == KlassenzielOderGefaehrdung.SehrGefaehrdet)
            {
              // Gefahr der Abweisung soll nur angekreuzt werden, wenn der Schüler zum Halbjahr wirklich gefährdet ist
              zeile[Konstanten.abweisungCol] = Konstanten.GetAbweisungString(schueler.GefahrDerAbweisung);
            }

            HandleSeminarfach(zeile, schueler);

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

            if (Konstanten.ZeugnisartFromString(zeile[Konstanten.zeugnisartCol]) != Zeugnisart.Zwischenzeugnis)
            {
              for (int i = 0; i < 20; i++)
              {
                // Wenn schon Jahresfortgangsnoten drinstehen, nicht anfassen!!!
                string jahresfortgang = faecher.FindeJahresfortgangsNoten(faecherspiegel, i, schueler.getKlasse.Schulart, schueler, zeitpunkt);
                if (zeile[Konstanten.jahresfortgangPflichtfach1Col + i] != "" && zeile[Konstanten.jahresfortgangPflichtfach1Col + i] != jahresfortgang)
                {
                  log.Warn("Der Jahresfortgang in einem Fach steht schon in der Schulverwaltung und stimmt nicht mit diNo überein: WinSV:" + zeile[Konstanten.jahresfortgangPflichtfach1Col + i] + "; diNo :" + jahresfortgang);
                }
                if (zeile[Konstanten.jahresfortgangPflichtfach1Col + i] != "" && zeile[Konstanten.jahresfortgangPflichtfach1Col + i] != jahresfortgang)
                {
                  zeile[Konstanten.jahresfortgangPflichtfach1Col + i] = jahresfortgang;
                }

                zeile[Konstanten.APschriftlichPflichtfach1Col + i] = faecher.FindeAPSchriftlichNoten(faecherspiegel, i, schueler.getKlasse.Schulart, schueler, zeitpunkt);
                zeile[Konstanten.APmuendlichPflichtfach1Col + i] = faecher.FindeAPMuendlichNoten(faecherspiegel, i, schueler.getKlasse.Schulart, schueler, zeitpunkt);
                zeile[Konstanten.gesamtNoteMitAPGanzzahlig1Col + i] = faecher.GetFachNoteString(faecherspiegel, i, schueler.getKlasse.Schulart, schueler, zeitpunkt);
              }
            }

            SucheWahlpflichtfach(zeitpunkt, faecher, zeile, schueler, Konstanten.weiteresFach1BezeichnungCol, Konstanten.weiteresFach1NoteCol);
            SucheWahlpflichtfach(zeitpunkt, faecher, zeile, schueler, Konstanten.weiteresFach2BezeichnungCol, Konstanten.weiteresFach2NoteCol);
            SucheWahlpflichtfach(zeitpunkt, faecher, zeile, schueler, Konstanten.weiteresFach3BezeichnungCol, Konstanten.weiteresFach3NoteCol);
          }

          // rausgeschrieben werden immer alle Zeugnisse, da im Import "ersetzen" angehakt werden muss
          writer.WriteLine(zeile.ToString());
        }
      }
    }

    private static void HandleSeminarfach(VerwalteZeile zeile, Schueler schueler)
    {
      var seminarfachNote = new SeminarfachnoteTableAdapter().GetDataBySchuelerId(schueler.Id);
      if (seminarfachNote != null && seminarfachNote.Count == 1 && !seminarfachNote[0].IsGesamtnoteNull())
      {
        string seminarfachnote = string.Format(CultureInfo.CurrentCulture, "{0:00}", seminarfachNote[0].Gesamtnote);
        if (zeile[Konstanten.seminarfachGesamtnote] != "" && zeile[Konstanten.seminarfachGesamtnote] != seminarfachnote)
        {
          log.Warn(schueler.NameVorname + ": da steht schon eine Seminarfachnote drin und die passt nicht zur diNo-Note. Alt: " + zeile[Konstanten.seminarfachGesamtnote] + " diNo: " + seminarfachnote);
        }
        if (zeile[Konstanten.seminarfachGesamtnote] == "")
        {
          zeile[Konstanten.seminarfachGesamtnote] = seminarfachnote;
        }

        string seminarfachThema = !string.IsNullOrEmpty(seminarfachNote[0].ThemaKurz) ? seminarfachNote[0].ThemaKurz : seminarfachNote[0].ThemaLang.Substring(0, 128);
        if (!seminarfachThema.StartsWith("\""))
        {
          seminarfachThema = "\"" + seminarfachThema;
        }
        if (!seminarfachThema.EndsWith("\""))
        {
          seminarfachThema = seminarfachThema + "\"";
        }

        if (zeile[Konstanten.seminarfachThema] != "" && zeile[Konstanten.seminarfachThema] != seminarfachThema)
        {
          log.Warn(schueler.NameVorname + ": da steht schon ein Seminarfachthema drin und die passt nicht zur diNo-Note. Alt: " + zeile[Konstanten.seminarfachThema] + " diNo: " + seminarfachThema);
        }

        if (zeile[Konstanten.seminarfachThema] == "")
        {
          zeile[Konstanten.seminarfachThema] = seminarfachThema;
        }
      }
    }

    private bool BrauchtZeugnis(Schueler schueler, Zeitpunkt zeitpunkt)
    {
      if (zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        return true; // zum Halbjahr bekommen alle ein Zeugnis
      }

      // zu den PA-Sitzungen werden nur für die 12. und 13. Jahrgangsstufe Zeugnisse übertragen
      if (zeitpunkt == Zeitpunkt.ErstePA || zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA)
      {
        return schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn || schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf;
      }

      if (zeitpunkt == Zeitpunkt.Jahresende)
      {
        return schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf || schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Vorklasse || schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Vorkurs;
      }

      return false;
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
        // falls in der WinSV schon eine Note eingetragen wurde, z. B. für Latein o. Ä. darf diese nicht überschrieben werden!
        if (zeile[noteCol] != "")
        {
          zeile[noteCol] = faecher.GetNotenString(wahlpflichtfach, zeitpunkt);
        }
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
          case Vorkommnisart.NichtZurPruefungZugelassen: return KlassenzielOderGefaehrdung.AbschlusspruefungOhneErfolg;
          case Vorkommnisart.Notenausgleich: return KlassenzielOderGefaehrdung.NotenausgleichGewaehrt;
          case Vorkommnisart.NichtBestanden: return KlassenzielOderGefaehrdung.AbschlusspruefungOhneErfolg;
        }
      }

      return ziel;
    }

    private static AbschlusspruefungBestanden GetBestanden(Zeitpunkt zeitpunkt, Schueler schueler)
    {
      KlassenzielOderGefaehrdung zielerreichung = GetZielerreichung(zeitpunkt, schueler);
      if (zielerreichung == KlassenzielOderGefaehrdung.AbschlusspruefungOhneErfolg)
      {
        return AbschlusspruefungBestanden.NichtBestanden;
      }

      if (zielerreichung == KlassenzielOderGefaehrdung.NotenausgleichGewaehrt)
      {
        return AbschlusspruefungBestanden.BestandenMitNotenausgleichArt33;
      }

      return AbschlusspruefungBestanden.Bestanden;
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
          // wenn die Sekretärinnen etwas nicht-leeres eingetragen haben und wir gerne den Eintrag löschen würden, dann "gewinnt" das Sekretariat (also keine Änderung)
          if (IstLeer(value) && !IstLeer(eintraege[i]))
          {
            return;
          }

          //dasselbe gilt, wenn die Sekretärinnen ein "-" zum leermachen verwenden, dann bleibt das auch drin
          if (string.IsNullOrEmpty(value) && eintraege[i]=="-")
          {
            return;
          }

          eintraege[i] = "\"" + value + "\"";
        }
      }

      private bool IstLeer(string eintrag)
      {
        return string.IsNullOrEmpty(eintrag) || eintrag == "-";
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
