using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace diNo
{
  public partial class Klassenansicht : BasisForm
  {
    private Schueler schueler=null;
    private SchuelerverwaltungController verwaltungController;
    private Brief frmBrief=null;
    private bool zeigeAlteFOSBOSO = false;

    public Klassenansicht()
    {
      InitializeComponent();
      this.olvColumnBezeichnung.AspectGetter = KlassenTreeViewController.SelectValueCol1;

      tabControl1.Controls.Remove(tabPageNoten); // Start mit neuer FOBOSO
      tabControl1.Controls.Remove(tabPageHjLeistung); // neue FOBOSO

      // Verwaltungsreiter
      if (Zugriff.Instance.HatVerwaltungsrechte) // hier wird zum ersten Mal Zugriff instanziiert.
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
          this.userControlSchueleransicht1.Schueler = schueler;
          this.userControlVorkommnisse1.Schueler = schueler;
          this.userControlFPAundSeminar1.Schueler = schueler;
          
          if (schueler.AlteFOBOSO())
          {
            if (!zeigeAlteFOSBOSO)
            {
              tabControl1.Controls.Remove(tabPageHjLeistung);
              tabControl1.Controls.Remove(tabPageNotenbogen);
              tabControl1.TabPages.Insert(1,tabPageNoten);
              zeigeAlteFOSBOSO = true;
            }
            notenbogen1.Schueler = schueler;
          }
          else
          {
            if (zeigeAlteFOSBOSO)
            {
              tabControl1.Controls.Remove(tabPageNoten);
              tabControl1.TabPages.Insert(1, tabPageNotenbogen);
              if (schueler != null && schueler.getKlasse.Jahrgangsstufe >= Jahrgangsstufe.Zwoelf)
              {
                // nur ab der zwölften Klasse anzeigen
                tabControl1.TabPages.Insert(2, tabPageHjLeistung);
              }
              zeigeAlteFOSBOSO = false;
            }
            userControlNotenbogen1.Schueler = schueler;
            userControlHjLeistung1.Schueler = schueler;
          }

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
          }          
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
      nameLabel.Text = "";
      klasseLabel.Text = "";
      pictureBoxImage.Image = null; 
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
          var xls = new OpenAlteNotendatei(fileName);
          bool alt = xls.IsAlteSchulordnung();
          xls.Dispose(); // dies schließt die Datei gleich wieder

          if (alt)
          {
            new LeseNotenAusExcelAlt(fileName, notenReader_OnStatusChange);
          }
          else
          {
            new LeseNotenAusExcel(fileName, notenReader_OnStatusChange);
          }
        }

        RefreshTreeView(); // Noten neu laden
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
      string rpt;
      var obj = SelectedObjects();
      if (Zugriff.Instance.HatVerwaltungsrechte || obj.Count == 1)
        rpt = "rptNotenbogen";
      else
        rpt = "rptKlassenliste";       
      new ReportSchuelerdruck(obj, rpt);
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
      else
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
      var c = new NotenCheckForm();
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
  }
}
