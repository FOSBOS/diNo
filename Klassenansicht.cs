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

    public Klassenansicht()
    {
      InitializeComponent();
      this.verwaltungController = new SchuelerverwaltungController(() => { Zugriff.Refresh(); this.treeListView1.Roots = Zugriff.Instance.Klassen; });
      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || Zugriff.Instance.lehrer.HatRolle(Rolle.Sekretariat))
      {
        this.treeListView1.IsSimpleDragSource = true;
        this.treeListView1.IsSimpleDropSink = true;
        this.treeListView1.ModelCanDrop += this.verwaltungController.treeListView1_ModelCanDrop;
        this.treeListView1.ModelDropped += this.verwaltungController.treeListView1_ModelDropped;
      }

      SchuelerverwaltungController.InitDateTimePicker(this.dateTimePicker1);
      SchuelerverwaltungController.FillCheckboxWahlpflichtfach(this.schueler, this.comboBoxWahlpflichtfach);
      SchuelerverwaltungController.FillCheckboxFremdsprache2(this.schueler, this.comboBoxFremdsprache2);
      SchuelerverwaltungController.FillCheckBoxReliOderEthik(this.schueler, this.comboBoxReliOderEthik);
      this.olvColumnBezeichnung.AspectGetter = KlassenTreeViewController.SelectValueCol1;
    }

    private void treeListView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      schueler = treeListView1.SelectedObject as Schueler;
      bool adminEnabled = schueler != null && (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || Zugriff.Instance.lehrer.HatRolle(Rolle.Sekretariat));
      this.tabPageAdmin.Enabled = adminEnabled;
      this.userControlKurszuordnungen1.Enabled = adminEnabled;
      this.dateTimePicker1.Enabled = adminEnabled;
      this.comboBoxFremdsprache2.Enabled = adminEnabled;
      this.comboBoxReliOderEthik.Enabled = adminEnabled;
      this.comboBoxWahlpflichtfach.Enabled = adminEnabled;
      this.checkBoxLegasthenie.Enabled = adminEnabled;

      if (schueler != null)
      {
        this.userControlSchueleransicht1.Schueler = schueler;
        this.userControlVorkommnisse1.Schueler = schueler;
        this.notenbogen1.Schueler = schueler;
        this.userControlFPAundSeminar1.Schueler = schueler;
        this.userControlKurszuordnungen1.Schueler = schueler;

        nameLabel.Text = schueler.NameVorname;
        klasseLabel.Text = schueler.KlassenBezeichnung;
        Image imageToUse = schueler.Data.Geschlecht == "W" ? global::diNo.Properties.Resources.avatarFrau : global::diNo.Properties.Resources.avatarMann;
        pictureBoxImage.Image = new Bitmap(imageToUse, pictureBoxImage.Size);
        btnBrief.Enabled = true;
        btnPrint.Enabled = true;

        btnSave.Enabled = Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.Seminarfach) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn ||
          schueler.BetreuerId == Zugriff.Instance.lehrer.Id ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpAWirtschaft) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && (schueler.Data.Ausbildungsrichtung == "W" || schueler.Data.Ausbildungsrichtung == "WVR") ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpASozial) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && schueler.Data.Ausbildungsrichtung == "S" ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpATechnik) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && schueler.Data.Ausbildungsrichtung == "T" ||
          Zugriff.Instance.lehrer.HatRolle(Rolle.FpAAgrar) && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf && (schueler.Data.Ausbildungsrichtung == "A");
        labelHinweise.Text = (schueler.IsLegastheniker ? "Legastheniker" : "");
        labelHinweise.ForeColor = Color.Red;

        this.dateTimePicker1.Value = !schueler.Data.IsAustrittsdatumNull() ? schueler.Data.Austrittsdatum : this.dateTimePicker1.MinDate;
        this.checkBoxLegasthenie.Checked = schueler.IsLegastheniker;
        switch (schueler.Fremdsprache2)
        {
          case "": this.comboBoxFremdsprache2.SelectedIndex = 0; break;
          case "F": this.comboBoxFremdsprache2.SelectedIndex = 1; break;
          default: throw new InvalidOperationException("Unbekannter Wert für Fremdsprache2" + schueler.Fremdsprache2);
        }

        switch (schueler.ReliOderEthik)
        {
          case "":
          case null: this.comboBoxReliOderEthik.SelectedIndex = 0; break;
          case "RK": this.comboBoxReliOderEthik.SelectedIndex = 1; break;
          case "EV": this.comboBoxReliOderEthik.SelectedIndex = 2; break;
          case "Eth": this.comboBoxReliOderEthik.SelectedIndex = 3; break;
          default: throw new InvalidOperationException("Unbekannter Wert für Religionskurs" + schueler.ReliOderEthik);
        }

        switch (schueler.Wahlpflichtfach)
        {
          case "": this.comboBoxWahlpflichtfach.SelectedIndex = 0; break;
          case "F": this.comboBoxWahlpflichtfach.SelectedIndex = 1; break;
          case "F-Wi":
          case "F3": this.comboBoxWahlpflichtfach.SelectedIndex = 2; break;
          case "Ku": this.comboBoxWahlpflichtfach.SelectedIndex = 3; break;
          case "WIn": this.comboBoxWahlpflichtfach.SelectedIndex = 4; break;
          default: throw new InvalidOperationException("Unbekannter Wert für Wahlpflichtfach" + schueler.Wahlpflichtfach);
        }
      }
      else
      {
        if (!Zugriff.Instance.IstNurNormalerLehrer)
          btnPrint.Enabled = true;

        this.dateTimePicker1.Value = this.dateTimePicker1.MinDate;
        this.comboBoxFremdsprache2.SelectedIndex = 0;
        this.comboBoxReliOderEthik.SelectedIndex = 0;
        this.comboBoxWahlpflichtfach.SelectedIndex = 0;
        this.checkBoxLegasthenie.Checked = false;
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
          var frmBrief = new Brief(this);
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

    private void btnAdministration_Click(object sender, EventArgs e)
    {
      if (this.schueler != null)
      {
        UserControlKurszuordnungen form = new UserControlKurszuordnungen(this.schueler);
      }
    }

    private void comboBoxWahlpflichtfach_SelectedValueChanged(object sender, EventArgs e)
    {
      SchuelerverwaltungController.SetValueWahlpflichtfach(this.schueler, this.comboBoxWahlpflichtfach.SelectedItem);
      this.userControlKurszuordnungen1.InitKurse();
    }

    private void comboBoxFremdsprache2_SelectedValueChanged(object sender, EventArgs e)
    {
      SchuelerverwaltungController.SetValueFremdsprache2(this.schueler, this.comboBoxFremdsprache2.SelectedItem);
      this.userControlKurszuordnungen1.InitKurse();
    }

    private void comboBoxReliOderEthik_SelectedValueChanged(object sender, EventArgs e)
    {
      SchuelerverwaltungController.SetValueReliOderEthik(this.schueler, this.comboBoxReliOderEthik.SelectedItem);
      this.userControlKurszuordnungen1.InitKurse();
    }

    private void checkBoxLegasthenie_CheckedChanged(object sender, EventArgs e)
    {
      SchuelerverwaltungController.SetValueLegasthenie(this.schueler, this.checkBoxLegasthenie.Checked);
    }

    private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
    {
      SchuelerverwaltungController.SetValueAustrittsdatum(this.schueler, this.dateTimePicker1.Value);
      this.userControlKurszuordnungen1.InitKurse();
    }
  }
}
