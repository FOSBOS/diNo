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
            this.tabPageNoten = new System.Windows.Forms.TabPage();
            this.notenbogen1 = new diNo.Notenbogen();
            this.tabPageVorkommnisse = new System.Windows.Forms.TabPage();
            this.userControlVorkommnisse1 = new diNo.UserControlVorkommnisse();
            this.klasseLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBoxImage = new System.Windows.Forms.PictureBox();
            this.treeListView1 = new BrightIdeasSoftware.TreeListView();
            this.olvColumnBezeichnung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabControl1.SuspendLayout();
            this.tabPageStammdaten.SuspendLayout();
            this.tabPageNoten.SuspendLayout();
            this.tabPageVorkommnisse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageStammdaten);
            this.tabControl1.Controls.Add(this.tabPageNoten);
            this.tabControl1.Controls.Add(this.tabPageVorkommnisse);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(275, 126);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(12, 7);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1078, 507);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageStammdaten
            // 
            this.tabPageStammdaten.Controls.Add(this.userControlSchueleransicht1);
            this.tabPageStammdaten.Location = new System.Drawing.Point(4, 33);
            this.tabPageStammdaten.Name = "tabPageStammdaten";
            this.tabPageStammdaten.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStammdaten.Size = new System.Drawing.Size(1070, 470);
            this.tabPageStammdaten.TabIndex = 0;
            this.tabPageStammdaten.Text = "Schülerdaten";
            this.tabPageStammdaten.UseVisualStyleBackColor = true;
            // 
            // userControlSchueleransicht1
            // 
            this.userControlSchueleransicht1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlSchueleransicht1.Location = new System.Drawing.Point(3, 3);
            this.userControlSchueleransicht1.Name = "userControlSchueleransicht1";
            this.userControlSchueleransicht1.Schueler = null;
            this.userControlSchueleransicht1.Size = new System.Drawing.Size(1064, 464);
            this.userControlSchueleransicht1.TabIndex = 7;
            // 
            // tabPageNoten
            // 
            this.tabPageNoten.Controls.Add(this.notenbogen1);
            this.tabPageNoten.Location = new System.Drawing.Point(4, 33);
            this.tabPageNoten.Name = "tabPageNoten";
            this.tabPageNoten.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNoten.Size = new System.Drawing.Size(1070, 470);
            this.tabPageNoten.TabIndex = 1;
            this.tabPageNoten.Text = "Notenbogen";
            this.tabPageNoten.UseVisualStyleBackColor = true;
            // 
            // notenbogen1
            // 
            this.notenbogen1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notenbogen1.Location = new System.Drawing.Point(3, 3);
            this.notenbogen1.Name = "notenbogen1";
            this.notenbogen1.Schueler = null;
            this.notenbogen1.Size = new System.Drawing.Size(1064, 464);
            this.notenbogen1.TabIndex = 0;
            // 
            // tabPageVorkommnisse
            // 
            this.tabPageVorkommnisse.Controls.Add(this.userControlVorkommnisse1);
            this.tabPageVorkommnisse.Location = new System.Drawing.Point(4, 33);
            this.tabPageVorkommnisse.Name = "tabPageVorkommnisse";
            this.tabPageVorkommnisse.Size = new System.Drawing.Size(1070, 470);
            this.tabPageVorkommnisse.TabIndex = 2;
            this.tabPageVorkommnisse.Text = "Vorkommnisse";
            this.tabPageVorkommnisse.UseVisualStyleBackColor = true;
            // 
            // userControlVorkommnisse1
            // 
            this.userControlVorkommnisse1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userControlVorkommnisse1.Location = new System.Drawing.Point(0, 0);
            this.userControlVorkommnisse1.Name = "userControlVorkommnisse1";
            this.userControlVorkommnisse1.Schueler = null;
            this.userControlVorkommnisse1.Size = new System.Drawing.Size(1070, 470);
            this.userControlVorkommnisse1.TabIndex = 0;
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
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxImage.Image")));
            this.pictureBoxImage.Location = new System.Drawing.Point(275, 12);
            this.pictureBoxImage.Name = "pictureBoxImage";
            this.pictureBoxImage.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxImage.TabIndex = 32;
            this.pictureBoxImage.TabStop = false;
            // 
            // treeListView1
            // 
            this.treeListView1.AllColumns.Add(this.olvColumnBezeichnung);
            this.treeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnBezeichnung});
            this.treeListView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeListView1.Location = new System.Drawing.Point(12, 12);
            this.treeListView1.Name = "treeListView1";
            this.treeListView1.OwnerDraw = true;
            this.treeListView1.ShowGroups = false;
            this.treeListView1.Size = new System.Drawing.Size(240, 617);
            this.treeListView1.TabIndex = 5;
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
            // Klassenansicht
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1365, 645);
            this.Controls.Add(this.pictureBoxImage);
            this.Controls.Add(this.klasseLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.treeListView1);
            this.Name = "Klassenansicht";
            this.Text = "Klassenansicht";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Klassenansicht_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageStammdaten.ResumeLayout(false);
            this.tabPageNoten.ResumeLayout(false);
            this.tabPageVorkommnisse.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private BrightIdeasSoftware.TreeListView treeListView1;
    private BrightIdeasSoftware.OLVColumn olvColumnBezeichnung;
    private UserControlSchueleransicht userControlSchueleransicht1;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPageStammdaten;
    private System.Windows.Forms.TabPage tabPageNoten;
    private System.Windows.Forms.TabPage tabPageVorkommnisse;
    private UserControlVorkommnisse userControlVorkommnisse1;
    private Notenbogen notenbogen1;
        private System.Windows.Forms.Label klasseLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBoxImage;
    }
}