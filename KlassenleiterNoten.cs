using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace diNo
{
  public class KlassenleiterNoten
  {
    public KlassenleiterNoten(List<Klasse> klassen)      
    {
      string datei;
      foreach (Klasse k in klassen)
      {
        //datei = "\\SRVFOSBOS\\" + k.Klassenleiter.Data.Windowsname + "\\Notenübersicht " +  k.Bezeichnung + ".pdf";
        datei = "C:\\tmp\\Notenübersicht " + k.Bezeichnung + ".pdf";
        CreatePdf(k, datei);
      }
    }

    // erzeugt eine Notenmitteilung in PDF-Form
    public void CreatePdf(Klasse k, string targetFile)
    {
      List<SchuelerDruck> liste = new List<SchuelerDruck>();

      //ReportViewer im Hintergrund erstellen, um PDF zu drucken, und Report zuweisen.
      ReportViewer rpt = new ReportViewer();
      ReportDataSource dataSource = new ReportDataSource();
      dataSource.Name = "DataSet1";

      //Report als Embedded Resource mit dem Namen "Report.rdlc" ... entsprechend anpassen
      rpt.LocalReport.ReportEmbeddedResource = "diNo.rptNotenmitteilung.rdlc";

      // Unterberichte einbinden
      rpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(subrptEventHandler);

      //Report mit Daten befüllen
      foreach (Schueler s in k.eigeneSchueler)
      {        
        liste.Add(SchuelerDruck.CreateSchuelerDruck(s, Bericht.Notenmitteilung, UnterschriftZeugnis.SL));
      }
      dataSource.Value = liste; // ob das geht???
      rpt.LocalReport.DataSources.Add(dataSource);

      //Report als PDF in Datei speichern
      byte[] PDF = rpt.LocalReport.Render("PDF");
      FileStream fsReport = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None);
      fsReport.Write(PDF, 0, PDF.Length);
      fsReport.Close();

      rpt.Dispose();
    }
   
    void subrptEventHandler(object sender, SubreportProcessingEventArgs e)
    {
      // ACHTUNG: Der Parameter muss im Haupt- und im Unterbericht definiert werden (mit gleichem Namen)
      // string subrpt = e.ReportPath; // jeder Unterbericht ruft diesen EventHandler auf; hier steht drin welcher es ist.
      int schuelerId;
      int.TryParse(e.Parameters[0].Values[0], out schuelerId);
      if (schuelerId > 0)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
        IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenDruck(Bericht.Notenmitteilung);
        e.DataSources.Add(new ReportDataSource("DataSet1", noten));
      }
    }
  }
}
