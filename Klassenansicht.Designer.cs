namespace diNo
{
  partial class Klassenansicht
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Klassenansicht));
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPageStammdaten = new System.Windows.Forms.TabPage();
      this.userControlSchueleransicht1 = new diNo.UserControlSchueleransicht();
      this.tabPageNotenbogen = new System.Windows.Forms.TabPage();
      this.userControlNotenbogen1 = new diNo.UserControlNotenbogen();
      this.tabPageFPASeminar = new System.Windows.Forms.TabPage();
      this.userControlFPAundSeminar1 = new diNo.UserControlFPAundSeminar();
      this.tabPageVorkommnisse = new System.Windows.Forms.TabPage();
      this.userControlVorkommnisse1 = new diNo.UserControlVorkommnisse();
      this.tabPageKurszuordnungen = new System.Windows.Forms.TabPage();
      this.userControlKurszuordnungen1 = new diNo.UserControlKurszuordnungen();
      this.label2 = new System.Windows.Forms.Label();
      this.tabPageSekretariat = new System.Windows.Forms.TabPage();
      this.userControlSekretariat1 = new diNo.UserControlSekretariat();
      this.tabPageAdministration = new System.Windows.Forms.TabPage();
      this.userControlAdministration1 = new diNo.UserControlAdministration();
      this.klasseLabel = new System.Windows.Forms.Label();
      this.nameLabel = new System.Windows.Forms.Label();
      this.treeListView1 = new BrightIdeasSoftware.TreeListView();
      this.olvColumnBezeichnung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.btnCheck = new System.Windows.Forms.Button();
      this.btnBrief = new System.Windows.Forms.Button();
      this.btnPrint = new System.Windows.Forms.Button();
      this.btnNotenabgeben = new System.Windows.Forms.Button();
      this.labelHinweise = new System.Windows.Forms.Label();
      this.pictureBoxImage = new System.Windows.Forms.PictureBox();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
      this.multiImageRenderer1 = new BrightIdeasSoftware.MultiImageRenderer();
      this.chkNurAktive = new System.Windows.Forms.CheckBox();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.lbTest = new System.Windows.Forms.Label();
      this.btnLNWabgeben = new System.Windows.Forms.Button();
      this.edSuchen = new System.Windows.Forms.TextBox();
      this.btnSuchen = new System.Windows.Forms.Button();
      this.lbSuchen = new System.Windows.Forms.Label();
      this.tabControl1.SuspendLayout();
      this.tabPageStammdaten.SuspendLayout();
      this.tabPageNotenbogen.SuspendLayout();
      this.tabPageFPASeminar.SuspendLayout();
      this.tabPageVorkommnisse.SuspendLayout();
      this.tabPageKurszuordnungen.SuspendLayout();
      this.tabPageSekretariat.SuspendLayout();
      this.tabPageAdministration.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.Images.SetKeyName(0, "Cancel.png");
      this.imageList1.Images.SetKeyName(1, "Ok.png");
      this.imageList1.Images.SetKeyName(2, "Edit.png");
      this.imageList1.Images.SetKeyName(3, "print.png");
      this.imageList1.Images.SetKeyName(4, "Save.png");
      this.imageList1.Images.SetKeyName(5, "Excel.png");
      this.imageList1.Images.SetKeyName(6, "Admin.png");
      this.imageList1.Images.SetKeyName(7, "Refresh.png");
      this.imageList1.Images.SetKeyName(8, "Ablage.png");
      this.imageList1.Images.SetKeyName(9, "lupe.jpg");
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.tabPageStammdaten);
      this.tabControl1.Controls.Add(this.tabPageNotenbogen);
      this.tabControl1.Controls.Add(this.tabPageFPASeminar);
      this.tabControl1.Controls.Add(this.tabPageVorkommnisse);
      this.tabControl1.Controls.Add(this.tabPageKurszuordnungen);
      this.tabControl1.Controls.Add(this.tabPageSekretariat);
      this.tabControl1.Controls.Add(this.tabPageAdministration);
      this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tabControl1.Location = new System.Drawing.Point(275, 133);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.Padding = new System.Drawing.Point(12, 7);
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(898, 487);
      this.tabControl1.TabIndex = 6;
      // 
      // tabPageStammdaten
      // 
      this.tabPageStammdaten.Controls.Add(this.userControlSchueleransicht1);
      this.tabPageStammdaten.Location = new System.Drawing.Point(4, 33);
      this.tabPageStammdaten.Name = "tabPageStammdaten";
      this.tabPageStammdaten.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageStammdaten.Size = new System.Drawing.Size(890, 450);
      this.tabPageStammdaten.TabIndex = 0;
      this.tabPageStammdaten.Text = "Schülerdaten";
      this.tabPageStammdaten.UseVisualStyleBackColor = true;
      // 
      // userControlSchueleransicht1
      // 
      this.userControlSchueleransicht1.AutoScroll = true;
      this.userControlSchueleransicht1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlSchueleransicht1.Location = new System.Drawing.Point(3, 3);
      this.userControlSchueleransicht1.Margin = new System.Windows.Forms.Padding(4);
      this.userControlSchueleransicht1.Name = "userControlSchueleransicht1";
      this.userControlSchueleransicht1.Schueler = null;
      this.userControlSchueleransicht1.Size = new System.Drawing.Size(884, 444);
      this.userControlSchueleransicht1.TabIndex = 0;
      // 
      // tabPageNotenbogen
      // 
      this.tabPageNotenbogen.Controls.Add(this.userControlNotenbogen1);
      this.tabPageNotenbogen.Location = new System.Drawing.Point(4, 33);
      this.tabPageNotenbogen.Name = "tabPageNotenbogen";
      this.tabPageNotenbogen.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageNotenbogen.Size = new System.Drawing.Size(890, 450);
      this.tabPageNotenbogen.TabIndex = 7;
      this.tabPageNotenbogen.Text = "Notenbogen";
      this.tabPageNotenbogen.UseVisualStyleBackColor = true;
      // 
      // userControlNotenbogen1
      // 
      this.userControlNotenbogen1.AutoScroll = true;
      this.userControlNotenbogen1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlNotenbogen1.Location = new System.Drawing.Point(3, 3);
      this.userControlNotenbogen1.Margin = new System.Windows.Forms.Padding(4);
      this.userControlNotenbogen1.Name = "userControlNotenbogen1";
      this.userControlNotenbogen1.Schueler = null;
      this.userControlNotenbogen1.Size = new System.Drawing.Size(884, 444);
      this.userControlNotenbogen1.TabIndex = 0;
      // 
      // tabPageFPASeminar
      // 
      this.tabPageFPASeminar.Controls.Add(this.userControlFPAundSeminar1);
      this.tabPageFPASeminar.Location = new System.Drawing.Point(4, 33);
      this.tabPageFPASeminar.Name = "tabPageFPASeminar";
      this.tabPageFPASeminar.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageFPASeminar.Size = new System.Drawing.Size(890, 450);
      this.tabPageFPASeminar.TabIndex = 3;
      this.tabPageFPASeminar.Text = "FPA / Seminar";
      this.tabPageFPASeminar.UseVisualStyleBackColor = true;
      // 
      // userControlFPAundSeminar1
      // 
      this.userControlFPAundSeminar1.AutoScroll = true;
      this.userControlFPAundSeminar1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlFPAundSeminar1.Location = new System.Drawing.Point(3, 3);
      this.userControlFPAundSeminar1.Margin = new System.Windows.Forms.Padding(4);
      this.userControlFPAundSeminar1.Name = "userControlFPAundSeminar1";
      this.userControlFPAundSeminar1.Schueler = null;
      this.userControlFPAundSeminar1.Size = new System.Drawing.Size(884, 444);
      this.userControlFPAundSeminar1.TabIndex = 0;
      // 
      // tabPageVorkommnisse
      // 
      this.tabPageVorkommnisse.Controls.Add(this.userControlVorkommnisse1);
      this.tabPageVorkommnisse.Location = new System.Drawing.Point(4, 33);
      this.tabPageVorkommnisse.Name = "tabPageVorkommnisse";
      this.tabPageVorkommnisse.Size = new System.Drawing.Size(890, 450);
      this.tabPageVorkommnisse.TabIndex = 2;
      this.tabPageVorkommnisse.Text = "Vorkommnisse";
      this.tabPageVorkommnisse.UseVisualStyleBackColor = true;
      // 
      // userControlVorkommnisse1
      // 
      this.userControlVorkommnisse1.AutoScroll = true;
      this.userControlVorkommnisse1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlVorkommnisse1.Location = new System.Drawing.Point(0, 0);
      this.userControlVorkommnisse1.Margin = new System.Windows.Forms.Padding(4);
      this.userControlVorkommnisse1.Name = "userControlVorkommnisse1";
      this.userControlVorkommnisse1.Schueler = null;
      this.userControlVorkommnisse1.Size = new System.Drawing.Size(890, 450);
      this.userControlVorkommnisse1.TabIndex = 0;
      // 
      // tabPageKurszuordnungen
      // 
      this.tabPageKurszuordnungen.Controls.Add(this.userControlKurszuordnungen1);
      this.tabPageKurszuordnungen.Controls.Add(this.label2);
      this.tabPageKurszuordnungen.Location = new System.Drawing.Point(4, 33);
      this.tabPageKurszuordnungen.Name = "tabPageKurszuordnungen";
      this.tabPageKurszuordnungen.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageKurszuordnungen.Size = new System.Drawing.Size(890, 450);
      this.tabPageKurszuordnungen.TabIndex = 4;
      this.tabPageKurszuordnungen.Text = "Kurszuordnungen";
      this.tabPageKurszuordnungen.UseVisualStyleBackColor = true;
      // 
      // userControlKurszuordnungen1
      // 
      this.userControlKurszuordnungen1.AutoScroll = true;
      this.userControlKurszuordnungen1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlKurszuordnungen1.Location = new System.Drawing.Point(3, 3);
      this.userControlKurszuordnungen1.Margin = new System.Windows.Forms.Padding(4);
      this.userControlKurszuordnungen1.Name = "userControlKurszuordnungen1";
      this.userControlKurszuordnungen1.Schueler = null;
      this.userControlKurszuordnungen1.Size = new System.Drawing.Size(884, 444);
      this.userControlKurszuordnungen1.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(304, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(0, 17);
      this.label2.TabIndex = 2;
      // 
      // tabPageSekretariat
      // 
      this.tabPageSekretariat.Controls.Add(this.userControlSekretariat1);
      this.tabPageSekretariat.Location = new System.Drawing.Point(4, 33);
      this.tabPageSekretariat.Name = "tabPageSekretariat";
      this.tabPageSekretariat.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageSekretariat.Size = new System.Drawing.Size(890, 450);
      this.tabPageSekretariat.TabIndex = 8;
      this.tabPageSekretariat.Text = "Sekretariat";
      this.tabPageSekretariat.UseVisualStyleBackColor = true;
      // 
      // userControlSekretariat1
      // 
      this.userControlSekretariat1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlSekretariat1.Location = new System.Drawing.Point(3, 3);
      this.userControlSekretariat1.Margin = new System.Windows.Forms.Padding(4);
      this.userControlSekretariat1.Name = "userControlSekretariat1";
      this.userControlSekretariat1.Schueler = null;
      this.userControlSekretariat1.Size = new System.Drawing.Size(884, 444);
      this.userControlSekretariat1.TabIndex = 0;
      // 
      // tabPageAdministration
      // 
      this.tabPageAdministration.Controls.Add(this.userControlAdministration1);
      this.tabPageAdministration.Location = new System.Drawing.Point(4, 33);
      this.tabPageAdministration.Name = "tabPageAdministration";
      this.tabPageAdministration.Size = new System.Drawing.Size(890, 450);
      this.tabPageAdministration.TabIndex = 5;
      this.tabPageAdministration.Text = "Administration";
      this.tabPageAdministration.UseVisualStyleBackColor = true;
      // 
      // userControlAdministration1
      // 
      this.userControlAdministration1.AutoScroll = true;
      this.userControlAdministration1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlAdministration1.Location = new System.Drawing.Point(0, 0);
      this.userControlAdministration1.Margin = new System.Windows.Forms.Padding(4);
      this.userControlAdministration1.Name = "userControlAdministration1";
      this.userControlAdministration1.Schueler = null;
      this.userControlAdministration1.Size = new System.Drawing.Size(890, 450);
      this.userControlAdministration1.TabIndex = 0;
      // 
      // klasseLabel
      // 
      this.klasseLabel.AutoSize = true;
      this.klasseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.klasseLabel.Location = new System.Drawing.Point(377, 43);
      this.klasseLabel.Name = "klasseLabel";
      this.klasseLabel.Size = new System.Drawing.Size(56, 20);
      this.klasseLabel.TabIndex = 10;
      this.klasseLabel.Text = "Klasse";
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
      this.nameLabel.Location = new System.Drawing.Point(375, 12);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(90, 31);
      this.nameLabel.TabIndex = 9;
      this.nameLabel.Text = "Name";
      // 
      // treeListView1
      // 
      this.treeListView1.AllColumns.Add(this.olvColumnBezeichnung);
      this.treeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnBezeichnung});
      this.treeListView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.treeListView1.HideSelection = false;
      this.treeListView1.Location = new System.Drawing.Point(12, 87);
      this.treeListView1.Name = "treeListView1";
      this.treeListView1.OwnerDraw = true;
      this.treeListView1.ShowGroups = false;
      this.treeListView1.Size = new System.Drawing.Size(240, 529);
      this.treeListView1.TabIndex = 0;
      this.treeListView1.UseCompatibleStateImageBehavior = false;
      this.treeListView1.View = System.Windows.Forms.View.Details;
      this.treeListView1.VirtualMode = true;
      this.treeListView1.SelectedIndexChanged += new System.EventHandler(this.treeListView1_SelectedIndexChanged);
      // 
      // olvColumnBezeichnung
      // 
      this.olvColumnBezeichnung.Hideable = false;
      this.olvColumnBezeichnung.IsEditable = false;
      this.olvColumnBezeichnung.Text = "Bezeichnung";
      this.olvColumnBezeichnung.Width = 191;
      // 
      // btnCheck
      // 
      this.btnCheck.ImageIndex = 1;
      this.btnCheck.ImageList = this.imageList1;
      this.btnCheck.Location = new System.Drawing.Point(519, 87);
      this.btnCheck.Name = "btnCheck";
      this.btnCheck.Size = new System.Drawing.Size(40, 40);
      this.btnCheck.TabIndex = 5;
      this.toolTipButtons.SetToolTip(this.btnCheck, "Notenbogen auf Vollständigkeit, Gefährdungen und Vorrücken prüfen");
      this.btnCheck.UseVisualStyleBackColor = true;
      this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
      // 
      // btnBrief
      // 
      this.btnBrief.Enabled = false;
      this.btnBrief.ImageIndex = 2;
      this.btnBrief.ImageList = this.imageList1;
      this.btnBrief.Location = new System.Drawing.Point(473, 88);
      this.btnBrief.Name = "btnBrief";
      this.btnBrief.Size = new System.Drawing.Size(40, 40);
      this.btnBrief.TabIndex = 4;
      this.toolTipButtons.SetToolTip(this.btnBrief, "Briefe für Nachtermine und Verweise erstellen");
      this.btnBrief.UseVisualStyleBackColor = true;
      this.btnBrief.Click += new System.EventHandler(this.btnBrief_Click);
      // 
      // btnPrint
      // 
      this.btnPrint.Enabled = false;
      this.btnPrint.ImageIndex = 3;
      this.btnPrint.ImageList = this.imageList1;
      this.btnPrint.Location = new System.Drawing.Point(427, 88);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(40, 40);
      this.btnPrint.TabIndex = 3;
      this.toolTipButtons.SetToolTip(this.btnPrint, "Notenbogen drucken");
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // btnNotenabgeben
      // 
      this.btnNotenabgeben.ImageIndex = 5;
      this.btnNotenabgeben.ImageList = this.imageList1;
      this.btnNotenabgeben.Location = new System.Drawing.Point(282, 88);
      this.btnNotenabgeben.Name = "btnNotenabgeben";
      this.btnNotenabgeben.Size = new System.Drawing.Size(40, 40);
      this.btnNotenabgeben.TabIndex = 1;
      this.toolTipButtons.SetToolTip(this.btnNotenabgeben, "Notendateien abgeben");
      this.btnNotenabgeben.UseVisualStyleBackColor = true;
      this.btnNotenabgeben.Click += new System.EventHandler(this.btnNotenabgeben_Click);
      // 
      // labelHinweise
      // 
      this.labelHinweise.AutoSize = true;
      this.labelHinweise.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelHinweise.Location = new System.Drawing.Point(569, 88);
      this.labelHinweise.Name = "labelHinweise";
      this.labelHinweise.Size = new System.Drawing.Size(509, 34);
      this.labelHinweise.TabIndex = 36;
      this.labelHinweise.Text = "Notendateien können ab sofort über die Excel-Schaltfläche abgegeben werden.\r\nÜber" +
    " den Stift können z.B. Verweise gedruckt werden.\r\n";
      // 
      // pictureBoxImage
      // 
      this.pictureBoxImage.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxImage.Image")));
      this.pictureBoxImage.Location = new System.Drawing.Point(282, 12);
      this.pictureBoxImage.Name = "pictureBoxImage";
      this.pictureBoxImage.Size = new System.Drawing.Size(70, 70);
      this.pictureBoxImage.TabIndex = 32;
      this.pictureBoxImage.TabStop = false;
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
      this.statusStrip1.Location = new System.Drawing.Point(0, 623);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(1185, 22);
      this.statusStrip1.TabIndex = 38;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // toolStripStatusLabel1
      // 
      this.toolStripStatusLabel1.MergeAction = System.Windows.Forms.MergeAction.Replace;
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
      this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
      // 
      // chkNurAktive
      // 
      this.chkNurAktive.AutoSize = true;
      this.chkNurAktive.Checked = true;
      this.chkNurAktive.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkNurAktive.Location = new System.Drawing.Point(12, 64);
      this.chkNurAktive.Name = "chkNurAktive";
      this.chkNurAktive.Size = new System.Drawing.Size(185, 17);
      this.chkNurAktive.TabIndex = 40;
      this.chkNurAktive.Text = "Abgemeldete Schüler ausblenden";
      this.chkNurAktive.UseVisualStyleBackColor = true;
      this.chkNurAktive.Click += new System.EventHandler(this.chkNurAktive_Click);
      // 
      // btnRefresh
      // 
      this.btnRefresh.ImageIndex = 7;
      this.btnRefresh.ImageList = this.imageList1;
      this.btnRefresh.Location = new System.Drawing.Point(381, 88);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(40, 40);
      this.btnRefresh.TabIndex = 41;
      this.toolTipButtons.SetToolTip(this.btnRefresh, "Daten neu laden");
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
      // 
      // lbTest
      // 
      this.lbTest.AutoSize = true;
      this.lbTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.lbTest.Location = new System.Drawing.Point(796, 22);
      this.lbTest.Name = "lbTest";
      this.lbTest.Size = new System.Drawing.Size(282, 46);
      this.lbTest.TabIndex = 42;
      this.lbTest.Text = "Testdatenbank";
      this.lbTest.Visible = false;
      // 
      // btnLNWabgeben
      // 
      this.btnLNWabgeben.ImageIndex = 8;
      this.btnLNWabgeben.ImageList = this.imageList1;
      this.btnLNWabgeben.Location = new System.Drawing.Point(328, 88);
      this.btnLNWabgeben.Name = "btnLNWabgeben";
      this.btnLNWabgeben.Size = new System.Drawing.Size(40, 40);
      this.btnLNWabgeben.TabIndex = 43;
      this.toolTipButtons.SetToolTip(this.btnLNWabgeben, "Angabe und Lösung von LNW abgeben");
      this.btnLNWabgeben.UseVisualStyleBackColor = true;
      this.btnLNWabgeben.Click += new System.EventHandler(this.btnLNWabgeben_Click);
      // 
      // edSuchen
      // 
      this.edSuchen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edSuchen.Location = new System.Drawing.Point(12, 30);
      this.edSuchen.Name = "edSuchen";
      this.edSuchen.Size = new System.Drawing.Size(194, 22);
      this.edSuchen.TabIndex = 44;
      // 
      // btnSuchen
      // 
      this.btnSuchen.ImageKey = "lupe.jpg";
      this.btnSuchen.ImageList = this.imageList1;
      this.btnSuchen.Location = new System.Drawing.Point(211, 14);
      this.btnSuchen.Name = "btnSuchen";
      this.btnSuchen.Size = new System.Drawing.Size(41, 40);
      this.btnSuchen.TabIndex = 45;
      this.btnSuchen.UseVisualStyleBackColor = true;
      this.btnSuchen.Click += new System.EventHandler(this.btnSuchen_Click);
      // 
      // lbSuchen
      // 
      this.lbSuchen.AutoSize = true;
      this.lbSuchen.Location = new System.Drawing.Point(9, 14);
      this.lbSuchen.Name = "lbSuchen";
      this.lbSuchen.Size = new System.Drawing.Size(81, 13);
      this.lbSuchen.TabIndex = 46;
      this.lbSuchen.Text = "Schüler suchen";
      // 
      // Klassenansicht
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.ClientSize = new System.Drawing.Size(1185, 645);
      this.Controls.Add(this.lbSuchen);
      this.Controls.Add(this.btnSuchen);
      this.Controls.Add(this.edSuchen);
      this.Controls.Add(this.btnLNWabgeben);
      this.Controls.Add(this.lbTest);
      this.Controls.Add(this.btnRefresh);
      this.Controls.Add(this.chkNurAktive);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.btnCheck);
      this.Controls.Add(this.labelHinweise);
      this.Controls.Add(this.btnBrief);
      this.Controls.Add(this.btnPrint);
      this.Controls.Add(this.btnNotenabgeben);
      this.Controls.Add(this.pictureBoxImage);
      this.Controls.Add(this.klasseLabel);
      this.Controls.Add(this.nameLabel);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.treeListView1);
      this.Name = "Klassenansicht";
      this.Text = "diNo";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.Klassenansicht_Load);
      this.tabControl1.ResumeLayout(false);
      this.tabPageStammdaten.ResumeLayout(false);
      this.tabPageNotenbogen.ResumeLayout(false);
      this.tabPageFPASeminar.ResumeLayout(false);
      this.tabPageVorkommnisse.ResumeLayout(false);
      this.tabPageKurszuordnungen.ResumeLayout(false);
      this.tabPageKurszuordnungen.PerformLayout();
      this.tabPageSekretariat.ResumeLayout(false);
      this.tabPageAdministration.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private BrightIdeasSoftware.TreeListView treeListView1;
    private BrightIdeasSoftware.OLVColumn olvColumnBezeichnung;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPageStammdaten;
    private System.Windows.Forms.Label klasseLabel;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.PictureBox pictureBoxImage;
    private System.Windows.Forms.TabPage tabPageVorkommnisse;
    private System.Windows.Forms.Button btnNotenabgeben;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Button btnBrief;
    private System.Windows.Forms.Label labelHinweise;
    private System.Windows.Forms.Button btnCheck;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    private System.Windows.Forms.TabPage tabPageFPASeminar;
    private UserControlFPAundSeminar userControlFPAundSeminar1;
    private UserControlSchueleransicht userControlSchueleransicht1;
    private UserControlVorkommnisse userControlVorkommnisse1;
    private System.Windows.Forms.TabPage tabPageKurszuordnungen;
    private BrightIdeasSoftware.MultiImageRenderer multiImageRenderer1;
    private UserControlKurszuordnungen userControlKurszuordnungen1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox chkNurAktive;
    private System.Windows.Forms.TabPage tabPageAdministration;
    private UserControlAdministration userControlAdministration1;
    private System.Windows.Forms.TabPage tabPageNotenbogen;
    private UserControlNotenbogen userControlNotenbogen1;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.TabPage tabPageSekretariat;
    private UserControlSekretariat userControlSekretariat1;
    private System.Windows.Forms.Label lbTest;
    private System.Windows.Forms.Button btnLNWabgeben;
    private System.Windows.Forms.TextBox edSuchen;
    private System.Windows.Forms.Button btnSuchen;
    private System.Windows.Forms.Label lbSuchen;
  }
}