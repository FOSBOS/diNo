namespace diNo
{
  partial class LehrerForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LehrerForm));
      this.edNachname = new System.Windows.Forms.TextBox();
      this.lblNachname = new System.Windows.Forms.Label();
      this.edVorname = new System.Windows.Forms.TextBox();
      this.lbVorname = new System.Windows.Forms.Label();
      this.edMail = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.edDienstbez = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.edWindowsname = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.edKuerzel = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.listLehrer = new System.Windows.Forms.ListBox();
      this.btnDel = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.gbGeschlecht = new System.Windows.Forms.GroupBox();
      this.opWeiblich = new System.Windows.Forms.RadioButton();
      this.opMaennlich = new System.Windows.Forms.RadioButton();
      this.groupBoxBerechtigungen = new System.Windows.Forms.GroupBox();
      this.checkBoxIsFpAUmwelt = new System.Windows.Forms.CheckBox();
      this.checkBoxIsFpATechnik = new System.Windows.Forms.CheckBox();
      this.checkBoxIsFpASozial = new System.Windows.Forms.CheckBox();
      this.checkBoxIsFpAWirtschaft = new System.Windows.Forms.CheckBox();
      this.checkBoxIsSeminarfach = new System.Windows.Forms.CheckBox();
      this.checkBoxIsSekretariat = new System.Windows.Forms.CheckBox();
      this.checkBoxIsSchulleitung = new System.Windows.Forms.CheckBox();
      this.checkBoxIsAdmin = new System.Windows.Forms.CheckBox();
      this.gbGeschlecht.SuspendLayout();
      this.groupBoxBerechtigungen.SuspendLayout();
      this.SuspendLayout();
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.Images.SetKeyName(0, "Cancel.png");
      this.imageList1.Images.SetKeyName(1, "Ok.png");
      this.imageList1.Images.SetKeyName(2, "Edit.png");
      this.imageList1.Images.SetKeyName(3, "print.png");
      this.imageList1.Images.SetKeyName(4, "Save.png");
      this.imageList1.Images.SetKeyName(5, "Excel.png");
      this.imageList1.Images.SetKeyName(6, "Plus.png");
      this.imageList1.Images.SetKeyName(7, "Minus.png");
      // 
      // edNachname
      // 
      this.edNachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edNachname.Location = new System.Drawing.Point(244, 45);
      this.edNachname.Name = "edNachname";
      this.edNachname.Size = new System.Drawing.Size(240, 20);
      this.edNachname.TabIndex = 1;
      // 
      // lblNachname
      // 
      this.lblNachname.AutoSize = true;
      this.lblNachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblNachname.Location = new System.Drawing.Point(241, 28);
      this.lblNachname.Name = "lblNachname";
      this.lblNachname.Size = new System.Drawing.Size(59, 13);
      this.lblNachname.TabIndex = 9;
      this.lblNachname.Text = "Nachname";
      // 
      // edVorname
      // 
      this.edVorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edVorname.Location = new System.Drawing.Point(244, 86);
      this.edVorname.Name = "edVorname";
      this.edVorname.Size = new System.Drawing.Size(240, 20);
      this.edVorname.TabIndex = 2;
      // 
      // lbVorname
      // 
      this.lbVorname.AutoSize = true;
      this.lbVorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbVorname.Location = new System.Drawing.Point(241, 69);
      this.lbVorname.Name = "lbVorname";
      this.lbVorname.Size = new System.Drawing.Size(49, 13);
      this.lbVorname.TabIndex = 11;
      this.lbVorname.Text = "Vorname";
      // 
      // edMail
      // 
      this.edMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edMail.Location = new System.Drawing.Point(244, 168);
      this.edMail.Name = "edMail";
      this.edMail.Size = new System.Drawing.Size(240, 20);
      this.edMail.TabIndex = 5;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(241, 151);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(32, 13);
      this.label1.TabIndex = 13;
      this.label1.Text = "eMail";
      // 
      // edDienstbez
      // 
      this.edDienstbez.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edDienstbez.Location = new System.Drawing.Point(244, 127);
      this.edDienstbez.Name = "edDienstbez";
      this.edDienstbez.Size = new System.Drawing.Size(95, 20);
      this.edDienstbez.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(241, 110);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(98, 13);
      this.label2.TabIndex = 15;
      this.label2.Text = "Dienstbezeichnung";
      // 
      // edWindowsname
      // 
      this.edWindowsname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edWindowsname.Location = new System.Drawing.Point(244, 209);
      this.edWindowsname.Name = "edWindowsname";
      this.edWindowsname.Size = new System.Drawing.Size(240, 20);
      this.edWindowsname.TabIndex = 6;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(241, 192);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(77, 13);
      this.label3.TabIndex = 17;
      this.label3.Text = "Windowsname";
      // 
      // edKuerzel
      // 
      this.edKuerzel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edKuerzel.Location = new System.Drawing.Point(389, 127);
      this.edKuerzel.Name = "edKuerzel";
      this.edKuerzel.Size = new System.Drawing.Size(95, 20);
      this.edKuerzel.TabIndex = 4;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(386, 111);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(36, 13);
      this.label4.TabIndex = 19;
      this.label4.Text = "Kürzel";
      // 
      // listLehrer
      // 
      this.listLehrer.DisplayMember = "KompletterName";
      this.listLehrer.FormattingEnabled = true;
      this.listLehrer.Location = new System.Drawing.Point(25, 27);
      this.listLehrer.Name = "listLehrer";
      this.listLehrer.Size = new System.Drawing.Size(180, 420);
      this.listLehrer.TabIndex = 20;
      this.listLehrer.SelectedValueChanged += new System.EventHandler(this.listLehrer_SelectedValueChanged);
      // 
      // btnDel
      // 
      this.btnDel.ImageIndex = 7;
      this.btnDel.ImageList = this.imageList1;
      this.btnDel.Location = new System.Drawing.Point(71, 462);
      this.btnDel.Name = "btnDel";
      this.btnDel.Size = new System.Drawing.Size(40, 40);
      this.btnDel.TabIndex = 25;
      this.btnDel.UseVisualStyleBackColor = true;
      this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
      // 
      // btnAdd
      // 
      this.btnAdd.ImageIndex = 6;
      this.btnAdd.ImageList = this.imageList1;
      this.btnAdd.Location = new System.Drawing.Point(25, 462);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(40, 40);
      this.btnAdd.TabIndex = 24;
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnSave
      // 
      this.btnSave.ImageIndex = 4;
      this.btnSave.ImageList = this.imageList1;
      this.btnSave.Location = new System.Drawing.Point(444, 462);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 26;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // gbGeschlecht
      // 
      this.gbGeschlecht.Controls.Add(this.opWeiblich);
      this.gbGeschlecht.Controls.Add(this.opMaennlich);
      this.gbGeschlecht.Location = new System.Drawing.Point(244, 250);
      this.gbGeschlecht.Name = "gbGeschlecht";
      this.gbGeschlecht.Size = new System.Drawing.Size(95, 61);
      this.gbGeschlecht.TabIndex = 27;
      this.gbGeschlecht.TabStop = false;
      this.gbGeschlecht.Text = "Geschlecht";
      // 
      // opWeiblich
      // 
      this.opWeiblich.AutoSize = true;
      this.opWeiblich.Location = new System.Drawing.Point(10, 35);
      this.opWeiblich.Name = "opWeiblich";
      this.opWeiblich.Size = new System.Drawing.Size(63, 17);
      this.opWeiblich.TabIndex = 2;
      this.opWeiblich.Text = "weiblich";
      this.opWeiblich.UseVisualStyleBackColor = true;
      // 
      // opMaennlich
      // 
      this.opMaennlich.AutoSize = true;
      this.opMaennlich.Checked = true;
      this.opMaennlich.Location = new System.Drawing.Point(10, 17);
      this.opMaennlich.Name = "opMaennlich";
      this.opMaennlich.Size = new System.Drawing.Size(67, 17);
      this.opMaennlich.TabIndex = 1;
      this.opMaennlich.TabStop = true;
      this.opMaennlich.Text = "männlich";
      this.opMaennlich.UseVisualStyleBackColor = true;
      // 
      // groupBoxBerechtigungen
      // 
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpAUmwelt);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpATechnik);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpASozial);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsFpAWirtschaft);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsSeminarfach);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsSekretariat);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsSchulleitung);
      this.groupBoxBerechtigungen.Controls.Add(this.checkBoxIsAdmin);
      this.groupBoxBerechtigungen.Location = new System.Drawing.Point(244, 332);
      this.groupBoxBerechtigungen.Name = "groupBoxBerechtigungen";
      this.groupBoxBerechtigungen.Size = new System.Drawing.Size(240, 115);
      this.groupBoxBerechtigungen.TabIndex = 28;
      this.groupBoxBerechtigungen.TabStop = false;
      this.groupBoxBerechtigungen.Text = "Berechtigungen";
      // 
      // checkBoxIsFpAUmwelt
      // 
      this.checkBoxIsFpAUmwelt.AutoSize = true;
      this.checkBoxIsFpAUmwelt.Location = new System.Drawing.Point(139, 88);
      this.checkBoxIsFpAUmwelt.Name = "checkBoxIsFpAUmwelt";
      this.checkBoxIsFpAUmwelt.Size = new System.Drawing.Size(83, 17);
      this.checkBoxIsFpAUmwelt.TabIndex = 9;
      this.checkBoxIsFpAUmwelt.Text = "FpA Umwelt";
      this.checkBoxIsFpAUmwelt.UseVisualStyleBackColor = true;
      // 
      // checkBoxIsFpATechnik
      // 
      this.checkBoxIsFpATechnik.AutoSize = true;
      this.checkBoxIsFpATechnik.Location = new System.Drawing.Point(139, 65);
      this.checkBoxIsFpATechnik.Name = "checkBoxIsFpATechnik";
      this.checkBoxIsFpATechnik.Size = new System.Drawing.Size(87, 17);
      this.checkBoxIsFpATechnik.TabIndex = 8;
      this.checkBoxIsFpATechnik.Text = "FpA Technik";
      this.checkBoxIsFpATechnik.UseVisualStyleBackColor = true;
      // 
      // checkBoxIsFpASozial
      // 
      this.checkBoxIsFpASozial.AutoSize = true;
      this.checkBoxIsFpASozial.Location = new System.Drawing.Point(139, 42);
      this.checkBoxIsFpASozial.Name = "checkBoxIsFpASozial";
      this.checkBoxIsFpASozial.Size = new System.Drawing.Size(76, 17);
      this.checkBoxIsFpASozial.TabIndex = 7;
      this.checkBoxIsFpASozial.Text = "FpA Sozial";
      this.checkBoxIsFpASozial.UseVisualStyleBackColor = true;
      // 
      // checkBoxIsFpAWirtschaft
      // 
      this.checkBoxIsFpAWirtschaft.AutoSize = true;
      this.checkBoxIsFpAWirtschaft.Location = new System.Drawing.Point(139, 19);
      this.checkBoxIsFpAWirtschaft.Name = "checkBoxIsFpAWirtschaft";
      this.checkBoxIsFpAWirtschaft.Size = new System.Drawing.Size(96, 17);
      this.checkBoxIsFpAWirtschaft.TabIndex = 6;
      this.checkBoxIsFpAWirtschaft.Text = "FpA Wirtschaft";
      this.checkBoxIsFpAWirtschaft.UseVisualStyleBackColor = true;
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
      // 
      // LehrerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(528, 524);
      this.Controls.Add(this.groupBoxBerechtigungen);
      this.Controls.Add(this.gbGeschlecht);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnDel);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.listLehrer);
      this.Controls.Add(this.edKuerzel);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.edWindowsname);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.edDienstbez);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.edMail);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.edVorname);
      this.Controls.Add(this.lbVorname);
      this.Controls.Add(this.edNachname);
      this.Controls.Add(this.lblNachname);
      this.Name = "LehrerForm";
      this.Text = "Lehrer";
      this.gbGeschlecht.ResumeLayout(false);
      this.gbGeschlecht.PerformLayout();
      this.groupBoxBerechtigungen.ResumeLayout(false);
      this.groupBoxBerechtigungen.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.TextBox edNachname;
    private System.Windows.Forms.Label lblNachname;
    private System.Windows.Forms.TextBox edVorname;
    private System.Windows.Forms.Label lbVorname;
    private System.Windows.Forms.TextBox edMail;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edDienstbez;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox edWindowsname;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox edKuerzel;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ListBox listLehrer;
    private System.Windows.Forms.Button btnDel;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.GroupBox gbGeschlecht;
    private System.Windows.Forms.RadioButton opWeiblich;
    private System.Windows.Forms.RadioButton opMaennlich;
    private System.Windows.Forms.GroupBox groupBoxBerechtigungen;
    private System.Windows.Forms.CheckBox checkBoxIsFpAUmwelt;
    private System.Windows.Forms.CheckBox checkBoxIsFpATechnik;
    private System.Windows.Forms.CheckBox checkBoxIsFpASozial;
    private System.Windows.Forms.CheckBox checkBoxIsFpAWirtschaft;
    private System.Windows.Forms.CheckBox checkBoxIsSeminarfach;
    private System.Windows.Forms.CheckBox checkBoxIsSekretariat;
    private System.Windows.Forms.CheckBox checkBoxIsSchulleitung;
    private System.Windows.Forms.CheckBox checkBoxIsAdmin;
  }
}