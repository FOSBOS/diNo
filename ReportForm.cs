using System;
using System.Windows.Forms;

namespace diNo
{
  public partial class ReportForm : Form
  {
    public ReportForm()
    {
      InitializeComponent();
    }

    private void ReportForm_Load(object sender, EventArgs e)
    {
      // TODO: Diese Codezeile lädt Daten in die Tabelle "diNoDataSet.Lehrer". Sie können sie bei Bedarf verschieben oder entfernen.
      // this.BerichtTableAdapter.Fill(this.diNoDataSet.Lehrer);

      // this.reportViewer.RefreshReport();
    }
  }
}
