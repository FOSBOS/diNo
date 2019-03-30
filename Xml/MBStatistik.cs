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

      // hier wird die Abschlusspruefungsstatistik zusammengebaut
      schule fos = new schule() { art = schuleArt.FOS,  nummer="0871" };
      schule bos = new schule() { art = schuleArt.BOS, nummer = "0841" };
      ap.schule = new schule[] { fos, bos };

      List<klasse> fosKlassen12 = new List<klasse>();
      List<klasse> fosKlassen13 = new List<klasse>();
      List<klasse> bosKlassen12 = new List<klasse>();
      List<klasse> bosKlassen13 = new List<klasse>();

      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        klasse xmlKlasse = new klasse
        {
          nummer = k.GetId().ToString()
        };
        switch (k.Zweig)
        {
          case Zweig.Sozial: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.ABU; break;
          case Zweig.Technik: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.T; break;
          case Zweig.Umwelt: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.ABU; break;
          case Zweig.Wirtschaft: xmlKlasse.ausbildungsrichtung = klasseAusbildungsrichtung.W; break;
          default: throw new InvalidOperationException("klasse ohne Ausbildungsrichtung geht so nicht");
        }

        // betrachte nur zwölfte und dreizehnte Klassen
        if (k.Schulart == Schulart.FOS)
        {
          if (k.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          {
            fosKlassen12.Add(xmlKlasse);
          }
          else if (k.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          {
            fosKlassen13.Add(xmlKlasse);
          }
        }
        else if (k.Schulart == Schulart.BOS)
        {
          if (k.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          {
            bosKlassen12.Add(xmlKlasse);
          }
          else if (k.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          {
            bosKlassen13.Add(xmlKlasse);
          }
        }

        foreach (diNoDataSet.SchuelerRow s in k.getSchueler)
        {
          Schueler unserSchueler = new Schueler(s);
          schueler xmlSchueler = new schueler
          {
            nummer = unserSchueler.Id.ToString(),
            grunddaten = new grunddaten()
          };
          xmlSchueler.grunddaten.geschlecht = unserSchueler.Data.Geschlecht == "m" ? grunddatenGeschlecht.m : grunddatenGeschlecht.w;
          xmlSchueler.grunddaten.herkunftsschule = unserSchueler.Data.EintrittAusSchulnummer.ToString();
          xmlSchueler.grunddaten.ausgetreten_am = unserSchueler.Data.Austrittsdatum.ToString("dd.mm.yyyy");
          xmlSchueler.grunddaten.durchschnittsnote = unserSchueler.Data.DNote.ToString();
          xmlSchueler.grunddaten.wdh_jgst = ErmittleWiederholungskennzahl(unserSchueler, xmlSchueler);
          xmlSchueler.grunddaten.wdh_jgstSpecified = xmlSchueler.grunddaten.wdh_jgst != grunddatenWdh_jgst.Item0;
          xmlSchueler.grunddaten.vorbildung = new vorbildung
          {
            besuchte_schulart_vor_eintritt = ErmittleVorherBesuchteSchulart(unserSchueler),
            msa_erworben_an_schulart = ErmittleSchulartDerMittlerenReife(unserSchueler),
            m_deutsch = unserSchueler.Data.MittlereReifeDeutschnote.ToString(),
            m_englisch = unserSchueler.Data.MittlereReifeEnglischnote.ToString(),
            m_mathematik = unserSchueler.Data.MittlereReifeMathenote.ToString()
          };
          if (unserSchueler.Fachreferat != null && unserSchueler.Fachreferat.Count > 0)
          {
            xmlSchueler.fachreferat = new fachreferat() { fach = unserSchueler.Fachreferat[0].getFach.Kuerzel, punkte = unserSchueler.Fachreferat[0].Punkte.ToString() };
          }
          if (unserSchueler.Seminarfachnote != null)
          {
            xmlSchueler.seminar = new seminar() { punkte = unserSchueler.Seminarfachnote.Gesamtnote.ToString() };
          }
          xmlSchueler.zweite_fremdsprache = getSprache(unserSchueler);

          xmlSchueler.abschlusspruefung = new abschlusspruefung();

          halbjahresergebnisse hjErg = new halbjahresergebnisse();
          xmlSchueler.Item = hjErg;

          hjErg.allgemeinbildende_faecher = new allgemeinbildende_faecher(); //TODO
          hjErg.fachpraktische_ausbildung = new fachpraktische_ausbildung(); //TODO
          hjErg.profilfaecher = new profilfaecher(); //TODO
          hjErg.wahlpflichtfaecher = new wahlpflichtfaecher (); //TODO
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

    private static zweite_fremdsprache getSprache(Schueler schueler)
    {
      if (string.IsNullOrEmpty(schueler.Fremdsprache2))
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
      return result;
    }

    private static vorbildungMsa_erworben_an_schulart ErmittleSchulartDerMittlerenReife(Schueler unserSchueler)
    {
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
