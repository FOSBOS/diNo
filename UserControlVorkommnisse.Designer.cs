namespace diNo
{
  partial class UserControlVorkommnisse
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

    #region Vom Komponenten-Designer generierter Code

    /// <summary> 
    /// Erforderliche Methode für die Designerunterstützung. 
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.lblDatum = new System.Windows.Forms.Label();
      this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
      this.lblArt = new System.Windows.Forms.Label();
      this.comboBoxArt = new System.Windows.Forms.ComboBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnNeuesVorkommnis = new System.Windows.Forms.Button();
      this.lblSchuelername = new System.Windows.Forms.Label();
      this.objectListViewVorkommnisse = new BrightIdeasSoftware.ObjectListView();
      this.olvColumnDatum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnArt = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnBemerkung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      ((System.ComponentModel.ISupportInitialize)(this.objectListViewVorkommnisse)).BeginInit();
      this.SuspendLayout();
      // 
      // lblDatum
      // 
      this.lblDatum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblDatum.AutoSize = true;
      this.lblDatum.Location = new System.Drawing.Point(3, 372);
      this.lblDatum.Name = "lblDatum";
      this.lblDatum.Size = new System.Drawing.Size(41, 13);
      this.lblDatum.TabIndex = 1;
      this.lblDatum.Text = "Datum:";
      // 
      // dateTimePicker1
      // 
      this.dateTimePicker1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dateTimePicker1.Location = new System.Drawing.Point(71, 366);
      this.dateTimePicker1.Name = "dateTimePicker1";
      this.dateTimePicker1.Size = new System.Drawing.Size(277, 20);
      this.dateTimePicker1.TabIndex = 2;
      // 
      // lblArt
      // 
      this.lblArt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblArt.AutoSize = true;
      this.lblArt.Location = new System.Drawing.Point(3, 394);
      this.lblArt.Name = "lblArt";
      this.lblArt.Size = new System.Drawing.Size(23, 13);
      this.lblArt.TabIndex = 3;
      this.lblArt.Text = "Art:";
      // 
      // comboBoxArt
      // 
      this.comboBoxArt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxArt.FormattingEnabled = true;
      this.comboBoxArt.Location = new System.Drawing.Point(71, 391);
      this.comboBoxArt.Name = "comboBoxArt";
      this.comboBoxArt.Size = new System.Drawing.Size(277, 21);
      this.comboBoxArt.TabIndex = 4;
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Location = new System.Drawing.Point(71, 418);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(555, 20);
      this.textBox1.TabIndex = 5;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 418);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(64, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Bemerkung:";
      // 
      // btnNeuesVorkommnis
      // 
      this.btnNeuesVorkommnis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNeuesVorkommnis.Location = new System.Drawing.Point(632, 388);
      this.btnNeuesVorkommnis.Name = "btnNeuesVorkommnis";
      this.btnNeuesVorkommnis.Size = new System.Drawing.Size(83, 50);
      this.btnNeuesVorkommnis.TabIndex = 7;
      this.btnNeuesVorkommnis.Text = "neues Vorkommnis";
      this.btnNeuesVorkommnis.UseVisualStyleBackColor = true;
      this.btnNeuesVorkommnis.Click += new System.EventHandler(this.btnNeuesVorkommnis_Click);
      // 
      // lblSchuelername
      // 
      this.lblSchuelername.AutoSize = true;
      this.lblSchuelername.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSchuelername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
      this.lblSchuelername.Location = new System.Drawing.Point(3, 3);
      this.lblSchuelername.Name = "lblSchuelername";
      this.lblSchuelername.Size = new System.Drawing.Size(184, 31);
      this.lblSchuelername.TabIndex = 8;
      this.lblSchuelername.Text = "Schülername";
      // 
      // objectListViewVorkommnisse
      // 
      this.objectListViewVorkommnisse.AllColumns.Add(this.olvColumnDatum);
      this.objectListViewVorkommnisse.AllColumns.Add(this.olvColumnArt);
      this.objectListViewVorkommnisse.AllColumns.Add(this.olvColumnBemerkung);
      this.objectListViewVorkommnisse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.objectListViewVorkommnisse.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnDatum,
            this.olvColumnArt,
            this.olvColumnBemerkung});
      this.objectListViewVorkommnisse.Location = new System.Drawing.Point(3, 37);
      this.objectListViewVorkommnisse.Name = "objectListViewVorkommnisse";
      this.objectListViewVorkommnisse.Size = new System.Drawing.Size(713, 310);
      this.objectListViewVorkommnisse.TabIndex = 0;
      this.objectListViewVorkommnisse.UseCompatibleStateImageBehavior = false;
      this.objectListViewVorkommnisse.View = System.Windows.Forms.View.Details;
      // 
      // olvColumnDatum
      // 
      this.olvColumnDatum.AspectName = "Datum";
      this.olvColumnDatum.IsEditable = false;
      this.olvColumnDatum.Text = "Datum";
      this.olvColumnDatum.Width = 100;
      // 
      // olvColumnArt
      // 
      this.olvColumnArt.IsEditable = false;
      this.olvColumnArt.Text = "Art";
      this.olvColumnArt.Width = 200;
      // 
      // olvColumnBemerkung
      // 
      this.olvColumnBemerkung.AspectName = "Bemerkung";
      this.olvColumnBemerkung.IsEditable = false;
      this.olvColumnBemerkung.Text = "Bemerkung";
      this.olvColumnBemerkung.Width = 300;
      // 
      // UserControlVorkommnisse
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblSchuelername);
      this.Controls.Add(this.btnNeuesVorkommnis);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.comboBoxArt);
      this.Controls.Add(this.lblArt);
      this.Controls.Add(this.dateTimePicker1);
      this.Controls.Add(this.lblDatum);
      this.Controls.Add(this.objectListViewVorkommnisse);
      this.Name = "UserControlVorkommnisse";
      this.Size = new System.Drawing.Size(718, 459);
      ((System.ComponentModel.ISupportInitialize)(this.objectListViewVorkommnisse)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private BrightIdeasSoftware.ObjectListView objectListViewVorkommnisse;
    private BrightIdeasSoftware.OLVColumn olvColumnDatum;
    private BrightIdeasSoftware.OLVColumn olvColumnArt;
    private BrightIdeasSoftware.OLVColumn olvColumnBemerkung;
    private System.Windows.Forms.Label lblDatum;
    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.Label lblArt;
    private System.Windows.Forms.ComboBox comboBoxArt;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnNeuesVorkommnis;
    private System.Windows.Forms.Label lblSchuelername;
  }
}
