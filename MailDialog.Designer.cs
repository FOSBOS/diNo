namespace diNo
{
  partial class MailDialog
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
      this.gbEmpfaenger = new System.Windows.Forms.GroupBox();
      this.opToEltern = new System.Windows.Forms.RadioButton();
      this.opToSchueler = new System.Windows.Forms.RadioButton();
      this.gbReply = new System.Windows.Forms.GroupBox();
      this.opReplyKL = new System.Windows.Forms.RadioButton();
      this.opSekretariat = new System.Windows.Forms.RadioButton();
      this.opReplyDino = new System.Windows.Forms.RadioButton();
      this.gbAnhang = new System.Windows.Forms.GroupBox();
      this.opAnhangAbsenzen = new System.Windows.Forms.RadioButton();
      this.opAnhangKeiner = new System.Windows.Forms.RadioButton();
      this.opAnhangPDF = new System.Windows.Forms.RadioButton();
      this.opAnhangNoten = new System.Windows.Forms.RadioButton();
      this.chkTest = new System.Windows.Forms.CheckBox();
      this.chkZip = new System.Windows.Forms.CheckBox();
      this.lbAnzahl = new System.Windows.Forms.Label();
      this.txtBody = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnSend = new System.Windows.Forms.Button();
      this.chkReadBodyText = new System.Windows.Forms.CheckBox();
      this.txtSubject = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.gbEmpfaenger.SuspendLayout();
      this.gbReply.SuspendLayout();
      this.gbAnhang.SuspendLayout();
      this.SuspendLayout();
      // 
      // gbEmpfaenger
      // 
      this.gbEmpfaenger.Controls.Add(this.opToEltern);
      this.gbEmpfaenger.Controls.Add(this.opToSchueler);
      this.gbEmpfaenger.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gbEmpfaenger.Location = new System.Drawing.Point(12, 97);
      this.gbEmpfaenger.Name = "gbEmpfaenger";
      this.gbEmpfaenger.Size = new System.Drawing.Size(300, 74);
      this.gbEmpfaenger.TabIndex = 89;
      this.gbEmpfaenger.TabStop = false;
      this.gbEmpfaenger.Text = "Mail geht an";
      // 
      // opToEltern
      // 
      this.opToEltern.AutoSize = true;
      this.opToEltern.Location = new System.Drawing.Point(14, 40);
      this.opToEltern.Name = "opToEltern";
      this.opToEltern.Size = new System.Drawing.Size(63, 21);
      this.opToEltern.TabIndex = 1;
      this.opToEltern.TabStop = true;
      this.opToEltern.Text = "Eltern";
      this.opToEltern.UseVisualStyleBackColor = true;
      // 
      // opToSchueler
      // 
      this.opToSchueler.AutoSize = true;
      this.opToSchueler.Checked = true;
      this.opToSchueler.Location = new System.Drawing.Point(14, 18);
      this.opToSchueler.Name = "opToSchueler";
      this.opToSchueler.Size = new System.Drawing.Size(172, 21);
      this.opToSchueler.TabIndex = 0;
      this.opToSchueler.TabStop = true;
      this.opToSchueler.Text = "schulische Mailadresse";
      this.opToSchueler.UseVisualStyleBackColor = true;
      // 
      // gbReply
      // 
      this.gbReply.Controls.Add(this.opReplyKL);
      this.gbReply.Controls.Add(this.opSekretariat);
      this.gbReply.Controls.Add(this.opReplyDino);
      this.gbReply.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gbReply.Location = new System.Drawing.Point(12, 188);
      this.gbReply.Name = "gbReply";
      this.gbReply.Size = new System.Drawing.Size(300, 95);
      this.gbReply.TabIndex = 90;
      this.gbReply.TabStop = false;
      this.gbReply.Text = "Antwortadresse";
      // 
      // opReplyKL
      // 
      this.opReplyKL.AutoSize = true;
      this.opReplyKL.Location = new System.Drawing.Point(14, 60);
      this.opReplyKL.Name = "opReplyKL";
      this.opReplyKL.Size = new System.Drawing.Size(107, 21);
      this.opReplyKL.TabIndex = 2;
      this.opReplyKL.TabStop = true;
      this.opReplyKL.Text = "Klassenleiter";
      this.opReplyKL.UseVisualStyleBackColor = true;
      // 
      // opSekretariat
      // 
      this.opSekretariat.AutoSize = true;
      this.opSekretariat.Location = new System.Drawing.Point(14, 38);
      this.opSekretariat.Name = "opSekretariat";
      this.opSekretariat.Size = new System.Drawing.Size(95, 21);
      this.opSekretariat.TabIndex = 1;
      this.opSekretariat.TabStop = true;
      this.opSekretariat.Text = "Sekretariat";
      this.opSekretariat.UseVisualStyleBackColor = true;
      // 
      // opReplyDino
      // 
      this.opReplyDino.AutoSize = true;
      this.opReplyDino.Checked = true;
      this.opReplyDino.Location = new System.Drawing.Point(14, 16);
      this.opReplyDino.Name = "opReplyDino";
      this.opReplyDino.Size = new System.Drawing.Size(55, 21);
      this.opReplyDino.TabIndex = 0;
      this.opReplyDino.TabStop = true;
      this.opReplyDino.Text = "diNo";
      this.opReplyDino.UseVisualStyleBackColor = true;
      // 
      // gbAnhang
      // 
      this.gbAnhang.Controls.Add(this.opAnhangAbsenzen);
      this.gbAnhang.Controls.Add(this.opAnhangKeiner);
      this.gbAnhang.Controls.Add(this.opAnhangPDF);
      this.gbAnhang.Controls.Add(this.opAnhangNoten);
      this.gbAnhang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gbAnhang.Location = new System.Drawing.Point(12, 289);
      this.gbAnhang.Name = "gbAnhang";
      this.gbAnhang.Size = new System.Drawing.Size(300, 110);
      this.gbAnhang.TabIndex = 91;
      this.gbAnhang.TabStop = false;
      this.gbAnhang.Text = "Anhang";
      // 
      // opAnhangAbsenzen
      // 
      this.opAnhangAbsenzen.AutoSize = true;
      this.opAnhangAbsenzen.Location = new System.Drawing.Point(14, 82);
      this.opAnhangAbsenzen.Name = "opAnhangAbsenzen";
      this.opAnhangAbsenzen.Size = new System.Drawing.Size(280, 21);
      this.opAnhangAbsenzen.TabIndex = 3;
      this.opAnhangAbsenzen.Text = "Absenzenübersicht (kein Bodytext nötig)";
      this.opAnhangAbsenzen.UseVisualStyleBackColor = true;
      this.opAnhangAbsenzen.Click += new System.EventHandler(this.opAnhangAbsenzen_Click);
      // 
      // opAnhangKeiner
      // 
      this.opAnhangKeiner.AutoSize = true;
      this.opAnhangKeiner.Checked = true;
      this.opAnhangKeiner.Location = new System.Drawing.Point(14, 19);
      this.opAnhangKeiner.Name = "opAnhangKeiner";
      this.opAnhangKeiner.Size = new System.Drawing.Size(65, 21);
      this.opAnhangKeiner.TabIndex = 2;
      this.opAnhangKeiner.TabStop = true;
      this.opAnhangKeiner.Text = "keiner";
      this.opAnhangKeiner.UseVisualStyleBackColor = true;
      // 
      // opAnhangPDF
      // 
      this.opAnhangPDF.AutoSize = true;
      this.opAnhangPDF.Location = new System.Drawing.Point(14, 61);
      this.opAnhangPDF.Name = "opAnhangPDF";
      this.opAnhangPDF.Size = new System.Drawing.Size(224, 21);
      this.opAnhangPDF.TabIndex = 1;
      this.opAnhangPDF.Text = "PDF (gleich für alle Empfänger)";
      this.opAnhangPDF.UseVisualStyleBackColor = true;
      // 
      // opAnhangNoten
      // 
      this.opAnhangNoten.AutoSize = true;
      this.opAnhangNoten.Location = new System.Drawing.Point(14, 40);
      this.opAnhangNoten.Name = "opAnhangNoten";
      this.opAnhangNoten.Size = new System.Drawing.Size(122, 21);
      this.opAnhangNoten.TabIndex = 0;
      this.opAnhangNoten.Text = "Notenübersicht";
      this.opAnhangNoten.UseVisualStyleBackColor = true;
      // 
      // chkTest
      // 
      this.chkTest.AutoSize = true;
      this.chkTest.Checked = true;
      this.chkTest.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkTest.Location = new System.Drawing.Point(12, 56);
      this.chkTest.Name = "chkTest";
      this.chkTest.Size = new System.Drawing.Size(265, 21);
      this.chkTest.TabIndex = 92;
      this.chkTest.Text = "Test (ein Mail an angemeldeten User)";
      this.chkTest.UseVisualStyleBackColor = true;
      // 
      // chkZip
      // 
      this.chkZip.AutoSize = true;
      this.chkZip.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkZip.Location = new System.Drawing.Point(12, 405);
      this.chkZip.Name = "chkZip";
      this.chkZip.Size = new System.Drawing.Size(344, 21);
      this.chkZip.TabIndex = 93;
      this.chkZip.Text = "Anhang zippen und verschlüsseln (Pw=FB-jjjjmmtt)";
      this.chkZip.UseVisualStyleBackColor = true;
      // 
      // lbAnzahl
      // 
      this.lbAnzahl.AutoSize = true;
      this.lbAnzahl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbAnzahl.Location = new System.Drawing.Point(12, 28);
      this.lbAnzahl.Name = "lbAnzahl";
      this.lbAnzahl.Size = new System.Drawing.Size(143, 17);
      this.lbAnzahl.TabIndex = 94;
      this.lbAnzahl.Text = "0 Schüler ausgewählt";
      // 
      // txtBody
      // 
      this.txtBody.Location = new System.Drawing.Point(417, 89);
      this.txtBody.Multiline = true;
      this.txtBody.Name = "txtBody";
      this.txtBody.Size = new System.Drawing.Size(311, 282);
      this.txtBody.TabIndex = 95;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(417, 69);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(202, 17);
      this.label1.TabIndex = 96;
      this.label1.Text = "Bodytext (Anrede automatisch)";
      // 
      // btnSend
      // 
      this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSend.Location = new System.Drawing.Point(417, 405);
      this.btnSend.Name = "btnSend";
      this.btnSend.Size = new System.Drawing.Size(311, 39);
      this.btnSend.TabIndex = 97;
      this.btnSend.Text = "Versenden";
      this.btnSend.UseVisualStyleBackColor = true;
      this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
      // 
      // chkReadBodyText
      // 
      this.chkReadBodyText.AutoSize = true;
      this.chkReadBodyText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkReadBodyText.Location = new System.Drawing.Point(417, 378);
      this.chkReadBodyText.Name = "chkReadBodyText";
      this.chkReadBodyText.Size = new System.Drawing.Size(202, 21);
      this.chkReadBodyText.TabIndex = 98;
      this.chkReadBodyText.Text = "Bodytext aus txt-Datei lesen";
      this.chkReadBodyText.UseVisualStyleBackColor = true;
      // 
      // txtSubject
      // 
      this.txtSubject.Location = new System.Drawing.Point(417, 40);
      this.txtSubject.Name = "txtSubject";
      this.txtSubject.Size = new System.Drawing.Size(311, 20);
      this.txtSubject.TabIndex = 99;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(417, 20);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(50, 17);
      this.label2.TabIndex = 100;
      this.label2.Text = "Betreff";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(414, 461);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(213, 17);
      this.label3.TabIndex = 101;
      this.label3.Text = "Fehlerausgabe unter Downloads";
      // 
      // MailDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(765, 487);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txtSubject);
      this.Controls.Add(this.chkReadBodyText);
      this.Controls.Add(this.btnSend);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtBody);
      this.Controls.Add(this.lbAnzahl);
      this.Controls.Add(this.chkZip);
      this.Controls.Add(this.chkTest);
      this.Controls.Add(this.gbAnhang);
      this.Controls.Add(this.gbReply);
      this.Controls.Add(this.gbEmpfaenger);
      this.Name = "MailDialog";
      this.Text = "Mail versenden";
      this.gbEmpfaenger.ResumeLayout(false);
      this.gbEmpfaenger.PerformLayout();
      this.gbReply.ResumeLayout(false);
      this.gbReply.PerformLayout();
      this.gbAnhang.ResumeLayout(false);
      this.gbAnhang.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

        #endregion

        private System.Windows.Forms.GroupBox gbEmpfaenger;
        private System.Windows.Forms.RadioButton opToEltern;
        private System.Windows.Forms.RadioButton opToSchueler;
        private System.Windows.Forms.GroupBox gbReply;
        private System.Windows.Forms.RadioButton opReplyKL;
        private System.Windows.Forms.RadioButton opSekretariat;
        private System.Windows.Forms.RadioButton opReplyDino;
        private System.Windows.Forms.GroupBox gbAnhang;
        private System.Windows.Forms.RadioButton opAnhangPDF;
        private System.Windows.Forms.RadioButton opAnhangNoten;
        private System.Windows.Forms.RadioButton opAnhangAbsenzen;
        private System.Windows.Forms.RadioButton opAnhangKeiner;
        private System.Windows.Forms.CheckBox chkTest;
        private System.Windows.Forms.CheckBox chkZip;
        private System.Windows.Forms.Label lbAnzahl;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.CheckBox chkReadBodyText;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}