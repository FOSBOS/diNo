using System.Collections.Generic;
using Microsoft.Reporting.WinForms;

namespace diNo
{
  // Reines RDLC->PDF-Rendering, ohne WinForms-UI-Abhängigkeit (kein ReportViewer-Control,
  // kein ReportForm). Wird sowohl vom interaktiven Druck-Pfad (ReportController.Show())
  // als auch vom Mail-Versand (MailTools.CreatePdf()) genutzt, damit beide dieselbe
  // Rendering- und Subreport-Logik verwenden statt sie unabhängig zu duplizieren.
  public static class PdfReportRenderer
  {
    public static byte[] RenderToPdf(LocalReport report)
    {
      string mimeType, encoding, filenameExtension;
      string[] streamIds;
      Warning[] warnings;
      return report.Render("PDF", null, out mimeType, out encoding,
          out filenameExtension, out streamIds, out warnings);
    }

    public static byte[] BuildAndRenderToPdf(
        string embeddedResourceName,
        string dataSourceName,
        object dataSourceValue,
        SubreportProcessingEventHandler subreportHandler,
        IList<ReportParameter> parameters = null)
    {
      var report = new LocalReport();
      report.ReportEmbeddedResource = embeddedResourceName;
      if (dataSourceValue != null)
        report.DataSources.Add(new ReportDataSource(dataSourceName, dataSourceValue));
      if (subreportHandler != null)
        report.SubreportProcessing += subreportHandler;
      if (parameters != null && parameters.Count > 0)
        report.SetParameters(parameters);
      try
      {
        return RenderToPdf(report);
      }
      finally
      {
        report.Dispose();
      }
    }
  }
}
