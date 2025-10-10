using System.Drawing;

namespace diNo
{
  partial class UserControlSchueleransicht
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
      this.lblAdresse = new System.Windows.Forms.Label();
      this.textBoxStrasse = new System.Windows.Forms.TextBox();
      this.lblTelefonnummer = new System.Windows.Forms.Label();
      this.textBoxTelefonnummer = new System.Windows.Forms.TextBox();
      this.lblGeburtsdatum = new System.Windows.Forms.Label();
      this.textBoxGeburtsdatum = new System.Windows.Forms.TextBox();
      this.lblGeburtsort = new System.Windows.Forms.Label();
      this.textBoxGeburtsort = new System.Windows.Forms.TextBox();
      this.labelWiederholungen = new System.Windows.Forms.Label();
      this.textBoxWiederholungen = new System.Windows.Forms.TextBox();
      this.labelAdresseEltern = new System.Windows.Forms.Label();
      this.textBoxAdresseEltern = new System.Windows.Forms.TextBox();
      this.labelProbezeitBis = new System.Windows.Forms.Label();
      this.lblEmail = new System.Windows.Forms.Label();
      this.textBoxEmail = new System.Windows.Forms.TextBox();
      this.cbStatus = new System.Windows.Forms.ComboBox();
      this.textBoxPLZ = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.textBoxOrt = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.textBoxBekenntnis = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.textBoxNotfalltelefonnummer = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.textBoxJahrgangsstufe = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.textBoxEintrittAm = new System.Windows.Forms.TextBox();
      this.label11 = new System.Windows.Forms.Label();
      this.btnSave = new System.Windows.Forms.Button();
      this.dateTimeAustritt = new System.Windows.Forms.DateTimePicker();
      this.dateTimeProbezeit = new System.Windows.Forms.DateTimePicker();
      this.btnResetProbezeit = new System.Windows.Forms.Button();
      this.labelAustrittHinweis = new System.Windows.Forms.Label();
      this.textBoxVorigeSchule = new System.Windows.Forms.TextBox();
      this.labelVorigeSchule = new System.Windows.Forms.Label();
      this.textBoxBeruflicheVorbildung = new System.Windows.Forms.TextBox();
      this.labelBeruflicheVorbildung = new System.Windows.Forms.Label();
      this.textBoxMailSchule = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblAdresse
      // 
      this.lblAdresse.AutoSize = true;
      this.lblAdresse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblAdresse.Location = new System.Drawing.Point(24, 23);
      this.lblAdresse.Name = "lblAdresse";
      this.lblAdresse.Size = new System.Drawing.Size(38, 13);
      this.lblAdresse.TabIndex = 7;
      this.lblAdresse.Text = "Straße";
      // 
      // textBoxStrasse
      // 
      this.textBoxStrasse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxStrasse.Location = new System.Drawing.Point(27, 38);
      this.textBoxStrasse.Name = "textBoxStrasse";
      this.textBoxStrasse.Size = new System.Drawing.Size(240, 20);
      this.textBoxStrasse.TabIndex = 0;
      // 
      // lblTelefonnummer
      // 
      this.lblTelefonnummer.AutoSize = true;
      this.lblTelefonnummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTelefonnummer.Location = new System.Drawing.Point(24, 103);
      this.lblTelefonnummer.Name = "lblTelefonnummer";
      this.lblTelefonnummer.Size = new System.Drawing.Size(80, 13);
      this.lblTelefonnummer.TabIndex = 9;
      this.lblTelefonnummer.Text = "Telefonnummer";
      // 
      // textBoxTelefonnummer
      // 
      this.textBoxTelefonnummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxTelefonnummer.Location = new System.Drawing.Point(27, 118);
      this.textBoxTelefonnummer.Name = "textBoxTelefonnummer";
      this.textBoxTelefonnummer.Size = new System.Drawing.Size(240, 20);
      this.textBoxTelefonnummer.TabIndex = 3;
      // 
      // lblGeburtsdatum
      // 
      this.lblGeburtsdatum.AutoSize = true;
      this.lblGeburtsdatum.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblGeburtsdatum.Location = new System.Drawing.Point(26, 237);
      this.lblGeburtsdatum.Name = "lblGeburtsdatum";
      this.lblGeburtsdatum.Size = new System.Drawing.Size(63, 13);
      this.lblGeburtsdatum.TabIndex = 11;
      this.lblGeburtsdatum.Text = "geboren am";
      // 
      // textBoxGeburtsdatum
      // 
      this.textBoxGeburtsdatum.Enabled = false;
      this.textBoxGeburtsdatum.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxGeburtsdatum.Location = new System.Drawing.Point(27, 252);
      this.textBoxGeburtsdatum.Name = "textBoxGeburtsdatum";
      this.textBoxGeburtsdatum.Size = new System.Drawing.Size(83, 20);
      this.textBoxGeburtsdatum.TabIndex = 6;
      // 
      // lblGeburtsort
      // 
      this.lblGeburtsort.AutoSize = true;
      this.lblGeburtsort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblGeburtsort.Location = new System.Drawing.Point(115, 237);
      this.lblGeburtsort.Name = "lblGeburtsort";
      this.lblGeburtsort.Size = new System.Drawing.Size(56, 13);
      this.lblGeburtsort.TabIndex = 13;
      this.lblGeburtsort.Text = "Geburtsort";
      // 
      // textBoxGeburtsort
      // 
      this.textBoxGeburtsort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxGeburtsort.Location = new System.Drawing.Point(118, 252);
      this.textBoxGeburtsort.Name = "textBoxGeburtsort";
      this.textBoxGeburtsort.Size = new System.Drawing.Size(149, 20);
      this.textBoxGeburtsort.TabIndex = 7;
      // 
      // labelWiederholungen
      // 
      this.labelWiederholungen.AutoSize = true;
      this.labelWiederholungen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelWiederholungen.Location = new System.Drawing.Point(318, 139);
      this.labelWiederholungen.Name = "labelWiederholungen";
      this.labelWiederholungen.Size = new System.Drawing.Size(85, 13);
      this.labelWiederholungen.TabIndex = 19;
      this.labelWiederholungen.Text = "Wiederholungen";
      // 
      // textBoxWiederholungen
      // 
      this.textBoxWiederholungen.Enabled = false;
      this.textBoxWiederholungen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxWiederholungen.Location = new System.Drawing.Point(318, 155);
      this.textBoxWiederholungen.Name = "textBoxWiederholungen";
      this.textBoxWiederholungen.Size = new System.Drawing.Size(240, 20);
      this.textBoxWiederholungen.TabIndex = 18;
      // 
      // labelAdresseEltern
      // 
      this.labelAdresseEltern.AutoSize = true;
      this.labelAdresseEltern.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelAdresseEltern.Location = new System.Drawing.Point(26, 276);
      this.labelAdresseEltern.Name = "labelAdresseEltern";
      this.labelAdresseEltern.Size = new System.Drawing.Size(74, 13);
      this.labelAdresseEltern.TabIndex = 27;
      this.labelAdresseEltern.Text = "Kontakt Eltern";
      // 
      // textBoxAdresseEltern
      // 
      this.textBoxAdresseEltern.Enabled = false;
      this.textBoxAdresseEltern.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxAdresseEltern.Location = new System.Drawing.Point(27, 291);
      this.textBoxAdresseEltern.Multiline = true;
      this.textBoxAdresseEltern.Name = "textBoxAdresseEltern";
      this.textBoxAdresseEltern.Size = new System.Drawing.Size(240, 60);
      this.textBoxAdresseEltern.TabIndex = 8;
      // 
      // labelProbezeitBis
      // 
      this.labelProbezeitBis.AutoSize = true;
      this.labelProbezeitBis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelProbezeitBis.Location = new System.Drawing.Point(318, 100);
      this.labelProbezeitBis.Name = "labelProbezeitBis";
      this.labelProbezeitBis.Size = new System.Drawing.Size(67, 13);
      this.labelProbezeitBis.TabIndex = 29;
      this.labelProbezeitBis.Text = "Probezeit bis";
      // 
      // lblEmail
      // 
      this.lblEmail.AutoSize = true;
      this.lblEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEmail.Location = new System.Drawing.Point(25, 198);
      this.lblEmail.Name = "lblEmail";
      this.lblEmail.Size = new System.Drawing.Size(26, 13);
      this.lblEmail.TabIndex = 34;
      this.lblEmail.Text = "Mail";
      // 
      // textBoxEmail
      // 
      this.textBoxEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEmail.Location = new System.Drawing.Point(27, 213);
      this.textBoxEmail.Name = "textBoxEmail";
      this.textBoxEmail.Size = new System.Drawing.Size(240, 20);
      this.textBoxEmail.TabIndex = 5;
      // 
      // cbStatus
      // 
      this.cbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbStatus.FormattingEnabled = true;
      this.cbStatus.Items.AddRange(new object[] {
            "Aktiv",
            "Abgemeldet"});
      this.cbStatus.Location = new System.Drawing.Point(318, 38);
      this.cbStatus.Name = "cbStatus";
      this.cbStatus.Size = new System.Drawing.Size(240, 21);
      this.cbStatus.TabIndex = 13;
      this.cbStatus.SelectedIndexChanged += new System.EventHandler(this.cbStatus_SelectedIndexChanged);
      // 
      // textBoxPLZ
      // 
      this.textBoxPLZ.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxPLZ.Location = new System.Drawing.Point(27, 79);
      this.textBoxPLZ.Name = "textBoxPLZ";
      this.textBoxPLZ.Size = new System.Drawing.Size(60, 20);
      this.textBoxPLZ.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(24, 68);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(0, 13);
      this.label2.TabIndex = 37;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(24, 64);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(27, 13);
      this.label3.TabIndex = 38;
      this.label3.Text = "PLZ";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(90, 64);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(21, 13);
      this.label4.TabIndex = 40;
      this.label4.Text = "Ort";
      // 
      // textBoxOrt
      // 
      this.textBoxOrt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxOrt.Location = new System.Drawing.Point(93, 79);
      this.textBoxOrt.Name = "textBoxOrt";
      this.textBoxOrt.Size = new System.Drawing.Size(174, 20);
      this.textBoxOrt.TabIndex = 2;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(318, 23);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(101, 13);
      this.label5.TabIndex = 41;
      this.label5.Text = "Status des Schülers";
      // 
      // textBoxBekenntnis
      // 
      this.textBoxBekenntnis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxBekenntnis.Location = new System.Drawing.Point(28, 382);
      this.textBoxBekenntnis.Name = "textBoxBekenntnis";
      this.textBoxBekenntnis.Size = new System.Drawing.Size(82, 20);
      this.textBoxBekenntnis.TabIndex = 9;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(26, 366);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(60, 13);
      this.label6.TabIndex = 43;
      this.label6.Text = "Bekenntnis";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(450, 100);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(56, 13);
      this.label8.TabIndex = 47;
      this.label8.Text = "Austritt am";
      // 
      // textBoxNotfalltelefonnummer
      // 
      this.textBoxNotfalltelefonnummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxNotfalltelefonnummer.Location = new System.Drawing.Point(27, 155);
      this.textBoxNotfalltelefonnummer.Name = "textBoxNotfalltelefonnummer";
      this.textBoxNotfalltelefonnummer.Size = new System.Drawing.Size(240, 20);
      this.textBoxNotfalltelefonnummer.TabIndex = 4;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.Location = new System.Drawing.Point(26, 140);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(111, 13);
      this.label9.TabIndex = 49;
      this.label9.Text = "Mailadresse der Eltern";
      // 
      // textBoxJahrgangsstufe
      // 
      this.textBoxJahrgangsstufe.Enabled = false;
      this.textBoxJahrgangsstufe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxJahrgangsstufe.Location = new System.Drawing.Point(453, 77);
      this.textBoxJahrgangsstufe.Name = "textBoxJahrgangsstufe";
      this.textBoxJahrgangsstufe.Size = new System.Drawing.Size(105, 20);
      this.textBoxJahrgangsstufe.TabIndex = 15;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(450, 62);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(90, 13);
      this.label10.TabIndex = 53;
      this.label10.Text = "in Jahrgangsstufe";
      // 
      // textBoxEintrittAm
      // 
      this.textBoxEintrittAm.Enabled = false;
      this.textBoxEintrittAm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEintrittAm.Location = new System.Drawing.Point(318, 77);
      this.textBoxEintrittAm.Name = "textBoxEintrittAm";
      this.textBoxEintrittAm.Size = new System.Drawing.Size(103, 20);
      this.textBoxEintrittAm.TabIndex = 14;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(318, 62);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(53, 13);
      this.label11.TabIndex = 51;
      this.label11.Text = "Eintritt am";
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::diNo.Properties.Resources.Save;
      this.btnSave.Location = new System.Drawing.Point(518, 362);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 26;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Visible = false;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // dateTimeAustritt
      // 
      this.dateTimeAustritt.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimeAustritt.Location = new System.Drawing.Point(453, 113);
      this.dateTimeAustritt.Name = "dateTimeAustritt";
      this.dateTimeAustritt.Size = new System.Drawing.Size(105, 20);
      this.dateTimeAustritt.TabIndex = 17;
      this.dateTimeAustritt.ValueChanged += new System.EventHandler(this.dateTimeAustritt_ValueChanged);
      // 
      // dateTimeProbezeit
      // 
      this.dateTimeProbezeit.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimeProbezeit.Location = new System.Drawing.Point(321, 113);
      this.dateTimeProbezeit.Name = "dateTimeProbezeit";
      this.dateTimeProbezeit.Size = new System.Drawing.Size(100, 20);
      this.dateTimeProbezeit.TabIndex = 16;
      // 
      // btnResetProbezeit
      // 
      this.btnResetProbezeit.Image = global::diNo.Properties.Resources.muell;
      this.btnResetProbezeit.Location = new System.Drawing.Point(424, 111);
      this.btnResetProbezeit.Name = "btnResetProbezeit";
      this.btnResetProbezeit.Size = new System.Drawing.Size(20, 20);
      this.btnResetProbezeit.TabIndex = 73;
      this.btnResetProbezeit.UseVisualStyleBackColor = true;
      this.btnResetProbezeit.Visible = false;
      this.btnResetProbezeit.Click += new System.EventHandler(this.buttonResetProbezeit_Click);
      // 
      // labelAustrittHinweis
      // 
      this.labelAustrittHinweis.AutoSize = true;
      this.labelAustrittHinweis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelAustrittHinweis.Location = new System.Drawing.Point(450, 132);
      this.labelAustrittHinweis.Name = "labelAustrittHinweis";
      this.labelAustrittHinweis.Size = new System.Drawing.Size(149, 13);
      this.labelAustrittHinweis.TabIndex = 74;
      this.labelAustrittHinweis.Text = "Bei Austritt erst Status ändern!";
      this.labelAustrittHinweis.Visible = false;
      // 
      // textBoxVorigeSchule
      // 
      this.textBoxVorigeSchule.Enabled = false;
      this.textBoxVorigeSchule.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxVorigeSchule.Location = new System.Drawing.Point(318, 291);
      this.textBoxVorigeSchule.Multiline = true;
      this.textBoxVorigeSchule.Name = "textBoxVorigeSchule";
      this.textBoxVorigeSchule.Size = new System.Drawing.Size(240, 60);
      this.textBoxVorigeSchule.TabIndex = 85;
      // 
      // labelVorigeSchule
      // 
      this.labelVorigeSchule.AutoSize = true;
      this.labelVorigeSchule.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelVorigeSchule.Location = new System.Drawing.Point(315, 276);
      this.labelVorigeSchule.Name = "labelVorigeSchule";
      this.labelVorigeSchule.Size = new System.Drawing.Size(72, 13);
      this.labelVorigeSchule.TabIndex = 86;
      this.labelVorigeSchule.Text = "vorige Schule";
      // 
      // textBoxBeruflicheVorbildung
      // 
      this.textBoxBeruflicheVorbildung.Enabled = false;
      this.textBoxBeruflicheVorbildung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxBeruflicheVorbildung.Location = new System.Drawing.Point(318, 252);
      this.textBoxBeruflicheVorbildung.Name = "textBoxBeruflicheVorbildung";
      this.textBoxBeruflicheVorbildung.Size = new System.Drawing.Size(240, 20);
      this.textBoxBeruflicheVorbildung.TabIndex = 84;
      // 
      // labelBeruflicheVorbildung
      // 
      this.labelBeruflicheVorbildung.AutoSize = true;
      this.labelBeruflicheVorbildung.Enabled = false;
      this.labelBeruflicheVorbildung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelBeruflicheVorbildung.Location = new System.Drawing.Point(318, 237);
      this.labelBeruflicheVorbildung.Name = "labelBeruflicheVorbildung";
      this.labelBeruflicheVorbildung.Size = new System.Drawing.Size(106, 13);
      this.labelBeruflicheVorbildung.TabIndex = 83;
      this.labelBeruflicheVorbildung.Text = "berufliche Vorbildung";
      // 
      // textBoxMailSchule
      // 
      this.textBoxMailSchule.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxMailSchule.Location = new System.Drawing.Point(318, 213);
      this.textBoxMailSchule.Name = "textBoxMailSchule";
      this.textBoxMailSchule.Size = new System.Drawing.Size(240, 20);
      this.textBoxMailSchule.TabIndex = 87;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(316, 199);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(118, 13);
      this.label1.TabIndex = 88;
      this.label1.Text = "Schulische Mailadresse";
      // 
      // UserControlSchueleransicht
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.Controls.Add(this.textBoxMailSchule);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textBoxVorigeSchule);
      this.Controls.Add(this.labelVorigeSchule);
      this.Controls.Add(this.textBoxBeruflicheVorbildung);
      this.Controls.Add(this.labelBeruflicheVorbildung);
      this.Controls.Add(this.labelAustrittHinweis);
      this.Controls.Add(this.btnResetProbezeit);
      this.Controls.Add(this.dateTimeProbezeit);
      this.Controls.Add(this.dateTimeAustritt);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.textBoxJahrgangsstufe);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.textBoxEintrittAm);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.textBoxNotfalltelefonnummer);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.textBoxBekenntnis);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.textBoxOrt);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.textBoxPLZ);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.cbStatus);
      this.Controls.Add(this.textBoxEmail);
      this.Controls.Add(this.lblEmail);
      this.Controls.Add(this.labelProbezeitBis);
      this.Controls.Add(this.textBoxAdresseEltern);
      this.Controls.Add(this.labelAdresseEltern);
      this.Controls.Add(this.textBoxWiederholungen);
      this.Controls.Add(this.labelWiederholungen);
      this.Controls.Add(this.textBoxGeburtsort);
      this.Controls.Add(this.lblGeburtsort);
      this.Controls.Add(this.textBoxGeburtsdatum);
      this.Controls.Add(this.lblGeburtsdatum);
      this.Controls.Add(this.textBoxTelefonnummer);
      this.Controls.Add(this.lblTelefonnummer);
      this.Controls.Add(this.textBoxStrasse);
      this.Controls.Add(this.lblAdresse);
      this.Name = "UserControlSchueleransicht";
      this.Size = new System.Drawing.Size(614, 462);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label lblAdresse;
    private System.Windows.Forms.TextBox textBoxStrasse;
    private System.Windows.Forms.Label lblTelefonnummer;
    private System.Windows.Forms.TextBox textBoxTelefonnummer;
    private System.Windows.Forms.Label lblGeburtsdatum;
    private System.Windows.Forms.TextBox textBoxGeburtsdatum;
    private System.Windows.Forms.Label lblGeburtsort;
    private System.Windows.Forms.TextBox textBoxGeburtsort;
    private System.Windows.Forms.Label labelWiederholungen;
    private System.Windows.Forms.TextBox textBoxWiederholungen;
    private System.Windows.Forms.Label labelAdresseEltern;
    private System.Windows.Forms.TextBox textBoxAdresseEltern;
    private System.Windows.Forms.Label labelProbezeitBis;
    private System.Windows.Forms.Label lblEmail;
    private System.Windows.Forms.TextBox textBoxEmail;
    private System.Windows.Forms.ComboBox cbStatus;
    private System.Windows.Forms.TextBox textBoxPLZ;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox textBoxOrt;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBoxBekenntnis;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox textBoxNotfalltelefonnummer;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox textBoxJahrgangsstufe;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox textBoxEintrittAm;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.DateTimePicker dateTimeAustritt;
    private System.Windows.Forms.DateTimePicker dateTimeProbezeit;
    private System.Windows.Forms.Button btnResetProbezeit;
    private System.Windows.Forms.Label labelAustrittHinweis;
    private System.Windows.Forms.TextBox textBoxVorigeSchule;
    private System.Windows.Forms.Label labelVorigeSchule;
    private System.Windows.Forms.TextBox textBoxBeruflicheVorbildung;
    private System.Windows.Forms.Label labelBeruflicheVorbildung;
        private System.Windows.Forms.TextBox textBoxMailSchule;
        private System.Windows.Forms.Label label1;
    }
}
