using diNo.Xml.Mbstatistik;
using log4net;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class Klassenansicht : BasisForm
  {
    private Schueler schueler = null;
    private SchuelerverwaltungController verwaltungController;
    private Brief frmBrief = null;
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private List<Schueler> SuchListe;
    private int SuchIndex=-1;

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
      edSuchen.Visible = Zugriff.Instance.SiehtAlles;
      btnSuchen.Visible = Zugriff.Instance.SiehtAlles;
      lbSuchen.Visible = Zugriff.Instance.SiehtAlles;

      // Kontextmenü für das Drucken von Kurslisten erzeugen
      foreach (var k in Zugriff.Instance.eigeneKurse)
      {
        Fach f = k.getFach;
        if (f.Typ == FachTyp.WPF || f.Kuerzel == "K" || f.Kuerzel == "Ev" || f.Kuerzel == "Eth")
        {
          var itm = new ToolStripMenuItem("Kursliste " + k.Kursbezeichnung, null, druKursliste);
          itm.Tag = k;
          contextMenuPrint.Items.Add(itm);   
        }
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

      labelHinweise.Text = (schueler.IsLegastheniker ? "Notenschutz (Legasthenie)" : "") 
        + (schueler.IsLegastheniker && schueler.Data.LRSZuschlagMax > 0 ? ", " : "")
        + (schueler.Data.LRSZuschlagMax>0 ? "Zeitzuschlag von " + schueler.Data.LRSZuschlagMin +  "% bis " + schueler.Data.LRSZuschlagMax + "%" : "");
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
      this.treeListView1.ChildrenGetter = delegate (object x) { return ((Klasse)x).Schueler; };
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
          new LeseNotenAusExcel(fileName, notenReader_OnStatusChange);
        }

        if (schueler != null)
        {
          schueler = Zugriff.Instance.SchuelerRep.Find(schueler.Id); // neues Objekt setzen
          SetSchueler();
        }
        MessageBox.Show("Die Notendateien wurden übertragen.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    // bei Click auf den Druck-Button öffnet sich das Kontextmenü mit verschiedenen Berichten
    private void btnPrint_Click(object sender, EventArgs e)
    {
      contextMenuPrint.Show(btnPrint,20,40);
    }
    
    private void druKlassenliste_Click(object sender, EventArgs e)
    {
      
      var obj = treeListView1.SelectedObjects;
      if (obj.Count > 0 && obj[0] is Klasse)
      {
        Klasse k = (Klasse)obj[0];
        new ReportSchuelerdruck(k.Schueler, Bericht.Klassenliste).Show();
      }
      else
      {
        MessageBox.Show("Bitte erst eine Klasse auswählen.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    // WPF oder Relikurse, die über das Kontextmenü ausgedruckt werden können
    private void druKursliste(Object itm,EventArgs args)
    {      
      Kurs k = (Kurs)((ToolStripMenuItem)itm).Tag;
      new ReportSchuelerdruck(k.Schueler, Bericht.Klassenliste).Show();
    }


    private void druNotenbogen_Click(object sender, EventArgs e)
    {
      var obj = SelectedObjects();
      if (obj.Count == 0 || !Zugriff.Instance.HatVerwaltungsrechte && obj.Count > 1)
      {
        MessageBox.Show("Bitte erst einen Schüler auswählen.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      
      new ReportSchuelerdruck(obj, Bericht.Notenbogen).Show();
    }

    private void druLegastheniker_Click(object sender, EventArgs e)
    {
       List<Schueler> lst = new List<Schueler>();
       foreach (Schueler s in Zugriff.Instance.SchuelerRep.getList())
       {
        if (s.IsLegastheniker || s.Data.LRSZuschlagMax>0)
          lst.Add(s);
       }
      new ReportSchuelerdruck(lst, Bericht.Legastheniker).Show();
    }
    
    // nur vom Adminbereich aus aufrufbar
    public List<Klasse> SelectedKlassen()
    {
      var res = new List<Klasse>();
      var obj = treeListView1.SelectedObjects; // Multiselect im Klassenbereich
      if (obj.Count > 0 && obj[0] is Klasse)
      {
        foreach (Klasse k in obj)
          res.Add(k);
      }
      return res;
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
      else if (obj.Count > 0 && obj[0] is Klasse)
      {
        foreach (Klasse k in obj)
        {
          foreach (Schueler s in k.Schueler)
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
      else if (schueler != null)
        res.Add(schueler); // nur aktuell ausgewählter Schüler

      return res;
    }

    private void btnBrief_Click(object sender, EventArgs e)
    {
      if (frmBrief == null) frmBrief = new Brief(this);
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

    private void btnLNWabgeben_Click(object sender, EventArgs e)
    {
      new CopyLNW();
    }

    private void btnSuchen_Click(object sender, EventArgs e)
    {
      // klappt leider nicht direkt, weil nur geöffnete Klassen durchsucht werden
      // ListViewItem s = treeListView1.FindItemWithText(edSuchen.Text,true,0,true);
      string cmp = edSuchen.Text;
      int c = 0;      
      if (SuchListe == null)  SuchListe = Zugriff.Instance.SchuelerRep.getList();
      int max = SuchListe.Count;
      lbSuchen.ForeColor = Color.Black;

      do
      {
        SuchIndex++;
        if (SuchIndex >= max) SuchIndex = 0; // wieder von vorn anfangen
        c++; // Endlosschleife verhindern

        if (SuchListe[SuchIndex].Name.StartsWith(cmp, StringComparison.OrdinalIgnoreCase))
        {
          schueler = SuchListe[SuchIndex];
          SetSchueler();
          break;
        }
      }
      while (c < max);
      if (c>=max)
        lbSuchen.ForeColor = Color.Red;
    }

  }
}
