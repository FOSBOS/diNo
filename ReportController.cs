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
        int schuelerId=0;

        public ReportNotenbogen(Schueler dataSource) : base(dataSource) {}

        public override void Init()
        {    
            schuelerId = ((Schueler)bindingDataSource).Id;
            vwNotenbogenTableAdapter ta =  new vwNotenbogenTableAdapter();
            rpt.BerichtBindingSource.DataMember = "vwNotenbogen";
            ta.ClearBeforeFill = true;
            
            ta.FillBySchuelerId(rpt.diNoDataSet.vwNotenbogen,schuelerId);   
                              
            //ta.FillByKlasseId(rpt.diNoDataSet.vwNotenbogen,89);  klappt noch nicht
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenbogen.rdlc";
            // Unterbericht einbinden
            rpt.reportViewer.LocalReport.SubreportProcessing +=
                    new SubreportProcessingEventHandler(subrptNotenbogenEventHandler);
        }

        void subrptNotenbogenEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            //e.Parameters verwenden, um Fremdschlüssel abzugreifen (SchülerId)
            //warum klappt FK-Übergabe nicht?
            //log.Debug("im Subreport Notenbogen");
          
            //int.TryParse(e.Parameters[0].Values[0],out SchuelerId);
            Schueler schueler = new Schueler(schuelerId);

            var noten = schueler.getNoten.SchuelerNotenDruck();
            e.DataSources.Add(new ReportDataSource("DataSet1",noten));
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
            BerichtTableAdapter.FillByKlasse(rpt.diNoDataSet.Schueler, 89); // 12Wf 
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptSchuelerliste.rdlc";
        }

    }

    public class ReportBrief : ReportController
    {
        public ReportBrief(BriefDaten dataSource) : base(dataSource) {}
        public override void Init()
        {
            /*
            SchuelerTableAdapter BerichtTableAdapter;
            rpt.BerichtBindingSource.DataMember = "Schueler";
            BerichtTableAdapter = new SchuelerTableAdapter();
            BerichtTableAdapter.ClearBeforeFill = true;
            BerichtTableAdapter.FillByKlasse(rpt.diNoDataSet.Schueler, 89);
            */
            rpt.BerichtBindingSource.DataSource = (BriefDaten)bindingDataSource;
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptBrief.rdlc";
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
