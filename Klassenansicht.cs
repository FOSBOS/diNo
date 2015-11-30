using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace diNo
{
  public partial class Klassenansicht : Form
  {
    public Klassenansicht()
    {
      InitializeComponent();

      this.olvColumnBezeichnung.AspectGetter = KlassenTreeViewController.SelectValueCol1;
    }

    private void treeListView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      var schueler = treeListView1.SelectedObject as Schueler;
      if (schueler != null)
      {
        this.userControlSchueleransicht1.Schueler = schueler;
        this.userControlVorkommnisse1.Schueler = schueler;
        this.notenbogen1.Schueler = schueler;
        nameLabel.Text = schueler.NameVorname;
        klasseLabel.Text = schueler.getKlasse.Bezeichnung;
        Image imageToUse = schueler.Data.Geschlecht == "W" ? global::diNo.Properties.Resources.avatarFrau : global::diNo.Properties.Resources.avatarMann;
        pictureBoxImage.Image = new Bitmap(imageToUse, pictureBoxImage.Size);

      }
    }

    private void Klassenansicht_Load(object sender, EventArgs e)
    {
        this.treeListView1.Roots = KlassenTreeViewController.GetSortedKlassenList(true);
        this.treeListView1.CanExpandGetter = delegate (object x) { return (x is Klasse); };
        this.treeListView1.ChildrenGetter = delegate (object x) { return KlassenTreeViewController.GetSortedSchuelerList((Klasse)x); };
        nameLabel.Text = "";
        klasseLabel.Text = "";
        pictureBoxImage.Image = null;    
    }
  }
}
