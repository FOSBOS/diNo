using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diNo
{
  public partial class ListeForm : Form
  {
    public ListeForm(List<String> lst)
    {
      InitializeComponent();
      foreach (var s in lst)
        listView1.Items.Add(s);
    }
  }
}
