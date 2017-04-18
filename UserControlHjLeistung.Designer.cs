namespace diNo
{
  partial class UserControlHjLeistung
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridHjLeistung = new System.Windows.Forms.DataGridView();
      this.cFach = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cVorHj1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cVorHj2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cHj1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cHj2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridHjLeistung)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridHjLeistung
      // 
      this.dataGridHjLeistung.AllowUserToAddRows = false;
      this.dataGridHjLeistung.AllowUserToDeleteRows = false;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridHjLeistung.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dataGridHjLeistung.ColumnHeadersHeight = 30;
      this.dataGridHjLeistung.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dataGridHjLeistung.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cFach,
            this.cVorHj1,
            this.cVorHj2,
            this.cHj1,
            this.cHj2});
      this.dataGridHjLeistung.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridHjLeistung.Location = new System.Drawing.Point(0, 0);
      this.dataGridHjLeistung.Name = "dataGridHjLeistung";
      this.dataGridHjLeistung.ReadOnly = true;
      this.dataGridHjLeistung.RowHeadersVisible = false;
      this.dataGridHjLeistung.RowHeadersWidth = 70;
      this.dataGridHjLeistung.Size = new System.Drawing.Size(941, 562);
      this.dataGridHjLeistung.TabIndex = 0;
      // 
      // cFach
      // 
      this.cFach.DividerWidth = 10;
      this.cFach.Frozen = true;
      this.cFach.HeaderText = "Fach";
      this.cFach.Name = "cFach";
      this.cFach.ReadOnly = true;
      // 
      // cVorHj1
      // 
      this.cVorHj1.Frozen = true;
      this.cVorHj1.HeaderText = "11/1";
      this.cVorHj1.Name = "cVorHj1";
      this.cVorHj1.ReadOnly = true;
      this.cVorHj1.Width = 50;
      // 
      // cVorHj2
      // 
      this.cVorHj2.DividerWidth = 5;
      this.cVorHj2.Frozen = true;
      this.cVorHj2.HeaderText = "11/2";
      this.cVorHj2.Name = "cVorHj2";
      this.cVorHj2.ReadOnly = true;
      this.cVorHj2.Width = 55;
      // 
      // cHj1
      // 
      this.cHj1.Frozen = true;
      this.cHj1.HeaderText = "12/1";
      this.cHj1.Name = "cHj1";
      this.cHj1.ReadOnly = true;
      this.cHj1.Width = 50;
      // 
      // cHj2
      // 
      this.cHj2.DividerWidth = 5;
      this.cHj2.Frozen = true;
      this.cHj2.HeaderText = "12/2";
      this.cHj2.Name = "cHj2";
      this.cHj2.ReadOnly = true;
      this.cHj2.Width = 55;
      // 
      // UserControlHjLeistung
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dataGridHjLeistung);
      this.Name = "UserControlHjLeistung";
      this.Size = new System.Drawing.Size(941, 562);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridHjLeistung)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridHjLeistung;
    private System.Windows.Forms.DataGridViewTextBoxColumn cFach;
    private System.Windows.Forms.DataGridViewTextBoxColumn cVorHj1;
    private System.Windows.Forms.DataGridViewTextBoxColumn cVorHj2;
    private System.Windows.Forms.DataGridViewTextBoxColumn cHj1;
    private System.Windows.Forms.DataGridViewTextBoxColumn cHj2;
  }
}
