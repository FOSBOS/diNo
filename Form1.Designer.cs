namespace diNo
{
	partial class Form1
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
      this.btnReadLehrer = new System.Windows.Forms.Button();
      this.btnReadExcelFile = new System.Windows.Forms.Button();
      this.btnCreateExcel = new System.Windows.Forms.Button();
      this.btnReadExcelKurse = new System.Windows.Forms.Button();
      this.btnImportSchueler = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnReadLehrer
      // 
      this.btnReadLehrer.Location = new System.Drawing.Point(12, 12);
      this.btnReadLehrer.Name = "btnReadLehrer";
      this.btnReadLehrer.Size = new System.Drawing.Size(132, 23);
      this.btnReadLehrer.TabIndex = 0;
      this.btnReadLehrer.Text = "Lehrer einlesen";
      this.btnReadLehrer.UseVisualStyleBackColor = true;
      this.btnReadLehrer.Click += new System.EventHandler(this.btnReadLehrer_Click);
      // 
      // btnReadExcelFile
      // 
      this.btnReadExcelFile.Location = new System.Drawing.Point(209, 12);
      this.btnReadExcelFile.Name = "btnReadExcelFile";
      this.btnReadExcelFile.Size = new System.Drawing.Size(97, 23);
      this.btnReadExcelFile.TabIndex = 2;
      this.btnReadExcelFile.Text = "Notendatei lesen";
      this.btnReadExcelFile.UseVisualStyleBackColor = true;
      this.btnReadExcelFile.Click += new System.EventHandler(this.btnReadExcelFile_Click);
      // 
      // btnCreateExcel
      // 
      this.btnCreateExcel.Location = new System.Drawing.Point(12, 114);
      this.btnCreateExcel.Name = "btnCreateExcel";
      this.btnCreateExcel.Size = new System.Drawing.Size(132, 23);
      this.btnCreateExcel.TabIndex = 3;
      this.btnCreateExcel.Text = "Excel-Dateien erzeugen";
      this.btnCreateExcel.UseVisualStyleBackColor = true;
      this.btnCreateExcel.Click += new System.EventHandler(this.btnCreateExcels_Click);
      // 
      // btnReadExcelKurse
      // 
      this.btnReadExcelKurse.Location = new System.Drawing.Point(12, 70);
      this.btnReadExcelKurse.Name = "btnReadExcelKurse";
      this.btnReadExcelKurse.Size = new System.Drawing.Size(132, 38);
      this.btnReadExcelKurse.TabIndex = 4;
      this.btnReadExcelKurse.Text = "Kursplan Excel einlesen und Schueler zuordnen";
      this.btnReadExcelKurse.UseVisualStyleBackColor = true;
      this.btnReadExcelKurse.Click += new System.EventHandler(this.btnReadExcelKurse_Click);
      // 
      // btnImportSchueler
      // 
      this.btnImportSchueler.Location = new System.Drawing.Point(12, 41);
      this.btnImportSchueler.Name = "btnImportSchueler";
      this.btnImportSchueler.Size = new System.Drawing.Size(132, 23);
      this.btnImportSchueler.TabIndex = 5;
      this.btnImportSchueler.Text = "WinSV Schülerdaten importieren";
      this.btnImportSchueler.UseVisualStyleBackColor = true;
      this.btnImportSchueler.Click += new System.EventHandler(this.btnImportSchueler_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(440, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(96, 23);
      this.button1.TabIndex = 6;
      this.button1.Text = "Klassenansicht";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(209, 70);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(96, 23);
      this.button3.TabIndex = 8;
      this.button3.Text = "Noten prüfen";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(726, 262);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.btnImportSchueler);
      this.Controls.Add(this.btnReadExcelKurse);
      this.Controls.Add(this.btnCreateExcel);
      this.Controls.Add(this.btnReadExcelFile);
      this.Controls.Add(this.btnReadLehrer);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

		}

		#endregion

    private System.Windows.Forms.Button btnReadLehrer;
    private System.Windows.Forms.Button btnReadExcelFile;
    private System.Windows.Forms.Button btnCreateExcel;
    private System.Windows.Forms.Button btnReadExcelKurse;
    private System.Windows.Forms.Button btnImportSchueler;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button3;
	}
}

