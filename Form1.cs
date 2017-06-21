using System;
using System.Linq;
using System.Windows.Forms;

using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using log4net;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;

namespace diNo
{

	public partial class Form1 : Form
	{
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public Form1()
		{
			InitializeComponent();
		}

    private void btnReadLehrer_Click(object sender, EventArgs e)
    {
      LehrerFileReader.Read("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\STDLEHR.txt");
    }

    private static List<KursplanZeile> FilterKurse(IList<KursplanZeile> alleKurseRaw)
    {
      var alleKurse = new List<KursplanZeile>();
      // für Kurse ohne namen vergibt selbst einen
      foreach (var kurs in alleKurseRaw)
      {
        if (string.IsNullOrEmpty(kurs.KursBezeichnung))
        {
          kurs.KursBezeichnung = kurs.FachKurzbez + kurs.Klasse;
        }

        // Todo: Was für Kurse kommen hier weg?
        if (kurs.Klasse != "")
        {
          alleKurse.Add(kurs);
        }
      }
      return alleKurse;
    }

    private void btnReadExcelFile_Click(object sender, EventArgs e)
    {            
      //foreach (string file in Directory.GetFiles(Konstanten.ExcelPfad + "\\Abgabe"))
      //{

      //      var reader = new LeseNotenAusExcel(file, notenReader_OnStatusChange, null);
      //      if (reader.success)
      //          Directory.Move(file, Konstanten.ExcelPfad + "\\Archiv");
      //}
     
    }

    
    void notenReader_OnStatusChange(Object sender, StatusChangedEventArgs e)
    {
      this.textBoxStatusMessage.Text = e.Meldung;
    }

    private void btnCreateExcels_Click(object sender, EventArgs e)
    {
       new ErzeugeAlleExcelDateien(this.notenReader_OnStatusChange);
    }

    private void btnReadExcelKurse_Click(object sender, EventArgs e)
    {
      UnterrichtExcelReader.ReadUnterricht("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\Daten_Stani 2015.xlsx");
    }

    private void btnImportSchueler_Click(object sender, EventArgs e)
    {
      WinSVSchuelerReader.ReadSchueler("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\Datenquelle_WINSV_2015.txt");
    }

    private void button1_Click(object sender, EventArgs e)
    {
      new Klassenansicht().Show();
    }

    private Zeitpunkt GetZeitpunkt()
    {
      string reason = (string)comboBoxZeitpunkt.SelectedItem;
      switch (reason)
      {
        case "Probezeit BOS": return Zeitpunkt.ProbezeitBOS;
        case "Halbjahr": return Zeitpunkt.HalbjahrUndProbezeitFOS;
        case "1. PA": return Zeitpunkt.ErstePA;
        case "2. PA": return Zeitpunkt.ZweitePA;
        case "3. PA": return Zeitpunkt.DrittePA;
        case "Jahresende": return Zeitpunkt.Jahresende;
        default: return Zeitpunkt.None;
      }
    }

    /// <summary>
    /// Führt alle Notenprüfungen durch.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button3_Click(object sender, EventArgs e)
    {
  
    }

    private void btnFixstand_Click(object sender, EventArgs e)
    {
      //TODO: method unchecked
      Zeitpunkt reason = GetZeitpunkt();
      var noteAdapter = new NoteTableAdapter();
      var fixNoteAdapter = new BerechneteNoteTableAdapter();
      var alleNotenDerSchule = fixNoteAdapter.GetData();
      foreach (var note in alleNotenDerSchule)
      {
        fixNoteAdapter.Insert(note.SchnittMuendlich, note.SchnittSchulaufgaben, note.JahresfortgangMitKomma,
          note.JahresfortgangGanzzahlig, note.PruefungGesamt, note.SchnittFortgangUndPruefung, note.Abschlusszeugnis,
          (int)reason, true, note.SchuelerId, note.KursId, note.ErstesHalbjahr);
      }
    }

        private void btnReport_Click(object sender, EventArgs e)
        {
            //new ReportSchuelerliste();
            //new ReportLehrerliste();
            //new ReportFachliste();
            //new ReportNotenbogen(new Schueler(8861));
        }

    private void btnSendMail_Click(object sender, EventArgs e)
    {
      new SendExcelMails(this.notenReader_OnStatusChange);
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.userControlVorkommnisse1.Visible = true;
      this.userControlVorkommnisse1.Schueler = new Schueler(8861);
    }

        private void btnVerweis_Click(object sender, EventArgs e)
        {           
        }

        private void button4_Click(object sender, EventArgs e)
        {
      int aktuelleSchO = 0;
      int davonNachNeuerBestanden = 0;
      int neueSchO = 0;
      int davonNachAlterBestanden = 0;
      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        if (k.Jahrgangsstufe == Jahrgangsstufe.Elf)
        {
          foreach (diNoDataSet.SchuelerRow row in k.getSchueler)
          {
            Schueler schueler = Zugriff.Instance.SchuelerRep.Find(row.Id);

            bool schonVorherAusgetreten = !schueler.Data.IsAustrittsdatumNull() && schueler.Data.Austrittsdatum < new DateTime(2017, 02, 01);
            if (schonVorherAusgetreten)
              continue;

            int anzahlFaecher = 0;
            int summePunkte = 0;
            foreach (var fachnote in schueler.getNoten.alleFaecher)
            {
              byte? note = fachnote.getRelevanteNote((Zeitpunkt)Zugriff.Instance.aktZeitpunkt);
              summePunkte += note != null ? (int)note : 0;
              anzahlFaecher++;
            }

            int fuenfer = schueler.getNoten.AnzahlNoten(5);
            int sechser = schueler.getNoten.AnzahlNoten(6);
            bool nachNeuerRegelungDurchgefallen = ((fuenfer == 1 && sechser == 0 && summePunkte < 5 * anzahlFaecher)
              || (((fuenfer == 2 && sechser == 0) || (fuenfer == 0 && sechser == 1)) && summePunkte < 6 * anzahlFaecher)
              || fuenfer > 2 || sechser > 1 || (sechser == 1 && fuenfer > 0));

            if (schueler.getNoten.HatNichtBestanden())
            {
              aktuelleSchO++;
              if (!nachNeuerRegelungDurchgefallen)
              {
                davonNachNeuerBestanden++;
              }
            }

            if (nachNeuerRegelungDurchgefallen)
            {
              neueSchO++;
              if (!schueler.getNoten.HatNichtBestanden())
                davonNachAlterBestanden++;
            }
          }
        }
      }

      string text = "Nach alter Regelung haben " + aktuelleSchO + " Schüler nicht bestanden.";
      text += "\n    davon werden künftig bestehen: " + davonNachNeuerBestanden;
      text += "\nNach neuer Regelung haben " + neueSchO + "Schüler nicht bestanden.";
      text += "\n    davon hatten nach alter Schulordnung " + davonNachAlterBestanden + " kein Problem";
      MessageBox.Show(text);
        }

    private void btnNotenWinSV_Click(object sender, EventArgs e)
    {
      // ImportExportJahresnoten.ImportiereNotenAusWinSD("H:\\Sicherung SD 14_15\\NotenPerExcelAusWinSD.csv");

      Zeitpunkt reason = (Zeitpunkt)Zugriff.Instance.aktZeitpunkt;
      string fileName = "C:\\projects\\diNo\\OmnisDB\\dzeugnis.txt";
      string fileNameNeu = "C:\\projects\\diNo\\OmnisDB\\dzeugnisNEU.txt";
      OmnisDB.DZeugnisFileController controller = new OmnisDB.DZeugnisFileController(fileName, fileNameNeu, reason);
    }
  }
}
