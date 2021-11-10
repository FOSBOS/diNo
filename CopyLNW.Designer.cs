namespace diNo
{
  partial class CopyLNW
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
            this.btnAngabe = new System.Windows.Forms.Button();
            this.cbKurs = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbArt = new System.Windows.Forms.ComboBox();
            this.lbArt = new System.Windows.Forms.Label();
            this.cbNummer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLsg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAngabe
            // 
            this.btnAngabe.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAngabe.Location = new System.Drawing.Point(44, 199);
            this.btnAngabe.Name = "btnAngabe";
            this.btnAngabe.Size = new System.Drawing.Size(167, 66);
            this.btnAngabe.TabIndex = 0;
            this.btnAngabe.Text = "Angabe abgeben";
            this.btnAngabe.UseVisualStyleBackColor = true;
            this.btnAngabe.Click += new System.EventHandler(this.btnAngabe_Click);
            // 
            // cbKurs
            // 
            this.cbKurs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbKurs.FormattingEnabled = true;
            this.cbKurs.Location = new System.Drawing.Point(141, 31);
            this.cbKurs.Name = "cbKurs";
            this.cbKurs.Size = new System.Drawing.Size(298, 24);
            this.cbKurs.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(41, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 17);
            this.label2.TabIndex = 17;
            this.label2.Text = "Kurs";
            // 
            // cbArt
            // 
            this.cbArt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbArt.FormattingEnabled = true;
            this.cbArt.Items.AddRange(new object[] {
            "SA",
            "KA",
            "Ex"});
            this.cbArt.Location = new System.Drawing.Point(365, 78);
            this.cbArt.Name = "cbArt";
            this.cbArt.Size = new System.Drawing.Size(74, 24);
            this.cbArt.TabIndex = 20;
            // 
            // lbArt
            // 
            this.lbArt.AutoSize = true;
            this.lbArt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbArt.Location = new System.Drawing.Point(41, 81);
            this.lbArt.Name = "lbArt";
            this.lbArt.Size = new System.Drawing.Size(191, 17);
            this.lbArt.TabIndex = 19;
            this.lbArt.Text = "Art des Leistungsnachweises";
            // 
            // cbNummer
            // 
            this.cbNummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbNummer.FormattingEnabled = true;
            this.cbNummer.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cbNummer.Location = new System.Drawing.Point(365, 123);
            this.cbNummer.Name = "cbNummer";
            this.cbNummer.Size = new System.Drawing.Size(74, 24);
            this.cbNummer.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(41, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 17);
            this.label1.TabIndex = 21;
            this.label1.Text = "Nummer in diesem Halbjahr";
            // 
            // btnLsg
            // 
            this.btnLsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLsg.Location = new System.Drawing.Point(269, 199);
            this.btnLsg.Name = "btnLsg";
            this.btnLsg.Size = new System.Drawing.Size(170, 66);
            this.btnLsg.TabIndex = 23;
            this.btnLsg.Text = "Erwartungshorizont abgeben";
            this.btnLsg.UseVisualStyleBackColor = true;
            this.btnLsg.Click += new System.EventHandler(this.btnLsg_Click);
            // 
            // CopyLNW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 314);
            this.Controls.Add(this.btnLsg);
            this.Controls.Add(this.cbNummer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbArt);
            this.Controls.Add(this.lbArt);
            this.Controls.Add(this.cbKurs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAngabe);
            this.Name = "CopyLNW";
            this.Text = "Abgabe von Leistungsnachweisen";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnAngabe;
    private System.Windows.Forms.ComboBox cbKurs;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cbArt;
    private System.Windows.Forms.Label lbArt;
    private System.Windows.Forms.ComboBox cbNummer;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnLsg;
  }
}