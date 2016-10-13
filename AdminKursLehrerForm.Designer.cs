namespace diNo
{
  partial class AdminKursLehrerForm
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
      this.listBoxKurse = new System.Windows.Forms.ListBox();
      this.comboBoxLehrer = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textBoxSuche = new System.Windows.Forms.TextBox();
      this.btnSucheKurse = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // listBoxKurse
      // 
      this.listBoxKurse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.listBoxKurse.DisplayMember = "Kursbezeichnung";
      this.listBoxKurse.FormattingEnabled = true;
      this.listBoxKurse.Location = new System.Drawing.Point(12, 55);
      this.listBoxKurse.Name = "listBoxKurse";
      this.listBoxKurse.Size = new System.Drawing.Size(248, 355);
      this.listBoxKurse.Sorted = true;
      this.listBoxKurse.TabIndex = 0;
      this.listBoxKurse.SelectedValueChanged += new System.EventHandler(this.listBoxKurse_SelectedValueChanged);
      // 
      // comboBoxLehrer
      // 
      this.comboBoxLehrer.DisplayMember = "KompletterName";
      this.comboBoxLehrer.FormattingEnabled = true;
      this.comboBoxLehrer.Location = new System.Drawing.Point(410, 120);
      this.comboBoxLehrer.Name = "comboBoxLehrer";
      this.comboBoxLehrer.Size = new System.Drawing.Size(121, 21);
      this.comboBoxLehrer.TabIndex = 1;
      this.comboBoxLehrer.SelectedValueChanged += new System.EventHandler(this.comboBoxLehrer_SelectedValueChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(349, 123);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(37, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Lehrer";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 20);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(72, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Kurse suchen";
      // 
      // textBoxSuche
      // 
      this.textBoxSuche.Location = new System.Drawing.Point(90, 18);
      this.textBoxSuche.Name = "textBoxSuche";
      this.textBoxSuche.Size = new System.Drawing.Size(129, 20);
      this.textBoxSuche.TabIndex = 4;
      // 
      // btnSucheKurse
      // 
      this.btnSucheKurse.Image = global::diNo.Properties.Resources.Lupe;
      this.btnSucheKurse.Location = new System.Drawing.Point(225, 12);
      this.btnSucheKurse.Name = "btnSucheKurse";
      this.btnSucheKurse.Size = new System.Drawing.Size(35, 35);
      this.btnSucheKurse.TabIndex = 5;
      this.btnSucheKurse.UseVisualStyleBackColor = true;
      this.btnSucheKurse.Click += new System.EventHandler(this.btnSucheKurse_Click);
      // 
      // AdminKursLehrer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(801, 420);
      this.Controls.Add(this.btnSucheKurse);
      this.Controls.Add(this.textBoxSuche);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.comboBoxLehrer);
      this.Controls.Add(this.listBoxKurse);
      this.Name = "AdminKursLehrer";
      this.Text = "AdminKursLehrer";
      this.Load += new System.EventHandler(this.AdminKursLehrer_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox listBoxKurse;
    private System.Windows.Forms.ComboBox comboBoxLehrer;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBoxSuche;
    private System.Windows.Forms.Button btnSucheKurse;
  }
}