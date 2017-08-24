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
      this.btnKlassenliste = new System.Windows.Forms.Button();
      this.btnAttestpflicht = new System.Windows.Forms.Button();
      this.btnNotenmitteilung = new System.Windows.Forms.Button();
      this.btnAbiergebnisse = new System.Windows.Forms.Button();
      this.groupBoxExport = new System.Windows.Forms.GroupBox();
      this.btnNotenNachWinSV = new System.Windows.Forms.Button();
      this.btnSendExcelFiles = new System.Windows.Forms.Button();
      this.btnCreateExcels = new System.Windows.Forms.Button();
      this.exportNoten = new System.Windows.Forms.Button();
      this.groupBoxImport = new System.Windows.Forms.GroupBox();
      this.btnImportKlassenleiter = new System.Windows.Forms.Button();
      this.btnImportSchueler = new System.Windows.Forms.Button();
      this.btnImportUnterricht = new System.Windows.Forms.Button();
      this.importNoten = new System.Windows.Forms.Button();
      this.groupBoxBerechtigungen = new System.Windows.Forms.GroupBox();
      this.btnBerechtigungen = new System.Windows.Forms.Button();
      this.btnKurseLehrer = new System.Windows.Forms.Button();
      this.groupBoxEinstellungen = new System.Windows.Forms.GroupBox();
      this.edBackupPfad = new System.Windows.Forms.TextBox();
      this.lbBackupPfad = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBoxZeitpunkt = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.edSchuljahr = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.chkSperre = new System.Windows.Forms.CheckBox();
      this.lblStatus = new System.Windows.Forms.Label();
      this.groupBoxDrucken.SuspendLayout();
      this.groupBoxExport.SuspendLayout();
      this.groupBoxImport.SuspendLayout();
      this.groupBoxBerechtigungen.SuspendLayout();
      this.groupBoxEinstellungen.SuspendLayout();
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
      this.groupBoxDrucken.Controls.Add(this.btnKlassenliste);
      this.groupBoxDrucken.Controls.Add(this.btnAttestpflicht);
      this.groupBoxDrucken.Controls.Add(this.btnNotenmitteilung);
      this.groupBoxDrucken.Controls.Add(this.btnAbiergebnisse);
      this.groupBoxDrucken.Location = new System.Drawing.Point(19, 28);
      this.groupBoxDrucken.Name = "groupBoxDrucken";
      this.groupBoxDrucken.Size = new System.Drawing.Size(250, 233);
      this.groupBoxDrucken.TabIndex = 3;
      this.groupBoxDrucken.TabStop = false;
      this.groupBoxDrucken.Text = "Drucken";
      // 
      // btnKlassenliste
      // 
      this.btnKlassenliste.Location = new System.Drawing.Point(21, 77);
      this.btnKlassenliste.Name = "btnKlassenliste";
      this.btnKlassenliste.Size = new System.Drawing.Size(102, 23);
      this.btnKlassenliste.TabIndex = 3;
      this.btnKlassenliste.Text = "Klassenliste";
      this.btnKlassenliste.UseVisualStyleBackColor = true;
      this.btnKlassenliste.Click += new System.EventHandler(this.btnKlassenliste_Click);
      // 
      // btnAttestpflicht
      // 
      this.btnAttestpflicht.Location = new System.Drawing.Point(21, 106);
      this.btnAttestpflicht.Name = "btnAttestpflicht";
      this.btnAttestpflicht.Size = new System.Drawing.Size(102, 23);
      this.btnAttestpflicht.TabIndex = 2;
      this.btnAttestpflicht.Text = "Attestpflicht";
      this.btnAttestpflicht.UseVisualStyleBackColor = true;
      this.btnAttestpflicht.Click += new System.EventHandler(this.btnAttestpflicht_Click);
      // 
      // btnNotenmitteilung
      // 
      this.btnNotenmitteilung.Location = new System.Drawing.Point(21, 21);
      this.btnNotenmitteilung.Name = "btnNotenmitteilung";
      this.btnNotenmitteilung.Size = new System.Drawing.Size(102, 23);
      this.btnNotenmitteilung.TabIndex = 1;
      this.btnNotenmitteilung.Text = "Notenmitteilung";
      this.btnNotenmitteilung.UseVisualStyleBackColor = true;
      this.btnNotenmitteilung.Click += new System.EventHandler(this.btnNotenmitteilung_Click);
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
      this.groupBoxExport.Controls.Add(this.btnNotenNachWinSV);
      this.groupBoxExport.Controls.Add(this.btnSendExcelFiles);
      this.groupBoxExport.Controls.Add(this.btnCreateExcels);
      this.groupBoxExport.Controls.Add(this.exportNoten);
      this.groupBoxExport.Location = new System.Drawing.Point(287, 267);
      this.groupBoxExport.Name = "groupBoxExport";
      this.groupBoxExport.Size = new System.Drawing.Size(250, 226);
      this.groupBoxExport.TabIndex = 4;
      this.groupBoxExport.TabStop = false;
      this.groupBoxExport.Text = "Export";
      // 
      // btnNotenNachWinSV
      // 
      this.btnNotenNachWinSV.Location = new System.Drawing.Point(6, 106);
      this.btnNotenNachWinSV.Name = "btnNotenNachWinSV";
      this.btnNotenNachWinSV.Size = new System.Drawing.Size(182, 23);
      this.btnNotenNachWinSV.TabIndex = 5;
      this.btnNotenNachWinSV.Text = "Noten an WinSV";
      this.btnNotenNachWinSV.UseVisualStyleBackColor = true;
      this.btnNotenNachWinSV.Click += new System.EventHandler(this.btnNotenWinSV_Click);
      // 
      // btnSendExcelFiles
      // 
      this.btnSendExcelFiles.Location = new System.Drawing.Point(6, 77);
      this.btnSendExcelFiles.Name = "btnSendExcelFiles";
      this.btnSendExcelFiles.Size = new System.Drawing.Size(182, 23);
      this.btnSendExcelFiles.TabIndex = 4;
      this.btnSendExcelFiles.Text = "Excel-Dateien versenden";
      this.btnSendExcelFiles.UseVisualStyleBackColor = true;
      this.btnSendExcelFiles.Click += new System.EventHandler(this.btnSendMail_Click);
      // 
      // btnCreateExcels
      // 
      this.btnCreateExcels.Location = new System.Drawing.Point(6, 48);
      this.btnCreateExcels.Name = "btnCreateExcels";
      this.btnCreateExcels.Size = new System.Drawing.Size(182, 23);
      this.btnCreateExcels.TabIndex = 3;
      this.btnCreateExcels.Text = "Excel-Dateien erstellen";
      this.btnCreateExcels.UseVisualStyleBackColor = true;
      this.btnCreateExcels.Click += new System.EventHandler(this.btnCreateExcelsClick);
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
      this.importNoten.Location = new System.Drawing.Point(6, 20);
      this.importNoten.Name = "importNoten";
      this.importNoten.Size = new System.Drawing.Size(182, 23);
      this.importNoten.TabIndex = 3;
      this.importNoten.Text = "Noten abgelegter Fächer aus csv";
      this.importNoten.UseVisualStyleBackColor = true;
      this.importNoten.Click += new System.EventHandler(this.importNoten_Click);
      // 
      // groupBoxBerechtigungen
      // 
      this.groupBoxBerechtigungen.Controls.Add(this.btnBerechtigungen);
      this.groupBoxBerechtigungen.Controls.Add(this.btnKurseLehrer);
      this.groupBoxBerechtigungen.Location = new System.Drawing.Point(555, 28);
      this.groupBoxBerechtigungen.Name = "groupBoxBerechtigungen";
      this.groupBoxBerechtigungen.Size = new System.Drawing.Size(250, 233);
      this.groupBoxBerechtigungen.TabIndex = 5;
      this.groupBoxBerechtigungen.TabStop = false;
      this.groupBoxBerechtigungen.Text = "Berechtigungen";
      // 
      // btnBerechtigungen
      // 
      this.btnBerechtigungen.Location = new System.Drawing.Point(6, 50);
      this.btnBerechtigungen.Name = "btnBerechtigungen";
      this.btnBerechtigungen.Size = new System.Drawing.Size(164, 23);
      this.btnBerechtigungen.TabIndex = 1;
      this.btnBerechtigungen.Text = "Berechtigungen verwalten";
      this.btnBerechtigungen.UseVisualStyleBackColor = true;
      this.btnBerechtigungen.Click += new System.EventHandler(this.btnBerechtigungen_Click);
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
      // groupBoxEinstellungen
      // 
      this.groupBoxEinstellungen.Controls.Add(this.edBackupPfad);
      this.groupBoxEinstellungen.Controls.Add(this.lbBackupPfad);
      this.groupBoxEinstellungen.Controls.Add(this.label2);
      this.groupBoxEinstellungen.Controls.Add(this.comboBoxZeitpunkt);
      this.groupBoxEinstellungen.Controls.Add(this.label1);
      this.groupBoxEinstellungen.Controls.Add(this.edSchuljahr);
      this.groupBoxEinstellungen.Controls.Add(this.btnSave);
      this.groupBoxEinstellungen.Controls.Add(this.chkSperre);
      this.groupBoxEinstellungen.Location = new System.Drawing.Point(555, 268);
      this.groupBoxEinstellungen.Name = "groupBoxEinstellungen";
      this.groupBoxEinstellungen.Size = new System.Drawing.Size(250, 226);
      this.groupBoxEinstellungen.TabIndex = 6;
      this.groupBoxEinstellungen.TabStop = false;
      this.groupBoxEinstellungen.Text = "Globale Einstellungen";
      // 
      // edBackupPfad
      // 
      this.edBackupPfad.Location = new System.Drawing.Point(16, 158);
      this.edBackupPfad.Name = "edBackupPfad";
      this.edBackupPfad.Size = new System.Drawing.Size(217, 20);
      this.edBackupPfad.TabIndex = 17;
      // 
      // lbBackupPfad
      // 
      this.lbBackupPfad.AutoSize = true;
      this.lbBackupPfad.Location = new System.Drawing.Point(13, 142);
      this.lbBackupPfad.Name = "lbBackupPfad";
      this.lbBackupPfad.Size = new System.Drawing.Size(65, 13);
      this.lbBackupPfad.TabIndex = 16;
      this.lbBackupPfad.Text = "Backuppfad";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(17, 92);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(95, 13);
      this.label2.TabIndex = 14;
      this.label2.Text = "aktueller Zeitpunkt";
      // 
      // comboBoxZeitpunkt
      // 
      this.comboBoxZeitpunkt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxZeitpunkt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboBoxZeitpunkt.FormattingEnabled = true;
      this.comboBoxZeitpunkt.Items.AddRange(new object[] {
            "Probezeit BOS",
            "Halbjahr",
            "1. PA",
            "2. PA",
            "3. PA",
            "Jahresende"});
      this.comboBoxZeitpunkt.Location = new System.Drawing.Point(16, 108);
      this.comboBoxZeitpunkt.Name = "comboBoxZeitpunkt";
      this.comboBoxZeitpunkt.Size = new System.Drawing.Size(217, 24);
      this.comboBoxZeitpunkt.TabIndex = 13;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 23);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(153, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Schuljahresbeginn (Jahreszahl)";
      // 
      // edSchuljahr
      // 
      this.edSchuljahr.Location = new System.Drawing.Point(16, 39);
      this.edSchuljahr.Name = "edSchuljahr";
      this.edSchuljahr.Size = new System.Drawing.Size(100, 20);
      this.edSchuljahr.TabIndex = 2;
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(16, 195);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(100, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Übernehmen";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // chkSperre
      // 
      this.chkSperre.AutoSize = true;
      this.chkSperre.Location = new System.Drawing.Point(16, 68);
      this.chkSperre.Name = "chkSperre";
      this.chkSperre.Size = new System.Drawing.Size(165, 17);
      this.chkSperre.TabIndex = 0;
      this.chkSperre.Text = "Notenschluss (Abgabesperre)";
      this.chkSperre.UseVisualStyleBackColor = true;
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new System.Drawing.Point(16, 497);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(77, 13);
      this.lblStatus.TabIndex = 7;
      this.lblStatus.Text = "Statusmeldung";
      // 
      // UserControlAdministration
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.groupBoxEinstellungen);
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
      this.groupBoxEinstellungen.ResumeLayout(false);
      this.groupBoxEinstellungen.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBoxAnalyse;
    private System.Windows.Forms.GroupBox groupBoxDrucken;
    private System.Windows.Forms.GroupBox groupBoxExport;
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
    private System.Windows.Forms.Button btnBerechtigungen;
    private System.Windows.Forms.Button btnAttestpflicht;
    private System.Windows.Forms.GroupBox groupBoxEinstellungen;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edSchuljahr;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.CheckBox chkSperre;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox comboBoxZeitpunkt;
    private System.Windows.Forms.Label lbBackupPfad;
    private System.Windows.Forms.TextBox edBackupPfad;
    private System.Windows.Forms.Button btnCreateExcels;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnSendExcelFiles;
    private System.Windows.Forms.Button btnNotenNachWinSV;
    private System.Windows.Forms.Button btnKlassenliste;
  }
}
