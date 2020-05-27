namespace diNo
{
  partial class UserControlKurszuordnungen
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
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
      this.olvColumnCurrentId = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnKurzbezeichnung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnBezeichnung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.objectListView2 = new BrightIdeasSoftware.ObjectListView();
      this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.label5 = new System.Windows.Forms.Label();
      this.textBoxReliOderEthik = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.objectListView2)).BeginInit();
      this.SuspendLayout();
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(17, 52);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(186, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Kurse, die der Schüler aktuell besucht";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(325, 52);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(168, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "weitere mögliche Kurse der Klasse";
      // 
      // objectListView1
      // 
      this.objectListView1.AllColumns.Add(this.olvColumnCurrentId);
      this.objectListView1.AllColumns.Add(this.olvColumnKurzbezeichnung);
      this.objectListView1.AllColumns.Add(this.olvColumnBezeichnung);
      this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnCurrentId,
            this.olvColumnKurzbezeichnung,
            this.olvColumnBezeichnung});
      this.objectListView1.HideSelection = false;
      this.objectListView1.Location = new System.Drawing.Point(20, 68);
      this.objectListView1.Name = "objectListView1";
      this.objectListView1.Size = new System.Drawing.Size(271, 270);
      this.objectListView1.TabIndex = 4;
      this.objectListView1.UseCompatibleStateImageBehavior = false;
      this.objectListView1.View = System.Windows.Forms.View.Details;
      this.objectListView1.DoubleClick += new System.EventHandler(this.objectListView1_DoubleClick);
      // 
      // olvColumnCurrentId
      // 
      this.olvColumnCurrentId.AspectName = "Id";
      this.olvColumnCurrentId.Groupable = false;
      this.olvColumnCurrentId.IsEditable = false;
      this.olvColumnCurrentId.Text = "Id";
      this.olvColumnCurrentId.Width = 50;
      // 
      // olvColumnKurzbezeichnung
      // 
      this.olvColumnKurzbezeichnung.AspectName = "Kurzbezeichnung";
      this.olvColumnKurzbezeichnung.Groupable = false;
      this.olvColumnKurzbezeichnung.IsEditable = false;
      this.olvColumnKurzbezeichnung.Text = "Kürzel";
      this.olvColumnKurzbezeichnung.Width = 85;
      // 
      // olvColumnBezeichnung
      // 
      this.olvColumnBezeichnung.AspectName = "Kursbezeichnung";
      this.olvColumnBezeichnung.Text = "Bezeichnung";
      this.olvColumnBezeichnung.Width = 250;
      // 
      // objectListView2
      // 
      this.objectListView2.AllColumns.Add(this.olvColumn1);
      this.objectListView2.AllColumns.Add(this.olvColumn2);
      this.objectListView2.AllColumns.Add(this.olvColumn3);
      this.objectListView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
      this.objectListView2.HideSelection = false;
      this.objectListView2.Location = new System.Drawing.Point(328, 68);
      this.objectListView2.Name = "objectListView2";
      this.objectListView2.Size = new System.Drawing.Size(270, 270);
      this.objectListView2.TabIndex = 5;
      this.objectListView2.UseCompatibleStateImageBehavior = false;
      this.objectListView2.View = System.Windows.Forms.View.Details;
      this.objectListView2.DoubleClick += new System.EventHandler(this.objectListView2_DoubleClick);
      // 
      // olvColumn1
      // 
      this.olvColumn1.AspectName = "Id";
      this.olvColumn1.Groupable = false;
      this.olvColumn1.IsEditable = false;
      this.olvColumn1.Text = "Id";
      this.olvColumn1.Width = 50;
      // 
      // olvColumn2
      // 
      this.olvColumn2.AspectName = "Kurzbezeichnung";
      this.olvColumn2.Groupable = false;
      this.olvColumn2.IsEditable = false;
      this.olvColumn2.Text = "Kürzel";
      this.olvColumn2.Width = 85;
      // 
      // olvColumn3
      // 
      this.olvColumn3.AspectName = "Kursbezeichnung";
      this.olvColumn3.Text = "Bezeichnung";
      this.olvColumn3.Width = 250;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(618, 52);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(94, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Religionsunterricht";
      // 
      // textBoxReliOderEthik
      // 
      this.textBoxReliOderEthik.Enabled = false;
      this.textBoxReliOderEthik.Location = new System.Drawing.Point(621, 68);
      this.textBoxReliOderEthik.Name = "textBoxReliOderEthik";
      this.textBoxReliOderEthik.Size = new System.Drawing.Size(78, 20);
      this.textBoxReliOderEthik.TabIndex = 10;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(17, 15);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(508, 17);
      this.label6.TabIndex = 12;
      this.label6.Text = "Um Kurse hinzuzufügen oder wegzunehmen auf die Kursnummer doppelclicken";
      // 
      // UserControlKurszuordnungen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.textBoxReliOderEthik);
      this.Controls.Add(this.objectListView2);
      this.Controls.Add(this.objectListView1);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Name = "UserControlKurszuordnungen";
      this.Size = new System.Drawing.Size(728, 365);
      ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.objectListView2)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private BrightIdeasSoftware.ObjectListView objectListView1;
    private BrightIdeasSoftware.OLVColumn olvColumnCurrentId;
    private BrightIdeasSoftware.OLVColumn olvColumnKurzbezeichnung;
    private BrightIdeasSoftware.ObjectListView objectListView2;
    private BrightIdeasSoftware.OLVColumn olvColumn1;
    private BrightIdeasSoftware.OLVColumn olvColumn2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBoxReliOderEthik;
    private System.Windows.Forms.Label label6;
    private BrightIdeasSoftware.OLVColumn olvColumn3;
    private BrightIdeasSoftware.OLVColumn olvColumnBezeichnung;
  }
}