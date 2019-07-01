using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  /// <summary>
  /// Verwaltet durchzuführenden Berechnung am übergebenen Zeitpunkt
  /// </summary>
  public class Berechnungen
  {      
    public delegate void Aufgabe(Schueler s);
    public List<Aufgabe> aufgaben = new List<Aufgabe>();    // speichert alle zu erledigenden Berechnungsaufgaben eines Schülers

    public Berechnungen()
    { }

    public Berechnungen(Zeitpunkt zeitpunkt)
    {      
      if (zeitpunkt == Zeitpunkt.ErstePA)
        aufgaben.Add(BerechneEinbringung);

      if (zeitpunkt >= Zeitpunkt.ErstePA && zeitpunkt <= Zeitpunkt.DrittePA)
        aufgaben.Add(BerechneGesErg);

      if (zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA)
        aufgaben.Add(BerechneDNote);

      if (zeitpunkt == Zeitpunkt.DrittePA)
        aufgaben.Add(BestimmeSprachniveau);
    }

    /// <summary>
    /// Führt alle Berechnungen, die in den Aufgaben vermerkt sind, für einen Schüler durch.
    /// </summary>
    public void BerechneSchueler(Schueler s)
    {      
      foreach (var a in aufgaben)
      {
        a(s);
      }
      s.Save();
      s.Refresh();
    }

    /*
    public void BerechneSchueler(Schueler s)
    { 
      schueler = s;
      if ((zeitpunkt == Zeitpunkt.ZweitePA || zeitpunkt == Zeitpunkt.DrittePA) && s.getKlasse.Jahrgangsstufe > Jahrgangsstufe.Elf)
      {
        s.berechneDNote(false);
        if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          s.berechneDNote(true); // DNote für allg. HSR

        s.Save();
      }
    }
    */



    
    /// <summary>
    /// Methode macht einen Vorschlag zur Einbringung der Halbjahresleistungen eines Schülers.
    /// </summary>
    /// <param name="s">Der Schueler.</param>
    public void BerechneEinbringung(Schueler s)
    {
      var sowiesoPflicht = new List<HjLeistung>(); // zählen nicht zu den 25 bzw. 17 HjLeistungen
      var einbringen = new List<HjLeistung>(); // enthält alle "weiteren" HjErgebnisse (außer FR, FPA, AP)
      var streichen = new List<HjLeistung>();
      var unbedingtStreichen = new List<HjLeistung>();

      // eine vorhandene Einbringung darf nicht überschrieben werden!
      if (s.AlteFOBOSO() || s.Data.Berechungsstatus >= (byte)Berechnungsstatus.Einbringung) return;

      s.Data.Berechungsstatus = (byte)Berechnungsstatus.Einbringung; // wird ggf. überschrieben, wenn zu wenige da sind
      foreach (var fachNoten in s.getNoten.alleFaecher)
      {
        // Fachweise nicht die Punkte suchen        
        if (!fachNoten.getFach.NichtNC)
        {
          var kuerzel = fachNoten.getFach.Kuerzel;
          HjLeistung hjLeistung;

          var hjLeistungen = new List<HjLeistung>();
          // HjArt.GetFields().Where(x => x.IsLiteral).Select(x => x.GetValue(null)); TODO: Subset von Enum.HjArt bilden
          foreach (HjArt art in Enum.GetValues(typeof(HjArt))) // 12./13. Klasse
          {
            if (art > HjArt.FR) continue; // JN, GE usw. dürfen nicht verwendet werden
            hjLeistung = fachNoten.getHjLeistung(art);
            if (hjLeistung == null)
              continue;

            if (hjLeistung.Art == HjArt.AP || hjLeistung.Art == HjArt.FR || kuerzel == "FpA" /*TODO: Oder Seminarfach*/)
            {
              sowiesoPflicht.Add(hjLeistung);
            }
            else
            {
              hjLeistungen.Add(hjLeistung);
            }
          }

          hjLeistung = fachNoten.getVorHjLeistung(HjArt.Hj1); // Leistung aus 11/1
          if (hjLeistung != null)
          {
            if (kuerzel == "FpA") hjLeistung.SetStatus(HjStatus.Einbringen);
            else if (hjLeistungen.Count == 0) hjLeistungen.Add(hjLeistung); // in 12 nicht vorhanden --> einbringbar
            else hjLeistung.SetStatus(HjStatus.NichtEinbringen); // kann nie eingebracht werden, wenn in 12 vorhanden
          }

          hjLeistung = fachNoten.getVorHjLeistung(HjArt.Hj2);
          if (hjLeistung != null)
          {
            if (kuerzel == "FpA") hjLeistung.SetStatus(HjStatus.Einbringen);
            else hjLeistungen.Add(hjLeistung); // 11/2 vorhanden --> einbringbar            
          }

          // jetzt stehen alle "normalen" Halbjahresleistungen in hjLeistungen.
          // Sortieren, nur eine davon kann gestrichen werden
          if (hjLeistungen.Count > 0)
          {
            hjLeistungen.Sort((x, y) => y.Punkte.CompareTo(x.Punkte));
            einbringen.AddRange(hjLeistungen.GetRange(0, hjLeistungen.Count - 1)); // bis auf eine müssen eingebracht werden

            // rutscht man mit allen unter 4, obwohl das bei einer Streichung nicht passiert, lassen wir den auf jeden Fall weg
            if (UnbedingtStreichen(hjLeistungen))
              unbedingtStreichen.Add(hjLeistungen[hjLeistungen.Count - 1]);
            else streichen.Add(hjLeistungen[hjLeistungen.Count - 1]);
          }
        }
        else // Nicht NC
        {
          HjLeistung hj;
          if ((hj = fachNoten.getHjLeistung(HjArt.Hj1))!=null) hj.SetStatus(HjStatus.NichtEinbringen);
          if ((hj = fachNoten.getHjLeistung(HjArt.Hj2)) != null) hj.SetStatus(HjStatus.NichtEinbringen);
        }
      }

      int fehlend = s.GetAnzahlEinbringung() - einbringen.Count;

      if (fehlend > streichen.Count) // nicht genügend vorhanden ==> ggf. aus unbedingtStreichen holen
      {
        einbringen.AddRange(streichen);
        fehlend -= streichen.Count;
        streichen.Clear();        
        if (fehlend > unbedingtStreichen.Count) // nicht mal da sind genügend vorhanden ==> S hat insgesamt zu wenige HjL
        {
          einbringen.AddRange(unbedingtStreichen);
          unbedingtStreichen.Clear();
          fehlend -= streichen.Count;
          s.Data.Berechungsstatus = (byte)Berechnungsstatus.ZuWenigeHjLeistungen;          
        }
        else
        {
          unbedingtStreichen.Sort((x, y) => y.Punkte.CompareTo(x.Punkte));
          einbringen.AddRange(unbedingtStreichen.GetRange(0, fehlend));
          unbedingtStreichen.RemoveRange(0, fehlend);
        }
      }
      else
      {
        streichen.Sort((x, y) => y.Punkte.CompareTo(x.Punkte));
        einbringen.AddRange(streichen.GetRange(0, fehlend));
        streichen.RemoveRange(0, fehlend);
      }

      foreach (var hjLeistung in sowiesoPflicht.Union(einbringen))
      {
        hjLeistung.Status = HjStatus.Einbringen;
        hjLeistung.WriteToDB();
      }
      foreach (var hjLeistung in streichen.Union(unbedingtStreichen))
      {
        hjLeistung.Status = HjStatus.NichtEinbringen;
        hjLeistung.WriteToDB();
      }      
    }

    // liefert wahr, wenn sich ohne Streichung zusätzlich eine 5 (oder 6) ergeben würde
    private bool UnbedingtStreichen(List <HjLeistung>hjl)
    {
      double s = hjl.Sum((x) => x.Punkte) / (double)hjl.Count;
      double s1 = hjl.GetRange(0, hjl.Count - 1).Sum((x) => x.Punkte) / (double)(hjl.Count - 1);

      return (s1 >= 3.5 && s < 3.5 || s1 >= 1.0 && s < 1.0);
    }

    
    /// <summary>
    /// Berechnet das Gesamtergebnis aller Fächer, sowie die Punktesumme
    /// </summary>
    public void BerechneGesErg(Schueler s)
    {
      if (s.AlteFOBOSO()) return;

      Punktesumme p = new Punktesumme(s);
      p.Clear();
      
      foreach (var f in s.getNoten.alleFaecher)
      {
        f.BerechneGesErg(p);
      }

      foreach (var f in s.Fachreferat)
        p.Add(PunktesummeArt.FR, f.Punkte);

      p.WriteToDB();            
    }

    public void BerechneDNote(Schueler s)
    {
      if (s.AlteFOBOSO())
      {
        berechneDNoteAlt(s, false);
        if (s.getKlasse.Jahrgangsstufe== Jahrgangsstufe.Dreizehn)
          berechneDNoteAlt(s, true);
        return;
      }

      int anz = s.punktesumme.Anzahl(PunktesummeArt.Gesamt);
      if (anz>0 && s.Data.Berechungsstatus != (byte)Berechnungsstatus.ZuWenigeHjLeistungen)
      {
        //decimal erg = 17/ (decimal)3.0 - 5 * (decimal)s.punktesumme.Summe(PunktesummeArt.Gesamt) / (15*anz);
        decimal erg = (17 - (decimal)s.punktesumme.Summe(PunktesummeArt.Gesamt) / anz) / 3;
        if (erg < 1)
        {
          erg = 1;
        }
        else
        {
          erg = Math.Floor(erg * 10) / 10; // auf 1 NK abrunden
        }
        s.Data.DNote = erg;
        s.Data.Berechungsstatus = (byte)Berechnungsstatus.Gesamtergebnis;
      }
      else
      {
        s.Data.SetDNoteNull();
      }    
    }

    private void berechneDNoteAlt(Schueler s, bool allgHSR)
    {
      int summe = 0, anz = 0;
      decimal erg;
      var faecher = s.getNoten.alleKurse;
      bool FranzVorhanden = !s.Data.IsAndereFremdspr2NoteNull();

      // Französisch wird nur in der 13. Klasse gewertet, wenn der Kurs belegt ist und
      // der Schüler nicht nur fachgebundene HSR bekommt (z.B. wegen Note 5 in F)
      // F-Wi zählt immer (auch 12. Klasse), weil es WIn ersetzt
      // eine andere eingetragene 2. Fremdsprache zählt auch immer (in der 13.); dies kann eine RS-Note, Ergänzungspr.,
      // aber auch bei fortgeführtem Franz. die Note der 11./12. oder 13. Klasse sein. Dadurch kann F-Wi sogar doppelt zählen!

      foreach (var fach in faecher)
      {
        // alle Fächer außer Sport und Kunst, Franz. nur in der 13. 
        var fk = fach.getFach.Kuerzel;
        byte? note = fach.getSchnitt(Halbjahr.Zweites).Abschlusszeugnis;

        if (note == null || fk == "Ku" || fk == "Smw" || fk == "Sw" || fk == "Sm" || ((fk == "F" || fk == "Frz" ) && !allgHSR))
          continue;

        // liegen die Voraussetzungen für allg. HSR vor?
        if (allgHSR && (fk == "Frz" || fk == "F" && note.GetValueOrDefault() > 3))
          FranzVorhanden = true;

        if (note == 0)
        {
          summe--; // Punktwert 0 wird als -1 gezählt
        }
        else
        {
          summe += note.GetValueOrDefault();
        }
        anz++;
      }

      if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        if (!s.Seminarfachnote.IsGesamtnoteNull())
        {
          summe += s.Seminarfachnote.Gesamtnote;
          anz++;
        }

        // Alternative 2. Fremdsprache für die allgemeine Hochschulreife
        if (allgHSR && !s.Data.IsAndereFremdspr2NoteNull())
        {
          summe += s.Data.AndereFremdspr2Note;
          anz++;
        }
      }

      if (allgHSR && !FranzVorhanden)
        s.Data.SetDNoteAllgNull();
      else if (anz > 0)
      {
        erg = (17 - (decimal)summe / anz) / 3;
        if (erg < 1)
        {
          erg = 1;
        }
        else
        {
          erg = Math.Floor(erg * 10) / 10; // auf 1 NK abrunden
        }
        if (allgHSR)
          s.Data.DNoteAllg = erg;
        else
          s.Data.DNote = erg;
      }
      else
      {
        s.Data.SetDNoteNull();
      }
    }

    public void BestimmeSprachniveau(Schueler s)
    {
      if (s.AlteFOBOSO()) return;
      var ta = new HjLeistungTableAdapter();
      ta.DeleteBySchuelerIdAndArt(s.Id, (byte)HjArt.Sprachenniveau); // vorher löschen, falls Stufe inzwischen nicht mehr erreicht wird

      HjLeistung ge,hj2,ap;
      foreach (var f in s.getNoten.alleSprachen)
      {
        Kursniveau n = f.getFach.getKursniveau();        
        if (n!=Kursniveau.None)
        {
          ge = f.getHjLeistung(HjArt.GesErgSprache);
          if (ge == null || ge.Punkte < 4) continue; // Gesamtergebnis muss immer mind. 4P sein.
            
          hj2 = f.getHjLeistung(HjArt.Hj2);
          ap = f.getHjLeistung(HjArt.AP);

          bool istSAP = n == Kursniveau.Englisch;
          if (hj2 == null || ap == null && istSAP) continue;
          if (hj2.Punkte < 4 && (!istSAP || ap.Punkte < 4)) continue; // Prüfung oder Hj2 muss mind. 4 sein

          HjLeistung niveau = new HjLeistung(s.Id, f.getFach, HjArt.Sprachenniveau, s.getKlasse.Jahrgangsstufe);
          niveau.Punkte = (byte)Fremdsprachen.GetSprachniveau(f.getFach.getKursniveau(), s.getKlasse.Jahrgangsstufe);
          niveau.WriteToDB();
        }
      }

    }
  }

  public enum Berechnungsstatus
  {
    Unberechnet = 0,
    ZuWenigeHjLeistungen,
    Einbringung,    
    Gesamtergebnis
  }
}