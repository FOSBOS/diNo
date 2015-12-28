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
      this.objectListViewVorkommnisse = new BrightIdeasSoftware.ObjectListView();
      this.olvColumnDatum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnArt = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnBemerkung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.label2 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.objectListViewVorkommnisse)).BeginInit();
      this.SuspendLayout();
      // 
      // lblDatum
      // 
      this.lblDatum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblDatum.AutoSize = true;
      this.lblDatum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDatum.Location = new System.Drawing.Point(5, 216);
      this.lblDatum.Name = "lblDatum";
      this.lblDatum.Size = new System.Drawing.Size(53, 17);
      this.lblDatum.TabIndex = 1;
      this.lblDatum.Text = "Datum:";
      // 
      // dateTimePicker1
      // 
      this.dateTimePicker1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dateTimePicker1.Location = new System.Drawing.Point(95, 210);
      this.dateTimePicker1.Name = "dateTimePicker1";
      this.dateTimePicker1.Size = new System.Drawing.Size(424, 23);
      this.dateTimePicker1.TabIndex = 2;
      // 
      // lblArt
      // 
      this.lblArt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblArt.AutoSize = true;
      this.lblArt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblArt.Location = new System.Drawing.Point(5, 238);
      this.lblArt.Name = "lblArt";
      this.lblArt.Size = new System.Drawing.Size(30, 17);
      this.lblArt.TabIndex = 3;
      this.lblArt.Text = "Art:";
      // 
      // comboBoxArt
      // 
      this.comboBoxArt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboBoxArt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboBoxArt.FormattingEnabled = true;
      this.comboBoxArt.Location = new System.Drawing.Point(95, 235);
      this.comboBoxArt.Name = "comboBoxArt";
      this.comboBoxArt.Size = new System.Drawing.Size(424, 24);
      this.comboBoxArt.TabIndex = 4;
      this.comboBoxArt.SelectedValueChanged += new System.EventHandler(this.comboBoxArt_SelectedValueChanged);
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox1.Location = new System.Drawing.Point(95, 262);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(612, 23);
      this.textBox1.TabIndex = 5;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(5, 262);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(84, 17);
      this.label1.TabIndex = 6;
      this.label1.Text = "Bemerkung:";
      // 
      // btnNeuesVorkommnis
      // 
      this.btnNeuesVorkommnis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnNeuesVorkommnis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNeuesVorkommnis.Location = new System.Drawing.Point(549, 206);
      this.btnNeuesVorkommnis.Name = "btnNeuesVorkommnis";
      this.btnNeuesVorkommnis.Size = new System.Drawing.Size(158, 50);
      this.btnNeuesVorkommnis.TabIndex = 7;
      this.btnNeuesVorkommnis.Text = "speichern";
      this.btnNeuesVorkommnis.UseVisualStyleBackColor = true;
      this.btnNeuesVorkommnis.Click += new System.EventHandler(this.btnNeuesVorkommnis_Click);
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
      this.objectListViewVorkommnisse.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.objectListViewVorkommnisse.Location = new System.Drawing.Point(7, 13);
      this.objectListViewVorkommnisse.Name = "objectListViewVorkommnisse";
      this.objectListViewVorkommnisse.Size = new System.Drawing.Size(700, 155);
      this.objectListViewVorkommnisse.TabIndex = 0;
      this.objectListViewVorkommnisse.UseCompatibleStateImageBehavior = false;
      this.objectListViewVorkommnisse.View = System.Windows.Forms.View.Details;
      // 
      // olvColumnDatum
      // 
      this.olvColumnDatum.AspectName = "Datum";
      this.olvColumnDatum.AspectToStringFormat = "{0:d}";
      this.olvColumnDatum.Groupable = false;
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
      this.olvColumnBemerkung.Width = 386;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(5, 184);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(300, 17);
      this.label2.TabIndex = 8;
      this.label2.Text = "Hier können Sie einen neuen Eintrag anlegen:";
      // 
      // UserControlVorkommnisse
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label2);
      this.Controls.Add(this.btnNeuesVorkommnis);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.comboBoxArt);
      this.Controls.Add(this.lblArt);
      this.Controls.Add(this.dateTimePicker1);
      this.Controls.Add(this.lblDatum);
      this.Controls.Add(this.objectListViewVorkommnisse);
      this.Name = "UserControlVorkommnisse";
      this.Size = new System.Drawing.Size(980, 381);
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
        private System.Windows.Forms.Label label2;
    }
}
