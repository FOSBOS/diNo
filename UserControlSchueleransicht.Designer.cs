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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlSchueleransicht));
            this.nameLabel = new System.Windows.Forms.Label();
            this.lblAdresse = new System.Windows.Forms.Label();
            this.textBoxAdresse = new System.Windows.Forms.TextBox();
            this.lblTelefonnummer = new System.Windows.Forms.Label();
            this.textBoxTelefonnummer = new System.Windows.Forms.TextBox();
            this.lblGeburtsdatum = new System.Windows.Forms.Label();
            this.textBoxGeburtsdatum = new System.Windows.Forms.TextBox();
            this.lblGeburtsort = new System.Windows.Forms.Label();
            this.textBoxGeburtsort = new System.Windows.Forms.TextBox();
            this.lblSchulischeVorbildung = new System.Windows.Forms.Label();
            this.textBoxSchulischeVorbildung = new System.Windows.Forms.TextBox();
            this.labelBeruflicheVorbildung = new System.Windows.Forms.Label();
            this.textBoxBeruflicheVorbildung = new System.Windows.Forms.TextBox();
            this.labelWiederholungen = new System.Windows.Forms.Label();
            this.textBoxWiederholungen = new System.Windows.Forms.TextBox();
            this.labelEintrittInJahrgangsstufe = new System.Windows.Forms.Label();
            this.textBoxJahrgangsstufe = new System.Windows.Forms.TextBox();
            this.labelAm = new System.Windows.Forms.Label();
            this.textBoxEintrittAm = new System.Windows.Forms.TextBox();
            this.labelVorigeSchule = new System.Windows.Forms.Label();
            this.textBoxVorigeSchule = new System.Windows.Forms.TextBox();
            this.labelAdresseEltern = new System.Windows.Forms.Label();
            this.textBoxAdresseEltern = new System.Windows.Forms.TextBox();
            this.labelProbezeitBis = new System.Windows.Forms.Label();
            this.textBoxProbezeit = new System.Windows.Forms.TextBox();
            this.pictureBoxImage = new System.Windows.Forms.PictureBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.nameLabel.Location = new System.Drawing.Point(74, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(160, 31);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Fritz Huber";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAdresse
            // 
            this.lblAdresse.AutoSize = true;
            this.lblAdresse.Location = new System.Drawing.Point(3, 92);
            this.lblAdresse.Name = "lblAdresse";
            this.lblAdresse.Size = new System.Drawing.Size(45, 13);
            this.lblAdresse.TabIndex = 7;
            this.lblAdresse.Text = "Adresse";
            // 
            // textBoxAdresse
            // 
            this.textBoxAdresse.Location = new System.Drawing.Point(97, 89);
            this.textBoxAdresse.Multiline = true;
            this.textBoxAdresse.Name = "textBoxAdresse";
            this.textBoxAdresse.Size = new System.Drawing.Size(178, 53);
            this.textBoxAdresse.TabIndex = 8;
            // 
            // lblTelefonnummer
            // 
            this.lblTelefonnummer.AutoSize = true;
            this.lblTelefonnummer.Location = new System.Drawing.Point(3, 157);
            this.lblTelefonnummer.Name = "lblTelefonnummer";
            this.lblTelefonnummer.Size = new System.Drawing.Size(80, 13);
            this.lblTelefonnummer.TabIndex = 9;
            this.lblTelefonnummer.Text = "Telefonnummer";
            // 
            // textBoxTelefonnummer
            // 
            this.textBoxTelefonnummer.Location = new System.Drawing.Point(97, 154);
            this.textBoxTelefonnummer.Name = "textBoxTelefonnummer";
            this.textBoxTelefonnummer.Size = new System.Drawing.Size(178, 20);
            this.textBoxTelefonnummer.TabIndex = 10;
            // 
            // lblGeburtsdatum
            // 
            this.lblGeburtsdatum.AutoSize = true;
            this.lblGeburtsdatum.Location = new System.Drawing.Point(369, 58);
            this.lblGeburtsdatum.Name = "lblGeburtsdatum";
            this.lblGeburtsdatum.Size = new System.Drawing.Size(63, 13);
            this.lblGeburtsdatum.TabIndex = 11;
            this.lblGeburtsdatum.Text = "geboren am";
            // 
            // textBoxGeburtsdatum
            // 
            this.textBoxGeburtsdatum.Location = new System.Drawing.Point(485, 55);
            this.textBoxGeburtsdatum.Name = "textBoxGeburtsdatum";
            this.textBoxGeburtsdatum.Size = new System.Drawing.Size(185, 20);
            this.textBoxGeburtsdatum.TabIndex = 12;
            // 
            // lblGeburtsort
            // 
            this.lblGeburtsort.AutoSize = true;
            this.lblGeburtsort.Location = new System.Drawing.Point(369, 85);
            this.lblGeburtsort.Name = "lblGeburtsort";
            this.lblGeburtsort.Size = new System.Drawing.Size(56, 13);
            this.lblGeburtsort.TabIndex = 13;
            this.lblGeburtsort.Text = "Geburtsort";
            // 
            // textBoxGeburtsort
            // 
            this.textBoxGeburtsort.Location = new System.Drawing.Point(485, 82);
            this.textBoxGeburtsort.Name = "textBoxGeburtsort";
            this.textBoxGeburtsort.Size = new System.Drawing.Size(185, 20);
            this.textBoxGeburtsort.TabIndex = 14;
            // 
            // lblSchulischeVorbildung
            // 
            this.lblSchulischeVorbildung.AutoSize = true;
            this.lblSchulischeVorbildung.Location = new System.Drawing.Point(369, 111);
            this.lblSchulischeVorbildung.Name = "lblSchulischeVorbildung";
            this.lblSchulischeVorbildung.Size = new System.Drawing.Size(110, 13);
            this.lblSchulischeVorbildung.TabIndex = 15;
            this.lblSchulischeVorbildung.Text = "schulische Vorbildung";
            // 
            // textBoxSchulischeVorbildung
            // 
            this.textBoxSchulischeVorbildung.Location = new System.Drawing.Point(485, 108);
            this.textBoxSchulischeVorbildung.Name = "textBoxSchulischeVorbildung";
            this.textBoxSchulischeVorbildung.Size = new System.Drawing.Size(185, 20);
            this.textBoxSchulischeVorbildung.TabIndex = 16;
            // 
            // labelBeruflicheVorbildung
            // 
            this.labelBeruflicheVorbildung.AutoSize = true;
            this.labelBeruflicheVorbildung.Location = new System.Drawing.Point(369, 137);
            this.labelBeruflicheVorbildung.Name = "labelBeruflicheVorbildung";
            this.labelBeruflicheVorbildung.Size = new System.Drawing.Size(106, 13);
            this.labelBeruflicheVorbildung.TabIndex = 17;
            this.labelBeruflicheVorbildung.Text = "berufliche Vorbildung";
            // 
            // textBoxBeruflicheVorbildung
            // 
            this.textBoxBeruflicheVorbildung.Location = new System.Drawing.Point(485, 134);
            this.textBoxBeruflicheVorbildung.Name = "textBoxBeruflicheVorbildung";
            this.textBoxBeruflicheVorbildung.Size = new System.Drawing.Size(185, 20);
            this.textBoxBeruflicheVorbildung.TabIndex = 18;
            // 
            // labelWiederholungen
            // 
            this.labelWiederholungen.AutoSize = true;
            this.labelWiederholungen.Location = new System.Drawing.Point(369, 174);
            this.labelWiederholungen.Name = "labelWiederholungen";
            this.labelWiederholungen.Size = new System.Drawing.Size(85, 13);
            this.labelWiederholungen.TabIndex = 19;
            this.labelWiederholungen.Text = "Wiederholungen";
            // 
            // textBoxWiederholungen
            // 
            this.textBoxWiederholungen.Location = new System.Drawing.Point(485, 171);
            this.textBoxWiederholungen.Name = "textBoxWiederholungen";
            this.textBoxWiederholungen.Size = new System.Drawing.Size(185, 20);
            this.textBoxWiederholungen.TabIndex = 20;
            // 
            // labelEintrittInJahrgangsstufe
            // 
            this.labelEintrittInJahrgangsstufe.AutoSize = true;
            this.labelEintrittInJahrgangsstufe.Location = new System.Drawing.Point(369, 202);
            this.labelEintrittInJahrgangsstufe.Name = "labelEintrittInJahrgangsstufe";
            this.labelEintrittInJahrgangsstufe.Size = new System.Drawing.Size(102, 13);
            this.labelEintrittInJahrgangsstufe.TabIndex = 21;
            this.labelEintrittInJahrgangsstufe.Text = "Eintritt in Jahrg.stufe";
            // 
            // textBoxJahrgangsstufe
            // 
            this.textBoxJahrgangsstufe.Location = new System.Drawing.Point(485, 199);
            this.textBoxJahrgangsstufe.Name = "textBoxJahrgangsstufe";
            this.textBoxJahrgangsstufe.Size = new System.Drawing.Size(40, 20);
            this.textBoxJahrgangsstufe.TabIndex = 22;
            // 
            // labelAm
            // 
            this.labelAm.AutoSize = true;
            this.labelAm.Location = new System.Drawing.Point(543, 202);
            this.labelAm.Name = "labelAm";
            this.labelAm.Size = new System.Drawing.Size(21, 13);
            this.labelAm.TabIndex = 23;
            this.labelAm.Text = "am";
            // 
            // textBoxEintrittAm
            // 
            this.textBoxEintrittAm.Location = new System.Drawing.Point(570, 199);
            this.textBoxEintrittAm.Name = "textBoxEintrittAm";
            this.textBoxEintrittAm.Size = new System.Drawing.Size(100, 20);
            this.textBoxEintrittAm.TabIndex = 24;
            // 
            // labelVorigeSchule
            // 
            this.labelVorigeSchule.AutoSize = true;
            this.labelVorigeSchule.Location = new System.Drawing.Point(369, 230);
            this.labelVorigeSchule.Name = "labelVorigeSchule";
            this.labelVorigeSchule.Size = new System.Drawing.Size(72, 13);
            this.labelVorigeSchule.TabIndex = 25;
            this.labelVorigeSchule.Text = "vorige Schule";
            // 
            // textBoxVorigeSchule
            // 
            this.textBoxVorigeSchule.Location = new System.Drawing.Point(485, 227);
            this.textBoxVorigeSchule.Name = "textBoxVorigeSchule";
            this.textBoxVorigeSchule.Size = new System.Drawing.Size(185, 20);
            this.textBoxVorigeSchule.TabIndex = 26;
            // 
            // labelAdresseEltern
            // 
            this.labelAdresseEltern.AutoSize = true;
            this.labelAdresseEltern.Location = new System.Drawing.Point(3, 202);
            this.labelAdresseEltern.Name = "labelAdresseEltern";
            this.labelAdresseEltern.Size = new System.Drawing.Size(74, 13);
            this.labelAdresseEltern.TabIndex = 27;
            this.labelAdresseEltern.Text = "Kontakt Eltern";
            // 
            // textBoxAdresseEltern
            // 
            this.textBoxAdresseEltern.Location = new System.Drawing.Point(97, 199);
            this.textBoxAdresseEltern.Multiline = true;
            this.textBoxAdresseEltern.Name = "textBoxAdresseEltern";
            this.textBoxAdresseEltern.Size = new System.Drawing.Size(178, 60);
            this.textBoxAdresseEltern.TabIndex = 28;
            // 
            // labelProbezeitBis
            // 
            this.labelProbezeitBis.AutoSize = true;
            this.labelProbezeitBis.Location = new System.Drawing.Point(369, 263);
            this.labelProbezeitBis.Name = "labelProbezeitBis";
            this.labelProbezeitBis.Size = new System.Drawing.Size(67, 13);
            this.labelProbezeitBis.TabIndex = 29;
            this.labelProbezeitBis.Text = "Probezeit bis";
            // 
            // textBoxProbezeit
            // 
            this.textBoxProbezeit.Location = new System.Drawing.Point(485, 260);
            this.textBoxProbezeit.Name = "textBoxProbezeit";
            this.textBoxProbezeit.Size = new System.Drawing.Size(185, 20);
            this.textBoxProbezeit.TabIndex = 30;
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxImage.Image")));
            this.pictureBoxImage.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxImage.Name = "pictureBoxImage";
            this.pictureBoxImage.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxImage.TabIndex = 31;
            this.pictureBoxImage.TabStop = false;
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(485, 18);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(40, 20);
            this.textBoxID.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(369, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Schüler-ID";
            // 
            // UserControlSchueleransicht
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxImage);
            this.Controls.Add(this.textBoxProbezeit);
            this.Controls.Add(this.labelProbezeitBis);
            this.Controls.Add(this.textBoxAdresseEltern);
            this.Controls.Add(this.labelAdresseEltern);
            this.Controls.Add(this.textBoxVorigeSchule);
            this.Controls.Add(this.labelVorigeSchule);
            this.Controls.Add(this.textBoxEintrittAm);
            this.Controls.Add(this.labelAm);
            this.Controls.Add(this.textBoxJahrgangsstufe);
            this.Controls.Add(this.labelEintrittInJahrgangsstufe);
            this.Controls.Add(this.textBoxWiederholungen);
            this.Controls.Add(this.labelWiederholungen);
            this.Controls.Add(this.textBoxBeruflicheVorbildung);
            this.Controls.Add(this.labelBeruflicheVorbildung);
            this.Controls.Add(this.textBoxSchulischeVorbildung);
            this.Controls.Add(this.lblSchulischeVorbildung);
            this.Controls.Add(this.textBoxGeburtsort);
            this.Controls.Add(this.lblGeburtsort);
            this.Controls.Add(this.textBoxGeburtsdatum);
            this.Controls.Add(this.lblGeburtsdatum);
            this.Controls.Add(this.textBoxTelefonnummer);
            this.Controls.Add(this.lblTelefonnummer);
            this.Controls.Add(this.textBoxAdresse);
            this.Controls.Add(this.lblAdresse);
            this.Controls.Add(this.nameLabel);
            this.Name = "UserControlSchueleransicht";
            this.Size = new System.Drawing.Size(694, 302);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.Label lblAdresse;
    private System.Windows.Forms.TextBox textBoxAdresse;
    private System.Windows.Forms.Label lblTelefonnummer;
    private System.Windows.Forms.TextBox textBoxTelefonnummer;
    private System.Windows.Forms.Label lblGeburtsdatum;
    private System.Windows.Forms.TextBox textBoxGeburtsdatum;
    private System.Windows.Forms.Label lblGeburtsort;
    private System.Windows.Forms.TextBox textBoxGeburtsort;
    private System.Windows.Forms.Label lblSchulischeVorbildung;
    private System.Windows.Forms.TextBox textBoxSchulischeVorbildung;
    private System.Windows.Forms.Label labelBeruflicheVorbildung;
    private System.Windows.Forms.TextBox textBoxBeruflicheVorbildung;
    private System.Windows.Forms.Label labelWiederholungen;
    private System.Windows.Forms.TextBox textBoxWiederholungen;
    private System.Windows.Forms.Label labelEintrittInJahrgangsstufe;
    private System.Windows.Forms.TextBox textBoxJahrgangsstufe;
    private System.Windows.Forms.Label labelAm;
    private System.Windows.Forms.TextBox textBoxEintrittAm;
    private System.Windows.Forms.Label labelVorigeSchule;
    private System.Windows.Forms.TextBox textBoxVorigeSchule;
    private System.Windows.Forms.Label labelAdresseEltern;
    private System.Windows.Forms.TextBox textBoxAdresseEltern;
    private System.Windows.Forms.Label labelProbezeitBis;
    private System.Windows.Forms.TextBox textBoxProbezeit;
    private System.Windows.Forms.PictureBox pictureBoxImage;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label1;
    }
}
