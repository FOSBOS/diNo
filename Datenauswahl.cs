using System;
using System.Collections.Generic;
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
      cbAuswahl.SelectedIndex = 0;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      var erg = Zugriff.Instance.markierteSchueler;
      var liste = new List<Schueler>();

      // TODO: Andere Fälle auswerten, Wert aus cbAuswahl an DruckObjekte durchreichen.

      Zugriff.Instance.selectedVorkommnisart = (Vorkommnisart)cbVorkommnisArt.SelectedValue;
      erg.Clear();
      foreach (var k in Zugriff.Instance.Klassen)
        foreach (var s in k.Schueler)
        {
          if (s.hatVorkommnis(Zugriff.Instance.selectedVorkommnisart))
          {
            erg.Add(s.Id, s);
            liste.Add(s); // je nach Verwendungszweck andere Liste
          }
        }

      Hide();
      if (MessageBox.Show("Es wurden " + erg.Count + " Schüler ausgewählt.\nSoll die Standardübersicht ausgegeben werden? \nKlicken Sie nein für einen individuellen Druckvorgang.","dino",MessageBoxButtons.YesNo,MessageBoxIcon.Question)== DialogResult.Yes)
        new ReportSchuelerdruck(liste,Bericht.Auswahlliste).Show();
    }

    private void cbAuswahl_SelectedIndexChanged(object sender, EventArgs e)
    {
      cbVorkommnisArt.Enabled = cbAuswahl.SelectedIndex == 0;
      cbZubringerschule.Enabled = cbAuswahl.SelectedIndex == 1;
    }
  }
}
