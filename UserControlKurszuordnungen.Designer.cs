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
      this.olvColumnCurrentBezeichnung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.objectListView2 = new BrightIdeasSoftware.ObjectListView();
      this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.textBoxWahlpflichtfach = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.textBoxFremdsprache2 = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.textBoxReliOderEthik = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.objectListView2)).BeginInit();
      this.SuspendLayout();
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 9);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(74, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "aktuelle Kurse";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(251, 9);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(123, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "weitere Kurse der Klasse";
      // 
      // objectListView1
      // 
      this.objectListView1.AllColumns.Add(this.olvColumnCurrentId);
      this.objectListView1.AllColumns.Add(this.olvColumnCurrentBezeichnung);
      this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnCurrentId,
            this.olvColumnCurrentBezeichnung});
      this.objectListView1.Location = new System.Drawing.Point(14, 25);
      this.objectListView1.Name = "objectListView1";
      this.objectListView1.Size = new System.Drawing.Size(210, 270);
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
      // 
      // olvColumnCurrentBezeichnung
      // 
      this.olvColumnCurrentBezeichnung.AspectName = "FachBezeichnung";
      this.olvColumnCurrentBezeichnung.Groupable = false;
      this.olvColumnCurrentBezeichnung.IsEditable = false;
      this.olvColumnCurrentBezeichnung.Text = "Bezeichnung";
      this.olvColumnCurrentBezeichnung.Width = 145;
      // 
      // objectListView2
      // 
      this.objectListView2.AllColumns.Add(this.olvColumn1);
      this.objectListView2.AllColumns.Add(this.olvColumn2);
      this.objectListView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2});
      this.objectListView2.Location = new System.Drawing.Point(254, 25);
      this.objectListView2.Name = "objectListView2";
      this.objectListView2.Size = new System.Drawing.Size(210, 270);
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
      // 
      // olvColumn2
      // 
      this.olvColumn2.AspectName = "FachBezeichnung";
      this.olvColumn2.Groupable = false;
      this.olvColumn2.IsEditable = false;
      this.olvColumn2.Text = "Bezeichnung";
      this.olvColumn2.Width = 145;
      // 
      // textBoxWahlpflichtfach
      // 
      this.textBoxWahlpflichtfach.Enabled = false;
      this.textBoxWahlpflichtfach.Location = new System.Drawing.Point(498, 68);
      this.textBoxWahlpflichtfach.Name = "textBoxWahlpflichtfach";
      this.textBoxWahlpflichtfach.Size = new System.Drawing.Size(78, 20);
      this.textBoxWahlpflichtfach.TabIndex = 6;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(495, 52);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(81, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Wahlpflichtfach";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(495, 96);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(80, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Fremdsprache2";
      // 
      // textBoxFremdsprache2
      // 
      this.textBoxFremdsprache2.Enabled = false;
      this.textBoxFremdsprache2.Location = new System.Drawing.Point(498, 112);
      this.textBoxFremdsprache2.Name = "textBoxFremdsprache2";
      this.textBoxFremdsprache2.Size = new System.Drawing.Size(78, 20);
      this.textBoxFremdsprache2.TabIndex = 8;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(495, 145);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(94, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Religionsunterricht";
      // 
      // textBoxReliOderEthik
      // 
      this.textBoxReliOderEthik.Enabled = false;
      this.textBoxReliOderEthik.Location = new System.Drawing.Point(498, 161);
      this.textBoxReliOderEthik.Name = "textBoxReliOderEthik";
      this.textBoxReliOderEthik.Size = new System.Drawing.Size(78, 20);
      this.textBoxReliOderEthik.TabIndex = 10;
      // 
      // UserControlKurszuordnungen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.Controls.Add(this.label5);
      this.Controls.Add(this.textBoxReliOderEthik);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.textBoxFremdsprache2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textBoxWahlpflichtfach);
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
    private BrightIdeasSoftware.OLVColumn olvColumnCurrentBezeichnung;
    private BrightIdeasSoftware.ObjectListView objectListView2;
    private BrightIdeasSoftware.OLVColumn olvColumn1;
    private BrightIdeasSoftware.OLVColumn olvColumn2;
    private System.Windows.Forms.TextBox textBoxWahlpflichtfach;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox textBoxFremdsprache2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBoxReliOderEthik;
  }
}