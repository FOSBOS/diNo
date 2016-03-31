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
      this.label3.Location = new System.Drawing.Point(313, 9);
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
      this.objectListView1.Size = new System.Drawing.Size(210, 243);
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
      this.objectListView2.Location = new System.Drawing.Point(316, 25);
      this.objectListView2.Name = "objectListView2";
      this.objectListView2.Size = new System.Drawing.Size(210, 243);
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
      // UserControlKurszuordnungen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.objectListView2);
      this.Controls.Add(this.objectListView1);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Name = "UserControlKurszuordnungen";
      this.Size = new System.Drawing.Size(571, 277);
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
  }
}