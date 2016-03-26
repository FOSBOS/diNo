using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace diNo
{
  public partial class Klassenansicht : BasisForm
  {
        private Schueler schueler=null;
        private Brief frmBrief=null;
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
        klasseLabel.Text = schueler.KlassenBezeichnung;
        Image imageToUse = schueler.Data.Geschlecht == "W" ? global::diNo.Properties.Resources.avatarFrau : global::diNo.Properties.Resources.avatarMann;
        pictureBoxImage.Image = new Bitmap(imageToUse, pictureBoxImage.Size);  
        btnBrief.Enabled = true;
        btnPrint.Enabled = true;
        
        btnSave.Enabled = Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || 
          Zugriff.Instance.lehrer.HatRolle(Rolle.Seminarfach) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn || 
          schueler.BetreuerId == Zugriff.Instance.lehrer.Id || 
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpAWirtschaft) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && (schueler.Data.Ausbildungsrichtung =="W" || schueler.Data.Ausbildungsrichtung == "WVR") ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpASozial) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && schueler.Data.Ausbildungsrichtung == "S" ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpATechnik) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && schueler.Data.Ausbildungsrichtung == "T" ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpAAgrar) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && (schueler.Data.Ausbildungsrichtung == "A");
        labelHinweise.Text = (schueler.IsLegastheniker ? "Legastheniker" : "");
        labelHinweise.ForeColor = Color.Red;
      }
      else if (!Zugriff.Instance.IstNurNormalerLehrer)      
        btnPrint.Enabled = true;
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
        btnNotenabgeben.Enabled = Zugriff.Instance.Sperre != Sperrtyp.Notenschluss || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin);
        btnAbidruck.Visible = !Zugriff.Instance.IstNurNormalerLehrer;
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
                this.treeListView1.Roots = Zugriff.Instance.Klassen; // Ansicht auffrischen
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
          Drucken(false);
        }

        private void Drucken(bool nurAbi)
        { 
          if (!Zugriff.Instance.IstNurNormalerLehrer)
          {
            var obj = treeListView1.SelectedObjects; // Multiselect im Klassenbereich
            if (obj.Count>0 && obj[0] is Klasse)
            {
              new ReportNotenbogen((ArrayList)obj, nurAbi).Show();              
              return;
            }
          }          
          if (schueler!= null) new ReportNotenbogen(schueler,nurAbi).Show();          
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

        private void btnSave_Click(object sender, EventArgs e)
        {   
          userControlFPAundSeminar1.DatenUebernehmen();
          if (!Zugriff.Instance.IstNurNormalerLehrer)
          {
            userControlSchueleransicht1.DatenUebernehmen();  
          }
          schueler.Save();            
        }

        public void RefreshVorkommnisse()
        {          
          userControlVorkommnisse1.RefreshVorkommnisse();
        }

    private void btnAbidruck_Click(object sender, EventArgs e)
    {
      Drucken(true);
    }
  }
}
