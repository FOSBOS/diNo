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

    private static Schulaufgabenwertung FindWertung(KursplanZeile aKurs, diNoDataSet.KursRow dbKurs, Schulaufgabenwertung wertungImKurs)
    {
      Schulaufgabenwertung wertungInDieserKlasse = Faecherkanon.GetSchulaufgabenwertung(aKurs.FachKurzbez, aKurs.Klasse);
      if (wertungImKurs == Schulaufgabenwertung.NotSet)
      {
        wertungImKurs = wertungInDieserKlasse;
      }
      else
      {
        if (wertungImKurs != wertungInDieserKlasse)
        {
          log.ErrorFormat("Die Schulaufgabenwertung im Kurs {0} von Lehrer {1} ist nicht konsistent: {2} vs. {3}", dbKurs.Bezeichnung, dbKurs.LehrerRow.Kuerzel, wertungImKurs, wertungInDieserKlasse);
        }
      }
      return wertungImKurs;
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
      foreach (string file in Directory.GetFiles("D:\\diNo\\Notendateien11T"))
      {
        var notenReader = new NotenAusExcelReader();
        notenReader.OnStatusChange += notenReader_OnStatusChange;
        notenReader.Synchronize(file);
      }
    }

    void notenReader_OnStatusChange(Object sender, NotenAusExcelReader.StatusChangedEventArgs e)
    {
      this.textBoxStatusMessage.Text = e.Status;
    }

    private void btnCreateExcels_Click(object sender, EventArgs e)
    {
      using (LehrerTableAdapter lehrerAdapter = new LehrerTableAdapter())
      using (KursTableAdapter kursAdapter = new KursTableAdapter())
      using (SchuelerKursTableAdapter schuelerKursAdapter = new SchuelerKursTableAdapter())
      using (SchuelerTableAdapter schuelerAdapter = new SchuelerTableAdapter())
      using (FachTableAdapter fachAdapter = new FachTableAdapter())
      {
        foreach (var kurs in kursAdapter.GetData())
        {
          var lehrer = lehrerAdapter.GetDataById(kurs.LehrerId)[0];

          string directoryName = "C:\\Projects\\diNo\\ExcelFiles\\" + lehrer.Kuerzel;
          if (!Directory.Exists(directoryName))
          {
            Directory.CreateDirectory(directoryName);
          }

          kurs.Bezeichnung = kurs.Bezeichnung.Replace('/', ' ');
          string fileName = directoryName + "\\" + kurs.Bezeichnung + ".xlsx";
          if (File.Exists(fileName))
          {
            continue;
          }

          var alleSchueler = schuelerKursAdapter.GetDataByKursId(kurs.Id);
          if (alleSchueler.Count == 0)
          {
            log.WarnFormat("Der Kurs {0} hat keine Schueler ", kurs.Bezeichnung);
            continue;
          }

          var fach = fachAdapter.GetDataById(kurs.FachId)[0];
          if (string.IsNullOrEmpty(fach.Bezeichnung))
          {
            // ignoriere FPA, Seminare und ähnliche Platzhalter
            log.Debug("Erzeuge keine Datei für das Fach " + fach.Kuerzel);
            continue;
          }

          Schulaufgabenwertung wertung = Schulaufgabenwertung.NotSet;
          var dieSchueler = new List<Schueler>();
          var klassen = new List<string>();
          foreach (var schueler in alleSchueler)
          {
            var dbSchueler = new SchuelerTableAdapter().GetDataById(schueler.SchuelerId)[0];
            string benutzterVorname = string.IsNullOrEmpty(dbSchueler.Rufname) ? dbSchueler.Vorname : dbSchueler.Rufname;
            bool isLegastheniker = dbSchueler.LRSStoerung || dbSchueler.LRSSchwaeche;
            dieSchueler.Add(new Schueler(schueler.SchuelerId, benutzterVorname, dbSchueler.Name, isLegastheniker));
            if (wertung == Schulaufgabenwertung.NotSet)
            {
              Schulart schulart = dbSchueler.KlasseWinSV.StartsWith("B") ? Schulart.BOS : Schulart.FOS;
              wertung = Faecherkanon.GetSchulaufgabenwertung(fach, Faecherkanon.GetJahrgangsstufe(dbSchueler.Jahrgangsstufe), Faecherkanon.GetZweig(dbSchueler.Ausbildungsrichtung), schulart);
            }

            if (!klassen.Contains(dbSchueler.KlasseWinSV))
            {
              klassen.Add(dbSchueler.KlasseWinSV);
            }
          }

          string klassenString = klassen.Aggregate((x, y) => x + y);
         
          // Trage die Sachen in eine leere Excel-Datei
          ExcelSheet.WriteExcelFile(wertung, fach.Bezeichnung, lehrer.Kuerzel, "2015 / 2016", klassenString, dieSchueler, fileName, kurs.Id);
        }
      }
    }

    private void btnReadExcelKurse_Click(object sender, EventArgs e)
    {
      SchuelerKursSelectorHolder kursSelector = new SchuelerKursSelectorHolder();
      kursSelector.AddSelector(new FremdspracheSelector());
      kursSelector.AddSelector(new ReliOderEthikSelector());
      UnterrichtExcelReader.ReadUnterricht("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\Daten_Stani.xlsx", kursSelector);
    }

    private void btnImportSchueler_Click(object sender, EventArgs e)
    {
      WinSVSchuelerReader.ReadSchueler("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\Datenquelle_WINSV.txt");
    }

    private void button1_Click(object sender, EventArgs e)
    {
      new Klassenansicht().Show();
    }

    private CheckReason GetCheckReason()
    {
      string reason = (string)comboBoxCheckReason.SelectedItem;
      switch (reason)
      {
        case "Probezeit BOS": return CheckReason.ProbezeitBOS;
        case "Halbjahr": return CheckReason.HalbjahrUndProbezeitFOS;
        case "1. PA": return CheckReason.ErstePA;
        case "2. PA": return CheckReason.ZweitePA;
        case "3. PA": return CheckReason.DrittePA;
        case "Jahresende": return CheckReason.Jahresende;
        default: return CheckReason.None;
      }
    }

    /// <summary>
    /// Führt alle Notenprüfungen durch.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button3_Click(object sender, EventArgs e)
    {
      CheckReason checkReason = GetCheckReason();

      //TODO: Festlegen, welche Prüfungen überhaupt durchgeführt werden müssen anhand CheckReason
      //TODO: einfach alle Klasse(n) prüfen oder sinnvolle Auswahl anhand CheckReason
      List<INotenCheck> alleNotenchecks = new List<INotenCheck>();
      alleNotenchecks.Add(new FachreferatChecker());
      alleNotenchecks.Add(new NotenanzahlChecker());
      alleNotenchecks.Add(new UnterpunktungChecker());

      // F11Technik-Klassen. TODO: So geht das künftig natürlich nicht!
      int[] klassenIDs = new[] { 15, 20, 34 };
      foreach (int klassenId in klassenIDs)
      {
        List<string> meldungen = new List<string>();
        var dieSchueler = new SchuelerTableAdapter().GetDataByKlasse(klassenId);
        foreach (var schueler in dieSchueler)
        {
          bool schuelerAdded = false;
          foreach (INotenCheck check in alleNotenchecks)
          {
            // TODO: Schulart usw. ermitteln. Für Test nur elfte Klassen.
            if (check.CheckIsNecessary(Faecherkanon.GetJahrgangsstufe(schueler.Jahrgangsstufe), Schulart.FOS, CheckReason.Jahresende))
            {
              var probleme = check.Check(schueler, checkReason);
              if (probleme.Count() > 0)
              {
                if (!schuelerAdded)
                {
                  meldungen.Add("Schüler: " + schueler.Vorname + " " + schueler.Name);
                  schuelerAdded = true;
                }

                meldungen.AddRange(probleme);
              }
            }
          }
        }

        UserControlChecks printControl = new UserControlChecks();
        printControl.Show();
        // TODO: Das geht hier natürlich noch wesentlich schicker
        printControl.Print(klassenId+ "", meldungen.ToArray());
      }
    }

    private bool IsCalculatedNote(Notentyp typ)
    {
      var array = new[] { Notentyp.Jahresfortgang, Notentyp.JahresfortgangMitNKS, Notentyp.Schnittmuendlich, Notentyp.SchnittSA };
      return array.Contains(typ);
    }

    private bool IsPruefungsnote (Notentyp typ)
    {
      var array = new[] { Notentyp.Abschlusszeugnis, Notentyp.APGesamt, Notentyp.APMuendlich, Notentyp.APSchriftlich, Notentyp.EndnoteMitNKS };
      return array.Contains(typ);
    }

    private void btnFixstand_Click(object sender, EventArgs e)
    {
      //TODO: method unchecked
      CheckReason reason = GetCheckReason();
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
            new ReportForm().Show();
        }
    }
}
