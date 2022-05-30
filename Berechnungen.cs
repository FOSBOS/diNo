using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace diNo
{
  /// <summary>
  /// Verwaltet durchzuführenden Berechnung am übergebenen Zeitpunkt
  /// </summary>
  public class Berechnungen
  {
    public delegate void Aufgabe(Schueler s);
    public List<Aufgabe> aufgaben = new List<Aufgabe>();    // speichert alle zu erledigenden Berechnungsaufgaben eines Schülers

    public Berechnungen() { }

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
      HjLeistung frzHj = null;  // Hj in der 2. FS, das am schlechtesten lief (relevant für 13. Klasse)

      // eine vorhandene Einbringung darf nicht überschrieben werden!
      if (s.Data.Berechungsstatus >= (byte)Berechnungsstatus.Einbringung) return;

      s.Data.Berechungsstatus = (byte)Berechnungsstatus.Einbringung; // wird ggf. überschrieben, wenn zu wenige da sind
      foreach (var fachNoten in s.getNoten.alleFaecher)
      {
        // Fachweise nicht die Punkte suchen        
        if (!fachNoten.getFach.NichtNC)
        {
          var kuerzel = fachNoten.getFach.Kuerzel;
          HjLeistung hjLeistung;
          byte anzUngueltig = 0;
          byte anzLeer = 0;

          var hjLeistungen = new List<HjLeistung>(); // enthält alle HjLeistungen dieses Fachs, die einbringbar wären
          // HjArt.GetFields().Where(x => x.IsLiteral).Select(x => x.GetValue(null)); TODO: Subset von Enum.HjArt bilden
          foreach (HjArt art in Enum.GetValues(typeof(HjArt))) // 12./13. Klasse
          {
            if (art > HjArt.FR) continue; // JN, GE usw. dürfen nicht verwendet werden
            hjLeistung = fachNoten.getHjLeistung(art);

            // sind ungültige dabei?
            if (hjLeistung == null)
            {
              if (art <= HjArt.Hj2)
                anzLeer++;
              continue;
            }
            if (hjLeistung.Status == HjStatus.Ungueltig)
            {
              anzUngueltig++;
              continue;
            }

            if (hjLeistung.Art == HjArt.AP || hjLeistung.Art == HjArt.FR || kuerzel == "FpA" /*TODO: Oder Seminarfach*/)
            {
              sowiesoPflicht.Add(hjLeistung);
            }
            else
            {
              hjLeistungen.Add(hjLeistung);
            }
          }
          if (anzLeer == 2) anzLeer = 0; // dann gehen wir von einem Fach aus, das nur in 11 unterrichtet wird

          if (s.hatVorHj)
          {
            hjLeistung = fachNoten.getVorHjLeistung(HjArt.Hj1); // Leistung aus 11/1
            if (hjLeistung != null)
            {
              if (kuerzel == "FpA") hjLeistung.SetStatus(HjStatus.Einbringen);
              else if (hjLeistungen.Count == 0) // in 12 nicht vorhanden --> einbringbar
              {
                if (hjLeistung.Status == HjStatus.Ungueltig) anzUngueltig++;
                else hjLeistungen.Add(hjLeistung);
              }
              else hjLeistung.SetStatus(HjStatus.NichtEinbringen); // kann nie eingebracht werden, wenn in 12 vorhanden
            }

            hjLeistung = fachNoten.getVorHjLeistung(HjArt.Hj2);
            if (hjLeistung != null)
            {
              if (kuerzel == "FpA") hjLeistung.SetStatus(HjStatus.Einbringen);
              else if (hjLeistung.Status == HjStatus.Ungueltig) anzUngueltig++;
              else hjLeistungen.Add(hjLeistung); // 11/2 vorhanden --> einbringbar            
            }
          }

          // jetzt stehen alle "normalen" Halbjahresleistungen in hjLeistungen.
          if (anzUngueltig + anzLeer > 1) // dann brauchen wir nicht weiter machen, keine Zulassung
          {
            s.Data.Berechungsstatus = (byte)Berechnungsstatus.ZuWenigeHjLeistungen;
            return;
          }
          // Sortieren, nur eine davon kann gestrichen werden
          if (hjLeistungen.Count > 0)
          {
            HJLSortierung(hjLeistungen);
            if (anzUngueltig > 0)
            {
              einbringen.AddRange(hjLeistungen);
            }
            // für allg. HSR verwendet: 2. FS nutzt nur was, wenn 2. Hj und Durchschnitt mind. 4 Punkte sind
            else if (frzHj == null && s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn && fachNoten.getFach.getKursniveau() > Kursniveau.Englisch
               && hjLeistungen.Count == 2 && hjLeistungen[1].Punkte >= 4 && hjLeistungen[0].Punkte + hjLeistungen[1].Punkte >= 7)
            {
              // die schlechtere merken wir uns für eine alternative Streichung vor
              frzHj = hjLeistungen[(hjLeistungen[0].Punkte < hjLeistungen[1].Punkte) ? 0 : 1];
              einbringen.AddRange(hjLeistungen); // alle einbringen
            }
            else
            {
              hjLeistungen.Sort((x, y) => y.Punkte.CompareTo(x.Punkte));
              einbringen.AddRange(hjLeistungen.GetRange(0, hjLeistungen.Count - 1)); // bis auf eine müssen eingebracht werden

              // rutscht man mit allen unter 4, obwohl das bei einer Streichung nicht passiert, lassen wir den auf jeden Fall weg
              if (UnbedingtStreichen(hjLeistungen))
                unbedingtStreichen.Add(hjLeistungen[hjLeistungen.Count - 1]);
              else streichen.Add(hjLeistungen[hjLeistungen.Count - 1]);
            }
          }
        }
        else // Nicht NC
        {
          HjLeistung hj;
          if ((hj = fachNoten.getHjLeistung(HjArt.Hj1)) != null && hj.Status != HjStatus.Ungueltig) hj.SetStatus(HjStatus.NichtEinbringen);
          if ((hj = fachNoten.getHjLeistung(HjArt.Hj2)) != null && hj.Status != HjStatus.Ungueltig) hj.SetStatus(HjStatus.NichtEinbringen);
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
          unbedingtStreichen.Sort((x, y) => y.Sortierung.CompareTo(x.Sortierung));
          einbringen.AddRange(unbedingtStreichen.GetRange(0, fehlend));
          unbedingtStreichen.RemoveRange(0, fehlend);
        }
      }
      else
      {
        streichen.Sort((x, y) => y.Sortierung.CompareTo(x.Sortierung));
        einbringen.AddRange(streichen.GetRange(0, fehlend));
        streichen.RemoveRange(0, fehlend);
        // alternative Streichung für 2. FS
        if (frzHj != null && streichen.Count > 0 && streichen[0].Punkte > frzHj.Punkte)
        {
          streichen[0].SetStatus(HjStatus.AlternativeEinbr);
          streichen.RemoveRange(0, 1);
        }
        //else
        //  frzHj = null; // haben wir nicht gebraucht, weil es eh besser war.
      }

      foreach (var hjLeistung in sowiesoPflicht.Union(einbringen))
        hjLeistung.SetStatus(HjStatus.Einbringen);

      foreach (var hjLeistung in streichen.Union(unbedingtStreichen))
        hjLeistung.SetStatus(HjStatus.NichtEinbringen);
    }

    // Belegt das Feld Sortierung in HjL (hjl sind einbringbare Hj eines Faches)
    private void HJLSortierung(List<HjLeistung> hjl)
    {
      hjl.Sort((x,y) => y.Punkte.CompareTo(x.Punkte));
      // ge wird berechnet, wenn die schlechteste gestrichen wird
      byte ge = (byte)Math.Round(hjl.GetRange(0,hjl.Count-1).Average((x)=> x.Punkte), MidpointRounding.AwayFromZero);
      
      foreach (var hj in hjl)
      {        
        hj.Sortierung = hj.Punkte * 16 - ge; // Punkte zählen immer mehr als das GE!
      }      
    }

    // liefert wahr, wenn sich ohne Streichung zusätzlich eine 5 (oder 6) ergeben würde
    private bool UnbedingtStreichen(List<HjLeistung> hjl)
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
      Punktesumme p = s.punktesumme;

      p.Clear();

      foreach (var f in s.getNoten.alleFaecher)
      {
        f.BerechneGesErg(p);
      }

      foreach (var f in s.Fachreferat)
        p.Add(PunktesummeArt.FR, f.Punkte);

      if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        FachSchuelerNoten fs2 = s.getNoten.ZweiteFSalt;
        // Ergänzungsprüfung zählt doppelt
        if (s.getZweiteFSArt() == ZweiteFSArt.ErgPr && !s.Data.IsAndereFremdspr2NoteNull())
          p.Add(PunktesummeArt.FremdspracheErgPr, s.Data.AndereFremdspr2Note, 2);

        // Frz. fortgeführt aus alter 12./13. Klasse
        else if (fs2 != null)
        {
          p.Add(PunktesummeArt.FremdspracheAus12, fs2.getHjLeistung(HjArt.Hj1).Punkte);
          p.Add(PunktesummeArt.FremdspracheAus12, fs2.getHjLeistung(HjArt.Hj2).Punkte);
        }
      }

      if (s.getNoten.AlternatEinbr != null)
        p.WriteToDB(s.getNoten.AlternatEinbr.Punkte - s.getNoten.AlternatZweiteFS.Punkte);
      else
        p.WriteToDB();
    }

    /// <summary>
    /// Aktualisiert das GE nur, wenn der Berechnungsstatus dafür gesetzt ist
    /// </summary>
    public void AktualisiereGE(Schueler s)
    {
      if (s.Data.Berechungsstatus >= (byte)Berechnungsstatus.Einbringung)
        BerechneGesErg(s);
      if (s.Data.Berechungsstatus == (byte)Berechnungsstatus.Gesamtergebnis)
        BerechneDNote(s);
    }

    private decimal FormelDNote(Schueler s, PunktesummeArt a)
    {
      int anz = s.punktesumme.Anzahl(a);
      if (anz == 0) return 0;
      //decimal erg = 17/ (decimal)3.0 - 5 * (decimal)s.punktesumme.Summe(PunktesummeArt.Gesamt) / (15*anz);
      decimal erg = (17 - (decimal)s.punktesumme.Summe(a) / anz) / 3;
      if (erg < 1)
      {
        erg = 1;
      }
      else
      {
        erg = Math.Floor(erg * 10) / 10; // auf 1 NK abrunden
      }
      return erg;
    }

    public void BerechneDNote(Schueler s)
    {
      if (s.Data.Berechungsstatus != (byte)Berechnungsstatus.ZuWenigeHjLeistungen)
      {
        decimal d;
        d = FormelDNote(s, PunktesummeArt.Gesamt);
        if (d == 0) s.Data.SetDNoteNull(); else s.Data.DNote = d;

        d = FormelDNote(s, PunktesummeArt.GesamtFachgebHSR);
        if (d == 0) s.Data.SetDNoteFachgebHSRNull(); else s.Data.DNoteFachgebHSR = d;

        s.Data.Berechungsstatus = (byte)Berechnungsstatus.Gesamtergebnis;
      }
      else
      {
        s.Data.SetDNoteNull();
        s.Data.SetDNoteFachgebHSRNull();
      }
    }

    public void BestimmeSprachniveau(Schueler s)
    {      
      // neue Logik ist, dass ein einmal erreichtes Sprachniveau an der FOS nicht mehr gelöscht wird.
      // Problem, wenn sich wegen falscher Noteneingabe nachträglich was ändert (dann ggf. manuell Sprachniveau löschen).
      // ta.DeleteBySchuelerIdAndArt(s.Id, (byte)HjArt.Sprachenniveau,(int)s.getKlasse.Jahrgangsstufe); 

      HjLeistung ge, hj2, ap;
      foreach (var f in s.getNoten.alleSprachen)
      {
        Kursniveau n = f.getFach.getKursniveau();
        ge = f.getHjLeistung(HjArt.GesErgSprache);
        if (ge == null || ge.Punkte < 4) continue; // Gesamtergebnis muss immer mind. 4P sein.

        hj2 = f.getHjLeistung(HjArt.Hj2);
        ap = f.getHjLeistung(HjArt.AP);

        bool istSAP = n == Kursniveau.Englisch;
        if (hj2 == null || ap == null && istSAP) continue;
        if (hj2.Punkte < 4 && (!istSAP || ap.Punkte < 4)) continue; // Prüfung oder Hj2 muss mind. 4 sein

        // Spachniveau anlegen oder aktualisieren
        HjLeistung niveau = f.getHjLeistung(HjArt.Sprachenniveau);
        HjLeistung.CreateOrUpdateSprachniveau(niveau, s.Id, f.getFach, s.getKlasse.Jahrgangsstufe, Fremdsprachen.GetSprachniveau(f.getFach.getKursniveau(), s.getKlasse.Jahrgangsstufe));
      }

      if (s.getZweiteFSArt() == ZweiteFSArt.ErgPr) // mitgebrachte Noten werden nicht ausgewiesen
                                                   // Sprachniveau von F-f aus 12/13alt sollte vom Import schon vorhanden sein.
      {
        Fach f = Zugriff.Instance.FachRep.Find(s.Data.AndereFremdspr2Fach);
        HjLeistung niveau = null;
        foreach (var spr in s.getNoten.alleSprachen) // prüfen, ob dafür schon ein Sprachniveau vorliegt
          if (spr.getFach == f)
          {
            niveau = spr.getHjLeistung(HjArt.Sprachenniveau);
          }
        HjLeistung.CreateOrUpdateSprachniveau(niveau, s.Id, f, Jahrgangsstufe.Dreizehn, Sprachniveau.B1);
      }
    }
  }

  public enum Berechnungsstatus
  {
    Unberechnet = 0,
    ZuWenigeHjLeistungen,
    Einbringung,    // GE liegt aber auch schon provisorisch vor (noch keine D-Note)
    Gesamtergebnis  // Abi ist da und die Durchschnittsnote ist berechnet
  }
}