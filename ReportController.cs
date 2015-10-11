using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;
using Microsoft.Reporting.WinForms;
using log4net;
using System.Data;

namespace diNo
{

    public abstract class ReportController    
    {
        protected static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
        
              
            IList<diNoDataSet.SchuelerRow>sListe = new List<diNoDataSet.SchuelerRow>();
            IList<Klasse>kListe = new List<Klasse>();
            /*var s =new Schueler(8500);
            kListe.Add(s.getKlasse);
            sListe.Add(s.Data);
            */
            var s =new Schueler(8534);
            kListe.Add(s.getKlasse);
            sListe.Add(s.Data);                      
            //ReportDataSource tSource = new ReportDataSource("DataSetNotenbogen", liste);
            ReportDataSource tSource = new ReportDataSource("DataSetSchuelerDB", sListe);
            rpt.reportViewer.LocalReport.DataSources.Add(tSource);

            ReportDataSource kSource = new ReportDataSource("DataSetKlasse", kListe);
            rpt.reportViewer.LocalReport.DataSources.Add(kSource);
            
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenbogen.rdlc";

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
            // Unterbericht einbinden
            rpt.reportViewer.LocalReport.SubreportProcessing +=
                    new SubreportProcessingEventHandler(subrptKursEventHandler);
        }

        void subrptKursEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            diNoDataSet.KursDataTable kurse = new diNoDataSet.KursDataTable();
            KursTableAdapter BerichtTableAdapter;
            BerichtTableAdapter = new KursTableAdapter();
            BerichtTableAdapter.ClearBeforeFill = true;

            //e.Parameters verwenden, um Fremdschlüssel abzugreifen
            int LehrerId=0;
            int.TryParse(e.Parameters[0].Values[0],out LehrerId);
            BerichtTableAdapter.FillByLehrerId(kurse, LehrerId);                 
            e.DataSources.Add(new ReportDataSource("DataSetKurs",(DataTable) kurse));
        }
    }
}
