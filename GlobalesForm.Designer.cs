namespace diNo
{
  partial class GlobalesForm : BasisForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalesForm));
      this.liste = new System.Windows.Forms.ListBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.edWert = new System.Windows.Forms.TextBox();
      this.lbkurzbez = new System.Windows.Forms.Label();
      this.edBezeichnung = new System.Windows.Forms.TextBox();
      this.lblBez = new System.Windows.Forms.Label();
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
      // liste
      // 
      this.liste.DisplayMember = "Bezeichnung";
      this.liste.FormattingEnabled = true;
      this.liste.Location = new System.Drawing.Point(25, 25);
      this.liste.Name = "liste";
      this.liste.Size = new System.Drawing.Size(180, 303);
      this.liste.TabIndex = 0;
      this.liste.SelectedValueChanged += new System.EventHandler(this.liste_SelectedValueChanged);
      // 
      // btnSave
      // 
      this.btnSave.ImageIndex = 4;
      this.btnSave.ImageList = this.imageList1;
      this.btnSave.Location = new System.Drawing.Point(541, 127);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 3;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // edWert
      // 
      this.edWert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edWert.Location = new System.Drawing.Point(231, 83);
      this.edWert.Name = "edWert";
      this.edWert.Size = new System.Drawing.Size(350, 20);
      this.edWert.TabIndex = 2;
      // 
      // lbkurzbez
      // 
      this.lbkurzbez.AutoSize = true;
      this.lbkurzbez.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbkurzbez.Location = new System.Drawing.Point(228, 66);
      this.lbkurzbez.Name = "lbkurzbez";
      this.lbkurzbez.Size = new System.Drawing.Size(30, 13);
      this.lbkurzbez.TabIndex = 28;
      this.lbkurzbez.Text = "Wert";
      // 
      // edBezeichnung
      // 
      this.edBezeichnung.Enabled = false;
      this.edBezeichnung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.edBezeichnung.Location = new System.Drawing.Point(231, 43);
      this.edBezeichnung.Name = "edBezeichnung";
      this.edBezeichnung.Size = new System.Drawing.Size(350, 20);
      this.edBezeichnung.TabIndex = 1;
      // 
      // lblBez
      // 
      this.lblBez.AutoSize = true;
      this.lblBez.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblBez.Location = new System.Drawing.Point(228, 25);
      this.lblBez.Name = "lblBez";
      this.lblBez.Size = new System.Drawing.Size(69, 13);
      this.lblBez.TabIndex = 27;
      this.lblBez.Text = "Bezeichnung";
      // 
      // GlobalesForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(610, 351);
      this.Controls.Add(this.edWert);
      this.Controls.Add(this.lbkurzbez);
      this.Controls.Add(this.edBezeichnung);
      this.Controls.Add(this.lblBez);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.liste);
      this.Name = "GlobalesForm";
      this.Text = "Globale Texte";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.ListBox liste;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TextBox edWert;
    private System.Windows.Forms.Label lbkurzbez;
    private System.Windows.Forms.TextBox edBezeichnung;
    private System.Windows.Forms.Label lblBez;
  }
}