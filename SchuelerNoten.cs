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
        private Zeitpunkt zeitpunkt=Zeitpunkt.None;
        public string Unterpunktungen;
        public bool hatDeutsch6 = false; // kann nicht ausgeglichen werden
        public int anz4P=0;

        public SchuelerNoten(Schueler s)
        {
          schueler = s;
          diNoDataSet.KursDataTable kurse = schueler.Kurse; // ermittle alle Kurse, die der S besucht
          alleFaecher = new List<FachSchuelerNoten>();
          alleKurse = new List<FachSchuelerNoten>();
          foreach (var kurs in kurse)
          {
            var fsn = new FachSchuelerNoten(schueler.Id, kurs.Id);
            alleFaecher.Add(fsn);
            alleKurse.Add(fsn);
          }

          // alle Fächer des Schülers ohne Kurs finden und diese HjLeistungen laden 
          diNoDataSet.FachDataTable fDT = (new FachTableAdapter()).GetFaecherOhneKurseBySchuelerId(schueler.Id);
          foreach (var fachR in fDT)
          {
            Fach fach = Zugriff.Instance.FachRep.Find(fachR.Id);
            alleFaecher.Add(new FachSchuelerNoten(schueler.Id, fach));
          }

          alleFaecher.Sort((x,y) => x.getFach.Sortierung.CompareTo(y.getFach.Sortierung));       
          alleKurse.Sort((x,y) => x.getFach.Sortierung.CompareTo(y.getFach.Sortierung));       
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

    /// <summary>
    /// Liefert eine Liste in der je Fach alle Noten in druckbarer Form vorliegen.
    /// </summary>
    public IList<FachSchuelerNotenDruckKurz> SchuelerNotenDruck(string rptName)
    {
      IList<FachSchuelerNotenDruckKurz> liste = new List<FachSchuelerNotenDruckKurz>();      
      foreach (FachSchuelerNoten f in alleKurse)
      {               
        liste.Add(new FachSchuelerNotenDruckKurz(f, f.getFach.IstSAFach(schueler.Zweig, schueler.getKlasse.Jahrgangsstufe),rptName));
      }
      if (schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Dreizehn)
      {
        if (!schueler.Data.IsAndereFremdspr2NoteNull())
        {
          liste.Add(new FachSchuelerNotenDruckKurz("Andere 2. Fremdsprache" + 
            (schueler.Data.IsAndereFremdspr2TextNull() ? "" : " (" + schueler.Data.AndereFremdspr2Text + ")"),
            schueler.Data.AndereFremdspr2Note));
        }

        liste.Add(new FachSchuelerNotenDruckKurz(schueler.Seminarfachnote));
      }

      // FPA ausgeben für Notenmitteilung (im Notenbogen als Bemerkung)
      if (rptName=="diNo.rptNotenmitteilungA5.rdlc" && schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Elf)
      {
        liste.Add(new FachSchuelerNotenDruckKurz(schueler.FPANoten));
      }
      return liste;
    }
    
    // TODO: Überflüssig, wegen lokaler Variable
    public string GetUnterpunktungenString(Zeitpunkt z)
    {
      string result = "";
      foreach (var fach in alleKurse)
      {
        byte? note = fach.getRelevanteNote(z);
        if (note != null && note < 4)
        {
          result += fach.getFach.Kuerzel + "(" + note + ")";
        }
      }

      return result;
    }
    
    // Zu diesem Zeitpunkt werden die Notenanzahlen gebildet,
    // wird dieser geändert, muss neu gerechnet werden
    public void SetZeitpunkt(Zeitpunkt z)
    {
      if (zeitpunkt!=z)
      {
        anzahlNoten = null;
        zeitpunkt = z;
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
      if (anzahlNoten == null)
      {
        if (zeitpunkt==Zeitpunkt.None) zeitpunkt = (Zeitpunkt)Zugriff.Instance.aktZeitpunkt;
        anzahlNoten = new int[7,2];
        InitAnzahlNoten();
      }
      if (SAP) return anzahlNoten[note,1];
      else return anzahlNoten[note,0];
    }

    private void InitAnzahlNoten()
    {      
      string kuerzel;
      Unterpunktungen="";
      foreach (var fachNoten in alleKurse)
      {
        kuerzel = fachNoten.getFach.Kuerzel;
        if (kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Ku") continue;  // keine Vorrückungsfächer
        byte? relevanteNote = fachNoten.getRelevanteNote(zeitpunkt);
        int istSAP = fachNoten.getFach.IstSAPFach() ? 1:0;
        if (relevanteNote != null)
        {                         
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
        if (relevanteNote == 0) anzahlNoten[6,0]++;          
        else if (relevanteNote < 4) anzahlNoten[5,0]++;
        else if (relevanteNote >=13) anzahlNoten[1,0]++;
        else if (relevanteNote >= 10) anzahlNoten[2,0]++;        

        if (relevanteNote <4)
          Unterpunktungen += "Sem (" + relevanteNote +") ";
      }
    }

    public bool HatNichtBestanden()
    {
      // Achtung: Vorklasse hat am Jahresende eine besondere Bestanden-Regelung
      return AnzahlNoten(6) > 0 || AnzahlNoten(5) > 1;
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
    
    // Mittlere Reife, falls nur 4er oder 1x5,1x2 oder 1x5,2x3, vgl. §58(5)
    public bool ErhaeltMittlereReife()
    {
      if (schueler.getKlasse.Jahrgangsstufe!=Jahrgangsstufe.Vorklasse 
        || schueler.getKlasse.Bezeichnung=="IV") return false;

      return (AnzahlNoten(6) == 0) && 
        (AnzahlNoten(5) == 0 ||
        (AnzahlNoten(5) == 1 && (AnzahlNoten(1) > 0 || AnzahlNoten(2) > 0 || AnzahlNoten(3) > 1)));
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
        if (kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Ku") continue;  // keine Vorrückungsfächer

        byte? note = f.getRelevanteNote(Zeitpunkt.ZweitePA);
        if (note<4)
        {          
          jf = f.getSchnitt(Halbjahr.Zweites).JahresfortgangMitKomma;
          istSAPFach = f.getFach.IstSAPFach();

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
        public int schuelerId;        
        private Fach fach=null;
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
    
        public FachSchuelerNoten(int aschuelerid, int akursid)
        {
            kursId = akursid;        
            schuelerId = aschuelerid;
            
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
        public FachSchuelerNoten(int aschuelerid, Fach aFach)
        {
          fach = aFach;
          kursId = 0;
          schuelerId = aschuelerid;
                            
          LeseNotenAusDB();
        }

        private void LeseNotenAusDB()
        {
          if (kursId>0)
          { 
            diNoDataSet.NoteDataTable notenDT;
            notenDT = new NoteTableAdapter().GetDataBySchuelerAndKurs(schuelerId, kursId);
            foreach (var noteR in notenDT)
            {
              noten[noteR.Halbjahr,noteR.Notenart].Add(noteR.Punktwert);
            }

            // Schnitte werden direkt gelesen
            diNoDataSet.BerechneteNoteDataTable bnotenDT;
            // liefert max. 2 Datensätze (einen für 1. und 2. Hj.), historische Stände werden nicht geliefert
            bnotenDT = new BerechneteNoteTableAdapter().GetDataBySchuelerAndKurs(kursId, schuelerId);
            foreach (var bnoteR in bnotenDT)
            {                
              schnitte[(int)(bnoteR.ErstesHalbjahr ? Halbjahr.Erstes : Halbjahr.Zweites)] = 
                        new BerechneteNote(kursId, schuelerId, bnoteR);
            }
          }
            // HjLeistungen
          diNoDataSet.HjLeistungDataTable hjDT;            
          hjDT = new HjLeistungTableAdapter().GetDataBySchuelerAndFach(schuelerId,getFach.Id);
          foreach (var hjR in hjDT)
          {                
            hjLeistung[(int)(hjR.Art)] = new HjLeistung(hjR);
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
            if (s==null) return new BerechneteNote(kursId,schuelerId); // gibt leere Berechnungstabelle zurück
            return s;
        }

        /// <summary>
        /// Liefert die Halbjahresleistungen
        /// </summary>
        public HjLeistung getHjLeistung(HjArt art)
        {
          return hjLeistung[(int)art];
        }

    /*
    public IList<int> getNoten(Notentyp typ)
    {
        IList<int> res = new List<int>(noten[(int)Halbjahr.Erstes, (int)typ]);
        noten[(int)Halbjahr.Zweites, (int)typ].CopyTo(res);
        return res;
    }
    */

    /// <summary>
    /// Liefert die zur Zeit z (z.B. Probezeit BOS) relevante Note (hier Jahresfortgang Ganzz. 1. Hj.)
    /// </summary>
    public byte? getRelevanteNote(Zeitpunkt z)
    {
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
        s += note + bez + " ";
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

    /// <summary>
    /// Liefert alle sonstige Leistungen eines Faches als Text
    /// </summary>
    public string sL(Halbjahr hj)
    {
      string s;
      s=NotenString(getNoten(hj, Notentyp.Kurzarbeit), "K");
      s+=NotenString(getNoten(hj, Notentyp.Ex), "");
      s+=NotenString(getNoten(hj, Notentyp.EchteMuendliche), "");
      // NotenString(getNoten(hj, Notentyp.Fachreferat), "F"); // nun ein Hj-Leistung!
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
      if (!jf.HasValue || kuerzel == "F" || kuerzel == "Smw" || kuerzel == "Ku") return ""; // in Nebenfächern wie G,TZ,... gibt es keine MAP

      int map;
      if (!getFach.IstSAPFach()) // Nebenfächer
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

      public FachSchuelerNotenDruckKurz(FachSchuelerNoten s, bool evalSA,string rptName)
      {
        fachBez = s.getFach.Bezeichnung;
        if (rptName=="diNo.rptNotenbogen.rdlc" && fachBez.Contains("irtschafts")) // Fachbezeichnung sind zu lang für Notenbogen
        {
          string kuerzel = s.getFach.Kuerzel;
          if (kuerzel == "BwR") fachBez = "Betriebswirt-schaftslehre";
          else if (kuerzel == "VWL") fachBez = "Volkswirt-schaftslehre";
          else if (kuerzel == "WIn") fachBez = "Wirtschafts-informatik";                      
        }

        Art=""; N1=""; D1=""; N2=""; D2=""; 
        var d1 = s.getSchnitt(Halbjahr.Erstes);
        var d2 = s.getSchnitt(Halbjahr.Zweites);
       
        if (rptName!="diNo.rptAbiergebnisse.rdlc")
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

        SAP = put(s.getNoten(Halbjahr.Zweites,Notentyp.APSchriftlich),0);
        MAP = put(s.getNoten(Halbjahr.Zweites,Notentyp.APMuendlich),0);
        APG = String.Format("{0:f2}",d2.PruefungGesamt);
        GesZ = String.Format("{0:f2}",d2.SchnittFortgangUndPruefung);                
        Z = d2.Abschlusszeugnis.ToString();

        if (rptName=="diNo.rptAbiergebnisse.rdlc" && MAP=="")
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
          JF2  =Z;
        }
      }

      public FachSchuelerNotenDruckKurz(diNoDataSet.FpANotenRow f)
      {
        fachBez = "Fachpraktische Ausbildung";
        if (!f.IsPunkte1HjNull())
        {
          N1 = f.Punkte1Hj.ToString() + " (" + ErfolgText(f.Erfolg1Hj) + ")";          
        }
        if (!f.IsPunkteNull())
        {
          N2 = f.Punkte2Hj.ToString() + "\nSchnitt: " + f.Punkte.ToString() + " ("+ ErfolgText(f.Erfolg) + ")";
        }
      }

      public FachSchuelerNotenDruckKurz(string fach, int note)
      {
        fachBez = fach;
        Z = note.ToString();
        JF2  =Z;
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
  /*
    public class FachSchuelerNotenDruck
    {
        FachSchuelerNoten schueler;
        
        // Arrays können in Bericht leider nicht gedruckt werden, daher einzeln:
        // für SA / sL wird je ein Datensatz erzeugt
        // JF und DGes wird nur bei sL mitgeschickt
        public string fachBez { get; private set; }
        public string Art { get; private set; } // gibt den Text SA oder sL aus
        public string N11 { get; private set; } 
        public string N12 { get; private set; } // 2. Note im 1. Hj.
        public string N13 { get; private set; }
        public string N14 { get; private set; }
        public string N15 { get; private set; }
        public string N16 { get; private set; }
        public string N21 { get; private set; }
        public string N22 { get; private set; }
        public string N23 { get; private set; }
        public string N24 { get; private set; }
        public string N25 { get; private set; }
        public string N26 { get; private set; }
        public string D1 { get; private set; } // Durchschnitt 1. Hj.
        public string DGes1 { get; private set; } // Schnitt Gesamt im 1. Hj.
        public string JF1 { get; private set; }
        public string D2 { get; private set; }
        public string DGes2 { get; private set; }
        public string JF2 { get; private set; }
        public string SAP { get; private set; }
        public string MAP { get; private set; }
        public string APG { get; private set; }
        public string GesZ { get; private set; }
        public string Z { get; private set; }
        
        public FachSchuelerNotenDruck(FachSchuelerNoten s, bool evalSA)
        {
            schueler = s;
            fachBez = s.getFach.Bezeichnung;

            var d1 = s.getSchnitt(Halbjahr.Erstes);
            var d2 = s.getSchnitt(Halbjahr.Zweites);

            IList<string> n1,n2;
            if (evalSA)
            {
                Art = "SA";
                n1 = s.SA(Halbjahr.Erstes);
                n2 = s.SA(Halbjahr.Zweites);
                D1 = String.Format("{0:f2}", d1.SchnittSchulaufgaben);
                D2 = String.Format("{0:f2}", d2.SchnittSchulaufgaben);
                DGes1 = String.Format("{0:f2}", d1.JahresfortgangMitKomma);
            }
            else
            {
                Art = "sL";
                n1 = s.sonstigeLeistungen(Halbjahr.Erstes);
                n2 = s.sonstigeLeistungen(Halbjahr.Zweites);
                D1 = String.Format("{0:f2}", d1.SchnittMuendlich);
                D2 = String.Format("{0:f2}", d2.SchnittMuendlich);
                DGes1 = String.Format("{0:f2}", d1.JahresfortgangMitKomma);
                JF1 = d1.JahresfortgangGanzzahlig.ToString();
                DGes2 = String.Format("{0:f2}", d2.JahresfortgangMitKomma);
                JF2 = d2.JahresfortgangGanzzahlig.ToString();

                SAP = put(s.getNoten(Halbjahr.Zweites,Notentyp.APSchriftlich),0);
                MAP = put(s.getNoten(Halbjahr.Zweites,Notentyp.APMuendlich),0);
                APG = String.Format("{0:f2}",d2.PruefungGesamt);
                GesZ = String.Format("{0:f2}",d2.SchnittFortgangUndPruefung);                
                Z = d2.Abschlusszeugnis.ToString();
            }                      
            checkLen(n1,6);
            N11 = put(n1,0);
            N12 = put(n1,1);
            N13 = put(n1,2);
            N14 = put(n1,3);
            N15 = put(n1,4);
            N16 = put(n1,5);

            checkLen(n2,6);
            N21 = put(n2,0);
            N22 = put(n2,1);
            N23 = put(n2,2);
            N24 = put(n2,3);
            N25 = put(n2,4);
            N26 = put(n2,5);

        }

        private string put(IList<string> n, int index)
        {
            if (index < n.Count)
                return n[index];
            else
                return "";
        }

        private string put(IList<int> n, int index)
        {
            if (index < n.Count)
                return n[index].ToString();
            else
                return "";
        }
        private void checkLen(IList<string> n, int maxindex)
        {
            if (n.Count>=maxindex)
            {
                throw new IndexOutOfRangeException("Notenbogendruck: Zuviele Noten im Fach " + schueler.getFach.Bezeichnung + " bei Schüler " + schueler.schuelerId);            
            }
        }
    }
    */
}
