﻿namespace diNo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotenCheckForm));
            this.comboBoxZeitpunkt = new System.Windows.Forms.ComboBox();
            this.btnUnterpunktungen = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.progressBarChecks = new System.Windows.Forms.ProgressBar();
            this.lbStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxZeitpunkt
            // 
            this.comboBoxZeitpunkt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxZeitpunkt.FormattingEnabled = true;
            this.comboBoxZeitpunkt.Items.AddRange(new object[] {
            "Probezeit BOS",
            "Halbjahr",
            "1. PA",
            "2. PA",
            "Jahresende"});
            this.comboBoxZeitpunkt.Location = new System.Drawing.Point(25, 37);
            this.comboBoxZeitpunkt.Name = "comboBoxZeitpunkt";
            this.comboBoxZeitpunkt.Size = new System.Drawing.Size(218, 24);
            this.comboBoxZeitpunkt.TabIndex = 12;
            // 
            // btnUnterpunktungen
            // 
            this.btnUnterpunktungen.ImageKey = "Ok.png";
            this.btnUnterpunktungen.ImageList = this.imageList1;
            this.btnUnterpunktungen.Location = new System.Drawing.Point(262, 28);
            this.btnUnterpunktungen.Name = "btnUnterpunktungen";
            this.btnUnterpunktungen.Size = new System.Drawing.Size(40, 40);
            this.btnUnterpunktungen.TabIndex = 13;
            this.btnUnterpunktungen.UseVisualStyleBackColor = true;
            this.btnUnterpunktungen.Click += new System.EventHandler(this.btnUnterpunktungen_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Ok.png");
            this.imageList1.Images.SetKeyName(1, "Cancel.png");
            // 
            // progressBarChecks
            // 
            this.progressBarChecks.Location = new System.Drawing.Point(25, 133);
            this.progressBarChecks.Name = "progressBarChecks";
            this.progressBarChecks.Size = new System.Drawing.Size(277, 20);
            this.progressBarChecks.TabIndex = 14;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(26, 105);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(37, 13);
            this.lbStatus.TabIndex = 15;
            this.lbStatus.Text = "Status";
            // 
            // NotenCheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 222);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.progressBarChecks);
            this.Controls.Add(this.btnUnterpunktungen);
            this.Controls.Add(this.comboBoxZeitpunkt);
            this.Name = "NotenCheckForm";
            this.Text = "Notenprüfung durchführen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxZeitpunkt;
        private System.Windows.Forms.Button btnUnterpunktungen;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ProgressBar progressBarChecks;
        private System.Windows.Forms.Label lbStatus;
    }
}