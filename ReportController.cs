using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
    public enum Berichtsliste
    {
        rptLehrerListe, 
        rptSchuelerliste,
        rptNotenbogen
    }


    class ReportController    
    {
        private Berichtsliste rptID;
        public ReportController(Berichtsliste aRptID)
        {
            string rptName, rptDataMember;
            ReportForm rpt;
            
            rpt = new ReportForm();
            
            rptID = aRptID;
            switch (rptID)
            {   case Berichtsliste.rptLehrerListe:
                    rptName = "rptLehrerListe.rdlc";
                    rptDataMember = "Lehrer";
                    //this.BerichtTableAdapter = new diNo.diNoDataSetTableAdapters.LehrerTableAdapter();
                    break;
                default:
                    break;
            }
        }

    }
}
