﻿using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
    protected List<string> warnungen = new List<String>();

    public BasisLeseNotenAusExcel(string fileName, StatusChanged StatusChangedMethod)
    {
      this.fileName = fileName;
      this.StatusChanged = StatusChangedMethod;
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
      this.StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Meldung = meldung });
    }

    protected void HinweiseAusgeben(BasisNotendatei xls)
    {
      string s = "";
      if (hinweise.Count > 0)
      {
        foreach (var h in hinweise)
        {
          s += h + "\n";
        }
        if (MessageBox.Show(s + "\nSollen obige Änderungen in Ihre Notendatei übernommen werden.", Path.GetFileNameWithoutExtension(fileName), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
          xls.workbook.Save();
      }

      if (warnungen.Count > 0)
      {
        s = "";
        foreach (var h in warnungen)
        {
          s += h + "\n";
        }
        MessageBox.Show(s + "\nÜberprüfen Sie Ihre Notendatei oder wenden Sie sich an einen Administrator.", Path.GetFileNameWithoutExtension(fileName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    /// <summary>
    /// Löscht die alten Noten dieses Kurses aus der Datenbank
    /// </summary>
    protected abstract void DeleteAlteNoten();

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

  public class LeseNotenAusExcel : BasisLeseNotenAusExcel, IDisposable
  {
    protected OpenNotendatei xls;

    public LeseNotenAusExcel(string afileName, StatusChanged StatusChangedMethod)
      : base(afileName, StatusChangedMethod)
    {
      try
      {
        xls = new OpenNotendatei(afileName);

        ReadBasisdaten(xls);
        BackupZiehen();
  
        Status("Synchronisiere Datei " + afileName);
        Synchronize();

        Status("Übertrage Noten aus Datei " + afileName);
        DeleteAlteNoten();
        UebertrageNoten();
        HinweiseAusgeben(xls);
      }
      finally
      {
        xls.Dispose();
        xls = null;
        Status("fertig mit Datei " + afileName);
      }          
    }

    private void BackupZiehen()
    {
      // Datei sichern
      string verz = Zugriff.Instance.getString(GlobaleStrings.Backuppfad);
      if (!string.IsNullOrEmpty(verz) && !Zugriff.Instance.IsTestDB)
      {
        try
        {
          verz += "\\" + kurs.getLehrer.Kuerzel + "_" + kurs.Kursbezeichnung + "\\";
          if (!Directory.Exists(verz))
            Directory.CreateDirectory(verz);

          File.Copy(fileName, verz + Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.ToString("_yyMMdd_hhmmss") + Path.GetExtension(fileName));
        }
        catch
        {
          if (Zugriff.Instance.HatRolle(Rolle.Admin))
            MessageBox.Show("Es konnte keine Sicherungskopie der Exceldatei angelegt werden.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
    }

    /// <summary>
    /// Löscht die alten Noten dieses Kurses aus der Datenbank
    /// </summary>
    protected override void DeleteAlteNoten()
    {
      // Einzelnoten werden nur gelöscht, wenn gerade das Halbjahr neu eingelesen wird (In UebertrageNoten())

      // Halbjahresleistungen werden nicht gelöscht. Entweder arbeiten wir im aktuellen Halbjahr
      // dann findet die Methode unten den entsprechenden Eintrag und ändert ihn ab
      // oder ein Halbjahr ist bereits vergangen, dann wird die HJLeistung nicht mehr angefasst
    }

    /// <summary>
    /// Gleicht die Schülerdaten zwischen DB und Excel ab. Prüft, ob neue Schüler hinzugekommen.
    /// </summary>        
    private void Synchronize()
    {
      var klasse = kurs.Schueler;

      foreach (var schueler in klasse)
      {
        // prüfen, ob neue Schüler dazugekommen sind
        if (!sidList.Contains(schueler.Id))
        {
          xls.AppendSchueler(schueler.Data);
          sidList.Add(schueler.Id);
          hinweise.Add(schueler.Name + ", " + schueler.Data.Rufname + " wurde neu aufgenommen.");
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
    protected void UebertrageNoten()
    {
      int i = 4;
      int indexAP = CellConstant.APZeileErsterSchueler;
      NoteTableAdapter ta = new NoteTableAdapter();
      bool alles = Zugriff.Instance.Lesemodus == LesemodusExcel.Vollstaendig;

      //ermittle erst mal welche Teile jetzt eingelesen werden sollen
      bool liesErstesHJ = alles || (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.ProbezeitBOS || (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS;
      bool liesZweitesHJ = alles || ErmittleObHJ2GelesenWird();
      bool liesSAP = alles || kurs.IstSAPKurs && (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.ZweitePA; // nur in Prüfungsklassen zur 2.PA
      bool liesMAP = alles || kurs.IstSAPKurs && (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.DrittePA ||
          kurs.getFach.getKursniveau() == Kursniveau.Englisch && (Zeitpunkt)Zugriff.Instance.aktZeitpunkt >= Zeitpunkt.ErstePA;
      if (liesErstesHJ)
      {
        ta.DeleteByKursAndHalbjahr(kurs.Id, (byte)Halbjahr.Erstes);
      }
      if (liesZweitesHJ)
      {
        ta.DeleteByKursAndHalbjahr(kurs.Id, (byte)Halbjahr.Zweites);
      }

      HjArt art = GetAktuellesHalbjahr();
      Berechnungen berechner = new Berechnungen();
      foreach (int sid in sidList)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(sid);
        bool aktiverKurs = schueler.BesuchtKurs(kurs.Id);
        //if (!aktiverKurs) continue; // ausgetretene Schüler gar nicht mehr ??

        FachSchuelerNoten fsn = schueler.getNoten.getFach(kurs.Id); // die aktuell gespeicherten Noten        
        Jahrgangsstufe jg;
        Klasse klasse = schueler.getKlasse;
        // WPFs werden Jg-Übergreifend angeboten, Klasse des Schülers verwenden, IntVk teilweise gemischt mit anderen JgStufen
        if (kurs.getFach.Typ == FachTyp.WPF && klasse.Jahrgangsstufe >= Jahrgangsstufe.Zwoelf || klasse.Jahrgangsstufe == Jahrgangsstufe.IntVk)
          jg = klasse.Jahrgangsstufe;
        else  // Kurs hängt an Klasse (alte Noten werden weiterhin dieser Klasse zugeordnet)
          jg = kurs.JgStufe;

        if (aktiverKurs && !alles)
          PruefeAlteNoten(i, fsn);

        if (liesErstesHJ)
        {
          LiesHalbjahr(xls.notenbogen, fsn, Halbjahr.Erstes, i, sid, jg);
        }

        if (liesZweitesHJ)
        {
          LiesHalbjahr(xls.notenbogen2, fsn, Halbjahr.Zweites, i, sid, jg);

          if (aktiverKurs)
          {
            byte? jahresnote = xls.ReadNote("R" + i, xls.notenbogen2);
            HjLeistung.CreateOrUpdate(fsn, sid, HjArt.JN, kurs.getFach, jg, jahresnote);
          }
        }

        // Abitur
        if (aktiverKurs && (liesSAP || liesMAP))
        {
          byte? sapNote = xls.ReadNote("G" + (i + 2), xls.AP); // xls
          byte? mapNote = xls.ReadNote("H" + (i + 2), xls.AP);

          if (liesSAP && sapNote != null)
          {
            Note dieSAPNote = SucheAPNote(ta, sid, kurs.Id, Notentyp.APSchriftlich);
            if (dieSAPNote == null)
            {
              dieSAPNote = ErzeugeAPNote(sid, kurs.Id, Notentyp.APSchriftlich, (byte)sapNote, "G" + (i + 2));
            }
            else
            {
              dieSAPNote.Punktwert = (byte)sapNote;
              dieSAPNote.writeToDB();
            }
          }

          if (liesMAP)
          {
            int? sap = fsn.getNote(Halbjahr.Zweites, Notentyp.APSchriftlich); // DB            
            if (!liesSAP && sapNote != null && sap != null && sap != sapNote)
            {
              warnungen.Add(schueler.NameVorname + ": Die Punktzahl der SAP wurde verändert (alt: " + sap + ", neu: " + sapNote + ").");
            }

            if (mapNote != null)
            {
              Note dieMAPNote = SucheAPNote(ta, sid, kurs.Id, Notentyp.APMuendlich);
              if (dieMAPNote == null)
              {
                dieMAPNote = ErzeugeAPNote(sid, kurs.Id, Notentyp.APMuendlich, (byte)mapNote, "H" + (i + 2));
              }
              else
              {
                dieMAPNote.Punktwert = (byte)mapNote;
                dieMAPNote.writeToDB();
              }
            }
          }

          // AP-Gesamtleistung        
          decimal? kommaAP = xls.ReadKommanote("I" + (i + 2), xls.AP);
          byte? apGesamt = xls.ReadNote("J" + (i + 2), xls.AP);
          HjLeistung.CreateOrUpdate(fsn, sid, HjArt.AP, kurs.getFach, jg, apGesamt, kommaAP);
        }

        // Fachreferat
        if (aktiverKurs && jg == Jahrgangsstufe.Zwoelf)
        {
          HjLeistung hjlFR = null;
          byte? xlsFR = xls.ReadNote("S" + i, xls.notenbogen2);
          foreach (var fr in schueler.Fachreferat)
            if (fr.getFach.Id == kurs.getFach.Id) // schon mal übertragen
              hjlFR = fr;

          // jetzt steht in hjlFR das FR der Datenbank und in xlsFR das FR der Exceldatei 
          if (hjlFR == null && xlsFR != null) // neu anlegen (aktuelles Fach nicht gefunden)
          {
            if (schueler.Fachreferat.Count > 0)
              warnungen.Add(schueler.NameVorname + " hat bereits ein anderes Fachreferat eingetragen.");

            hjlFR = new HjLeistung(sid, kurs.getFach, HjArt.FR, jg);
          }
          if (xlsFR != null) // FR übertragen (überschreiben oder neu anlegen)
          {
            hjlFR.Punkte = (byte)xlsFR;
            hjlFR.Punkte2Dez = Convert.ToDecimal((byte)xlsFR);
            hjlFR.WriteToDB();
          }
          else if (hjlFR != null) // FR wurde in dieser Exceldatei gelöscht
          {
            hjlFR.Delete();
          }
        }
        schueler.ReloadNoten(); // neu initialisieren
        berechner.AktualisiereGE(schueler); // damit das GE wird konsistent zum Abi wird

        i++;
        indexAP++;
      }
    }

    /// <summary>
    /// Prüft ob die Noten, die nicht mehr verändert werden dürfen auch wirklich noch unverändert sind.
    /// </summary>
    /// <param name="i">Der wie vielte Schüler der Datei.</param>
    /// <param name="f">Noten dieses Faches.</param>
    /// <param name="jg">Die Jahrgangsstufe des Schülers.</param>
    private void PruefeAlteNoten(int i, FachSchuelerNoten f)
    {
      Schueler s = f.schueler;
      if ((Zeitpunkt)Zugriff.Instance.aktZeitpunkt > Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        byte? zeugnisnoteHJ1 = xls.ReadNote("O" + i, xls.notenbogen);
        if (zeugnisnoteHJ1 != null)
        {
          // Prüfe, ob die Gesamtnote aus dem ersten Halbjahr unverändert ist          
          HjLeistung hjNote1 = f.getHjLeistung(HjArt.Hj1);
          if (hjNote1 != null && hjNote1.Punkte != zeugnisnoteHJ1)
          {
            warnungen.Add(s.NameVorname + ": Die Punktzahl im 1. Halbjahr wurde verändert (alt: " + hjNote1.Punkte + ", neu: " + zeugnisnoteHJ1 + ").");
          }
        }
      }

      if ((Zeitpunkt)Zugriff.Instance.aktZeitpunkt > Zeitpunkt.ErstePA && (int)s.getKlasse.Jahrgangsstufe > 11)
      {
        // prüfe, ob sich die Noten des zweiten Halbjahres noch im richtigen Zustand befinden
        // TODO: Gibt Französisch später noch Noten ein?
        byte? zeugnisnoteHJ2 = xls.ReadNote("O" + i, xls.notenbogen2);
        if (zeugnisnoteHJ2 != null)
        {
          HjLeistung hjNote2 = f.getHjLeistung(HjArt.Hj2);
          if (hjNote2 != null && hjNote2.Punkte != zeugnisnoteHJ2)
          {
            warnungen.Add(s.NameVorname + ": Die Punktzahl im 2. Halbjahr wurde verändert (alt: " + hjNote2.Punkte + ", neu: " + zeugnisnoteHJ2 + ").");
          }
        }
      }
    }

    /// <summary>
    /// Ermittelt, ob die Noten des zweiten Halbjahres gelesen werden sollen.
    /// </summary>
    /// <returns>true wenn wir uns im zweiten Halbjahr befinden und dieses für diese Jahrgangsstufe auch noch nicht abgeschlossen ist.</returns>
    private bool ErmittleObHJ2GelesenWird()
    {
      if ((Zeitpunkt)Zugriff.Instance.aktZeitpunkt <= Zeitpunkt.HalbjahrUndProbezeitFOS)
        return false; // solange des erste Halbjahr noch läuft, wird das zweite nicht eingelesen
      
      if (kurs.Klassen.Count > 0 && kurs.Klassen[0].Jahrgangsstufe > Jahrgangsstufe.Elf)
      {
        // in den Prüfungsklassen wird das zweite Halbjahr nur von Februar bis zur ersten PA gelesen
        return (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.ErstePA;
      }
      else return true;
    }

    /// <summary>
    /// Liest die Einzelnoten eines Halbjahres ein
    /// </summary>
    /// <param name="sheet">Auf welchem Excel-Sheet sind die Noten?</param>
    /// <param name="hj">Welches Halbjahr ist es?</param>
    /// <param name="i">Der wievielte Schüler?</param>
    /// <param name="ada">Table Adapter für Halbjahresleistungen.</param>
    /// <param name="sid">Die SchülerId des Schülers.</param>
    /// <param name="jg">Die Jahrgangsstufe des Schülers.</param>
    private void LiesHalbjahr(Microsoft.Office.Interop.Excel.Worksheet sheet, FachSchuelerNoten f, Halbjahr hj, int i, int sid, Jahrgangsstufe jg)
    {
      ErzeugeNoten(sheet, i, sid, new string[] { "C", "D" }, hj, Notentyp.Kurzarbeit);
      ErzeugeNoten(sheet, i, sid, new string[] { "E", "F", "G" }, hj, Notentyp.Ex);
      ErzeugeNoten(sheet, i, sid, new string[] { "H", "I", "J" }, hj, Notentyp.EchteMuendliche);
      ErzeugeNoten(sheet, i, sid, new string[] { "L", "M" }, hj, Notentyp.Schulaufgabe);
      byte? zeugnisnote = xls.ReadNote("O" + i, sheet);
      if (zeugnisnote != null && f != null)
      {
        decimal? Punkte2Dez = xls.ReadKommanote("N" + i, sheet);
        decimal? SchnittMdl = xls.ReadKommanote("K" + i, sheet);
        HjArt art = hj == Halbjahr.Erstes ? HjArt.Hj1 : HjArt.Hj2;

        HjLeistung.CreateOrUpdate(f, sid, art, kurs.getFach, jg, zeugnisnote, Punkte2Dez, SchnittMdl);
      }
    }

    private Note SucheAPNote(NoteTableAdapter ta, int sid, int kursId, Notentyp typ)
    {
      //TODO: Aus Performancegründen im Table Adapter noch eine neue Methode mit Zusatzfilter Typ = sap/map
      var noten = ta.GetDataBySchuelerAndKurs(sid, kursId);
      if (noten != null && noten.Count > 0)
      {
        foreach (var einzelNote in noten)
        {
          Note aNote = new Note(einzelNote);
          if (aNote.Typ == typ)
          {
            return aNote;
          }
        }
      }

      return null;
    }

    private Note ErzeugeAPNote(int sid, int kursId, Notentyp typ, byte punktwert, string zelle)
    {
      Note note = new Note(kurs.Id, sid);
      note.Halbjahr = Halbjahr.Zweites;
      note.Typ = typ;
      note.Zelle = zelle;
      note.Punktwert = (byte)punktwert;
      note.writeToDB();
      return note;
    }

    private HjLeistung FindOrCreateHjLeistung(int sid, HjLeistungTableAdapter ada, HjArt art, Jahrgangsstufe jg)
    {
      var vorhandeneNote = FindHjLeistung(sid, ada, art, jg);
      return vorhandeneNote ?? new HjLeistung(sid, kurs.getFach, art, jg);
    }

    private HjLeistung FindHjLeistung(int sid, HjLeistungTableAdapter ada, HjArt art, Jahrgangsstufe jg)
    {
      var vorhandeneNoten = ada.GetDataBySchuelerAndFach(sid, kurs.getFach.Id).Where(x => x.Art == (byte)art && x.JgStufe == (byte)jg);
      return vorhandeneNoten != null && vorhandeneNoten.Count() == 1 ? new HjLeistung(vorhandeneNoten.First()) : null;
    }

    /// <summary>
    /// Liest mehrere Einzel-Noten und trägt sie in die Datenbank ein.
    /// </summary>
    /// <param name="sheet">Das Excel-Sheet.</param>
    /// <param name="i">Der wievielte Schüler.</param>
    /// <param name="sid">Die Schüler ID des Schülers.</param>
    /// <param name="spalten">Welche Spalten enthalten solche Noten.</param>
    /// <param name="hj">Das Halbjahr.</param>
    /// <param name="typ">Der Typ der Noten.</param>
    private void ErzeugeNoten(Microsoft.Office.Interop.Excel.Worksheet sheet, int i, int sid, string[] spalten, Halbjahr hj, Notentyp typ)
    {
      foreach (var zelle in spalten)
      {
        byte? p = xls.ReadNote(zelle + i, sheet);
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

    public void Dispose()
    {
      if (xls != null)
      {
        xls.Dispose();
        xls = null;
      }
    }
  }
}
