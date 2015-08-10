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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.diNoDataSet = new diNo.diNoDataSet();
            this.BerichtBindingSource = new System.Windows.Forms.BindingSource(this.components);
         // this.BerichtTableAdapter = new diNo.diNoDataSetTableAdapters.LehrerTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BerichtBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer
            // 
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource2.Name = "DataSet1";
            reportDataSource2.Value = this.BerichtBindingSource;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource2);
        //  this.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptLehrerliste.rdlc";
            this.reportViewer.Location = new System.Drawing.Point(0, 0);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.Size = new System.Drawing.Size(284, 262);
            this.reportViewer.TabIndex = 0;
            // 
            // diNoDataSet
            // 
            this.diNoDataSet.DataSetName = "diNoDataSet";
            this.diNoDataSet.EnforceConstraints = false;
            this.diNoDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // LehrerBindingSource
            // 
        //  this.BerichtBindingSource.DataMember = "Lehrer";
            this.BerichtBindingSource.DataSource = this.diNoDataSet;
            // 
            // LehrerTableAdapter
            // 
         //   this.BerichtTableAdapter.ClearBeforeFill = true;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.reportViewer);
            this.Name = "ReportForm";
            this.Text = "Drucken...";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
         // this.Load += new System.EventHandler(this.ReportForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BerichtBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        public System.Windows.Forms.BindingSource BerichtBindingSource;
        public diNoDataSet diNoDataSet;
     // public diNoDataSetTableAdapters.LehrerTableAdapter BerichtTableAdapter;
    }
}