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
    private List<int> sidList = new List<int>(); // enthält die SIDs der Schüler, in der Reihenfolge wie im Excel
    private Kurs kurs; // der Kurs, den diese Datei abbildet
    public bool success = false;
    public StatusChanged StatusChanged;

    public LeseNotenAusExcel(string afileName, StatusChanged StatusChangedMethod)
    {
      fileName = afileName;
      this.StatusChanged = StatusChangedMethod;

      Status("Öffne Datei " + afileName);
      xls = new OpenNotendatei(fileName);
      // Liste der gespeicherten Sids bereitstellen (alte Sids sollen nicht aus Excel gelöscht werden)
      for (int i = CellConstant.zeileSIdErsterSchueler; i < CellConstant.zeileSIdErsterSchueler + OpenNotendatei.MaxAnzahlSchueler; i++)
      {
        int sid = Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.SId + i));
        if (sid == int.MaxValue)
        {
          continue; // Schüler wurde gelöscht
        }
        else
        {
          if (sid == 0) break;
          sidList.Add(sid);
        }
      }

      kurs = new Kurs(Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.KursId)));

      Status("Synchronisiere Datei " + afileName);
      Synchronize();

      Status("Übertrage Noten aus Datei " + afileName);
      DeleteAlteNoten();
      UebertrageNoten();
      success = true;

      // TODO: Gefährlich, private Variablen zu disposen?
      xls.Dispose();
      xls = null;

      Status("fertig mit Datei " + afileName);
    }

    private void Status(string meldung)
    {
      if (this.StatusChanged != null)
      {
        this.StatusChanged(this, new StatusChangedEventArgs() { Meldung = meldung });
      }
    }

    /// <summary>
    /// Gleicht die Schülerdaten zwischen DB und Excel ab. Prüft, ob neue Schüler hinzugekommen, oder ob Legasthenie neu vermerkt wurde.
    /// </summary>        
    private void Synchronize()
    {
      var klasse = kurs.getSchueler(true);
      foreach (var schueler in klasse)
      {
        if (!sidList.Contains(schueler.Id))
        {
          xls.AppendSchueler(schueler);
          sidList.Add(schueler.Id);
        }
        if (kurs.FachBezeichnung == "Englisch") // nur dort ändert sich die Wertung
          CheckLegastheniker(schueler);
      }

      var klassenSIds = klasse.Select(x => x.Id);
      foreach (var schuelerId in sidList)
      {
        if (!klassenSIds.Contains(schuelerId))
        {
          xls.RemoveSchueler(schuelerId);
        }
      }
    }


    /// <summary>
    /// Prüft, ob die Legasthenievermerke der Datenbank mit der Excel-Datei übereinstimmen.
    /// </summary>
    /// <param name="schueler">Liste aller Schüler aus der Datenbank.</param>
    private void CheckLegastheniker(diNoDataSet.SchuelerRow schueler)
    {
      Schueler schuelerObj = new Schueler(schueler);
      if (schuelerObj.IsLegastheniker) // eigentlich auch zum Entfernen gedacht, aber dann werden jedesmal alle Vermerke neu gesetzt (nicht performant).
      {
        xls.SetLegasthenievermerk(schuelerObj.Id, schuelerObj.IsLegastheniker);
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
    private void UebertrageNoten()
    {
      int i = CellConstant.ZeileErsterSchueler;
      int indexAP = CellConstant.APZeileErsterSchueler;

      foreach (int sid in sidList)
      {
        for (Halbjahr hj = Halbjahr.Erstes; hj <= Halbjahr.Zweites; hj++)
        {
          foreach (Notentyp typ in Enum.GetValues(typeof(Notentyp)))
          //new[] { Notentyp.Schulaufgabe, Notentyp.Ex, Notentyp.EchteMuendliche,
          //           Notentyp.Fachreferat, Notentyp.Ersatzprüfung, Notentyp.APSchriftlich, Notentyp.APMuendlich })
          {
            string[] zellen = CellConstant.getLNWZelle(typ, hj, i);
            foreach (string zelle in zellen)
            {
              byte? p = xls.ReadNote(typ, zelle);
              if (p != null)
              {
                Note note = new Note(kurs.Id, sid);
                note.Halbjahr = hj;
                note.Typ = typ;
                note.Zelle = zelle;
                note.Punktwert = (byte)p;
                note.writeToDB();
              }
            }
          }

          BerechneteNote bnote = new BerechneteNote(kurs.Id, sid);
          bnote.ErstesHalbjahr = (hj == Halbjahr.Erstes);
          bnote.SchnittSchulaufgaben = xls.ReadSchnitt(BerechneteNotentyp.SchnittSA, hj, i);
          bnote.SchnittMuendlich = xls.ReadSchnitt(BerechneteNotentyp.Schnittmuendlich, hj, i);
          bnote.JahresfortgangMitKomma = xls.ReadSchnitt(BerechneteNotentyp.JahresfortgangMitNKS, hj, i);
          bnote.JahresfortgangGanzzahlig = xls.ReadSchnittGanzzahlig(BerechneteNotentyp.Jahresfortgang, hj, i);
          bnote.PruefungGesamt = xls.ReadSchnitt(BerechneteNotentyp.APGesamt, hj, indexAP);
          bnote.SchnittFortgangUndPruefung = xls.ReadSchnitt(BerechneteNotentyp.EndnoteMitNKS, hj, indexAP);
          bnote.Abschlusszeugnis = xls.ReadSchnittGanzzahlig(BerechneteNotentyp.Abschlusszeugnis, hj, indexAP);
          // Erst wenn JF feststeht, wird diese Schnittkonstellation gespeichert
          if (bnote.JahresfortgangGanzzahlig != null)
            bnote.writeToDB();
        }
        i += 2;
        indexAP++;
      }
    }
  }
}
