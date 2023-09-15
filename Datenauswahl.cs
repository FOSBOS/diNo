using System;
using System.Linq;
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
      Zugriff.Instance.selectedVorkommnisart = (Vorkommnisart)cbVorkommnisArt.SelectedValue;
      erg.Clear();
      foreach (var k in Zugriff.Instance.Klassen)
        foreach (var s in k.Schueler)
        {
          if (s.hatVorkommnis(Zugriff.Instance.selectedVorkommnisart))
            erg.Add(s.Id, s);
        }

      Hide();
      MessageBox.Show("Es wurden " + erg.Count + " Schüler ausgewählt.\nWählen Sie nun einen Menüpunkt unter Drucken.");
    }
  }
}
