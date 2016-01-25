using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo.OmnisDB
{
  public static class Konstanten
  {
    public static int zeugnisIdCol = 0;
    public static int schuelerIdCol = 1;
    public static int zeugnisartCol = 2;
    public static int faecherspiegelCol = 3;
    public static int fpaCol = 7;
    public static int klassenzielOderGefaehrdungCol = 9;
    public static int abweisungCol = 11;
    public static int fachDerErgaenzungspruefungCol = 28;
    public static int faecherspiegelAPCol = 66;
    public static int viertesPruefungsfachCol = 68;
    public static int APBestandenCol = 75;
    public static int weiteresFach1BezeichnungCol = 79; //Für Wahlpflichtfächer, z. B. F, F3
    public static int weiteresFach2BezeichnungCol = 80;
    public static int weiteresFach3BezeichnungCol = 81;
    public static int weiteresFach1NoteCol = 82;
    public static int weiteresFach2NoteCol = 83;
    public static int weiteresFach3NoteCol = 84;
    public static int anzahlVersaeumnisseCol = 88; // z. B. 12,5
    public static int anzahlUnentschuldigterVersaeumnisseCol = 89;
    public static int wahlfach1NoteCol = 99;
    public static int wahlfach2NoteCol = 100;
    public static int wahlfach3NoteCol = 101;
    public static int wahlfach4NoteCol = 102;
    public static int notePflichtfach1Col = 110; // geht bis 139, also 30 Fächer denkbar
    public static int noteMitTendenzAP1Col = 140; // geht bis 229, was wo reingehört weiß ich momentan noch nicht

    /// <summary>
    /// Liefert den String, den die OmnisDB zur übergebenen fpa-Note erwartet.
    /// </summary>
    /// <param name="note">Die fpaNote.</param>
    /// <returns>Der Omnis-DbString.</returns>
    public static string GetFpaString (fpaNote note)
    {
      switch(note)
      {
        case fpaNote.Entfaellt: return "-";
        case fpaNote.OhneErfolg: return "O";
        case fpaNote.MitErfolg: return "M";
        case fpaNote.MitGutemErfolg: return "G";
        case fpaNote.MitSehrGutemErfolg: return "S";
        default: throw new InvalidOperationException("unbekannte fpaNote "+note);
      }
    }

    public static string GetZeugnisartString(Zeugnisart art)
    {
      switch(art)
      {
        case Zeugnisart.Zwischenzeugnis: return "Z";
        case Zeugnisart.Jahreszeugnis: return "J";
        case Zeugnisart.Abschlusszeugnis: return "A";
        case Zeugnisart.Entlassungszeugnis: return "E";
        case Zeugnisart.Uebertrittszeugnis: return "U";
        case Zeugnisart.Bescheinigung: return "B";
        default: throw new InvalidOperationException("unbekannte Zeugnisart " + art);
      }
    }

    public static Zeugnisart ZeugnisartFromString(string zeugnisart)
    {
      switch (zeugnisart)
      {
        case "Z": return Zeugnisart.Zwischenzeugnis;
        case "J": return Zeugnisart.Jahreszeugnis;
        case "A": return Zeugnisart.Abschlusszeugnis;
        case "E": return Zeugnisart.Entlassungszeugnis;
        case "U": return Zeugnisart.Uebertrittszeugnis;
        case "B": return Zeugnisart.Bescheinigung;
        default: throw new InvalidOperationException("unbekannte Zeugnisart " + zeugnisart);
      }
    }

    public static string GetKlassenzielOderGefaehrdungString(KlassenzielOderGefaehrdung ziel)
    {
      switch (ziel)
      {
        case KlassenzielOderGefaehrdung.VorrueckenOK: return "-";
        case KlassenzielOderGefaehrdung.NotenausgleichGewaehrt: return "A";
        case KlassenzielOderGefaehrdung.VorrueckenNichtErhalten: return "N";
        case KlassenzielOderGefaehrdung.VorrueckenAufProbe: return "P";
        case KlassenzielOderGefaehrdung.AbschlusspruefungOhneErfolg: return "O";
        case KlassenzielOderGefaehrdung.NichtGefaehrdet: return "-";
        case KlassenzielOderGefaehrdung.BeiWeiteremAbsinkenGefaehrdet: return "B";
        case KlassenzielOderGefaehrdung.Gefaehrdet: return "G";
        case KlassenzielOderGefaehrdung.SehrGefaehrdet: return "S";
        case KlassenzielOderGefaehrdung.VorrueckenNichtMehrMoeglich: return "V";
        default: throw new InvalidOperationException("unbekannter Klassenzielerreichungs- oder Gefährdungsgrad "+ziel);
      }
    }

    public static string GetAbweisungString(Abweisung abweisung)
    {
      switch (abweisung)
      {
        case Abweisung.Nein: return "-";
        case Abweisung.Art53BayEUG: return "3";
        case Abweisung.Art54BayEUG: return "4";
        case Abweisung.Art55BayEUG: return "5";
        case Abweisung.Art53Und55BayEUG: return "8";
        default: throw new InvalidOperationException("unbekannte Abweisungsart "+abweisung);
      }
    }

    public static string GetBestandenString(AbschlusspruefungBestanden bestanden)
    {
      switch (bestanden)
      {
        case AbschlusspruefungBestanden.Bestanden: return "B";
        case AbschlusspruefungBestanden.BestandenMitInternemAusgleichArt45: return "BA";
        case AbschlusspruefungBestanden.BestandenMitNotenausgleichArt33: return "BN";
        case AbschlusspruefungBestanden.NichtBestanden: return "N";
        case AbschlusspruefungBestanden.NichtWiederholungArt54: return "NW";
        case AbschlusspruefungBestanden.Nachtermin: return "T";
        case AbschlusspruefungBestanden.NachterminGenehmigt: return "TG";
        case AbschlusspruefungBestanden.KeineFHRPruefung: return "K";
        default: throw new InvalidOperationException("unbekannter Grad des Bestehens " + bestanden);
      }
    }
  }

  /// <summary>
  /// Mögliche fpaNoten.
  /// </summary>
  public enum fpaNote
  {
    Entfaellt = 0,
    MitSehrGutemErfolg = 1,
    MitGutemErfolg = 2,
    MitErfolg = 3,
    OhneErfolg = 4
  }

  /// <summary>
  /// Mögliche Zeugnisarten.
  /// </summary>
  public enum Zeugnisart
  {
    Zwischenzeugnis = 0,
    Jahreszeugnis = 1,
    Abschlusszeugnis = 2,
    Entlassungszeugnis = 3,
    Uebertrittszeugnis = 4,
    Bescheinigung = 5
  }

  /// <summary>
  /// Mögliche Werte für Klassenzielerreichung oder Gefährdung.
  /// </summary>
  public enum KlassenzielOderGefaehrdung
  {
    VorrueckenOK = 0,
    NotenausgleichGewaehrt = 1,
    VorrueckenNichtErhalten = 2,
    VorrueckenAufProbe = 3,
    AbschlusspruefungOhneErfolg = 4,
    NichtGefaehrdet = 5, // beim Zwischenzeugnis
    BeiWeiteremAbsinkenGefaehrdet = 6, // beim Zwischenzeugnis
    Gefaehrdet = 7, // beim Zwischenzeugnis
    SehrGefaehrdet = 8, // beim Zwischenzeugnis
    VorrueckenNichtMehrMoeglich = 9 // beim Zwischenzeugnis
  }

  /// <summary>
  /// Abweisung bzw. im Zwischenzeugnis Gefahr der Abweisung
  /// </summary>
  public enum Abweisung
  {
    Nein = 0,
    Art53BayEUG = 1,
    Art54BayEUG = 2,
    Art55BayEUG = 3,
    Art53Und55BayEUG = 4
  }

  /// <summary>
  /// Abschlussprüfung Bestanden. Todo: Was heißt denn NichtWiederholung Art 54? Darf nicht wiederholen oder hat bei Wiederholung nicht bestanden?
  /// </summary>
  public enum AbschlusspruefungBestanden
  {
    Bestanden = 0,
    BestandenMitInternemAusgleichArt45 = 1,
    BestandenMitNotenausgleichArt33 = 2,
    NichtBestanden = 3,
    NichtWiederholungArt54 = 4,
    Nachtermin = 5,
    NachterminGenehmigt = 6,
    KeineFHRPruefung = 7 //gilt nur bei BOS
  }
}
