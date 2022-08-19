namespace diNo
{
    partial class ReportForm
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
      Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
      this.BerichtBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.diNoDataSet = new diNo.diNoDataSet();
      this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
      ((System.ComponentModel.ISupportInitialize)(this.BerichtBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).BeginInit();
      this.SuspendLayout();
      // 
      // BerichtBindingSource
      // 
      this.BerichtBindingSource.DataSource = this.diNoDataSet;
      this.BerichtBindingSource.Position = 0;
      // 
      // diNoDataSet
      // 
      this.diNoDataSet.DataSetName = "diNoDataSet";
      this.diNoDataSet.EnforceConstraints = false;
      this.diNoDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
      // 
      // reportViewer
      // 
      this.reportViewer.AutoScroll = true;
      this.reportViewer.AutoSize = true;
      this.reportViewer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
      reportDataSource1.Name = "DataSet1";
      reportDataSource1.Value = this.BerichtBindingSource;
      this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
      this.reportViewer.Location = new System.Drawing.Point(0, 0);
      this.reportViewer.Name = "reportViewer";
      this.reportViewer.ServerReport.BearerToken = null;
      this.reportViewer.Size = new System.Drawing.Size(455, 280);
      this.reportViewer.TabIndex = 0;
      // 
      // ReportForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(455, 280);
      this.Controls.Add(this.reportViewer);
      this.Name = "ReportForm";
      this.Text = "Drucken...";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      ((System.ComponentModel.ISupportInitialize)(this.BerichtBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        public Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        public System.Windows.Forms.BindingSource BerichtBindingSource;
        public diNoDataSet diNoDataSet;
     // public diNoDataSetTableAdapters.LehrerTableAdapter BerichtTableAdapter;
    }
}