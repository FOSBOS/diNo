using diNo.diNoDataSetTableAdapters;
using System;
using System.Data;

namespace diNo
{

  public partial class GlobalesForm : BasisForm
  {
    private diNoDataSet.GlobaleStringsRow q;
    private GlobaleStringsTableAdapter ta;

    public GlobalesForm()
    {
      ta = new GlobaleStringsTableAdapter();
      InitializeComponent();
      var dt = ta.GetData();
      liste.DataSource = dt;
    }

    private void liste_SelectedValueChanged(object sender, EventArgs e)
    {
      q = ((DataRowView)liste.SelectedItem).Row as diNoDataSet.GlobaleStringsRow;
      if (q != null)
      {
        edBezeichnung.Text = q.Bezeichnung;
        edWert.Text = q.Wert;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      q.Wert = edWert.Text;
      ta.Update(q);
      Zugriff.Instance.RefreshGlobalesStrings();
    }
  }
}
