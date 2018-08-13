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
   // war mal als abstrakte Basisklasse für Stammdatenformulare gedacht, aber der Designer macht da riesen Ärger
   // jetzt Kopiervorlage

  public partial class Stammdaten : BasisForm
  {
    public Stammdaten()
    {
      InitializeComponent();
      
    }

    protected void ValueChanged(IRepositoryObject obj) {; }
    protected void Add(IRepositoryObject obj) {; }
    protected void Delete(IRepositoryObject obj) {; }
    protected void Save(IRepositoryObject obj) {; }

    private void liste_SelectedValueChanged(object sender, EventArgs e)
    {
      var t = liste.SelectedItem as IRepositoryObject;
      if (t!=null)
        ValueChanged(liste.SelectedItem as IRepositoryObject);
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      Add(liste.SelectedItem as IRepositoryObject);
    }

    private void btnDel_Click(object sender, EventArgs e)
    {
      Delete(liste.SelectedItem as IRepositoryObject);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      Save(liste.SelectedItem as IRepositoryObject);
    }
  }
}
