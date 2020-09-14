using diNo.Xml.Schulerfolgsstatistik;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace diNo.Xml
{
  public class SEStatistik
  {
    public static void Serialize(String fileName)
    {
      schulerfolg se = new schulerfolg();

      // ermittle nächste freie KlassenId (für Mischklassen werden künstliche IDs benötigt)
      int hoechsteKlassenId = 0;
      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        hoechsteKlassenId = k.GetId() > hoechsteKlassenId ? k.GetId() : hoechsteKlassenId;
      }

      schule fos = new schule() { art = schuleArt.FOS, nummer = Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS) };
      schule bos = new schule() { art = schuleArt.BOS, nummer = Zugriff.Instance.getString(GlobaleStrings.SchulnummerBOS) };

      se.schule = new schule[] { fos, bos };

      List<klasse> fosKlassen11 = new List<klasse>();
      List<klasse> fosKlassen12 = new List<klasse>();
      List<klasse> fosKlassen13 = new List<klasse>();
      // List<klasse> bosKlassen11 = new List<klasse>(); BOS11 soll nicht gemeldet werden
      List<klasse> bosKlassen12 = new List<klasse>();
      List<klasse> bosKlassen13 = new List<klasse>();

      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        Dictionary<Zweig, klasse> xmlKlassen = new Dictionary<Zweig, klasse>();
        Dictionary<Zweig, List<schueler>> schuelerDict = new Dictionary<Zweig, List<schueler>>();
        foreach (var s in k.getSchueler)
        {
          Schueler schueler = new Schueler(s);

          if (!SchuelerMussInStatistik(schueler))
            continue;

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
          if (k.Jahrgangsstufe == Jahrgangsstufe.Elf)
          {
            fosKlassen11.AddRange(xmlKlassen.Values);
          }
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
          if (k.Jahrgangsstufe == Jahrgangsstufe.Elf || k.Jahrgangsstufe == Jahrgangsstufe.Vorklasse)
          {
            // bosKlassen11.AddRange(xmlKlassen.Values);
          }
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

          if (SchuelerMussInStatistik(unserSchueler))
          {
            schueler xmlSchueler = new schueler
            {
              nummer = unserSchueler.Id.ToString(),
              grunddaten = new grunddaten()
            };

            schuelerDict[unserSchueler.Zweig].Add(xmlSchueler);
            FuelleGrunddaten(unserSchueler, xmlSchueler);
          }
        }

        foreach (var kvp in xmlKlassen)
        {
          // weise das Schueler-Array der richtigen xml-Klasse zu
          kvp.Value.schueler = schuelerDict[kvp.Key].ToArray();
        }
        if (fosKlassen11.Count > 0)
        {
          fos.jahrgangsstufe11 = new jahrgangsstufe11 { klasse = fosKlassen11.ToArray() };
        }
        if (fosKlassen12.Count > 0)
        {
          fos.jahrgangsstufe12 = new jahrgangsstufe12 { klasse = fosKlassen12.ToArray() };
        }
        if (fosKlassen13.Count > 0)
        {
          fos.jahrgangsstufe13 = new jahrgangsstufe13 { klasse = fosKlassen13.ToArray() };
        }
        // bos.jahrgangsstufe11 = new jahrgangsstufe11 { klasse = bosKlassen11.ToArray() };
        if (bosKlassen12.Count > 0)
        {
          bos.jahrgangsstufe12 = new jahrgangsstufe12 { klasse = bosKlassen12.ToArray() };
        }
        if (bosKlassen13.Count > 0)
        {
          bos.jahrgangsstufe13 = new jahrgangsstufe13 { klasse = bosKlassen13.ToArray() };
        }
      }

      XmlSerializer ser = new XmlSerializer(typeof(schulerfolg));
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
      ns.Add("", "http://tempuri.org/schulerfolg_1.1");
      using (XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
        writer.WriteDocType("schulerfolg", null, "schulerfolg_1.1.dtd", null);
        ser.Serialize(writer, se, ns);
      }
    }

    private static bool SchuelerMussInStatistik (Schueler schueler)
    {
      if (schueler.getKlasse.Schulart == Schulart.FOS)
      {
        if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
          return true; // 11.Klassen immer melden

        if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf || schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
        {
          // 12. und 13. Klassen nur diejenigen melden, die ausgetreten sind bzw. die Prüfung nicht abgelegt haben
          if (schueler.Status == Schuelerstatus.Abgemeldet || schueler.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen) || schueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
            return true;
        }

        return false;
      }
      else
      {
        // in der BOS nur bei 12. und 13. Klassen die ausgetretenen bzw. nicht zur Prüfung angetretetenen
        if (schueler.Status == Schuelerstatus.Abgemeldet || schueler.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen) || schueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
          return true;
      }

      return false;
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
        case Zweig.None: break; // IV hat keine Ausbildungsrichtung
        default: throw new InvalidOperationException("klasse ohne Ausbildungsrichtung geht so nicht");
      }

      return xmlKlasse;
    }

    private static void FuelleGrunddaten(Schueler unserSchueler, schueler xmlSchueler)
    {
      xmlSchueler.grunddaten.geschlecht = unserSchueler.Data.Geschlecht == "m" ? grunddatenGeschlecht.m : grunddatenGeschlecht.w;
      if (!unserSchueler.Data.IsProbezeitBisNull())
      {
        xmlSchueler.grunddaten.pz_bis = unserSchueler.Data.ProbezeitBis.ToString("dd.MM.yyyy");
        // jetzt noch schauen, ob er die Probezeit auch bestanden hat
        xmlSchueler.grunddaten.pz_bestanden = (unserSchueler.hatVorkommnis(Vorkommnisart.ProbezeitNichtBestanden)) ? grunddatenPz_bestanden.nein : grunddatenPz_bestanden.ja;
        xmlSchueler.grunddaten.pz_bestandenSpecified = true;
      }
      if (!unserSchueler.Data.IsAustrittsdatumNull())
      {
        xmlSchueler.grunddaten.ausgetreten_am = unserSchueler.Data.Austrittsdatum.ToString("dd.MM.yyyy");
      }
      
      xmlSchueler.grunddaten.jgst_bestanden = grunddatenJgst_bestanden.ja;
      if (unserSchueler.Status == Schuelerstatus.Abgemeldet || unserSchueler.hatVorkommnis(Vorkommnisart.ProbezeitNichtBestanden) || unserSchueler.hatVorkommnis(Vorkommnisart.KeineVorrueckungserlaubnis) || unserSchueler.hatVorkommnis(Vorkommnisart.NichtBestanden) || unserSchueler.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen) || unserSchueler.hatVorkommnis(Vorkommnisart.nichtBestandenMAPnichtZugelassen) || unserSchueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
      {
        xmlSchueler.grunddaten.jgst_bestanden = grunddatenJgst_bestanden.nein;
      }

      if (unserSchueler.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen))
      {
        xmlSchueler.grunddaten.pruefung = "0"; // 0 = nicht zugelassen
      }
      if (unserSchueler.hatVorkommnis(Vorkommnisart.PruefungAbgebrochen))
      {
        xmlSchueler.grunddaten.pruefung = "1"; // 1 = abgebrochen
      }
    }
  }
}
