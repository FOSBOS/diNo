using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace diNo
{
  public partial class Klassenansicht : Form
  {
        private Schueler schueler=null;
        public Klassenansicht()
        {
          InitializeComponent();

          this.olvColumnBezeichnung.AspectGetter = KlassenTreeViewController.SelectValueCol1;
        }

    private void treeListView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      schueler = treeListView1.SelectedObject as Schueler;
      if (schueler != null)
      {
        this.userControlSchueleransicht1.Schueler = schueler;
        this.userControlVorkommnisse1.Schueler = schueler;
        this.notenbogen1.Schueler = schueler;
        this.userControlFPAundSeminar1.Schueler = schueler;

        nameLabel.Text = schueler.NameVorname;
        klasseLabel.Text = schueler.getKlasse.Bezeichnung;
        Image imageToUse = schueler.Data.Geschlecht == "W" ? global::diNo.Properties.Resources.avatarFrau : global::diNo.Properties.Resources.avatarMann;
        pictureBoxImage.Image = new Bitmap(imageToUse, pictureBoxImage.Size);
        btnBrief.Enabled = ! Zugriff.Instance.IsAdmin; // Admins haben kein Lehrerobjekt und können daher keine Briefe schreiben
        btnPrint.Enabled = true;
        btnSave.Enabled = true;

      }
    }

    private void Klassenansicht_Load(object sender, EventArgs e)
    {
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
                    new LeseNotenAusExcel(fileName, notenReader_OnStatusChange);
                }
                Zugriff.Refresh(); // Noten neu laden
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
            new ReportNotenbogen(schueler);
        }

        private void btnBrief_Click(object sender, EventArgs e)
        {
            var b = new Brief(schueler);
            b.ShowDialog();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            var c = new NotenCheckForm();
             c.Show();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {   
            userControlFPAundSeminar1.DatenUebernehmen();         
            schueler.Save();            
        }
    }
}
