namespace diNoTray
{
  partial class diNoTrayHiddenMainForm
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
        // Release the icon resource.
        trayIcon.Dispose();
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
      this.btnNotenSync = new System.Windows.Forms.Button();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnNotenbogen = new System.Windows.Forms.Button();
      this.btnUnterpunktungen = new System.Windows.Forms.Button();
      this.lblStatus = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.comboBoxZeitpunkt = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnNotenSync
      // 
      this.btnNotenSync.Location = new System.Drawing.Point(21, 79);
      this.btnNotenSync.Name = "btnNotenSync";
      this.btnNotenSync.Size = new System.Drawing.Size(145, 23);
      this.btnNotenSync.TabIndex = 0;
      this.btnNotenSync.Text = "Noten abgeben";
      this.btnNotenSync.UseVisualStyleBackColor = true;
      this.btnNotenSync.Click += new System.EventHandler(this.btnNotenSync_Click);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::diNoTray.Properties.Resources.stegosaurus_304011__180;
      this.pictureBox1.Location = new System.Drawing.Point(260, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(297, 182);
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 35F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.Firebrick;
      this.label1.Location = new System.Drawing.Point(12, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(123, 54);
      this.label1.TabIndex = 2;
      this.label1.Text = "diNo";
      // 
      // btnNotenbogen
      // 
      this.btnNotenbogen.Location = new System.Drawing.Point(21, 108);
      this.btnNotenbogen.Name = "btnNotenbogen";
      this.btnNotenbogen.Size = new System.Drawing.Size(145, 23);
      this.btnNotenbogen.TabIndex = 3;
      this.btnNotenbogen.Text = "Notenbogen anschauen";
      this.btnNotenbogen.UseVisualStyleBackColor = true;
      this.btnNotenbogen.Click += new System.EventHandler(this.btnNotenbogen_Click);
      // 
      // btnUnterpunktungen
      // 
      this.btnUnterpunktungen.Enabled = false;
      this.btnUnterpunktungen.Location = new System.Drawing.Point(6, 46);
      this.btnUnterpunktungen.Name = "btnUnterpunktungen";
      this.btnUnterpunktungen.Size = new System.Drawing.Size(139, 23);
      this.btnUnterpunktungen.TabIndex = 4;
      this.btnUnterpunktungen.Text = "Prüfung starten";
      this.btnUnterpunktungen.UseVisualStyleBackColor = true;
      this.btnUnterpunktungen.Click += new System.EventHandler(this.btnUnterpunktungen_Click);
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new System.Drawing.Point(18, 228);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(40, 13);
      this.lblStatus.TabIndex = 5;
      this.lblStatus.Text = "Status:";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.comboBoxZeitpunkt);
      this.groupBox1.Controls.Add(this.btnUnterpunktungen);
      this.groupBox1.Location = new System.Drawing.Point(21, 137);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(155, 76);
      this.groupBox1.TabIndex = 6;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Notenprüfung";
      // 
      // comboBoxZeitpunkt
      // 
      this.comboBoxZeitpunkt.FormattingEnabled = true;
      this.comboBoxZeitpunkt.Items.AddRange(new object[] {
            "Probezeit BOS",
            "Halbjahr",
            "1. PA",
            "2. PA",
            "Jahresende"});
      this.comboBoxZeitpunkt.Location = new System.Drawing.Point(6, 19);
      this.comboBoxZeitpunkt.Name = "comboBoxZeitpunkt";
      this.comboBoxZeitpunkt.Size = new System.Drawing.Size(139, 21);
      this.comboBoxZeitpunkt.TabIndex = 11;
      this.comboBoxZeitpunkt.SelectedIndexChanged += new System.EventHandler(this.comboBoxZeitpunkt_SelectedIndexChanged);
      // 
      // diNoTrayHiddenMainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(569, 250);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.btnNotenbogen);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.btnNotenSync);
      this.Name = "diNoTrayHiddenMainForm";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnNotenSync;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnNotenbogen;
    private System.Windows.Forms.Button btnUnterpunktungen;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ComboBox comboBoxZeitpunkt;
  }
}

