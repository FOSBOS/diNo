namespace diNo
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
      this.radioButton4 = new System.Windows.Forms.RadioButton();
      this.radioButton3 = new System.Windows.Forms.RadioButton();
      this.radioButton2 = new System.Windows.Forms.RadioButton();
      this.radioButton1 = new System.Windows.Forms.RadioButton();
      this.opMitteilung = new System.Windows.Forms.RadioButton();
      this.opVerweis = new System.Windows.Forms.RadioButton();
      this.edBetreff = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.edInhalt = new System.Windows.Forms.TextBox();
      this.datTermin = new System.Windows.Forms.DateTimePicker();
      this.datZeit = new System.Windows.Forms.DateTimePicker();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.boxAuswahl.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOK.Location = new System.Drawing.Point(383, 386);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(123, 38);
      this.btnOK.TabIndex = 5;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnEsc
      // 
      this.btnEsc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnEsc.Location = new System.Drawing.Point(521, 386);
      this.btnEsc.Name = "btnEsc";
      this.btnEsc.Size = new System.Drawing.Size(123, 38);
      this.btnEsc.TabIndex = 6;
      this.btnEsc.Text = "Abbrechen";
      this.btnEsc.UseVisualStyleBackColor = true;
      this.btnEsc.Click += new System.EventHandler(this.btnEsc_Click);
      // 
      // boxAuswahl
      // 
      this.boxAuswahl.Controls.Add(this.radioButton4);
      this.boxAuswahl.Controls.Add(this.radioButton3);
      this.boxAuswahl.Controls.Add(this.radioButton2);
      this.boxAuswahl.Controls.Add(this.radioButton1);
      this.boxAuswahl.Controls.Add(this.opMitteilung);
      this.boxAuswahl.Controls.Add(this.opVerweis);
      this.boxAuswahl.Enabled = false;
      this.boxAuswahl.Location = new System.Drawing.Point(39, 36);
      this.boxAuswahl.Name = "boxAuswahl";
      this.boxAuswahl.Size = new System.Drawing.Size(149, 203);
      this.boxAuswahl.TabIndex = 0;
      this.boxAuswahl.TabStop = false;
      // 
      // radioButton4
      // 
      this.radioButton4.AutoSize = true;
      this.radioButton4.Enabled = false;
      this.radioButton4.Location = new System.Drawing.Point(17, 133);
      this.radioButton4.Name = "radioButton4";
      this.radioButton4.Size = new System.Drawing.Size(90, 17);
      this.radioButton4.TabIndex = 6;
      this.radioButton4.Text = "Ersatzprüfung";
      this.radioButton4.UseVisualStyleBackColor = true;
      // 
      // radioButton3
      // 
      this.radioButton3.AutoSize = true;
      this.radioButton3.Enabled = false;
      this.radioButton3.Location = new System.Drawing.Point(17, 111);
      this.radioButton3.Name = "radioButton3";
      this.radioButton3.Size = new System.Drawing.Size(96, 17);
      this.radioButton3.TabIndex = 5;
      this.radioButton3.Text = "Nachtermin KA";
      this.radioButton3.UseVisualStyleBackColor = true;
      // 
      // radioButton2
      // 
      this.radioButton2.AutoSize = true;
      this.radioButton2.Enabled = false;
      this.radioButton2.Location = new System.Drawing.Point(17, 88);
      this.radioButton2.Name = "radioButton2";
      this.radioButton2.Size = new System.Drawing.Size(96, 17);
      this.radioButton2.TabIndex = 4;
      this.radioButton2.Text = "Nachtermin SA";
      this.radioButton2.UseVisualStyleBackColor = true;
      // 
      // radioButton1
      // 
      this.radioButton1.AutoSize = true;
      this.radioButton1.Enabled = false;
      this.radioButton1.Location = new System.Drawing.Point(17, 65);
      this.radioButton1.Name = "radioButton1";
      this.radioButton1.Size = new System.Drawing.Size(77, 17);
      this.radioButton1.TabIndex = 3;
      this.radioButton1.Text = "Nacharbeit";
      this.radioButton1.UseVisualStyleBackColor = true;
      // 
      // opMitteilung
      // 
      this.opMitteilung.AutoSize = true;
      this.opMitteilung.Enabled = false;
      this.opMitteilung.Location = new System.Drawing.Point(17, 42);
      this.opMitteilung.Name = "opMitteilung";
      this.opMitteilung.Size = new System.Drawing.Size(70, 17);
      this.opMitteilung.TabIndex = 2;
      this.opMitteilung.Text = "Mitteilung";
      this.opMitteilung.UseVisualStyleBackColor = true;
      // 
      // opVerweis
      // 
      this.opVerweis.AutoSize = true;
      this.opVerweis.Checked = true;
      this.opVerweis.Location = new System.Drawing.Point(17, 19);
      this.opVerweis.Name = "opVerweis";
      this.opVerweis.Size = new System.Drawing.Size(62, 17);
      this.opVerweis.TabIndex = 1;
      this.opVerweis.TabStop = true;
      this.opVerweis.Text = "Verweis";
      this.opVerweis.UseVisualStyleBackColor = true;
      // 
      // edBetreff
      // 
      this.edBetreff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edBetreff.Location = new System.Drawing.Point(241, 52);
      this.edBetreff.Multiline = true;
      this.edBetreff.Name = "edBetreff";
      this.edBetreff.Size = new System.Drawing.Size(403, 40);
      this.edBetreff.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(238, 36);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(78, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Grund / Betreff";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(36, 254);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "freier Mitteilungstext";
      // 
      // edInhalt
      // 
      this.edInhalt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edInhalt.Enabled = false;
      this.edInhalt.Location = new System.Drawing.Point(39, 270);
      this.edInhalt.Multiline = true;
      this.edInhalt.Name = "edInhalt";
      this.edInhalt.Size = new System.Drawing.Size(605, 95);
      this.edInhalt.TabIndex = 4;
      // 
      // datTermin
      // 
      this.datTermin.Enabled = false;
      this.datTermin.Location = new System.Drawing.Point(241, 120);
      this.datTermin.Name = "datTermin";
      this.datTermin.Size = new System.Drawing.Size(238, 20);
      this.datTermin.TabIndex = 2;
      this.datTermin.Value = new System.DateTime(2015, 11, 28, 10, 46, 16, 0);
      // 
      // datZeit
      // 
      this.datZeit.CustomFormat = "H\':\'m";
      this.datZeit.Enabled = false;
      this.datZeit.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.datZeit.Location = new System.Drawing.Point(508, 120);
      this.datZeit.Name = "datZeit";
      this.datZeit.ShowUpDown = true;
      this.datZeit.Size = new System.Drawing.Size(136, 20);
      this.datZeit.TabIndex = 3;
      this.datZeit.Value = new System.DateTime(2015, 11, 28, 13, 30, 0, 0);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(238, 105);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(21, 13);
      this.label3.TabIndex = 9;
      this.label3.Text = "am";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(505, 105);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(21, 13);
      this.label4.TabIndex = 10;
      this.label4.Text = "um";
      // 
      // Brief
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(685, 431);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.datZeit);
      this.Controls.Add(this.datTermin);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.edInhalt);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.edBetreff);
      this.Controls.Add(this.boxAuswahl);
      this.Controls.Add(this.btnEsc);
      this.Controls.Add(this.btnOK);
      this.Name = "Brief";
      this.Text = "Verweis";
      this.boxAuswahl.ResumeLayout(false);
      this.boxAuswahl.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnEsc;
        private System.Windows.Forms.GroupBox boxAuswahl;
        private System.Windows.Forms.RadioButton opMitteilung;
        private System.Windows.Forms.RadioButton opVerweis;
        private System.Windows.Forms.TextBox edBetreff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edInhalt;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.DateTimePicker datTermin;
        private System.Windows.Forms.DateTimePicker datZeit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}