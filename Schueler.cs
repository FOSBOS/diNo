using BrightIdeasSoftware;
using diNo.diNoDataSetTableAdapters;
using diNo.Properties;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;

namespace diNo
{
  /// <summary>
  /// Ein Schüler.
  /// </summary>
  public class Schueler
  {
    private diNoDataSet.SchuelerRow data;   // nimmt SchülerRecordset auf
    private Klasse klasse;                  // Objektverweis zur Klasse dieses Schülers
    private diNoDataSet.KursDataTable kurse; // Recordset-Menge aller Kurse dieses Schülers
    private SchuelerNoten noten;            // verwaltet alle Noten dieses Schülers
    private IList<Vorkommnis> vorkommnisse; // verwaltet alle Vorkommnisse für diesen Schüler
    private diNoDataSet.FpANotenRow fpa; // Recordset der FPA-Noten
    private diNoDataSet.FpANotenDataTable fpaDT; // wird zum Speichern benötigt
    private diNoDataSet.SeminarfachnoteRow seminar;
    private diNoDataSet.SeminarfachnoteDataTable seminarDT;

    public Schueler(int id)
    {
      this.Id = id;
      this.Refresh();
    }

    public Schueler(diNoDataSet.SchuelerRow s)
    {
      this.Id = s.Id;
      this.data = s;
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
        throw new InvalidOperationException("Konstruktor Schueler: Ungültige ID.");
      }

      this.klasse = null;
      this.kurse = null;
      this.noten = null;
      this.vorkommnisse = null;
    }

    /// <summary>
    /// Speichert den Schüler zurück in die DB
    /// </summary>
    public void Save()
    {
          (new SchuelerTableAdapter()).Update(data);
          if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && fpaDT != null)
            {
                (new FpANotenTableAdapter()).Update(fpaDT);
            }
          if (getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn && seminarDT != null)
            {
                (new SeminarfachnoteTableAdapter()).Update(seminarDT);
            }

    }


    /// <summary>
    /// Die Id des Schülers in der Datenbank.
    /// </summary>
    [OLVColumn(Title="Id", Width = 50, DisplayIndex = 4, TextAlign = HorizontalAlignment.Right)]
    public int Id
    {
      get;
      internal set;
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

    /// <summary>
    /// Name und Adresse des Schülers in drei Zeilen
    /// </summary>
    public string NameUndAdresse
    {
      get
      {
        return this.benutzterVorname + " " + this.Data.Name + "\n" + this.Data.AnschriftStrasse + "\n" + this.Data.AnschriftPLZ + " " + this.Data.AnschriftOrt;
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
    /// Ob der Schüler Legastheniker ist (so dass in Englisch und Französisch 1:1 gewertet werden muss).
    /// </summary>
    [OLVColumn(Title="Legasthenie", Width = 80)]
    public bool IsLegastheniker
    {
      get { return this.data.LRSStoerung; }
      set
      {
        this.data.LRSStoerung = value;
        this.data.LRSSchwaeche = value;
        Save();
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
          klasse = new Klasse(this.data.KlasseId);
        }

        return klasse;
      }
    }


    /// <summary>
    /// FPA-Noten
    /// </summary>
    public diNoDataSet.FpANotenRow FPANoten
    {
      get
      {
        if (fpa == null)
        {
            fpaDT = (new FpANotenTableAdapter()).GetDataBySchuelerId(Id);
            if (fpaDT.Count == 0)
            {
                fpa = fpaDT.NewFpANotenRow();
                fpa.SchuelerId = Id;
                fpaDT.AddFpANotenRow(fpa);
            }
            else fpa = fpaDT[0];
        }
        return fpa;
      }
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



    /// <summary>
    /// Liefert entweder
    /// F für Wahlfach Französisch
    /// F3 für fortgeführtes Französisch
    /// einen Leerstring für Schüler die gar kein Französisch haben
    /// 
    /// Achtung: Beim Setzen wird auch gleich der Kurs umgemeldet!
    /// </summary>
    [OLVColumn(Title = "Wahlpflichtfach", Width = 100)]
    public string Wahlpflichtfach
    {
      get
      {
        return this.Data.Wahlpflichtfach;
      }
      set
      {
        MeldeAb(this.Data.Wahlpflichtfach);
        MeldeAn(value);
        this.Data.Wahlpflichtfach = value;
        this.Data.AcceptChanges();
        Save();                
      }
    }

    /// <summary>
    /// Liefert oder setzt den Fremdsprache2-Eintrag.
    /// 
    /// Achtung: Beim Setzen wird auch gleich der Kurs umgemeldet!
    /// </summary>
    [OLVColumn(Title = "Fremdsprache2", Width = 100)]
    public string Fremdsprache2
    {
      get
      {
        return this.Data.Fremdsprache2;
      }
      set
      {
        MeldeAb(this.Data.Fremdsprache2);
        MeldeAn(value);
        this.Data.Fremdsprache2 = value;
        this.Data.AcceptChanges();
        Save();                
      }
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
        return this.Data.ReligionOderEthik;
      }

      set
      {
        MeldeAb(this.GetFachKuerzel(this.Data.ReligionOderEthik));
        if (!string.IsNullOrEmpty(value))
        {
          MeldeAn(this.GetFachKuerzel(value));
        }

        this.Data.ReligionOderEthik = value;
        this.Data.AcceptChanges();
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

    public Zweig Zweig
    {
      get
      {
        return Klasse.GetZweig(this.data.Ausbildungsrichtung);
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

    public string getWiederholungen()
    {
      string result = string.Empty;
                  
      if (!this.Data.IsWiederholung1JahrgangsstufeNull() && isAWiederholung(this.Data.Wiederholung1Jahrgangsstufe))
      {
        result += this.Data.Wiederholung1Jahrgangsstufe;
        result += "(" + this.Data.Wiederholung1Grund + ")";
      }
      if (!this.Data.IsWiederholung2JahrgangsstufeNull() && isAWiederholung(this.Data.Wiederholung2Jahrgangsstufe))
      {
        result += this.Data.Wiederholung2Jahrgangsstufe;
        result += "(" + this.Data.Wiederholung2Grund + ")";
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

    public diNoDataSet.KursDataTable Kurse
    {
      get
      {
        if (kurse == null)
        {
          kurse = new KursTableAdapter().GetDataBySchulerId(this.Id);
        }

        return kurse;
      }
    }

    public void RemoveVorkommnis(int vorkommnisId)
    {
      (new VorkommnisTableAdapter()).Delete(vorkommnisId);
        this.vorkommnisse = null; // damit er die neu lädt
    }

    public void AddVorkommnis(Vorkommnisart art, DateTime datum, string bemerkung)
    {
      new VorkommnisTableAdapter().Insert(datum, bemerkung, this.Id, (int)art);
      if (art == Vorkommnisart.ProbezeitNichtBestanden)
      {
        if (MessageBox.Show("Soll der Schüler aus allen Kursen abgemeldet werden?","diNo",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
          Austritt(Data.ProbezeitBis);
      }

      this.vorkommnisse = null; // damit er die neu lädt
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

    [OLVColumn(Title = "DNote", Width = 50)]
    public double DNote
    {
      get
      {
        return this.berechneDNote();
      }
    }

    public double berechneDNote()
    {
      int summe = 0, anz = 0;
      double erg;
      var faecher = new BerechneteNoteTableAdapter().GetDataBySchueler4DNote(this.Id);
      foreach (var fach in faecher)
      {
        if (true /*!fach.KursRow.FachRow.Kuerzel in ['F','Ku','Sp']*/)
        {
          if (fach.Abschlusszeugnis == 0)
          {
            summe--; // Punktwert 0 wird als -1 gezählt
          }
          else
          {
            summe += fach.Abschlusszeugnis;
          }

          anz++;
        }
      }
      if (anz > 0)
      {
        erg = (17 - summe / anz) / 3;
        if (erg < 1)
        {
          erg = 1;
        }
        else
        {
          erg = Math.Floor(erg * 10) / 10; // auf 1 NK abrunden
        }
      }
      else
      {
        erg = 0;
      }

      return erg;
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
        MeldeAb(new Kurs(kurs));
      }

      var kursSelector = UnterrichtExcelReader.GetStandardKursSelector();
      var klasse = Klasse.FindKlassenTeilMitKursen(nachKlasse.Bezeichnung, Faecherkanon.GetZweig(this.Data.Ausbildungsrichtung));
      if (klasse == null)
      {
        throw new InvalidOperationException("Für die Klasse "+nachKlasse.Bezeichnung+ " konnten keine Kurse gefunden werden");
      }

      foreach (var kurs in klasse.Kurse)
      {
        // prüfe, ob der Schüler in diesen Kurs gehen soll und trage ihn ein.
        UnterrichtExcelReader.AddSchuelerToKurs(kurs.Data, kursSelector, this.Data);
      }

      DateTime? austrittsdatum = this.Data.IsAustrittsdatumNull() ? (DateTime?)null : this.Data.Austrittsdatum;

      this.data.KlasseId = nachKlasse.Data.Id;
      this.Save();
      this.Refresh();
    }

    /// <summary>
    /// Austritt eines Schülers. Das Feld Austrittsdatum wird gesetzt und der Schüler aus allen Kursen abgemeldet.
    /// </summary>
    /// <param name="when">Wann der Schüler ausgetreten ist.</param>
    public void Austritt(DateTime when)
    {
      foreach (var kurs in this.Kurse)
      {
        MeldeAb(new Kurs(kurs));
      }

      this.kurse = null;
      this.Data.Austrittsdatum = when;
      this.Save();
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
      FachTableAdapter ada = new FachTableAdapter();
      foreach (var kurs in this.Kurse)
      {
        var fach = ada.GetDataById(kurs.FachId)[0];
        if (fach.Kuerzel == vonFachKuerzel)
        {
          MeldeAb(new Kurs(kurs));
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
      foreach (var kurs in this.getKlasse.FindeAlleMöglichenKurse(Klasse.GetZweig(this.data.Ausbildungsrichtung)))
      {
        var fach = kurs.getFach;
        if (fach.Kuerzel == nachFachKuerzel)
        {
          MeldeAn(kurs);
        }
      }
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
      return (schulenInBayern.ContainsKey(schulnummer)) ? schulenInBayern[schulnummer] : "";
    }
  }
}
