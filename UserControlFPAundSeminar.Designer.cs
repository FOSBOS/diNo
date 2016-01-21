namespace diNo
{
    partial class UserControlFPAundSeminar
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
      this.buttonSpeichern = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.pnlFPA = new System.Windows.Forms.Panel();
      this.numPunkte1Hj = new diNo.NumericUpDownNullable();
      this.numPunkte2Hj = new diNo.NumericUpDownNullable();
      this.numPunkte = new diNo.NumericUpDownNullable();
      this.label9 = new System.Windows.Forms.Label();
      this.cbFPAErfolg = new System.Windows.Forms.ComboBox();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.lbHj2 = new System.Windows.Forms.Label();
      this.lbHj1 = new System.Windows.Forms.Label();
      this.cbFPAErfolg1Hj = new System.Windows.Forms.ComboBox();
      this.textBoxFpABemerkung = new System.Windows.Forms.TextBox();
      this.pnlSeminar = new System.Windows.Forms.Panel();
      this.numSeminarpunkte = new diNo.NumericUpDownNullable();
      this.label11 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.textBoxSeminarfachthemaKurz = new System.Windows.Forms.TextBox();
      this.textBoxSeminarfachthemaLang = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.pnlBetreuer = new System.Windows.Forms.Panel();
      this.label12 = new System.Windows.Forms.Label();
      this.cbBetreuer = new System.Windows.Forms.ComboBox();
      this.pnlFPA.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numPunkte1Hj)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numPunkte2Hj)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numPunkte)).BeginInit();
      this.pnlSeminar.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numSeminarpunkte)).BeginInit();
      this.pnlBetreuer.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonSpeichern
      // 
      this.buttonSpeichern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonSpeichern.Location = new System.Drawing.Point(-135, 475);
      this.buttonSpeichern.Name = "buttonSpeichern";
      this.buttonSpeichern.Size = new System.Drawing.Size(75, 23);
      this.buttonSpeichern.TabIndex = 28;
      this.buttonSpeichern.Text = "speichern";
      this.buttonSpeichern.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(-138, 435);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(92, 13);
      this.label3.TabIndex = 22;
      this.label3.Text = "Seminarfach Note";
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(-138, 376);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(132, 13);
      this.label1.TabIndex = 19;
      this.label1.Text = "fachpraktische Ausbildung";
      // 
      // pnlFPA
      // 
      this.pnlFPA.Controls.Add(this.numPunkte1Hj);
      this.pnlFPA.Controls.Add(this.numPunkte2Hj);
      this.pnlFPA.Controls.Add(this.numPunkte);
      this.pnlFPA.Controls.Add(this.label9);
      this.pnlFPA.Controls.Add(this.cbFPAErfolg);
      this.pnlFPA.Controls.Add(this.label8);
      this.pnlFPA.Controls.Add(this.label7);
      this.pnlFPA.Controls.Add(this.label6);
      this.pnlFPA.Controls.Add(this.label2);
      this.pnlFPA.Controls.Add(this.lbHj2);
      this.pnlFPA.Controls.Add(this.lbHj1);
      this.pnlFPA.Controls.Add(this.cbFPAErfolg1Hj);
      this.pnlFPA.Controls.Add(this.textBoxFpABemerkung);
      this.pnlFPA.Location = new System.Drawing.Point(15, 96);
      this.pnlFPA.Name = "pnlFPA";
      this.pnlFPA.Size = new System.Drawing.Size(522, 289);
      this.pnlFPA.TabIndex = 0;
      // 
      // numPunkte1Hj
      // 
      this.numPunkte1Hj.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numPunkte1Hj.Location = new System.Drawing.Point(135, 79);
      this.numPunkte1Hj.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
      this.numPunkte1Hj.Name = "numPunkte1Hj";
      this.numPunkte1Hj.Size = new System.Drawing.Size(88, 23);
      this.numPunkte1Hj.TabIndex = 1;
      this.numPunkte1Hj.Value = null;
      // 
      // numPunkte2Hj
      // 
      this.numPunkte2Hj.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numPunkte2Hj.Location = new System.Drawing.Point(135, 109);
      this.numPunkte2Hj.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
      this.numPunkte2Hj.Name = "numPunkte2Hj";
      this.numPunkte2Hj.Size = new System.Drawing.Size(88, 23);
      this.numPunkte2Hj.TabIndex = 3;
      this.numPunkte2Hj.Value = null;
      // 
      // numPunkte
      // 
      this.numPunkte.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numPunkte.Location = new System.Drawing.Point(135, 149);
      this.numPunkte.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
      this.numPunkte.Name = "numPunkte";
      this.numPunkte.Size = new System.Drawing.Size(88, 23);
      this.numPunkte.TabIndex = 4;
      this.numPunkte.Value = null;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.Location = new System.Drawing.Point(14, 9);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(239, 24);
      this.label9.TabIndex = 62;
      this.label9.Text = "Fachpraktische Ausbildung";
      // 
      // cbFPAErfolg
      // 
      this.cbFPAErfolg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbFPAErfolg.FormattingEnabled = true;
      this.cbFPAErfolg.Items.AddRange(new object[] {
            "",
            "mit sehr gutem Erfolg",
            "mit gutem Erfolg",
            "mit Erfolg",
            "ohne Erfolg"});
      this.cbFPAErfolg.Location = new System.Drawing.Point(272, 148);
      this.cbFPAErfolg.Name = "cbFPAErfolg";
      this.cbFPAErfolg.Size = new System.Drawing.Size(239, 24);
      this.cbFPAErfolg.TabIndex = 5;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(12, 188);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(80, 17);
      this.label8.TabIndex = 56;
      this.label8.Text = "Bemerkung";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(12, 151);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(87, 17);
      this.label7.TabIndex = 54;
      this.label7.Text = "Durchschnitt";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(269, 47);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(75, 17);
      this.label6.TabIndex = 53;
      this.label6.Text = "Bewertung";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(132, 47);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(52, 17);
      this.label2.TabIndex = 0;
      this.label2.Text = "Punkte";
      // 
      // lbHj2
      // 
      this.lbHj2.AutoSize = true;
      this.lbHj2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbHj2.Location = new System.Drawing.Point(12, 111);
      this.lbHj2.Name = "lbHj2";
      this.lbHj2.Size = new System.Drawing.Size(77, 17);
      this.lbHj2.TabIndex = 51;
      this.lbHj2.Text = "2. Halbjahr";
      // 
      // lbHj1
      // 
      this.lbHj1.AutoSize = true;
      this.lbHj1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbHj1.Location = new System.Drawing.Point(12, 81);
      this.lbHj1.Name = "lbHj1";
      this.lbHj1.Size = new System.Drawing.Size(77, 17);
      this.lbHj1.TabIndex = 50;
      this.lbHj1.Text = "1. Halbjahr";
      // 
      // cbFPAErfolg1Hj
      // 
      this.cbFPAErfolg1Hj.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbFPAErfolg1Hj.FormattingEnabled = true;
      this.cbFPAErfolg1Hj.Items.AddRange(new object[] {
            "",
            "mit sehr gutem Erfolg",
            "mit gutem Erfolg",
            "mit Erfolg",
            "ohne Erfolg"});
      this.cbFPAErfolg1Hj.Location = new System.Drawing.Point(272, 78);
      this.cbFPAErfolg1Hj.Name = "cbFPAErfolg1Hj";
      this.cbFPAErfolg1Hj.Size = new System.Drawing.Size(239, 24);
      this.cbFPAErfolg1Hj.TabIndex = 2;
      // 
      // textBoxFpABemerkung
      // 
      this.textBoxFpABemerkung.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxFpABemerkung.Location = new System.Drawing.Point(15, 208);
      this.textBoxFpABemerkung.Multiline = true;
      this.textBoxFpABemerkung.Name = "textBoxFpABemerkung";
      this.textBoxFpABemerkung.Size = new System.Drawing.Size(496, 59);
      this.textBoxFpABemerkung.TabIndex = 6;
      // 
      // pnlSeminar
      // 
      this.pnlSeminar.Controls.Add(this.numSeminarpunkte);
      this.pnlSeminar.Controls.Add(this.label11);
      this.pnlSeminar.Controls.Add(this.label10);
      this.pnlSeminar.Controls.Add(this.textBoxSeminarfachthemaKurz);
      this.pnlSeminar.Controls.Add(this.textBoxSeminarfachthemaLang);
      this.pnlSeminar.Controls.Add(this.label5);
      this.pnlSeminar.Controls.Add(this.label4);
      this.pnlSeminar.Location = new System.Drawing.Point(15, 391);
      this.pnlSeminar.Name = "pnlSeminar";
      this.pnlSeminar.Size = new System.Drawing.Size(522, 253);
      this.pnlSeminar.TabIndex = 1;
      // 
      // numSeminarpunkte
      // 
      this.numSeminarpunkte.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numSeminarpunkte.Location = new System.Drawing.Point(135, 61);
      this.numSeminarpunkte.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
      this.numSeminarpunkte.Name = "numSeminarpunkte";
      this.numSeminarpunkte.Size = new System.Drawing.Size(88, 23);
      this.numSeminarpunkte.TabIndex = 0;
      this.numSeminarpunkte.Value = null;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(15, 63);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(89, 17);
      this.label11.TabIndex = 65;
      this.label11.Text = "Notenpunkte";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(14, 24);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(126, 24);
      this.label10.TabIndex = 64;
      this.label10.Text = "Seminararbeit";
      // 
      // textBoxSeminarfachthemaKurz
      // 
      this.textBoxSeminarfachthemaKurz.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxSeminarfachthemaKurz.Location = new System.Drawing.Point(18, 123);
      this.textBoxSeminarfachthemaKurz.MaxLength = 117;
      this.textBoxSeminarfachthemaKurz.Name = "textBoxSeminarfachthemaKurz";
      this.textBoxSeminarfachthemaKurz.Size = new System.Drawing.Size(496, 23);
      this.textBoxSeminarfachthemaKurz.TabIndex = 1;
      // 
      // textBoxSeminarfachthemaLang
      // 
      this.textBoxSeminarfachthemaLang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxSeminarfachthemaLang.Location = new System.Drawing.Point(18, 177);
      this.textBoxSeminarfachthemaLang.MaxLength = 1024;
      this.textBoxSeminarfachthemaLang.Multiline = true;
      this.textBoxSeminarfachthemaLang.Name = "textBoxSeminarfachthemaLang";
      this.textBoxSeminarfachthemaLang.Size = new System.Drawing.Size(496, 63);
      this.textBoxSeminarfachthemaLang.TabIndex = 2;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(15, 103);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(167, 17);
      this.label5.TabIndex = 61;
      this.label5.Text = "Thema (bis 117 Zeichen)";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(15, 157);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(93, 17);
      this.label4.TabIndex = 60;
      this.label4.Text = "Thema (lang)";
      // 
      // pnlBetreuer
      // 
      this.pnlBetreuer.Controls.Add(this.label12);
      this.pnlBetreuer.Controls.Add(this.cbBetreuer);
      this.pnlBetreuer.Location = new System.Drawing.Point(17, 14);
      this.pnlBetreuer.Name = "pnlBetreuer";
      this.pnlBetreuer.Size = new System.Drawing.Size(519, 66);
      this.pnlBetreuer.TabIndex = 29;
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label12.Location = new System.Drawing.Point(10, 11);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(143, 17);
      this.label12.TabIndex = 55;
      this.label12.Text = "Betreuende Lehrkraft";
      // 
      // cbBetreuer
      // 
      this.cbBetreuer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbBetreuer.FormattingEnabled = true;
      this.cbBetreuer.Items.AddRange(new object[] {
            "",
            "mit sehr gutem Erfolg",
            "mit gutem Erfolg",
            "mit Erfolg",
            "ohne Erfolg"});
      this.cbBetreuer.Location = new System.Drawing.Point(13, 31);
      this.cbBetreuer.Name = "cbBetreuer";
      this.cbBetreuer.Size = new System.Drawing.Size(496, 24);
      this.cbBetreuer.TabIndex = 54;
      // 
      // UserControlFPAundSeminar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pnlBetreuer);
      this.Controls.Add(this.pnlSeminar);
      this.Controls.Add(this.pnlFPA);
      this.Controls.Add(this.buttonSpeichern);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Name = "UserControlFPAundSeminar";
      this.Size = new System.Drawing.Size(565, 647);
      this.pnlFPA.ResumeLayout(false);
      this.pnlFPA.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numPunkte1Hj)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numPunkte2Hj)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numPunkte)).EndInit();
      this.pnlSeminar.ResumeLayout(false);
      this.pnlSeminar.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numSeminarpunkte)).EndInit();
      this.pnlBetreuer.ResumeLayout(false);
      this.pnlBetreuer.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonSpeichern;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlFPA;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbFPAErfolg;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbHj2;
        private System.Windows.Forms.Label lbHj1;
        private System.Windows.Forms.ComboBox cbFPAErfolg1Hj;
        private System.Windows.Forms.TextBox textBoxFpABemerkung;
        private System.Windows.Forms.Panel pnlSeminar;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxSeminarfachthemaKurz;
        private System.Windows.Forms.TextBox textBoxSeminarfachthemaLang;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private NumericUpDownNullable numPunkte;
        private NumericUpDownNullable numPunkte1Hj;
        private NumericUpDownNullable numPunkte2Hj;
        private NumericUpDownNullable numSeminarpunkte;
    private System.Windows.Forms.Panel pnlBetreuer;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.ComboBox cbBetreuer;
  }
}
