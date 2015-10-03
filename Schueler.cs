using System;
using System.Collections.Generic;
using System.Drawing;
using diNo.diNoDataSetTableAdapters;

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
    private string franzKurs;                // der ermittelte Französisch-Kurs-Fachkürzel
    private string reliOderEthik;           // Ob der Schüler Reli oder Ethik hat

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
      this.reliOderEthik = string.Empty;
      this.franzKurs = string.Empty;
    }

    /// <summary>
    /// Die Id des Schülers in der Datenbank.
    /// </summary>
    public int Id
    {
      get;
      internal set;
    }

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
    /// Ob der Schüler Legastheniker ist.
    /// </summary>
    public bool IsLegastheniker
    {
      get { return this.data.LRSStoerung || this.data.LRSSchwaeche; }
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
    /// </summary>
    public string FranzoesischKurs
    {
      get
      {
        if (!string.IsNullOrEmpty(this.franzKurs))
        {
          return this.franzKurs;
        }
        else
        {
          KursTableAdapter ada = new KursTableAdapter();
          foreach (var kurs in ada.GetDataBySchulerId(this.Id))
          {
            Kurs kursObj = new Kurs(kurs);
            if (kursObj.getFach.Kuerzel == "F")
            {
              franzKurs = "F";
              break;
            }

            if (kursObj.getFach.Kuerzel == "F-Wi")
            {
              franzKurs = "F-Wi";
              break;
            }
          }
        }

        return this.franzKurs;
      }
    }

    /// <summary>
    /// Liefert entweder
    /// K falls der Schüler in kath. Religionslehre geht
    /// Ev falls evangelisch
    /// Eth falls der Schüler in Ethik geht
    /// Leerstring falls gar keine Zuordnung
    /// </summary>
    public string ReliOderEthik
    {
      get
      {
        if (!string.IsNullOrEmpty(this.reliOderEthik))
        {
          return this.reliOderEthik;
        }
        else
        {
          KursTableAdapter ada = new KursTableAdapter();
          foreach (var kurs in ada.GetDataBySchulerId(this.Id))
          {
            Kurs kursObj = new Kurs(kurs);
            if (kursObj.getFach.Kuerzel == "K")
            {
              this.reliOderEthik = "K";
              break;
            }

            if (kursObj.getFach.Kuerzel == "Ev")
            {
              this.reliOderEthik = "Ev";
              break;
            }

            if (kursObj.getFach.Kuerzel == "Eth")
            {
              this.reliOderEthik = "Eth";
              break;
            }
          }
        }

        return this.reliOderEthik;
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
      foreach (var klassekurs in new KlasseKursTableAdapter().GetDataByKlasse(nachKlasse.Data.Id))
      {
        var kurs = new KursTableAdapter().GetDataById(klassekurs.KursId)[0];
        // prüfe, ob der Schüler in diesen Kurs gehen soll und trage ihn ein.
        UnterrichtExcelReader.AddSchuelerToKurs(kurs, kursSelector, schueler.Data);
      }

      new SchuelerTableAdapter().UpdateManyThings(nachKlasse.Data.Id, schueler.Data.Fremdsprache2, schueler.Data.ReligionOderEthik, schueler.Data.Austrittsdatum, schueler.Data.LRSStoerung, schueler.Data.LRSSchwaeche, schueler.Id);
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
        new SchuelerTableAdapter().UpdateManyThings(schueler.Data.KlasseId, schueler.Data.Fremdsprache2, schueler.Data.ReligionOderEthik, when, schueler.Data.LRSStoerung, schueler.Data.LRSSchwaeche, schueler.Id);
        schueler.kurse = null;
        schueler.Data.Austrittsdatum = when;
      }
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
      new SchuelerKursTableAdapter().Insert(schueler.Id, beiKurs.Id);
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
        }
      }
    }
  }
}

