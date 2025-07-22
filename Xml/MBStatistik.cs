using diNo.Xml.Mbstatistik;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace diNo.Xml
{
  /// <summary>
  /// Klasse erstellt die MB-Statistik
  /// </summary>
  public class MBStatistik
  {
    private const string DatenVersion = "abschlusspruefung_1.5";

    public struct SchulartZweig
    {
      public SchulartZweig(string s, Zweig z)
      {       
        S = s;
        Z = z;
      }

      public string S { get; }
      public Zweig Z { get; }

      public override string ToString() => $"({S}, {Z})";
    }

    public static void Serialize(String fileName)
    {
      abschlusspruefungsstatistik ap = new abschlusspruefungsstatistik();

      int KlassenId = 0;
      
      // hier wird die Abschlusspruefungsstatistik zusammengebaut
      schule fos = new schule() { art = schuleArt.FOS, nummer = Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS) };
      schule bos = new schule() { art = schuleArt.BOS, nummer = Zugriff.Instance.getString(GlobaleStrings.SchulnummerBOS) };
      ap.schule = new schule[] { fos, bos };

      List<klasse> fosKlassen12 = new List<klasse>();
      List<klasse> fosKlassen13 = new List<klasse>();
      List<klasse> bosKlassen12 = new List<klasse>();
      List<klasse> bosKlassen13 = new List<klasse>();

      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        if (k.Jahrgangsstufe != Jahrgangsstufe.Zwoelf && k.Jahrgangsstufe != Jahrgangsstufe.Dreizehn)
          continue; // in MB-Statistik nur 12te und 13te Klassen

        // verwaltet alle Teilklasse je nach Schulart und Zweig in Klasse k
        Dictionary<SchulartZweig, klasse> xmlKlassen = new Dictionary<SchulartZweig, klasse>(); // Key = Schulart + Zweig 
        // Liste aller Schüler in dieser Teilklasse
        Dictionary<SchulartZweig, List<schueler>> xmlSchuelerliste = new Dictionary<SchulartZweig, List<schueler>>();
        klasse xmlKlasse;
        
        foreach (var schueler in k.Schueler)
        {
          // nur Schüler in MB-Statistik, die die Prüfung vollständig abgelegt haben
          if (schueler.Status == Schuelerstatus.Abgemeldet || schueler.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen)) // || schueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
            continue;
          
          SchulartZweig sz = new SchulartZweig(schueler.Data.Schulart, schueler.Zweig);
          if (!xmlKlassen.TryGetValue(sz, out xmlKlasse))
          {
            KlassenId++;
            xmlKlasse = CreateXMLKlasse(KlassenId, schueler.Zweig);
            xmlKlassen.Add(sz, xmlKlasse);
            xmlSchuelerliste.Add(sz, new List<schueler>());
            Jahrgangsstufe jg = k.Jahrgangsstufe;
            if (schueler.Data.Schulart == "F")
            {
              if (jg == Jahrgangsstufe.Zwoelf)              
                fosKlassen12.Add(xmlKlasse);
              else if (jg == Jahrgangsstufe.Dreizehn)
                fosKlassen13.Add(xmlKlasse);
            }
            else if (schueler.Data.Schulart == "B")
            {
              if (jg == Jahrgangsstufe.Zwoelf)
                bosKlassen12.Add(xmlKlasse);
              else if (jg == Jahrgangsstufe.Dreizehn)
                bosKlassen13.Add(xmlKlasse);
            }
          }
        
          
          schueler xmlSchueler = new schueler
          {
            nummer = schueler.Id.ToString(),
            grunddaten = new grunddaten()
          };

          List<schueler> szList = xmlSchuelerliste[sz];  // da sollte keine Exception kommen, weil xmlKlassen und xmlSchuelerlist gemeinsam gefüllt werden
          szList.Add(xmlSchueler); // ordne den Schüler der passenden Klasse (nach Schulart + Zweig) zu

          FuelleGrunddaten(schueler, xmlSchueler);

          if (schueler.Fachreferat != null && schueler.Fachreferat.Count > 0)
          {
            var f = schueler.Fachreferat[0].getFach.PlatzInMBStatistik.Split(':');
            xmlSchueler.fachreferat = new fachreferat() { fach = f[f.Length-1], punkte = schueler.Fachreferat[0].Punkte.ToString() };
          }
          if (schueler.Seminarfachnote != null && !schueler.Seminarfachnote.IsGesamtnoteNull())
          {
            xmlSchueler.seminar = new seminar() { punkte = schueler.Seminarfachnote.Gesamtnote.ToString() };
          }
          xmlSchueler.zweite_fremdsprache = getSprache(schueler);

          FuelleAPGrunddaten(schueler, xmlSchueler);

          halbjahresergebnisse hjErg = new halbjahresergebnisse();
          xmlSchueler.Item = hjErg;
          FuelleFpA(schueler, hjErg);

          hjErg.allgemeinbildende_faecher = new allgemeinbildende_faecher();
          hjErg.profilfaecher = new profilfaecher();
          hjErg.wahlpflichtfaecher = new wahlpflichtfaecher();

          foreach (var fach in schueler.getNoten.alleFaecher)
          {
            if (fach.getFach.PlatzInMBStatistik == "sport" && schueler.hatVorkommnis(Vorkommnisart.Sportbefreiung))
              continue;

            FuelleAPWennVorhanden(schueler, xmlSchueler, fach);
            var xmlFachObject = SucheRichtigenXMLKnoten(hjErg, fach);
            if (xmlFachObject != null)
            {
              FuelleHalbjahre(schueler, fach, xmlFachObject);
            }
          }
        }
        
        foreach (var kvp in xmlKlassen)
        {
          // weise das Schueler-Array der richtigen xml-Klasse zu
          kvp.Value.schueler = xmlSchuelerliste[kvp.Key].ToArray();
        }
      }

      if (fosKlassen12.Count > 0)
      {
        fos.jahrgangsstufe12 = new jahrgangsstufe12 { klasse = fosKlassen12.ToArray() };
      }
      if (fosKlassen13.Count > 0)
      {
        fos.jahrgangsstufe13 = new jahrgangsstufe13 { klasse = fosKlassen13.ToArray() };
      }
      if (bosKlassen12.Count > 0)
      {
        bos.jahrgangsstufe12 = new jahrgangsstufe12 { klasse = bosKlassen12.ToArray() };
      }
      if (bosKlassen13.Count > 0)
      {
        bos.jahrgangsstufe13 = new jahrgangsstufe13 { klasse = bosKlassen13.ToArray() };
      }

      XmlSerializer ser = new XmlSerializer(typeof(abschlusspruefungsstatistik));
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
      
      using (XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
        writer.WriteDocType("abschlusspruefungsstatistik", null, DatenVersion + ".dtd", null);
        ser.Serialize(writer, ap, ns);
      }
    }

    private static klasse CreateXMLKlasse(int id, Zweig zweig)
    {
      klasse xmlKlasse = new klasse
      {
        nummer = id.ToString()
      };
      switch (zweig)
      {
        case Zweig.Sozial: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.S; break;
        case Zweig.Technik: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.T; break;
        case Zweig.Umwelt: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.ABU; break;
        case Zweig.Wirtschaft: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.W; break;
        default: throw new InvalidOperationException("klasse ohne Ausbildungsrichtung geht so nicht");
      }

      return xmlKlasse;
    }

    private static object SucheRichtigenXMLKnoten(halbjahresergebnisse hjErg, FachSchuelerNoten fach)
    {
      switch (fach.getFach.Typ)
      {
        case FachTyp.Allgemein: return CreateXMLFachObject(hjErg.allgemeinbildende_faecher, fach.getFach);
        case FachTyp.Profilfach: return CreateXMLFachObject(hjErg.profilfaecher, fach.getFach);
        case FachTyp.WPF: return CreateXMLFachObject(hjErg.wahlpflichtfaecher, fach.getFach);
      }

      return null;
    }

    private static void FuelleHalbjahre(Schueler schueler, FachSchuelerNoten fach, object xmlFachObject)
    {
      List<HjLeistung> leistungen = new List<HjLeistung>();
      leistungen.Add(fach.getVorHjLeistung(HjArt.Hj1));
      leistungen.Add(fach.getVorHjLeistung(HjArt.Hj2));
      var hjLeistungAktuell_1 = fach.getHjLeistung(HjArt.Hj1);
      var hjLeistungAktuell_2 = fach.getHjLeistung(HjArt.Hj2);
      if (hjLeistungAktuell_1 == null && hjLeistungAktuell_2 != null)
      {
        // nicht vorhandene HJLeistungen gehen nicht. Im Zweifelsfall dummy anlegen
        hjLeistungAktuell_1 = new HjLeistung(schueler.Id, fach.getFach, HjArt.Hj1, hjLeistungAktuell_2.JgStufe);
        hjLeistungAktuell_1.Status = HjStatus.Ungueltig;
      }
      if (hjLeistungAktuell_1 != null && hjLeistungAktuell_2 == null)
      {
        // nicht vorhandene HJLeistungen gehen nicht. Im Zweifelsfall dummy anlegen
        hjLeistungAktuell_2 = new HjLeistung(schueler.Id, fach.getFach, HjArt.Hj2, hjLeistungAktuell_1.JgStufe);
        hjLeistungAktuell_2.Status = HjStatus.Ungueltig;
      }

      leistungen.Add(hjLeistungAktuell_1);
      leistungen.Add(hjLeistungAktuell_2);

      List<object> unzuordenbareHalbjahre = new List<object>();

      foreach (var hj in leistungen)
      {
        if (hj == null) { continue; }

        if (hj.JgStufe == Jahrgangsstufe.Elf && hj.Art == HjArt.Hj1)
        {
          SucheProperty("hj_11_1", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ1Xml, true);
        }
        if (hj.JgStufe == Jahrgangsstufe.Elf && hj.Art == HjArt.Hj2)
        {
          SucheProperty("hj_11_2", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ2Xml, true);
        }
        if (hj.JgStufe == Jahrgangsstufe.Zwoelf && hj.Art == HjArt.Hj1)
        {
          if (!SucheProperty("hj_12_1", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ1Xml, true))
          {
            SucheProperty("Item", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ1Xml, false);
          }
        }
        if (hj.JgStufe == Jahrgangsstufe.Zwoelf && hj.Art == HjArt.Hj2)
        {
          if (!SucheProperty("hj_12_2", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ2Xml, true))
          {
            SucheProperty("Item1", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ2Xml, false);
          }
        }
        if (hj.JgStufe == Jahrgangsstufe.Dreizehn && hj.Art == HjArt.Hj1)
        {
          if (!SucheProperty("hj_13_1", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ1Xml, true))
          {
            SucheProperty("Item", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ1Xml, false);
          }
        }
        if (hj.JgStufe == Jahrgangsstufe.Dreizehn && hj.Art == HjArt.Hj2)
        {
          if (!SucheProperty("hj_13_2", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ2Xml, true))
          {
            SucheProperty("Item1", xmlFachObject, unzuordenbareHalbjahre, hj, GetHJ2Xml, false);
          }
        }
      }

      if (unzuordenbareHalbjahre.Count > 0)
      {
        if (xmlFachObject.GetType().GetProperty("Items") != null)
        {
          xmlFachObject.GetType().GetProperty("Items").SetValue(xmlFachObject, unzuordenbareHalbjahre.ToArray());
        }
      }
    }

    private static bool SucheProperty(string propertyName, object xmlFachObject, List<object> unzuordenbareHalbjahre, HjLeistung hj, Func<HjLeistung, object> createFunc, bool addToUnzuordenbare)
    {
      var property = xmlFachObject.GetType().GetProperty(propertyName);
      if (property != null)
      {
        property.SetValue(xmlFachObject, createFunc(hj));
        return true;
      }
      else
      {
        if (addToUnzuordenbare)
        {
          unzuordenbareHalbjahre.Add(createFunc(hj));
        }

        return false;
      }
    }

    private static void FuelleAPWennVorhanden(Schueler unserSchueler, schueler xmlSchueler, FachSchuelerNoten fach)
    {
      // erst Mal nach Abschlussprüfung gucken
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
    }

    private static void FuelleFpA(Schueler unserSchueler, halbjahresergebnisse hjErg)
    {
      var fpaNoten = unserSchueler.getNoten.FindeFach("FpA");
      if (fpaNoten != null)
      {
        var fpa1 = fpaNoten.getVorHjLeistung(HjArt.Hj1);
        var fpa2 = fpaNoten.getVorHjLeistung(HjArt.Hj2);
        if (fpa1 != null && fpa2 != null)
        {
          hjErg.fachpraktische_ausbildung = new fachpraktische_ausbildung
          {
            hj_11_1 = (hj_11_1)GetHJ1Xml(fpa1), //new hj_11_1() { eingebracht = hj_11_1Eingebracht.ja, punkte = fpa1.Punkte.ToString() },
            hj_11_2 = (hj_11_2)GetHJ2Xml(fpa1) //{ eingebracht = hj_11_2Eingebracht.ja, punkte = fpa2.Punkte.ToString() }
          };
        }
      }
    }

    private static void FuelleAPGrunddaten(Schueler unserSchueler, schueler xmlSchueler)
    {
      xmlSchueler.abschlusspruefung = new abschlusspruefung();
      
      if (unserSchueler.hatVorkommnis(Vorkommnisart.ErhaeltNachtermin))
      {
        xmlSchueler.abschlusspruefung.abgelegt = abschlusspruefungAbgelegt.Item1; // abgebrochen - erhaelt nachprüfung
      }
      else if(unserSchueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
      {
        xmlSchueler.abschlusspruefung.abgelegt = abschlusspruefungAbgelegt.Item2; // abgebrochen - selbst schuld
      }
      else
      {
        xmlSchueler.abschlusspruefung.abgelegt = abschlusspruefungAbgelegt.Item0; // vollständig abgelegt
      }

      //xmlSchueler.abschlusspruefung.bestanden = unserSchueler.hatVorkommnis(Vorkommnisart.PruefungNichtBestanden) ? abschlusspruefungBestanden.nein : abschlusspruefungBestanden.ja;
      xmlSchueler.abschlusspruefung.bestanden = unserSchueler.hatVorkommnis(Vorkommnisart.Jahreszeugnis) ? abschlusspruefungBestanden.nein : abschlusspruefungBestanden.ja;
      xmlSchueler.abschlusspruefung.bestandenSpecified = true;
    }

    private static void FuelleGrunddaten(Schueler unserSchueler, schueler xmlSchueler)
    {
      xmlSchueler.grunddaten.geschlecht = unserSchueler.Data.Geschlecht.ToUpper() == "M" ? grunddatenGeschlecht.m : grunddatenGeschlecht.w;
      if (!unserSchueler.Data.IsEintrittAusSchulnummerNull())
      {
        xmlSchueler.grunddaten.herkunftsschule =  unserSchueler.Data.EintrittAusSchulnummer.ToString();
      } 
      if (!unserSchueler.Data.IsAustrittsdatumNull())
      { 
        xmlSchueler.grunddaten.ausgetreten_am = unserSchueler.Data.Austrittsdatum.ToString("dd.MM.yyyy"); 
      }
      if (!unserSchueler.Data.IsDNoteNull())
      {
        xmlSchueler.grunddaten.durchschnittsnote = unserSchueler.Data.DNote.ToString(CultureInfo.InvariantCulture);
      }
      xmlSchueler.grunddaten.wdh_jgst = ErmittleWiederholungskennzahl(unserSchueler, xmlSchueler);
      xmlSchueler.grunddaten.wdh_jgstSpecified = true;
      xmlSchueler.grunddaten.vorbildung = new vorbildung
      {
        besuchte_schulart_vor_eintritt = ErmittleVorherBesuchteSchulart(unserSchueler),
        msa_erworben_an_schulart = ErmittleSchulartDerMittlerenReife(unserSchueler),
      };

      xmlSchueler.grunddaten.vorbildung.m_deutsch = !unserSchueler.Data.IsMittlereReifeDeutschnoteNull() ? unserSchueler.Data.MittlereReifeDeutschnote.ToString() : "0";
      xmlSchueler.grunddaten.vorbildung.m_englisch = !unserSchueler.Data.IsMittlereReifeEnglischnoteNull() ? unserSchueler.Data.MittlereReifeEnglischnote.ToString() : "0";
      xmlSchueler.grunddaten.vorbildung.m_mathematik = !unserSchueler.Data.IsMittlereReifeMathenoteNull() ? unserSchueler.Data.MittlereReifeMathenote.ToString() : "0";
    }

    private static object CreateXMLAPObject(abschlusspruefung parent, Fach fach, Schueler schueler)
    {
      if (string.IsNullOrEmpty(fach.PlatzInMBStatistik))
      {
        return null; // fach hat laut datenbank keine verbindung zur MB-Statistik
      }

      string[] pfadBestandteile = fach.PlatzInMBStatistik.Split(':');

      if (fach.Typ == FachTyp.Profilfach)
      {
        // An dieser Stelle heißen die Profilfächer nicht wie sie heißen sondern nur "Profilfach1" usw...
        int profilfachNr = fach.Sortierung(schueler.Zweig) - 100;
        pfadBestandteile[0] = "profilfach" + profilfachNr;
      }

      // in der AP heißt das, was in den normalen HJ-Leistungen "Item1" heißt nur "Item" !?!
      // und das, was in den HJ-Leistungen "Item" heißt ist hier als eigene Fächer aufgeführt
      if (pfadBestandteile[0] == "Item")
      {
        pfadBestandteile[0] = pfadBestandteile[1]; // klappt für Reli und Ethik
      }
      if (pfadBestandteile[0] == "Item1")
      {
        pfadBestandteile[0] = "Item"; // für Englisch
      }

      var property = parent.GetType().GetProperty(pfadBestandteile[0]);
      if (property == null)
      {
        // suche nach der Property mit "ap_"-Präfix
        property = parent.GetType().GetProperty("ap_" + pfadBestandteile[0]);
        if (property == null)
        {
          throw new InvalidOperationException("Unbekannte Eigenschaft in MB-Statistik: " + pfadBestandteile[0]);
        }
      }

      // erzeuge ein neues Objekt vom gesuchten Typ mittels des Standardkonstruktors - hoffentlich haben das alle
      string neededTypeName = "ap_" + (pfadBestandteile.Length == 1 ? pfadBestandteile[0] : pfadBestandteile[1]);
      Type neededType = Type.GetType("diNo.Xml.Mbstatistik." + neededTypeName, true);
      object newObject = neededType.GetConstructor(new Type[] { }).Invoke(new object[] { });
      property.SetValue(parent, newObject);
      return newObject;
    }

    /// <summary>
    /// Diese Methode macht ziemlich viel Magie. Hoffentlich klappt es auch.
    /// Mittels Reflection wird aus dem Pfad des Faches in der MB-Statistik das passende Objekt
    /// in der MB-Statistik erzeugt und zurückgegeben.
    /// </summary>
    /// <param name="hjErg">MB-Statistik-Objekt als einstieg, z.B. "wahlpflichtfaecher".</param>
    /// <param name="fach">Das Fach aus unserer Fach-Datenbank.</param>
    /// <returns>Das gesuchte Objekt, in welches die Noten eingetragen werden können.</returns>
    private static object CreateXMLFachObject(object parent, Fach fach)
    {
      if (string.IsNullOrEmpty(fach.PlatzInMBStatistik))
      {
        return null; // fach hat laut datenbank keine verbindung zur MB-Statistik
      }

      string[] pfadBestandteile = fach.PlatzInMBStatistik.Split(':');

      var property = parent.GetType().GetProperty(pfadBestandteile[0]);
      if (property == null)
      {
        throw new InvalidOperationException("Unbekannte Eigenschaft in MB-Statistik: " + pfadBestandteile[0]);
      }

      // erzeuge ein neues Objekt vom gesuchten Typ mittels des Standardkonstruktors - hoffentlich haben das alle
      string neededTypeName = pfadBestandteile.Length == 1 ? pfadBestandteile[0] : pfadBestandteile[1];
      Type neededType = Type.GetType("diNo.Xml.Mbstatistik." + neededTypeName, true);
      object newObject = neededType.GetConstructor(new Type[] { }).Invoke(new object[] { });
      if (property.PropertyType.IsArray)
      {
        var array = Array.CreateInstance(neededType, 1);
        array.SetValue(newObject, 0);
        property.SetValue(parent, array); // mal gucken ob das gut geht...
      }
      else
      {
        property.SetValue(parent, newObject);
      }

      return newObject;
    }


    private static void AddHJ(List<object> halbjahre, HjLeistung hj)
    {
      if (hj == null)
      {
        return;
      }
      else if (hj.Art == HjArt.Hj1)
      {
        var hjXml = GetHJ1Xml(hj);
        if (hjXml != null)
        {
          halbjahre.Add(hjXml);
        }
      }
      else if (hj.Art == HjArt.Hj2)
      {
        var hjXml = GetHJ2Xml(hj);
        if (hjXml != null)
        {
          halbjahre.Add(hjXml);
        }
      }
    }

    private static object GetHJ1Xml(HjLeistung hj1)
    {
      switch (hj1.JgStufe)
      {
        case Jahrgangsstufe.Elf: return new hj_11_1() { punkte = hj1.Punkte.ToString(), eingebracht = (hj1.Status == HjStatus.Einbringen) ? hj_11_1Eingebracht.ja : hj_11_1Eingebracht.nein };
        case Jahrgangsstufe.Zwoelf: return new hj_12_1() { punkte = hj1.Punkte.ToString(), eingebracht = (hj1.Status == HjStatus.Einbringen) ? hj_12_1Eingebracht.ja : hj_12_1Eingebracht.nein };
        case Jahrgangsstufe.Dreizehn: return new hj_13_1() { punkte = hj1.Punkte.ToString(), eingebracht = (hj1.Status == HjStatus.Einbringen) ? hj_13_1Eingebracht.ja : hj_13_1Eingebracht.nein };
        default: return null;   // andere Jahrgangsstufen werden ignoriert
      }
    }

    private static object GetHJ2Xml(HjLeistung hj2)
    {
      switch (hj2.JgStufe)
      {
        case Jahrgangsstufe.Elf: return new hj_11_2() { punkte = hj2.Punkte.ToString(), eingebracht = (hj2.Status == HjStatus.Einbringen) ? hj_11_2Eingebracht.ja : hj_11_2Eingebracht.nein };
        case Jahrgangsstufe.Zwoelf: return new hj_12_2() { punkte = hj2.Punkte.ToString(), eingebracht = (hj2.Status == HjStatus.Einbringen) ? hj_12_2Eingebracht.ja : hj_12_2Eingebracht.nein };
        case Jahrgangsstufe.Dreizehn: return new hj_13_2() { punkte = hj2.Punkte.ToString(), eingebracht = (hj2.Status == HjStatus.Einbringen) ? hj_13_2Eingebracht.ja : hj_13_2Eingebracht.nein };
        default: return null;  // andere Jahrgangsstufen werden ignoriert
      }
    }

    private static zweite_fremdsprache getSprache(Schueler schueler)
    {
      if (schueler.HatZweiteFremdsprache())
      {
        zweite_fremdsprache result = new zweite_fremdsprache();
        switch (schueler.Data.AndereFremdspr2Art)
        {
          case (int)ZweiteFSArt.RS: result.art = zweite_fremdspracheArt.VN; break;
          case (int)ZweiteFSArt.FFAlt: result.art = zweite_fremdspracheArt.UT;break;
          case (int)ZweiteFSArt.ErgPr: result.art = zweite_fremdspracheArt.EP; break;
        }

        if (!schueler.Data.IsAndereFremdspr2FachNull() && !schueler.Data.IsAndereFremdspr2NoteNull())
        {
          // dies dürfte der Fall sein bei Ersatzprüfungen oder Noten aus der vorigen Schule
          Fach sprachfach = new Fach(schueler.Data.AndereFremdspr2Fach);
          result.sprache = GetSprache(sprachfach);
          result.Item = new note() { gesamt = schueler.Data.AndereFremdspr2Note.ToString() };
          return result;
        }
        else
        {
          unterricht unterricht = new unterricht();
          // Schüler muss irgendwas bei uns gehabt haben
          if (schueler.Data.AndereFremdspr2Art == (int)ZweiteFSArt.FFAlt)
          {
            result.sprache = GetSprache(schueler.getNoten.ZweiteFSalt.getFach);
            //TODO: stimmt das so ungefähr?
            unterricht.halbjahr1 = schueler.getNoten.ZweiteFSalt.getHjLeistung(HjArt.Hj1).Punkte.ToString();
            unterricht.halbjahr2 = schueler.getNoten.ZweiteFSalt.getHjLeistung(HjArt.Hj2).Punkte.ToString();
          }
          else
          {
            // Schüler muss dieses Jahr Französisch bei uns gehabt haben
            var zweitsprachen = new List<FachSchuelerNoten> (schueler.getNoten.alleSprachen);
            zweitsprachen.RemoveAll(x => x.getFach.Kuerzel == "E");
            //if (zweitsprachen.Count != 1)
            //  throw new InvalidOperationException("Schüler hat mehr als 1 Zweitsprache.");
            result.sprache = GetSprache(zweitsprachen[0].getFach);
            unterricht.halbjahr1 = zweitsprachen[0].getHjLeistung(HjArt.Hj1).Punkte.ToString();
            unterricht.halbjahr2 = zweitsprachen[0].getHjLeistung(HjArt.Hj2).Punkte.ToString();
          }
          result.Item = unterricht;
        }

        return result;
       
      }

      return null;

      /* TODO: AndereFremdspr2Art abfragen und Sprache aus AndereFremdspr2Text entnehmen!
      if (schueler.Data.IsAndereFremdspr2TextNull() || schueler.Data.IsAndereFremdspr2NoteNull())
      {
        return null;
      }

      zweite_fremdsprache result = new zweite_fremdsprache() { art = zweite_fremdspracheArt.VN };
      switch (schueler.Fremdsprache2)
      {
        case "It": result.sprache = zweite_fremdspracheSprache.Italienisch; break;
        case "F": result.sprache = zweite_fremdspracheSprache.Franzoesisch; break;
        case "L": result.sprache = zweite_fremdspracheSprache.Latein; break;
        case "Ru": result.sprache = zweite_fremdspracheSprache.Russisch; break;
        case "Sp": result.sprache = zweite_fremdspracheSprache.Spanisch; break;
      }

      result.note = new note() { gesamt = schueler.Data.AndereFremdspr2Note.ToString() };
      return result;*/
    }

    private static zweite_fremdspracheSprache GetSprache(Fach sprachfach)
    {
      switch (sprachfach.Kuerzel)
      {
        case "It": return zweite_fremdspracheSprache.Italienisch;
        case "F": return zweite_fremdspracheSprache.Franzoesisch;
        case "F-f": return zweite_fremdspracheSprache.Franzoesisch;
        case "L": return zweite_fremdspracheSprache.Latein;
        case "Ru": return zweite_fremdspracheSprache.Russisch;
        case "Sp": return zweite_fremdspracheSprache.Spanisch;
        default: throw new InvalidOperationException("unbekannte Sprache: "+ sprachfach.Kuerzel);
      }
    }

    private static vorbildungMsa_erworben_an_schulart ErmittleSchulartDerMittlerenReife(Schueler unserSchueler)
    {
      if (unserSchueler.Data.IsEintrittAusSchulartNull() || string.IsNullOrEmpty(unserSchueler.Data.EintrittAusSchulart))
      {
        return vorbildungMsa_erworben_an_schulart.SO;
      }

      switch (unserSchueler.Data.EintrittAusSchulart)
      {
        case "GY": return vorbildungMsa_erworben_an_schulart.GY;
        case "RS": return vorbildungMsa_erworben_an_schulart.RS;
        case "MS": return vorbildungMsa_erworben_an_schulart.MS;
        case "BS": return vorbildungMsa_erworben_an_schulart.BS;
        case "WS": return vorbildungMsa_erworben_an_schulart.WS;
        case "BO": return vorbildungMsa_erworben_an_schulart.BO;
        case "SO": return vorbildungMsa_erworben_an_schulart.SO;
        case "KS": return vorbildungMsa_erworben_an_schulart.KS;
      }

      return vorbildungMsa_erworben_an_schulart.SO;
    }

    private static bool wiederholt(Schueler s, string jg)
    {
        bool wh;
        wh = !s.Data.IsWiederholung1JahrgangsstufeNull() && s.Data.Wiederholung1Jahrgangsstufe == jg;
        wh = wh || !s.Data.IsWiederholung2JahrgangsstufeNull() && s.Data.Wiederholung2Jahrgangsstufe == jg;
        return wh;
    }

    private static grunddatenWdh_jgst ErmittleWiederholungskennzahl(Schueler unserSchueler, schueler xmlSchueler)
    {
      bool wdh11 = wiederholt(unserSchueler, "11");
      bool wdh12 = wiederholt(unserSchueler, "12");
      bool wdh13 = wiederholt(unserSchueler, "13");

            /*Wiederholung von...
              O: keiner Jgst
              1: 11.Jgst
              2: 12.Jgst
              3: 13.Jgst
              4: 11.und 12.Jgst
              5: 11.und 13.Jgst
              6: 12.und 13.Jgst
             */

      if (wdh11 && !wdh12 && !wdh13) return grunddatenWdh_jgst.Item1;
      if (!wdh11 && wdh12 && !wdh13) return grunddatenWdh_jgst.Item2;
      if (!wdh11 && !wdh12 && wdh13) return grunddatenWdh_jgst.Item3;
      if (wdh11 && wdh12 && !wdh13) return grunddatenWdh_jgst.Item4;
      if (wdh11 && !wdh12 && wdh13) return grunddatenWdh_jgst.Item5;
      if (!wdh11 && wdh12 && wdh13) return grunddatenWdh_jgst.Item6;

      return grunddatenWdh_jgst.Item0;
    }

    private static vorbildungBesuchte_schulart_vor_eintritt ErmittleVorherBesuchteSchulart(Schueler unserSchueler)
    {
      if (unserSchueler.Data.IsSchulischeVorbildungNull() || string.IsNullOrEmpty(unserSchueler.Data.SchulischeVorbildung))
      {
        return vorbildungBesuchte_schulart_vor_eintritt.SO;
      }

      vorbildungBesuchte_schulart_vor_eintritt vorherBesuchteSchulart = vorbildungBesuchte_schulart_vor_eintritt.SO;
      switch (unserSchueler.Data.SchulischeVorbildung)
      {
        case "BFo":
        case "BFS":
        case "BP":
        case "BSo": vorherBesuchteSchulart = vorbildungBesuchte_schulart_vor_eintritt.BS; break;
        case "GY0":
        case "GY1": vorherBesuchteSchulart = vorbildungBesuchte_schulart_vor_eintritt.GY; break;
        case "H":
        case "HSo":
        case "HSq":
        case "M": vorherBesuchteSchulart = vorbildungBesuchte_schulart_vor_eintritt.MS; break;
        case "F10": vorherBesuchteSchulart = vorbildungBesuchte_schulart_vor_eintritt.BO; break; // FOS10 = Berufliche Oberschule
        case "R3a":
        case "R3b":
        case "RS":
        case "RS1":
        case "RS2":
        case "RS3": vorherBesuchteSchulart = vorbildungBesuchte_schulart_vor_eintritt.RS; break;
        case "WS":
        case "WSH":
        case "WSM": vorherBesuchteSchulart = vorbildungBesuchte_schulart_vor_eintritt.WS; break;
          //TODO: FAo, QB, SoM, VSo und was ist besuchte Schulart KS?
      }
      return vorherBesuchteSchulart;
    }
  }
}
