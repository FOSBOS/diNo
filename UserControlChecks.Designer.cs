namespace diNo
{
  partial class UserControlChecks
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
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
        if (memoryImage != null)
        {
          memoryImage.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.KlasseLabel = new System.Windows.Forms.Label();
      this.DatumLabel = new System.Windows.Forms.Label();
      this.textBoxKlasse = new System.Windows.Forms.TextBox();
      this.textBoxDatum = new System.Windows.Forms.TextBox();
      this.listViewChecks = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.SuspendLayout();
      // 
      // KlasseLabel
      // 
      this.KlasseLabel.AutoSize = true;
      this.KlasseLabel.Location = new System.Drawing.Point(14, 11);
      this.KlasseLabel.Name = "KlasseLabel";
      this.KlasseLabel.Size = new System.Drawing.Size(134, 13);
      this.KlasseLabel.TabIndex = 0;
      this.KlasseLabel.Text = "erfolgte Prüfungen Klasse: ";
      // 
      // DatumLabel
      // 
      this.DatumLabel.AutoSize = true;
      this.DatumLabel.Location = new System.Drawing.Point(14, 35);
      this.DatumLabel.Name = "DatumLabel";
      this.DatumLabel.Size = new System.Drawing.Size(44, 13);
      this.DatumLabel.TabIndex = 1;
      this.DatumLabel.Text = "Datum: ";
      // 
      // textBoxKlasse
      // 
      this.textBoxKlasse.Location = new System.Drawing.Point(154, 8);
      this.textBoxKlasse.Name = "textBoxKlasse";
      this.textBoxKlasse.Size = new System.Drawing.Size(100, 20);
      this.textBoxKlasse.TabIndex = 2;
      // 
      // textBoxDatum
      // 
      this.textBoxDatum.Location = new System.Drawing.Point(154, 35);
      this.textBoxDatum.Name = "textBoxDatum";
      this.textBoxDatum.Size = new System.Drawing.Size(100, 20);
      this.textBoxDatum.TabIndex = 3;
      // 
      // listViewChecks
      // 
      this.listViewChecks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.listViewChecks.Location = new System.Drawing.Point(17, 65);
      this.listViewChecks.Name = "listViewChecks";
      this.listViewChecks.Size = new System.Drawing.Size(759, 1006);
      this.listViewChecks.TabIndex = 4;
      this.listViewChecks.UseCompatibleStateImageBehavior = false;
      this.listViewChecks.View = System.Windows.Forms.View.List;
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "Meldung";
      this.columnHeader1.Width = 807;
      // 
      // UserControlChecks
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.listViewChecks);
      this.Controls.Add(this.textBoxDatum);
      this.Controls.Add(this.textBoxKlasse);
      this.Controls.Add(this.DatumLabel);
      this.Controls.Add(this.KlasseLabel);
      this.Name = "UserControlChecks";
      this.Size = new System.Drawing.Size(827, 1169);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label KlasseLabel;
    private System.Windows.Forms.Label DatumLabel;
    private System.Windows.Forms.TextBox textBoxKlasse;
    private System.Windows.Forms.TextBox textBoxDatum;
    private System.Windows.Forms.ListView listViewChecks;
    private System.Windows.Forms.ColumnHeader columnHeader1;
  }
}
