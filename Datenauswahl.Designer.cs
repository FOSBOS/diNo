namespace diNo
{
  partial class Datenauswahl
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
      this.cbVorkommnisArt = new System.Windows.Forms.ComboBox();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.cbAuswahl = new System.Windows.Forms.ComboBox();
      this.cbZubringerschule = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // cbVorkommnisArt
      // 
      this.cbVorkommnisArt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbVorkommnisArt.FormattingEnabled = true;
      this.cbVorkommnisArt.Location = new System.Drawing.Point(12, 81);
      this.cbVorkommnisArt.Name = "cbVorkommnisArt";
      this.cbVorkommnisArt.Size = new System.Drawing.Size(327, 24);
      this.cbVorkommnisArt.TabIndex = 2;
      // 
      // btnOK
      // 
      this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnOK.Location = new System.Drawing.Point(12, 161);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(159, 40);
      this.btnOK.TabIndex = 4;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.Location = new System.Drawing.Point(190, 161);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(149, 40);
      this.btnCancel.TabIndex = 5;
      this.btnCancel.Text = "Abbrechen";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // cbAuswahl
      // 
      this.cbAuswahl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbAuswahl.FormattingEnabled = true;
      this.cbAuswahl.Items.AddRange(new object[] {
            "mit Vorkommnis",
            "von Zubringerschule",
            "die diese Jahrgangsstufe wiederholen",
            "mit Probezeit",
            "mit 2. Fremdsprache"});
      this.cbAuswahl.Location = new System.Drawing.Point(12, 40);
      this.cbAuswahl.Name = "cbAuswahl";
      this.cbAuswahl.Size = new System.Drawing.Size(327, 24);
      this.cbAuswahl.TabIndex = 6;
      this.cbAuswahl.SelectedIndexChanged += new System.EventHandler(this.cbAuswahl_SelectedIndexChanged);
      // 
      // cbZubringerschule
      // 
      this.cbZubringerschule.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbZubringerschule.FormattingEnabled = true;
      this.cbZubringerschule.Items.AddRange(new object[] {
            "*",
            "RS*",
            "RS1",
            "RS2",
            "RS3a",
            "RS3b",
            "GY*",
            "GY0",
            "GY1",
            "WS",
            "F10"});
      this.cbZubringerschule.Location = new System.Drawing.Point(12, 122);
      this.cbZubringerschule.Name = "cbZubringerschule";
      this.cbZubringerschule.Size = new System.Drawing.Size(327, 24);
      this.cbZubringerschule.TabIndex = 7;
      // 
      // Datenauswahl
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(362, 228);
      this.Controls.Add(this.cbZubringerschule);
      this.Controls.Add(this.cbAuswahl);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.cbVorkommnisArt);
      this.Name = "Datenauswahl";
      this.Text = "Datenauswahl";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox cbVorkommnisArt;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbAuswahl;
        private System.Windows.Forms.ComboBox cbZubringerschule;
    }
}