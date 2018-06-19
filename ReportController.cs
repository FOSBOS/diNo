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
  
  
  // kann diverse Schüler-/Notenberichte drucken, die Grunddaten sind jeweils eine Schülerliste  
  public class ReportSchuelerdruck : ReportController
  {
    private List<SchuelerDruck> bindingDataSource= new List<SchuelerDruck>();
    private string rptName;
    private Bericht rptTyp;

    public ReportSchuelerdruck(List<Schueler> dataSource, Bericht b) : base()
    {      
      foreach (Schueler s in dataSource)
      {
        bindingDataSource.Add(SchuelerDruck.CreateSchuelerDruck(s, b));
      }

      rptTyp = b;
      rptName = SchuelerDruck.GetBerichtsname(b);
      if ((b == Bericht.Notenbogen || b == Bericht.Notenmitteilung || b == Bericht.Abiergebnisse)
            && dataSource[0].getKlasse.Jahrgangsstufe > Jahrgangsstufe.Elf)
      {
        rptName += "Alt"; // für diese Typen müssen noch Bericht nach alter FOBOSO verwendet werden.
      }
      rptName = "diNo." + rptName + ".rdlc";
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

        if (subrpt == "subrptZwischenzeugnis" || subrpt == "subrptJahreszeugnis" )
        {          
          IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenZeugnisDruck(rptTyp);
          e.DataSources.Add(new ReportDataSource("DataSet1", noten));
        }
        else if (subrpt == "subrptFPANoten")
        {
          e.DataSources.Add(new ReportDataSource("DataSetFPANoten", schueler.FPANotenDruck()));
        }
        else if (subrpt == "subrptNotenSjAlt" || subrpt == "subrptAbiergebnisseAlt")
        {
          var d = schueler.getNoten.SchuelerNotenDruckAlt(rptTyp);          
          e.DataSources.Add(new ReportDataSource("DataSet1", d));
        }
        else if (subrpt == "subrptVorkommnis" || subrpt == "subrptAbiVorkommnis")
        {
          diNoDataSet.vwVorkommnisDataTable vorkommnisse = new diNoDataSet.vwVorkommnisDataTable();
          vwVorkommnisTableAdapter BerichtTableAdapter;
          BerichtTableAdapter = new vwVorkommnisTableAdapter();
          if (subrpt == "subrptAbiVorkommnis")
            BerichtTableAdapter.FillBySchuelerIdForAbi(vorkommnisse, schuelerId);   //nur bestimmte Vorkommnisse selektieren              
          else
            BerichtTableAdapter.FillBySchuelerId(vorkommnisse, schuelerId);
          e.DataSources.Add(new ReportDataSource("DataSetVorkommnis", (DataTable)vorkommnisse));
        }
        else 
        {
          IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenDruck(rptTyp);
          e.DataSources.Add(new ReportDataSource("DataSet1", noten));
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

  public class ReportGefaehrdungen : ReportController
  {
    private List<Schueler> orgdataSource;
    private List<BriefDaten> bindingDataSource;
    public ReportGefaehrdungen(List<Schueler> dataSource) : base() 
    {
      orgdataSource = dataSource;
    }
    public override void Init()
    {
      // suche alle Schüler mit Gefährdungen
      bindingDataSource = new List<BriefDaten>();      
      foreach (var s in orgdataSource)
        {
          if (s.hatVorkommnis(Vorkommnisart.BeiWeiteremAbsinken) || s.hatVorkommnis(Vorkommnisart.starkeGefaehrdungsmitteilung))
          {
            var b = new BriefDaten(s, BriefTyp.Gefaehrdung);
            if (s.hatVorkommnis(Vorkommnisart.BeiWeiteremAbsinken))
              b.Inhalt = "Bei weiterem Absinken der Leistungen ist das Erreichen des Klassenziels gefährdet.";
            else
              b.Inhalt = "Das Erreichen des Klassenziels ist sehr gefährdet.";
            if (s.hatVorkommnis(Vorkommnisart.GefahrDerAbweisung))
              b.Inhalt += "\nDie Jahrgangstufe darf nicht mehr wiederholt werden.";

            b.Inhalt2 = s.VornameName + " hat ";
            if (!s.AlteFOBOSO()) b.Inhalt2 += "bei einem Punktedurchschnitt von " + String.Format("{0:0.00}", s.getNoten.Punkteschnitt) + " ";
            b.Inhalt2 += "in den folgenden Fächern nur die angeführten Leistungen erzielt:";
            bindingDataSource.Add(b);
          }

        }

      rpt.BerichtBindingSource.DataSource = bindingDataSource;
      rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptGefaehrdungen.rdlc";
      // Unterberichte einbinden
      rpt.reportViewer.LocalReport.SubreportProcessing +=
         new SubreportProcessingEventHandler(subrptEventHandler);
    }

    void subrptEventHandler(object sender, SubreportProcessingEventArgs e)
    {
      int schuelerId;
      int.TryParse(e.Parameters[0].Values[0], out schuelerId);
      if (schuelerId > 0)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
        IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenZeugnisDruck(Bericht.Gefaehrdung);
        e.DataSources.Add(new ReportDataSource("DataSet1", noten));
      }
    }
  }

  public class Dummy
  {
    public int Id { get; private set; }
    public Dummy()
    {
      Id=0;
    }
  }

}
