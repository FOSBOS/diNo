using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
