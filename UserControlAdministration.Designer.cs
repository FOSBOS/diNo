namespace diNo
{
  partial class UserControlAdministration
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
      this.groupBoxAnalyse = new System.Windows.Forms.GroupBox();
      this.groupBoxDrucken = new System.Windows.Forms.GroupBox();
      this.btnAbiergebnisse = new System.Windows.Forms.Button();
      this.groupBoxExport = new System.Windows.Forms.GroupBox();
      this.exportNoten = new System.Windows.Forms.Button();
      this.btnFrm1 = new System.Windows.Forms.Button();
      this.groupBoxImport = new System.Windows.Forms.GroupBox();
      this.importNoten = new System.Windows.Forms.Button();
      this.btnImportUnterricht = new System.Windows.Forms.Button();
      this.groupBoxDrucken.SuspendLayout();
      this.groupBoxExport.SuspendLayout();
      this.groupBoxImport.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBoxAnalyse
      // 
      this.groupBoxAnalyse.Location = new System.Drawing.Point(19, 268);
      this.groupBoxAnalyse.Name = "groupBoxAnalyse";
      this.groupBoxAnalyse.Size = new System.Drawing.Size(288, 226);
      this.groupBoxAnalyse.TabIndex = 4;
      this.groupBoxAnalyse.TabStop = false;
      this.groupBoxAnalyse.Text = "Datenanalyse";
      // 
      // groupBoxDrucken
      // 
      this.groupBoxDrucken.Controls.Add(this.btnAbiergebnisse);
      this.groupBoxDrucken.Location = new System.Drawing.Point(19, 28);
      this.groupBoxDrucken.Name = "groupBoxDrucken";
      this.groupBoxDrucken.Size = new System.Drawing.Size(288, 233);
      this.groupBoxDrucken.TabIndex = 3;
      this.groupBoxDrucken.TabStop = false;
      this.groupBoxDrucken.Text = "Drucken";
      // 
      // btnAbiergebnisse
      // 
      this.btnAbiergebnisse.Location = new System.Drawing.Point(20, 28);
      this.btnAbiergebnisse.Name = "btnAbiergebnisse";
      this.btnAbiergebnisse.Size = new System.Drawing.Size(102, 23);
      this.btnAbiergebnisse.TabIndex = 0;
      this.btnAbiergebnisse.Text = "Abiergebnisse";
      this.btnAbiergebnisse.UseVisualStyleBackColor = true;
      this.btnAbiergebnisse.Click += new System.EventHandler(this.btnAbiergebnisse_Click);
      // 
      // groupBoxExport
      // 
      this.groupBoxExport.Controls.Add(this.exportNoten);
      this.groupBoxExport.Controls.Add(this.btnFrm1);
      this.groupBoxExport.Location = new System.Drawing.Point(324, 268);
      this.groupBoxExport.Name = "groupBoxExport";
      this.groupBoxExport.Size = new System.Drawing.Size(288, 226);
      this.groupBoxExport.TabIndex = 4;
      this.groupBoxExport.TabStop = false;
      this.groupBoxExport.Text = "Export";
      // 
      // exportNoten
      // 
      this.exportNoten.Location = new System.Drawing.Point(6, 19);
      this.exportNoten.Name = "exportNoten";
      this.exportNoten.Size = new System.Drawing.Size(182, 23);
      this.exportNoten.TabIndex = 2;
      this.exportNoten.Text = "Noten und FpA nach csv";
      this.exportNoten.UseVisualStyleBackColor = true;
      this.exportNoten.Click += new System.EventHandler(this.exportNoten_Click);
      // 
      // btnFrm1
      // 
      this.btnFrm1.Location = new System.Drawing.Point(177, 70);
      this.btnFrm1.Name = "btnFrm1";
      this.btnFrm1.Size = new System.Drawing.Size(105, 42);
      this.btnFrm1.TabIndex = 1;
      this.btnFrm1.Text = "Die berühmte Form1 aufrufen";
      this.btnFrm1.UseVisualStyleBackColor = true;
      this.btnFrm1.Click += new System.EventHandler(this.btnFrm1_Click);
      // 
      // groupBoxImport
      // 
      this.groupBoxImport.Controls.Add(this.btnImportUnterricht);
      this.groupBoxImport.Controls.Add(this.importNoten);
      this.groupBoxImport.Location = new System.Drawing.Point(324, 28);
      this.groupBoxImport.Name = "groupBoxImport";
      this.groupBoxImport.Size = new System.Drawing.Size(288, 233);
      this.groupBoxImport.TabIndex = 3;
      this.groupBoxImport.TabStop = false;
      this.groupBoxImport.Text = "Import";
      // 
      // importNoten
      // 
      this.importNoten.Location = new System.Drawing.Point(6, 19);
      this.importNoten.Name = "importNoten";
      this.importNoten.Size = new System.Drawing.Size(182, 23);
      this.importNoten.TabIndex = 3;
      this.importNoten.Text = "Noten abgelegter Fächer aus csv";
      this.importNoten.UseVisualStyleBackColor = true;
      this.importNoten.Click += new System.EventHandler(this.importNoten_Click);
      // 
      // btnImportUnterricht
      // 
      this.btnImportUnterricht.Location = new System.Drawing.Point(6, 48);
      this.btnImportUnterricht.Name = "btnImportUnterricht";
      this.btnImportUnterricht.Size = new System.Drawing.Size(182, 23);
      this.btnImportUnterricht.TabIndex = 4;
      this.btnImportUnterricht.Text = "Kurse aus Untis-Excel";
      this.btnImportUnterricht.UseVisualStyleBackColor = true;
      this.btnImportUnterricht.Click += new System.EventHandler(this.btnImportUnterricht_Click);
      // 
      // UserControlAdministration
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBoxExport);
      this.Controls.Add(this.groupBoxImport);
      this.Controls.Add(this.groupBoxAnalyse);
      this.Controls.Add(this.groupBoxDrucken);
      this.Name = "UserControlAdministration";
      this.Size = new System.Drawing.Size(671, 530);
      this.groupBoxDrucken.ResumeLayout(false);
      this.groupBoxExport.ResumeLayout(false);
      this.groupBoxImport.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBoxAnalyse;
    private System.Windows.Forms.GroupBox groupBoxDrucken;
    private System.Windows.Forms.GroupBox groupBoxExport;
    private System.Windows.Forms.Button btnFrm1;
    private System.Windows.Forms.GroupBox groupBoxImport;
    private System.Windows.Forms.Button btnAbiergebnisse;
    private System.Windows.Forms.Button exportNoten;
    private System.Windows.Forms.Button importNoten;
    private System.Windows.Forms.Button btnImportUnterricht;
  }
}
