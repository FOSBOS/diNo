using System;
using System.Windows.Forms;

namespace diNo
{
  public partial class InputBox : Form
  {
    public InputBox()
    {
      InitializeComponent();
    }

    public static string Show(string label, string wert)
    {
      var i = new InputBox();
      i.lbInput.Text = label;
      i.edInput.Text = wert;
      if (i.ShowDialog() == DialogResult.Cancel) return "";
      return i.edInput.Text;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      Close();
    }
  }


}
