using BrightIdeasSoftware;
using diNo.diNoDataSetTableAdapters;
using diNo.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace diNo
{
  /// <summary>
  /// Ein Schüler.
  /// </summary>
  public class Schueler : IRepositoryObject
  {
    private diNoDataSet.SchuelerRow data;   // nimmt SchülerRecordset auf
    private Klasse klasse;                  // Objektverweis zur Klasse dieses Schülers
    private List<Kurs> kurse; // Recordset-Menge aller Kurse dieses Schülers
    private SchuelerNoten noten;            // verwaltet alle Noten dieses Schülers
    private IList<Vorkommnis> vorkommnisse; // verwaltet alle Vorkommnisse für diesen Schüler    
    private diNoDataSet.FpaDataTable fpaDT; // wird zum Speichern benötigt: FPA-Halbjahr 1 und 2
    private diNoDataSet.SeminarfachnoteRow seminar;
    private diNoDataSet.SeminarfachnoteDataTable seminarDT;
    public Zweig Zweig;
    public Punktesumme punktesumme;
    public List<string> absenzen;

    public Schueler(int id)
    {
      this.Id = id;
      this.Refresh();
      punktesumme = new Punktesumme(this);
      absenzen = new List<string>();
    }

    public Schueler(diNoDataSet.SchuelerRow s)
    {
      this.Id = s.Id;
      this.data = s;
      punktesumme = new Punktesumme(this);
      absenzen = new List<string>();
      Zweig = Faecherkanon.GetZweig(data.Ausbildungsrichtung);
    }

    // statische Methode für das Repository
    public static Schueler CreateSchueler(int id)
    {
      return new Schueler(id);
    }

    /// <summary>
    /// Hole alle Daten von Neuem aus der Datenbank.
    /// </summary>
    public void Refresh()
    {
      var rst = new SchuelerTableAdapter().GetDataById(this.Id);
      if (rst.Count == 1)
      {
        this.data = rst[0];
      }
      else
      {
        throw new InvalidOperationException("Konstruktor Schueler: Ungültige ID=" + Id);
      }

      klasse = null;
      kurse = null;
      noten = null;
      vorkommnisse = null;
      punktesumme = new Punktesumme(this);
      Zweig = Faecherkanon.GetZweig(data.Ausbildungsrichtung);
    }

    /// <summary>
    /// Speichert den Schüler zurück in die DB
    /// </summary>
    public void Save()
    {
      (new SchuelerTableAdapter()).Update(data);
      /* erledigt FPA-Klasse
      if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && fpaDT != null)
      {
        (new FpaTableAdapter()).Update(fpaDT);
      }*/
      if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn && seminarDT != null)
      {
        (new SeminarfachnoteTableAdapter()).Update(seminarDT);
      }

    }


    /// <summary>
    /// Die Id des Schülers in der Datenbank.
    /// </summary>
    [OLVColumn(Title = "Id", Width = 50, DisplayIndex = 4, TextAlign = HorizontalAlignment.Right)]
    public int Id
    {
      get;
      internal set;
    }

    public int GetId()
    {
      return Id;
    }

    public string Comparer()
    {
      return NameVorname; // wird nicht verwendet (nur fürs Interface)
    }

    [OLVColumn(Title = "Rufname", Width = 100, DisplayIndex = 3)]
    public string benutzterVorname
    {
      get { return string.IsNullOrEmpty(data.Rufname) ? data.Vorname : data.Rufname; }
    }

    /// <summary>
    /// Name und Rufname des Schülers, durch ", " getrennt.
    /// </summary>
    public string NameVorname
    {
      get
      {
        return this.Data.Name + ", " + benutzterVorname;
      }
    }

    public string VornameName
    {
      get
      {
        return benutzterVorname + " " + Data.Name;
      }
    }
    // Klassenbezeichnung des Schülers, ggf. bei Mischklassen um den Zweig ergänzt.
    public string KlassenBezeichnung
    {
      get
      {
        var k = getKlasse;
        if (k.Zweig == Zweig.None)
          return k.Bezeichnung + ((k.Zweig == Zweig.None && data.Ausbildungsrichtung != "V") ? "_" + data.Ausbildungsrichtung : "");
        else if (k.Bezeichnung.Substring(0, 2) == "FB")
          return k.Bezeichnung + "_" + Data.Schulart;
        else return k.Bezeichnung;
      }
    }

    public string KlasseName
    {
      get
      {
        return getKlasse.Bezeichnung + ", " + Data.Name + ", " + Data.Rufname;
      }
    }


    [OLVColumn(Title = "Name", Width = 100, DisplayIndex = 1)]
    public string Name
    {
      get
      {
        return this.Data.Name;
      }
    }

    [OLVColumn(Title = "Vorname", Width = 100, DisplayIndex = 2)]
    public string Vorname
    {
      get
      {
        return this.Data.Vorname;
      }
    }

    /// <summary>
    /// Ob der Schüler Notenschutz hat
    /// </summary>
    [OLVColumn(Title = "Legasthenie", Width = 80)]
    public bool HatNachteilsausgleich
    {
      get { return this.data.LRSStoerung || data.LRSZuschlagMax>0; }      
    }

    public String getNTAText
    {
       get
       {
          return (Data.LRSStoerung ? "Notenschutz" : "")
            + (Data.LRSStoerung && Data.LRSZuschlagMax > 0 ? ", " : "")
            + (Data.LRSZuschlagMax > 0 ? "Zeitzuschlag von " + Data.LRSZuschlagMin + "% bis " + Data.LRSZuschlagMax + "%" : "");
        }
    }
    /// <summary>
    /// Die Klassenbezeichnung 
    /// </summary>
    public Klasse getKlasse
    {
      get
      {
        if (klasse == null)
        {
          klasse = Zugriff.Instance.KlassenRep.Find(data.KlasseId);
        }

        return klasse;
      }
      set
      {
        klasse = value;
        if (klasse.Data.Id != data.KlasseId)
          throw new InvalidOperationException("Klasse passt nicht zur KlassenID des Schülers.");
      }
    }

    public bool hatVorHj
    { get { return (!Data.SonderfallNur2Hj) && (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf) && (Data.Schulart == "F"); }
}

public int APFaktor
    { get { return (hatVorHj ? 3 : 2); } }

    /// <summary>
    /// FPA-Noten
    /// </summary>
    public diNoDataSet.FpaDataTable FPANoten
    {
      get
      {
        if (fpaDT == null)
        {
          fpaDT = (new FpaTableAdapter()).GetDataBySchuelerId(Id);
          while (fpaDT.Count < 2) // es werden intern 2 Halbjahre angelegt
          {
            var fpa = fpaDT.NewFpaRow();
            fpa.SchuelerId = Id;
            fpa.Halbjahr = (byte)fpaDT.Count;
            fpaDT.AddFpaRow(fpa);
          }
        }
        return fpaDT;
      }
    }

    public List<FPADruck> FPANotenDruck()
    {
      List<FPADruck> res = new List<FPADruck>();
      if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
      {
        res.Add(new FPADruck(FPANoten[0], "1"));
        res.Add(new FPADruck(FPANoten[1], "2"));
      }
      return res;
    }



    public diNoDataSet.SeminarfachnoteRow Seminarfachnote
    {
      get
      {
        if (seminar == null)
        {
          seminarDT = (new SeminarfachnoteTableAdapter()).GetDataBySchuelerId(Id);
          if (seminarDT.Count == 0)
          {
            seminar = seminarDT.NewSeminarfachnoteRow();
            seminar.SchuelerId = Id;
            seminarDT.AddSeminarfachnoteRow(seminar);
          }
          else seminar = seminarDT[0];
        }
        return seminar;
      }
    }

    public bool Wiederholt()
    {
      bool wh = false;
      if (!Data.IsWiederholung1JahrgangsstufeNull())
      {
        wh = getKlasse.Jahrgangsstufe == Faecherkanon.GetJahrgangsstufe(Data.Wiederholung1Jahrgangsstufe);
      }

      if (!wh && !Data.IsWiederholung2JahrgangsstufeNull())
      {
        wh = getKlasse.Jahrgangsstufe == Faecherkanon.GetJahrgangsstufe(Data.Wiederholung2Jahrgangsstufe);
      }

      return wh;
    }


    /// <summary>
    /// Liefert entweder
    /// K falls der Schüler in kath. Religionslehre geht
    /// Ev falls evangelisch
    /// Eth falls der Schüler in Ethik geht
    /// Leerstring falls gar keine Zuordnung
    /// </summary>
    [OLVColumn(Title = "ReliOderEthik", Width = 100)]
    public string ReliOderEthik
    {
      get
      {
        return this.Data.IsReligionOderEthikNull() ? "" : this.Data.ReligionOderEthik;
      }

      set
      {
        if (!this.Data.IsReligionOderEthikNull())
          MeldeAb(this.GetFachKuerzel(this.Data.ReligionOderEthik));
        if (!string.IsNullOrEmpty(value))
        {
          MeldeAn(this.GetFachKuerzel(value));
        }

        this.Data.ReligionOderEthik = value;
        Save();
      }
    }

    /// <summary>
    /// vorliegende Methode wird benötigt, weil aus irgendwelchen Gründen z. B. in der Spalte ReliOderEthik "RK" stehen muss
    /// das korrekte Fachkürzel (laut Fächerliste) für kath. Religionslehre einfach "K" lautet
    /// </summary>
    /// <param name="aKuerzel">Ein Fachkürzel aus der Spalte ReliOderEthik</param>
    /// <returns>Ein korrektes Fachkürzel für die Datenbank</returns>
    private string GetFachKuerzel(string aKuerzel)
    {
      switch (aKuerzel)
      {
        case "RK": return "K";
        case "EV": return "Ev";
        case "Eth": return "Eth";
        case "": return "";
        default: throw new InvalidOperationException("ungültiger Wert für ReliOderEthik: " + aKuerzel);
      }
    }

    public DateTime? EintrittAm
    {
      get
      {
        return this.Data.IsEintrittAmNull() ? null : (DateTime?)this.Data.EintrittAm;
      }
    }

    public string EintrittInJahrgangsstufe
    {
      get
      {
        return this.Data.EintrittJahrgangsstufe;
      }
    }

    public string EintrittAusSchulname
    {
      get
      {
        return SchulnummernHolder.GetSchulname(this.Data.EintrittAusSchulnummer);
      }
    }

    /// <summary>
    /// Alle Noten (je Fach/Kurs) dieses Schülers
    /// </summary>
    public SchuelerNoten getNoten
    {
      get
      {
        if (noten == null)
        {
          noten = new SchuelerNoten(this);
        }
        return noten;
      }
    }

    public void ReloadNoten()
    {
      noten = null;
    }

    public List<HjLeistung> Fachreferat
    {
      get { return getNoten.Fachreferat; }
    }

    public Schuelerstatus Status
    {
      get { return (Schuelerstatus)data.Status; }
      set { data.Status = (int)value; }
    }

    public string getWiederholungen()
    {
      string result = string.Empty;

      if (!this.Data.IsWiederholung1JahrgangsstufeNull() && isAWiederholung(this.Data.Wiederholung1Jahrgangsstufe))
      {
        result += Data.Wiederholung1Jahrgangsstufe;
        result += " (" + Data.Wiederholung1Grund + ")";
      }
      if (!this.Data.IsWiederholung2JahrgangsstufeNull() && isAWiederholung(this.Data.Wiederholung2Jahrgangsstufe))
      {
        result += ", " + Data.Wiederholung2Jahrgangsstufe;
        result += " (" + Data.Wiederholung2Grund + ")";
      }

      return result;
    }

    private bool isAWiederholung(string aWiederholungsEintrag)
    {
      int zahl;
      if (int.TryParse(aWiederholungsEintrag, out zahl))
      {
        return (zahl != 0);
      }

      return false;
    }

    public diNoDataSet.SchuelerRow Data
    {
      get { return this.data; }
    }

    public List<Kurs> Kurse
    {
      get
      {
        if (kurse == null)
        {
          kurse = new List<Kurs>();
          var kursDT = new KursTableAdapter().GetDataBySchulerId(this.Id);
          foreach (var k in kursDT)
            kurse.Add(Zugriff.Instance.KursRep.Find(k.Id));
        }

        return kurse;
      }
    }

    public bool BesuchtKurs(int id)
    {
      foreach (var k in Kurse)
        if (k.Id == id) return true;

      return false;
    }

    public void RemoveVorkommnis(int vorkommnisId)
    {
      (new VorkommnisTableAdapter()).Delete(vorkommnisId);
      this.vorkommnisse = null; // damit er die neu lädt
    }

    public void AddVorkommnis(Vorkommnisart art, string bemerkung, bool DuplikateErlaubt = false)
    {
      DateTime datum = DateTime.Today;
      if (art == Vorkommnisart.MittlereReife) datum = Zugriff.Instance.Zeugnisdatum; // ggf. um andere Zeugnisarten ergänzen
      AddVorkommnis(art, datum, bemerkung, DuplikateErlaubt);
    }

    public void AddVorkommnis(Vorkommnisart art, DateTime datum, string bemerkung, bool DuplikateErlaubt = false)
    {
      if (DuplikateErlaubt || !hatVorkommnis(art))
      {
        new VorkommnisTableAdapter().Insert(datum, bemerkung, this.Id, (int)art);

        if (art == Vorkommnisart.ProbezeitNichtBestanden)
        {
          if ((Zeitpunkt)(Zugriff.Instance.aktZeitpunkt) == Zeitpunkt.HalbjahrUndProbezeitFOS)
          {
            var ta = new VorkommnisTableAdapter();
            ta.DeleteForPZBySchuelerId(Id); // löscht Zwischenzeugnis und Gefährdungsmitteilungen
            ta.UpdateGefahrAbweisung(Id); // wird "darf nicht mehr wiederholen"
          }
          if (MessageBox.Show("Soll der Schüler aus allen Kursen abgemeldet werden?", "diNo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            Austritt(Data.ProbezeitBis);
        }


        this.vorkommnisse = null; // damit er die neu lädt
      }
    }

    public IList<Vorkommnis> Vorkommnisse
    {
      get
      {
        if (this.vorkommnisse == null)
        {
          this.vorkommnisse = new List<Vorkommnis>();
          foreach (var vorkommnis in new VorkommnisTableAdapter().GetDataBySchuelerId(this.Id))
          {
            this.vorkommnisse.Add(new Vorkommnis(vorkommnis));
          }
        }

        return this.vorkommnisse;
      }
    }

    // gibt an, ob der Schüler das übergebene Vorkommnis bereits gespeichert hat (z.B. um Duplikate zu vermeiden)
    public bool hatVorkommnis(Vorkommnisart art)
    {
      foreach (var v in Vorkommnisse)
      {
        if (v.Art == art) return true;
      }
      return false;
    }

    public Vorkommnis getVorkommnis(Vorkommnisart art){
      foreach (var v in Vorkommnisse)
      {
        if (v.Art == art) return v;
      }
      return null;
    }

    // nur für 13. Klasse: hat erfolgreich die 2. FS besucht
    public bool HatZweiteFremdsprache()
    {
      foreach (var f in getNoten.alleSprachen)
      {
        if (f.getFach.getKursniveau() == Kursniveau.Englisch) continue;
        if (Fremdsprachen.HjToSprachniveau(f) >= Sprachniveau.B1) return true;
      }

      return false;
    }

    public Vorkommnisart Zeugnisart(Zeitpunkt zeitpunkt)
    {
      if (zeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS)
        return Vorkommnisart.Zwischenzeugnis;

      else if (zeitpunkt == Zeitpunkt.DrittePA && getKlasse.Jahrgangsstufe >= Jahrgangsstufe.Zwoelf)
      {
        if (getNoten.HatNichtBestanden() || hatVorkommnis(Vorkommnisart.PruefungNichtBestanden))
          return Vorkommnisart.Jahreszeugnis;
        else if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
          return Vorkommnisart.Fachabiturzeugnis;
        else if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          return (HatZweiteFremdsprache() ? Vorkommnisart.allgemeineHochschulreife : Vorkommnisart.fachgebundeneHochschulreife);
      }
      else if (zeitpunkt == Zeitpunkt.Jahresende && getKlasse.Jahrgangsstufe < Jahrgangsstufe.Zwoelf)
        return Vorkommnisart.Jahreszeugnis;

      return Vorkommnisart.NotSet;
    }



    /// <summary>
    /// Methode für den Klassenwechsel ohne Notenmitnahme.
    /// </summary>
    /// <param name="nachKlasse"></param>
    public void WechsleKlasse(Klasse nachKlasse)
    {
      // melde den Schüler aus allen Kursen ab.
      foreach (var kurs in this.Kurse)
      {
        MeldeAb(kurs);
      }

      data.KlasseId = nachKlasse.Data.Id;
      getKlasse = nachKlasse;
      Save();

      var kurse = AlleNotwendigenKurse();
      foreach (var kurs in kurse)
      {
        MeldeAn(kurs);
      }

      this.Refresh();
    }

    /// <summary>
    /// Austritt eines Schülers. Das Feld Austrittsdatum wird gesetzt und der Schüler aus allen Kursen abgemeldet.
    /// </summary>
    /// <param name="when">Wann der Schüler ausgetreten ist.</param>
    public void Austritt(DateTime when)
    {
      Status = Schuelerstatus.Abgemeldet;
      data.Austrittsdatum = when;
      Save();
      //if (Wiederholt())  
      //  AddVorkommnis(Vorkommnisart.DarfNichtMehrWiederholen, ""); // wird aus Gefahr der Abweisung generiert.
    }

    /// <summary>
    /// Wechselt einen Schüler aus einem Kurs in einen anderen.
    /// Konkret: Aus dem Von-Kurs-Fachkürzel wird er rausgeworfen und im Nach-Kurs-Fachkürzel angemeldet.
    /// meist: K für katholisch, Ev für Evangelisch, Eth für Ethik, F für Französisch, F-Wi für Französisch (fortgeführt)
    /// </summary>
    /// <param name="von"></param>
    /// <param name="nach">K für katholisch, Ev für Evangelisch, Eth für Ethik</param>
    public void WechsleKurse(string von, string nach)
    {
      MeldeAb(von);
      MeldeAn(nach);
    }

    public void MeldeAb(string vonFachKuerzel)
    {
      foreach (var kurs in this.Kurse)
      {
        if (kurs.getFach.Kuerzel == vonFachKuerzel)
        {
          MeldeAb(kurs);
        }
      }
    }

    public void MeldeAb(Kurs vonKurs)
    {
      new SchuelerKursTableAdapter().Delete(this.Id, vonKurs.Id);
      this.Refresh();
    }

    public void MeldeAn(Kurs beiKurs)
    {
      SchuelerKursTableAdapter skAda = new SchuelerKursTableAdapter();
      if (skAda.GetCountBySchuelerAndKurs(this.Id, beiKurs.Id) == 0)
      {
        new SchuelerKursTableAdapter().Insert(this.Id, beiKurs.Id);
        this.Refresh();
      }
    }

    public void MeldeAn(string nachFachKuerzel)
    {
      FachTableAdapter ada = new FachTableAdapter();
      foreach (var kurs in AlleMoeglichenKurse())
      {
        var fach = kurs.getFach;
        if (fach.Kuerzel == nachFachKuerzel)
        {
          MeldeAn(kurs);
        }
      }
    }

    // prüft, ob der übergebene Kurs ein potenzielle Kandidat für diesen Schüler ist
    // berücksichtigt dabei den gewählten Zweig bei Mischklassen
    public bool KursPasstZumZweig(Kurs k)
    {
      string kuerzel = k.getFach.Kuerzel;
      if (k.Data.IsZweigNull()) return true;
      if (k.Data.Zweig == "F" || k.Data.Zweig == "B") // Zweckentfremdung für BOS oder FOS-Kurs bei FB-Mischklassen
      {
        if (Data.IsSchulartNull()) return true;
        return k.Data.Zweig == Data.Schulart;
      }
      else
      {
        return k.Data.Zweig.Contains(Data.Ausbildungsrichtung);
      }
    }

    // prüft, ob der übergebene Kurs ein potenzielle Kandidat für diesen Schüler ist
    // berücksichtigt dabei den gewählten Zweig bei Mischklassen, Reliunterricht, Französisch
    // eignet sich für die Vorbelegung von Kursen beim Schülerimport
    public bool KursPasstZumSchueler(Kurs k)
    {
      bool result = this.KursPasstOhneGeschlechtspruefung(k);
      if (result)
      {
        // entweder ist der Kurs für alle Geschlechter oder das Geschlecht passt
        return k.Geschlecht == null || k.Geschlecht == this.Data.Geschlecht;
      }
      else
      {
        return false;
      }
    }

    private bool KursPasstOhneGeschlechtspruefung(Kurs k)
    {
      string kuerzel = k.getFach.Kuerzel;
      string reli = getReliKuerzel();
      
      if (kuerzel == "K" || kuerzel == "Ev" || kuerzel == "Eth") return (kuerzel == reli);
      /* obsolet mit WPF
      else if (kuerzel == "F") return !Data.IsFremdsprache2Null() && (kuerzel == Data.Fremdsprache2);
      else if (kuerzel == "F-Wi" && Data.Wahlpflichtfach == "F-Wi") return true;
      // die Wirtschaftler gehen in Wirtschaftsinformatik, sofern sie nicht franz. fortgeführt als wahlpflichtfach haben
      else if (kuerzel == "WIn" && Zweig == Zweig.Wirtschaft && (Data.IsWahlpflichtfachNull() || Data.Wahlpflichtfach != "F-Wi")) return true; // Standardfall (oft unbelegt)
      // alle anderen Zweige müssen den Franz. oder WInf Kurs schon wählen, damit sie reingehen
      else if (kuerzel == "F-Wi" || kuerzel == "WIn") return (kuerzel == Data.Wahlpflichtfach);
      */
      else return KursPasstZumZweig(k);
    }


    // wandelt das beim Schüler gespeicherte Bekenntnis in das Fachkürzel um
    public string getReliKuerzel()
    {
      // manche belegen einen anderen Reliunterricht als das zugehörige Bekenntnis:
      string unt = Data.IsReligionOderEthikNull() ? Data.Bekenntnis : Data.ReligionOderEthik;

      if (unt == "RK") return "K";
      else if (unt == "EV") return "Ev";
      else return "Eth";
    }

    public IList<Kurs> AlleMoeglichenKurse()
    {
      var result = new List<Kurs>();
      foreach (Kurs k in getKlasse.Kurse)
        if (KursPasstZumZweig(k))
          result.Add(k);

      return result;
    }

    // liefert alle Kurse, die der Schüler besuchen sollte (je nach Einstellungen in seinen Wahlpflichtfächern)    
    public IList<Kurs> AlleNotwendigenKurse()
    {
      var result = new List<Kurs>();
      foreach (Kurs k in getKlasse.Kurse)
        if (KursPasstZumSchueler(k) && k.getFach.Typ != FachTyp.WPF)
          result.Add(k);

      return result;
    }

    // wenn dem Schüler ein neuer Kurs hinzugefügt wird, z.B. kath. Religion, dann wird automatisch das
    // Feld ReligionOderEthik angepasst
    public void PasseWahlfachschluesselAn(Kurs k)
    {
      string kuerzel = k.getFach.Kuerzel;
      if (kuerzel == "K") Data.ReligionOderEthik = "RK";
      else if (kuerzel == "Ev") Data.ReligionOderEthik = "EV";
      else if (kuerzel == "Eth") Data.ReligionOderEthik = "Eth";
    }


    // Liefert den Zeitpunkt des PZ-Endes (bezogen auf das laufende Schuljahr)
    public Zeitpunkt HatProbezeitBis()
    {
      if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
      {
        ;
      }
      if (!data.IsProbezeitBisNull())
      {
        // PZ im Dezember = BOS
        if (data.ProbezeitBis > DateTime.Parse("1.12." + Zugriff.Instance.Schuljahr)
            && data.ProbezeitBis < DateTime.Parse("20.12." + Zugriff.Instance.Schuljahr))
          return Zeitpunkt.ProbezeitBOS;

        // PZ im Februar = FOS
        if (data.ProbezeitBis > DateTime.Parse("1.2." + (Zugriff.Instance.Schuljahr + 1))
            && data.ProbezeitBis < DateTime.Parse("1.3." + (Zugriff.Instance.Schuljahr + 1)))
          return Zeitpunkt.HalbjahrUndProbezeitFOS;
      }
      return Zeitpunkt.None;
    }

    public int GetAnzahlEinbringung()
    {
      if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        return 16;
      }

      if (!hatVorHj)
      {
        return 17;
      }

      return 25;
    }

    public ZweiteFSArt getZweiteFSArt()
    {
      return (ZweiteFSArt)(getNoten.ZweiteFSalt != null ? 2 : Data.AndereFremdspr2Art);
    }

    public int Alter()
    {
      return Alter(DateTime.Now);
    }

    public int Alter(DateTime datum)
    {
      int jahre = datum.Year - data.Geburtsdatum.Year;
      var dieserGebTag = data.Geburtsdatum.AddYears(jahre);
      if (datum.CompareTo(dieserGebTag) < 0) { jahre--; }
      return jahre;
    }

    public string getErSie(bool Satzanfang = false)
    {
      if (Satzanfang) return (Data.Geschlecht == "M" ? "Er" : "Sie");
      else return (Data.Geschlecht == "M" ? "er" : "sie");
    }

    public string getHerrFrau()
    {
      return (Data.Geschlecht == "M" ? "Herr" : "Frau");
    }

    public string getHerrnFrau()
    {
      return getHerrnFrau(Data.Geschlecht);
    }

    private string getHerrnFrau(string k)
    {
      switch (k)
      {
        case "M":
        case "H": return "Herrn ";
        case "W":
        case "F": return "Frau ";
        case "U": return "Herrn und Frau ";
        default: return "";
      }
    }

    private string erzAnr(string anrede, string nachname)
    {
      if (anrede == "M" || anrede == "H")
        return "Sehr geehrter Herr " + nachname + ",<br>";
      else
        return "Sehr geehrte Frau " + nachname + ",<br>";
    }

    public string ErzeugeAnrede(bool ElternadresseVerwenden)
    {
      if (ElternadresseVerwenden)
      {
        string s = "";
        if (Data.AnredeEltern1 != "") s = erzAnr(Data.AnredeEltern1, data.NachnameEltern1);
        if (Data.AnredeEltern2 != "") s += erzAnr(Data.AnredeEltern2, data.NachnameEltern2);
        s += "<br>";
        return s;
      }
      else
      {
        return erzAnr(Data.Geschlecht, Data.Name) + "<br>";
      }
    }

    // Adresse von Minderjährigen mit den Eltern
    public string ErzeugeAdresse(bool ElternadresseVerwenden)
    {
      string s = "";
      if (ElternadresseVerwenden)
      {
        // wenn beide Eltern getrennt gespeichert sind, muss die Anrede in dieselbe Zeile, sonst extra:
        s = getHerrnFrau(Data.AnredeEltern1) + (Data.AnredeEltern2 == "" ? "\n" : "") + Data.VornameEltern1 + " " + Data.NachnameEltern1 + "\n";
        if (Data.AnredeEltern2 != "")
          s += getHerrnFrau(Data.AnredeEltern2) + Data.VornameEltern2 + " " + Data.NachnameEltern2 + "\n";
      }
      else
        s = getHerrnFrau(Data.Geschlecht) + "\n" + VornameName + "\n";

      s += Data.AnschriftStrasse + "\n";
      s += Data.AnschriftPLZ + " " + Data.AnschriftOrt;
      return s;
    }

    public string getLoginname()
    {      
        string s = Data.MailSchule;
        int i = s.IndexOf("@");
        if (i < 1) return s;
        return s.Substring(0,i);      
    }    
  }

  public static class SchulnummernHolder
  {
    private static Dictionary<int, string> schulenInBayern = ReadFromResource();

    private static Dictionary<int, string> ReadFromResource()
    {
      Dictionary<int, string> result = new Dictionary<int, string>();
      foreach (string line in Resources.ListeAllerSchulenInBayern.Split('\n'))
      {
        string[] array = line.Split(';');
        int schulnummer = int.Parse(array[0]);
        string name = array[2].Trim();

        if (!result.ContainsKey(schulnummer))
        {
          result.Add(schulnummer, name);
        }
      }

      return result;
    }

    public static string GetSchulname(int schulnummer)
    {      
      bool b = schulenInBayern.ContainsKey(schulnummer);
      return b ? schulenInBayern[schulnummer] : "";      
    }
  }

  public enum Schuelerstatus
  {
    Aktiv = 0,
    Abgemeldet = 1
  }

}