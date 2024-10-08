namespace diNo
{
    partial class NotenCheckForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotenCheckForm));
      this.comboBoxZeitpunkt = new System.Windows.Forms.ComboBox();
      this.btnStart = new System.Windows.Forms.Button();
      this.progressBarChecks = new System.Windows.Forms.ProgressBar();
      this.lbStatus = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBoxCheckModus = new System.Windows.Forms.ComboBox();
      this.chkKurzfassung = new System.Windows.Forms.CheckBox();
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
      // 
      // comboBoxZeitpunkt
      // 
      this.comboBoxZeitpunkt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxZeitpunkt.Enabled = false;
      this.comboBoxZeitpunkt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboBoxZeitpunkt.FormattingEnabled = true;
      this.comboBoxZeitpunkt.Items.AddRange(new object[] {
            "Probezeit BOS",
            "Halbjahr",
            "Zulassung Abitur",
            "SAP",
            "MAP",
            "Jahresende"});
      this.comboBoxZeitpunkt.Location = new System.Drawing.Point(25, 80);
      this.comboBoxZeitpunkt.Name = "comboBoxZeitpunkt";
      this.comboBoxZeitpunkt.Size = new System.Drawing.Size(218, 24);
      this.comboBoxZeitpunkt.TabIndex = 12;
      // 
      // btnStart
      // 
      this.btnStart.ImageIndex = 1;
      this.btnStart.ImageList = this.imageList1;
      this.btnStart.Location = new System.Drawing.Point(262, 71);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(40, 40);
      this.btnStart.TabIndex = 13;
      this.btnStart.UseVisualStyleBackColor = true;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // progressBarChecks
      // 
      this.progressBarChecks.Location = new System.Drawing.Point(25, 170);
      this.progressBarChecks.Name = "progressBarChecks";
      this.progressBarChecks.Size = new System.Drawing.Size(277, 20);
      this.progressBarChecks.TabIndex = 14;
      // 
      // lbStatus
      // 
      this.lbStatus.AutoSize = true;
      this.lbStatus.Location = new System.Drawing.Point(22, 153);
      this.lbStatus.Name = "lbStatus";
      this.lbStatus.Size = new System.Drawing.Size(37, 13);
      this.lbStatus.TabIndex = 15;
      this.lbStatus.Text = "Status";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(22, 64);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(79, 13);
      this.label1.TabIndex = 16;
      this.label1.Text = "Prüfungsanlass";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(22, 12);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(80, 13);
      this.label2.TabIndex = 18;
      this.label2.Text = "Prüfungsmodus";
      // 
      // comboBoxCheckModus
      // 
      this.comboBoxCheckModus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxCheckModus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboBoxCheckModus.FormattingEnabled = true;
      this.comboBoxCheckModus.Location = new System.Drawing.Point(25, 28);
      this.comboBoxCheckModus.Name = "comboBoxCheckModus";
      this.comboBoxCheckModus.Size = new System.Drawing.Size(218, 24);
      this.comboBoxCheckModus.TabIndex = 17;
      // 
      // chkKurzfassung
      // 
      this.chkKurzfassung.AutoSize = true;
      this.chkKurzfassung.Location = new System.Drawing.Point(25, 124);
      this.chkKurzfassung.Name = "chkKurzfassung";
      this.chkKurzfassung.Size = new System.Drawing.Size(149, 17);
      this.chkKurzfassung.TabIndex = 19;
      this.chkKurzfassung.Text = "Nur Problemfälle anzeigen";
      this.chkKurzfassung.UseVisualStyleBackColor = true;
      // 
      // NotenCheckForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(329, 222);
      this.Controls.Add(this.chkKurzfassung);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.comboBoxCheckModus);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.lbStatus);
      this.Controls.Add(this.progressBarChecks);
      this.Controls.Add(this.btnStart);
      this.Controls.Add(this.comboBoxZeitpunkt);
      this.Name = "NotenCheckForm";
      this.Text = "Notenprüfung durchführen";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxZeitpunkt;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ProgressBar progressBarChecks;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox comboBoxCheckModus;
        private System.Windows.Forms.CheckBox chkKurzfassung;
    }
}