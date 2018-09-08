using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
    /// <summary>
    /// Verwaltet alle Noten eines Schülers
    /// </summary>
    public class SchuelerNoten
    {
        private Schueler schueler;
        public List<FachSchuelerNoten> alleKurse; // enthält nur die aktuell besuchten Kurse
        public List<FachSchuelerNoten> alleFaecher; // enthält alle Fächer, die der S jemals belegt hat
 
        // die folgendes Array verwaltet die Anzahl der Einser, Zweier, usw., getrennt nach SAP-Fach und Nebenfach
        // anzahlNoten[6,1] ergibt z.B. die Anzahl der Sechser in SAP-Fächern, anzahlNoten[5,0] die Anzahl der Fünfer in Nebenfächern
        private int[,] anzahlNoten;
        private Zeitpunkt zeitpunkt= (Zeitpunkt)Zugriff.Instance.aktZeitpunkt;
        public string Unterpunktungen;
        public bool hatDeutsch6 = false; // kann nicht ausgeglichen werden
        public int anz4P=0;
        public double Punkteschnitt = 0;

        public SchuelerNoten(Schueler s)
        {
          schueler = s;
          diNoDataSet.KursDataTable kurse = schueler.Kurse; // ermittle alle Kurse, die der S besucht
          alleFaecher = new List<FachSchuelerNoten>();
          alleKurse = new List<FachSchuelerNoten>();
          foreach (var kurs in kurse)
          {
            var fsn = new FachSchuelerNoten(schueler, kurs.Id);
            alleFaecher.Add(fsn);
            alleKurse.Add(fsn);
          }

          // alle Fächer des Schülers ohne Kurs finden und diese HjLeistungen laden 
          diNoDataSet.FachDataTable fDT = (new FachTableAdapter()).GetFaecherOhneKurseBySchuelerId(schueler.Id);
          foreach (var fachR in fDT)
          {
            Fach fach = Zugriff.Instance.FachRep.Find(fachR.Id);
            alleFaecher.Add(new FachSchuelerNoten(schueler, fach));
          }
          Zweig z = (schueler.AlteFOBOSO() ? Zweig.None : schueler.Zweig); // Profilfächer haben neue Sortierung
          alleFaecher.Sort((x,y) => x.getFach.Sortierung(z).CompareTo(y.getFach.Sortierung(z)));
          alleKurse.Sort((x,y) => x.getFach.Sortierung(z).CompareTo(y.getFach.Sortierung(z)));

          // Schnitt, Unterpunktungen etc. berechnen
          anzahlNoten = new int[7, 2];
          InitAnzahlNoten();
        }

        public FachSchuelerNoten getFach(int kursid)
        {
            foreach (FachSchuelerNoten f in alleFaecher)
            {
                if (f.kursId == kursid) return f;
            }
            throw new IndexOutOfRangeException("FachSchuelerNoten.getFach: falsche kursid");            
        }

    /// <summary>
    /// Liefert die Noten des Schülers im übergebenen Fach.
    /// </summary>
    /// <param name="fachKuerzel">Das Fachkürzel.</param>
    /// <returns>Die FachNoten oder null, wenn der Schüler das fach nicht belegt.</returns>
    public FachSchuelerNoten FindeFach(string fachKuerzel, bool throwExceptionIfNotFound)
    {
      foreach (FachSchuelerNoten f in alleFaecher)
      {
        if (f.getFach.Kuerzel.Equals(fachKuerzel, StringComparison.OrdinalIgnoreCase)) return f;
      }
       
      if (throwExceptionIfNotFound)
      {
        throw new InvalidOperationException("Der Schüler belegt Fach " + fachKuerzel + " gar nicht.");
      }

      return null;
    }

    public IList<FachSchuelerNotenDruckAlt> SchuelerNotenDruckAlt(Bericht rptName)
    {
      IList<FachSchuelerNotenDruckAlt> liste = new List<FachSchuelerNotenDruckAlt>();
      foreach (FachSchuelerNoten f in alleKurse)
      {                
        liste.Add(new FachSchuelerNotenDruckAlt(f, f.getFach.IstSAFach(schueler.Zweig, schueler.getKlasse.Jahrgangsstufe), rptName));
      }
      
      if (schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Dreizehn)
      {
        if (!schueler.Data.IsAndereFremdspr2NoteNull())
        {
          liste.Add(new FachSchuelerNotenDruckAlt(
            (schueler.Data.IsAndereFremdspr2TextNull() ? "Andere 2. Fremdsprache" :  schueler.Data.AndereFremdspr2Text ),
            schueler.Data.AndereFremdspr2Note));
        }
        liste.Add(new FachSchuelerNotenDruckAlt(schueler.Seminarfachnote));
      }

      return liste;
    }


    /// <summary>
    /// Liefert eine Liste in der je Fach alle Noten in druckbarer Form vorliegen.
    /// </summary>
    public IList<NotenDruck> SchuelerNotenDruck(Bericht rptName)
    {
      IList<NotenDruck> liste = new List<NotenDruck>();      
      foreach (FachSchuelerNoten f in alleKurse)
      {               
        liste.Add(NotenDruck.CreateNotenDruck(f,rptName));
      }
      /*
      if (schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Dreizehn)
      {
        if (!schueler.Data.IsAndereFremdspr2NoteNull())
        {
          liste.Add(new NotenSjDruck(
            (schueler.Data.IsAndereFremdspr2TextNull() ? "Andere 2. Fremdsprache" :  schueler.Data.AndereFremdspr2Text ),
            schueler.Data.AndereFremdspr2Note));
        }
        liste.Add(new FachSchuelerNotenDruckKurz(schueler.Seminarfachnote));
      }
      */
      // FPA ausgeben für Notenmitteilung (im Notenbogen als Bemerkung)
      /*if (rptName=="diNo.rptNotenmitteilung.rdlc" && schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Elf)
      {
        liste.Add(new NotenDruck(schueler.FPANoten));
      }*/
      return liste;
    }

    public IList<NotenDruck> SchuelerNotenZeugnisDruck(Bericht rptName)
    {
      IList<NotenDruck> liste = new List<NotenDruck>();      
      foreach (FachSchuelerNoten f in alleKurse)
      {
        if (rptName!=Bericht.Gefaehrdung || f.getRelevanteNote(zeitpunkt)<=4)
          liste.Add(new NotenZeugnisDruck(f, rptName));
      }
      if (rptName != Bericht.Gefaehrdung && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
      {        
        NotenZeugnisDruck f = new NotenZeugnisDruck(schueler.FPANoten);          
        liste.Add(f);
      }

      return liste;
    }
     
    // Zu diesem Zeitpunkt werden die Notenanzahlen gebildet,
    // wird dieser geändert, muss neu gerechnet werden
    public void SetZeitpunkt(Zeitpunkt z)
    {
      if (zeitpunkt!=z)
      {
        anzahlNoten = null;
        anzahlNoten = new int[7, 2];
        zeitpunkt = z;
        InitAnzahlNoten();        
      }
    }
    
    // liefert Anzahl der Sechser, Fünfer, ..., egal ob Prüfungsfach oder nicht
    public int AnzahlNoten(int note)
    {
      return AnzahlNoten(note,true) + AnzahlNoten(note, false);      
    }

    // Doku s. private Variable
    public int AnzahlNoten(int note, bool SAP)
    {
      if (SAP) return anzahlNoten[note,1];
      else return anzahlNoten[note,0];
    }

    private void InitAnzahlNoten()
    {      
      string kuerzel;
      int Punktesumme = 0;
      int AnzahlFaecher = 0;
      Unterpunktungen="";
      foreach (var fachNoten in alleKurse)
      {
        kuerzel = fachNoten.getFach.Kuerzel;
        if (kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Sw" || kuerzel == "Sm" || kuerzel == "Ku") continue;  // keine Vorrückungsfächer
        byte? relevanteNote = fachNoten.getRelevanteNote(zeitpunkt);
        int istSAP = fachNoten.getFach.IstSAPFach(schueler.Zweig, schueler.getKlasse.Jahrgangsstufe <= Jahrgangsstufe.Vorklasse) ? 1:0;
        if (relevanteNote != null)
        {
          Punktesumme += relevanteNote.GetValueOrDefault();
          AnzahlFaecher++;                       
          if (relevanteNote == 0)
          {
            anzahlNoten[6,istSAP]++;
            if (kuerzel=="D") hatDeutsch6=true;
          }
          else if (relevanteNote < 4) anzahlNoten[5,istSAP]++;          
          else if (relevanteNote >=13) anzahlNoten[1,istSAP]++;
          else if (relevanteNote >= 10) anzahlNoten[2,istSAP]++;
          else if (relevanteNote >= 7) anzahlNoten[3,istSAP]++;
          else anzahlNoten[4,istSAP]++; 

          if (relevanteNote==4) anz4P++; // nur 4P für Gefährdungsmitteilung

          if (relevanteNote <4 || relevanteNote == 4 && zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
            Unterpunktungen += fachNoten.getFach.Kuerzel + "(" + relevanteNote +") ";
        }
      }

      // TODO: in alleFaecher integrieren
      if (!schueler.Seminarfachnote.IsGesamtnoteNull())
      {
        int relevanteNote = schueler.Seminarfachnote.Gesamtnote;
        Punktesumme += relevanteNote;
        AnzahlFaecher++;

        if (relevanteNote == 0) anzahlNoten[6,0]++;          
        else if (relevanteNote < 4) anzahlNoten[5,0]++;
        else if (relevanteNote >=13) anzahlNoten[1,0]++;
        else if (relevanteNote >= 10) anzahlNoten[2,0]++;        

        if (relevanteNote <4)
          Unterpunktungen += "Sem (" + relevanteNote +") ";
      }
      Punkteschnitt = Math.Round((double)Punktesumme / AnzahlFaecher, 2, MidpointRounding.AwayFromZero);
      if (Unterpunktungen != "" && !schueler.AlteFOBOSO() && !(zeitpunkt==Zeitpunkt.Jahresende && schueler.getKlasse.Jahrgangsstufe <= Jahrgangsstufe.Vorklasse))
        Unterpunktungen += " Schnitt: " + String.Format("{0:0.00}", Punkteschnitt);
    }

    public bool HatNichtBestanden()
    {
      if (schueler.AlteFOBOSO() && zeitpunkt!=Zeitpunkt.ProbezeitBOS)        
        return AnzahlNoten(6) > 0 || AnzahlNoten(5) > 1;

      // Achtung: Vorklasse hat am Jahresende eine besondere Bestanden-Regelung
      else if (zeitpunkt == Zeitpunkt.Jahresende && schueler.getKlasse.Jahrgangsstufe <= Jahrgangsstufe.Vorklasse)
      {
        if (AnzahlNoten(6) == 0 && AnzahlNoten(5) == 0) return false;  // bestanden
        if (AnzahlNoten(6) > 0 || AnzahlNoten(5) > 1) return true;  // da geht nichts mehr
        if (AnzahlNoten(5, true) > 0) // eine 5 in D,E oder M kann nur dadurch ausgeglichen werden
          return !(AnzahlNoten(1, true) + AnzahlNoten(2, true) > 0 || AnzahlNoten(3, true) > 1);
        else
          return !(AnzahlNoten(1) + AnzahlNoten(2) > 0 || AnzahlNoten(3) > 1);
      }
      else 
        return !(AnzahlNoten(6) == 0 && AnzahlNoten(5) == 0 ||
          AnzahlNoten(6) == 0 && AnzahlNoten(5) == 1 && Punkteschnitt >= 5 ||
          (AnzahlNoten(6) * 2 + AnzahlNoten(5)) == 2 && Punkteschnitt >= 6);
    }

    public bool KannAusgleichen()
    {
      // geht nur, wenn 1x6 und keine 5 oder 2x5 und keine 6 vorliegt.
      if (hatDeutsch6 || 2*AnzahlNoten(6) + AnzahlNoten(5) >2 || schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Vorklasse)
        return false;

      if (AnzahlNoten(3,true)>=3) return true; // Ausgleich mit 3x3 in Prüfungsfächern
      if (AnzahlNoten(1)>=1 || AnzahlNoten(2)>=2) // Ausgleich mit 1x1 oder 2x2
      {
        // Prüfungsfächer müssen mit Prüfungsfächern ausgeglichen werden:
        if (2*AnzahlNoten(6,true) + AnzahlNoten(5, true) <= 2*AnzahlNoten(1,true) + AnzahlNoten(2,true))
          return true;
      }
      return false;
    }

    public bool HatIn12KeinePZ()
    { // überall mindestens eine 3
      return AnzahlNoten(6) == 0 && AnzahlNoten(5) == 0 && AnzahlNoten(4) == 0;
    }
    
    public bool MAPmoeglich()
    {
      // Anzahl 5er und 6er nach einen MAP mit "bestmöglichem" Ergebnis
      int anz5=0; 
      int anz6=0;
      bool MAPinSAPFach=false; // je eine MAP möglich
      bool MAPinNebenFach=false;
      bool istSAPFach;
      decimal? jf=null;
      decimal? sap=null;
      
      // Seminarfachnote ist gesetzt
      if (!schueler.Seminarfachnote.IsGesamtnoteNull() && schueler.Seminarfachnote.Gesamtnote<4) anz5++;

      foreach (var f in alleKurse)
      {
        string kuerzel = f.getFach.Kuerzel;
        if (kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Sw" || kuerzel == "Sm" || kuerzel == "Ku") continue;  // keine Vorrückungsfächer

        byte? note = f.getRelevanteNote(Zeitpunkt.ZweitePA);
        if (note<4)
        {          
          jf = f.getSchnitt(Halbjahr.Zweites).JahresfortgangMitKomma;
          istSAPFach = f.getFach.IstSAPFach(schueler.Zweig);

          // Nur Fächer in denen eine MAP möglich ist (nicht Englisch und Fächer aus der 11. Klasse)
          if (kuerzel!="E" && jf!=null)
          {
            // 1x MAP im Prüfungsfach und 1x im Nebenfach möglich:
            if (istSAPFach && !MAPinSAPFach) 
            {
              if (f.getNotenanzahl(Halbjahr.Zweites,Notentyp.APSchriftlich)==0) // Note liegt ggf. noch nicht vor.
                return true;                                                    // Meldung über NotenanzahlChecker
              
              sap = f.getNoten(Halbjahr.Zweites,Notentyp.APSchriftlich)[0];
              byte noteMoegl = Notentools.BerechneZeugnisnote(jf,sap,15);
              if (noteMoegl>=4 || note==0) // wenn er auf dem 5er bleibt, soll er lieber ein anderes Fach nehmen
              {
                note = noteMoegl;
                MAPinSAPFach = true;
              }
            }
            if (!istSAPFach && !MAPinNebenFach)
            {
              note = 5; // im Nebenfach nie ein Problem (wenn er sich anstrengt)
              MAPinNebenFach = true;
            }
          }
          if (note==0) anz6++;
          else if (note<4) anz5++;
        }
      }    

      return (anz6==0 && anz5<=1); // dann hätte er bestanden
    }

  }

    /// <summary>
    /// Verwaltet alle Noten eines Schülers in einem Fach (=Kurs)
    /// </summary>
    public class FachSchuelerNoten
    {
        public Schueler schueler;        
        private Fach fach=null;
        public bool NoteUngueltig = false;
        public int kursId
        {
            get;
            private set;
        }


        // das Array wird über das Halbjahr und den Notentyp indiziert, 
        // jedes Arrayelement enthält eine Liste mit Noten dieses Typs.
        private IList<int>[,] noten = new List<int>[Enum.GetValues(typeof(Halbjahr)).Length,Enum.GetValues(typeof(Notentyp)).Length];
        private BerechneteNote[] schnitte = new BerechneteNote[Enum.GetValues(typeof(Halbjahr)).Length];
        private HjLeistung[] hjLeistung = new HjLeistung[Enum.GetValues(typeof(HjArt)).Length];
        private HjLeistung[] vorHjLeistung = new HjLeistung[2]; // enthält nur 11/1 und 11/2

        public FachSchuelerNoten(Schueler aschueler, int akursid)
        {
            kursId = akursid;        
            schueler = aschueler;
            
            foreach (Halbjahr hj in Enum.GetValues(typeof(Halbjahr)))
            {
                // erstmal leere Notenlisten anlegen
                foreach (Notentyp typ in Enum.GetValues(typeof(Notentyp)))
                    noten[(int)hj,(int)typ] = new List<int>();
            }

            // Notenlisten füllen je Typ und Halbjahr
            LeseNotenAusDB();
        }

        // Kursunabhängige HjLeistungen (z.B. abgelegte Fächer oder Seminararbeit) können hier erzeugt werden
        public FachSchuelerNoten(Schueler aschueler, Fach aFach)
        {
          fach = aFach;
          kursId = 0;
          schueler = aschueler;
                            
          LeseNotenAusDB();
        }

        private void LeseNotenAusDB()
        {
          if (kursId>0)
          { 
            diNoDataSet.NoteDataTable notenDT;
            notenDT = new NoteTableAdapter().GetDataBySchuelerAndKurs(schueler.Id, kursId);
            foreach (var noteR in notenDT)
            {
              noten[noteR.Halbjahr,noteR.Notenart].Add(noteR.Punktwert);
            }

            // Schnitte werden direkt gelesen
            diNoDataSet.BerechneteNoteDataTable bnotenDT;
            // liefert max. 2 Datensätze (einen für 1. und 2. Hj.), historische Stände werden nicht geliefert
            bnotenDT = new BerechneteNoteTableAdapter().GetDataBySchuelerAndKurs(kursId, schueler.Id);
            foreach (var bnoteR in bnotenDT)
            {                
              schnitte[(int)(bnoteR.ErstesHalbjahr ? Halbjahr.Erstes : Halbjahr.Zweites)] = 
                        new BerechneteNote(kursId, schueler.Id, bnoteR);
            }
          }
          // HjLeistungen          
          // nur die HjLeistungen holen, die der aktuellen JgStufe entsprechen
          int jg = (int)schueler.getKlasse.Jahrgangsstufe;
          var hjDT = new HjLeistungTableAdapter().GetDataBySchuelerAndFach(schueler.Id,getFach.Id).Where(x => x.JgStufe == jg);
          foreach (var hjR in hjDT)
          {                
            hjLeistung[(int)(hjR.Art)] = new HjLeistung(hjR);
          }

          if (schueler.hatVorHj) // suche 11. Klassnoten
          {
            hjDT = new HjLeistungTableAdapter().GetDataBySchuelerAndFach(schueler.Id, getFach.Id).Where(x => x.JgStufe == 11 && x.Art < 2);
            foreach (var hjR in hjDT)
            {
              vorHjLeistung[(int)(hjR.Art)] = new HjLeistung(hjR);
            }
          }
        }

        /// <summary>
        /// Liefert alle Noten eines Schülers in einem Fach von diesem Typ
        /// </summary>
        public IList<int> getNoten(Halbjahr hj,Notentyp typ)
        {          
            return noten[(int)hj,(int)typ]; // klappt der Cast immer???
        }

        /// <summary>
        /// Liefert die Notenschnitte
        /// </summary>
        public BerechneteNote getSchnitt(Halbjahr hj)
        {
            var s = schnitte[(int)hj];
            if (s==null) return new BerechneteNote(kursId,schueler.Id); // gibt leere Berechnungstabelle zurück
            return s;
        }

        /// <summary>
        /// Liefert die Halbjahresleistungen
        /// </summary>
        public HjLeistung getHjLeistung(HjArt art)
        {
          return hjLeistung[(int)art];
        }

    public HjLeistung getVorHjLeistung(HjArt art)
    {
      return vorHjLeistung[(int)art];
    }

    /// <summary>
    /// Berechnet das Gesamtergebnis aufgrund der vorliegenden HjLeistungen neu (nur Abiturklassen)
    /// </summary>
    public void calcGesErg()
    {
      HjLeistung gesErg = getHjLeistung(HjArt.GesErg);

      // 11. und Vk: Gesamtergebnis besteht aus den 2 Hj-Leistungen
      if ((int)schueler.getKlasse.Jahrgangsstufe >= 12) 
      { /*
        if (getHjLeistung(HjArt.Hj1) != null && getHjLeistung(HjArt.Hj2) != null)
        {
          if (gesErg==null)
          {
            gesErg = new HjLeistung(schueler.Id, fach, HjArt.GesErg);
          }
          gesErg.Punkte2Dez = (decimal)((getHjLeistung(HjArt.Hj1).Punkte + getHjLeistung(HjArt.Hj2).Punkte) / 2.0);
          gesErg.Punkte = (byte)Math.Round((getHjLeistung(HjArt.Hj1).Punkte + getHjLeistung(HjArt.Hj2).Punkte) / 2.0, MidpointRounding.AwayFromZero);
          

          gesErg.WriteToDB();
        }
        else
          gesErg.Delete(); // ggf. nicht mehr gültig
          */
      }
    }
    
    /// <summary>
    /// Liefert die zur Zeit z (z.B. Probezeit BOS) relevante Note (hier Jahresfortgang Ganzz. 1. Hj.)
    /// </summary>
    public byte? getRelevanteNote(Zeitpunkt z)
    {
      if (!schueler.AlteFOBOSO()) // neue FOBOSO:
      {
        HjLeistung hj;
        if (z <= Zeitpunkt.HalbjahrUndProbezeitFOS) hj = getHjLeistung(HjArt.Hj1);
        else if ((byte)schueler.getKlasse.Jahrgangsstufe < 12)
        {
          hj = getHjLeistung(HjArt.JN);
          if (hj == null)
          {
            hj = getHjLeistung(HjArt.Hj2); // Behelfskonstruktion, falls im 1. Hj keine Note gebildet wurde (z.B. Rücktritt in Fvk)
            NoteUngueltig = true;
          }
        }
        else hj = getHjLeistung(HjArt.GesErg);

        if (hj == null) return null;
        else
        {
          if (hj.Status == HjStatus.Ungueltig)
            NoteUngueltig = true;
          return hj.Punkte;
        }
      }

      // Alte FOBOSO:
      if (Zugriff.Instance.KursRep.Find(kursId).getLehrer == null)
      {
        // wenn der Kurs keinen Lehrer hat, handelt es sich vermutlich um eine Note aus der 11ten Klasse
        return getSchnitt(Halbjahr.Zweites).Abschlusszeugnis;
      }

      if (z == Zeitpunkt.ProbezeitBOS || z == Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        return getSchnitt(Halbjahr.Erstes).JahresfortgangGanzzahlig;
      }
      else if (z == Zeitpunkt.ErstePA || z == Zeitpunkt.Jahresende)
      {
        return getSchnitt(Halbjahr.Zweites).JahresfortgangGanzzahlig;
      }
      else // 2./3.PA
      {
        return getSchnitt(Halbjahr.Zweites).Abschlusszeugnis;
      }
    }

        public int getNotenanzahl(Halbjahr hj, Notentyp typ)
        {
            return noten[(int)hj, (int)typ].Count;
        }

        public int getNotenanzahl(Notentyp typ)
        {
            return noten[(int)Halbjahr.Erstes, (int)typ].Count + noten[(int)Halbjahr.Zweites, (int)typ].Count;
        }

        public Fach getFach
        {
            get
            {
                if (fach == null)
                {
                    Kurs k = Zugriff.Instance.KursRep.Find(kursId);
                    fach = Zugriff.Instance.FachRep.Find(k.Data.FachId);
                }
                return fach;
            }
        }

    private string NotenString(IList<int>noten, string bez="")
    {
      string s="";
      foreach (var note in noten)
      {        
        s += note + bez + "  ";
      }
      return s;
    }

    /// <summary>
    /// Liefert alle SA eines Faches als Text
    /// </summary>
    public string SA(Halbjahr hj)
    {
      return (NotenString(getNoten(hj, Notentyp.Schulaufgabe), "")).TrimEnd();
    }

    public string ToString(Halbjahr hj, Notentyp typ)
    {
      return (NotenString(getNoten(hj, typ), "")).TrimEnd();
    }

    /// <summary>
    /// Liefert alle sonstige Leistungen eines Faches als Text
    /// </summary>
    public string sL(Halbjahr hj)
    {
      string s;
      s=NotenString(getNoten(hj, Notentyp.Kurzarbeit), "K");
      s+=NotenString(getNoten(hj, Notentyp.Ex), "");
      s+=NotenString(getNoten(hj, Notentyp.EchteMuendliche), "");
      s+=NotenString(getNoten(hj, Notentyp.Fachreferat), "F"); // TODO: bald eine Hj-Leistung!
      s+=NotenString(getNoten(hj, Notentyp.Ersatzprüfung), "E"); 
      return s.TrimEnd();
    }

    /// <summary>
    /// Liefert eine druckbare Liste für alle sonstigen Leistungen
    /// </summary>
    public IList<string> sonstigeLeistungen(Halbjahr hj)
    {
      IList<string> liste = new List<string>();
      InsertNoten(liste, getNoten(hj, Notentyp.Kurzarbeit), "K");
      InsertNoten(liste, getNoten(hj, Notentyp.Ex), "");
      InsertNoten(liste, getNoten(hj, Notentyp.EchteMuendliche), "");
      InsertNoten(liste, getNoten(hj, Notentyp.Fachreferat), "F");
      InsertNoten(liste, getNoten(hj, Notentyp.Ersatzprüfung), "E"); 
      return liste;           
    }

    private void InsertNoten(IList<string> liste, IList<int>noten, string bez="")
    {
      foreach (var note in noten)
      {
        liste.Add(note + (bez=="" ? "" : /*" " + */ bez));                
      }        
    }

    public string NotwendigeNoteInMAP(int Zielpunkte)
    {
      int? zeugnis = getSchnitt(Halbjahr.Zweites).Abschlusszeugnis;  
      if (!zeugnis.HasValue || zeugnis>=Zielpunkte) return ""; // Ziel schon erreicht

      string kuerzel = getFach.Kuerzel;
      decimal? jf = getSchnitt(Halbjahr.Zweites).JahresfortgangMitKomma;
      if (!jf.HasValue || kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Sw" || kuerzel == "Sm" || kuerzel == "Ku") return ""; // in Nebenfächern wie G,TZ,... gibt es keine MAP

      int map;
      if (!getFach.IstSAPFach(schueler.Zweig)) // Nebenfächer
      {
        map = (int)Math.Ceiling((decimal)(6.99)-jf.GetValueOrDefault());
        return map.ToString();
      }

      // Prüfungsfächer
      if (getNoten(Halbjahr.Zweites,Notentyp.APSchriftlich).Count==0) return "";
      int sap = getNoten(Halbjahr.Zweites,Notentyp.APSchriftlich)[0];
       map=sap+1; // Mündliche muss größer als schriftliche sein, sonst bringt es nichts.
      while (map<=15 && Notentools.BerechneZeugnisnote(jf,sap,map)<Zielpunkte)
        map++;

      if (map>15) return "nicht möglich";
      else return map.ToString();
    }     
  }

}
