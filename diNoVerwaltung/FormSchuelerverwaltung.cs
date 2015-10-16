using BrightIdeasSoftware;
using diNo;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace diNoVerwaltung
{
  public partial class FormSchuelerverwaltung : Form
  {
    public FormSchuelerverwaltung()
    {
      InitializeComponent();

      this.olvColumn1.AspectGetter = KlassenTreeViewController.SelectValueCol1;
      this.olvSpalteLegasthenie.AspectGetter = FormSchuelerverwaltungController.SelectValueLegasthenie;
      this.olvSpalteLegasthenie.AspectPutter = FormSchuelerverwaltungController.SetValueLegasthenie;
      this.olvSpalteLegasthenie.Renderer = new MyCheckStateRenderer();

      this.olvSpalteWahlpflichtfach.AspectGetter = FormSchuelerverwaltungController.SelectValueWahlpflichtfach;
      this.olvSpalteWahlpflichtfach.AspectPutter = FormSchuelerverwaltungController.SetValueWahlpflichtfach;

      this.olvSpalteFremdsprache2.AspectGetter = FormSchuelerverwaltungController.SelectValueFremdsprache2;
      this.olvSpalteFremdsprache2.AspectPutter = FormSchuelerverwaltungController.SetValueFremdsprache2;

      this.olvSpalteReli.AspectGetter = FormSchuelerverwaltungController.SelectValueReli;
      this.olvSpalteReli.AspectPutter = FormSchuelerverwaltungController.SetValueReliOderEthik;

      this.olvSpalteAustrittsdatum.AspectToStringConverter = FormSchuelerverwaltungController.GetDisplayValueForDate;
      this.olvSpalteAustrittsdatum.AspectGetter = FormSchuelerverwaltungController.SelectValueAustrittsdatum;
      this.olvSpalteAustrittsdatum.AspectPutter = FormSchuelerverwaltungController.SetValueAustrittsdatum;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      treeListView1.Roots = KlassenTreeViewController.GetSortedKlassenList();
      this.treeListView1.CanExpandGetter = delegate (object x) { return (x is Klasse); };
      this.treeListView1.ChildrenGetter = delegate (object x) { return KlassenTreeViewController.GetSortedSchuelerList((Klasse)x); };
    }

    private void listViewComplex_CellEditStarting(object sender, CellEditEventArgs e)
    {
      Schueler schueler = e.RowObject as Schueler;
      if (schueler == null)
      {
        // nur Schülerobjekte werden geändert
        e.Cancel = true;
        return;
      }

      if (e.Column.Equals(this.olvSpalteAustrittsdatum))
      {
        e.Control = FormSchuelerverwaltungController.CreateDateTimePicker(e.CellBounds, e.Value);
      }

      // We only want to mess with the Französisch or Reli column
      if (!e.Column.Equals(this.olvSpalteWahlpflichtfach) && !e.Column.Equals(this.olvSpalteReli))
        return;

      ComboBox cb = new ComboBox();
      cb.Bounds = e.CellBounds;
      cb.Font = ((ObjectListView)sender).Font;
      cb.DropDownStyle = ComboBoxStyle.DropDownList;
      if (e.Column.Equals(this.olvSpalteWahlpflichtfach))
      {
        FormSchuelerverwaltungController.FillCheckboxWahlpflichtfach(schueler, cb);
      }
      if (e.Column.Equals(this.olvSpalteFremdsprache2))
      {
        FormSchuelerverwaltungController.FillCheckboxFremdsprache2(schueler, cb);
      }
      if (e.Column.Equals(this.olvSpalteReli))
      {
        FormSchuelerverwaltungController.FillCheckBoxReliOderEthik(schueler, cb);
      }

      cb.Tag = e.RowObject; // remember which person we are editing
      e.Control = cb;
    }

    private void listViewComplex_CellEditFinishing(object sender, CellEditEventArgs e)
    {
      // We only want to mess with the Französisch or Reli column
      if (!e.Column.Equals(this.olvSpalteWahlpflichtfach) && !e.Column.Equals(this.olvSpalteReli) && !e.Column.Equals(this.olvSpalteFremdsprache2))
      {
        return;
      }

      var box = e.Control as ComboBox;
      if (box != null && box.SelectedValue == null && box.SelectedItem != null)
      {
        e.NewValue = ((ComboBoxItem)box.SelectedItem).Key;
      }

      // Here we simply make the list redraw the involved ListViewItem
      ((ObjectListView)sender).RefreshItem(e.ListViewItem);
    }

    private class MyCheckStateRenderer : CheckStateRenderer
    {
      /// <summary>
      /// Draw our cell
      /// </summary>
      /// <param name="g"></param>
      /// <param name="r"></param>
      public override void Render(Graphics g, Rectangle r)
      {
        this.DrawBackground(g, r);
        Schueler schueler = this.RowObject as Schueler;
        if (this.Column == null || schueler == null)
          return;
        r = this.ApplyCellPadding(r);
        CheckState state = this.Column.GetCheckState(this.RowObject);
        if (this.IsPrinting)
        {
          // Renderers don't work onto printer DCs, so we have to draw the image ourselves
          string key = ObjectListView.CHECKED_KEY;
          if (state == CheckState.Unchecked)
            key = ObjectListView.UNCHECKED_KEY;
          if (state == CheckState.Indeterminate)
            key = ObjectListView.INDETERMINATE_KEY;
          this.DrawAlignedImage(g, r, this.ListView.SmallImageList.Images[key]);
        }
        else
        {
          r = this.CalculateCheckBoxBounds(g, r);
          CheckBoxRenderer.DrawCheckBox(g, r.Location, this.GetCheckBoxState(state));
        }
      }
    }
  }
}
