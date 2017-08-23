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
        
        public ReportController()
        {
            rpt = new ReportForm();                                 
        }
        
        public abstract void Init();
     
        public void Show()
        {
          Init();
          rpt.reportViewer.RefreshReport();
          rpt.reportViewer.SetDisplayMode( DisplayMode.PrintLayout ); // Darstellung sofort im Seitenlayout
          rpt.reportViewer.ZoomMode = ZoomMode.Percent;
          rpt.reportViewer.ZoomPercent = 100;
          rpt.Show();                    
        }            
    }

    public class ReportNotencheck : ReportController
    {
        private NotenCheckResults bindingDataSource;
        public ReportNotencheck(NotenCheckResults dataSource) : base()
        {
          bindingDataSource = dataSource;
        }

        public override void Init()
        {            
            rpt.BerichtBindingSource.DataSource = bindingDataSource.list;
            rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenCheck.rdlc";            
        }
    }
  
  // kann diverse Notenberichte drucken, die Grunddaten sind jeweils eine Schülerliste  
  public class ReportNotendruck : ReportController
  {
    private List<SchuelerDruck> bindingDataSource;
    private string rptName;

    public ReportNotendruck(List<SchuelerDruck> dataSource, string Berichtsname) : base()
    {
      rptName = Berichtsname;
      bindingDataSource = dataSource;
    }
        
    public override void Init()
    {    
      rpt.BerichtBindingSource.DataSource = bindingDataSource;
      rpt.reportViewer.LocalReport.ReportEmbeddedResource = rptName;

      // Unterberichte einbinden
      rpt.reportViewer.LocalReport.SubreportProcessing +=
         new SubreportProcessingEventHandler(subrptEventHandler);
    }

    void subrptEventHandler(object sender, SubreportProcessingEventArgs e)
    {
        // ACHTUNG: Der Parameter muss im Haupt- und im Unterbericht definiert werden (mit gleichem Namen)
        string subrpt = e.ReportPath; // jeder Unterbericht ruft diesen EventHandler auf; hier steht drin welcher es ist.
        int schuelerId;
        int.TryParse(e.Parameters[0].Values[0],out schuelerId);
        if (schuelerId>0)
        {
          Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
          if (subrpt=="subrptFachSchuelerNoten" || subrpt=="subrptFachSchuelerNoten11Klasse" ||
              subrpt=="subrptAbiergebnisseNoten")
          {
            IList<FachSchuelerNotenDruckKurz> noten = schueler.getNoten.SchuelerNotenDruck(rptName);
            e.DataSources.Add(new ReportDataSource("DataSetFachSchuelerNoten",noten));
          }                    
          else if (subrpt=="subrptVorkommnis" || subrpt=="subrptAbiVorkommnis" )
          {
            diNoDataSet.vwVorkommnisDataTable vorkommnisse = new diNoDataSet.vwVorkommnisDataTable();
            vwVorkommnisTableAdapter BerichtTableAdapter;
            BerichtTableAdapter = new vwVorkommnisTableAdapter();
            if (subrpt=="subrptAbiVorkommnis")
              BerichtTableAdapter.FillBySchuelerIdForAbi(vorkommnisse, schuelerId);   //nur bestimmte Vorkommnisse selektieren              
            else 
              BerichtTableAdapter.FillBySchuelerId(vorkommnisse, schuelerId);
            e.DataSources.Add(new ReportDataSource("DataSetVorkommnis",(DataTable) vorkommnisse));
          }          
      }     
    }    
  }
  
    public class ReportBrief : ReportController
    {
      private BriefDaten bindingDataSource;
        public ReportBrief(BriefDaten dataSource) : base() {bindingDataSource = dataSource; }
        public override void Init()
        {            
            rpt.BerichtBindingSource.DataSource = bindingDataSource;
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

  public class ReportKlassenliste : ReportController
  {
    private BriefDaten bindingDataSource;
    public ReportKlassenliste(BriefDaten dataSource) : base() { bindingDataSource = dataSource; }

    public override void Init()
    {
      rpt.BerichtBindingSource.DataSource = bindingDataSource;
      rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptKlassenliste.rdlc";
    }
  }
}
