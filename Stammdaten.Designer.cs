namespace diNo
{
  partial class Stammdaten : BasisForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stammdaten));
      this.liste = new System.Windows.Forms.ListBox();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnDel = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
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
      this.liste.FormattingEnabled = true;
      this.liste.Location = new System.Drawing.Point(25, 25);
      this.liste.Name = "liste";
      this.liste.Size = new System.Drawing.Size(180, 472);
      this.liste.TabIndex = 21;
      this.liste.SelectedValueChanged += new System.EventHandler(this.liste_SelectedValueChanged);
      // 
      // btnAdd
      // 
      this.btnAdd.ImageIndex = 6;
      this.btnAdd.ImageList = this.imageList1;
      this.btnAdd.Location = new System.Drawing.Point(25, 503);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(40, 40);
      this.btnAdd.TabIndex = 22;
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
      // btnSave
      // 
      this.btnSave.ImageIndex = 4;
      this.btnSave.ImageList = this.imageList1;
      this.btnSave.Location = new System.Drawing.Point(912, 503);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 24;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // Stammdaten
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(964, 550);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnDel);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.liste);
      this.Name = "Stammdaten";
      this.Text = "Stammdaten";
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ListBox liste;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnDel;
    private System.Windows.Forms.Button btnSave;
  }
}