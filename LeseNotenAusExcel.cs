using System;
using System.Linq;
using System.Collections.Generic;
using diNo.diNoDataSetTableAdapters;
using System.Windows.Forms;
using System.IO;

namespace diNo
{
  /// <summary>
  /// abstrakte Basisklasse für Notenleser.
  /// </summary>
  public abstract class BasisLeseNotenAusExcel
  {
    private StatusChanged StatusChanged;    
    protected string fileName;
    protected List<int> sidList = new List<int>(); // enthält die SIDs der Schüler, in der Reihenfolge wie im Excel
    protected Kurs kurs;
    protected List<string> hinweise = new List<String>();

    public BasisLeseNotenAusExcel(string fileName, StatusChanged StatusChangedMethod)
    {
      this.fileName = fileName;
      this.StatusChanged = StatusChangedMethod;     

      Sichern();
    }

    private void Sichern()
    {
      // Datei sichern
      string sicherungsverzeichnis = Zugriff.Instance.globaleKonstanten.BackupPfad;
      if (!string.IsNullOrEmpty(sicherungsverzeichnis))
      {
        try
        {
          File.Copy(fileName, sicherungsverzeichnis +"\\" + Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.ToString("_yyMMdd_hhmmss") + Path.GetExtension(fileName));
        }
        catch
        {
          if (Zugriff.Instance.HatRolle(Rolle.Admin))
            MessageBox.Show("Es konnte keine Sicherungskopie der Exceldatei angelegt werden.","diNo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }
      }
    }

    /// <summary>
    /// Die SchuelerIDs sind immer an derselben Stelle.
    /// </summary>
    /// <param name="xls">Die Notendatei.</param>
    protected void ReadBasisdaten(BasisNotendatei xls)
    {
      // Liste der gespeicherten Sids bereitstellen (alte Sids sollen nicht aus Excel gelöscht werden)
      for (int i = CellConstant.zeileSIdErsterSchueler; i < CellConstant.zeileSIdErsterSchueler + BasisNotendatei.MaxAnzahlSchueler; i++)
      {
        int sid = Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.SId + i));
        if (sid == 0) break; // wir sind wohl am Ende der Datei
        sidList.Add(sid);
      }

      kurs = Zugriff.Instance.KursRep.Find(Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.KursId)));
    }

    protected void Status(string meldung)
    {
      if (this.StatusChanged != null)
      {
        this.StatusChanged(this, new StatusChangedEventArgs() { Meldung = meldung });
      }
    }

    protected void HinweiseAusgeben(BasisNotendatei xls)
    {
      string s = "";
      foreach (var h in hinweise)
      {
        s += h + "\n";
      }
      if (MessageBox.Show(s + "\nSollen obige Änderungen in Ihre Notendatei übernommen werden.", Path.GetFileNameWithoutExtension(fileName), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        xls.workbook.Save();
    }

    /// <summary>
    /// Löscht die alten Noten dieses Kurses aus der Datenbank
    /// </summary>
    protected void DeleteAlteNoten()
    {
      NoteTableAdapter ta = new NoteTableAdapter();
      ta.DeleteByKursId(kurs.Id);

      BerechneteNoteTableAdapter bta = new BerechneteNoteTableAdapter();
      bta.DeleteByKursId(kurs.Id);

      HjLeistungTableAdapter hja = new HjLeistungTableAdapter();
      hja.DeleteByFachAndHalbjahr(kurs.getFach.Id, (byte)GetAktuellesHalbjahr());
    }

    protected HjArt GetAktuellesHalbjahr()
    {
      switch ((Zeitpunkt)Zugriff.Instance.aktZeitpunkt)
      {
        case Zeitpunkt.HalbjahrUndProbezeitFOS: return HjArt.Hj1;
        case Zeitpunkt.ProbezeitBOS: return HjArt.Hj1;
      }
      // alle anderen Zeitpunkte gehören zum zweiten Halbjahr
      return HjArt.Hj2;
    }
  }

  public class LeseNotenAusExcel : BasisLeseNotenAusExcel
  {
    private OpenNotendatei xls;

    public LeseNotenAusExcel(string afileName, StatusChanged StatusChangedMethod)
      : base(afileName, StatusChangedMethod)
    {
      xls = new OpenNotendatei(afileName);
      ReadBasisdaten(xls);

      Status("Synchronisiere Datei " + afileName);
      Synchronize();

      Status("Übertrage Noten aus Datei " + afileName);
      DeleteAlteNoten();
      UebertrageNoten();

      if (hinweise.Count > 0) // es sind Meldungen aufgetreten
        HinweiseAusgeben(xls);

      xls.Dispose();
      xls = null;

      Status("fertig mit Datei " + afileName);
    }

    /// <summary>
    /// Gleicht die Schülerdaten zwischen DB und Excel ab. Prüft, ob neue Schüler hinzugekommen, oder ob Legasthenie neu vermerkt wurde.
    /// </summary>        
    private void Synchronize()
    {
      var klasse = kurs.getSchueler(true);

      foreach (var schueler in klasse)
      {
        // prüfen, ob neue Schüler dazugekommen sind
        if (!sidList.Contains(schueler.Id))
        {
          xls.AppendSchueler(schueler, kurs.getFach.Kuerzel == "F" || kurs.getFach.Kuerzel == "E");
          sidList.Add(schueler.Id);
          hinweise.Add(schueler.Name + ", " + schueler.Rufname + " wurde neu aufgenommen.");
        }
      }

      // prüfen, ob Schüler entfernt werden müssen
      var klassenSIds = klasse.Select(x => x.Id);
      foreach (var schuelerId in sidList)
      {
        if (!klassenSIds.Contains(schuelerId))
        {
          if (xls.RemoveSchueler(schuelerId))
          {
            Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
            hinweise.Add(schueler.Name + ", " + schueler.Data.Rufname + " hat die Klasse verlassen.");
          }
        }
      }
    }

    /// <summary>
    /// Trägt die Noten eines Schülers aus Excel in die Datenbank ein.
    /// </summary>
    private void UebertrageNoten()
    {
      int i = 4;
      int indexAP = CellConstant.APZeileErsterSchueler;
      HjLeistungTableAdapter ada = new HjLeistungTableAdapter();

      foreach (int sid in sidList)
      {
        HjArt art = GetAktuellesHalbjahr();
        if (art == HjArt.Hj1)
        {
          ErzeugeNoten(i, sid, new string[] { "C", "D", "E" }, Halbjahr.Erstes, Notentyp.Ex); // Exen bzw. Kurzarbeiten 1. HJ
          ErzeugeNoten(i, sid, new string[] { "F", "G", "H" }, Halbjahr.Erstes, Notentyp.EchteMuendliche);
          ErzeugeNoten(i, sid, new string[] { "J", "K" }, Halbjahr.Erstes, Notentyp.Schulaufgabe);
          byte? zeugnisnote = xls.ReadNote("M" + i, xls.notenbogen);
          if (zeugnisnote != null)
          {
            HjLeistung l = FindOrCreateHjLeistung(sid, ada, HjArt.Hj1);
            l.Punkte = (byte)zeugnisnote;
            l.Punkte2Dez = (byte)xls.ReadKommanote("L" + i, xls.notenbogen);
            l.SchnittMdl = (byte)xls.ReadKommanote("I" + i, xls.notenbogen);
            l.WriteToDB();
          }
        }
        else
        {
          ErzeugeNoten(i, sid, new string[] { "N", "O", "P" }, Halbjahr.Zweites, Notentyp.Ex);
          ErzeugeNoten(i, sid, new string[] { "Q", "R", "S" }, Halbjahr.Zweites, Notentyp.EchteMuendliche);
          ErzeugeNoten(i, sid, new string[] { "U", "V" }, Halbjahr.Zweites, Notentyp.Schulaufgabe);
          byte? zeugnisnote2 = xls.ReadNote("X" + i, xls.notenbogen);
          if (zeugnisnote2 != null)
          {
            HjLeistung l = FindOrCreateHjLeistung(sid, ada, HjArt.Hj2);
            l.Punkte = (byte)zeugnisnote2;
            l.Punkte2Dez = (byte)xls.ReadKommanote("W" + i, xls.notenbogen);
            l.SchnittMdl = (byte)xls.ReadKommanote("T" + i, xls.notenbogen);
            l.WriteToDB();
          }

          // Prüfe, ob die Gesamtnote aus dem ersten Halbjahr unverändert ist
          byte? zeugnisnoteHJ1 = xls.ReadNote("M" + i, xls.notenbogen);
          if (zeugnisnoteHJ1 != null)
          {
            HjLeistung hjNote1 = FindHjLeistung(sid, ada, HjArt.Hj1);
            if (hjNote1 != null && hjNote1.Punkte != zeugnisnoteHJ1)
            {
              hinweise.Add("Die Note aus dem ersten Halbjahr (" + zeugnisnoteHJ1 + ") stimmt nicht mit der Datenbank (" + hjNote1 + ") überein. Prüfen Sie Ihre Noten bzw. wenden Sie sich an den Administrator!");
            }
          }
        }

        byte? fachreferat = xls.ReadNote("Z" + i, xls.notenbogen);
        if (fachreferat != null)
        {
          HjLeistung l = FindOrCreateHjLeistung(sid, ada, HjArt.FR);
          l.Punkte = (byte)fachreferat;
          l.Punkte2Dez = Convert.ToDecimal((byte)fachreferat);
          l.SchnittMdl = Convert.ToDecimal((byte)fachreferat);
          l.WriteToDB();
        }

        i++;
        indexAP++;
      }
    }

    private HjLeistung FindOrCreateHjLeistung(int sid, HjLeistungTableAdapter ada, HjArt art)
    {
      var vorhandeneNote = FindHjLeistung(sid, ada, art);
      return vorhandeneNote != null ? vorhandeneNote : new HjLeistung(sid, kurs.getFach, art);
    }

    private HjLeistung FindHjLeistung(int sid, HjLeistungTableAdapter ada, HjArt art)
    {
      var vorhandeneNoten = ada.GetDataBySchuelerAndFach(sid, kurs.getFach.Id).Where(x => x.Art == (byte)art);
      return vorhandeneNoten != null && vorhandeneNoten.Count() == 1 ? new HjLeistung(vorhandeneNoten.First()) : null;
    }

    private void ErzeugeNoten(int i, int sid, string[] spalten, Halbjahr hj, Notentyp typ)
    {
      foreach (var zelle in spalten)
      {
        byte? p = xls.ReadNote(zelle + i, xls.notenbogen);
        if (p != null)
        {
          Note note = new Note(kurs.Id, sid);
          note.Halbjahr = hj;
          if (typ == Notentyp.Ex)
          {
            // prüfe, ob es sich um eine Kurzarbeit handelt.
            note.Typ = (xls.ReadValue(xls.notenbogen, Char.ToString(zelle[0]) + 39) == "2") ? Notentyp.Kurzarbeit : Notentyp.Ex;
          }
          else
          {
            note.Typ = typ;
          }

          note.Zelle = zelle;
          note.Punktwert = (byte)p;
          note.writeToDB();
        }
      }
    }
  }

  /// <summary>
  /// Klasse zum Einlesen der Noten aus einer Excel-Datei und Eintragen der Noten in die Datenbank.
  /// </summary>
  public class LeseNotenAusExcelAlt: BasisLeseNotenAusExcel
  {
    private OpenAlteNotendatei xls;

    public LeseNotenAusExcelAlt(string afileName, StatusChanged StatusChangedMethod)
      : base(afileName, StatusChangedMethod)
    {
      xls = new OpenAlteNotendatei(fileName);
      ReadBasisdaten(xls);

      Status("Synchronisiere Datei " + afileName);
      Synchronize();

      Status("Übertrage Noten aus Datei " + afileName);
      DeleteAlteNoten();
      UebertrageNoten();

      if (hinweise.Count > 0) // es sind Meldungen aufgetreten
        HinweiseAusgeben(xls);

      xls.Dispose();
      xls = null;

      Status("fertig mit Datei " + afileName);
    }

    /// <summary>
    /// Gleicht die Schülerdaten zwischen DB und Excel ab. Prüft, ob neue Schüler hinzugekommen, oder ob Legasthenie neu vermerkt wurde.
    /// </summary>        
    private void Synchronize()
    {
      var klasse = kurs.getSchueler(true);
      
      foreach (var schueler in klasse)
      {
        // prüfen, ob neue Schüler dazugekommen sind
        if (!sidList.Contains(schueler.Id))
        {
          xls.AppendSchueler(schueler, kurs.getFach.Kuerzel == "F" || kurs.getFach.Kuerzel == "E");
          sidList.Add(schueler.Id);
          hinweise.Add(schueler.Name + ", " + schueler.Rufname + " wurde neu aufgenommen.");
        }

        CheckLegastheniker(schueler);
      }

      // prüfen, ob Schüler entfernt werden müssen
      var klassenSIds = klasse.Select(x => x.Id);
      foreach (var schuelerId in sidList)
      {
        if (!klassenSIds.Contains(schuelerId))
        {          
          if (xls.RemoveSchueler(schuelerId))
          {
            Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
            hinweise.Add(schueler.Name + ", " + schueler.Data.Rufname + " hat die Klasse verlassen.");
          }
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
      bool isVermerkGesetzt = xls.GetLegasthenievermerk(schuelerObj.Id);
      bool sollteGesetztSein = schuelerObj.IsLegastheniker && (kurs.getFach.Kuerzel == "E" || kurs.getFach.Kuerzel == "F");
      if ((sollteGesetztSein && !isVermerkGesetzt) || (!sollteGesetztSein && isVermerkGesetzt))
      {
        string textbaustein = sollteGesetztSein ? "neu gesetzt" : "gelöscht";
        hinweise.Add("Bei " + schueler.Rufname + " " + schueler.Name + " wird der Legasthenievermerk "+ textbaustein + ". Sollte dies aus Ihrer Sicht nicht korrekt sein, wenden Sie sich bitte an das Sekretariat.");
        xls.SetLegasthenievermerk(schuelerObj.Id, sollteGesetztSein);
      }
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
          {
            if (typ==Notentyp.Kurzarbeit) continue; // wird unter Ex bearbeitet

            string[] zellen = CellConstant.getLNWZelle(typ, hj, i, indexAP);
            foreach (string zelle in zellen)
            {
              byte? p = xls.ReadNote(typ, zelle);
              if (p != null)
              {
                Note note = new Note(kurs.Id, sid);
                note.Halbjahr = hj;
                // Gewichtung steht bei Ex auf 2, also KA
                if ((typ==Notentyp.Ex) && (xls.ReadValue(xls.notenbogen,Char.ToString(zelle[0]) + CellConstant.GewichteExen)=="2"))
                   { note.Typ = Notentyp.Kurzarbeit; }
                else
                   { note.Typ = typ; }

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

          // für die PZ im 1. Hj. reicht ggf. auch eine mündliche Note
          // im 2. Hj. kann das nicht so einfach übernommen werden, da Teilnoten aus dem 1. Hj. feststehen
          if (bnote.JahresfortgangGanzzahlig == null && bnote.ErstesHalbjahr)
          {
               if (bnote.SchnittMuendlich !=null) bnote.JahresfortgangMitKomma = bnote.SchnittMuendlich;
               else if (bnote.SchnittSchulaufgaben !=null) bnote.JahresfortgangMitKomma = bnote.SchnittSchulaufgaben;
               bnote.RundeJFNote();
          }

          // Nur wenn einer der Schnitte feststeht, wird diese Schnittkonstellation gespeichert
          if (bnote.SchnittMuendlich != null || bnote. SchnittSchulaufgaben != null)
            bnote.writeToDB();
        }
         
        i += 2;
        indexAP++;
      }
    }

  }
}
