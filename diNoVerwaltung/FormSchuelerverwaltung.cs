using BrightIdeasSoftware;
using diNo;
using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diNoVerwaltung
{
  public partial class FormSchuelerverwaltung : Form
  {
    private static DateTime NullDate = new DateTimePicker().MinDate;

    public FormSchuelerverwaltung()
    {
      InitializeComponent();
      this.treeListView1.CellEditStarting += listViewComplex_CellEditStarting;
      this.treeListView1.CellEditFinishing += listViewComplex_CellEditFinishing;
      // this.treeListView1.AddDecoration(new EditingCellBorderDecoration(true));

      this.olvColumn1.AspectGetter = SelectValueCol1;
      this.olvSpalteLegasthenie.AspectGetter = SelectValueLegasthenie;
      this.olvSpalteLegasthenie.AspectPutter = SetValueLegasthenie;
      this.olvSpalteLegasthenie.Renderer = new MyCheckStateRenderer();

      this.olvSpalteFranzoesisch.AspectGetter = SelectValueFranzoesisch;
      this.olvSpalteFranzoesisch.AspectPutter = SetValueFranzoesisch;

      this.olvSpalteReli.AspectGetter = SelectValueReli;
      this.olvSpalteReli.AspectPutter = SetValueReliOderEthik;

      this.olvSpalteAustrittsdatum.AspectToStringConverter = GetDisplayValueForDate;
      this.olvSpalteAustrittsdatum.AspectGetter = SelectValueAustrittsdatum;
      this.olvSpalteAustrittsdatum.AspectPutter = SetValueAustrittsdatum;

      this.olvSpalteKlassenwechsel.AspectGetter = delegate (Object rowObject) { return "-->"; };
    }

    private string GetDisplayValueForDate(object value)
    {
      if ((DateTime)value == NullDate)
      {
        return string.Empty;
      }
      else
      {
        return ((DateTime)value).ToString("dd.MM.yyyy");
      }
    }

    private void SetValueAustrittsdatum(object rowObject, object newValue)
    {
      if (rowObject is Klasse)
      {
        return;
      }

      var schueler = rowObject as Schueler;
      if (schueler != null)
      {
        if (schueler.Data.IsAustrittsdatumNull() && (DateTime)newValue == NullDate)
        {
          return;
        }
        else
        {
          Schueler.Austritt(schueler, (DateTime)newValue);
        }
      }

      //ToDo: Sollte es möglich sein, einen Austritt wieder rückgängig zu machen (z. B. wenn beim falschen Schüler gesetzt)?
    }

    private object SelectValueAustrittsdatum(object rowObject)
    {
      if (rowObject is Klasse)
      {
        return NullDate;
      }

      var schueler = rowObject as Schueler;
      if (schueler != null)
      {
        return schueler.Data.IsAustrittsdatumNull() ? NullDate : schueler.Data.Austrittsdatum;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }

    private void SetValueReliOderEthik(object rowObject, object newValue)
    {
      Schueler schueler = rowObject as Schueler;
      if (schueler == null)
      {
        return;
      }

      string oldValue = schueler.ReliOderEthik;
      Schueler.WechsleKurse(schueler, oldValue, (string)newValue);
    }

    private void SetValueFranzoesisch(object rowObject, object newValue)
    {
      Schueler schueler = rowObject as Schueler;
      if (schueler == null)
      {
        return;
      }

      string oldValue = schueler.FranzoesischKurs;
      Schueler.WechsleKurse(schueler, oldValue, (string)newValue);
      // TODO: Auch Spalte Fremdsprache bzw. Wahlpflichtfach anpassen
    }

    private void SetValueLegasthenie(object rowObject, object newValue)
    {
      Schueler schueler = rowObject as Schueler;
      if (schueler == null)
      {
        return;
      }

      schueler.IsLegastheniker = (bool)newValue;
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
        DateTimePicker picker = new DateTimePicker();
        picker.Format = DateTimePickerFormat.Custom;
        picker.Bounds = e.CellBounds;
        picker.ValueChanged += (s, eventArgs) => { picker.CustomFormat = (picker.Checked && picker.Value != picker.MinDate) ? "dd.MM.yyyy" : " "; };
        picker.Value = (e.Value != null) ? (DateTime)e.Value : picker.MinDate;
        e.Control = picker;
      }

      // We only want to mess with the Französisch or Reli column
      if (!e.Column.Equals(this.olvSpalteFranzoesisch) && !e.Column.Equals(this.olvSpalteReli))
        return;

      ComboBox cb = new ComboBox();
      cb.Bounds = e.CellBounds;
      cb.Font = ((ObjectListView)sender).Font;
      cb.DropDownStyle = ComboBoxStyle.DropDownList;
      if (e.Column.Equals(this.olvSpalteFranzoesisch) && (e.RowObject is Schueler))
      {
        cb.Items.Add(new ComboBoxItem("", ""));
        cb.Items.Add(new ComboBoxItem("F", "F"));
        cb.Items.Add(new ComboBoxItem("F-Wi", "F3"));
        switch (schueler.FranzoesischKurs)
        {
          case "": cb.SelectedIndex = 0; break;
          case "F": cb.SelectedIndex = 1; break;
          case "F-Wi": cb.SelectedIndex = 2; break;
          default: throw new InvalidOperationException("Unbekannter Wert für FranzösischKurs" + ((Schueler)e.RowObject).FranzoesischKurs);
        }
      }
      if (e.Column.Equals(this.olvSpalteReli) && (e.RowObject is Schueler))
      {
        cb.Items.Add(new ComboBoxItem("", ""));
        cb.Items.Add(new ComboBoxItem("K", "katholisch"));
        cb.Items.Add(new ComboBoxItem("Ev", "evangelisch"));
        cb.Items.Add(new ComboBoxItem("Eth", "Ethik"));
        switch (schueler.ReliOderEthik)
        {
          case "": cb.SelectedIndex = 0; break;
          case "K": cb.SelectedIndex = 1; break;
          case "Ev": cb.SelectedIndex = 2; break;
          case "Eth": cb.SelectedIndex = 3; break;
          default: throw new InvalidOperationException("Unbekannter Wert für Religionskurs" + ((Schueler)e.RowObject).ReliOderEthik);
        }
      }

      cb.Tag = e.RowObject; // remember which person we are editing
      e.Control = cb;
    }

    private void listViewComplex_CellEditFinishing(object sender, CellEditEventArgs e)
    {
      // We only want to mess with the Französisch or Reli column
      if (!e.Column.Equals(this.olvSpalteFranzoesisch) && !!e.Column.Equals(this.olvSpalteReli))
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

    private object SelectValueCol1(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return ((Klasse)rowObject).Bezeichnung;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).NameVorname;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }

    private object SelectValueLegasthenie(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return null;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).IsLegastheniker;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }

    private object SelectValueFranzoesisch(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return null;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).FranzoesischKurs;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }

    private object SelectValueReli(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return null;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).ReliOderEthik;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      List<Klasse> klassen = new List<Klasse>();
      foreach (var klasse in (new KlasseTableAdapter().GetData()))
      {
        Klasse dieKlasse = new Klasse(klasse);
        klassen.Add(dieKlasse);
      }

      klassen.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));

      treeListView1.Roots = klassen;
      this.treeListView1.CanExpandGetter = delegate (object x) { return (x is Klasse); };
      this.treeListView1.ChildrenGetter = delegate (object x) { return GetSortedSchuelerList((Klasse)x); };
    }

    private static List<Schueler> GetSortedSchuelerList(Klasse klasse)
    {
      List<Schueler> schueler = new List<Schueler>();
      foreach (var einSchueler in klasse.getSchueler)
      {
        schueler.Add(new Schueler(einSchueler));
      }

      schueler.Sort((x, y) => x.NameVorname.CompareTo(y.NameVorname));
      return schueler;
    }

    private void treeListView1_HyperlinkClicked(object sender, HyperlinkClickedEventArgs e)
    {

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
