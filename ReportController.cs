using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;
using Microsoft.Reporting.WinForms;
using log4net;
using System.Data;
using System.Collections;

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
            rpt.reportViewer.SetDisplayMode( DisplayMode.PrintLayout ); // Darstellung sofort im Seitenlayout
            rpt.reportViewer.ZoomMode = ZoomMode.Percent;
            rpt.reportViewer.ZoomPercent = 100;
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
    public ReportNotenbogen(Schueler dataSource) : base(dataSource) {}
    //public ReportNotenbogen(IList<int> dataSource) : base(dataSource) {}
    public ReportNotenbogen(ArrayList dataSource) : base(dataSource) {}
        
    public override void Init()
    {    
      vwNotenbogenTableAdapter BerichtTableAdapter;
      rpt.BerichtBindingSource.DataMember = "vwNotenbogen";
      BerichtTableAdapter =  new vwNotenbogenTableAdapter();            
      BerichtTableAdapter.ClearBeforeFill = true;

      if (bindingDataSource is Schueler)      
        BerichtTableAdapter.FillBySchuelerId(rpt.diNoDataSet.vwNotenbogen,((Schueler)bindingDataSource).Id);                                 
      else
      { // Klassenweise (auch mehrere) drucken
        foreach (var klasse in (ArrayList)bindingDataSource)
        {
          BerichtTableAdapter.FillByKlasseId(rpt.diNoDataSet.vwNotenbogen,((Klasse)klasse).Data.Id);
          BerichtTableAdapter.ClearBeforeFill = false;
        }
      }

      rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenbogen.rdlc";

      // Unterberichte einbinden
      rpt.reportViewer.LocalReport.SubreportProcessing +=
              new SubreportProcessingEventHandler(subrptNotenbogenEventHandler);
      rpt.reportViewer.LocalReport.SubreportProcessing +=
              new SubreportProcessingEventHandler(subrptVorkommnisEventHandler);
    }

    void subrptNotenbogenEventHandler(object sender, SubreportProcessingEventArgs e)
    {
        // ACHTUNG: Der Parameter muss im Haupt- und im Unterbericht definiert werden (mit gleichem Namen)
        int schuelerId;
        int.TryParse(e.Parameters[0].Values[0],out schuelerId);
        if (schuelerId>0) // hier kommt er 2x rein.
        {
          Schueler schueler = new Schueler(schuelerId);
          var noten = schueler.getNoten.SchuelerNotenDruck();
          e.DataSources.Add(new ReportDataSource("DataSetFachSchuelerNoten",noten));
        }
    }

    void subrptVorkommnisEventHandler(object sender, SubreportProcessingEventArgs e)
    {
        diNoDataSet.vwVorkommnisDataTable vorkommnisse = new diNoDataSet.vwVorkommnisDataTable();
        vwVorkommnisTableAdapter BerichtTableAdapter;
        BerichtTableAdapter = new vwVorkommnisTableAdapter();
        BerichtTableAdapter.ClearBeforeFill = true;

        int schuelerId;
        int.TryParse(e.Parameters[0].Values[0],out schuelerId);
        if (schuelerId>0)
        {     
          BerichtTableAdapter.FillBySchuelerId(vorkommnisse, schuelerId);                 
          e.DataSources.Add(new ReportDataSource("DataSetVorkommnis",(DataTable) vorkommnisse));
        }
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
