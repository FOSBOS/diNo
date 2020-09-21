using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlAdministration : UserControl
  {
    private Schueler schueler;
    private diNoDataSet.GlobaleKonstantenRow konstanten;

    public UserControlAdministration()
    {
      InitializeComponent();
      konstanten = Zugriff.Instance.globaleKonstanten;
      if (!Zugriff.Instance.HatRolle(Rolle.Admin))
      {
        groupBoxStammdaten.Visible = false;
        groupBoxImport.Visible = false;
        groupBoxExport.Visible = false;
        groupBoxEinstellungen.Visible = false;
        groupBoxReparatur.Visible = false;
      }
      else
      {
        chkSperre.Checked = konstanten.Sperre == 1;
        edSchuljahr.Text = konstanten.Schuljahr.ToString();
        comboBoxZeitpunkt.SelectedIndex = konstanten.aktZeitpunkt - 1;
        opNurAktuelleNoten.Checked = konstanten.LeseModusExcel == 0;
        opVollstaendig.Checked = konstanten.LeseModusExcel == 1;
      }
      groupboxTest.Visible = Zugriff.Instance.IsTestDB;
      dateZeugnis.Value = konstanten.Zeugnisdatum;
      cbNotendruck.SelectedIndex = 0;
      lblStatus.Text = "";
    }

    public Schueler Schueler
    {
      get
      {
        return schueler;
      }
      set
      {
        this.schueler = value;
        if (this.schueler != null)
        {
        }
      }
    }

    // liefert die fürs Drucken ausgewählten Objekte (einzelne Schüler oder ein Menge von Klassen)
    private List<Schueler> getSelectedObjects()
    {
      // Elternreihenfolge: usercontrol -> Tabpage -> pageControl -> Form Klassenansicht
      var obj = ((Klassenansicht)(Parent.Parent.Parent)).SelectedObjects();
      if (obj.Count == 0)
        MessageBox.Show("Bitte zuerst einen Schüler oder eine/mehrere Klassen markieren.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return obj;
    }

    private void exportNoten_Click(object sender, EventArgs e)
    {
      SaveFileDialog dia = new SaveFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        Cursor = Cursors.WaitCursor;
        ImportExportJahresnoten.ExportiereHjLeistungen(dia.FileName);
        Cursor = Cursors.Default;
      }
    }

    private void importNoten_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        Cursor = Cursors.WaitCursor;
        ImportExportJahresnoten.ImportiereHJLeistungen(dia.FileName);
        Cursor = Cursors.Default;
      }
    }

    private void btnImportUnterricht_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Die Unterrichtsmatrix muss als Exceldatei in Tabelle1 vorliegen (via Untis-Export).\nBitte kontrollieren, ob Daten ab Zeile 5 vorhanden sind\nund ob Spalte A die Unterrichtsnummer, Spalte E das Lehrerkürzel, Spalte F das Fach und Spalte G alle zugeordneten Klassen enthält.", "Import Unterrichtsmatrix", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        Cursor = Cursors.WaitCursor;
        UnterrichtExcelReader.ReadUnterricht(dia.FileName);
        Cursor = Cursors.Default;
      }
    }

    private void btnImportSchueler_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        Cursor = Cursors.WaitCursor;
        WinSVSchuelerReader.ReadSchueler(dia.FileName);
        Cursor = Cursors.Default;
      }
    }

    private void btnImportKlassenleiter_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Die Exceldatei muss folgendes Format haben:\nDaten sind ab Zeile 2 vorhanden.\nSpalte C enthält das Lehrerkürzel, Spalte D die Klasse.", "Import Klassenleiter", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
      new ImportKlassenleiter();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      konstanten.Sperre = chkSperre.Checked ? 1 : 0;
      konstanten.Schuljahr = int.Parse(edSchuljahr.Text);
      konstanten.aktZeitpunkt = comboBoxZeitpunkt.SelectedIndex + 1;
      konstanten.Zeugnisdatum = dateZeugnis.Value;
      konstanten.LeseModusExcel = opVollstaendig.Checked ? 1 : 0;
      (new GlobaleKonstantenTableAdapter()).Update(konstanten);
    }

    private void btnCreateExcelsClick(object sender, EventArgs e)
    {
      if (MessageBox.Show("Die Dateien werden ins Verzeichnis " + Zugriff.Instance.getString(GlobaleStrings.VerzeichnisExceldateien) + " geschrieben (einstellbar unter globale Texte).\nDort muss auch die Vorlage.xlsx liegen.", "Notendateien erzeugen", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
      Cursor = Cursors.WaitCursor;
      new ErzeugeAlleExcelDateien(this.onStatusChange);
      Cursor = Cursors.Default;
    }

    private void btnSendMail_Click(object sender, EventArgs e)
    {
      new SendExcelMails(this.onStatusChange);
    }

    void onStatusChange(Object sender, StatusChangedEventArgs e)
    {
      this.lblStatus.Text = e.Meldung;
    }

    private void btnKlassenliste_Click(object sender, EventArgs e)
    {
      new ReportSchuelerdruck(getSelectedObjects(), Bericht.Klassenliste).Show();
    }

    private void btnSelect_Click(object sender, EventArgs e)
    {
      new Datenauswahl().ShowDialog();
    }

    private UnterschriftZeugnis getUnterschriftZeugnis()
    {
      if (opStv.Checked) return UnterschriftZeugnis.Stv;
      else if (opGez.Checked) return UnterschriftZeugnis.gez;
      else return UnterschriftZeugnis.SL;

    }

    private void btnNotendruck_Click(object sender, EventArgs e)
    {
      konstanten.Zeugnisdatum = dateZeugnis.Value; // lokale Übernahme (Speichern nur durch Übernehmen-Button)
      var obj = getSelectedObjects();
      if (obj.Count > 0)
      {
        if (cbNotendruck.SelectedIndex == 1)
        {
          if (Zugriff.Instance.aktZeitpunkt == (int)Zeitpunkt.HalbjahrUndProbezeitFOS)
            new ReportGefaehrdungen(obj).Show(); // Gefährdungen werden anders selektiert
          else
            MessageBox.Show("Gefährdungen können nur zum Halbjahr gedruckt werden.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else
        {
          new ReportSchuelerdruck(obj, (Bericht)cbNotendruck.SelectedIndex, getUnterschriftZeugnis()).Show();
        }
      }
    }

    private void btnLehrer_Click(object sender, EventArgs e)
    {
      new LehrerForm().ShowDialog();
    }

    private void btnKurs_Click(object sender, EventArgs e)
    {
      new KurseForm().ShowDialog();
    }

    private void btnGlobales_Click(object sender, EventArgs e)
    {
      new GlobalesForm().ShowDialog();
    }

    private void btnEinserAbi_Click(object sender, EventArgs e)
    {
      List<Schueler> alle = Zugriff.Instance.SchuelerRep.getList();
      List<Schueler> liste = new List<Schueler>();

      foreach (var s in alle)
      {
        if (!s.Data.IsDNoteNull() && (double)s.Data.DNote < 2.0) liste.Add(s);
      }
      new ReportSchuelerdruck(liste, Bericht.EinserAbi).Show();
    }

    private void btnBerechtigungen_Click(object sender, EventArgs e)
    {
      new ReportBerechtigungen(LehrerRolleDruck.CreateLehrerRolleDruck()).Show();
    }

    private void btnReadWahlpflichtfaecher_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Die WPF und ggf. auch die Religionskurse müssen als Textdatei (csv mit ; als Trennzeichen) vorliegen:\nSpalte 2 die Unterrichtsnummer des WPF, Spalte 7 enthält die Schüler-ID, weitere Spalten werden ignoriert.\nIst eine Schüler-ID nicht vorhanden, so kann behelfsmäßig über Spalte 1 (eindeutiger Schülername) importiert werden. Dazu muss vorher eine Zuordnungsdatei im selben Verzeichnis mit dem Namen ZuordnungSchueler.txt abgelegt sein.\nDie Zuordnungsdatei enthält im csv-Format in Spalte 1 die Schüler-ID, in Spalte 5 den eindeutigen Namen wie in der Importdatei.", "Import Wahlpflichtfächer", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        Cursor = Cursors.WaitCursor;
        WahlpflichtfachReader.Read(dia.FileName);
        Cursor = Cursors.Default;
      }
    }

    private void btnHjLeistungenWuerfeln_Click(object sender, EventArgs e)
    {
      Cursor = Cursors.WaitCursor;
      var t = new Testdaten();
      var obj = getSelectedObjects();
      foreach (var s in obj)
        t.ZufallHjLeistung(s);

      Zugriff.Instance.SchuelerRep.Clear();
      Cursor = Cursors.Default;
    }

    private void btnEinbringung_Click(object sender, EventArgs e)
    {
      try
      {
        Cursor = Cursors.WaitCursor;
        var obj = getSelectedObjects();
        var b = new Berechnungen();
        b.aufgaben.Add(b.BerechneEinbringung);
        foreach (var s in obj)
          b.BerechneSchueler(s);

        RefreshNotenbogen();
      }
      finally
      {
        Cursor = Cursors.Default;
      }
    }

    private void DelEinbr(HjLeistung hj)
    {
      if (hj != null && hj.Status != HjStatus.Ungueltig) hj.SetStatus(HjStatus.None);
    }

    private void btnDelEinbringung_Click(object sender, EventArgs e)
    {
      var obj = getSelectedObjects();
      foreach (var s in obj)
      {
        foreach (var f in s.getNoten.alleFaecher)
        {
          DelEinbr(f.getHjLeistung(HjArt.Hj1));
          DelEinbr(f.getHjLeistung(HjArt.Hj2));
          DelEinbr(f.getVorHjLeistung(HjArt.Hj1));
          DelEinbr(f.getVorHjLeistung(HjArt.Hj2));
        }
        s.Data.Berechungsstatus = (byte)Berechnungsstatus.Unberechnet;
        s.Save();
      }
      RefreshNotenbogen();
    }

    private void btnGesErg_Click(object sender, EventArgs e)
    {
      try
      {
        Cursor = Cursors.WaitCursor;
        var obj = getSelectedObjects();
        var b = new Berechnungen(Zeitpunkt.None);
        b.aufgaben.Add(b.BerechneGesErg);
        b.aufgaben.Add(b.BerechneDNote);
        b.aufgaben.Add(b.BestimmeSprachniveau);
        foreach (var s in obj)
          b.BerechneSchueler(s);
        RefreshNotenbogen();
      }
      finally
      {
        Cursor = Cursors.Default;
      }
    }

    private void RefreshNotenbogen()
    {
      ((Klassenansicht)(Parent.Parent.Parent)).RefreshTabs();
    }

    private void btnMBStatistik_Click(object sender, EventArgs e)
    {
      SaveFileDialog dia = new SaveFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        Xml.MBStatistik.Serialize(dia.FileName);
      }
    }

    private void btnSeStatistik_Click(object sender, EventArgs e)
    {
      SaveFileDialog dia = new SaveFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        Xml.SEStatistik.Serialize(dia.FileName);
      }
    }

    private void btnWPF_Click(object sender, EventArgs e)
    {
      List<Schueler> alle = Zugriff.Instance.SchuelerRep.getList();
      List<Schueler> liste = new List<Schueler>();

      foreach (var s in alle)
      {
        Jahrgangsstufe jg = s.getKlasse.Jahrgangsstufe;
        if (jg < Jahrgangsstufe.Zwoelf) continue;
        byte notw = 1;
        if (jg == Jahrgangsstufe.Zwoelf && s.Data.Schulart == "F") // FOS 12
          notw = 2;

        byte anz = 0;
        foreach (var k in s.Kurse)
        {
          if (k.getFach.Typ == FachTyp.WPF) anz++;
        }
        if (anz < notw) liste.Add(s);
      }
      if (liste.Count == 0) MessageBox.Show("Alles in Ordnung.", "diNo", MessageBoxButtons.OK);
      else new ReportSchuelerdruck(liste, Bericht.Klassenliste).Show();
    }

    private void btnKurseZuweisen_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Achtung: Alle Kurszuordnungen der Schüler werden gemäß den Kursdaten neu erstellt. Wahlpflichtfachzuordnungen gehen verloren.", "diNo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel) return;
      Cursor = Cursors.WaitCursor;
      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        foreach (Schueler s in k.eigeneSchueler)
          s.WechsleKlasse(k);
      }
      Cursor = Cursors.Default;
    }

    private void btnExportKurswahl_Click(object sender, EventArgs e)
    {
      var ek = new ExportKurswahl(getSelectedObjects());
      SaveFileDialog dia = new SaveFileDialog();
      dia.Title = "Schüler exportieren";
      dia.FileName = "Schueler.txt";
      dia.Filter = "Textdateien (*.txt)|*.txt";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ek.ExportSchueler(dia.FileName);
      }

      dia.Title = "Alte Kurse exportieren";
      dia.FileName = "AlteKurse.txt";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ek.ExportAlteWPF(dia.FileName);
      }
    }

    private void btnCorona2HJKlonen_Click(object sender, EventArgs e)
    {
      foreach (var klasse in Zugriff.Instance.Klassen)
      {
        if (klasse.Jahrgangsstufe >= Jahrgangsstufe.Zwoelf)
          continue;

        foreach (var schueler in klasse.eigeneSchueler)
        {
          foreach (var noten in schueler.getNoten.alleKurse)
          {
            var hj1 = noten.getHjLeistung(HjArt.Hj1);
            var hj2 = noten.getHjLeistung(HjArt.Hj2);
            var jahresnote = noten.getHjLeistung(HjArt.JN);

            if (hj2 != null)
            {
              hj2.Delete();
            }

            if (hj1 == null) // das betrifft eigentlich nur die in die Vorklasse zurückgetretenen. Wie geht man damit um? Bekommen die irgendein Zeugnis?
              continue;

            hj2 = new HjLeistung(schueler.Id, noten.getFach, HjArt.Hj2, hj1.JgStufe);
            hj2.Punkte = hj1.Punkte;
            hj2.Punkte2Dez = hj1.Punkte2Dez;

            // um kopierte Noten zu erkennen
            if ((schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf) &&  
                (noten.getFach.Kuerzel == "G" || // Geschichte immer
                (noten.getFach.Kuerzel == "C" && schueler.Data.Ausbildungsrichtung == "S") || //Chemie im Sozialzweig
                (noten.getFach.Kuerzel == "Rl" && schueler.Data.Ausbildungsrichtung == "W")) // Rechtslehre im Wirtschaftszweig
               )
            {
              hj2.SchnittMdl = 22;
            }
            else
            {
              hj2.SchnittMdl = 21;
            }

            hj2.WriteToDB();

            //Jahresnote
            HjLeistung.CreateOrUpdate(noten, schueler.Id, HjArt.JN, noten.getFach, hj1.JgStufe, hj1.Punkte, null, hj2.SchnittMdl);

            //Einzelnoten löschen
            new NoteTableAdapter().DeleteByKursAndHalbjahr(noten.kursId, (byte)Halbjahr.Zweites);
          }
          
          // fpA-Note
          if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && schueler.FPANoten.Count == 2)
          {
            schueler.FPANoten[1].Anleitung = schueler.FPANoten[0].Anleitung;
            schueler.FPANoten[1].Bemerkung = "aufgrund der Corona-Regelung aus dem ersten Halbjahr kopiert";
            schueler.FPANoten[1].Betrieb = schueler.FPANoten[0].Betrieb;
            schueler.FPANoten[1].Gesamt = schueler.FPANoten[0].Gesamt;
            schueler.FPANoten[1].Jahrespunkte = schueler.FPANoten[0].Gesamt;
            schueler.FPANoten[1].Stelle = "";
            schueler.FPANoten[1].Vertiefung = schueler.FPANoten[0].Vertiefung;
            if (!schueler.FPANoten[0].IsVertiefung1Null())
              schueler.FPANoten[1].Vertiefung1 = schueler.FPANoten[0].Vertiefung1;
            if (!schueler.FPANoten[0].IsVertiefung2Null())
              schueler.FPANoten[1].Vertiefung2 = schueler.FPANoten[0].Vertiefung2;
            (new FpaTableAdapter()).Update(schueler.FPANoten[1]);
          }


        }
      }
    }

    private void btnKlassen_Click(object sender, EventArgs e)
    {
      new KlasseForm().ShowDialog();
    }
  }
}

