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

        public LeseNotenAusExcel(string afileName)
        {            
            fileName = afileName;
            xls = new OpenNotendatei(fileName);
            // Liste der gespeicherten Sids bereitstellen (alte Sids sollen nicht aus Excel gelöscht werden)
            for (int i = CellConstant.zeileSIdErsterSchueler; i < CellConstant.zeileSIdErsterSchueler + OpenNotendatei.MaxAnzahlSchueler; i++)
            {
                int sid = Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.SId + i));
                if (sid == 0) break;
                sidList.Add(i);
            }

            kurs = new Kurs(Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.KursId)));
            Synchronize();
            DeleteAlteNoten();
            UebertrageNoten();
            success = true;
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
                if (kurs.FachBezeichnung == "Englisch") // nur dort ändert sich die Wertung
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
        private void UebertrageNoten()
        {
            int i = CellConstant.ZeileErsterSchueler;
            int indexAP = CellConstant.APZeileErsterSchueler;
            List<BerechneteNote> schnitte = new List<BerechneteNote>();

            foreach (int sid in sidList)
            {
                // Schueler schueler = new Schueler(sid); // dieser Schüler ist jetzt dran
                for (Halbjahr hj = Halbjahr.Erstes; hj <= Halbjahr.Zweites; hj++)
                {
                    foreach (Notentyp typ in new[] { Notentyp.Schulaufgabe, Notentyp.Ex, Notentyp.EchteMuendliche,
                                    Notentyp.Fachreferat, Notentyp.Ersatzprüfung, Notentyp.APSchriftlich, Notentyp.APMuendlich })
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

                    BerechneteNote bnote = new BerechneteNote(sid, kurs.Id);
                    bnote.ErstesHalbjahr = (hj == Halbjahr.Erstes);
                    bnote.SchnittSchulaufgaben = xls.ReadSchnitt(Notentyp.SchnittSA, hj, i);
                    bnote.SchnittMuendlich = xls.ReadSchnitt(Notentyp.Schnittmuendlich, hj, i);
                    bnote.JahresfortgangMitKomma = xls.ReadSchnitt(Notentyp.JahresfortgangMitNKS, hj, i);
                    bnote.JahresfortgangGanzzahlig = xls.ReadSchnittGanzzahlig(Notentyp.Jahresfortgang, hj, i);
                    bnote.PruefungGesamt = xls.ReadSchnitt(Notentyp.APGesamt, hj, indexAP);
                    bnote.SchnittFortgangUndPruefung = xls.ReadSchnitt(Notentyp.EndnoteMitNKS, hj, indexAP);
                    bnote.Abschlusszeugnis = xls.ReadSchnittGanzzahlig(Notentyp.Abschlusszeugnis, hj, indexAP);
                    schnitte.Add(bnote);
                }

                // Erst wenn JF des 2. Hj. feststeht, wird diese Schnittkonstellation verwendet, sonst erstes Hj.
                if (schnitte[1].JahresfortgangGanzzahlig != null)
                    schnitte[1].writeToDB(); // 2. Hj
                else
                    schnitte[0].writeToDB(); // 1. Hj
            }
        }
    }
}
