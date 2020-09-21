namespace diNo
{
  partial class KlasseForm : BasisForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KlasseForm));
      this.btnSave = new System.Windows.Forms.Button();
      this.btnDel = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.liste = new System.Windows.Forms.ListBox();
      this.edBezeichnung = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.edJgStufe = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.cbSchulart = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.edZweig = new System.Windows.Forms.TextBox();
      this.cbKlassenleiter = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
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
      // btnSave
      // 
      this.btnSave.ImageIndex = 4;
      this.btnSave.ImageList = this.imageList1;
      this.btnSave.Location = new System.Drawing.Point(418, 457);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 24;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnDel
      // 
      this.btnDel.ImageIndex = 7;
      this.btnDel.ImageList = this.imageList1;
      this.btnDel.Location = new System.Drawing.Point(71, 503);
      this.btnDel.Name = "btnDel";
      this.btnDel.Size = new System.Drawing.Size(40, 40);
      this.btnDel.TabIndex = 23;
      this.btnDel.UseVisualStyleBackColor = true;
      this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
      // 
      // btnAdd
      // 
      this.btnAdd.Enabled = false;
      this.btnAdd.ImageIndex = 6;
      this.btnAdd.ImageList = this.imageList1;
      this.btnAdd.Location = new System.Drawing.Point(25, 503);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(40, 40);
      this.btnAdd.TabIndex = 22;
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // liste
      // 
      this.liste.FormattingEnabled = true;
      this.liste.Location = new System.Drawing.Point(25, 25);
      this.liste.Name = "liste";
      this.liste.Size = new System.Drawing.Size(180, 472);
      this.liste.TabIndex = 21;
      this.liste.SelectedValueChanged += new System.EventHandler(this.liste_SelectedValueChanged);
      // 
      // edBezeichnung
      // 
      this.edBezeichnung.Location = new System.Drawing.Point(304, 56);
      this.edBezeichnung.Name = "edBezeichnung";
      this.edBezeichnung.Size = new System.Drawing.Size(154, 20);
      this.edBezeichnung.TabIndex = 25;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(301, 40);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(69, 13);
      this.label1.TabIndex = 26;
      this.label1.Text = "Bezeichnung";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(301, 100);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(79, 13);
      this.label2.TabIndex = 28;
      this.label2.Text = "Jahrgangsstufe";
      // 
      // edJgStufe
      // 
      this.edJgStufe.Location = new System.Drawing.Point(304, 116);
      this.edJgStufe.Name = "edJgStufe";
      this.edJgStufe.Size = new System.Drawing.Size(154, 20);
      this.edJgStufe.TabIndex = 27;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(301, 146);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(46, 13);
      this.label3.TabIndex = 30;
      this.label3.Text = "Schulart";
      // 
      // cbSchulart
      // 
      this.cbSchulart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbSchulart.FormattingEnabled = true;
      this.cbSchulart.Items.AddRange(new object[] {
            "",
            "FOS",
            "BOS"});
      this.cbSchulart.Location = new System.Drawing.Point(304, 162);
      this.cbSchulart.Name = "cbSchulart";
      this.cbSchulart.Size = new System.Drawing.Size(154, 21);
      this.cbSchulart.TabIndex = 31;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(301, 198);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(36, 13);
      this.label4.TabIndex = 33;
      this.label4.Text = "Zweig";
      // 
      // edZweig
      // 
      this.edZweig.Location = new System.Drawing.Point(304, 214);
      this.edZweig.Name = "edZweig";
      this.edZweig.Size = new System.Drawing.Size(154, 20);
      this.edZweig.TabIndex = 32;
      // 
      // cbKlassenleiter
      // 
      this.cbKlassenleiter.DisplayMember = "KompletterName";
      this.cbKlassenleiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbKlassenleiter.FormattingEnabled = true;
      this.cbKlassenleiter.Items.AddRange(new object[] {
            "",
            "FOS",
            "BOS"});
      this.cbKlassenleiter.Location = new System.Drawing.Point(304, 273);
      this.cbKlassenleiter.Name = "cbKlassenleiter";
      this.cbKlassenleiter.Size = new System.Drawing.Size(154, 21);
      this.cbKlassenleiter.TabIndex = 35;
      this.cbKlassenleiter.ValueMember = "Id";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(301, 257);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(66, 13);
      this.label5.TabIndex = 34;
      this.label5.Text = "Klassenleiter";
      // 
      // KlasseForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(516, 550);
      this.Controls.Add(this.cbKlassenleiter);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.edZweig);
      this.Controls.Add(this.cbSchulart);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.edJgStufe);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.edBezeichnung);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnDel);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.liste);
      this.Name = "KlasseForm";
      this.Text = "Klassen";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.ListBox liste;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnDel;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TextBox edBezeichnung;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox edJgStufe;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox cbSchulart;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox edZweig;
    private System.Windows.Forms.ComboBox cbKlassenleiter;
    private System.Windows.Forms.Label label5;
  }
}