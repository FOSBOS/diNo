using BrightIdeasSoftware;
using diNo;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace diNoVerwaltung
{
  public class FormSchuelerverwaltungController
  {
    public static DateTime NullDate = new DateTimePicker().MinDate;
    private Action refreshFunc;

    public FormSchuelerverwaltungController(Action aRefreshFunc)
    {
      this.refreshFunc = aRefreshFunc;
    }

    public static string GetDisplayValueForDate(object value)
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

    public static void SetValueAustrittsdatum(object rowObject, object newValue)
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

    public static void SetValueReliOderEthik(object rowObject, object newValue)
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
    public static void SetValueWahlpflichtfach(object rowObject, object newValue)
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
    public static void SetValueFremdsprache2(object rowObject, object newValue)
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

    public static void SetValueLegasthenie(object rowObject, object newValue)
    {
      Schueler schueler = rowObject as Schueler;
      if (schueler == null)
      {
        return;
      }

      schueler.IsLegastheniker = (bool)newValue;
    }

    #endregion


    #region AspectSelection

    public static object SelectValueLegasthenie(Object rowObject)
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

    public static object SelectValueWahlpflichtfach(Object rowObject)
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

    public static object SelectValueFremdsprache2(Object rowObject)
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

    public static object SelectValueReli(Object rowObject)
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

    public static object SelectValueAustrittsdatum(object rowObject)
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

    public void treeListView1_ModelCanDrop(object sender, ModelDropEventArgs e)
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

    public static Klasse GetDragTargetKlasse(ModelDropEventArgs e)
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

    public static Schueler GetDraggedSchueler(IList draggedSourceModels)
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

    public void treeListView1_ModelDropped(object sender, ModelDropEventArgs e)
    {
      Schueler derSchueler = GetDraggedSchueler(e.SourceModels);
      Klasse targetKlasse = GetDragTargetKlasse(e);
      var questionResult = MessageBox.Show("Soll der Schüler " + derSchueler.NameVorname + " von der " + derSchueler.getKlasse.Bezeichnung + " in die " + targetKlasse.Bezeichnung + " verschoben werden?", "Nachfrage", MessageBoxButtons.YesNo);
      if (questionResult == DialogResult.Yes)
      {
        Schueler.WechsleKlasse(derSchueler, targetKlasse);
        if (this.refreshFunc != null)
        {
          this.refreshFunc();
        }
      }
      else
      {
        e.Effect = DragDropEffects.None;
      }
    }

    #endregion

    public static DateTimePicker CreateDateTimePicker(Rectangle bounds, object value)
    {
      DateTimePicker picker = new DateTimePicker();
      picker.Format = DateTimePickerFormat.Custom;
      picker.Bounds = bounds;
      picker.ValueChanged += (s, eventArgs) => { picker.CustomFormat = (picker.Checked && picker.Value != picker.MinDate) ? "dd.MM.yyyy" : " "; };
      picker.Value = (value != null) ? (DateTime)value : picker.MinDate;
      return picker;
    }

    public static void FillCheckboxWahlpflichtfach(Schueler schueler, ComboBox cb)
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
        default: throw new InvalidOperationException("Unbekannter Wert für Wahlpflichtfach" + schueler.Wahlpflichtfach);
      }
    }

    public static void FillCheckboxFremdsprache2(Schueler schueler, ComboBox cb)
    {
      cb.Items.Add(new ComboBoxItem("", ""));
      cb.Items.Add(new ComboBoxItem("F", "F"));
        switch (schueler.Fremdsprache2)
        {
          case "": cb.SelectedIndex = 0; break;
          case "F": cb.SelectedIndex = 1; break;
        default: throw new InvalidOperationException("Unbekannter Wert für Fremdsprache2" + schueler.Fremdsprache2);
      }
    }

    public static void FillCheckBoxReliOderEthik(Schueler schueler, ComboBox cb)
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
        default: throw new InvalidOperationException("Unbekannter Wert für Religionskurs" + schueler.ReliOderEthik);
      }
    }
      

  }
}
