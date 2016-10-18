namespace diNo
{
  partial class AdminBerechtigungenForm
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
      this.comboBoxLehrer = new System.Windows.Forms.ComboBox();
      this.lblLehrer = new System.Windows.Forms.Label();
      this.checkBoxIsAdmin = new System.Windows.Forms.CheckBox();
      this.groupBoxBerechtigungen = new System.Windows.Forms.GroupBox();
      this.checkBoxIsSchulleitung = new System.Windows.Forms.CheckBox();
      this.checkBoxIsSekretariat = new System.Windows.Forms.CheckBox();
      this.checkBoxIsSeminarfach = new System.Windows.Forms.CheckBox();
      this.checkBoxIsFpAWirtschaft = new System.Windows.Forms.CheckBox();
      this.checkBoxIsFpASozial = new System.Windows.Forms.CheckBox();
      this.checkBoxIsFpATechnik = new System.Windows.Forms.CheckBox();
      this.checkBoxIsFpAAgrar = new System.Windows.Forms.CheckBox();
      this.comboBoxBerechtigungen = new System.Windows.Forms.ComboBox();
      this.labelBerechtigungsstufe = new System.Windows.Forms.Label();
      this.textBoxLehrerNamen = new System.Windows.Forms.TextBox();
      this.groupBoxBerechtigungen.SuspendLayout();
      this.SuspendLayout();
      // 
      // comboBoxLehrer
      // 
      this.comboBoxLehrer.DisplayMember = "Name";
      this.comboBoxLehrer.FormattingEnabled = true;
      this.comboBoxLehrer.Location = new System.Drawing.Point(12, 43);
      this.comboBoxLehrer.Name = "comboBoxLehrer";
      this.comboBoxLehrer.Size = new System.Drawing.Size(180, 21);
      this.comboBoxLehrer.TabIndex = 0;
      this.comboBoxLehrer.SelectedValueChanged += new System.EventHandler(this.comboBoxLehrer_SelectedValueChanged);
      // 
      // lblLehrer
      // 
      this.lblLehrer.AutoSize = true;
      this.lblLehrer.Location = new System.Drawing.Point(12, 27);
      this.lblLehrer.Name = "lblLehrer";
      this.lblLehrer.Size = new System.Drawing.Size(49, 13);
      this.lblLehrer.TabIndex = 1;
      this.lblLehrer.Text = "Lehrkraft";
      // 
      // checkBoxIsAdmin
      // 
      this.checkBoxIsAdmin.AutoSize = true;
      this.checkBoxIsAdmin.Location = new System.Drawing.Point(6, 19);
      this.checkBoxIsAdmin.Name = "checkBoxIsAdmin";
      this.checkBoxIsAdmin.Size = new System.Drawing.Size(86, 17);
      this.checkBoxIsAdmin.TabIndex = 2;
      this.checkBoxIsAdmin.Text = "Administrator";
      this.checkBoxIsAdmin.UseVisualStyleBackColor = true;
      this.checkBoxIsAdmin.CheckedChanged += new System.EventHandler(this.checkBoxIsAdmin_CheckedChanged);
      // 
      // groupBoxBerechtigungen
      // 
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpAAgrar);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpATechnik);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpASozial);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpAWirtschaft);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsSeminarfach);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsSekretariat);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsSchulleitung);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsAdmin);
      this.groupBoxBerechtigungen.Location = new System.Drawing.Point(210, 27);
      this.groupBoxBerechtigungen.Name = "groupBoxBerechtigungen";
      this.groupBoxBerechtigungen.Size = new System.Drawing.Size(316, 115);
      this.groupBoxBerechtigungen.TabIndex = 3;
      this.groupBoxBerechtigungen.TabStop = false;
      this.groupBoxBerechtigungen.Text = "Berechtigungen";
      // 
      // checkBoxIsSchulleitung
      // 
      this.checkBoxIsSchulleitung.AutoSize = true;
      this.checkBoxIsSchulleitung.Location = new System.Drawing.Point(6, 42);
      this.checkBoxIsSchulleitung.Name = "checkBoxIsSchulleitung";
      this.checkBoxIsSchulleitung.Size = new System.Drawing.Size(84, 17);
      this.checkBoxIsSchulleitung.TabIndex = 3;
      this.checkBoxIsSchulleitung.Text = "Schulleitung";
      this.checkBoxIsSchulleitung.UseVisualStyleBackColor = true;
      this.checkBoxIsSchulleitung.CheckedChanged += new System.EventHandler(this.checkBoxIsSchulleitung_CheckedChanged);
      // 
      // checkBoxIsSekretariat
      // 
      this.checkBoxIsSekretariat.AutoSize = true;
      this.checkBoxIsSekretariat.Location = new System.Drawing.Point(6, 65);
      this.checkBoxIsSekretariat.Name = "checkBoxIsSekretariat";
      this.checkBoxIsSekretariat.Size = new System.Drawing.Size(77, 17);
      this.checkBoxIsSekretariat.TabIndex = 4;
      this.checkBoxIsSekretariat.Text = "Sekretariat";
      this.checkBoxIsSekretariat.UseVisualStyleBackColor = true;
      this.checkBoxIsSekretariat.CheckedChanged += new System.EventHandler(this.checkBoxIsSekretariat_CheckedChanged);
      // 
      // checkBoxIsSeminarfach
      // 
      this.checkBoxIsSeminarfach.AutoSize = true;
      this.checkBoxIsSeminarfach.Location = new System.Drawing.Point(6, 88);
      this.checkBoxIsSeminarfach.Name = "checkBoxIsSeminarfach";
      this.checkBoxIsSeminarfach.Size = new System.Drawing.Size(85, 17);
      this.checkBoxIsSeminarfach.TabIndex = 5;
      this.checkBoxIsSeminarfach.Text = "Seminarfach";
      this.checkBoxIsSeminarfach.UseVisualStyleBackColor = true;
      this.checkBoxIsSeminarfach.CheckedChanged += new System.EventHandler(this.checkBoxIsSeminarfach_CheckedChanged);
      // 
      // checkBoxIsFpAWirtschaft
      // 
      this.checkBoxIsFpAWirtschaft.AutoSize = true;
      this.checkBoxIsFpAWirtschaft.Location = new System.Drawing.Point(178, 19);
      this.checkBoxIsFpAWirtschaft.Name = "checkBoxIsFpAWirtschaft";
      this.checkBoxIsFpAWirtschaft.Size = new System.Drawing.Size(96, 17);
      this.checkBoxIsFpAWirtschaft.TabIndex = 6;
      this.checkBoxIsFpAWirtschaft.Text = "FpA Wirtschaft";
      this.checkBoxIsFpAWirtschaft.UseVisualStyleBackColor = true;
      this.checkBoxIsFpAWirtschaft.CheckedChanged += new System.EventHandler(this.checkBoxIsFpAWirtschaft_CheckedChanged);
      // 
      // checkBoxIsFpASozial
      // 
      this.checkBoxIsFpASozial.AutoSize = true;
      this.checkBoxIsFpASozial.Location = new System.Drawing.Point(178, 42);
      this.checkBoxIsFpASozial.Name = "checkBoxIsFpASozial";
      this.checkBoxIsFpASozial.Size = new System.Drawing.Size(76, 17);
      this.checkBoxIsFpASozial.TabIndex = 7;
      this.checkBoxIsFpASozial.Text = "FpA Sozial";
      this.checkBoxIsFpASozial.UseVisualStyleBackColor = true;
      this.checkBoxIsFpASozial.CheckedChanged += new System.EventHandler(this.checkBoxIsFpASozial_CheckedChanged);
      // 
      // checkBoxIsFpATechnik
      // 
      this.checkBoxIsFpATechnik.AutoSize = true;
      this.checkBoxIsFpATechnik.Location = new System.Drawing.Point(178, 65);
      this.checkBoxIsFpATechnik.Name = "checkBoxIsFpATechnik";
      this.checkBoxIsFpATechnik.Size = new System.Drawing.Size(87, 17);
      this.checkBoxIsFpATechnik.TabIndex = 8;
      this.checkBoxIsFpATechnik.Text = "FpA Technik";
      this.checkBoxIsFpATechnik.UseVisualStyleBackColor = true;
      this.checkBoxIsFpATechnik.CheckedChanged += new System.EventHandler(this.checkBoxIsFpATechnik_CheckedChanged);
      // 
      // checkBoxIsFpAAgrar
      // 
      this.checkBoxIsFpAAgrar.AutoSize = true;
      this.checkBoxIsFpAAgrar.Location = new System.Drawing.Point(178, 88);
      this.checkBoxIsFpAAgrar.Name = "checkBoxIsFpAAgrar";
      this.checkBoxIsFpAAgrar.Size = new System.Drawing.Size(73, 17);
      this.checkBoxIsFpAAgrar.TabIndex = 9;
      this.checkBoxIsFpAAgrar.Text = "FpA Agrar";
      this.checkBoxIsFpAAgrar.UseVisualStyleBackColor = true;
      this.checkBoxIsFpAAgrar.CheckedChanged += new System.EventHandler(this.checkBoxIsFpAAgrar_CheckedChanged);
      // 
      // comboBoxBerechtigungen
      // 
      this.comboBoxBerechtigungen.DisplayMember = "Value";
      this.comboBoxBerechtigungen.FormattingEnabled = true;
      this.comboBoxBerechtigungen.Location = new System.Drawing.Point(12, 222);
      this.comboBoxBerechtigungen.Name = "comboBoxBerechtigungen";
      this.comboBoxBerechtigungen.Size = new System.Drawing.Size(180, 21);
      this.comboBoxBerechtigungen.TabIndex = 4;
      this.comboBoxBerechtigungen.SelectedValueChanged += new System.EventHandler(this.comboBoxBerechtigungen_SelectedValueChanged);
      // 
      // labelBerechtigungsstufe
      // 
      this.labelBerechtigungsstufe.AutoSize = true;
      this.labelBerechtigungsstufe.Location = new System.Drawing.Point(12, 206);
      this.labelBerechtigungsstufe.Name = "labelBerechtigungsstufe";
      this.labelBerechtigungsstufe.Size = new System.Drawing.Size(168, 13);
      this.labelBerechtigungsstufe.TabIndex = 5;
      this.labelBerechtigungsstufe.Text = "Zeige alle Lehrer mit Berechtigung";
      // 
      // textBoxLehrerNamen
      // 
      this.textBoxLehrerNamen.Location = new System.Drawing.Point(216, 223);
      this.textBoxLehrerNamen.Multiline = true;
      this.textBoxLehrerNamen.Name = "textBoxLehrerNamen";
      this.textBoxLehrerNamen.ReadOnly = true;
      this.textBoxLehrerNamen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBoxLehrerNamen.Size = new System.Drawing.Size(310, 192);
      this.textBoxLehrerNamen.TabIndex = 6;
      // 
      // AdminBerechtigungenForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(733, 471);
      this.Controls.Add(this.textBoxLehrerNamen);
      this.Controls.Add(this.labelBerechtigungsstufe);
      this.Controls.Add(this.comboBoxBerechtigungen);
      this.Controls.Add(this.groupBoxBerechtigungen);
      this.Controls.Add(this.lblLehrer);
      this.Controls.Add(this.comboBoxLehrer);
      this.Name = "AdminBerechtigungenForm";
      this.Text = "AdminBerechtigungenForm";
      this.Load += new System.EventHandler(this.AdminBerechtigungenForm_Load);
      this.groupBoxBerechtigungen.ResumeLayout(false);
      this.groupBoxBerechtigungen.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox comboBoxLehrer;
    private System.Windows.Forms.Label lblLehrer;
    private System.Windows.Forms.CheckBox checkBoxIsAdmin;
    private System.Windows.Forms.GroupBox groupBoxBerechtigungen;
    private System.Windows.Forms.CheckBox checkBoxIsFpAAgrar;
    private System.Windows.Forms.CheckBox checkBoxIsFpATechnik;
    private System.Windows.Forms.CheckBox checkBoxIsFpASozial;
    private System.Windows.Forms.CheckBox checkBoxIsFpAWirtschaft;
    private System.Windows.Forms.CheckBox checkBoxIsSeminarfach;
    private System.Windows.Forms.CheckBox checkBoxIsSekretariat;
    private System.Windows.Forms.CheckBox checkBoxIsSchulleitung;
    private System.Windows.Forms.ComboBox comboBoxBerechtigungen;
    private System.Windows.Forms.Label labelBerechtigungsstufe;
    private System.Windows.Forms.TextBox textBoxLehrerNamen;
  }
}