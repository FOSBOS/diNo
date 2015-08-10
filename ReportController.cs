using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
    public enum Berichtsliste
    {
        rptLehrerliste, 
        rptSchuelerliste,
        rptFachliste,
        rptNotenbogen
    }


    class ReportController    
    {
        private Berichtsliste rptID;

        public ReportController(Berichtsliste aRptID)
        {
            ReportForm rpt = new ReportForm();
            
            rptID = aRptID;
            switch (rptID)
            {   case Berichtsliste.rptLehrerliste:
                    {
                        LehrerTableAdapter BerichtTableAdapter;

                        rpt.BerichtBindingSource.DataMember = "Lehrer";
                        BerichtTableAdapter = new LehrerTableAdapter();
                        BerichtTableAdapter.ClearBeforeFill = true;
                        BerichtTableAdapter.Fill(rpt.diNoDataSet.Lehrer);
                        rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptLehrerliste.rdlc";

                        break;
                    }    
                case Berichtsliste.rptSchuelerliste:
                    {
                        SchuelerTableAdapter BerichtTableAdapter;

                        rpt.BerichtBindingSource.DataMember = "Schueler";
                        BerichtTableAdapter = new SchuelerTableAdapter();
                        BerichtTableAdapter.ClearBeforeFill = true;
                        BerichtTableAdapter.FillByKlasse(rpt.diNoDataSet.Schueler, 34); // 11Tb 
                        rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptSchuelerliste.rdlc";

                        break;
                    }
                case Berichtsliste.rptFachliste:
                    {
                        FachTableAdapter BerichtTableAdapter;

                        rpt.BerichtBindingSource.DataMember = "Fach";
                        BerichtTableAdapter = new FachTableAdapter();
                        BerichtTableAdapter.ClearBeforeFill = true;
                        BerichtTableAdapter.Fill(rpt.diNoDataSet.Fach);
                        rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptFachliste.rdlc";

                        break;
                    }
                default:
                    break;
            }
            rpt.reportViewer.RefreshReport();
            rpt.Show();

        }

    }
}
