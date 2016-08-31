using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlAdministration : UserControl
  {
    private Schueler schueler;


    public UserControlAdministration()
    {
      InitializeComponent();
    }

    public Schueler Schueler
    {
      get
      {
        return schueler;
      }
      set
      {
        this.schueler = value;
        if (this.schueler != null)
        {
        }
      }
    }

    private void btnFrm1_Click(object sender, EventArgs e)
    {
      Form1 frm = new Form1();
      frm.Show();
    }

    private void btnAbiergebnisse_Click(object sender, EventArgs e)
    {
      // Elternreihenfolge: usercontrol -> Tabpage -> pageControl -> Form Klassenansicht
      var obj = ((Klassenansicht)(Parent.Parent.Parent)).SelectedObjects();
      new ReportNotenbogen(obj,true).Show();
    }
  }
}
