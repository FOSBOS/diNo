using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;
using Microsoft.Reporting.WinForms;

namespace diNo
{
  
    public abstract class ReportController    
    {
        protected ReportForm rpt;
        protected Object bindingDataSource;
        
        public ReportController(Object dataSource = null)
        {
            rpt = new ReportForm();          
            bindingDataSource = dataSource;
            Init();
            rpt.reportViewer.RefreshReport();
            rpt.Show();           
        }
        
        public abstract void Init(); 
    }

    public class ReportNotencheck : ReportController
    {
        public ReportNotencheck(NotenCheckResults dataSource) : base(dataSource) {}

        public override void Init()
        {            
            rpt.BerichtBindingSource.DataSource = ((NotenCheckResults)bindingDataSource).list;
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenCheck.rdlc";            
        }

    }

    public class ReportNotenbogen : ReportController
    {
        public ReportNotenbogen(NotenCheckResults dataSource) : base(dataSource) {}

        public override void Init()
        {    
        
              
            IList<Schueler>liste = new List<Schueler>();
            liste.Add(new Schueler(8500));
            liste.Add(new Schueler(8534));
            
            ReportDataSource tSource = new ReportDataSource("DataSetNotenbogen", liste);
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenbogen.rdlc";
            rpt.reportViewer.LocalReport.DataSources.Add(tSource);
            

            // rpt.BerichtBindingSource.DataSource = liste;
            // rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenbogen.rdlc";            
        }

    }

    public class ReportFachliste : ReportController
    {
        public override void Init()
        {
            FachTableAdapter BerichtTableAdapter;
            rpt.BerichtBindingSource.DataMember = "Fach";
            BerichtTableAdapter = new FachTableAdapter();
            BerichtTableAdapter.ClearBeforeFill = true;
            BerichtTableAdapter.Fill(rpt.diNoDataSet.Fach);
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptFachliste.rdlc";     
        }
    }
    
    public class ReportSchuelerliste : ReportController
    {
        public override void Init()
        {
            SchuelerTableAdapter BerichtTableAdapter;
            rpt.BerichtBindingSource.DataMember = "Schueler";
            BerichtTableAdapter = new SchuelerTableAdapter();
            BerichtTableAdapter.ClearBeforeFill = true;
            BerichtTableAdapter.FillByKlasse(rpt.diNoDataSet.Schueler, 34); // 11Tb 
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptSchuelerliste.rdlc";
        }
    }

   
    public class ReportLehrerliste : ReportController
    {
        public override void Init()
        {
                        LehrerTableAdapter BerichtTableAdapter;

                        rpt.BerichtBindingSource.DataMember = "Lehrer";
                        BerichtTableAdapter = new LehrerTableAdapter();
                        BerichtTableAdapter.ClearBeforeFill = true;
                        BerichtTableAdapter.Fill(rpt.diNoDataSet.Lehrer);
                        rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptLehrerliste.rdlc";
        }
    }
}
