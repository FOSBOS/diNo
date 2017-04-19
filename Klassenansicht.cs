using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace diNo
{
  public partial class Klassenansicht : BasisForm
  {
    private Schueler schueler=null;
    private SchuelerverwaltungController verwaltungController;
    private Brief frmBrief=null;

    public Klassenansicht()
    {
      InitializeComponent();
      this.olvColumnBezeichnung.AspectGetter = KlassenTreeViewController.SelectValueCol1;

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
          this.notenbogen1.Schueler = schueler;
          this.userControlFPAundSeminar1.Schueler = schueler;
          userControlHjLeistung1.Schueler = schueler;

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
            if (Zugriff.Instance.HatRolle(Rolle.Admin))
              userControlAdministration1.Schueler = schueler;
          }
        }
      }

      btnPrint.Enabled = Zugriff.Instance.HatVerwaltungsrechte || schueler != null;
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
            new LeseNotenAusExcelAlt(fileName, notenReader_OnStatusChange, Properties.Settings.Default.sicherungsverzeichnis);
          }
          else
          {
            new LeseNotenAusExcel(fileName, notenReader_OnStatusChange, Properties.Settings.Default.sicherungsverzeichnis);
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

    private void btnPrint_Click(object sender, EventArgs e)
    { 
      new ReportNotendruck(SelectedObjects(),"diNo.rptNotenbogen.rdlc").Show();
    }

    // liefert den angeklickten Schüler, oder eine Liste von Klassen (nur für Admins)
    public Object SelectedObjects()
    {
      if (Zugriff.Instance.HatVerwaltungsrechte)
      {
        var obj = treeListView1.SelectedObjects; // Multiselect im Klassenbereich
        if (obj.Count>0 && obj[0] is Klasse)
        {
          return (ArrayList)obj;          
        }
      }
      return schueler;
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
    }

    public void RefreshVorkommnisse()
    {          
      userControlVorkommnisse1.RefreshVorkommnisse();
    }

    private void chkNurAktive_Click(object sender, EventArgs e)
    {
      RefreshTreeView();
    }
  }
}
