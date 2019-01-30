using System;
using System.Windows.Forms;
using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using diNo.Zeugnisprogramm;

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
        comboBoxZeitpunkt.SelectedIndex = konstanten.aktZeitpunkt-1;        
        opNurAktuelleNoten.Checked = konstanten.LeseModusExcel == 0;
        opVollstaendig.Checked = konstanten.LeseModusExcel == 1;
      }
      string usr = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
      groupboxTest.Visible = usr == "Markus-PC\\Markus" || usr == "ClausPC\\Claus";
      dateZeugnis.Value = konstanten.Zeugnisdatum;
      cbNotendruck.SelectedIndex = 0;
      lblStatus.Text = "";

      if (System.Security.Principal.WindowsIdentity.GetCurrent().Name == "ClausPC\\Claus")
      {
        cbNotendruck.SelectedIndex = 7;
      }
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
      var obj =  ((Klassenansicht)(Parent.Parent.Parent)).SelectedObjects();
      if (obj.Count==0)
        MessageBox.Show("Bitte zuerst einen Schüler oder eine/mehrere Klassen markieren.","diNo",MessageBoxButtons.OK,MessageBoxIcon.Information);
      return obj;
    }

    private void exportNoten_Click(object sender, EventArgs e)
    {
      SaveFileDialog dia = new SaveFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ImportExportJahresnoten.ExportiereHjLeistungen(dia.FileName);
      }
    }

    private void importNoten_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ImportExportJahresnoten.ImportierteHJLeistungen(dia.FileName);
      }
    }

    private void btnImportUnterricht_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        UnterrichtExcelReader.ReadUnterricht(dia.FileName);
      }
    }

    private void btnImportSchueler_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        WinSVSchuelerReader.ReadSchueler(dia.FileName);
      }
    }

    private void btnImportKlassenleiter_Click(object sender, EventArgs e)
    {
        new ImportKlassenleiter();
    }
    
    private void btnAttestpflicht_Click(object sender, EventArgs e)
    {      
      if (schueler != null)
      {        
        var b = new BriefDaten(schueler, BriefTyp.Attestpflicht);
        b.Betreff = "Attestpflicht";
        b.Inhalt += "da sich im laufenden Schuljahr bei ";
        if (b.IstU18) b.Inhalt += (schueler.Data.Geschlecht == "M" ? "Ihrem Sohn " : "Ihrer Tochter ") + schueler.VornameName;
        else b.Inhalt += "Ihnen";
        b.Inhalt += " die krankheitsbedingten Schulversäumnisse häufen, werden Sie gemäß § 20 (2) BaySchO dazu verpflichtet, künftig jede weitere krankheitsbedingte Abwesenheit ";
        b.Inhalt += "durch ein aktuelles ärztliches Zeugnis (Schulunfähigkeitsbescheinigung) zu belegen.<br><br>";
        b.Inhalt += "Wird das Zeugnis nicht unverzüglich vorgelegt, so gilt das Fernbleiben als unentschuldigt.";                        
        b.Unterschrift2 = "Helga Traut, OStDin\nSchulleiterin";
        new ReportBrief(b).Show();

        schueler.AddVorkommnis(Vorkommnisart.Attestpflicht,"", false);
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      konstanten.Sperre = chkSperre.Checked ? 1 : 0;
      konstanten.Schuljahr = int.Parse(edSchuljahr.Text);
      konstanten.aktZeitpunkt = comboBoxZeitpunkt.SelectedIndex+1;      
      konstanten.Zeugnisdatum = dateZeugnis.Value;
      konstanten.LeseModusExcel = opVollstaendig.Checked ? 1 : 0;
      (new GlobaleKonstantenTableAdapter()).Update(konstanten);
    }

    private void btnCreateExcelsClick(object sender, EventArgs e)
    {
      new ErzeugeAlleExcelDateien(this.onStatusChange);
    }

    private void btnSendMail_Click(object sender, EventArgs e)
    {
      new SendExcelMails(this.onStatusChange);
    }

    private void btnNotenWinSV_Click(object sender, EventArgs e)
    {
      Zeitpunkt reason = (Zeitpunkt)Zugriff.Instance.aktZeitpunkt;
      string fileName = "C:\\projects\\diNo\\OmnisDB\\dzeugnis.txt";
      string fileNameNeu = "C:\\projects\\diNo\\OmnisDB\\dzeugnisNEU.txt";
      OmnisDB.DZeugnisFileController controller = new OmnisDB.DZeugnisFileController(fileName, fileNameNeu, reason);
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

    private void btnExportSchuelerZeugnisprogramm_Click(object sender, EventArgs e)
    {
      SaveFileDialog dia = new SaveFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        ExportSchueler.Write(dia.FileName);
        ExportLehrer.Write(dia.FileName + "_Lehrer.csv");
      }
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
      if (obj.Count>0)
      {
        if (cbNotendruck.SelectedIndex == 1)
        {
          if (Zugriff.Instance.aktZeitpunkt == (int)Zeitpunkt.HalbjahrUndProbezeitFOS)
            new ReportGefaehrdungen(obj).Show(); // Gefährdungen werden anders selektiert
          else
            MessageBox.Show("Gefährdungen können nur zum Halbjahr gedruckt werden.","diNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else
        {
          new ReportSchuelerdruck(obj, (Bericht) cbNotendruck.SelectedIndex, getUnterschriftZeugnis()).Show();
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
        if (!s.Data.IsDNoteAllgNull())
        {
          if ((double)Math.Min(s.Data.DNote,s.Data.DNoteAllg)< 2.0) liste.Add(s);
        }
        else if (!s.Data.IsDNoteNull() && (double)s.Data.DNote < 2.0) liste.Add(s);
      }
      new ReportSchuelerdruck(liste, Bericht.EinserAbi).Show();
    }

    private void btnBerechtigungen_Click(object sender, EventArgs e)
    {
      new ReportBerechtigungen(LehrerRolleDruck.CreateLehrerRolleDruck()).Show();
    }

    private void btnReadWahlpflichtfaecher_Click(object sender, EventArgs e)
    {
      OpenFileDialog dia = new OpenFileDialog();
      dia.Title = "Dateiname wählen";
      if (dia.ShowDialog() == DialogResult.OK)
      {
        WahlpflichtfachReader.Read(dia.FileName);
      }
    }

    private void btnHjLeistungenWuerfeln_Click(object sender, EventArgs e)
    {
      var t = new Testdaten();
      var obj = getSelectedObjects();
      foreach (var s in obj)
        t.ZufallHjLeistung(s);

    }

    private void btnEinbringung_Click(object sender, EventArgs e)
    {
      var obj = getSelectedObjects();
      var b = new Berechnungen(Zeitpunkt.None);
      b.aufgaben.Add(b.BerechneEinbringung);
      foreach (var s in obj)
        b.BerechneSchueler(s);

      RefreshNotenbogen();
    }

    private void DelEinbr(HjLeistung hj)
    {
      if (hj != null) hj.SetStatus(HjStatus.None);
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
      }
      RefreshNotenbogen();
    }

    private void btnGesErg_Click(object sender, EventArgs e)
    {
      var obj = getSelectedObjects();
      var b = new Berechnungen(Zeitpunkt.None);
      b.aufgaben.Add(b.BerechneGesErg);
      b.aufgaben.Add(b.BerechneDNote);
      b.aufgaben.Add(b.BestimmeSprachniveau);
      foreach (var s in obj)
        b.BerechneSchueler(s);
      RefreshNotenbogen();
    }

    private void RefreshNotenbogen()
    {
      ((Klassenansicht)(Parent.Parent.Parent)).RefreshTabs();
    }
  }
}

