﻿namespace diNo
{
    partial class Brief
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
      this.btnOK = new System.Windows.Forms.Button();
      this.btnEsc = new System.Windows.Forms.Button();
      this.boxAuswahl = new System.Windows.Forms.GroupBox();
      this.opMitteilung = new System.Windows.Forms.RadioButton();
      this.opAttestpflicht = new System.Windows.Forms.RadioButton();
      this.opVerschVerweis = new System.Windows.Forms.RadioButton();
      this.opNacharbeit = new System.Windows.Forms.RadioButton();
      this.opVerweis = new System.Windows.Forms.RadioButton();
      this.opMEP = new System.Windows.Forms.RadioButton();
      this.opSEP = new System.Windows.Forms.RadioButton();
      this.opKA = new System.Windows.Forms.RadioButton();
      this.opSA = new System.Windows.Forms.RadioButton();
      this.pnlNachterminAm = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.edRaum = new System.Windows.Forms.TextBox();
      this.cbFach = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.datZeit = new System.Windows.Forms.DateTimePicker();
      this.datTermin = new System.Windows.Forms.DateTimePicker();
      this.pnlVersaeumtAm = new System.Windows.Forms.Panel();
      this.label6 = new System.Windows.Forms.Label();
      this.datVersaeumtAm = new System.Windows.Forms.DateTimePicker();
      this.pnlInhalt = new System.Windows.Forms.Panel();
      this.labelInhalt = new System.Windows.Forms.Label();
      this.edInhalt = new System.Windows.Forms.TextBox();
      this.boxAuswahl.SuspendLayout();
      this.pnlNachterminAm.SuspendLayout();
      this.pnlVersaeumtAm.SuspendLayout();
      this.pnlInhalt.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(416, 437);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(123, 38);
      this.btnOK.TabIndex = 4;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnEsc
      // 
      this.btnEsc.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnEsc.Location = new System.Drawing.Point(548, 437);
      this.btnEsc.Name = "btnEsc";
      this.btnEsc.Size = new System.Drawing.Size(123, 38);
      this.btnEsc.TabIndex = 5;
      this.btnEsc.Text = "Abbrechen";
      this.btnEsc.UseVisualStyleBackColor = true;
      this.btnEsc.Click += new System.EventHandler(this.btnEsc_Click);
      // 
      // boxAuswahl
      // 
      this.boxAuswahl.Controls.Add(this.opMitteilung);
      this.boxAuswahl.Controls.Add(this.opAttestpflicht);
      this.boxAuswahl.Controls.Add(this.opVerschVerweis);
      this.boxAuswahl.Controls.Add(this.opNacharbeit);
      this.boxAuswahl.Controls.Add(this.opVerweis);
      this.boxAuswahl.Controls.Add(this.opMEP);
      this.boxAuswahl.Controls.Add(this.opSEP);
      this.boxAuswahl.Controls.Add(this.opKA);
      this.boxAuswahl.Controls.Add(this.opSA);
      this.boxAuswahl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.boxAuswahl.Location = new System.Drawing.Point(39, 38);
      this.boxAuswahl.Name = "boxAuswahl";
      this.boxAuswahl.Size = new System.Drawing.Size(193, 242);
      this.boxAuswahl.TabIndex = 0;
      this.boxAuswahl.TabStop = false;
      // 
      // opMitteilung
      // 
      this.opMitteilung.AutoSize = true;
      this.opMitteilung.Location = new System.Drawing.Point(16, 198);
      this.opMitteilung.Name = "opMitteilung";
      this.opMitteilung.Size = new System.Drawing.Size(147, 21);
      this.opMitteilung.TabIndex = 10;
      this.opMitteilung.Text = "Mitteilung an Eltern";
      this.opMitteilung.UseVisualStyleBackColor = true;
      this.opMitteilung.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opAttestpflicht
      // 
      this.opAttestpflicht.AutoSize = true;
      this.opAttestpflicht.Location = new System.Drawing.Point(16, 176);
      this.opAttestpflicht.Name = "opAttestpflicht";
      this.opAttestpflicht.Size = new System.Drawing.Size(99, 21);
      this.opAttestpflicht.TabIndex = 9;
      this.opAttestpflicht.Text = "Attestpflicht";
      this.opAttestpflicht.UseVisualStyleBackColor = true;
      this.opAttestpflicht.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opVerschVerweis
      // 
      this.opVerschVerweis.AutoSize = true;
      this.opVerschVerweis.Location = new System.Drawing.Point(16, 132);
      this.opVerschVerweis.Name = "opVerschVerweis";
      this.opVerschVerweis.Size = new System.Drawing.Size(155, 21);
      this.opVerschVerweis.TabIndex = 6;
      this.opVerschVerweis.Text = "verschärfter Verweis";
      this.opVerschVerweis.UseVisualStyleBackColor = true;
      this.opVerschVerweis.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opNacharbeit
      // 
      this.opNacharbeit.AutoSize = true;
      this.opNacharbeit.Location = new System.Drawing.Point(16, 154);
      this.opNacharbeit.Name = "opNacharbeit";
      this.opNacharbeit.Size = new System.Drawing.Size(95, 21);
      this.opNacharbeit.TabIndex = 7;
      this.opNacharbeit.Text = "Nacharbeit";
      this.opNacharbeit.UseVisualStyleBackColor = true;
      this.opNacharbeit.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opVerweis
      // 
      this.opVerweis.AutoSize = true;
      this.opVerweis.Location = new System.Drawing.Point(16, 110);
      this.opVerweis.Name = "opVerweis";
      this.opVerweis.Size = new System.Drawing.Size(75, 21);
      this.opVerweis.TabIndex = 5;
      this.opVerweis.Text = "Verweis";
      this.opVerweis.UseVisualStyleBackColor = true;
      this.opVerweis.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opMEP
      // 
      this.opMEP.AutoSize = true;
      this.opMEP.Location = new System.Drawing.Point(16, 88);
      this.opMEP.Name = "opMEP";
      this.opMEP.Size = new System.Drawing.Size(145, 21);
      this.opMEP.TabIndex = 4;
      this.opMEP.Text = "mdl. Ersatzprüfung";
      this.opMEP.UseVisualStyleBackColor = true;
      this.opMEP.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opSEP
      // 
      this.opSEP.AutoSize = true;
      this.opSEP.Location = new System.Drawing.Point(16, 66);
      this.opSEP.Name = "opSEP";
      this.opSEP.Size = new System.Drawing.Size(164, 21);
      this.opSEP.TabIndex = 3;
      this.opSEP.Text = "schriftl. Ersatzprüfung";
      this.opSEP.UseVisualStyleBackColor = true;
      this.opSEP.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opKA
      // 
      this.opKA.AutoSize = true;
      this.opKA.Location = new System.Drawing.Point(16, 44);
      this.opKA.Name = "opKA";
      this.opKA.Size = new System.Drawing.Size(120, 21);
      this.opKA.TabIndex = 2;
      this.opKA.Text = "Nachtermin KA";
      this.opKA.UseVisualStyleBackColor = true;
      this.opKA.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // opSA
      // 
      this.opSA.AutoSize = true;
      this.opSA.Checked = true;
      this.opSA.Location = new System.Drawing.Point(16, 22);
      this.opSA.Name = "opSA";
      this.opSA.Size = new System.Drawing.Size(120, 21);
      this.opSA.TabIndex = 1;
      this.opSA.TabStop = true;
      this.opSA.Text = "Nachtermin SA";
      this.opSA.UseVisualStyleBackColor = true;
      this.opSA.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // pnlNachterminAm
      // 
      this.pnlNachterminAm.Controls.Add(this.label1);
      this.pnlNachterminAm.Controls.Add(this.edRaum);
      this.pnlNachterminAm.Controls.Add(this.cbFach);
      this.pnlNachterminAm.Controls.Add(this.label2);
      this.pnlNachterminAm.Controls.Add(this.label4);
      this.pnlNachterminAm.Controls.Add(this.label3);
      this.pnlNachterminAm.Controls.Add(this.datZeit);
      this.pnlNachterminAm.Controls.Add(this.datTermin);
      this.pnlNachterminAm.Location = new System.Drawing.Point(256, 105);
      this.pnlNachterminAm.Name = "pnlNachterminAm";
      this.pnlNachterminAm.Size = new System.Drawing.Size(429, 118);
      this.pnlNachterminAm.TabIndex = 2;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(317, 64);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(109, 17);
      this.label1.TabIndex = 18;
      this.label1.Text = "Raum (optional)";
      // 
      // edRaum
      // 
      this.edRaum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edRaum.Location = new System.Drawing.Point(320, 86);
      this.edRaum.Name = "edRaum";
      this.edRaum.Size = new System.Drawing.Size(86, 23);
      this.edRaum.TabIndex = 17;
      // 
      // cbFach
      // 
      this.cbFach.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbFach.FormattingEnabled = true;
      this.cbFach.Location = new System.Drawing.Point(3, 85);
      this.cbFach.Name = "cbFach";
      this.cbFach.Size = new System.Drawing.Size(298, 24);
      this.cbFach.TabIndex = 16;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 64);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 17);
      this.label2.TabIndex = 15;
      this.label2.Text = "im Fach";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(317, 10);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(27, 17);
      this.label4.TabIndex = 14;
      this.label4.Text = "um";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(2, 11);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(281, 17);
      this.label3.TabIndex = 13;
      this.label3.Text = "Der Nachtermin / Nacharbeit findet statt am";
      // 
      // datZeit
      // 
      this.datZeit.CustomFormat = "H\':\'mm";
      this.datZeit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.datZeit.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.datZeit.Location = new System.Drawing.Point(320, 31);
      this.datZeit.Name = "datZeit";
      this.datZeit.ShowUpDown = true;
      this.datZeit.Size = new System.Drawing.Size(86, 23);
      this.datZeit.TabIndex = 12;
      this.datZeit.Value = new System.DateTime(2015, 11, 28, 13, 30, 0, 0);
      // 
      // datTermin
      // 
      this.datTermin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.datTermin.Location = new System.Drawing.Point(3, 31);
      this.datTermin.Name = "datTermin";
      this.datTermin.Size = new System.Drawing.Size(298, 23);
      this.datTermin.TabIndex = 11;
      this.datTermin.Value = new System.DateTime(2015, 11, 28, 10, 46, 16, 0);
      // 
      // pnlVersaeumtAm
      // 
      this.pnlVersaeumtAm.Controls.Add(this.label6);
      this.pnlVersaeumtAm.Controls.Add(this.datVersaeumtAm);
      this.pnlVersaeumtAm.Location = new System.Drawing.Point(256, 45);
      this.pnlVersaeumtAm.Name = "pnlVersaeumtAm";
      this.pnlVersaeumtAm.Size = new System.Drawing.Size(429, 54);
      this.pnlVersaeumtAm.TabIndex = 1;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(0, 4);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(310, 17);
      this.label6.TabIndex = 0;
      this.label6.Text = "Es wurde der Leistungsnachweis versäumt vom ";
      // 
      // datVersaeumtAm
      // 
      this.datVersaeumtAm.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.datVersaeumtAm.Location = new System.Drawing.Point(3, 24);
      this.datVersaeumtAm.Name = "datVersaeumtAm";
      this.datVersaeumtAm.Size = new System.Drawing.Size(298, 23);
      this.datVersaeumtAm.TabIndex = 11;
      this.datVersaeumtAm.Value = new System.DateTime(2015, 12, 19, 0, 0, 0, 0);
      // 
      // pnlInhalt
      // 
      this.pnlInhalt.Controls.Add(this.labelInhalt);
      this.pnlInhalt.Controls.Add(this.edInhalt);
      this.pnlInhalt.Location = new System.Drawing.Point(22, 286);
      this.pnlInhalt.Name = "pnlInhalt";
      this.pnlInhalt.Size = new System.Drawing.Size(663, 135);
      this.pnlInhalt.TabIndex = 3;
      // 
      // labelInhalt
      // 
      this.labelInhalt.AutoSize = true;
      this.labelInhalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelInhalt.Location = new System.Drawing.Point(14, 8);
      this.labelInhalt.Name = "labelInhalt";
      this.labelInhalt.Size = new System.Drawing.Size(102, 17);
      this.labelInhalt.TabIndex = 6;
      this.labelInhalt.Text = "Grund / Betreff";
      // 
      // edInhalt
      // 
      this.edInhalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edInhalt.Location = new System.Drawing.Point(17, 28);
      this.edInhalt.Multiline = true;
      this.edInhalt.Name = "edInhalt";
      this.edInhalt.Size = new System.Drawing.Size(632, 104);
      this.edInhalt.TabIndex = 5;
      // 
      // Brief
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnEsc;
      this.ClientSize = new System.Drawing.Size(697, 508);
      this.Controls.Add(this.pnlInhalt);
      this.Controls.Add(this.pnlVersaeumtAm);
      this.Controls.Add(this.pnlNachterminAm);
      this.Controls.Add(this.boxAuswahl);
      this.Controls.Add(this.btnEsc);
      this.Controls.Add(this.btnOK);
      this.Name = "Brief";
      this.Text = "Verweis";
      this.boxAuswahl.ResumeLayout(false);
      this.boxAuswahl.PerformLayout();
      this.pnlNachterminAm.ResumeLayout(false);
      this.pnlNachterminAm.PerformLayout();
      this.pnlVersaeumtAm.ResumeLayout(false);
      this.pnlVersaeumtAm.PerformLayout();
      this.pnlInhalt.ResumeLayout(false);
      this.pnlInhalt.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnEsc;
        private System.Windows.Forms.GroupBox boxAuswahl;
        private System.Windows.Forms.RadioButton opSEP;
        private System.Windows.Forms.RadioButton opKA;
        private System.Windows.Forms.RadioButton opSA;
    private System.Windows.Forms.RadioButton opMEP;
    private System.Windows.Forms.Panel pnlNachterminAm;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.DateTimePicker datZeit;
    private System.Windows.Forms.DateTimePicker datTermin;
    private System.Windows.Forms.Panel pnlVersaeumtAm;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.DateTimePicker datVersaeumtAm;
    private System.Windows.Forms.ComboBox cbFach;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RadioButton opNacharbeit;
    private System.Windows.Forms.RadioButton opVerweis;
    private System.Windows.Forms.Panel pnlInhalt;
    private System.Windows.Forms.Label labelInhalt;
    private System.Windows.Forms.TextBox edInhalt;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edRaum;
    private System.Windows.Forms.RadioButton opVerschVerweis;
    private System.Windows.Forms.RadioButton opAttestpflicht;
        private System.Windows.Forms.RadioButton opMitteilung;
    }
}