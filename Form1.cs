using System;
using System.Linq;
using System.Windows.Forms;

using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using log4net;
using System.IO;

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
      foreach (string file in Directory.GetFiles("C:\\Projects\\diNo\\Notendateien11T"))
      {
        NotenAusExcelReader.Synchronize(file);
      }
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
            dieSchueler.Add(new Schueler(schueler.Id, benutzterVorname, dbSchueler.Name, isLegastheniker));
            if (wertung == Schulaufgabenwertung.NotSet)
            {
              Schulart schulart = dbSchueler.KlasseWinSV.StartsWith("B") ? Schulart.BOS : Schulart.FOS;
              wertung = Faecherkanon.GetSchulaufgabenwertung(fach, GetJahrgangsstufe(dbSchueler.Jahrgangsstufe), GetZweig(dbSchueler.Ausbildungsrichtung), schulart);
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

    private Zweig GetZweig(string zweig)
    {
      switch (zweig)
      {
        case "S": return Zweig.Sozial;
        case "T": return Zweig.Technik;
        case "WVR":
        case "W":
          return Zweig.Wirtschaft;
        case "V": return Zweig.None; //Vorklasse FOS ohne Zweigzuordnung
        default: throw new InvalidOperationException("unbekannter Zweig " + zweig);
      }
    }

    private Jahrgangsstufe GetJahrgangsstufe (string jahrgangsstufe)
    {
      switch (jahrgangsstufe)
      {
        case "10": return Jahrgangsstufe.Zehn; // FOS Vorklasse
        case "11": return Jahrgangsstufe.Elf; 
        case "12": return Jahrgangsstufe.Zwoelf;
        case "13": return Jahrgangsstufe.Dreizehn;
        default: throw new InvalidOperationException("unbekannte Jahrgangsstufe "+jahrgangsstufe);
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

    private void button2_Click(object sender, EventArgs e)
    {
      // TODO: Schüler auswählen
      new Notenbogen(8351).Show();
    }

    /// <summary>
    /// Führt alle Notenprüfungen durch.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button3_Click(object sender, EventArgs e)
    {
      //TODO: Auswahl, welche Prüfungen überhaupt durchgeführt werden sollen

      
    }
	}
}
