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
        (new SchuelerTableAdapter()).UpdateLRS(this.data.LRSStoerung, this.data.LRSSchwaeche, this.Id);
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
        MeldeAb(this, this.Data.Wahlpflichtfach);
        MeldeAn(this, value);
        this.Data.Wahlpflichtfach = value;
        this.Data.AcceptChanges();
        new SchuelerTableAdapter().UpdateWahlpflichtfach(value, this.Id);
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
        MeldeAb(this, this.Data.Fremdsprache2);
        MeldeAn(this, value);
        this.Data.Fremdsprache2 = value;
        this.Data.AcceptChanges();
        new SchuelerTableAdapter().UpdateFremdsprache2(value, this.Id);
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
        MeldeAb(this, this.Data.ReligionOderEthik);
        switch (value)
        {
          case "RK": MeldeAn(this, "K"); break;
          case "EV": MeldeAn(this, "Ev"); break;
          case "Eth": MeldeAn(this, "Eth"); break;
          case "": break;
          default: throw new InvalidOperationException("ungültiger Wert für ReliOderEthik: "+value);
        }

        this.Data.ReligionOderEthik = value;
        this.Data.AcceptChanges();
        new SchuelerTableAdapter().UpdateReliOderEthik(value, this.Id);
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

    public string getWiederholungen()
    {
      string result = string.Empty;
      
      if (!string.IsNullOrEmpty(this.Data.Wiederholung1Jahrgangsstufe) && isAWiederholung(this.Data.Wiederholung1Jahrgangsstufe))
      {
        result += this.Data.Wiederholung1Jahrgangsstufe;
        result += "(" + this.Data.Wiederholung1Grund + ")";
      }
      if (!string.IsNullOrEmpty(this.Data.Wiederholung2Jahrgangsstufe) && isAWiederholung(this.Data.Wiederholung2Jahrgangsstufe))
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
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="nachKlasse"></param>
    public static void WechsleKlasse(Schueler schueler, Klasse nachKlasse)
    {
      // melde den Schüler aus allen Kursen ab.
      foreach (var kurs in schueler.Kurse)
      {
        MeldeAb(schueler, new Kurs(kurs));
      }

      var kursSelector = UnterrichtExcelReader.GetStandardKursSelector();
      var klasse = Klasse.FindKlassenTeilMitKursen(nachKlasse.Bezeichnung, Faecherkanon.GetZweig(schueler.Data.Ausbildungsrichtung));
      if (klasse == null)
      {
        throw new InvalidOperationException("Für die Klasse "+nachKlasse.Bezeichnung+ " konnten keine Kurse gefunden werden");
      }

      foreach (var kurs in klasse.Kurse)
      {
        // prüfe, ob der Schüler in diesen Kurs gehen soll und trage ihn ein.
        UnterrichtExcelReader.AddSchuelerToKurs(kurs.Data, kursSelector, schueler.Data);
      }

      DateTime? austrittsdatum = schueler.Data.IsAustrittsdatumNull() ? (DateTime?)null : schueler.Data.Austrittsdatum;
      new SchuelerTableAdapter().UpdateManyThings(nachKlasse.Data.Id, schueler.Data.Fremdsprache2, schueler.Data.ReligionOderEthik, austrittsdatum, schueler.Data.LRSStoerung, schueler.Data.LRSSchwaeche, schueler.Id);
      schueler.Refresh();
    }

    /// <summary>
    /// Austritt eines Schülers. Das Feld Austrittsdatum wird gesetzt und der Schüler aus allen Kursen abgemeldet.
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="when">Wann der Schüler ausgetreten ist.</param>
    public static void Austritt(Schueler schueler, DateTime when)
    {
      foreach (var kurs in schueler.Kurse)
      {
        MeldeAb(schueler, new Kurs(kurs));
      }

      new SchuelerTableAdapter().UpdateManyThings(schueler.Data.KlasseId, schueler.Data.Fremdsprache2, schueler.Data.ReligionOderEthik, when, schueler.Data.LRSStoerung, schueler.Data.LRSSchwaeche, schueler.Id);
      schueler.kurse = null;
      schueler.Data.Austrittsdatum = when;
    }

    /// <summary>
    /// Wechselt einen Schüler aus einem Kurs in einen anderen.
    /// Konkret: Aus dem Von-Kurs-Fachkürzel wird er rausgeworfen und im Nach-Kurs-Fachkürzel angemeldet.
    /// meist: K für katholisch, Ev für Evangelisch, Eth für Ethik, F für Französisch, F-Wi für Französisch (fortgeführt)
    /// </summary>
    /// <param name="schueler">Der Schüler.</param>
    /// <param name="von"></param>
    /// <param name="nach">K für katholisch, Ev für Evangelisch, Eth für Ethik</param>
    public static void WechsleKurse(Schueler schueler, string von, string nach)
    {
      MeldeAb(schueler, von);
      MeldeAn(schueler, nach);
    }

    private static void MeldeAb(Schueler schueler, string vonFachKuerzel)
    {
      FachTableAdapter ada = new FachTableAdapter();
      foreach (var kurs in schueler.Kurse)
      {
        var fach = ada.GetDataById(kurs.FachId)[0];
        if (fach.Kuerzel == vonFachKuerzel)
        {
          MeldeAb(schueler, new Kurs(kurs));
        }
      }
    }

    private static void MeldeAb(Schueler schueler, Kurs vonKurs)
    {
      new SchuelerKursTableAdapter().Delete(schueler.Id, vonKurs.Id);
    }

    private static void MeldeAn(Schueler schueler, Kurs beiKurs)
    {
      SchuelerKursTableAdapter skAda = new SchuelerKursTableAdapter();
      if (skAda.GetCountBySchuelerAndKurs(schueler.Id, beiKurs.Id) == 0)
      {
        new SchuelerKursTableAdapter().Insert(schueler.Id, beiKurs.Id);
      }
    }

    private static void MeldeAn(Schueler schueler, string nachFachKuerzel)
    {
      FachTableAdapter ada = new FachTableAdapter();
      foreach (var kursZuKlasse in new KlasseKursTableAdapter().GetDataByKlasse(schueler.getKlasse.Data.Id))
      {
        var kurs = new KursTableAdapter().GetDataById(kursZuKlasse.KursId)[0];
        var fach = ada.GetDataById(kurs.FachId)[0];
        if (fach.Kuerzel == nachFachKuerzel)
        {
          MeldeAn(schueler, new Kurs(kurs));
          schueler.Refresh();
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

