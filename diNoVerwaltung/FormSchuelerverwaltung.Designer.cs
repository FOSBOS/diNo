namespace diNoVerwaltung
{
  partial class FormSchuelerverwaltung
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.treeListView1 = new BrightIdeasSoftware.TreeListView();
      this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvSpalteLegasthenie = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvSpalteWahlpflichtfach = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvSpalteReli = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvSpalteAustrittsdatum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.klasseWechselnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.abmeldungToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.label1 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.olvSpalteFremdsprache2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // treeListView1
      // 
      this.treeListView1.AllColumns.Add(this.olvColumn1);
      this.treeListView1.AllColumns.Add(this.olvSpalteWahlpflichtfach);
      this.treeListView1.AllColumns.Add(this.olvSpalteFremdsprache2);
      this.treeListView1.AllColumns.Add(this.olvSpalteReli);
      this.treeListView1.AllColumns.Add(this.olvSpalteLegasthenie);
      this.treeListView1.AllColumns.Add(this.olvSpalteAustrittsdatum);
      this.treeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeListView1.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvSpalteWahlpflichtfach,
            this.olvSpalteFremdsprache2,
            this.olvSpalteReli,
            this.olvSpalteLegasthenie,
            this.olvSpalteAustrittsdatum});
      this.treeListView1.GridLines = true;
      this.treeListView1.IsSimpleDragSource = true;
      this.treeListView1.IsSimpleDropSink = true;
      this.treeListView1.Location = new System.Drawing.Point(12, 62);
      this.treeListView1.MultiSelect = false;
      this.treeListView1.Name = "treeListView1";
      this.treeListView1.OwnerDraw = true;
      this.treeListView1.ShowGroups = false;
      this.treeListView1.Size = new System.Drawing.Size(785, 447);
      this.treeListView1.TabIndex = 6;
      this.treeListView1.UseCompatibleStateImageBehavior = false;
      this.treeListView1.View = System.Windows.Forms.View.Details;
      this.treeListView1.VirtualMode = true;
      this.treeListView1.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.listViewComplex_CellEditFinishing);
      this.treeListView1.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.listViewComplex_CellEditStarting);
      this.treeListView1.ModelCanDrop += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.treeListView1_ModelCanDrop);
      this.treeListView1.ModelDropped += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.treeListView1_ModelDropped);
      // 
      // olvColumn1
      // 
      this.olvColumn1.AspectName = "";
      this.olvColumn1.IsEditable = false;
      this.olvColumn1.Text = "Bezeichnung";
      this.olvColumn1.Width = 221;
      // 
      // olvSpalteLegasthenie
      // 
      this.olvSpalteLegasthenie.CheckBoxes = true;
      this.olvSpalteLegasthenie.Text = "Legasthenie";
      this.olvSpalteLegasthenie.Width = 80;
      // 
      // olvSpalteWahlpflichtfach
      // 
      this.olvSpalteWahlpflichtfach.Text = "Wahlpflichtfach";
      this.olvSpalteWahlpflichtfach.Width = 100;
      // 
      // olvSpalteReli
      // 
      this.olvSpalteReli.Text = "Reli oder Ethik";
      this.olvSpalteReli.Width = 100;
      // 
      // olvSpalteAustrittsdatum
      // 
      this.olvSpalteAustrittsdatum.Text = "Austrittsdatum";
      this.olvSpalteAustrittsdatum.Width = 80;
      // 
      // klasseWechselnToolStripMenuItem
      // 
      this.klasseWechselnToolStripMenuItem.Name = "klasseWechselnToolStripMenuItem";
      this.klasseWechselnToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
      // 
      // abmeldungToolStripMenuItem
      // 
      this.abmeldungToolStripMenuItem.Name = "abmeldungToolStripMenuItem";
      this.abmeldungToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
      this.label1.Location = new System.Drawing.Point(78, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(213, 29);
      this.label1.TabIndex = 1;
      this.label1.Text = "diNo Schülerdaten";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::diNoVerwaltung.Properties.Resources.stegosaurus_304011__180_klein;
      this.pictureBox1.Location = new System.Drawing.Point(12, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(60, 43);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // olvSpalteFremdsprache2
      // 
      this.olvSpalteFremdsprache2.Text = "Fremdsprache2";
      this.olvSpalteFremdsprache2.Width = 100;
      // 
      // FormSchuelerverwaltung
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(809, 521);
      this.Controls.Add(this.treeListView1);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.pictureBox1);
      this.Name = "FormSchuelerverwaltung";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label1;
    private BrightIdeasSoftware.TreeListView treeListView1;
    private BrightIdeasSoftware.OLVColumn olvColumn1;
    private BrightIdeasSoftware.OLVColumn olvSpalteLegasthenie;
    private BrightIdeasSoftware.OLVColumn olvSpalteWahlpflichtfach;
    private BrightIdeasSoftware.OLVColumn olvSpalteReli;
    private System.Windows.Forms.ToolStripMenuItem klasseWechselnToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem abmeldungToolStripMenuItem;
    private BrightIdeasSoftware.OLVColumn olvSpalteAustrittsdatum;
    private BrightIdeasSoftware.OLVColumn olvSpalteFremdsprache2;
  }
}

