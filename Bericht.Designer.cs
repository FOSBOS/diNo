namespace diNo
{
    partial class Bericht
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
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.diNoDataSet = new diNo.diNoDataSet();
            this.LehrerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.LehrerTableAdapter = new diNo.diNoDataSetTableAdapters.LehrerTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LehrerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this.LehrerBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "diNo.rptLehrerliste.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(284, 262);
            this.reportViewer1.TabIndex = 0;
            // 
            // diNoDataSet
            // 
            this.diNoDataSet.DataSetName = "diNoDataSet";
            this.diNoDataSet.EnforceConstraints = false;
            this.diNoDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // LehrerBindingSource
            // 
            this.LehrerBindingSource.DataMember = "Lehrer";
            this.LehrerBindingSource.DataSource = this.diNoDataSet;
            // 
            // LehrerTableAdapter
            // 
            this.LehrerTableAdapter.ClearBeforeFill = true;
            // 
            // Bericht
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.reportViewer1);
            this.Name = "Bericht";
            this.Text = "Bericht";
            this.Load += new System.EventHandler(this.Bericht_Load);
            ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LehrerBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource LehrerBindingSource;
        private diNoDataSet diNoDataSet;
        private diNoDataSetTableAdapters.LehrerTableAdapter LehrerTableAdapter;
    }
}