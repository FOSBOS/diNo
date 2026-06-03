using diNo;
using diNo.Xml.Mbstatistik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace diNo
{

  /// <summary>
  /// Export-Klasse für ASV-Daten (Amtliche Schulverwaltung Bayern)
  /// Exportiert Schüler- und Notendaten im ASV-XML-Format
  /// </summary>
  public class ASVExport
  {
    
    private readonly string _exportDateiPfad;

    public ASVExport(string exportDateiPfad)
    {      
      _exportDateiPfad = exportDateiPfad;
    }

    /// <summary>
    /// Hauptmethode zum Exportieren der ASV-Daten
    /// zwei Aufrufe nötig einmal mit true und mit false
    /// </summary>
    public void ExportiereASVDaten(bool pruefungsartFachabi)
    {
      // 064 = Fachabitur, 074 = Abitur
      string pruefungsart = pruefungsartFachabi ? "064" : "074";

      var root = new XElement("Root",
          new XAttribute("Generierungsdatum", DateTime.Now.ToString("yyyy-MM-dd")),
          new XElement("ApDaten",
              ErstelleSchulInformationen(),
              new XElement("Schuljahr", "SCHULJAHR_" + Zugriff.Instance.Schuljahr),
              new XElement("Pruefungsart", "ABSPRUEART_" + pruefungsart),
              ErstelleSchuelerListe(pruefungsartFachabi)
          )
      );

      var doc = new XDocument(
          new XDeclaration("1.0", "utf-8", null),
          root
      );

      doc.Save(_exportDateiPfad);
    }

    private XElement ErstelleSchulInformationen()
    {
      return new XElement("Schulinformationen",
          new XElement("Schulnummer", Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS)),
          new XElement("Schulart", "1152_52")
      );
    }

    private IEnumerable<XElement> ErstelleSchuelerListe(bool pruefungsartFachabi)
    {
      var schuelerListe = Zugriff.Instance.SchuelerRep.getList()
          .Where(s => s.getKlasse.Jahrgangsstufe == (pruefungsartFachabi ? Jahrgangsstufe.Zwoelf : Jahrgangsstufe.Dreizehn))
          .Where(s => !s.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen));

      foreach (var schueler in schuelerListe)
      {
        yield return ErstelleSchuelerElement(schueler);
      }
    }

    private XElement ErstelleSchuelerElement(Schueler schueler)
    {
      return new XElement("Schueler",
          ErstellePersonendaten(schueler),
          ErstelleZusatzinformationen(schueler),
          ErstelleEinzeldaten(schueler)
      );
    }

    private XElement ErstellePersonendaten(Schueler schueler)
    {
      return new XElement("Person",
          new XElement("Rufname", schueler.benutzterVorname),
          new XElement("Familienname", schueler.Name),
          new XElement("LDM", schueler.Data.asv_id ?? $"DUMMY_LDM_{schueler.Id}")
      );
    }

    private XElement ErstelleZusatzinformationen(Schueler schueler)
    {
      var zusatzinfo = new XElement("Zusatzinformationen");

      // Gesamtleistung (falls vorhanden)
      if (!schueler.Data.IsDNoteNull())
      {
        zusatzinfo.Add(new XElement("Gesamtleistung",
            new XElement("Gesamtpunkte", schueler.punktesumme.Summe(PunktesummeArt.Gesamt)),
            new XElement("Gesamtnote", schueler.Data.DNote.ToString("F2"))
        ));
      }

      // Schülerstatus
      zusatzinfo.Add(new XElement("Schuelerstatus", "1330_20")); // Stammschüler, externe 1330_21

      // Prüfungsstatus
      zusatzinfo.Add(new XElement("Pruefungsstatus", GetPruefungsStatus(schueler)));

      return zusatzinfo;
    }

    private IEnumerable<XElement> ErstelleEinzeldaten(Schueler schueler)
    {
      var einzeldaten = new List<XElement>();

      // Halbjahresleistungen (11.1 bis 13.2)
      einzeldaten.AddRange(ErstelleHalbjahresleistungen(schueler));

      // Abschlussprüfungsnoten
      //einzeldaten.AddRange(ErstelleAbschlusspruefungsnoten(schueler));

      return einzeldaten;
    }

    private IEnumerable<XElement> ErstelleHalbjahresleistungen(Schueler schueler)
    {      
      var einzeldaten = new List<XElement>();

      foreach (var fach in schueler.getNoten.alleFaecher)
      {
        // Sport-Befreiung berücksichtigen
        if (fach.getFach.BezZeugnis.StartsWith("Sport", StringComparison.OrdinalIgnoreCase) && schueler.hatVorkommnis(Vorkommnisart.Sportbefreiung))
            continue;

        foreach (HjArt art in new[] { HjArt.Hj1, HjArt.Hj2 }) // 11. Klasse
        {
          HjLeistung hj = fach.getVorHjLeistung(art);
          if (hj != null)
          {
            einzeldaten.Add(ErstelleExtendedPruefungsteil(schueler, fach.getFach, hj));
          }
        }
        foreach (HjArt art in Enum.GetValues(typeof(HjArt))) // 12./13. Klasse
        {
          if (art == HjArt.FR || art == HjArt.Sprachenniveau || art == HjArt.GesErgSprache) continue; // werden nicht oder nicht hier übermittelt

          HjLeistung hj = fach.getHjLeistung(art);
          if (hj != null)
          {
            einzeldaten.Add(ErstelleExtendedPruefungsteil(schueler, fach.getFach, hj));
          }
        }

        // alle AP-Ergebnisse
        if (fach.getFach.IstSAPFach(schueler.Zweig))
        {
          //einzeldaten.Add(ErstelleBasePruefungsteil(fach));
        }
      }

      // Fachreferat
      HjLeistung fr = schueler.Fachreferat.FirstOrDefault();
      if (fr!=null){
        einzeldaten.Add(
        new XElement("Einzeldaten",
          new XElement("ExtendedPruefungsteil",
              new XElement("Note", fr.Punkte),
              //new XElement("SchuleFach", fach.Id),
              new XElement("Teil", GetTeilCode(fr)),
              new XElement("Belegart", "1038_35"), // Sondercode FR
              new XElement("Zeugnisart", "1198_25"),
              new XElement("Jahrgangsstufe", "JAHRGA_101"),
              new XElement("Einbringung", "1"),
              new XElement("Schuljahr", "SCHULJAHR_" + Zugriff.Instance.Schuljahr),
              new XElement("Sonderfall", (fr.Status == HjStatus.Ungueltig).ToString().ToLower())
          ))
      );
      }

      // Seminar
      var sem = schueler.Seminarfachnote;      
      if (!sem.IsGesamtnoteNull())
      {
        einzeldaten.Add(
        new XElement("Einzeldaten",
          new XElement("ExtendedPruefungsteil",
              new XElement("Note", sem.Gesamtnote),              
              new XElement("Belegart", "1038_34"), // Sondercode Seminar
              new XElement("Zeugnisart", "1198_25"),
              new XElement("Jahrgangsstufe", "JAHRGA_102"),
              new XElement("Schuljahr", "SCHULJAHR_" + Zugriff.Instance.Schuljahr),
              new XElement("Sonderfall", "false")
          )
      ));
      }

      return einzeldaten;
    }

    private XElement ErstelleExtendedPruefungsteil(Schueler schueler, Fach fach, HjLeistung hj)
    {
      return new XElement("Einzeldaten",
          new XElement("ExtendedPruefungsteil",
              new XElement("Note", hj.Punkte),
              new XElement("SchuleFach", fach.Id), // TODO: GUID b9be4cf6/a3a0/4652/ab7c/55afd7d73cec
              new XElement("Teil", GetTeilCode(hj)),
              new XElement("Belegart", GetBelegart(hj.getFach)),
              //new XElement("Zeugnisart", GetZeugnisart(halbjahr)),
              new XElement("Jahrgangsstufe", GetJahrgangsstufe(hj)),
              new XElement("Einbringung", hj.Status == HjStatus.Einbringen ? 1 : 0),
              new XElement("Schuljahr", "SCHULJAHR_" + Zugriff.Instance.Schuljahr),
              new XElement("Sonderfall", (hj.Status == HjStatus.Ungueltig).ToString().ToLower())
          )
      );
    }
    /*
    private IEnumerable<XElement> ErstelleAbschlusspruefungsnoten(Schueler schueler)
    {
      var einzeldaten = new List<XElement>();

      // Schriftliche Prüfungen
      var schriftlicheNoten = _repository.GetAbschlusspruefungsnotenSchriftlich(schueler.Id);
      foreach (var note in schriftlicheNoten)
      {
        einzeldaten.Add(ErstelleBasePruefungsteil(schueler, note, "1243_116")); // Schriftlich
      }

      // Mündliche Prüfungen
      var muendlicheNoten = _repository.GetAbschlusspruefungsnotenMuendlich(schueler.Id);
      foreach (var note in muendlicheNoten)
      {
        einzeldaten.Add(ErstelleBasePruefungsteil(schueler, note, "1243_117")); // Mündlich
      }

      return einzeldaten;
    }

    private XElement ErstelleBasePruefungsteil(Schueler schueler, Note note, string teilCode)
    {
     var apGesamt = fach.getHjLeistung(HjArt.AP);
      var apmuendlich = fach.getNote(Halbjahr.Zweites, Notentyp.APMuendlich);
      var apschriftlich = fach.getNote(Halbjahr.Zweites, Notentyp.APSchriftlich);
      if (apGesamt != null)
      {
        // nur, wenn alle Noten vorhanden sind
        var apObject = CreateXMLAPObject(xmlSchueler.abschlusspruefung, fach.getFach, unserSchueler);
        if (apmuendlich != null)
        {
          apObject.GetType().GetProperty("muendlich").SetValue(apObject, apmuendlich.Value.ToString());
        }
        apObject.GetType().GetProperty("schriftlich").SetValue(apObject, apschriftlich.Value.ToString());
        apObject.GetType().GetProperty("gesamt").SetValue(apObject, apGesamt.Punkte.ToString());
      }

      return new XElement("Einzeldaten",
          new XElement("BasePruefungsteil",
              new XElement("Note", note.Punkte), // Bereits 0-15 Punkte
              new XElement("Schuelerfach", GetSchuelerFachID(note.Fach)),
              new XElement("Teil", teilCode)
          )
      );
    }
    */
    // ==================== HELPER-METHODEN ====================

    /// <summary>
    /// Gibt die SchuleFachId aus der Datenbank zurück
    /// </summary>
    private string GetSchuleFachID(Fach fach)
    {/*
      if (!string.IsNullOrEmpty(fach.Data.SchuleFachId))
      {
        return fach.Data.SchuleFachId;
      }*/

      // Fallback: Dummy-Wert
      return $"DUMMY_SCHULE_FACH_{fach.Id}";
    }

    /// <summary>
    /// Berechnet die SchuelerFachId aus der SchuleFachId (SchuleFachId + 1)
    /// Beispiel: 8a498182-999d87d3-0199-9ea7ca92-5187 -> 8a498182-999d87d3-0199-9ea7ca92-5188
    /// </summary>
    private string GetSchuelerFachID(Fach fach)
    {
      var schuleFachId = GetSchuleFachID(fach);

      if (schuleFachId.StartsWith("DUMMY_"))
      {
        return schuleFachId.Replace("DUMMY_SCHULE_FACH_", "DUMMY_SCHUELER_FACH_");
      }

      try
      {
        // UUID in Teile zerlegen
        var parts = schuleFachId.Split('-');
        var lastPart = parts[parts.Length - 1];

        // Letzten Teil als Hex-Zahl parsen und um 1 erhöhen
        var incremented = (int.Parse(lastPart, System.Globalization.NumberStyles.HexNumber) + 1)
            .ToString("x4");

        // UUID wieder zusammensetzen
        parts[parts.Length - 1] = incremented;
        return string.Join("-", parts);
      }
      catch
      {
        // Fallback bei Parsing-Fehler
        return $"DUMMY_SCHUELER_FACH_{fach.Id}";
      }
    }

    /// <summary>
    /// Mapping: Halbjahr -> ASV Teil-Code (1243_XXX)
    /// Quelle: Prüfungsteil (1243).txt
    /// </summary>
    private string GetTeilCode(HjLeistung hj)
    {
      int c = 0;

      switch (hj.Art)
      {
        case HjArt.Hj1:
        case HjArt.Hj2:
          c = (hj.Art == HjArt.Hj1) ? 450 : 451;
          if (hj.getFach.Kuerzel == "FpA") c -= 20;
          else if (hj.JgStufe == Jahrgangsstufe.Zwoelf) c += 2;
          else if (hj.JgStufe == Jahrgangsstufe.Dreizehn) c += 4;

          if (hj.Status != HjStatus.Einbringen) c += 20; // 470 bis 475
          break;

        case HjArt.AP: c = 490;
          break;

        case HjArt.GesErg: c = 491;
          break;

        case HjArt.FR:
          c = 432;
          break;

        case HjArt.JN:
          c = 413;
          break;
      }

      return "1243_" + c;
    }

    /// <summary>
    /// Mapping: Fachtyp -> ASV Belegart (1038_XX)
    /// Quelle: Belegart (1038).txt
    /// </summary>
    private string GetBelegart(Fach fach)
    {
      // 1038_34 = Seminar
      // 1038_35 = Fachreferat
      if (fach.Typ == FachTyp.WPF) return "1038_14"; // Wahlpflichtfach
      return "1038_13"; // Pflichtfach
    }

    /// <summary>
    /// Mapping: Halbjahr -> ASV Zeugnisart (1198_XX)
    /// Quelle: Zeugnisart (1038).txt
    /// </summary>
    private string GetZeugnisart(string halbjahr)
    {
      // muss bei FR und Seminar so gesetzt werden
      return "1198_25"; // Jahreszeugnis FOS/BOS
    }

    /// <summary>
    /// Mapping: Halbjahr -> ASV Jahrgangsstufe (JAHRGA_XXX)
    /// Quelle: X Grundlagen für Drittprogramme.pdf
    /// </summary>
    private string GetJahrgangsstufe(HjLeistung hj)
    {
      return "JAHRGA_" + (int)hj.JgStufe + 89;
      /*
      JAHRGA_100 // Klasse 11
      JAHRGA_101 // Klasse 12
      JAHRGA_102 // Klasse 13
      */
      
    }

    /// <summary>
    /// Mapping: Prüfungsstatus -> ASV Code (1139_XX)
    /// Quelle: X Grundlagen für Drittprogramme.pdf
    /// </summary>
    private string GetPruefungsStatus(Schueler schueler)
    {
      /*
      1139_10 = Nicht zugelassen
      1139_15 = zugelassen ???
      1139_20 = nicht bestanden
      1139_30 = bestanden
      */      
      if (schueler.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen)) return "1139_10";
      if (schueler.hatVorkommnis(Vorkommnisart.NichtBestanden)) return "1139_20";
      return "1139_30";
    }
  }
}