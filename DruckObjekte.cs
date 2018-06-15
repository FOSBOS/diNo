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
    Abiergebnisse,
    Bescheinigung,
    Zwischenzeugnis,
    Jahreszeugnis,
    Abizeugnis,
    Notenbogen,
    Klassenliste
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
    protected Jahrgangsstufe jg;

    public SchuelerDruck(Schueler s, Bericht Berichtsname)
    {
      jg = s.getKlasse.Jahrgangsstufe;

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
      
      if (jg == Jahrgangsstufe.Dreizehn)
      {
        if (!s.Seminarfachnote.IsThemaKurzNull())
        {
          Bemerkung = "<b>Thema der Seminararbeit:</b><br>";
          Bemerkung += s.Seminarfachnote.ThemaKurz;
        }
      }
    }
    
    public static string GetBerichtsname(Bericht b)
    {
      switch (b)
      {
        case Bericht.Notenbogen: return "rptNotenbogen";
        case Bericht.Bescheinigung: 
        case Bericht.Zwischenzeugnis: 
        case Bericht.Jahreszeugnis: return "rptZeugnis";
        case Bericht.Gefaehrdung: return "rptGefaehrdung";
        case Bericht.Abiergebnisse: return "rptAbiergebnisse";
        case Bericht.Notenmitteilung: return "rptNotenmitteilung";
        
        case Bericht.Klassenliste: return "rptKlassenliste";
        default: return "";
      }
    }

    public static SchuelerDruck CreateSchuelerDruck(Schueler s, Bericht b)
    {
      switch (b)
      {
        case Bericht.Notenbogen: return new NotenbogenDruck(s);
        case Bericht.Bescheinigung:
        case Bericht.Zwischenzeugnis: 
        case Bericht.Jahreszeugnis: 
        case Bericht.Gefaehrdung: return new ZeugnisDruck(s, b);
        case Bericht.Abiergebnisse: 
        case Bericht.Notenmitteilung: return new NotenmitteilungDruck(s, b);        
        default: return new SchuelerDruck(s,b);
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
    }
  }

  public class NotenmitteilungDruck : SchuelerDruck
  {
    public string DNote { get; private set; }
    public string FPAText { get; private set; }

    public NotenmitteilungDruck(Schueler s, Bericht b) : base(s, b)
    {
      if (jg == Jahrgangsstufe.Elf)
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
      if (!s.hatVorkommnis(Vorkommnisart.NichtBestanden))
      {
        if (!s.Data.IsDNoteNull())
        {
          if (Bemerkung != "") Bemerkung += "<br>";
          Bemerkung += "<b>Durchschnittsnote (" + (jg == Jahrgangsstufe.Zwoelf ? "Fachhochschulreife" : "fachgebundene Hochschulreife") + "): " + s.Data.DNote + "</b><br>";
          DNote = "Durchschnittsnote*: " + s.Data.DNote;
        }
        if (!s.Data.IsDNoteAllgNull())
        {
          Bemerkung += "<b>Durchschnittsnote (allgemeine Hochschulreife): " + s.Data.DNoteAllg + "</b><br>";
          DNote += " (allg. HSR: " + s.Data.DNoteAllg + ")";
        }
      }
    }
  }

  public class ZeugnisDruck : SchuelerDruck
  {
    public string KlasseAR { get; private set; }
    public string JgKurz { get; private set; }
    public string ZeugnisArt { get; private set; }
    public string DatumZeugnis { get; private set; }
    public bool ShowKenntnisGenommen { get; private set; } // Minderjährige beim Zwischenzeugnis
    public bool ShowGezSL { get; private set; } // statt eigenhändige Unterschrift erscheint gez. Helga Traut
    public bool ShowFOBOSOHinweis { get; private set; } // Diesem Zeungnis liegt die Schulordnung ...
    public string Siegel { get; private set; } // (Siegel) andrucken
    
    public ZeugnisDruck(Schueler s, Bericht b) : base(s, b)
    {
      if (s.getKlasse.Bezeichnung == "IV") JgKurz = "IV";
      else if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Vorklasse) JgKurz = "VKL";
      else JgKurz = ((int)s.getKlasse.Jahrgangsstufe).ToString();

      if (b == Bericht.Bescheinigung) ZeugnisArt = "Bescheinigung";
      else if (b == Bericht.Zwischenzeugnis) ZeugnisArt = "Zwischenzeugnis";
      else /*if (b == Bericht.Jahreszeugnis)*/ ZeugnisArt = "Jahreszeugnis";
      ZeugnisArt = ZeugnisArt.ToUpper();

      KlasseAR = (b == Bericht.Zwischenzeugnis ? "besucht" : "besuchte") + " im " + Schuljahr;
      KlasseAR += " die " + s.getKlasse.JahrgangsstufeZeugnis + " der " + (s.Data.Schulart == "B" ? "Berufsoberschule" : "Fachoberschule");
      if (s.Data.Ausbildungsrichtung != "V") // IV idR. ohne AR
        KlasseAR += ",\nAusbildungsrichtung " + Faecherkanon.GetZweigText(s);
      KlasseAR += " in der Klasse " + s.getKlasse.Bezeichnung;
      if (b == Bericht.Bescheinigung)
        KlasseAR += "\nund ist heute aus der Schule ausgetreten. \n\nIm laufenden Schulhalbjahr erzielte " + s.getErSie() + " bis zum Austritt folgende Leistungen:";
      else
        KlasseAR += ".";

      DatumZeugnis = Zugriff.Instance.Zeugnisdatum.ToString("dd.MM.yyyy");

      // allgemeine Zeugnisbemerkungen (als HTML-Text!)
      Bemerkung = "Bemerkungen:";
      if (!s.Data.IsZeugnisbemerkungNull())
        Bemerkung += "<br>" + s.Data.Zeugnisbemerkung;
      if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Vorklasse && s.getKlasse.Bezeichnung != "IV")
        Bemerkung += "<br>Der Unterricht im Fach Religionslehre/Ethik konnte nicht erteilt werden.";
      if (s.IsLegastheniker)
        Bemerkung += "<br>Auf die Bewertung des Rechtschreibens wurde verzichtet. In den Fremdsprachen wurden die mündlichen Leistungen stärker gewichtet.";
      if (s.hatVorkommnis(Vorkommnisart.Sportbefreiung))
        Bemerkung += "<br>" + (s.Data.Geschlecht == "M" ? "Der Schüler" : "Die Schülerin") + " war vom Unterricht im Fach Sport befreit.";
      if (s.hatVorkommnis(Vorkommnisart.MittlereReife))
        Bemerkung += "<br><b>Dieses Zeugnis verleiht den mittleren Schulabschluss gemäß Art. 25 Abs. 1 Satz 2 Nr. 6 BayEUG.</b>";
      if (jg==Jahrgangsstufe.Elf)        
        Bemerkung += "<br><b>Die Erlaubnis zum Vorrücken in die Jahrgangsstufe 12 hat " + s.getErSie() + (s.hatVorkommnis(Vorkommnisart.KeineVorrueckungserlaubnis) ? " nicht": "") + " erhalten.</b>";
      if (jg >= Jahrgangsstufe.Zwoelf && b == Bericht.Jahreszeugnis)
      {
        if (jg == Jahrgangsstufe.Zwoelf && s.Data.Schulart == "B")
          Bemerkung += "<br>Die Erlaubnis zum Vorrücken in die Jahrgangsstufe 13 hat " + s.getErSie() + (!s.hatVorkommnis(Vorkommnisart.VorrueckenBOS13moeglich) ? " nicht" : "") + " erhalten.";
        else
          Bemerkung += "<br> " + (s.Data.Geschlecht == "M" ? "Der Schüler" : "Die Schülerin") + " hat sich der Fachabiturprüfung ohne Erfolg unterzogen. " + s.getErSie(true) + " darf die Prüfung gemäß Art. 54 Abs. 5 Satz 1 BayEUG " 
              + (s.hatVorkommnis(Vorkommnisart.DarfNichtMehrWiederholen) ? "nicht mehr": "noch einmal") + " wiederholen.";
      }
      Bemerkung += " ---";
      ShowKenntnisGenommen = s.Alter(Zugriff.Instance.Zeugnisdatum) < 18 && b == Bericht.Zwischenzeugnis;
      ShowGezSL = (b == Bericht.Zwischenzeugnis);
      if (b == Bericht.Jahreszeugnis || b == Bericht.Abizeugnis)
        Siegel = "(Siegel)";
      else Siegel = "";      
    }
  }


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

  public class FachSchuelerNotenDruckKurz
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

    public FachSchuelerNotenDruckKurz(FachSchuelerNoten s, bool evalSA, string rptName)
    {
      fachBez = s.getFach.Bezeichnung;
      if (rptName == "diNo.rptNotenbogen.rdlc" && fachBez.Contains("irtschafts")) // Fachbezeichnung sind zu lang für Notenbogen
      {
        string kuerzel = s.getFach.Kuerzel;
        if (kuerzel == "BwR") fachBez = "Betriebswirt-schaftslehre";
        else if (kuerzel == "VWL") fachBez = "Volkswirt-schaftslehre";
        else if (kuerzel == "WIn") fachBez = "Wirtschafts-informatik";
      }

      Art = ""; N1 = ""; D1 = ""; N2 = ""; D2 = "";
      var d1 = s.getSchnitt(Halbjahr.Erstes);
      var d2 = s.getSchnitt(Halbjahr.Zweites);

      if (rptName != "diNo.rptAbiergebnisse.rdlc")
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

      if (rptName == "diNo.rptAbiergebnisse.rdlc" && MAP == "")
      {
        MAP4P = s.NotwendigeNoteInMAP(4);
        MAP1P = s.NotwendigeNoteInMAP(1);
      }
    }

    public FachSchuelerNotenDruckKurz(diNoDataSet.SeminarfachnoteRow s)
    {
      fachBez = "Seminararbeit";
      if (!s.IsGesamtnoteNull())
      {
        Z = s.Gesamtnote.ToString();
        JF2 = Z;
      }
    }

    public FachSchuelerNotenDruckKurz(diNoDataSet.FpaDataTable f)
    {
      fachBez = "Fachpraktische Ausbildung";
      /*
              if (!f.IsPunkte1HjNull())
              {
                N1 = f.Punkte1Hj.ToString() + " (" + ErfolgText(f.Erfolg1Hj) + ")";          
              }
              if (!f.IsPunkteNull())
              {
                N2 = f.Punkte2Hj.ToString() + "\nSchnitt: " + f.Punkte.ToString() + " ("+ ErfolgText(f.Erfolg) + ")";
              }
      */
    }

    public FachSchuelerNotenDruckKurz(string fach, int note)
    {
      fachBez = fach;
      Z = note.ToString();
      JF2 = Z;
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

  public class FachSchuelerNotenDruck11
  {
    public string fachBez { get; private set; }
    public string SA1 { get; private set; }  // SA-Noten 1. Hj.
    public string sL1 { get; private set; }  // mdl. 1. Hj.
    public string SsL1 { get; private set; }  // Schnitt mdl. 1. Hj.
    public string S1 { get; private set; }   // Schnitt 1. Hj.
    public string Hj1 { get; private set; }  // Halbjahrespunktzahl 1.Hj
    public string SA2 { get; private set; }
    public string sL2 { get; private set; }
    public string SsL2 { get; private set; }
    public string S2 { get; private set; }
    public string Hj2 { get; private set; }
    public string GE { get; private set; } // Gesamtergebnis

    public FachSchuelerNotenDruck11(FachSchuelerNoten s, string rptName)
    {
      fachBez = s.getFach.Bezeichnung;
      if (rptName == "diNo.rptNotenbogen.rdlc" && fachBez.Contains("irtschafts")) // Fachbezeichnung sind zu lang für Notenbogen
      {
        string kuerzel = s.getFach.Kuerzel;
        if (kuerzel == "BwR") fachBez = "Betriebswirt-schaftslehre";
        else if (kuerzel == "VWL") fachBez = "Volkswirt-schaftslehre";
        else if (kuerzel == "WIn") fachBez = "Wirtschafts-informatik";
      }

      var hj1 = s.getHjLeistung(HjArt.Hj1);
      var hj2 = s.getHjLeistung(HjArt.Hj2);

      SA1 = s.SA(Halbjahr.Erstes);
      SA2 = s.SA(Halbjahr.Zweites);
      sL1 = s.sL(Halbjahr.Erstes);
      sL2 = s.sL(Halbjahr.Zweites);
      if (hj1 != null)
      {
        SsL1 = hj1.SchnittMdl == null ? "" : String.Format("{0:f2}", hj1.SchnittMdl);
        S1 = hj1.Punkte2Dez == null ? "" : String.Format("{0:f2}", hj1.Punkte2Dez);
        Hj1 = hj1.Punkte.ToString();
      }
      if (hj2 != null)
      {
        SsL2 = hj2.SchnittMdl == null ? "" : String.Format("{0:f2}", hj2.SchnittMdl);
        S2 = hj2.Punkte2Dez == null ? "" : String.Format("{0:f2}", hj2.Punkte2Dez);
        Hj2 = hj2.Punkte.ToString();
      }
      hj2 = s.getHjLeistung(HjArt.GesErg);
      if (hj2 != null) GE = hj2.Punkte.ToString();
    }
  }

  /* ------------ Objekte für Unterberichte ---------------- */

  public class FachSchuelerNotenZeugnisDruck
  {
    public string fachGruppe { get; private set; }
    public string fachBez { get; private set; }
    public string Hj1 { get; set; }  // Halbjahrespunktzahl 1.Hj
    public string Hj2 { get; set; }
    public string GE { get; set; } // Gesamtergebnis

    public FachSchuelerNotenZeugnisDruck(diNoDataSet.FpaDataTable f)
    {
      fachGruppe = "Profilfächer"; // Workaround: läuft unter dieser Gruppe, weil Gruppe gleich das Fach ist.
      fachBez = "<b>Fachpraktische Ausbildung</b>";
      Hj1 = FpaToZeugnis(f[0]);
      Hj2 = FpaToZeugnis(f[1]);
    }

    public FachSchuelerNotenZeugnisDruck(FachSchuelerNoten s, string rptName)
    {
      switch (s.getFach.Typ)
      {
        case FachTyp.Allgemein: fachGruppe = "Allgemeinbildende Fächer"; break;
        case FachTyp.Profilfach: fachGruppe = "Profilfächer"; break;
        default: fachGruppe = "Wahlpflichtfächer"; break;
      }
      fachBez = s.getFach.BezZeugnis;
      if (s.schueler.AlteFOBOSO())
      {
        Hj1 = relNoteToZeugnis(s.getRelevanteNote(Zeitpunkt.HalbjahrUndProbezeitFOS));
        Hj2 = relNoteToZeugnis(s.getRelevanteNote(Zeitpunkt.Jahresende));
        return;
      }

      Hj1 = HjToZeugnis(s.getHjLeistung(HjArt.Hj1));
      Hj2 = HjToZeugnis(s.getHjLeistung(HjArt.Hj2));
      GE = HjToZeugnis(s.getHjLeistung(HjArt.GesErg));
    }

    private string relNoteToZeugnis(byte? t) // für AlteFOBOSO
    {
      if (t == null) return "--";
      else return t.GetValueOrDefault().ToString("D2");
    }

    private string HjToZeugnis(HjLeistung t) // für NeueFOBOSO
    {
      if (t == null) return "--";
      else return t.Punkte.ToString("D2");
    }

    private string FpaToZeugnis(diNoDataSet.FpaRow t)
    {
      if (t == null || t.IsGesamtNull()) return "--";
      else return t.Gesamt.ToString("D2");
    }
  }

}
