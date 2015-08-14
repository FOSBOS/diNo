using System;
using System.Linq;
using System.Collections.Generic;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  /// <summary>
  /// Klasse zum Einlesen der Noten aus einer Excel-Datei und Eintragen der Noten in die Datenbank.
  /// </summary>
  public class LeseNotenAusExcel
  {
        private OpenNotendatei xls;
        private string fileName;
        private List<int> sidList = new List<int>();
        private Kurs kurs;

        public LeseNotenAusExcel(string afileName)
        {          
            fileName = afileName;
            xls = new OpenNotendatei(fileName);
            // Liste der gespeicherten Sids bereitstellen (alte Sids sollen nicht aus Excel gelöscht werden)
            for (int i= CellConstant.zeileSIdErsterSchueler; i < CellConstant.zeileSIdErsterSchueler + OpenNotendatei.MaxAnzahlSchueler; i++)
            {
                int sid = Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.SId + i));
                if (sid == 0) break;
                sidList.Add(i);
            }

            kurs = new Kurs(Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.KursId)));
            Synchronize();
        }

        /// <summary>
        /// Gleicht die Schülerdaten zwischen DB und Excel ab. Prüft, ob neue Schüler hinzugekommen, oder ob Legasthenie neu vermerkt wurde.
        /// </summary>        
        private void Synchronize()
        {
            /* ???
            if (OnStatusChange != null)
            {
                OnStatusChange(this, new StatusChangedEventArgs() { Status = "synchronisiere " + fileName });
            }
            */
                       
            
            diNoDataSet.SchuelerDataTable klasse = kurs.getSchueler;
            foreach (var schueler in klasse)
            {
                if (!sidList.Contains(schueler.Id))
                {
                    xls.AppendSchueler(schueler);
                }
                if (kurs.FachBezeichnung=="Englisch") // nur dort ändert sich die Wertung
                    CheckLegastheniker(schueler);
            }
        }


    /// <summary>
    /// Prüft, ob die Legasthenievermerke der Datenbank mit der Excel-Datei übereinstimmen.
    /// </summary>
    /// <param name="schueler">Liste aller Schüler aus der Datenbank.</param>
    private void CheckLegastheniker(diNoDataSet.SchuelerRow schueler)
    {
            if (schueler.LRSSchwaeche || schueler.LRSStoerung)
            {
                // Vermerk setzen
                ;
            }
    }
         
    /// <summary>
    /// Löscht die alten Noten dieses Kurses aus der Datenbank
    /// </summary>
    private void DeleteAlteNoten()
    {            
            NoteTableAdapter ta = new NoteTableAdapter();
            ta.DeleteByKursId(kurs.Id);

            BerechneteNoteTableAdapter bta = new BerechneteNoteTableAdapter();
            bta.DeleteByKursId(kurs.Id);
    }


    /// <summary>
    /// Trägt die Noten eines Schülers aus Excel in die Datenbank ein.
    /// </summary>
    /// <param name="schueler">Der Schüler (samt Noten dieses Kurses).</param>
    private static void InsertNoten(int kursId, NoteTableAdapter noteAdapter, BerechneteNoteTableAdapter berechneteNoteAdapter, Schueler schueler)
    {
      foreach (var note in schueler.Einzelnoten)
      {
        // trage alle Noten in die DB ein
        var noteId = 0;
        noteAdapter.Insert((int)note.Typ, note.Punktwert, DateTime.Now.Date, note.Zelle, (byte)note.Halbjahr, schueler.Id, kursId, out noteId);
      }

      berechneteNoteAdapter.Insert(schueler.BerechneteNotenErstesHalbjahr.SchnittMuendlich, schueler.BerechneteNotenErstesHalbjahr.SchnittSchulaufgaben,
        schueler.BerechneteNotenErstesHalbjahr.JahresfortgangMitKomma, schueler.BerechneteNotenErstesHalbjahr.JahresfortgangGanzzahlig,
        schueler.BerechneteNotenErstesHalbjahr.PruefungGesamt, schueler.BerechneteNotenErstesHalbjahr.SchnittFortgangUndPruefung,
        schueler.BerechneteNotenErstesHalbjahr.Abschlusszeugnis, (int)CheckReason.None, false, schueler.Id, kursId, true);

      berechneteNoteAdapter.Insert(schueler.BerechneteNoten.SchnittMuendlich, schueler.BerechneteNoten.SchnittSchulaufgaben,
        schueler.BerechneteNoten.JahresfortgangMitKomma, schueler.BerechneteNoten.JahresfortgangGanzzahlig,
        schueler.BerechneteNoten.PruefungGesamt, schueler.BerechneteNoten.SchnittFortgangUndPruefung,
        schueler.BerechneteNoten.Abschlusszeugnis, (int)CheckReason.None, false, schueler.Id, kursId, false);
    }
  }
}
