﻿namespace diNo
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
      this.datVorkommnis = new System.Windows.Forms.DateTimePicker();
      this.lblArt = new System.Windows.Forms.Label();
      this.comboBoxArt = new System.Windows.Forms.ComboBox();
      this.edVorkommnisBemerkung = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnNeuesVorkommnis = new System.Windows.Forms.Button();
      this.objectListViewVorkommnisse = new BrightIdeasSoftware.ObjectListView();
      this.olvColumnDatum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnArt = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnBemerkung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnLoeschen = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.objectListViewVorkommnisse)).BeginInit();
      this.SuspendLayout();
      // 
      // lblDatum
      // 
      this.lblDatum.AutoSize = true;
      this.lblDatum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDatum.Location = new System.Drawing.Point(7, 35);
      this.lblDatum.Name = "lblDatum";
      this.lblDatum.Size = new System.Drawing.Size(53, 17);
      this.lblDatum.TabIndex = 1;
      this.lblDatum.Text = "Datum:";
      // 
      // datVorkommnis
      // 
      this.datVorkommnis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.datVorkommnis.Location = new System.Drawing.Point(97, 29);
      this.datVorkommnis.Name = "datVorkommnis";
      this.datVorkommnis.Size = new System.Drawing.Size(327, 23);
      this.datVorkommnis.TabIndex = 0;
      // 
      // lblArt
      // 
      this.lblArt.AutoSize = true;
      this.lblArt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblArt.Location = new System.Drawing.Point(7, 57);
      this.lblArt.Name = "lblArt";
      this.lblArt.Size = new System.Drawing.Size(30, 17);
      this.lblArt.TabIndex = 3;
      this.lblArt.Text = "Art:";
      // 
      // comboBoxArt
      // 
      this.comboBoxArt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboBoxArt.FormattingEnabled = true;
      this.comboBoxArt.Location = new System.Drawing.Point(97, 54);
      this.comboBoxArt.Name = "comboBoxArt";
      this.comboBoxArt.Size = new System.Drawing.Size(327, 24);
      this.comboBoxArt.TabIndex = 1;
      this.comboBoxArt.SelectedValueChanged += new System.EventHandler(this.comboBoxArt_SelectedValueChanged);
      // 
      // edVorkommnisBemerkung
      // 
      this.edVorkommnisBemerkung.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edVorkommnisBemerkung.Location = new System.Drawing.Point(97, 81);
      this.edVorkommnisBemerkung.Multiline = true;
      this.edVorkommnisBemerkung.Name = "edVorkommnisBemerkung";
      this.edVorkommnisBemerkung.Size = new System.Drawing.Size(327, 60);
      this.edVorkommnisBemerkung.TabIndex = 2;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(7, 81);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(84, 17);
      this.label1.TabIndex = 6;
      this.label1.Text = "Bemerkung:";
      // 
      // btnNeuesVorkommnis
      // 
      this.btnNeuesVorkommnis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNeuesVorkommnis.Image = global::diNo.Properties.Resources.Save;
      this.btnNeuesVorkommnis.Location = new System.Drawing.Point(430, 101);
      this.btnNeuesVorkommnis.Name = "btnNeuesVorkommnis";
      this.btnNeuesVorkommnis.Size = new System.Drawing.Size(40, 40);
      this.btnNeuesVorkommnis.TabIndex = 3;
      this.btnNeuesVorkommnis.UseVisualStyleBackColor = true;
      this.btnNeuesVorkommnis.Click += new System.EventHandler(this.btnNeuesVorkommnis_Click);
      // 
      // objectListViewVorkommnisse
      // 
      this.objectListViewVorkommnisse.AllColumns.Add(this.olvColumnDatum);
      this.objectListViewVorkommnisse.AllColumns.Add(this.olvColumnArt);
      this.objectListViewVorkommnisse.AllColumns.Add(this.olvColumnBemerkung);
      this.objectListViewVorkommnisse.AllColumns.Add(this.olvColumnLoeschen);
      this.objectListViewVorkommnisse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.objectListViewVorkommnisse.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.objectListViewVorkommnisse.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnDatum,
            this.olvColumnArt,
            this.olvColumnBemerkung,
            this.olvColumnLoeschen});
      this.objectListViewVorkommnisse.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.objectListViewVorkommnisse.Location = new System.Drawing.Point(10, 183);
      this.objectListViewVorkommnisse.Name = "objectListViewVorkommnisse";
      this.objectListViewVorkommnisse.OwnerDraw = true;
      this.objectListViewVorkommnisse.ShowImagesOnSubItems = true;
      this.objectListViewVorkommnisse.Size = new System.Drawing.Size(460, 153);
      this.objectListViewVorkommnisse.TabIndex = 4;
      this.objectListViewVorkommnisse.TabStop = false;
      this.objectListViewVorkommnisse.UseCompatibleStateImageBehavior = false;
      this.objectListViewVorkommnisse.View = System.Windows.Forms.View.Details;
      this.objectListViewVorkommnisse.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.objectListViewVorkommnisse_CellEditStarting);
      // 
      // olvColumnDatum
      // 
      this.olvColumnDatum.AspectName = "Datum";
      this.olvColumnDatum.AspectToStringFormat = "{0:d}";
      this.olvColumnDatum.Groupable = false;
      this.olvColumnDatum.IsEditable = false;
      this.olvColumnDatum.Text = "Datum";
      this.olvColumnDatum.Width = 67;
      // 
      // olvColumnArt
      // 
      this.olvColumnArt.IsEditable = false;
      this.olvColumnArt.Text = "Art";
      this.olvColumnArt.Width = 167;
      // 
      // olvColumnBemerkung
      // 
      this.olvColumnBemerkung.AspectName = "Bemerkung";
      this.olvColumnBemerkung.IsEditable = false;
      this.olvColumnBemerkung.Text = "Bemerkung";
      this.olvColumnBemerkung.Width = 368;
      // 
      // olvColumnLoeschen
      // 
      this.olvColumnLoeschen.Groupable = false;
      this.olvColumnLoeschen.Hideable = false;
      this.olvColumnLoeschen.Searchable = false;
      this.olvColumnLoeschen.ShowTextInHeader = false;
      this.olvColumnLoeschen.Sortable = false;
      this.olvColumnLoeschen.Text = "";
      this.olvColumnLoeschen.Width = 25;
      // 
      // label2
      // 
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(7, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(300, 17);
      this.label2.TabIndex = 8;
      this.label2.Text = "Hier können Sie einen neuen Eintrag anlegen:";
      // 
      // label3
      // 
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(7, 163);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(300, 17);
      this.label3.TabIndex = 9;
      this.label3.Text = "bisherige Vorkommnisse:";
      // 
      // UserControlVorkommnisse
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.btnNeuesVorkommnis);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.edVorkommnisBemerkung);
      this.Controls.Add(this.comboBoxArt);
      this.Controls.Add(this.lblArt);
      this.Controls.Add(this.datVorkommnis);
      this.Controls.Add(this.lblDatum);
      this.Controls.Add(this.objectListViewVorkommnisse);
      this.Name = "UserControlVorkommnisse";
      this.Size = new System.Drawing.Size(487, 353);
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
    private System.Windows.Forms.DateTimePicker datVorkommnis;
    private System.Windows.Forms.Label lblArt;
    private System.Windows.Forms.ComboBox comboBoxArt;
    private System.Windows.Forms.TextBox edVorkommnisBemerkung;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnNeuesVorkommnis;
        private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private BrightIdeasSoftware.OLVColumn olvColumnLoeschen;
  }
}
