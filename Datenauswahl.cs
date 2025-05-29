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
  public partial class Datenauswahl : Form
  {
    public Datenauswahl()
    {
      InitializeComponent();
      cbVorkommnisArt.BeginUpdate();
      cbVorkommnisArt.DataSource = Vorkommnisse.Instance.Liste.ToList();
      cbVorkommnisArt.DisplayMember = "Value";
      cbVorkommnisArt.ValueMember = "Key";
      cbVorkommnisArt.EndUpdate();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      var erg = Zugriff.Instance.markierteSchueler;
      erg.Clear();
      foreach (var k in Zugriff.Instance.Klassen)
        foreach (var s in k.eigeneSchueler)
        {
          if (s.hatVorkommnis((Vorkommnisart)cbVorkommnisArt.SelectedValue))
            erg.Add(s.Id, s);
        }

      Hide();
      MessageBox.Show("Es wurden " + erg.Count + " Schüler ausgewählt.\nWählen Sie nun einen Menüpunkt unter Drucken.");
    }
  }
}
