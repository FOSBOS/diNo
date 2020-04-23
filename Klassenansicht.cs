using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace diNo
{
  public partial class Klassenansicht : BasisForm
  {
    private Schueler schueler=null;
    private SchuelerverwaltungController verwaltungController;
    private Brief frmBrief=null;
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public Klassenansicht()
    {
      log.Debug("Starte Klassenansicht.");
      InitializeComponent();
      this.olvColumnBezeichnung.AspectGetter = KlassenTreeViewController.SelectValueCol1;

      // Verwaltungsreiter
      if (Zugriff.Instance.HatVerwaltungsrechte)
      {
        this.verwaltungController = new SchuelerverwaltungController(() => { RefreshTreeView(); });
        this.treeListView1.IsSimpleDragSource = true;
        this.treeListView1.IsSimpleDropSink = true;
        this.treeListView1.ModelCanDrop += this.verwaltungController.treeListView1_ModelCanDrop;
        this.treeListView1.ModelDropped += this.verwaltungController.treeListView1_ModelDropped;               
      }
      else
      { 
        tabControl1.Controls.Remove(tabPageKurszuordnungen); // man kann die Seite nicht unsichtbar machen, nur entfernen
        tabControl1.Controls.Remove(tabPageAdministration);
        tabControl1.Controls.Remove(tabPageSekretariat);
      }

      lbTest.Visible = Zugriff.Instance.IsTestDB;    
      log.Debug("Klassenansicht fertig.");
    }

    public void RefreshTabs()
    {
      SetSchueler();
      //userControlSchueleransicht1.Schueler = null;
      //treeListView1_SelectedIndexChanged(this, null);
    }

    private void SetSchueler()
    {
      userControlSchueleransicht1.Schueler = schueler;
      userControlVorkommnisse1.Schueler = schueler;
      userControlFPAundSeminar1.Schueler = schueler;
      if (schueler == null) return;
        userControlNotenbogen1.Schueler = schueler;

      nameLabel.Text = schueler.NameVorname;
      klasseLabel.Text = schueler.KlassenBezeichnung;
      Image imageToUse = schueler.Data.Geschlecht == "W" ? global::diNo.Properties.Resources.avatarFrau : global::diNo.Properties.Resources.avatarMann;
      pictureBoxImage.Image = new Bitmap(imageToUse, pictureBoxImage.Size);
      btnBrief.Enabled = true;

      labelHinweise.Text = (schueler.IsLegastheniker ? "Legasthenie" : "");
      labelHinweise.ForeColor = Color.Red;

      if (Zugriff.Instance.HatVerwaltungsrechte)
      {
        userControlKurszuordnungen1.Schueler = schueler;
        userControlAdministration1.Schueler = schueler;
        userControlSekretariat1.Schueler = schueler;
      }
    }

    private void treeListView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      Zugriff.Instance.markierteSchueler.Clear();
      if (treeListView1.SelectedObject is Schueler)
        schueler = treeListView1.SelectedObject as Schueler;      
      
      if (schueler != null)
      {
        // aus irgendwelchen Gründen kommt das Ereignis beim Wechseln des Schülers zwei Mal,
        // davon einmal mit dem alten Schüler (sinnloser Refresh, sollte verhindert werden?)
        if (this.userControlSchueleransicht1.Schueler == null || this.userControlSchueleransicht1.Schueler.Id != schueler.Id)
        {
          SetSchueler();
        }
      }

      btnPrint.Enabled = true;
    }
    
    private void Klassenansicht_Load(object sender, EventArgs e)
    {
      btnNotenabgeben.Enabled = Zugriff.Instance.Sperre != Sperrtyp.Notenschluss || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin);
      RefreshTreeView();
    }

    private void RefreshTreeView()
    {
      Zugriff.Instance.Refresh(chkNurAktive.Checked);
      this.treeListView1.Roots = Zugriff.Instance.Klassen;
      this.treeListView1.CanExpandGetter = delegate (object x) { return (x is Klasse); };
      this.treeListView1.ChildrenGetter = delegate (object x) { return ((Klasse)x).eigeneSchueler; };
      /*
      nameLabel.Text = "";
      klasseLabel.Text = "";
      pictureBoxImage.Image = null; 
      */
      toolStripStatusLabel1.Text = "";     
    }

    private void btnNotenabgeben_Click(object sender, EventArgs e)
    {
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Excel Files|*.xls*";
      fileDialog.Multiselect = true;
      // Call the ShowDialog method to show the dialog box.
      bool userClickedOK = fileDialog.ShowDialog() == DialogResult.OK;

      // Process input if the user clicked OK.
      if (userClickedOK == true)
      {
        Cursor.Current = Cursors.WaitCursor;
        foreach (string fileName in fileDialog.FileNames)
        {
          new LeseNotenAusExcel(fileName, notenReader_OnStatusChange, false);
        }

        RefreshTreeView(); // Noten neu laden
        if (schueler != null)
        {
          schueler = Zugriff.Instance.SchuelerRep.Find(schueler.Id); // neues Objekt setzen
          SetSchueler();
        }
        MessageBox.Show("Die Notendateien wurden übertragen.","diNo",MessageBoxButtons.OK,MessageBoxIcon.Information);
        toolStripStatusLabel1.Text = "";
        Cursor.Current = Cursors.Default;
      }
    }
            
    /// <summary>
    /// Event Handler für Statusmeldungen vom Notenleser.
    /// </summary>
    /// <param name="e">Event Args mit dem neuen Status.</param>
    /// <param name="sender">Der Sender des Events.</param>
    void notenReader_OnStatusChange(Object sender, StatusChangedEventArgs e)
    {
          toolStripStatusLabel1.Text = e.Meldung;
    }

    // normale Lehrer können über den Druckbutton einen Notenbogen ausdrucken, oder bei Wahl
    // einer Klasse die Klassenliste
    private void btnPrint_Click(object sender, EventArgs e)
    {
      Bericht rpt;
      var obj = SelectedObjects();
      if (Zugriff.Instance.HatVerwaltungsrechte || obj.Count == 1)
        rpt = Bericht.Notenbogen;
      else
        rpt = Bericht.Klassenliste;
      new ReportSchuelerdruck(obj, rpt).Show();
    }

    // liefert den angeklickten Schüler, oder eine Liste von Klassen (nur für Admins)
    public List<Schueler> SelectedObjects()
    {
      var res = new List<Schueler>();
      var obj = treeListView1.SelectedObjects; // Multiselect im Klassenbereich

      // Schüler, die über NotenCheck gewählt wurden
      if (Zugriff.Instance.HatVerwaltungsrechte && Zugriff.Instance.markierteSchueler.Count > 0) 
      {
        foreach (Schueler s in Zugriff.Instance.markierteSchueler.Values)
        {
          res.Add(s);
        }
      }
      else if (obj.Count>0 && obj[0] is Klasse)
      {
        foreach (Klasse k in obj)
        {
          foreach (Schueler s in k.eigeneSchueler)
          {               
            res.Add(s);
          }
        }
      }
      else if (obj.Count > 0 && obj[0] is Schueler)
      {
        foreach (Schueler s in obj)
        {
          res.Add(s);
        }
      }
      else if (schueler!=null)
        res.Add(schueler); // nur aktuell ausgewählter Schüler

      return res;
    } 

    private void btnBrief_Click(object sender, EventArgs e)
    {
      if (frmBrief== null) frmBrief = new Brief(this);
      frmBrief.Anzeigen(schueler);
    }

    private void btnCheck_Click(object sender, EventArgs e)
    {
      var obj = treeListView1.SelectedObjects;
      List<Klasse> SelKlassen = new List<Klasse>();
      if (Zugriff.Instance.HatVerwaltungsrechte && obj.Count > 0 && obj[0] is Klasse)
      {
        foreach (Klasse k in obj)
          SelKlassen.Add(k);
      }  
      var c = new NotenCheckForm(SelKlassen);
      c.Show();
      btnPrint.Enabled = btnPrint.Enabled || Zugriff.Instance.HatVerwaltungsrechte;
    }

    public void RefreshVorkommnisse()
    {          
      userControlVorkommnisse1.RefreshVorkommnisse();
    }

    private void chkNurAktive_Click(object sender, EventArgs e)
    {
      RefreshTreeView();
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      RefreshTreeView();
    }

    private void btnCorona_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Wähle alle Notendateien aus, bei denen fehlende Halbjahresleistungen automatisch für das Corona-Abitur aus dem 1. Halbjahr übernommen werden sollen.", "diNo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)!=DialogResult.OK) return;

      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Excel Files|*.xls*";
      fileDialog.Multiselect = true;
      // Call the ShowDialog method to show the dialog box.
      if (fileDialog.ShowDialog() != DialogResult.OK) return;

      Cursor.Current = Cursors.WaitCursor;
      foreach (string fileName in fileDialog.FileNames)
      {
        toolStripStatusLabel1.Text = "Erzeuge Coronadatei für " + fileName;
        string datei = Path.GetFileName(fileName);
        string verz = fileName.Substring(0, fileName.LastIndexOf('\\')) + "\\Corona1\\";
        if (!Directory.Exists(verz))
          Directory.CreateDirectory(verz);
        File.Copy(fileName, verz+datei, true);

        //new Corona(verz + datei);
        //new LeseNotenAusExcel(fileName, notenReader_OnStatusChange);
      }

      RefreshTreeView(); // Noten neu laden
      if (schueler != null)
      {
        schueler = Zugriff.Instance.SchuelerRep.Find(schueler.Id); // neues Objekt setzen
        SetSchueler();
      }
      MessageBox.Show("Die Notendateien wurden kopiert und auch schon eingelesen, eine Abgabe ist nicht mehr notwendig. Bitte kontrolliere aber die Noten auf Plausibilität.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      toolStripStatusLabel1.Text = "";
      Cursor.Current = Cursors.Default;
    }
  }
}
