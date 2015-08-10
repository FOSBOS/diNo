namespace diNo
{
    partial class NotenbogenBericht
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
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.diNoDataSet = new diNo.diNoDataSet();
            this.SchuelerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.SchuelerTableAdapter = new diNo.diNoDataSetTableAdapters.SchuelerTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SchuelerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer
            // 
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this.SchuelerBindingSource;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.Notenbogen.rdlc";
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
            // SchuelerBindingSource
            // 
            this.SchuelerBindingSource.DataMember = "Schueler";
            this.SchuelerBindingSource.DataSource = this.diNoDataSet;
            // 
            // SchuelerTableAdapter
            // 
            this.SchuelerTableAdapter.ClearBeforeFill = true;
            // 
            // NotenbogenBericht
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.reportViewer);
            this.Name = "NotenbogenBericht";
            this.Text = "NotenbogenBericht";
            this.Load += new System.EventHandler(this.NotenbogenBericht_Load);
            ((System.ComponentModel.ISupportInitialize)(this.diNoDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SchuelerBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.BindingSource SchuelerBindingSource;
        private diNoDataSet diNoDataSet;
        private diNoDataSetTableAdapters.SchuelerTableAdapter SchuelerTableAdapter;
    }
}