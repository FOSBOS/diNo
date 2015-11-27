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
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPageStammdaten = new System.Windows.Forms.TabPage();
      this.tabPageNoten = new System.Windows.Forms.TabPage();
      this.tabPageVorkommnisse = new System.Windows.Forms.TabPage();
      this.userControlSchueleransicht1 = new diNo.UserControlSchueleransicht();
      this.notenbogen1 = new diNo.Notenbogen();
      this.userControlVorkommnisse1 = new diNo.UserControlVorkommnisse();
      this.treeListView1 = new BrightIdeasSoftware.TreeListView();
      this.olvColumnBezeichnung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.tabControl1.SuspendLayout();
      this.tabPageStammdaten.SuspendLayout();
      this.tabPageNoten.SuspendLayout();
      this.tabPageVorkommnisse.SuspendLayout();
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
      this.tabControl1.Location = new System.Drawing.Point(225, 12);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(694, 546);
      this.tabControl1.TabIndex = 8;
      // 
      // tabPageStammdaten
      // 
      this.tabPageStammdaten.Controls.Add(this.userControlSchueleransicht1);
      this.tabPageStammdaten.Location = new System.Drawing.Point(4, 22);
      this.tabPageStammdaten.Name = "tabPageStammdaten";
      this.tabPageStammdaten.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageStammdaten.Size = new System.Drawing.Size(686, 520);
      this.tabPageStammdaten.TabIndex = 0;
      this.tabPageStammdaten.Text = "Schülerdaten";
      this.tabPageStammdaten.UseVisualStyleBackColor = true;
      // 
      // tabPageNoten
      // 
      this.tabPageNoten.Controls.Add(this.notenbogen1);
      this.tabPageNoten.Location = new System.Drawing.Point(4, 22);
      this.tabPageNoten.Name = "tabPageNoten";
      this.tabPageNoten.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageNoten.Size = new System.Drawing.Size(686, 520);
      this.tabPageNoten.TabIndex = 1;
      this.tabPageNoten.Text = "Notenbogen";
      this.tabPageNoten.UseVisualStyleBackColor = true;
      // 
      // tabPageVorkommnisse
      // 
      this.tabPageVorkommnisse.Controls.Add(this.userControlVorkommnisse1);
      this.tabPageVorkommnisse.Location = new System.Drawing.Point(4, 22);
      this.tabPageVorkommnisse.Name = "tabPageVorkommnisse";
      this.tabPageVorkommnisse.Size = new System.Drawing.Size(686, 520);
      this.tabPageVorkommnisse.TabIndex = 2;
      this.tabPageVorkommnisse.Text = "Vorkommnisse";
      this.tabPageVorkommnisse.UseVisualStyleBackColor = true;
      // 
      // userControlSchueleransicht1
      // 
      this.userControlSchueleransicht1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlSchueleransicht1.Location = new System.Drawing.Point(3, 3);
      this.userControlSchueleransicht1.Name = "userControlSchueleransicht1";
      this.userControlSchueleransicht1.Schueler = null;
      this.userControlSchueleransicht1.Size = new System.Drawing.Size(680, 514);
      this.userControlSchueleransicht1.TabIndex = 7;
      // 
      // notenbogen1
      // 
      this.notenbogen1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.notenbogen1.Location = new System.Drawing.Point(3, 3);
      this.notenbogen1.Name = "notenbogen1";
      this.notenbogen1.Schueler = null;
      this.notenbogen1.Size = new System.Drawing.Size(680, 514);
      this.notenbogen1.TabIndex = 0;
      // 
      // userControlVorkommnisse1
      // 
      this.userControlVorkommnisse1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.userControlVorkommnisse1.Location = new System.Drawing.Point(0, 0);
      this.userControlVorkommnisse1.Name = "userControlVorkommnisse1";
      this.userControlVorkommnisse1.Schueler = null;
      this.userControlVorkommnisse1.Size = new System.Drawing.Size(686, 520);
      this.userControlVorkommnisse1.TabIndex = 0;
      // 
      // treeListView1
      // 
      this.treeListView1.AllColumns.Add(this.olvColumnBezeichnung);
      this.treeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnBezeichnung});
      this.treeListView1.Location = new System.Drawing.Point(12, 12);
      this.treeListView1.Name = "treeListView1";
      this.treeListView1.OwnerDraw = true;
      this.treeListView1.ShowGroups = false;
      this.treeListView1.Size = new System.Drawing.Size(207, 546);
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
      this.olvColumnBezeichnung.Width = 204;
      // 
      // Klassenansicht
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(931, 570);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.treeListView1);
      this.Name = "Klassenansicht";
      this.Text = "Klassenansicht";
      this.Load += new System.EventHandler(this.Klassenansicht_Load);
      this.tabControl1.ResumeLayout(false);
      this.tabPageStammdaten.ResumeLayout(false);
      this.tabPageNoten.ResumeLayout(false);
      this.tabPageVorkommnisse.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
      this.ResumeLayout(false);

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
  }
}