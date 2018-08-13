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
      this.btnEinserAbi = new System.Windows.Forms.Button();
      this.btnSelect = new System.Windows.Forms.Button();
      this.groupBoxDrucken = new System.Windows.Forms.GroupBox();
      this.gbUnterschrift = new System.Windows.Forms.GroupBox();
      this.opGez = new System.Windows.Forms.RadioButton();
      this.opStv = new System.Windows.Forms.RadioButton();
      this.opSL = new System.Windows.Forms.RadioButton();
      this.lbZeugnis = new System.Windows.Forms.Label();
      this.dateZeugnis = new System.Windows.Forms.DateTimePicker();
      this.cbNotendruck = new System.Windows.Forms.ComboBox();
      this.btnKlassenliste = new System.Windows.Forms.Button();
      this.btnAttestpflicht = new System.Windows.Forms.Button();
      this.btnNotendruck = new System.Windows.Forms.Button();
      this.groupBoxExport = new System.Windows.Forms.GroupBox();
      this.btnExportSchuelerZeugnisprogramm = new System.Windows.Forms.Button();
      this.btnNotenNachWinSV = new System.Windows.Forms.Button();
      this.btnSendExcelFiles = new System.Windows.Forms.Button();
      this.btnCreateExcels = new System.Windows.Forms.Button();
      this.exportNoten = new System.Windows.Forms.Button();
      this.groupBoxImport = new System.Windows.Forms.GroupBox();
      this.btnImportKlassenleiter = new System.Windows.Forms.Button();
      this.btnImportSchueler = new System.Windows.Forms.Button();
      this.btnImportUnterricht = new System.Windows.Forms.Button();
      this.importNoten = new System.Windows.Forms.Button();
      this.groupBoxStammdaten = new System.Windows.Forms.GroupBox();
      this.btnGlobales = new System.Windows.Forms.Button();
      this.btnLehrer = new System.Windows.Forms.Button();
      this.btnKurse = new System.Windows.Forms.Button();
      this.groupBoxEinstellungen = new System.Windows.Forms.GroupBox();
      this.gbLeseModusExcel = new System.Windows.Forms.GroupBox();
      this.opVollstaendig = new System.Windows.Forms.RadioButton();
      this.opNurAktuelleNoten = new System.Windows.Forms.RadioButton();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBoxZeitpunkt = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.edSchuljahr = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.chkSperre = new System.Windows.Forms.CheckBox();
      this.lblStatus = new System.Windows.Forms.Label();
      this.btnBerechtigungen = new System.Windows.Forms.Button();
      this.groupBoxAnalyse.SuspendLayout();
      this.groupBoxDrucken.SuspendLayout();
      this.gbUnterschrift.SuspendLayout();
      this.groupBoxExport.SuspendLayout();
      this.groupBoxImport.SuspendLayout();
      this.groupBoxStammdaten.SuspendLayout();
      this.groupBoxEinstellungen.SuspendLayout();
      this.gbLeseModusExcel.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBoxAnalyse
      // 
      this.groupBoxAnalyse.Controls.Add(this.btnEinserAbi);
      this.groupBoxAnalyse.Controls.Add(this.btnSelect);
      this.groupBoxAnalyse.Location = new System.Drawing.Point(19, 268);
      this.groupBoxAnalyse.Name = "groupBoxAnalyse";
      this.groupBoxAnalyse.Size = new System.Drawing.Size(250, 226);
      this.groupBoxAnalyse.TabIndex = 4;
      this.groupBoxAnalyse.TabStop = false;
      this.groupBoxAnalyse.Text = "Datenanalyse";
      // 
      // btnEinserAbi
      // 
      this.btnEinserAbi.Location = new System.Drawing.Point(21, 52);
      this.btnEinserAbi.Name = "btnEinserAbi";
      this.btnEinserAbi.Size = new System.Drawing.Size(132, 23);
      this.btnEinserAbi.TabIndex = 3;
      this.btnEinserAbi.Text = "Einser-Abi";
      this.btnEinserAbi.UseVisualStyleBackColor = true;
      this.btnEinserAbi.Click += new System.EventHandler(this.btnEinserAbi_Click);
      // 
      // btnSelect
      // 
      this.btnSelect.Location = new System.Drawing.Point(21, 23);
      this.btnSelect.Name = "btnSelect";
      this.btnSelect.Size = new System.Drawing.Size(132, 23);
      this.btnSelect.TabIndex = 2;
      this.btnSelect.Text = "Schüler auswählen";
      this.btnSelect.UseVisualStyleBackColor = true;
      this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
      // 
      // groupBoxDrucken
      // 
      this.groupBoxDrucken.Controls.Add(this.gbUnterschrift);
      this.groupBoxDrucken.Controls.Add(this.lbZeugnis);
      this.groupBoxDrucken.Controls.Add(this.dateZeugnis);
      this.groupBoxDrucken.Controls.Add(this.cbNotendruck);
      this.groupBoxDrucken.Controls.Add(this.btnKlassenliste);
      this.groupBoxDrucken.Controls.Add(this.btnAttestpflicht);
      this.groupBoxDrucken.Controls.Add(this.btnNotendruck);
      this.groupBoxDrucken.Location = new System.Drawing.Point(19, 28);
      this.groupBoxDrucken.Name = "groupBoxDrucken";
      this.groupBoxDrucken.Size = new System.Drawing.Size(250, 233);
      this.groupBoxDrucken.TabIndex = 3;
      this.groupBoxDrucken.TabStop = false;
      this.groupBoxDrucken.Text = "Drucken";
      // 
      // gbUnterschrift
      // 
      this.gbUnterschrift.Controls.Add(this.opGez);
      this.gbUnterschrift.Controls.Add(this.opStv);
      this.gbUnterschrift.Controls.Add(this.opSL);
      this.gbUnterschrift.Location = new System.Drawing.Point(159, 19);
      this.gbUnterschrift.Name = "gbUnterschrift";
      this.gbUnterschrift.Size = new System.Drawing.Size(68, 77);
      this.gbUnterschrift.TabIndex = 31;
      this.gbUnterschrift.TabStop = false;
      this.gbUnterschrift.Text = "Unterschr";
      // 
      // opGez
      // 
      this.opGez.AutoSize = true;
      this.opGez.Location = new System.Drawing.Point(14, 52);
      this.opGez.Name = "opGez";
      this.opGez.Size = new System.Drawing.Size(42, 17);
      this.opGez.TabIndex = 2;
      this.opGez.TabStop = true;
      this.opGez.Text = "gez";
      this.opGez.UseVisualStyleBackColor = true;
      // 
      // opStv
      // 
      this.opStv.AutoSize = true;
      this.opStv.Location = new System.Drawing.Point(14, 34);
      this.opStv.Name = "opStv";
      this.opStv.Size = new System.Drawing.Size(41, 17);
      this.opStv.TabIndex = 1;
      this.opStv.TabStop = true;
      this.opStv.Text = "Stv";
      this.opStv.UseVisualStyleBackColor = true;
      // 
      // opSL
      // 
      this.opSL.AutoSize = true;
      this.opSL.Checked = true;
      this.opSL.Location = new System.Drawing.Point(14, 16);
      this.opSL.Name = "opSL";
      this.opSL.Size = new System.Drawing.Size(38, 17);
      this.opSL.TabIndex = 0;
      this.opSL.TabStop = true;
      this.opSL.Text = "SL";
      this.opSL.UseVisualStyleBackColor = true;
      // 
      // lbZeugnis
      // 
      this.lbZeugnis.AutoSize = true;
      this.lbZeugnis.Location = new System.Drawing.Point(18, 37);
      this.lbZeugnis.Name = "lbZeugnis";
      this.lbZeugnis.Size = new System.Drawing.Size(74, 13);
      this.lbZeugnis.TabIndex = 30;
      this.lbZeugnis.Text = "Zeugnisdatum";
      // 
      // dateZeugnis
      // 
      this.dateZeugnis.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateZeugnis.Location = new System.Drawing.Point(21, 53);
      this.dateZeugnis.Name = "dateZeugnis";
      this.dateZeugnis.Size = new System.Drawing.Size(132, 20);
      this.dateZeugnis.TabIndex = 29;
      // 
      // cbNotendruck
      // 
      this.cbNotendruck.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbNotendruck.FormattingEnabled = true;
      this.cbNotendruck.Items.AddRange(new object[] {
            "Notenmitteilung",
            "Gefährdungen",
            "Abiergebnisse",
            "Bescheinigung",
            "Zwischenzeugnis",
            "Jahreszeugnis",
            "Abiturzeugnis"});
      this.cbNotendruck.Location = new System.Drawing.Point(21, 102);
      this.cbNotendruck.Name = "cbNotendruck";
      this.cbNotendruck.Size = new System.Drawing.Size(132, 21);
      this.cbNotendruck.TabIndex = 7;
      // 
      // btnKlassenliste
      // 
      this.btnKlassenliste.Location = new System.Drawing.Point(21, 166);
      this.btnKlassenliste.Name = "btnKlassenliste";
      this.btnKlassenliste.Size = new System.Drawing.Size(132, 23);
      this.btnKlassenliste.TabIndex = 3;
      this.btnKlassenliste.Text = "Klassenliste";
      this.btnKlassenliste.UseVisualStyleBackColor = true;
      this.btnKlassenliste.Click += new System.EventHandler(this.btnKlassenliste_Click);
      // 
      // btnAttestpflicht
      // 
      this.btnAttestpflicht.Location = new System.Drawing.Point(21, 195);
      this.btnAttestpflicht.Name = "btnAttestpflicht";
      this.btnAttestpflicht.Size = new System.Drawing.Size(132, 23);
      this.btnAttestpflicht.TabIndex = 2;
      this.btnAttestpflicht.Text = "Attestpflicht";
      this.btnAttestpflicht.UseVisualStyleBackColor = true;
      this.btnAttestpflicht.Click += new System.EventHandler(this.btnAttestpflicht_Click);
      // 
      // btnNotendruck
      // 
      this.btnNotendruck.Location = new System.Drawing.Point(159, 102);
      this.btnNotendruck.Name = "btnNotendruck";
      this.btnNotendruck.Size = new System.Drawing.Size(69, 23);
      this.btnNotendruck.TabIndex = 1;
      this.btnNotendruck.Text = "drucken";
      this.btnNotendruck.UseVisualStyleBackColor = true;
      this.btnNotendruck.Click += new System.EventHandler(this.btnNotendruck_Click);
      // 
      // groupBoxExport
      // 
      this.groupBoxExport.Controls.Add(this.btnExportSchuelerZeugnisprogramm);
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
      // btnExportSchuelerZeugnisprogramm
      // 
      this.btnExportSchuelerZeugnisprogramm.Location = new System.Drawing.Point(6, 134);
      this.btnExportSchuelerZeugnisprogramm.Name = "btnExportSchuelerZeugnisprogramm";
      this.btnExportSchuelerZeugnisprogramm.Size = new System.Drawing.Size(182, 23);
      this.btnExportSchuelerZeugnisprogramm.TabIndex = 6;
      this.btnExportSchuelerZeugnisprogramm.Text = "Schüler an Zeugnisprogramm";
      this.btnExportSchuelerZeugnisprogramm.UseVisualStyleBackColor = true;
      this.btnExportSchuelerZeugnisprogramm.Click += new System.EventHandler(this.btnExportSchuelerZeugnisprogramm_Click);
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
      // groupBoxStammdaten
      // 
      this.groupBoxStammdaten.Controls.Add(this.btnBerechtigungen);
      this.groupBoxStammdaten.Controls.Add(this.btnGlobales);
      this.groupBoxStammdaten.Controls.Add(this.btnLehrer);
      this.groupBoxStammdaten.Controls.Add(this.btnKurse);
      this.groupBoxStammdaten.Location = new System.Drawing.Point(543, 277);
      this.groupBoxStammdaten.Name = "groupBoxStammdaten";
      this.groupBoxStammdaten.Size = new System.Drawing.Size(211, 217);
      this.groupBoxStammdaten.TabIndex = 5;
      this.groupBoxStammdaten.TabStop = false;
      this.groupBoxStammdaten.Text = "Stammdaten";
      // 
      // btnGlobales
      // 
      this.btnGlobales.Location = new System.Drawing.Point(17, 135);
      this.btnGlobales.Name = "btnGlobales";
      this.btnGlobales.Size = new System.Drawing.Size(163, 23);
      this.btnGlobales.TabIndex = 3;
      this.btnGlobales.Text = "Globale Texte";
      this.btnGlobales.UseVisualStyleBackColor = true;
      this.btnGlobales.Click += new System.EventHandler(this.btnGlobales_Click);
      // 
      // btnLehrer
      // 
      this.btnLehrer.Location = new System.Drawing.Point(16, 27);
      this.btnLehrer.Name = "btnLehrer";
      this.btnLehrer.Size = new System.Drawing.Size(164, 23);
      this.btnLehrer.TabIndex = 2;
      this.btnLehrer.Text = "Lehrer";
      this.btnLehrer.UseVisualStyleBackColor = true;
      this.btnLehrer.Click += new System.EventHandler(this.btnLehrer_Click);
      // 
      // btnKurse
      // 
      this.btnKurse.Location = new System.Drawing.Point(17, 56);
      this.btnKurse.Name = "btnKurse";
      this.btnKurse.Size = new System.Drawing.Size(164, 23);
      this.btnKurse.TabIndex = 0;
      this.btnKurse.Text = "Kurse";
      this.btnKurse.UseVisualStyleBackColor = true;
      this.btnKurse.Click += new System.EventHandler(this.btnKurs_Click);
      // 
      // groupBoxEinstellungen
      // 
      this.groupBoxEinstellungen.Controls.Add(this.gbLeseModusExcel);
      this.groupBoxEinstellungen.Controls.Add(this.label2);
      this.groupBoxEinstellungen.Controls.Add(this.comboBoxZeitpunkt);
      this.groupBoxEinstellungen.Controls.Add(this.label1);
      this.groupBoxEinstellungen.Controls.Add(this.edSchuljahr);
      this.groupBoxEinstellungen.Controls.Add(this.btnSave);
      this.groupBoxEinstellungen.Controls.Add(this.chkSperre);
      this.groupBoxEinstellungen.Location = new System.Drawing.Point(543, 28);
      this.groupBoxEinstellungen.Name = "groupBoxEinstellungen";
      this.groupBoxEinstellungen.Size = new System.Drawing.Size(211, 233);
      this.groupBoxEinstellungen.TabIndex = 6;
      this.groupBoxEinstellungen.TabStop = false;
      this.groupBoxEinstellungen.Text = "Globale Einstellungen";
      // 
      // gbLeseModusExcel
      // 
      this.gbLeseModusExcel.Controls.Add(this.opVollstaendig);
      this.gbLeseModusExcel.Controls.Add(this.opNurAktuelleNoten);
      this.gbLeseModusExcel.Location = new System.Drawing.Point(16, 127);
      this.gbLeseModusExcel.Name = "gbLeseModusExcel";
      this.gbLeseModusExcel.Size = new System.Drawing.Size(144, 61);
      this.gbLeseModusExcel.TabIndex = 15;
      this.gbLeseModusExcel.TabStop = false;
      this.gbLeseModusExcel.Text = "Lesemodus Exceldateien";
      // 
      // opVollstaendig
      // 
      this.opVollstaendig.AutoSize = true;
      this.opVollstaendig.Location = new System.Drawing.Point(10, 35);
      this.opVollstaendig.Name = "opVollstaendig";
      this.opVollstaendig.Size = new System.Drawing.Size(118, 17);
      this.opVollstaendig.TabIndex = 2;
      this.opVollstaendig.Text = "Vollständig einlesen";
      this.opVollstaendig.UseVisualStyleBackColor = true;
      // 
      // opNurAktuelleNoten
      // 
      this.opNurAktuelleNoten.AutoSize = true;
      this.opNurAktuelleNoten.Checked = true;
      this.opNurAktuelleNoten.Location = new System.Drawing.Point(10, 17);
      this.opNurAktuelleNoten.Name = "opNurAktuelleNoten";
      this.opNurAktuelleNoten.Size = new System.Drawing.Size(114, 17);
      this.opNurAktuelleNoten.TabIndex = 1;
      this.opNurAktuelleNoten.TabStop = true;
      this.opNurAktuelleNoten.Text = "Nur aktuelle Noten";
      this.opNurAktuelleNoten.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(13, 81);
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
      this.comboBoxZeitpunkt.Location = new System.Drawing.Point(16, 97);
      this.comboBoxZeitpunkt.Name = "comboBoxZeitpunkt";
      this.comboBoxZeitpunkt.Size = new System.Drawing.Size(175, 24);
      this.comboBoxZeitpunkt.TabIndex = 13;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(94, 26);
      this.label1.TabIndex = 3;
      this.label1.Text = "Schuljahresbeginn\r\n(Jahreszahl)";
      // 
      // edSchuljahr
      // 
      this.edSchuljahr.Location = new System.Drawing.Point(113, 18);
      this.edSchuljahr.Name = "edSchuljahr";
      this.edSchuljahr.Size = new System.Drawing.Size(78, 20);
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
      this.chkSperre.Location = new System.Drawing.Point(16, 53);
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
      // btnBerechtigungen
      // 
      this.btnBerechtigungen.Location = new System.Drawing.Point(17, 179);
      this.btnBerechtigungen.Name = "btnBerechtigungen";
      this.btnBerechtigungen.Size = new System.Drawing.Size(163, 23);
      this.btnBerechtigungen.TabIndex = 5;
      this.btnBerechtigungen.Text = "Berechtigungen";
      this.btnBerechtigungen.UseVisualStyleBackColor = true;
      this.btnBerechtigungen.Click += new System.EventHandler(this.btnBerechtigungen_Click);
      // 
      // UserControlAdministration
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.groupBoxEinstellungen);
      this.Controls.Add(this.groupBoxStammdaten);
      this.Controls.Add(this.groupBoxExport);
      this.Controls.Add(this.groupBoxImport);
      this.Controls.Add(this.groupBoxAnalyse);
      this.Controls.Add(this.groupBoxDrucken);
      this.Name = "UserControlAdministration";
      this.Size = new System.Drawing.Size(907, 530);
      this.groupBoxAnalyse.ResumeLayout(false);
      this.groupBoxDrucken.ResumeLayout(false);
      this.groupBoxDrucken.PerformLayout();
      this.gbUnterschrift.ResumeLayout(false);
      this.gbUnterschrift.PerformLayout();
      this.groupBoxExport.ResumeLayout(false);
      this.groupBoxImport.ResumeLayout(false);
      this.groupBoxStammdaten.ResumeLayout(false);
      this.groupBoxEinstellungen.ResumeLayout(false);
      this.groupBoxEinstellungen.PerformLayout();
      this.gbLeseModusExcel.ResumeLayout(false);
      this.gbLeseModusExcel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBoxAnalyse;
    private System.Windows.Forms.GroupBox groupBoxDrucken;
    private System.Windows.Forms.GroupBox groupBoxExport;
    private System.Windows.Forms.GroupBox groupBoxImport;
    private System.Windows.Forms.Button exportNoten;
    private System.Windows.Forms.Button importNoten;
    private System.Windows.Forms.Button btnImportUnterricht;
    private System.Windows.Forms.Button btnImportSchueler;
    private System.Windows.Forms.Button btnImportKlassenleiter;
    private System.Windows.Forms.GroupBox groupBoxStammdaten;
    private System.Windows.Forms.Button btnKurse;
    private System.Windows.Forms.Button btnAttestpflicht;
    private System.Windows.Forms.GroupBox groupBoxEinstellungen;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edSchuljahr;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.CheckBox chkSperre;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox comboBoxZeitpunkt;
    private System.Windows.Forms.Button btnCreateExcels;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnSendExcelFiles;
    private System.Windows.Forms.Button btnNotenNachWinSV;
    private System.Windows.Forms.Button btnKlassenliste;
    private System.Windows.Forms.Button btnSelect;
    private System.Windows.Forms.Button btnExportSchuelerZeugnisprogramm;
    private System.Windows.Forms.ComboBox cbNotendruck;
    private System.Windows.Forms.Button btnNotendruck;
    private System.Windows.Forms.Label lbZeugnis;
    private System.Windows.Forms.DateTimePicker dateZeugnis;
    private System.Windows.Forms.GroupBox gbUnterschrift;
    private System.Windows.Forms.RadioButton opGez;
    private System.Windows.Forms.RadioButton opStv;
    private System.Windows.Forms.RadioButton opSL;
    private System.Windows.Forms.GroupBox gbLeseModusExcel;
    private System.Windows.Forms.RadioButton opVollstaendig;
    private System.Windows.Forms.RadioButton opNurAktuelleNoten;
    private System.Windows.Forms.Button btnLehrer;
    private System.Windows.Forms.Button btnGlobales;
    private System.Windows.Forms.Button btnEinserAbi;
    private System.Windows.Forms.Button btnBerechtigungen;
  }
}
