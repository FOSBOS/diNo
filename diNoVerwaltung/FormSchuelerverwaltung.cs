using BrightIdeasSoftware;
using diNo;
using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace diNoVerwaltung
{
  public partial class FormSchuelerverwaltung : Form
  {
    private static DateTime NullDate = new DateTimePicker().MinDate;

    public FormSchuelerverwaltung()
    {
      InitializeComponent();

      this.olvColumn1.AspectGetter = SelectValueCol1;
      this.olvSpalteLegasthenie.AspectGetter = SelectValueLegasthenie;
      this.olvSpalteLegasthenie.AspectPutter = SetValueLegasthenie;
      this.olvSpalteLegasthenie.Renderer = new MyCheckStateRenderer();

      this.olvSpalteWahlpflichtfach.AspectGetter = SelectValueWahlpflichtfach;
      this.olvSpalteWahlpflichtfach.AspectPutter = SetValueWahlpflichtfach;

      this.olvSpalteFremdsprache2.AspectGetter = SelectValueFremdsprache2;
      this.olvSpalteFremdsprache2.AspectPutter = SetValueFremdsprache2;

      this.olvSpalteReli.AspectGetter = SelectValueReli;
      this.olvSpalteReli.AspectPutter = SetValueReliOderEthik;

      this.olvSpalteAustrittsdatum.AspectToStringConverter = GetDisplayValueForDate;
      this.olvSpalteAustrittsdatum.AspectGetter = SelectValueAustrittsdatum;
      this.olvSpalteAustrittsdatum.AspectPutter = SetValueAustrittsdatum;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      List<Klasse> klassen = new List<Klasse>();
      foreach (var klasse in (new KlasseTableAdapter().GetData()))
      {
        Klasse dieKlasse = new Klasse(klasse);
        if (dieKlasse.getSchueler.Count > 0)
        {
          // Dies filtert wenigstens ein Paar Dummy- und Spassklassen heraus
          klassen.Add(dieKlasse);
        }
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

    #region AspectValueSetter

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

    private void SetValueReliOderEthik(object rowObject, object newValue)
    {
      Schueler schueler = rowObject as Schueler;
      if (schueler == null)
      {
        return;
      }

      schueler.ReliOderEthik = (string)newValue;
    }

    /// <summary>
    /// Setzt den Wert für das Wahlpflichtfach, sollte F, F3, Win oder Ku sein
    /// </summary>
    /// <param name="rowObject">Der Schüler.</param>
    /// <param name="newValue">Der neue Wert für das Wahlpflichtfach.</param>
    private void SetValueWahlpflichtfach(object rowObject, object newValue)
    {
      Schueler schueler = rowObject as Schueler;
      if (schueler == null)
      {
        return;
      }

      if (schueler.Wahlpflichtfach != (string)newValue)
      {
        schueler.Wahlpflichtfach = (string)newValue;
      }
    }

    /// <summary>
    /// Setzt den Wert für die Fremdsprache, sollte F oder leer sein.
    /// </summary>
    /// <param name="rowObject">Der Schüler.</param>
    /// <param name="newValue">Der neue Wert für die Fremdsprache.</param>
    private void SetValueFremdsprache2(object rowObject, object newValue)
    {
      Schueler schueler = rowObject as Schueler;
      if (schueler == null)
      {
        return;
      }

      if (schueler.Fremdsprache2 != (string)newValue)
      {
        schueler.Fremdsprache2 = (string)newValue;
      }
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

    #endregion

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
      if (!e.Column.Equals(this.olvSpalteWahlpflichtfach) && !e.Column.Equals(this.olvSpalteReli))
        return;

      ComboBox cb = new ComboBox();
      cb.Bounds = e.CellBounds;
      cb.Font = ((ObjectListView)sender).Font;
      cb.DropDownStyle = ComboBoxStyle.DropDownList;
      if (e.Column.Equals(this.olvSpalteWahlpflichtfach) && (e.RowObject is Schueler))
      {
        cb.Items.Add(new ComboBoxItem("", ""));
        cb.Items.Add(new ComboBoxItem("F", "F"));
        cb.Items.Add(new ComboBoxItem("F-Wi", "F3"));
        cb.Items.Add(new ComboBoxItem("Ku", "Ku"));
        cb.Items.Add(new ComboBoxItem("WIn", "WIn"));
        switch (schueler.Wahlpflichtfach)
        {
          case "": cb.SelectedIndex = 0; break;
          case "F": cb.SelectedIndex = 1; break;
          case "F-Wi": cb.SelectedIndex = 2; break;
          case "Ku": cb.SelectedIndex = 3; break;
          case "WIn": cb.SelectedIndex = 4; break;
          default: throw new InvalidOperationException("Unbekannter Wert für Wahlpflichtfach" + ((Schueler)e.RowObject).Wahlpflichtfach);
        }
      }
      if (e.Column.Equals(this.olvSpalteFremdsprache2) && (e.RowObject is Schueler))
      {
        cb.Items.Add(new ComboBoxItem("", ""));
        cb.Items.Add(new ComboBoxItem("F", "F"));
        switch (schueler.Fremdsprache2)
        {
          case "": cb.SelectedIndex = 0; break;
          case "F": cb.SelectedIndex = 1; break;
          default: throw new InvalidOperationException("Unbekannter Wert für Fremdsprache2" + ((Schueler)e.RowObject).Fremdsprache2);
        }
      }
      if (e.Column.Equals(this.olvSpalteReli) && (e.RowObject is Schueler))
      {
        cb.Items.Add(new ComboBoxItem("", ""));
        cb.Items.Add(new ComboBoxItem("RK", "katholisch"));
        cb.Items.Add(new ComboBoxItem("EV", "evangelisch"));
        cb.Items.Add(new ComboBoxItem("Eth", "Ethik"));
        switch (schueler.ReliOderEthik)
        {
          case "":
          case null: cb.SelectedIndex = 0; break;
          case "RK": cb.SelectedIndex = 1; break;
          case "EV": cb.SelectedIndex = 2; break;
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

    #region AspectSelection

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

    private object SelectValueWahlpflichtfach(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return null;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).Wahlpflichtfach;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }


    private object SelectValueFremdsprache2(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return null;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).Fremdsprache2;
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

#endregion

    #region Drag and Drop

    private void treeListView1_ModelCanDrop(object sender, ModelDropEventArgs e)
    {
      e.Handled = true;
      e.Effect = DragDropEffects.None;

      Klasse targetKlasse = GetDragTargetKlasse(e);

      if (targetKlasse == null)
      {
        e.InfoMessage = "Ziel konnte nicht erkannt werden.";
        return;
      }

      Schueler derSchueler = GetDraggedSchueler(e.SourceModels);
      if (derSchueler == null)
      {
        e.InfoMessage = "Nur Schülerzeilen können in eine andere Klasse verschoben werden.";
        return;
      }
      else
      {
        e.Effect = DragDropEffects.Move;
      }
    }

    private static Klasse GetDragTargetKlasse(ModelDropEventArgs e)
    {
      Klasse targetKlasse = e.TargetModel as Klasse;
      if (targetKlasse == null)
      {
        Schueler targetSchueler = e.TargetModel as Schueler;
        if (targetSchueler != null)
        {
          targetKlasse = targetSchueler.getKlasse;
        }
      }

      return targetKlasse;
    }

    private Schueler GetDraggedSchueler(IList draggedSourceModels)
    {
      foreach (var obj in draggedSourceModels)
      {
        Schueler schueler = obj as Schueler;
        if (schueler != null)
        {
          return schueler;
        }
      }

      return null;
    }

    private void treeListView1_ModelDropped(object sender, ModelDropEventArgs e)
    {
      Schueler derSchueler = GetDraggedSchueler(e.SourceModels);
      Klasse targetKlasse = GetDragTargetKlasse(e);
      var questionResult = MessageBox.Show("Soll der Schüler " + derSchueler.NameVorname + " von der " + derSchueler.getKlasse.Bezeichnung + " in die " + targetKlasse.Bezeichnung + " verschoben werden?", "Nachfrage", MessageBoxButtons.YesNo);
      if (questionResult == DialogResult.Yes)
      {
        Schueler.WechsleKlasse(derSchueler, targetKlasse);
      }
      else
      {
        e.Effect = DragDropEffects.None;
      }
    }

    #endregion

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
