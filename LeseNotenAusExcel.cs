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
      string sicherungsverzeichnis = Zugriff.Instance.getString(GlobaleStrings.Backuppfad);
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
      this.StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Meldung = meldung });
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
      NoteTableAdapter ta = new NoteTableAdapter();

      //ermittle erst mal welche Teile jetzt eingelesen werden sollen
      bool liesErstesHJ = (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.ProbezeitBOS || (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.HalbjahrUndProbezeitFOS;
      bool liesZweitesHJ = ErmittleObHJ2GelesenWird();
      bool liesSAP = kurs.IstSAPKurs && (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.ZweitePA; // nur in Prüfungsklassen zur 2.PA
      bool liesMAP = kurs.IstSAPKurs && (Zeitpunkt)Zugriff.Instance.aktZeitpunkt == Zeitpunkt.DrittePA;
      if (liesErstesHJ)
      {
        ta.DeleteByKursAndHalbjahr(kurs.Id, (byte)Halbjahr.Erstes);
      }
      if (liesZweitesHJ)
      {
        ta.DeleteByKursAndHalbjahr(kurs.Id, (byte)Halbjahr.Zweites);
      }

      foreach (int sid in sidList)
      {
        HjArt art = GetAktuellesHalbjahr();
        Jahrgangsstufe jg;
        if (kurs.getFach.Typ == FachTyp.WPF) // WPFs werden Jg-Übergreifend angeboten, Klasse des Schülers verwenden
          jg = (Zugriff.Instance.SchuelerRep.Find(sid)).getKlasse.Jahrgangsstufe;
        else  // Kurs hängt an Klasse (alte Noten werden weiterhin dieser Klasse zugeordnet)
          jg = kurs.JgStufe;

        PruefeAlteNoten(i, ada, sid, jg);


        if (liesErstesHJ)
        {
          LiesHalbjahr(xls.notenbogen, Halbjahr.Erstes, i, ada, sid, jg);
        }

        if (liesZweitesHJ)
        {
          LiesHalbjahr(xls.notenbogen2, Halbjahr.Zweites, i, ada, sid, jg);

          byte? jahresnote = xls.ReadNote("R" + i, xls.notenbogen2);
          if (jahresnote != null)
          {
            HjLeistung l = FindOrCreateHjLeistung(sid, ada, HjArt.JN, jg);
            l.Punkte = (byte)jahresnote;
            l.WriteToDB();
          }
        }

        if (liesSAP)
        {
          byte? note = xls.ReadNote("G" + (i + 2), xls.AP);
          if (note != null)
          {
            HjLeistung sap = FindOrCreateHjLeistung(sid, ada, HjArt.AP, jg);
            sap.Punkte = (byte)note;
            sap.Punkte2Dez = note;
            sap.Status = HjStatus.Einbringen;
            sap.WriteToDB();

            Note dieSAPNote = SucheAPNote(ta, sid, kurs.Id, Notentyp.APSchriftlich);
            if (dieSAPNote == null)
            {
              dieSAPNote = ErzeugeAPNote(sid, kurs.Id, Notentyp.APSchriftlich, (byte)note, "G" + (i + 2));
            }
            else
            {
              dieSAPNote.Punktwert = (byte)note;
              dieSAPNote.writeToDB();
            }
          }
        }

        if (liesMAP)
        {
          HjLeistung ap = FindOrCreateHjLeistung(sid, ada, HjArt.AP, jg);
          byte? sapNote = xls.ReadNote("G" + (i + 2), xls.AP);
          byte? mapNote = xls.ReadNote("H" + (i + 2), xls.AP);
          decimal? kommaAP = xls.ReadKommanote("I" + (i + 2), xls.AP);
          byte? apGesamt = xls.ReadNote("J" + (i + 2), xls.AP);

          if (sapNote != null)
          {
            Note dieSAPNote = SucheAPNote(ta, sid, kurs.Id, Notentyp.APSchriftlich);
            if (dieSAPNote != null && dieSAPNote.Punktwert != sapNote)
            {
              hinweise.Add(sid + ": Die Punktzahl der SAP wurde verändert (alt: " + dieSAPNote.Punktwert + ", neu: " + sapNote + ").");              
            }
          }

          if (mapNote != null)
          {
            Note dieMAPNote = SucheAPNote(ta, sid, kurs.Id, Notentyp.APSchriftlich);
            if (dieMAPNote == null)
            {
              dieMAPNote = ErzeugeAPNote(sid, kurs.Id, Notentyp.APSchriftlich, (byte)mapNote, "H" + (i + 2));
            }
            else
            {
              dieMAPNote.Punktwert = (byte)mapNote;
              dieMAPNote.writeToDB();
            }

            ap.Punkte = (byte)apGesamt;
            ap.SchnittMdl = mapNote;
            ap.Punkte2Dez = (byte)kommaAP;
            ap.WriteToDB();
          }
        }

        byte? fachreferat = xls.ReadNote("S" + i, xls.notenbogen2);
        if (fachreferat != null)
        {
          HjLeistung l = FindOrCreateHjLeistung(sid, ada, HjArt.FR, jg);
          if (l.getFach.Id != kurs.getFach.Id)
          {
            Schueler derSchueler = Zugriff.Instance.SchuelerRep.Find(sid);
            hinweise.Add("Sie haben ein vorhandenes Fachreferat von " + derSchueler.NameVorname + " im Fach " + l.getFach.Bezeichnung + " überschrieben.");
            hinweise.Add("Bitte prüfen Sie ggf. mit der Lehrkraft des Faches die Korrektheit der Fachreferat-Eintragungen!");
            l.getFach = kurs.getFach;
          }
          l.Punkte = (byte)fachreferat;
          l.Punkte2Dez = Convert.ToDecimal((byte)fachreferat);
          l.WriteToDB();
        }

        i++;
        indexAP++;
      }
    }

    /// <summary>
    /// Prüft ob die Noten, die nicht mehr verändert werden dürfen auch wirklich noch unverändert sind.
    /// </summary>
    /// <param name="i">Der wie vielte Schüler der Datei.</param>
    /// <param name="ada">Table Adapter für Halbjahresleistungen.</param>
    /// <param name="sid">Die SchülerId des Schülers.</param>
    /// <param name="jg">Die Jahrgangsstufe des Schülers.</param>
    private void PruefeAlteNoten(int i, HjLeistungTableAdapter ada, int sid, Jahrgangsstufe jg)
    {
      Schueler s = Zugriff.Instance.SchuelerRep.Find(sid);
      if ((Zeitpunkt)Zugriff.Instance.aktZeitpunkt > Zeitpunkt.HalbjahrUndProbezeitFOS)
      {
        byte? zeugnisnoteHJ1 = xls.ReadNote("O" + i, xls.notenbogen);
        if (zeugnisnoteHJ1 != null)
        {
          // Prüfe, ob die Gesamtnote aus dem ersten Halbjahr unverändert ist          
          HjLeistung hjNote1 = FindHjLeistung(sid, ada, HjArt.Hj1, jg);
          if (hjNote1 != null && hjNote1.Punkte != zeugnisnoteHJ1)
          {            
            hinweise.Add(s.NameVorname + ": Die Punktzahl im 1. Halbjahr wurde verändert (alt: " + hjNote1.Punkte + ", neu: "+ zeugnisnoteHJ1 + ").");
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
          HjLeistung hjNote2 = FindHjLeistung(sid, ada, HjArt.Hj2, jg);
          if (hjNote2 != null && hjNote2.Punkte != zeugnisnoteHJ2)
          {            
            hinweise.Add(s.NameVorname + ": Die Punktzahl im 2. Halbjahr wurde verändert (alt: " + hjNote2.Punkte + ", neu: " + zeugnisnoteHJ2 + ").");
          }
        }
      }

      /*
      if ((Zeitpunkt)Zugriff.Instance.aktZeitpunkt > Zeitpunkt.ZweitePA && (int)s.getKlasse.Jahrgangsstufe > 11)
      {
        // prüfe, ob sich die Noten der schriftlichen AP noch im richtigen Zustand befinden
        byte? SAPxls = xls.ReadNote("G" + i, xls.AP);
        if (SAPxls != null)
        {
          byte? SAPdb = s.getNoten.FindeFach() kursid;

          if (SAPdb != null && SAPdb != SAPxls)
          {
            hinweise.Add(s.NameVorname + ": Die Punktzahl der SAP wurde verändert (alt: " + SAPdb + ", neu: " + SAPxls + ").");
          }
        }
      }
      */
    }

    /// <summary>
    /// Ermittelt, ob die Noten des zweiten Halbjahres gelesen werden sollen.
    /// </summary>
    /// <returns>true wenn wir uns im zweiten Halbjahr befinden und dieses für diese Jahrgangsstufe auch noch nicht abgeschlossen ist.</returns>
    private bool ErmittleObHJ2GelesenWird()
    {
      if ((Zeitpunkt)Zugriff.Instance.aktZeitpunkt <= Zeitpunkt.HalbjahrUndProbezeitFOS)
        return false; // solange des erste Halbjahr noch läuft, wird das zweite nicht eingelesen

      if (kurs.IstSAPKurs)
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
    private void LiesHalbjahr(Microsoft.Office.Interop.Excel.Worksheet sheet, Halbjahr hj, int i, HjLeistungTableAdapter ada, int sid, Jahrgangsstufe jg)
    {
      ErzeugeNoten(sheet, i, sid, new string[] { "C", "D" }, hj, Notentyp.Kurzarbeit);
      ErzeugeNoten(sheet, i, sid, new string[] { "E", "F", "G" }, hj, Notentyp.Ex);
      ErzeugeNoten(sheet, i, sid, new string[] { "H", "I", "J" }, hj, Notentyp.EchteMuendliche);
      ErzeugeNoten(sheet, i, sid, new string[] { "L", "M" }, hj, Notentyp.Schulaufgabe);
      byte? zeugnisnote = xls.ReadNote("O" + i, sheet);
      if (zeugnisnote != null)
      {
        HjLeistung l = FindOrCreateHjLeistung(sid, ada, hj == Halbjahr.Erstes ? HjArt.Hj1 : HjArt.Hj2, jg);
        l.Punkte = (byte)zeugnisnote;
        l.Punkte2Dez = xls.ReadKommanote("N" + i, sheet);
        l.SchnittMdl = xls.ReadKommanote("K" + i, sheet);
        l.WriteToDB();
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
      note.Halbjahr = Halbjahr.Ohne;
      note.Typ = typ;
      note.Zelle = zelle;
      note.Punktwert = (byte)punktwert;
      note.writeToDB();
      return note;
    }

    private HjLeistung FindOrCreateHjLeistung(int sid, HjLeistungTableAdapter ada, HjArt art, Jahrgangsstufe jg)
    {
      var vorhandeneNote = FindHjLeistung(sid, ada, art,jg);
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
    /// Löscht die alten Noten dieses Kurses aus der Datenbank.
    /// </summary>
    protected override void DeleteAlteNoten()
    {
      NoteTableAdapter ta = new NoteTableAdapter();
      ta.DeleteByKursId(kurs.Id);

      BerechneteNoteTableAdapter bta = new BerechneteNoteTableAdapter();
      bta.DeleteByKursId(kurs.Id);
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

    private HjLeistung FindOrCreateHjLeistung(int sid, HjLeistungTableAdapter ada, HjArt art)
    {
      var vorhandeneNote = FindHjLeistung(sid, ada, art);
      return vorhandeneNote != null ? vorhandeneNote : new HjLeistung(sid, kurs.getFach, art, Jahrgangsstufe.IntVk); //Hack für IV
    }

    private HjLeistung FindHjLeistung(int sid, HjLeistungTableAdapter ada, HjArt art)
    {
      var vorhandeneNoten = ada.GetDataBySchuelerAndFach(sid, kurs.getFach.Id).Where(x => x.Art == (byte)art);
      return vorhandeneNoten != null && vorhandeneNoten.Count() == 1 ? new HjLeistung(vorhandeneNoten.First()) : null;
    }

  }
}
