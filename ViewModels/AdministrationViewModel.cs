using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;
using Microsoft.Win32;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von UserControlAdministration.cs (WinForms). Größter, aber konzeptionell
  // einfachster Rest der Migration: überwiegend unabhängige Button-Kommandos, die Datei-Dialoge
  // öffnen und bestehende Business-Logik-Klassen aufrufen (Import/Export/Reports/Statistik).
  public partial class AdministrationViewModel : ObservableObject
  {
    private readonly diNoDataSet.GlobaleKonstantenRow konstanten = Zugriff.Instance.globaleKonstanten;

    // Ersatz für die frühere Parent-Chain-Kopplung an Klassenansicht - vom Host-Fenster gesetzt.
    public Func<List<Schueler>> SelectedObjectsProvider { get; set; }
    public Action SchuelerChangedNotifier { get; set; }

    public bool IsAdmin => Zugriff.Instance.HatRolle(Rolle.Admin);
    public bool ShowTest => Zugriff.Instance.IsTestDB;

    [ObservableProperty] private string statusText = "";

    // Globale Einstellungen
    [ObservableProperty] private bool sperreChecked;
    [ObservableProperty] private string schuljahr = "";
    [ObservableProperty] private int zeitpunktIndex;
    [ObservableProperty] private bool leseModusVollstaendig;
    public bool LeseModusNurAktuell
    {
      get => !LeseModusVollstaendig;
      set => LeseModusVollstaendig = !value;
    }
    partial void OnLeseModusVollstaendigChanged(bool value) => OnPropertyChanged(nameof(LeseModusNurAktuell));
    [ObservableProperty] private DateTime zeugnisDatum = DateTime.Today;

    // Drucken
    [ObservableProperty] private int notendruckIndex;
    [ObservableProperty] private bool unterschriftSL = true;
    [ObservableProperty] private bool unterschriftStv;
    [ObservableProperty] private bool unterschriftGez;
    [ObservableProperty] private bool rptDruck;

    public AdministrationViewModel()
    {
      if (IsAdmin)
      {
        SperreChecked = konstanten.Sperre == 1;
        Schuljahr = konstanten.Schuljahr.ToString();
        ZeitpunktIndex = konstanten.aktZeitpunkt - 1;
        LeseModusVollstaendig = konstanten.LeseModusExcel == 1;
      }
      ZeugnisDatum = konstanten.Zeugnisdatum;
    }

    partial void OnRptDruckChanged(bool value) => Zugriff.Instance.RptDruck = value;

    private List<Schueler> GetSelectedObjects()
    {
      var obj = SelectedObjectsProvider();
      if (obj.Count == 0)
        System.Windows.MessageBox.Show("Bitte zuerst einen Schüler oder eine/mehrere Klassen markieren.", "diNo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
      return obj;
    }

    private UnterschriftZeugnis GetUnterschriftZeugnis()
    {
      if (UnterschriftStv) return UnterschriftZeugnis.Stv;
      if (UnterschriftGez) return UnterschriftZeugnis.gez;
      return UnterschriftZeugnis.SL;
    }

    private void RefreshNotenbogen() => SchuelerChangedNotifier?.Invoke();

    private static bool Ask(string text, string title) =>
      System.Windows.MessageBox.Show(text, title, System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.OK;

    private static void Info(string text, string title = "diNo") =>
      System.Windows.MessageBox.Show(text, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

    private static void NichtUnterstuetzt() =>
      System.Windows.MessageBox.Show("Funktion wird aktuell nicht unterstützt!", "Notendateien versenden", System.Windows.MessageBoxButton.OK);

    // -------------------- Drucken --------------------

    [RelayCommand]
    private void Select() => new Views.DatenauswahlWindow().ShowDialog();

    [RelayCommand]
    private void Klassenliste() => new ReportSchuelerdruck(GetSelectedObjects(), Bericht.Klassenliste).Show();

    [RelayCommand]
    private void Notendruck()
    {
      konstanten.Zeugnisdatum = ZeugnisDatum; // lokale Übernahme (Speichern nur durch Übernehmen-Button)
      var obj = GetSelectedObjects();
      if (obj.Count == 0) return;

      if (NotendruckIndex == 1)
      {
        if (Zugriff.Instance.aktZeitpunkt == (int)Zeitpunkt.HalbjahrUndProbezeitFOS)
          new ReportGefaehrdungen(obj).Show(); // Gefährdungen werden anders selektiert
        else
          System.Windows.MessageBox.Show("Gefährdungen können nur zum Halbjahr gedruckt werden.", "diNo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
      }
      else
        new ReportSchuelerdruck(obj, (Bericht)NotendruckIndex, GetUnterschriftZeugnis()).Show();
    }

    // -------------------- Datenanalyse --------------------

    [RelayCommand]
    private void EinserAbi() => Auswertungen.AbiBesten();

    [RelayCommand]
    private void Schnitte() => Auswertungen.AbiSchnitte();

    [RelayCommand]
    private void Abschluss()
    {
      var obj = GetSelectedObjects();
      if (obj.Count > 0)
      {
        Zugriff.Instance.selectedAuswahlart = Auswahlart.Abschluss;
        new ReportSchuelerdruck(obj, Bericht.Auswahlliste).Show();
      }
    }

    // -------------------- Stammdaten --------------------

    [RelayCommand] private void Lehrer() => new Views.LehrerWindow().ShowDialog();
    [RelayCommand] private void Kurs() => new Views.KurseWindow().ShowDialog();
    [RelayCommand] private void Globales() => new Views.GlobalesWindow().ShowDialog();
    [RelayCommand] private void Klassen() => new Views.KlasseWindow().ShowDialog();
    [RelayCommand] private void Berechtigungen() => new ReportBerechtigungen(LehrerRolleDruck.CreateLehrerRolleDruck()).Show();
    [RelayCommand] private void Mails() => new Views.MailDialogWindow(GetSelectedObjects()).ShowDialog();

    // -------------------- Globale Einstellungen --------------------

    [RelayCommand]
    private void Save()
    {
      konstanten.Sperre = SperreChecked ? 1 : 0;
      konstanten.Schuljahr = int.Parse(Schuljahr);
      konstanten.aktZeitpunkt = ZeitpunktIndex + 1;
      konstanten.Zeugnisdatum = ZeugnisDatum;
      konstanten.LeseModusExcel = LeseModusVollstaendig ? 1 : 0;
      new GlobaleKonstantenTableAdapter().Update(konstanten);
    }

    // -------------------- Import --------------------

    [RelayCommand]
    private void ImportASVID()
    {
      var dia = new OpenFileDialog { Title = "Dateiname wählen" };
      if (dia.ShowDialog() != true) return;

      var importer = new ASVImporter();
      int anzahlErfolgreich = importer.ImportiereASVDaten(dia.FileName);
      Console.WriteLine(importer.GetKompletteProtokoll());
      importer.SpeichereFehlerProtokoll(@"C:\tmp\fehlerprotokoll.txt");
      importer.SpeichereErfolgsProtokoll(@"C:\tmp\erfolgsprotokoll.txt");

      var asvkurs = new AsvXmlKursMapper();
      asvkurs.VerarbeiteXml(dia.FileName);

      Info($"Import abgeschlossen. {anzahlErfolgreich} Schüler importiert. Protokolle unter C:\\tmp");
    }

    [RelayCommand]
    private void ImportNoten()
    {
      var dia = new OpenFileDialog { Title = "Dateiname wählen" };
      if (dia.ShowDialog() != true) return;
      RunBusy(() => ImportExportJahresnoten.ImportiereHJLeistungen(dia.FileName));
    }

    [RelayCommand]
    private void ImportUnterricht()
    {
      if (!Ask("Die Unterrichtdaten müssen als GPU002.txt aus Untis vorliegen.\nDatenbank unbedingt vorher sichern, da der Import ohne Fehler durchlaufen sollte.\n" +
        "Dazu die Error-Datei im selben Verzeichnis beachten.", "Import Unterrichtsmatrix")) return;
      var dia = new OpenFileDialog { Title = "Dateiname wählen" };
      if (dia.ShowDialog() != true) return;
      RunBusy(() => new ImportUnterricht(dia.FileName).Import());
    }

    [RelayCommand]
    private void ImportKlassenleiter() => new ImportKlassenleiter();

    [RelayCommand]
    private void ReadWahlpflichtfaecher()
    {
      if (!Ask("Die WPF müssen als Textdatei (csv mit ; als Trennzeichen) vorliegen:\nSpalte 1 enthält die ASV-ID, Spalte 2 die Unterrichtsnummer des WPF, weitere Spalten werden ignoriert.", "Import Wahlpflichtfächer")) return;
      var dia = new OpenFileDialog { Title = "Dateiname wählen" };
      if (dia.ShowDialog() != true) return;
      RunBusy(() => WahlpflichtfachReader.Read(dia.FileName));
    }

    [RelayCommand]
    private void ImportCheck() => RunBusy(() => new ImportCheck());

    [RelayCommand]
    private void ImportLoginnamen() => new ImportLoginnamen();

    [RelayCommand]
    private void ImportFPA() => new ImportFPAStellen();

    // -------------------- Export --------------------

    [RelayCommand]
    private void ExportNoten()
    {
      var dia = new SaveFileDialog { Title = "Dateiname wählen" };
      if (dia.ShowDialog() != true) return;
      RunBusy(() => ImportExportJahresnoten.ExportiereHjLeistungen(dia.FileName));
    }

    [RelayCommand]
    private void CreateExcels()
    {
      if (!Ask("Die Dateien werden ins Verzeichnis " + Zugriff.Instance.getString(GlobaleStrings.VerzeichnisExceldateien) + " geschrieben (einstellbar unter globale Texte).\nDort muss auch die Vorlage.xlsx liegen.", "Notendateien erzeugen")) return;
      RunBusy(() => new ErzeugeAlleExcelDateien(OnStatusChange));
    }

    [RelayCommand]
    private void SendExcelFiles()
    {
       var mail = new MailTools();
       mail.SendNotendateien();
    }

    [RelayCommand]
    private void MBStatistik()
    {
      var dia = new SaveFileDialog
      {
        Title = "Dateiname wählen",
        FileName = "S" + Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS) + "_" + (Zugriff.Instance.Schuljahr - 2000 + 1)
      };
      if (dia.ShowDialog() != true) return;
      RunBusy(() => new ASVExport(dia.FileName));
      Info("Fertig.");
    }

    [RelayCommand]
    private void SeStatistik()
    {
      var dia = new SaveFileDialog
      {
        Title = "Dateiname wählen",
        FileName = "S" + Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS) + "_Erfolg" + (Zugriff.Instance.Schuljahr - 2000 + 1) + ".xml"
      };
      if (dia.ShowDialog() != true) return;
      RunBusy(() => Xml.SEStatistik.Serialize(dia.FileName));
      Info("Fertig. Bitte in Zeile 3 der Datei die Attribute löschen, so dass dort nur noch <schulerfolg> steht.\nAnschließend das Prüfprogramm verwenden.");
    }

    [RelayCommand]
    private void ExportKurswahl()
    {
      var ek = new ExportKurswahl(GetSelectedObjects());
      var dia = new SaveFileDialog { Title = "Schüler exportieren", FileName = "Schueler.txt", Filter = "Textdateien (*.txt)|*.txt" };
      if (dia.ShowDialog() == true) ek.ExportSchueler(dia.FileName);

      dia.Title = "Alte Kurse exportieren";
      dia.FileName = "AlteKurse.txt";
      if (dia.ShowDialog() == true) ek.ExportAlteWPF(dia.FileName);
    }

    [RelayCommand]
    private void Absenzen() { /* new AbsenzenMail(); - im Original bereits auskommentiert */ }

    // -------------------- Reparaturen --------------------

    [RelayCommand]
    private void Einbringung()
    {
      RunBusy(() =>
      {
        var obj = GetSelectedObjects();
        var b = new Berechnungen();
        b.aufgaben.Add(b.BerechneEinbringung);
        foreach (var s in obj) b.BerechneSchueler(s);
        RefreshNotenbogen();
      });
    }

    [RelayCommand]
    private void DelEinbringung()
    {
      var obj = GetSelectedObjects();
      var ta = new PunktesummeTableAdapter();
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
        ta.DeleteBySchuelerId(s.Id);
        s.Refresh();
      }
      RefreshNotenbogen();
    }

    private static void DelEinbr(HjLeistung hj)
    {
      if (hj != null && hj.Status != HjStatus.Ungueltig) hj.SetStatus(HjStatus.None);
    }

    [RelayCommand]
    private void GesErg()
    {
      RunBusy(() =>
      {
        var obj = GetSelectedObjects();
        var b = new Berechnungen(Zeitpunkt.None);
        b.aufgaben.Add(b.BerechneGesErg);
        b.aufgaben.Add(b.BerechneDNote);
        b.aufgaben.Add(b.BestimmeSprachniveau);
        foreach (var s in obj) b.BerechneSchueler(s);
        RefreshNotenbogen();
      });
    }

    [RelayCommand]
    private void KurseZuweisen()
    {
      if (!Ask("Achtung: Alle Kurszuordnungen der Schüler werden gemäß den Kursdaten neu erstellt. Wahlpflichtfachzuordnungen gehen verloren.", "diNo")) return;
      RunBusy(() =>
      {
        foreach (Klasse k in Zugriff.Instance.Klassen)
          foreach (Schueler s in k.Schueler)
            s.WechsleKlasse(k);
      });
    }

    // -------------------- Testverfahren --------------------

    [RelayCommand]
    private void HjLeistungenWuerfeln()
    {
      RunBusy(() =>
      {
        var t = new Testdaten();
        foreach (var s in GetSelectedObjects()) t.ZufallHjLeistung(s);
        Zugriff.Instance.SchuelerRep.Clear();
      });
    }

    [RelayCommand]
    private void Notenmitteilung() => NichtUnterstuetzt();

    [RelayCommand]
    private void NotenmailSchueler() => NichtUnterstuetzt();

    private void OnStatusChange(object sender, StatusChangedEventArgs e) => StatusText = e.Meldung;

    private static void RunBusy(Action action)
    {
      System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
      try { action(); }
      finally { System.Windows.Input.Mouse.OverrideCursor = null; }
    }
  }
}
