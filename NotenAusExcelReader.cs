using diNo.diNoDataSetTableAdapters;
using System;
using System.Linq;

namespace diNo
{
  /// <summary>
  /// Klasse zum Einlesen der Noten aus einer Excel-Datei und Eintragen der Noten in die Datenbank.
  /// </summary>
  public class NotenAusExcelReader
  {
    /// <summary>
    /// Synchronisiert die Excel-Datei mit der Datenbank.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void Synchronize(string fileName)
    {
      using (ExcelSheet sheet = new ExcelSheet(fileName))
      {
        // erst mal schauen, ob der Kurs laut DB existiert. Todo: nächstes Jahr kursId verwenden!
        KursTableAdapter kursAdapter = new KursTableAdapter();
        var kurse = kursAdapter.GetDataByBezeichnung(sheet.Kursbezeichnung);
        if (kurse.Count != 1)
        {
          kurse = kursAdapter.GetDataByBezeichnung(sheet.Fachname + " " + sheet.Kursbezeichnung);
          if (kurse.Count != 1)
          {
            throw new InvalidOperationException("Kurs nicht oder mehrfach gefunden: " + sheet.Kursbezeichnung);
          }
        }

        //TODO: Nur zum Test am Jahresende: Unbedingt wieder rauswerfen!
        int klasseId = -1;
        if (sheet.Kursbezeichnung.ToUpper().Contains("F11TA")) { klasseId = 20;}
        if (sheet.Kursbezeichnung.ToUpper().Contains("F11TB")) { klasseId = 34;}
        if (sheet.Kursbezeichnung.ToUpper().Contains("F11TE")) { klasseId = 15;}

        int kursId = kurse[0].Id;
        using (NoteTableAdapter noteAdapter = new NoteTableAdapter())
        {
          DeleteAlteNoten(kursId, noteAdapter);
          foreach (var schueler in sheet.Schueler)
          {
            CheckId(schueler, klasseId);
            InsertNoten(kursId, noteAdapter, schueler);
          }

          var alleSchueler = CheckSchueler(sheet, kursId);
          CheckLegastheniker(sheet, kursAdapter, kurse[0], alleSchueler);
        }
      }
    }

    private static void CheckId(Schueler schueler, int klasseId)
    {
      if (schueler.Id == int.MaxValue)
      {
        var gefunden = new SchuelerTableAdapter().GetDataByKlasseUndName(klasseId, schueler.Nachname, schueler.Vorname);
        if (gefunden.Count <= 0)
        {
          var newNachname = schueler.Nachname.Replace("oe", "ö");
          newNachname = newNachname.Replace("ae", "ä");
          newNachname = newNachname.Replace("ue", "ü");
          newNachname = newNachname.Replace("ss", "ß");
          var newVorname = schueler.Vorname.Replace("oe", "ö");
          newVorname = newVorname.Replace("ae", "ä");
          newVorname = newVorname.Replace("ue", "ü");
          newVorname = newVorname.Replace("ss", "ß");
          gefunden = new SchuelerTableAdapter().GetDataByKlasseUndName(klasseId, newNachname, newVorname);
          if (gefunden.Count <= 0)
          {
            throw new InvalidOperationException("Unbekannter Schüler " + schueler.Vorname + " " + schueler.Nachname);
          }
        }

        schueler.Id = gefunden[0].Id;
      }
    }

    /// <summary>
    /// Prüft, ob die Legasthenievermerke der Datenbank mit der Excel-Datei übereinstimmen.
    /// </summary>
    /// <param name="sheet">Das Excel-Sheet.</param>
    /// <param name="kursAdapter">Der Kurs-Adapter.</param>
    /// <param name="kurs">Die Zeile des aktuellen Kurses in der Datenbank.</param>
    /// <param name="alleSchueler">Liste aller Schüler aus der Datenbank.</param>
    private static void CheckLegastheniker(ExcelSheet sheet, KursTableAdapter kursAdapter, diNoDataSet.KursRow kurs, diNoDataSet.SchuelerKursDataTable alleSchueler)
    {
      //TODO: Methode ungetestet
      using (FachTableAdapter fachAdapter = new FachTableAdapter())
      {
        var deutsch = fachAdapter.GetDataByKuerzel("D")[0];
        var englisch = fachAdapter.GetDataByKuerzel("E")[0];
        if (kurs.FachId == deutsch.Id || kurs.FachId == englisch.Id)
        {
          foreach (var schueler in alleSchueler)
          {
            var excelSchueler = sheet.Schueler.FirstOrDefault(
              x => x.Id == schueler.SchuelerId
              );

            //falls der Schüler noch in der Excel-Datei drinsteht. Könnte ja sein, dass er schon ausgetreten o. ä. ist
            if (excelSchueler != null)
            {
              var dbSchueler = new SchuelerTableAdapter().GetDataById(schueler.SchuelerId)[0];
              if (excelSchueler.IsLegastheniker != (dbSchueler.LRSSchwaeche || dbSchueler.LRSStoerung))
              {
                excelSchueler.IsLegastheniker = (dbSchueler.LRSSchwaeche || dbSchueler.LRSStoerung);
                sheet.SetLegasthenieVermerk(excelSchueler);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Holt sich die aktuelle Schülerliste aus der Datenbank. Falls ein Schüler fehlt, wird dieser in Excel unten angehängt.
    /// Todo: Abgemeldete Schüler rauslöschen (zumindest den Namen).
    /// </summary>
    /// <param name="sheet">Das ExcelSheet.</param>
    /// <param name="kursId">Die Id des Kurses.</param>
    /// <returns>Die Datentabelle mit allen Schüler-Kurs-Zuordnungen.</returns>
    private static diNoDataSet.SchuelerKursDataTable CheckSchueler(ExcelSheet sheet, int kursId)
    {
      var alleSchueler = new SchuelerKursTableAdapter().GetDataByKursId(kursId);
      foreach (var schueler in alleSchueler)
      {
        if (!sheet.Schueler.Any(x => x.Id == schueler.SchuelerId))
        {
          // Dieser Schüler fehlt offenbar bisher
          var dbSchueler = new SchuelerTableAdapter().GetDataById(schueler.SchuelerId)[0];
        
          // TODO: Testen, ob da wirklich null drinsteht oder eher DateTime.MinValue oder sowas
          if (dbSchueler.Austrittsdatum == null)
          {
            sheet.AppendSchueler(new Schueler(dbSchueler.Id, dbSchueler.Vorname, dbSchueler.Name, dbSchueler.LRSSchwaeche || dbSchueler.LRSStoerung));
          }
        }
      }

      return alleSchueler;
    }

    /// <summary>
    /// Löscht die alten Noten dieses Kurses aus der Datenbank (evtl. später nur Invalid setzen).
    /// </summary>
    /// <param name="kursId">Die Id des Kurses.</param>
    /// <param name="noteAdapter">Der Notenadapter.</param>
    private static void DeleteAlteNoten(int kursId, NoteTableAdapter noteAdapter)
    {
      foreach (var note in noteAdapter.GetDataByKursId(kursId))
      {
        // lösche die Note
        noteAdapter.Delete(note.Id);
      }
    }

    /// <summary>
    /// Trägt die Noten eines Schülers aus Excel in die Datenbank ein.
    /// </summary>
    /// <param name="kursId">Die Id des Kurses.</param>
    /// <param name="noteAdapter">Der Note-Adapter.</param>
    /// <param name="schueler">Der Schüler (samt Noten dieses Kurses).</param>
    private static void InsertNoten(int kursId, NoteTableAdapter noteAdapter, Schueler schueler)
    {
      foreach (var note in schueler.Noten)
      {
        // trage alle Noten in die DB ein
        var noteId = 0; 
        noteAdapter.Insert((int)note.Typ, note.Punktwert, DateTime.Now.Date, note.Zelle, (byte)note.Halbjahr, schueler.Id, kursId, out noteId);
      }
    }
  }
}
