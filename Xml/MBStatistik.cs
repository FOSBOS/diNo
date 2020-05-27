using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using diNo.Xml.Mbstatistik;

namespace diNo.Xml
{
  /// <summary>
  /// Klasse erstellt die MB-Statistik
  /// </summary>
  public class MBStatistik
  {
    public static void Serialize(String fileName)
    {
      abschlusspruefungsstatistik ap = new abschlusspruefungsstatistik();

      int hoechsteKlassenId = 0;
      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        hoechsteKlassenId = k.GetId() > hoechsteKlassenId ? k.GetId() : hoechsteKlassenId;
      }

      // hier wird die Abschlusspruefungsstatistik zusammengebaut
      schule fos = new schule() { art = schuleArt.FOS, nummer = "0871" };
      schule bos = new schule() { art = schuleArt.BOS, nummer = "0841" };
      ap.schule = new schule[] { fos, bos };

      List<klasse> fosKlassen12 = new List<klasse>();
      List<klasse> fosKlassen13 = new List<klasse>();
      List<klasse> bosKlassen12 = new List<klasse>();
      List<klasse> bosKlassen13 = new List<klasse>();

      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        if (k.Jahrgangsstufe != Jahrgangsstufe.Zwoelf /*&& k.Jahrgangsstufe != Jahrgangsstufe.Dreizehn*/)
        {
          continue; // in MB-Statistik nur 12te und 13te Klassen
        }

        Dictionary<Zweig, klasse> xmlKlassen = new Dictionary<Zweig, klasse>();
        Dictionary<Zweig, List<schueler>> schuelerDict = new Dictionary<Zweig, List<schueler>>();
        foreach (var s in k.getSchueler)
        {
          Schueler schueler = new Schueler(s);
          if (!xmlKlassen.ContainsKey(schueler.Zweig))
          {
            //die erste Teilklasse erhält einfach die Id der Klasse. Nur bei Mischklassen künstliche Ids.
            int id = k.GetId();
            if (xmlKlassen.Count > 0)
            {
              hoechsteKlassenId++;
              id = hoechsteKlassenId;
            }

            xmlKlassen.Add(schueler.Zweig, CreateXMLKlasse(id, schueler.Zweig));
            schuelerDict.Add(schueler.Zweig, new List<schueler>());
          }
        }

        if (k.Schulart == Schulart.FOS)
        {
          if (k.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          {
            fosKlassen12.AddRange(xmlKlassen.Values);
          }
          else if (k.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          {
            fosKlassen13.AddRange(xmlKlassen.Values);
          }
        }
        else if (k.Schulart == Schulart.BOS)
        {
          if (k.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          {
            bosKlassen12.AddRange(xmlKlassen.Values);
          }
          else if (k.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          {
            bosKlassen13.AddRange(xmlKlassen.Values);
          }
        }

        foreach (diNoDataSet.SchuelerRow s in k.getSchueler)
        {
          Schueler unserSchueler = new Schueler(s);

          if (unserSchueler.Status == Schuelerstatus.Aktiv && !unserSchueler.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen) && !unserSchueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
          {
            // nur Schüler in MB-Statistik, die die Prüfung vollständig abgelegt haben

            schueler xmlSchueler = new schueler
            {
              nummer = unserSchueler.Id.ToString(),
              grunddaten = new grunddaten()
            };

            schuelerDict[unserSchueler.Zweig].Add(xmlSchueler);
            FuelleGrunddaten(unserSchueler, xmlSchueler);

            if (unserSchueler.Fachreferat != null && unserSchueler.Fachreferat.Count > 0)
            {
              xmlSchueler.fachreferat = new fachreferat() { fach = unserSchueler.Fachreferat[0].getFach.Kuerzel, punkte = unserSchueler.Fachreferat[0].Punkte.ToString() };
            }
            if (unserSchueler.Seminarfachnote != null && !unserSchueler.Seminarfachnote.IsGesamtnoteNull())
            {
              xmlSchueler.seminar = new seminar() { punkte = unserSchueler.Seminarfachnote.Gesamtnote.ToString() };
            }
            xmlSchueler.zweite_fremdsprache = getSprache(unserSchueler);

            FuelleAPGrunddaten(unserSchueler, xmlSchueler);

            halbjahresergebnisse hjErg = new halbjahresergebnisse();
            xmlSchueler.Item = hjErg;
            FuelleFpA(unserSchueler, hjErg);

            hjErg.allgemeinbildende_faecher = new allgemeinbildende_faecher();
            hjErg.profilfaecher = new profilfaecher();
            hjErg.wahlpflichtfaecher = new wahlpflichtfaecher();

            foreach (var fach in unserSchueler.getNoten.alleFaecher)
            {
              if (fach.getFach.PlatzInMBStatistik == "sport" && unserSchueler.hatVorkommnis(Vorkommnisart.Sportbefreiung))
                continue;

              FuelleAPWennVorhanden(unserSchueler, xmlSchueler, fach);
              var xmlFachObject = SucheRichtigenXMLKnoten(hjErg, fach);
              if (xmlFachObject != null)
              {
                FuelleHalbjahre(fach, xmlFachObject);
              }
            }
          }
        }

        foreach (var kvp in xmlKlassen)
        {
          // weise das Schueler-Array der richtigen xml-Klasse zu
          kvp.Value.schueler = schuelerDict[kvp.Key].ToArray();
        }
      }

      fos.jahrgangsstufe12 = new jahrgangsstufe12 { klasse = fosKlassen12.ToArray() };
      fos.jahrgangsstufe13 = new jahrgangsstufe13 { klasse = fosKlassen13.ToArray() };
      bos.jahrgangsstufe12 = new jahrgangsstufe12 { klasse = bosKlassen12.ToArray() };
      bos.jahrgangsstufe13 = new jahrgangsstufe13 { klasse = bosKlassen13.ToArray() };

      using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        XmlSerializer ser = new XmlSerializer(typeof(abschlusspruefungsstatistik));
        ser.Serialize(stream, ap);
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

    private static void FuelleHalbjahre(FachSchuelerNoten fach, object xmlFachObject)
    {
      List<HjLeistung> leistungen = new List<HjLeistung>();
      leistungen.Add(fach.getVorHjLeistung(HjArt.Hj1));
      leistungen.Add(fach.getVorHjLeistung(HjArt.Hj2));
      leistungen.Add(fach.getHjLeistung(HjArt.Hj1));
      leistungen.Add(fach.getHjLeistung(HjArt.Hj2));

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

    private static bool SucheProperty(string propertyName, object xmlFachObject, List<object> unzuordenbareHalbjahre, HjLeistung hj, Func<HjLeistung, object> createFunc, bool addToUnzuordenbare )
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
      var fpaNoten = unserSchueler.getNoten.FindeFach("FpA", false);
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
      if (unserSchueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
      {
        if (unserSchueler.hatVorkommnis(Vorkommnisart.ErhaeltNachtermin))
        {
          xmlSchueler.abschlusspruefung.abgelegt = abschlusspruefungAbgelegt.Item1; // abgebrochen - erhaelt nachprüfung
        }
        else
        {
          xmlSchueler.abschlusspruefung.abgelegt = abschlusspruefungAbgelegt.Item2; // abgebrochen - selbst schuld
        }
      }
      else
      {
        xmlSchueler.abschlusspruefung.abgelegt = abschlusspruefungAbgelegt.Item0; // vollständig abgelegt
      }

      xmlSchueler.abschlusspruefung.bestanden = unserSchueler.hatVorkommnis(Vorkommnisart.PruefungNichtBestanden) ? abschlusspruefungBestanden.nein : abschlusspruefungBestanden.ja;
      xmlSchueler.abschlusspruefung.bestandenSpecified = true;
    }

    private static void FuelleGrunddaten(Schueler unserSchueler, schueler xmlSchueler)
    {
      xmlSchueler.grunddaten.geschlecht = unserSchueler.Data.Geschlecht == "m" ? grunddatenGeschlecht.m : grunddatenGeschlecht.w;
      xmlSchueler.grunddaten.herkunftsschule = unserSchueler.Data.IsEintrittAusSchulnummerNull() ? "" : unserSchueler.Data.EintrittAusSchulnummer.ToString();
      xmlSchueler.grunddaten.ausgetreten_am = unserSchueler.Data.IsAustrittsdatumNull() ? "" : unserSchueler.Data.Austrittsdatum.ToString("dd.mm.yyyy");
      xmlSchueler.grunddaten.durchschnittsnote = unserSchueler.Data.IsDNoteNull() ? "" : unserSchueler.Data.DNote.ToString();
      xmlSchueler.grunddaten.wdh_jgst = ErmittleWiederholungskennzahl(unserSchueler, xmlSchueler);
      xmlSchueler.grunddaten.wdh_jgstSpecified = true;
      xmlSchueler.grunddaten.vorbildung = new vorbildung
      {
        besuchte_schulart_vor_eintritt = ErmittleVorherBesuchteSchulart(unserSchueler),
        msa_erworben_an_schulart = ErmittleSchulartDerMittlerenReife(unserSchueler),
        // wenn ein Schüler keine Vornote mitbringt ist doch 5 eine gute Schätzung
        m_deutsch = unserSchueler.Data.IsMittlereReifeDeutschnoteNull() ? "5" : unserSchueler.Data.MittlereReifeDeutschnote.ToString(),
        m_englisch = unserSchueler.Data.IsMittlereReifeEnglischnoteNull() ? "5" : unserSchueler.Data.MittlereReifeEnglischnote.ToString(),
        m_mathematik = unserSchueler.Data.IsMittlereReifeMathenoteNull() ? "5" : unserSchueler.Data.MittlereReifeMathenote.ToString()
      };
    }

    private static object CreateXMLAPObject (abschlusspruefung parent, Fach fach, Schueler schueler)
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
      string neededTypeName = "ap_"+(pfadBestandteile.Length == 1 ? pfadBestandteile[0] : pfadBestandteile[1]);
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
      Type neededType = Type.GetType("diNo.Xml.Mbstatistik."+neededTypeName, true);
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
        case Jahrgangsstufe.Dreizehn: return  new hj_13_1() { punkte = hj1.Punkte.ToString(), eingebracht = (hj1.Status == HjStatus.Einbringen) ? hj_13_1Eingebracht.ja : hj_13_1Eingebracht.nein };
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

    private static grunddatenWdh_jgst ErmittleWiederholungskennzahl(Schueler unserSchueler, schueler xmlSchueler)
    {
      bool wdh11 = unserSchueler.Data.Wiederholung1Jahrgangsstufe == "11" || unserSchueler.Data.Wiederholung2Jahrgangsstufe == "11";
      bool wdh12 = unserSchueler.Data.Wiederholung1Jahrgangsstufe == "12" || unserSchueler.Data.Wiederholung2Jahrgangsstufe == "12";
      bool wdh13 = unserSchueler.Data.Wiederholung1Jahrgangsstufe == "13" || unserSchueler.Data.Wiederholung2Jahrgangsstufe == "13";

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
