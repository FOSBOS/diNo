using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public enum Bericht
  {
    Notenmitteilung,
    Gefaehrdung,
    Einbringung,
    Abiergebnisse,
    Bescheinigung,
    Zwischenzeugnis,
    Jahreszeugnis,
    Abiturzeugnis,
    Notenbogen,
    Klassenliste,
    EinserAbi
  }

  public enum UnterschriftZeugnis
  {
    SL = 0, // Chef unterschreibt das Zeugnis
    Stv = 1, // Konrektor
    gez = 2 // ohne Unterschrift, nur mit gez. Chef
  }

  // Basisklasse für Berichtsdaten von Notenbögen und Zeugnissen
  public class SchuelerDruck
  {
    public int Id { get; private set; }
    public string Nachname { get; private set; }
    public string Vorname { get; private set; }
    public string Rufname { get; private set; }
    public string Anrede { get; private set; }
    public string Klasse { get; private set; }

    public string Klassenleiter { get; private set; }
    public string KlassenleiterText { get; private set; }
    public string Schuljahr { get; private set; }
    public string Schulart { get; private set; }

    // Zeugnisbemerkung muss im Bericht als HTML eingestellt sein (re. Maus auf Datenfeld)
    public string Bemerkung { get; protected set; }

    public bool HideHj2 { get; private set; } // wird als Parameter an den Unterbericht gereicht
    public bool HideVorHj { get; private set; }
    public bool HideAbi { get; private set; }
    public byte jg { get; private set; }
    public string JgKurz { get; private set; }

    public SchuelerDruck(Schueler s, Bericht Berichtsname)
    {
      jg = (byte)s.getKlasse.Jahrgangsstufe;
      if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.IntVk) JgKurz = "IV";
      else if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Vorklasse) JgKurz = "Vk";
      else JgKurz = jg.ToString();

      Id = s.Id;//.ToString();
      Nachname = s.Name;
      Vorname = s.Vorname;
      Rufname = s.Data.Rufname;
      Anrede = s.getHerrFrau();
      Klasse = s.getKlasse.Bezeichnung;

      var KL = s.getKlasse.Klassenleiter;
      Klassenleiter = KL.NameDienstbezeichnung; // + (s.AlteFOBOSO() && Berichtsname == Bericht.Notenbogen ? "" : "\n" + KL.KLString);
      KlassenleiterText = KL.KLString;

      if (s.Data.Schulart == "B")
        Schulart = "Staatliche Berufsoberschule Kempten (Allgäu)";
      else
        Schulart = "Staatliche Fachoberschule Kempten (Allgäu)";

      Schuljahr = "Schuljahr " + Zugriff.Instance.Schuljahr + "/" + (Zugriff.Instance.Schuljahr + 1);

      HideHj2 = Zugriff.Instance.aktHalbjahr == Halbjahr.Erstes;
      HideVorHj = !s.hatVorHj; // nur bei F12 anzeigen
      HideAbi = jg < 12 || Zugriff.Instance.aktZeitpunkt <= (int)Zeitpunkt.ErstePA;
    }

    public static string GetBerichtsname(Bericht b)
    {
      switch (b)
      {
        case Bericht.Notenbogen: return "rptNotenbogen";
        case Bericht.Bescheinigung:
        case Bericht.Zwischenzeugnis:
        case Bericht.Jahreszeugnis: return "rptZeugnis";
        case Bericht.Abiturzeugnis: return "rptAbiturzeugnis";
        case Bericht.Gefaehrdung: return "rptGefaehrdung";
        case Bericht.Abiergebnisse: return "rptAbiergebnisse";
        case Bericht.Notenmitteilung: return "rptNotenmitteilung";
        case Bericht.Einbringung: return "rptEinbringung";

        case Bericht.Klassenliste: return "rptKlassenliste";
        case Bericht.EinserAbi: return "rptEinserAbi";
        default: return "";
      }
    }

    public static SchuelerDruck CreateSchuelerDruck(Schueler s, Bericht b, UnterschriftZeugnis u)
    {
      switch (b)
      {
        case Bericht.Notenbogen: return new NotenbogenDruck(s);
        case Bericht.Bescheinigung:
        case Bericht.Zwischenzeugnis:
        case Bericht.Jahreszeugnis:
        case Bericht.Abiturzeugnis:
        case Bericht.Gefaehrdung: return new ZeugnisDruck(s, b, u);
        case Bericht.Abiergebnisse:
        case Bericht.EinserAbi:
        case Bericht.Notenmitteilung: return new NotenmitteilungDruck(s, b);
        default: return new SchuelerDruck(s, b);
      }
    }
  }

  public class NotenbogenDruck : SchuelerDruck
  {
    public string Anschrift { get; private set; }
    public string Telefon { get; private set; }
    public string GeborenInAm { get; private set; }
    public string KlasseMitZweig { get; private set; }
    public string Laufbahn { get; private set; }

    public NotenbogenDruck(Schueler s) : base(s, Bericht.Notenbogen)
    {
      Anschrift = s.Data.AnschriftStrasse + "\n" + s.Data.AnschriftPLZ + " " + s.Data.AnschriftOrt;
      Telefon = s.Data.AnschriftTelefonnummer;
      GeborenInAm = "geboren am " + s.Data.Geburtsdatum.ToString("dd.MM.yyyy") + " in " + s.Data.Geburtsort;
      KlasseMitZweig = s.KlassenBezeichnung;

      Laufbahn = "Bekenntnis: " + s.Data.Bekenntnis;
      Laufbahn += "<br>Eintritt in Jgst. " + s.Data.EintrittJahrgangsstufe + " am " + s.Data.EintrittAm.ToString("dd.MM.yyyy");
      Laufbahn += " aus " + s.Data.SchulischeVorbildung + " von " + s.EintrittAusSchulname;//.Substring(0,25);
      if (!s.Data.IsProbezeitBisNull()) Laufbahn += "<br>Probezeit bis " + s.Data.ProbezeitBis.ToString("dd.MM.yyyy");
      if (!s.Data.IsAustrittsdatumNull()) Laufbahn += "<br>Austritt am " + s.Data.Austrittsdatum.ToString("dd.MM.yyyy");
      // Wiederholungen
      string tmp = s.getWiederholungen();
      if (tmp != "") Laufbahn += "<br>Wiederholungen: " + tmp;
      if (s.Data.LRSStoerung) Laufbahn += "<br><b>Legasthenie</b>";

      if (jg == 13)
      {
        if (!s.Seminarfachnote.IsThemaKurzNull())
        {
          Bemerkung = "<b>Thema der Seminararbeit:</b><br>";
          Bemerkung += s.Seminarfachnote.ThemaKurz + "<br><br>";
        }
      }

      if (!s.Data.IsDNoteNull() && !s.hatVorkommnis(Vorkommnisart.NichtBestanden))
      {
        Bemerkung += "<b>Durchschnittsnote (" + (jg == 12 ? "Fachhochschulreife" : "fachgebundene Hochschulreife") + "): " + s.Data.DNote + "</b><br>";
        if (!s.Data.IsDNoteAllgNull())
        {
          Bemerkung += "<b>Durchschnittsnote (allgemeine Hochschulreife): " + s.Data.DNoteAllg + "</b><br>";
        }
      }
    }
  }

  public class NotenmitteilungDruck : SchuelerDruck
  {
    public string DNote { get; private set; }
    public string FPAText { get; private set; }

    public NotenmitteilungDruck(Schueler s, Bericht b) : base(s, b)
    {
      if (jg == 11)
      {
        diNoDataSet.FpaRow fpa;
        if (Zugriff.Instance.aktZeitpunkt <= (int)Zeitpunkt.HalbjahrUndProbezeitFOS)
          fpa = s.FPANoten[0];
        else
          fpa = s.FPANoten[1];
        if (!fpa.IsGesamtNull())
        {
          FPAText = "<b>FPA (" + (fpa.Halbjahr + 1) + ". Halbjahr):  " + fpa.Gesamt + " Punkte</b><br>";
          FPAText += "Betrieb " + fpa.Betrieb + ", Anleitung " + fpa.Anleitung;// + "<br>";
          FPAText += ", Vertiefung " + fpa.Vertiefung + (fpa.IsVertiefung1Null() ? "" : " (Teilnoten " + fpa.Vertiefung1 + " " + fpa.Vertiefung2 + ") ");
        }
      }

      // für Abiergebnisse
      if (!s.Data.IsDNoteNull() && !s.hatVorkommnis(Vorkommnisart.NichtBestanden))
      {
        if (Bemerkung != "") Bemerkung += "<br>";
        DNote = (b == Bericht.EinserAbi ? "" : "Durchschnittsnote*: ") + String.Format("{0:0.0}", s.Data.DNote);

        if (!s.Data.IsDNoteAllgNull())
        {
          DNote += " (allg. HSR: " + String.Format("{0:0.0}", s.Data.DNoteAllg) + ")";
        }
      }
    }
  }

  public class ZeugnisDruck : SchuelerDruck
  {
    public string KlasseAR { get; private set; }
    public string GeborenInAm { get; private set; }    
    public string ZeugnisArt { get; private set; }
    public string DatumZeugnis { get; private set; }
    public string Schulleiter { get; private set; }
    public string SchulleiterText { get; private set; }
    public bool ShowKenntnisGenommen { get; private set; } // Minderjährige beim Zwischenzeugnis
    public bool ShowGezSL { get; private set; } // statt eigenhändige Unterschrift erscheint gez. Helga Traut
    public bool ShowFOBOSOHinweis { get; private set; } // Diesem Zeungnis liegt die Schulordnung ...
    public string Siegel { get; private set; } // (Siegel) andrucken
    public bool IstJahreszeugnis { get; private set; }
    public string BestandenText { get; private set; } // nur für Abizeugnis

    public ZeugnisDruck(Schueler s, Bericht b, UnterschriftZeugnis u) : base(s, b)
    {
      if (b == Bericht.Abiturzeugnis) // dort nicht mit Großbuchstaben
      {
        BestandenText = "hat die " + (jg == 12 ? "Fachabiturprüfung" : "Abiturprüfung") + " bestanden. Der Prüfungsausschuss hat ";
        BestandenText += (s.Data.Geschlecht == "M" ? "ihm" : "ihr") + " die";

        if (jg == 12) ZeugnisArt = "Fachhochschulreife";
        else if (s.hatVorkommnis(Vorkommnisart.allgemeineHochschulreife)) ZeugnisArt = "allgemeine Hochschulreife";
        else ZeugnisArt = "fachgebundene Hochschulreife";
        IstJahreszeugnis = SprachniveauDruck.HatFremdsprachen(s); // verbogen für "---" im Fremdpsrachenblock des Abizeugnisses, falls keine Sprachen da sind
      }
      else
      {
        if (b == Bericht.Bescheinigung) ZeugnisArt = "Bescheinigung";
        else if (b == Bericht.Zwischenzeugnis) ZeugnisArt = "Zwischenzeugnis";
        else if (b == Bericht.Jahreszeugnis) ZeugnisArt = "Jahreszeugnis";
        ZeugnisArt = ZeugnisArt.ToUpper();
        IstJahreszeugnis = (b == Bericht.Jahreszeugnis);
      }
      
      GeborenInAm = "geboren am " + s.Data.Geburtsdatum.ToString("dd.MM.yyyy") + " in " + s.Data.Geburtsort;
      KlasseAR = (b == Bericht.Zwischenzeugnis ? "besucht" : "besuchte") + " im " + Schuljahr;
      KlasseAR += " die " + s.getKlasse.JahrgangsstufeZeugnis + " der " + (s.Data.Schulart == "B" ? "Berufsoberschule" : "Fachoberschule");
      if (b == Bericht.Abiturzeugnis)
        KlasseAR += " und unterzog sich als Schüler" + (s.Data.Geschlecht == "M" ? "":"in") + " der Klasse " + s.getKlasse.Bezeichnung + " der Fachabiturprüfung in der Ausbildungsrichtung " + Faecherkanon.GetZweigText(s) + ".";
      else
      {
        if (s.Data.Ausbildungsrichtung != "V") // IV idR. ohne AR
          KlasseAR += ",\nAusbildungsrichtung " + Faecherkanon.GetZweigText(s);
        KlasseAR += " in der Klasse " + s.getKlasse.Bezeichnung;
        if (b == Bericht.Bescheinigung)
          KlasseAR += "\nund ist heute aus der Schule ausgetreten. \n\nIm laufenden Schulhalbjahr erzielte " + s.getErSie() + " bis zum Austritt folgende Leistungen:";
        else
          KlasseAR += ".";
      }
      DatumZeugnis = Zugriff.Instance.getString(GlobaleStrings.SchulOrt) +", den " + Zugriff.Instance.Zeugnisdatum.ToString("dd.MM.yyyy");
      ShowGezSL = (u == UnterschriftZeugnis.gez);
      if (u == UnterschriftZeugnis.Stv)
      {
        Schulleiter = Zugriff.Instance.getString(GlobaleStrings.Stellvertreter);
        SchulleiterText = Zugriff.Instance.getString(GlobaleStrings.StellvertreterText);
      }
      else
      {
        Schulleiter = Zugriff.Instance.getString(GlobaleStrings.Schulleiter);
        SchulleiterText = Zugriff.Instance.getString(GlobaleStrings.SchulleiterText);
      }
   
      // allgemeine Zeugnisbemerkungen (als HTML-Text!)
      if (jg == 11)
        Bemerkung = "Die fachpraktische Ausbildung wurde im Umfang eines halben Schuljahres in außerschulischen Betrieben bzw. schuleigenen Werkstätten abgeleistet.<br><br>Bemerkungen:";
      else if (b!=Bericht.Abiturzeugnis)
        Bemerkung = "Bemerkungen:";

      if (!s.Data.IsZeugnisbemerkungNull())
        Bemerkung += "<br>" + s.Data.Zeugnisbemerkung;
      if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Vorklasse)
        Bemerkung += "<br>Der Unterricht im Fach Religionslehre/Ethik konnte nicht erteilt werden.";
      if (s.IsLegastheniker)
        Bemerkung += "<br>Auf die Bewertung des Rechtschreibens wurde verzichtet. In den Fremdsprachen wurden die mündlichen Leistungen stärker gewichtet.";
      if (s.hatVorkommnis(Vorkommnisart.Sportbefreiung))
        Bemerkung += "<br>" + (s.Data.Geschlecht == "M" ? "Der Schüler" : "Die Schülerin") + " war vom Unterricht im Fach Sport befreit.";
      if (s.hatVorkommnis(Vorkommnisart.MittlereReife))
        Bemerkung += "<br><b>Dieses Zeugnis verleiht den mittleren Schulabschluss gemäß Art. 25 Abs. 1 Satz 2 Nr. 6 BayEUG.</b>";
      if (jg == 11 && b == Bericht.Jahreszeugnis)
        Bemerkung += "<br><b>Die Erlaubnis zum Vorrücken in die Jahrgangsstufe 12 hat " + s.getErSie() + (s.hatVorkommnis(Vorkommnisart.KeineVorrueckungserlaubnis) ? " nicht" : "") + " erhalten.</b>";
      if (jg >= 12 && b == Bericht.Jahreszeugnis)
      {
        if (jg == 12 && s.Data.Schulart == "B")
          Bemerkung += "<br>Die Erlaubnis zum Vorrücken in die Jahrgangsstufe 13 hat " + s.getErSie() + (!s.hatVorkommnis(Vorkommnisart.VorrueckenBOS13moeglich) ? " nicht" : "") + " erhalten.";
        else
          Bemerkung += "<br> " + (s.Data.Geschlecht == "M" ? "Der Schüler" : "Die Schülerin") + " hat sich der Fachabiturprüfung ohne Erfolg unterzogen. " + s.getErSie(true) + " darf die Prüfung gemäß Art. 54 Abs. 5 Satz 1 BayEUG "
              + (s.hatVorkommnis(Vorkommnisart.DarfNichtMehrWiederholen) ? "nicht mehr" : "noch einmal") + " wiederholen.";
      }
      if (b == Bericht.Abiturzeugnis)
        Bemerkung = (Bemerkung == null ? "---" : Bemerkung.Substring(4) + " ---");
      else
        Bemerkung += " ---";
      ShowKenntnisGenommen = s.Alter(Zugriff.Instance.Zeugnisdatum) < 18 && b == Bericht.Zwischenzeugnis;
      if (b == Bericht.Jahreszeugnis)
        Siegel = "(Siegel)";
      else Siegel = "";
    }
  }

  /*
*****************************************************************************************************
**********************************  Unterberichtsdaten **********************************************
   */

  // erzeugt die Druckdaten für die FPA, halbjahresweise
  public class FPADruck
  {
    public string Betrieb { get; private set; }
    public string Anleitung { get; private set; }
    public string Vertiefung12 { get; private set; }
    public string Vertiefung { get; private set; }
    public string Gesamt { get; private set; }
    public string Stelle { get; private set; }

    public FPADruck(diNoDataSet.FpaRow f, string hj)
    {
      Betrieb = f.IsBetriebNull() ? "" : f.Betrieb.ToString();
      Anleitung = f.IsAnleitungNull() ? "" : f.Anleitung.ToString();
      Vertiefung12 = f.IsVertiefung1Null() ? "" : f.Vertiefung1.ToString() + "  ";
      Vertiefung12 += f.IsVertiefung2Null() ? "" : f.Vertiefung2.ToString();
      Vertiefung = f.IsVertiefungNull() ? "" : f.Vertiefung.ToString();
      Gesamt = f.IsGesamtNull() ? "" : f.Gesamt.ToString();
      Stelle = hj + ". Halbjahr: " + (f.IsStelleNull() ? "" : f.Stelle);
      if (!f.IsBemerkungNull()) Stelle += "\n" + f.Bemerkung;
    }
  }

  public class FachSchuelerNotenDruckAlt
  {
    // Arrays können in Bericht leider nicht gedruckt werden, daher einzeln:
    // für SA / sL wird je ein Datensatz erzeugt
    // JF und DGes wird nur bei sL mitgeschickt
    public string fachBez { get; private set; }
    public string Art { get; private set; } // gibt den Text SA oder sL aus
    public string N1 { get; private set; }  // Einzelnoten
    public string D1 { get; private set; }  // Durchschnitt 1. Hj.
    public string DGes1 { get; private set; } // Schnitt Gesamt im 1. Hj.
    public string JF1 { get; private set; }
    public string N2 { get; private set; }
    public string D2 { get; private set; }
    public string DGes2 { get; private set; }
    public string JF2 { get; private set; }
    public string SAP { get; private set; }
    public string MAP { get; private set; }
    public string APG { get; private set; }
    public string GesZ { get; private set; } // SchnittFortgangUndPruefung
    public string Z { get; private set; }
    public string MAP4P { get; private set; } // nötige Punktzahl in einer mündlichen Prüfung um auf 4 im Zeugnis zu kommen
    public string MAP1P { get; private set; }

    public FachSchuelerNotenDruckAlt(FachSchuelerNoten s, bool evalSA, Bericht rpt)
    {
      fachBez = s.getFach.Bezeichnung;
      if (rpt == Bericht.Notenbogen && fachBez.Contains("irtschafts")) // Fachbezeichnung sind zu lang für Notenbogen
      {
        string kuerzel = s.getFach.Kuerzel;
        if (kuerzel == "BwR") fachBez = "Betriebswirt-schaftslehre";
        else if (kuerzel == "VWL") fachBez = "Volkswirt-schaftslehre";
        else if (kuerzel == "WIn") fachBez = "Wirtschafts-informatik";
      }

      Art = ""; N1 = ""; D1 = ""; N2 = ""; D2 = "";
      var d1 = s.getSchnitt(Halbjahr.Erstes);
      var d2 = s.getSchnitt(Halbjahr.Zweites);

      if (rpt != Bericht.Abiergebnisse)
      {
        if (evalSA)
        {
          Art = "SA\n";
          N1 = s.SA(Halbjahr.Erstes) + "\n";
          N2 = s.SA(Halbjahr.Zweites) + "\n";
          D1 = String.Format("{0:f2}", d1.SchnittSchulaufgaben) + "\n";
          D2 = String.Format("{0:f2}", d2.SchnittSchulaufgaben) + "\n";
        }
        Art += "sL";
        N1 += s.sL(Halbjahr.Erstes);
        N2 += s.sL(Halbjahr.Zweites);
        D1 += String.Format("{0:f2}", d1.SchnittMuendlich);
        D2 += String.Format("{0:f2}", d2.SchnittMuendlich);
        DGes1 = String.Format("{0:f2}", d1.JahresfortgangMitKomma);
        JF1 = d1.JahresfortgangGanzzahlig.ToString();
        JF2 = d2.JahresfortgangGanzzahlig.ToString();
      }
      DGes2 = String.Format("{0:f2}", d2.JahresfortgangMitKomma);

      SAP = put(s.getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich), 0);
      MAP = put(s.getNoten(Halbjahr.Zweites, Notentyp.APMuendlich), 0);
      APG = String.Format("{0:f2}", d2.PruefungGesamt);
      GesZ = String.Format("{0:f2}", d2.SchnittFortgangUndPruefung);
      Z = d2.Abschlusszeugnis.ToString();

      if (rpt == Bericht.Abiergebnisse && MAP == "")
      {
        MAP4P = s.NotwendigeNoteInMAP(4);
        MAP1P = s.NotwendigeNoteInMAP(1);
      }
    }

    public FachSchuelerNotenDruckAlt(string fach, int note)
    {
      fachBez = fach;
      Z = note.ToString();
      JF2 = Z;
    }

    public FachSchuelerNotenDruckAlt(diNoDataSet.SeminarfachnoteRow s)
    {
      fachBez = "Seminararbeit";
      if (!s.IsGesamtnoteNull())
      {
        Z = s.Gesamtnote.ToString();
        JF2 = Z;
      }
    }

    private string put(IList<int> n, int index)
    {
      if (index < n.Count)
        return n[index].ToString();
      else
        return "";
    }

    // ausnahmsweise kopiert, weil es nächstes Jahr eh rausfällt:
    private string ErfolgText(int note)
    {
      switch (note)
      {
        case 1: return "mit sehr gutem Erfolg";
        case 2: return "mit gutem Erfolg";
        case 3: return "mit Erfolg";
        default: return "ohne Erfolg";
      }
    }
  }

  // Basisklasse für die Noten eines Faches
  public abstract class NotenDruck
  {
    public string fachBez { get; protected set; }
    public string Hj1 { get; protected set; }  // Halbjahrespunktzahl 1.Hj
    public string Hj2 { get; protected set; }
    public string GE { get; private set; } // Gesamtergebnis
    public string SGE { get; private set; } // Schnitt-Gesamtergebnis (2 Dez)
    protected HjLeistung hj1, hj2;

    public string VorHj1 { get; protected set; }  // 11/1 und 11/2
    public string VorHj2 { get; protected set; }

    public NotenDruck()
    {
    }

    public NotenDruck(FachSchuelerNoten s, Bericht b)
    {
      fachBez = s.getFach.Bezeichnung;
      if ((b == Bericht.Einbringung || b == Bericht.Abiergebnisse) && s.getFach.NichtNC) fachBez += "*";
      if (b == Bericht.Notenbogen && fachBez.Contains("irtschafts")) // Fachbezeichnung sind zu lang für Notenbogen
      {
        string kuerzel = s.getFach.Kuerzel;
        if (kuerzel == "BwR") fachBez = "Betriebswirt-schaftslehre";
        else if (kuerzel == "VWL") fachBez = "Volkswirt-schaftslehre";
        else if (kuerzel == "WIn") fachBez = "Wirtschafts-informatik";
      }
      hj1 = s.getHjLeistung(HjArt.Hj1);
      hj2 = s.getHjLeistung(HjArt.Hj2);
      Hj1 = putHj(hj1);
      Hj2 = putHj(hj2);
      if (s.schueler.hatVorHj)
      {
        VorHj1 = putHj(s.getVorHjLeistung(HjArt.Hj1));
        VorHj2 = putHj(s.getVorHjLeistung(HjArt.Hj2));
      }
      var ge = s.getHjLeistung(HjArt.GesErg);
      GE = putHj(ge);
      SGE = ge == null ? "" : String.Format("{0:f2}", ge.Punkte2Dez);
    }

    // aktuell nur für Fachreferat
    public NotenDruck(HjLeistung h)
    {
      if (h.Art == HjArt.FR)
        fachBez = "Fachreferat in " + h.getFach.Bezeichnung;
      else
        fachBez = h.getFach.Bezeichnung;

      Hj2 = h.Punkte.ToString();
      GE = h.Punkte.ToString();
    }
    

    public static NotenDruck CreateNotenDruck(FachSchuelerNoten s, Bericht b)
    {
      if (b == Bericht.Abiergebnisse) return new NotenAbiDruck(s);
      if (b == Bericht.Notenbogen && (byte)s.schueler.getKlasse.Jahrgangsstufe < 12) return new NotenSjDruck(s);
      return new NotenHjDruck(s,b);
    }

    protected string putHj(FachSchuelerNoten s, HjArt a)
    {
      var hjl = s.getHjLeistung(a);
      return putHj(hjl);
    }

    protected string putHj(HjLeistung hjl)
    {
      if (hjl == null || hjl.Status == HjStatus.Ungueltig) return "";
      if (hjl.Status == HjStatus.NichtEinbringen) return "(" + hjl.Punkte.ToString() + ")";
      return hjl.Punkte.ToString();      
    }

    // Liefert die erste Note einer Notenliste (v.a. bei einelementigen Noten, wie SAP,...)
    protected string getFirst(IList<int> n)
    {
      if (n!=null && n.Count > 0)
        return n[0].ToString();
      else
        return "";
    }

    protected string putFpa(diNoDataSet.FpaRow t, out string sL)
    {
      sL = "";
      if (t == null) return "";
      if (!t.IsBetriebNull()) sL = t.Betrieb.ToString() + "B ";
      if (!t.IsVertiefungNull()) sL += t.Vertiefung.ToString() + "V ";
      if (!t.IsVertiefung1Null() && !t.IsVertiefung2Null())
        sL += "(" + t.Vertiefung1.ToString() + " " + t.Vertiefung2.ToString() + ") ";
      if (!t.IsAnleitungNull()) sL += t.Anleitung.ToString() + "A";

      if (!t.IsGesamtNull()) return t.Gesamt.ToString(); else return "";
    }
  }

  // Alle Noten eines Halbjahres (z.B. für Notenbogen)
  public class NotenHjDruck : NotenDruck
  {
    public string SA { get; private set; }  // SA-Noten
    public string sL { get; private set; }  // mdl. 1. Hj.
    public string SsL { get; private set; }  // Schnitt mdl. 1. Hj.
    public string S { get; private set; }   // Schnitt 1. Hj.
    public string JN { get; private set; } // Jahresnote    
    public string SAP { get; private set; }
    public string MAP { get; private set; }
    public string APG { get; private set; }

    public NotenHjDruck(FachSchuelerNoten s, Bericht b) : base(s, b)
    {
      Halbjahr h = Zugriff.Instance.aktHalbjahr;
      SA = s.SA(h);
      sL = s.sL(h);
      HjLeistung hj = (h == Halbjahr.Erstes ? hj1 : hj2);

      if (hj != null)
      {
        SsL = hj.SchnittMdl == null ? "" : String.Format("{0:f2}", hj.SchnittMdl);
        S = hj.Punkte2Dez == null ? "" : String.Format("{0:f2}", hj.Punkte2Dez);
      }
      JN = putHj(s, HjArt.JN);
      SAP = getFirst(s.getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich));
      MAP = getFirst(s.getNoten(Halbjahr.Zweites, Notentyp.APMuendlich));
      APG = putHj(s, HjArt.AP);
    }

    public NotenHjDruck(HjLeistung h) : base(h)
    {
    }

    public NotenHjDruck(diNoDataSet.FpaDataTable f)
    {
      fachBez = "Fachpraktische Ausbildung";      
      Hj1 = putFpa(f[0], out string s1);      
      Hj2 = putFpa(f[1], out string s2);      
      if (Zugriff.Instance.aktHalbjahr == Halbjahr.Erstes) sL = s1; else sL = s2;
      if (f[1] != null && !f[1].IsJahrespunkteNull()) JN = f[1].Jahrespunkte.ToString();
    }

  }

  // Alle Noten des Schuljahres (z.B. für Notenmitteilung)
  public class NotenSjDruck : NotenDruck
  {
    public string SA1 { get; private set; }  // SA-Noten 1. Hj.
    public string sL1 { get; private set; }  // mdl. 1. Hj.
    public string SsL1 { get; private set; }  // Schnitt mdl. 1. Hj.
    public string S1 { get; private set; }   // Schnitt 1. Hj.
    public string SA2 { get; private set; }
    public string sL2 { get; private set; }
    public string SsL2 { get; private set; }
    public string S2 { get; private set; }
    public string FR { get; private set; }
    public string JN { get; private set; } // Jahresnote    

    public NotenSjDruck(FachSchuelerNoten s) : base(s, Bericht.Notenmitteilung)
    {

      SA1 = s.SA(Halbjahr.Erstes);
      SA2 = s.SA(Halbjahr.Zweites);
      sL1 = s.sL(Halbjahr.Erstes);
      sL2 = s.sL(Halbjahr.Zweites);
      if (hj1 != null)
      {
        SsL1 = hj1.SchnittMdl == null ? "" : String.Format("{0:f2}", hj1.SchnittMdl);
        S1 = hj1.Punkte2Dez == null ? "" : String.Format("{0:f2}", hj1.Punkte2Dez);
      }
      if (hj2 != null)
      {
        SsL2 = hj2.SchnittMdl == null ? "" : String.Format("{0:f2}", hj2.SchnittMdl);
        S2 = hj2.Punkte2Dez == null ? "" : String.Format("{0:f2}", hj2.Punkte2Dez);
      }
      JN = putHj(s, HjArt.JN);      
    }

    public NotenSjDruck(diNoDataSet.FpaDataTable f)
    {      
      fachBez = "Fachpraktische Ausbildung";      
      Hj1 = putFpa(f[0],out string s1);
      sL1 = s1;
      Hj2 = putFpa(f[1], out string s2);
      sL2 = s2;
      if (f[1]!= null && !f[1].IsJahrespunkteNull()) JN = f[1].Jahrespunkte.ToString();      
    }    
  }

  // Abiergebnisse
  public class NotenAbiDruck : NotenHjDruck
  {
    public string MAP1P { get; private set; }
    public string MAP4P { get; private set; }

    public NotenAbiDruck(FachSchuelerNoten s) : base(s, Bericht.Abiergebnisse)
    {      
      //MAP4P = NotwendigeNoteInMAP(s,4);
      //MAP1P = NotwendigeNoteInMAP(s,1);
    }

    // TODO:
    private string NotwendigeNoteInMAP(FachSchuelerNoten s, int Zielpunkte)
    {
      return "";
    }
  }

  public class NotenZeugnisDruck : NotenDruck
  {
    public string fachGruppe { get; private set; }
    public string JN { get; set; } // Jahresnote oder Gesamtergebnis
    public string JNText { get; set; }
    public string APG { get; private set; }
    private Bericht rpt;

    public NotenZeugnisDruck(FachSchuelerNoten s, Bericht arpt)
    {
      rpt = arpt;
      switch (s.getFach.Typ)
      {
        case FachTyp.Allgemein: fachGruppe = "Allgemeinbildende Fächer"; break;
        case FachTyp.Profilfach: fachGruppe = "Profilfächer"; break;
        default: fachGruppe = "Wahlpflichtfächer"; break;
      }
      fachBez = s.getFach.BezZeugnis;
      if (s.getFach.NichtNC && rpt == Bericht.Abiturzeugnis) fachBez += "*";

      if (s.schueler.AlteFOBOSO()) // nur zum Test
      {
        Hj1 = relNoteToZeugnis(s.getRelevanteNote(Zeitpunkt.HalbjahrUndProbezeitFOS));
        Hj2 = relNoteToZeugnis(s.getRelevanteNote(Zeitpunkt.Jahresende));
        return;
      }

      if (s.schueler.hatVorHj)
      {
        VorHj1 = HjToZeugnis(s.getVorHjLeistung(HjArt.Hj1));
        VorHj2 = HjToZeugnis(s.getVorHjLeistung(HjArt.Hj2));
      }
      Hj1 = HjToZeugnis(s.getHjLeistung(HjArt.Hj1));
      Hj2 = HjToZeugnis(s.getHjLeistung(HjArt.Hj2));
      if (rpt == Bericht.Abiturzeugnis)
      {
        JNToZeugnis(s.getHjLeistung(HjArt.GesErg));
        APG = HjToZeugnis(s.getHjLeistung(HjArt.AP));
      }
      else
        JNToZeugnis(s.getHjLeistung(HjArt.JN));
    }

    public NotenZeugnisDruck(diNoDataSet.FpaDataTable f)
    {
      fachGruppe = "Profilfächer"; // Workaround: läuft unter dieser Gruppe, weil Gruppe gleich das Fach ist.
      fachBez = "<b>Fachpraktische Ausbildung</b>";
      Hj1 = FpaToZeugnis(f[0]);
      Hj2 = FpaToZeugnis(f[1]);
      byte? p;
      if (f[1].IsJahrespunkteNull()) p = null;
      else p = f[1].Jahrespunkte;
      JNToZeugnis(p);
    }

    // für FR und Seminar, wo nur die Gesamtnote angezeigt wird
    public NotenZeugnisDruck(HjLeistung f, string FachBezeichnung)
    {
      fachGruppe = "Wahlpflichtfächer"; // Workaround: läuft unter dieser Gruppe, weil Gruppe gleich das Fach ist.
      fachBez = "<b>" + FachBezeichnung + "</b>";
      Hj2 = HjToZeugnis(f);
      JNToZeugnis(f);
    }

    private string relNoteToZeugnis(byte? t) // für AlteFOBOSO
    {
      if (t == null) return "--";
      else return t.GetValueOrDefault().ToString("D2");
    }

    private string HjToZeugnis(HjLeistung t) // für NeueFOBOSO
    {
      if (t == null || t.Status == HjStatus.Ungueltig) return rpt == Bericht.Abiturzeugnis ? "" : "--";
      else if (rpt == Bericht.Abiturzeugnis && t.Status == HjStatus.NichtEinbringen) return "(" + t.Punkte.ToString("D2") + ")";
      else return t.Punkte.ToString("D2");
    }

    private string FpaToZeugnis(diNoDataSet.FpaRow t)
    {
      if (t == null || t.IsGesamtNull()) return "--";
      else return t.Gesamt.ToString("D2");
    }

    private void JNToZeugnis(HjLeistung t)
    {
      if (t == null || t.Status == HjStatus.Ungueltig) JNToZeugnis((byte?)null);
      else JNToZeugnis(t.Punkte);
    }

    private void JNToZeugnis(byte? p)
    {
      if (p == null)
      {
        JNText = "-----";
        JN = "--";
        return;
      }
      JN = p.GetValueOrDefault().ToString("D2");
      if (p == 0) JNText = "ungenügend";
      else if (p <= 3) JNText = "mangelhaft";
      else if (p <= 6) JNText = "ausreichend";
      else if (p <= 9) JNText = "befriedigend";
      else if (p <= 12) JNText = "gut";
      else JNText = "sehr gut";
    }
  }

  public class PunkteSummeDruck
  {
    public string Text { get; set; }
    public string Punkte { get; set; }
    public string InWorten { get; set; }

    public PunkteSummeDruck(string text, string punkte, string inWorten)
    {
      Text = text;
      Punkte = punkte;
      InWorten = inWorten;
    }

    public static List<PunkteSummeDruck> Create(Schueler s)
    {
      List<PunkteSummeDruck> list = new List<PunkteSummeDruck>();
      Punktesumme p = s.punktesumme;
      foreach (PunktesummeArt a in Enum.GetValues(typeof(PunktesummeArt)))
      {
        if (p.Anzahl(a) > 0)
        {
          list.Add(new PunkteSummeDruck(ArtToText(a, p, s), p.Summe(a).ToString(), ""));
        }
      }
      if (!s.Data.IsDNoteNull())
      {
        list.Add(new PunkteSummeDruck("", "", "")); // Leerzeile
        list.Add(new PunkteSummeDruck("Durchschnittsnote", string.Format("{0:F1}", s.Data.DNote), ZahlToText(s.Data.DNote)));
      }      
      return list;
    }

    public static string ArtToText(PunktesummeArt art, Punktesumme p, Schueler s)
    {
      switch (art)
      {
        case PunktesummeArt.AP: return "- Punktesumme der vier Prüfungsergebnisse " + (s.hatVorHj ? "(dreifach)" : "(zweifach)");
        case PunktesummeArt.FPA: return "- Punktesumme der fachpraktischen Ausbildung aus 11/1 und 11/2";
        case PunktesummeArt.FR: return "- Ergebnis des Fachreferats";
        case PunktesummeArt.HjLeistungen: return "- Punktesumme aus " + p.Anzahl(art) + " einzubringenden Halbjahresergebnissen";
        case PunktesummeArt.Gesamt: return "Summe";
        default: return "";
      }
    }

    public static string ZahlToText(decimal zahl)
    {
      int vk = (int)Math.Floor(zahl);
      int nk = (int)Math.Floor((zahl - vk) * 10 + (decimal)0.01);
      return ZifferToText(vk) + "," + ZifferToText(nk);
    }

    public static string ZifferToText(int ziffer)
    {
      switch (ziffer)
      {
        case 0: return "null";
        case 1: return "eins";
        case 2: return "zwei";
        case 3: return "drei";
        case 4: return "vier";
        case 5: return "fünf";
        case 6: return "sechs";
        case 7: return "sieben";
        case 8: return "acht";
        case 9: return "neun";
        default: return "";
      }
    }
  }
  
  public class SprachniveauDruck
  {
    public string fachBez { get; private set; }
    public string Stufe { get; private set; }

    public SprachniveauDruck(FachSchuelerNoten f)
    {
      int i;
      fachBez = f.getFach.BezZeugnis;
      i = fachBez.IndexOf(" ");
      if (i > 0) fachBez = fachBez.Substring(0,i); 
      Stufe = Fremdsprachen.NiveauText(Fremdsprachen.HjToSprachniveau(f));
    }

    public static bool HatFremdsprachen(Schueler s)
    {
      foreach (var f in s.getNoten.alleSprachen)
      {
        if (f.getHjLeistung(HjArt.Sprachenniveau) != null)
        {
          return true;
        }
      }
      return false;
    }

    public static List<SprachniveauDruck> Create(Schueler s)
    {
      List<SprachniveauDruck> list = new List<SprachniveauDruck>();
      foreach (var f in s.getNoten.alleSprachen)
      {
        if (f.getHjLeistung(HjArt.Sprachenniveau)!=null)
        {
          list.Add(new SprachniveauDruck(f));
        }
      }
      return list;
    }
  }
  
  public class LehrerRolleDruck
  {
    public string RechteBezeichnung { get; private set; }
    public string LehrerName { get; set; }

    public LehrerRolleDruck(string bez, string vorname, string nachname)
    {
      RechteBezeichnung = bez;
      LehrerName = nachname + ", " + vorname;
    }

    public static List<LehrerRolleDruck> CreateLehrerRolleDruck()
    {
      List<LehrerRolleDruck> liste = new List<LehrerRolleDruck>();
      var ta = new vwLehrerRolleTableAdapter();
      var dt = ta.GetData();
      foreach (var r in dt)
        liste.Add(new LehrerRolleDruck(r.Bezeichnung, r.Vorname, r.Nachname));
      return liste;
    }
  }
}
  




