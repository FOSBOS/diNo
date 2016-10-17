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
      this.btnImportKlassenleiter = new System.Windows.Forms.Button();
      this.btnImportSchueler = new System.Windows.Forms.Button();
      this.btnImportUnterricht = new System.Windows.Forms.Button();
      this.importNoten = new System.Windows.Forms.Button();
      this.groupBoxBerechtigungen = new System.Windows.Forms.GroupBox();
      this.btnKurseLehrer = new System.Windows.Forms.Button();
      this.btnNotenmitteilung = new System.Windows.Forms.Button();
      this.groupBoxDrucken.SuspendLayout();
      this.groupBoxExport.SuspendLayout();
      this.groupBoxImport.SuspendLayout();
      this.groupBoxBerechtigungen.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBoxAnalyse
      // 
      this.groupBoxAnalyse.Location = new System.Drawing.Point(19, 268);
      this.groupBoxAnalyse.Name = "groupBoxAnalyse";
      this.groupBoxAnalyse.Size = new System.Drawing.Size(250, 226);
      this.groupBoxAnalyse.TabIndex = 4;
      this.groupBoxAnalyse.TabStop = false;
      this.groupBoxAnalyse.Text = "Datenanalyse";
      // 
      // groupBoxDrucken
      // 
      this.groupBoxDrucken.Controls.Add(this.btnNotenmitteilung);
      this.groupBoxDrucken.Controls.Add(this.btnAbiergebnisse);
      this.groupBoxDrucken.Location = new System.Drawing.Point(19, 28);
      this.groupBoxDrucken.Name = "groupBoxDrucken";
      this.groupBoxDrucken.Size = new System.Drawing.Size(250, 233);
      this.groupBoxDrucken.TabIndex = 3;
      this.groupBoxDrucken.TabStop = false;
      this.groupBoxDrucken.Text = "Drucken";
      // 
      // btnAbiergebnisse
      // 
      this.btnAbiergebnisse.Location = new System.Drawing.Point(21, 48);
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
      this.groupBoxExport.Location = new System.Drawing.Point(287, 267);
      this.groupBoxExport.Name = "groupBoxExport";
      this.groupBoxExport.Size = new System.Drawing.Size(250, 226);
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
      this.btnFrm1.Location = new System.Drawing.Point(139, 69);
      this.btnFrm1.Name = "btnFrm1";
      this.btnFrm1.Size = new System.Drawing.Size(105, 42);
      this.btnFrm1.TabIndex = 1;
      this.btnFrm1.Text = "Die berühmte Form1 aufrufen";
      this.btnFrm1.UseVisualStyleBackColor = true;
      this.btnFrm1.Click += new System.EventHandler(this.btnFrm1_Click);
      // 
      // groupBoxImport
      // 
      this.groupBoxImport.Controls.Add(this.btnImportKlassenleiter);
      this.groupBoxImport.Controls.Add(this.btnImportSchueler);
      this.groupBoxImport.Controls.Add(this.btnImportUnterricht);
      this.groupBoxImport.Controls.Add(this.importNoten);
      this.groupBoxImport.Location = new System.Drawing.Point(287, 28);
      this.groupBoxImport.Name = "groupBoxImport";
      this.groupBoxImport.Size = new System.Drawing.Size(250, 233);
      this.groupBoxImport.TabIndex = 3;
      this.groupBoxImport.TabStop = false;
      this.groupBoxImport.Text = "Import";
      // 
      // btnImportKlassenleiter
      // 
      this.btnImportKlassenleiter.Location = new System.Drawing.Point(6, 106);
      this.btnImportKlassenleiter.Name = "btnImportKlassenleiter";
      this.btnImportKlassenleiter.Size = new System.Drawing.Size(182, 23);
      this.btnImportKlassenleiter.TabIndex = 6;
      this.btnImportKlassenleiter.Text = "Import Klassenleiter aus Excel";
      this.btnImportKlassenleiter.UseVisualStyleBackColor = true;
      this.btnImportKlassenleiter.Click += new System.EventHandler(this.btnImportKlassenleiter_Click);
      // 
      // btnImportSchueler
      // 
      this.btnImportSchueler.Location = new System.Drawing.Point(6, 77);
      this.btnImportSchueler.Name = "btnImportSchueler";
      this.btnImportSchueler.Size = new System.Drawing.Size(182, 23);
      this.btnImportSchueler.TabIndex = 5;
      this.btnImportSchueler.Text = "Schülerdaten aus WinSV";
      this.btnImportSchueler.UseVisualStyleBackColor = true;
      this.btnImportSchueler.Click += new System.EventHandler(this.btnImportSchueler_Click);
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
      // groupBoxBerechtigungen
      // 
      this.groupBoxBerechtigungen.Controls.Add(this.btnKurseLehrer);
      this.groupBoxBerechtigungen.Location = new System.Drawing.Point(555, 28);
      this.groupBoxBerechtigungen.Name = "groupBoxBerechtigungen";
      this.groupBoxBerechtigungen.Size = new System.Drawing.Size(250, 233);
      this.groupBoxBerechtigungen.TabIndex = 5;
      this.groupBoxBerechtigungen.TabStop = false;
      this.groupBoxBerechtigungen.Text = "Berechtigungen";
      // 
      // btnKurseLehrer
      // 
      this.btnKurseLehrer.Location = new System.Drawing.Point(6, 21);
      this.btnKurseLehrer.Name = "btnKurseLehrer";
      this.btnKurseLehrer.Size = new System.Drawing.Size(164, 23);
      this.btnKurseLehrer.TabIndex = 0;
      this.btnKurseLehrer.Text = "Kurse und Lehrer zuordnen";
      this.btnKurseLehrer.UseVisualStyleBackColor = true;
      this.btnKurseLehrer.Click += new System.EventHandler(this.btnKurseLehrer_Click);
      // 
      // btnNotenmitteilung
      // 
      this.btnNotenmitteilung.Location = new System.Drawing.Point(21, 19);
      this.btnNotenmitteilung.Name = "btnNotenmitteilung";
      this.btnNotenmitteilung.Size = new System.Drawing.Size(102, 25);
      this.btnNotenmitteilung.TabIndex = 1;
      this.btnNotenmitteilung.Text = "Notenmitteilung";
      this.btnNotenmitteilung.UseVisualStyleBackColor = true;
      this.btnNotenmitteilung.Click += new System.EventHandler(this.btnNotenmitteilung_Click);
      // 
      // UserControlAdministration
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBoxBerechtigungen);
      this.Controls.Add(this.groupBoxExport);
      this.Controls.Add(this.groupBoxImport);
      this.Controls.Add(this.groupBoxAnalyse);
      this.Controls.Add(this.groupBoxDrucken);
      this.Name = "UserControlAdministration";
      this.Size = new System.Drawing.Size(907, 530);
      this.groupBoxDrucken.ResumeLayout(false);
      this.groupBoxExport.ResumeLayout(false);
      this.groupBoxImport.ResumeLayout(false);
      this.groupBoxBerechtigungen.ResumeLayout(false);
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
    private System.Windows.Forms.Button btnImportSchueler;
    private System.Windows.Forms.Button btnImportKlassenleiter;
    private System.Windows.Forms.GroupBox groupBoxBerechtigungen;
    private System.Windows.Forms.Button btnKurseLehrer;
    private System.Windows.Forms.Button btnNotenmitteilung;
  }
}
