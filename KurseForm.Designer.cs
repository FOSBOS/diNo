namespace diNo
{
  partial class KurseForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KurseForm));
      this.edBezeichnung = new System.Windows.Forms.TextBox();
      this.lblBez = new System.Windows.Forms.Label();
      this.edKurzbez = new System.Windows.Forms.TextBox();
      this.lbkurzbez = new System.Windows.Forms.Label();
      this.liste = new System.Windows.Forms.ListBox();
      this.btnDel = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.edZweig = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.edId = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.gbGeschlecht = new System.Windows.Forms.GroupBox();
      this.opUndef = new System.Windows.Forms.RadioButton();
      this.opWeiblich = new System.Windows.Forms.RadioButton();
      this.opMaennlich = new System.Windows.Forms.RadioButton();
      this.label3 = new System.Windows.Forms.Label();
      this.cbLehrer = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.cbFach = new System.Windows.Forms.ComboBox();
      this.checkedListBoxKlassen = new System.Windows.Forms.CheckedListBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.btnErzeugeExcel = new System.Windows.Forms.Button();
      this.listSchueler = new System.Windows.Forms.ListBox();
      this.gbGeschlecht.SuspendLayout();
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
      // edBezeichnung
      // 
      this.edBezeichnung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edBezeichnung.Location = new System.Drawing.Point(319, 46);
      this.edBezeichnung.Name = "edBezeichnung";
      this.edBezeichnung.Size = new System.Drawing.Size(350, 20);
      this.edBezeichnung.TabIndex = 1;
      // 
      // lblBez
      // 
      this.lblBez.AutoSize = true;
      this.lblBez.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblBez.Location = new System.Drawing.Point(316, 28);
      this.lblBez.Name = "lblBez";
      this.lblBez.Size = new System.Drawing.Size(69, 13);
      this.lblBez.TabIndex = 9;
      this.lblBez.Text = "Bezeichnung";
      // 
      // edKurzbez
      // 
      this.edKurzbez.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edKurzbez.Location = new System.Drawing.Point(319, 86);
      this.edKurzbez.Name = "edKurzbez";
      this.edKurzbez.Size = new System.Drawing.Size(350, 20);
      this.edKurzbez.TabIndex = 2;
      // 
      // lbkurzbez
      // 
      this.lbkurzbez.AutoSize = true;
      this.lbkurzbez.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbkurzbez.Location = new System.Drawing.Point(316, 69);
      this.lbkurzbez.Name = "lbkurzbez";
      this.lbkurzbez.Size = new System.Drawing.Size(89, 13);
      this.lbkurzbez.TabIndex = 11;
      this.lbkurzbez.Text = "Kurzbezeichnung";
      // 
      // liste
      // 
      this.liste.DisplayMember = "Kursbezeichnung";
      this.liste.FormattingEnabled = true;
      this.liste.Location = new System.Drawing.Point(25, 27);
      this.liste.Name = "liste";
      this.liste.Size = new System.Drawing.Size(257, 420);
      this.liste.TabIndex = 0;
      this.liste.SelectedValueChanged += new System.EventHandler(this.listLehrer_SelectedValueChanged);
      // 
      // btnDel
      // 
      this.btnDel.ImageIndex = 7;
      this.btnDel.ImageList = this.imageList1;
      this.btnDel.Location = new System.Drawing.Point(71, 462);
      this.btnDel.Name = "btnDel";
      this.btnDel.Size = new System.Drawing.Size(40, 40);
      this.btnDel.TabIndex = 10;
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
      this.btnAdd.TabIndex = 9;
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnSave
      // 
      this.btnSave.ImageIndex = 4;
      this.btnSave.ImageList = this.imageList1;
      this.btnSave.Location = new System.Drawing.Point(319, 462);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 8;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // edZweig
      // 
      this.edZweig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edZweig.Location = new System.Drawing.Point(319, 236);
      this.edZweig.Name = "edZweig";
      this.edZweig.Size = new System.Drawing.Size(59, 20);
      this.edZweig.TabIndex = 5;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(316, 219);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(36, 13);
      this.label1.TabIndex = 28;
      this.label1.Text = "Zweig";
      // 
      // edId
      // 
      this.edId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edId.Location = new System.Drawing.Point(319, 279);
      this.edId.Name = "edId";
      this.edId.Size = new System.Drawing.Size(59, 20);
      this.edId.TabIndex = 6;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(316, 262);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(16, 13);
      this.label2.TabIndex = 30;
      this.label2.Text = "Id";
      // 
      // gbGeschlecht
      // 
      this.gbGeschlecht.Controls.Add(this.opUndef);
      this.gbGeschlecht.Controls.Add(this.opWeiblich);
      this.gbGeschlecht.Controls.Add(this.opMaennlich);
      this.gbGeschlecht.Location = new System.Drawing.Point(438, 219);
      this.gbGeschlecht.Name = "gbGeschlecht";
      this.gbGeschlecht.Size = new System.Drawing.Size(95, 80);
      this.gbGeschlecht.TabIndex = 7;
      this.gbGeschlecht.TabStop = false;
      this.gbGeschlecht.Text = "Geschlecht";
      // 
      // opUndef
      // 
      this.opUndef.AutoSize = true;
      this.opUndef.Checked = true;
      this.opUndef.Location = new System.Drawing.Point(10, 17);
      this.opUndef.Name = "opUndef";
      this.opUndef.Size = new System.Drawing.Size(78, 17);
      this.opUndef.TabIndex = 3;
      this.opUndef.TabStop = true;
      this.opUndef.Text = "koedukativ";
      this.opUndef.UseVisualStyleBackColor = true;
      // 
      // opWeiblich
      // 
      this.opWeiblich.AutoSize = true;
      this.opWeiblich.Location = new System.Drawing.Point(10, 51);
      this.opWeiblich.Name = "opWeiblich";
      this.opWeiblich.Size = new System.Drawing.Size(63, 17);
      this.opWeiblich.TabIndex = 2;
      this.opWeiblich.Text = "weiblich";
      this.opWeiblich.UseVisualStyleBackColor = true;
      // 
      // opMaennlich
      // 
      this.opMaennlich.AutoSize = true;
      this.opMaennlich.Location = new System.Drawing.Point(10, 34);
      this.opMaennlich.Name = "opMaennlich";
      this.opMaennlich.Size = new System.Drawing.Size(67, 17);
      this.opMaennlich.TabIndex = 1;
      this.opMaennlich.Text = "männlich";
      this.opMaennlich.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(316, 125);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(37, 13);
      this.label3.TabIndex = 33;
      this.label3.Text = "Lehrer";
      // 
      // cbLehrer
      // 
      this.cbLehrer.DisplayMember = "KompletterName";
      this.cbLehrer.FormattingEnabled = true;
      this.cbLehrer.Location = new System.Drawing.Point(319, 141);
      this.cbLehrer.Name = "cbLehrer";
      this.cbLehrer.Size = new System.Drawing.Size(214, 21);
      this.cbLehrer.TabIndex = 3;
      this.cbLehrer.ValueMember = "Id";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(316, 167);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(31, 13);
      this.label4.TabIndex = 35;
      this.label4.Text = "Fach";
      // 
      // cbFach
      // 
      this.cbFach.DisplayMember = "Bezeichnung";
      this.cbFach.FormattingEnabled = true;
      this.cbFach.Location = new System.Drawing.Point(319, 183);
      this.cbFach.Name = "cbFach";
      this.cbFach.Size = new System.Drawing.Size(214, 21);
      this.cbFach.TabIndex = 4;
      this.cbFach.ValueMember = "Id";
      // 
      // checkedListBoxKlassen
      // 
      this.checkedListBoxKlassen.CheckOnClick = true;
      this.checkedListBoxKlassen.FormattingEnabled = true;
      this.checkedListBoxKlassen.Location = new System.Drawing.Point(319, 316);
      this.checkedListBoxKlassen.Name = "checkedListBoxKlassen";
      this.checkedListBoxKlassen.Size = new System.Drawing.Size(214, 124);
      this.checkedListBoxKlassen.TabIndex = 36;
      // 
      // textBox1
      // 
      this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.textBox1.Location = new System.Drawing.Point(553, 316);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(154, 124);
      this.textBox1.TabIndex = 38;
      this.textBox1.Text = "Wenn die Klassenzuordnung geändert wird, werden die Schüler nicht automatisch in " +
    "diesen Kurse angemeldet. Verwende dazu den Befehl unter Administration/Reparatur" +
    "en.";
      // 
      // btnErzeugeExcel
      // 
      this.btnErzeugeExcel.ImageIndex = 5;
      this.btnErzeugeExcel.ImageList = this.imageList1;
      this.btnErzeugeExcel.Location = new System.Drawing.Point(377, 462);
      this.btnErzeugeExcel.Name = "btnErzeugeExcel";
      this.btnErzeugeExcel.Size = new System.Drawing.Size(40, 40);
      this.btnErzeugeExcel.TabIndex = 39;
      this.toolTipButtons.SetToolTip(this.btnErzeugeExcel, "Erzeuge Excel-Datei für diesen Kurs");
      this.btnErzeugeExcel.UseVisualStyleBackColor = true;
      this.btnErzeugeExcel.Click += new System.EventHandler(this.btnErzeugeExcel_Click);
      // 
      // listSchueler
      // 
      this.listSchueler.DisplayMember = "KlasseName";
      this.listSchueler.FormattingEnabled = true;
      this.listSchueler.Location = new System.Drawing.Point(730, 27);
      this.listSchueler.Name = "listSchueler";
      this.listSchueler.Size = new System.Drawing.Size(243, 420);
      this.listSchueler.TabIndex = 40;
      // 
      // KurseForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1015, 524);
      this.Controls.Add(this.listSchueler);
      this.Controls.Add(this.btnErzeugeExcel);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.checkedListBoxKlassen);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.cbFach);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.cbLehrer);
      this.Controls.Add(this.gbGeschlecht);
      this.Controls.Add(this.edId);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.edZweig);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnDel);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.liste);
      this.Controls.Add(this.edKurzbez);
      this.Controls.Add(this.lbkurzbez);
      this.Controls.Add(this.edBezeichnung);
      this.Controls.Add(this.lblBez);
      this.Name = "KurseForm";
      this.Text = "Kurse";
      this.gbGeschlecht.ResumeLayout(false);
      this.gbGeschlecht.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.TextBox edBezeichnung;
    private System.Windows.Forms.Label lblBez;
    private System.Windows.Forms.TextBox edKurzbez;
    private System.Windows.Forms.Label lbkurzbez;
    private System.Windows.Forms.ListBox liste;
    private System.Windows.Forms.Button btnDel;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TextBox edZweig;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edId;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox gbGeschlecht;
    private System.Windows.Forms.RadioButton opUndef;
    private System.Windows.Forms.RadioButton opWeiblich;
    private System.Windows.Forms.RadioButton opMaennlich;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox cbLehrer;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox cbFach;
    private System.Windows.Forms.CheckedListBox checkedListBoxKlassen;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button btnErzeugeExcel;
    private System.Windows.Forms.ListBox listSchueler;
  }
}