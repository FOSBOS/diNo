using log4net;
using System;
using System.Collections.Generic;
using System.IO;
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
    private StreamWriter _log;
    Random random = new Random();

    public ASVExport(string exportDateiPfad)
    {
      Directory.CreateDirectory(@"C:\tmp");
      string logPfad = $@"C:\tmp\ASVExport.log";

      using (_log = new StreamWriter(logPfad, append: false, encoding: System.Text.Encoding.UTF8))
      {
        Log($"=== ASVExport gestartet ===");
        Log($"XML-Datei: {exportDateiPfad}");

        ExportiereASVDaten(true, exportDateiPfad + "_Fachabitur.xml");
        ExportiereASVDaten(false, exportDateiPfad + "_Abitur.xml");
      }
    }
    

    private void Log(string message)
    {
      string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
      _log.WriteLine(line);
      _log.Flush();
    }

    /// <summary>
    /// Hauptmethode zum Exportieren der ASV-Daten
    /// zwei Aufrufe nötig einmal mit true und mit false
    /// </summary>
    public void ExportiereASVDaten(bool pruefungsartFachabi, string exportDateiPfad)
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

      doc.Save(exportDateiPfad);
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
          .Where(s => s.getKlasse.Jahrgangsstufe == (pruefungsartFachabi ? Jahrgangsstufe.Zwoelf : Jahrgangsstufe.Dreizehn));
          //.Where(s => !s.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen));

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
      if (schueler.Data.Isasv_idNull())
      {
        Log(schueler.NameVorname + " hat keine LDM-ID");
      }
      return new XElement("Person",
          new XElement("Rufname", schueler.benutzterVorname),
          new XElement("Familienname", schueler.Name),
          new XElement("LDM", schueler.Data.Isasv_idNull() ? $"DUMMY_LDM_{schueler.Id}" : schueler.Data.asv_id)
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
            new XElement("Gesamtnote", schueler.Data.DNote)//.ToString("F1"))
        ));
      }

      // Schülerstatus
      zusatzinfo.Add(new XElement("Schuelerstatus", "1330_20")); // Stammschüler, externe 1330_21

      // Prüfungsstatus
      zusatzinfo.Add(new XElement("Pruefungsstatus", GetPruefungsStatus(schueler)));

      return zusatzinfo;
    }

    // Alle Einzeldaten des Schülers
    private IEnumerable<XElement> ErstelleEinzeldaten(Schueler schueler)
    {
      var einzeldaten = new List<XElement>();
      
      foreach (var fach in schueler.getNoten.alleFaecher)
      {
        // Sport-Befreiung berücksichtigen
        if (fach.getFach.BezZeugnis.StartsWith("Sport", StringComparison.OrdinalIgnoreCase) && schueler.hatVorkommnis(Vorkommnisart.Sportbefreiung))
            continue;
        if (schueler.Data.Schulart != "B") // 11. Klasse
        {
          ErstelleExtendedPruefungsteil(einzeldaten, schueler, fach, HjArt.Hj1, true);
          ErstelleExtendedPruefungsteil(einzeldaten, schueler, fach, HjArt.Hj2, true);
        }
        ErstelleExtendedPruefungsteil(einzeldaten, schueler, fach, HjArt.Hj1);
        ErstelleExtendedPruefungsteil(einzeldaten, schueler, fach, HjArt.Hj2);
        
        // alle AP-Ergebnisse
        if (fach.getFach.IstSAPFach(schueler.Zweig))
        {          
          ErstelleBasePruefungsteil(einzeldaten, fach.kurs, fach.getNote(Halbjahr.Zweites, Notentyp.APSchriftlich), 414);
          ErstelleBasePruefungsteil(einzeldaten, fach.kurs, fach.getNote(Halbjahr.Zweites, Notentyp.APMuendlich), 418);
          ErstelleBasePruefungsteil(einzeldaten, fach.kurs, fach.getHjLeistung(HjArt.AP), 490);
        }
        // immer das Gesamtergebnis:
        ErstelleBasePruefungsteil(einzeldaten, fach.kurs, fach.getHjLeistung(HjArt.GesErg), 491);
      }

      // Fachreferat
      HjLeistung fr = schueler.Fachreferat.FirstOrDefault();
      if (fr!=null){
        einzeldaten.Add(
        new XElement("Einzeldaten",
          new XElement("ExtendedPruefungsteil",
              new XElement("Note", fr.Punkte),
              new XElement("SchuleFach", fr.getFach.Data.schule_fach_id),
              new XElement("Teil", GetTeilCode(fr)),
              new XElement("Belegart", "1038_35"), // Sondercode FR
              new XElement("Zeugnisart", "1198_25"),
              new XElement("Jahrgangsstufe", "JAHRGA_121"),
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
        // wir würfeln uns ein Seminarfach:
        FachSchuelerNoten f = schueler.getNoten.alleFaecher[random.Next(schueler.getNoten.alleFaecher.Count)];
        einzeldaten.Add(
        new XElement("Einzeldaten",
          new XElement("ExtendedPruefungsteil",
              new XElement("Note", sem.Gesamtnote),
              new XElement("SchuleFach", f.getFach.Data.schule_fach_id),
              new XElement("Teil", "1243_433"),
              new XElement("Belegart", "1038_34"), // Sondercode Seminar
              new XElement("Zeugnisart", "1198_25"),
              new XElement("Jahrgangsstufe", "JAHRGA_131"),
              new XElement("Einbringung", "1"),
              new XElement("Schuljahr", "SCHULJAHR_" + Zugriff.Instance.Schuljahr),
              new XElement("Sonderfall", "false")
          )
      ));
      }

      return einzeldaten;
    }

    private void ErstelleExtendedPruefungsteil(List<XElement> einzeldaten,
      Schueler schueler, FachSchuelerNoten fach, HjArt art, bool vorJahr = false)
    {
      HjLeistung hj;
      if (vorJahr)
        hj = fach.getVorHjLeistung(art);
      else
        hj = fach.getHjLeistung(art);

      if (hj != null)
      {
        if (fach.getFach.Data.Isschule_fach_idNull())
        {
          Log(fach.kurs.Kursbezeichnung + " hat keine schule_fach_id");
          return;
        }        
        //string asvid = (fach.kurs==null || fach.kurs.Data.Isschule_fach_idNull()) ? $"DUMMY_{fach.getFach.Kuerzel}" : fach.kurs.Data.schule_fach_id;
        einzeldaten.Add(new XElement("Einzeldaten",
          new XElement("ExtendedPruefungsteil",
              new XElement("Note", hj.Punkte),
              new XElement("SchuleFach", fach.getFach.Data.schule_fach_id), 
              new XElement("Teil", GetTeilCode(hj)),
              new XElement("Belegart", GetBelegart(hj.getFach)),
              new XElement("Zeugnisart", GetZeugnisart(hj)),
              new XElement("Jahrgangsstufe", GetJahrgangsstufe(hj)),
              new XElement("Einbringung", hj.Status == HjStatus.Einbringen ? 1 : 0),
              new XElement("Schuljahr", "SCHULJAHR_" + (Zugriff.Instance.Schuljahr - (vorJahr ? 1 : 0))),
              new XElement("Sonderfall", (hj.Status == HjStatus.Ungueltig).ToString().ToLower())
          )
      ));
      }
    }
    
    // für AP schriftlich und mündlich
    private void ErstelleBasePruefungsteil(List<XElement> einzeldaten, Kurs kurs, int? note, int teil)
    {
      if (note != null && kurs != null)
      {
        if (kurs.getFach.Data.Isschuelerfach_idNull())
        {
          Log(kurs.Kursbezeichnung + " hat keine schuelerfach_id");
          return;
        }
        //string asvid = kurs.Data.Isschuelerfach_idNull() ? $"DUMMY_{kurs.Id}" : kurs.Data.schuelerfach_id;
        einzeldaten.Add(new XElement("Einzeldaten",
          new XElement("BasePruefungsteil",
              new XElement("Note", note),
              new XElement("Schuelerfach", kurs.getFach.Data.schuelerfach_id),
              new XElement("Teil", "1243_" + teil)
          )));
      }
    }

    // für AP Gesamt und GesErg
    private void ErstelleBasePruefungsteil(List<XElement> einzeldaten, Kurs kurs, HjLeistung hj, int teil)
    {
      
      if (hj != null && kurs != null)
      {
        if (kurs.getFach.Data.Isschuelerfach_idNull())
        {
          Log(kurs.Kursbezeichnung + " hat keine schuelerfach_id");
          return;
        }
        //string asvid = kurs == null || kurs.Data.Isschuelerfach_idNull() ? "DUMMY" : kurs.Data.schuelerfach_id;
        einzeldaten.Add(new XElement("Einzeldaten",
          new XElement("BasePruefungsteil",
              new XElement("Note", hj.Punkte),
              new XElement("Schuelerfach", kurs.getFach.Data.schuelerfach_id),
              new XElement("Teil", "1243_" + teil)
          )));
      }
    }
       
    // ==================== HELPER-METHODEN ====================

   
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
          if (hj.getFach.Typ == FachTyp.FPA) c -= 20;
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
      if (fach.Typ == FachTyp.WPF) return "1038_26"; // Wahlpflichtfach
      return "1038_13"; // Pflichtfach
    }

    /// <summary>
    /// Mapping: Halbjahr -> ASV Zeugnisart (1198_XX)
    /// Quelle: Zeugnisart (1038).txt
    /// </summary>
    private string GetZeugnisart(HjLeistung hj)
    {
      int c;
      if (hj.JgStufe == Jahrgangsstufe.Elf) c = 76;
      else if (hj.JgStufe == Jahrgangsstufe.Dreizehn) c = 58;
      else c = 78; // 12. Klasse

      if (hj.Art == HjArt.Hj2)
        c++;

      return "1198_" + c; // Jahreszeugnis FOS/BOS
    }

    /// <summary>
    /// Mapping: Halbjahr -> ASV Jahrgangsstufe (JAHRGA_XXX)
    /// Quelle: X Grundlagen für Drittprogramme.pdf
    /// </summary>
    private string GetJahrgangsstufe(HjLeistung hj)
    {
      return "JAHRGA_" + (int)hj.JgStufe + "1";
      /*
      JAHRGA_111 // Klasse 11
      JAHRGA_121 // Klasse 12
      JAHRGA_131 // Klasse 13
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
      //if (schueler.hatVorkommnis(Vorkommnisart.NichtBestanden)) return "1139_20";
      //return "1139_30";
      return "1139_15";
    }
  }
}