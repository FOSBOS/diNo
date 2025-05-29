using BrightIdeasSoftware;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace diNo
{
  public class SchuelerverwaltungController
  {
    public static DateTime NullDate = new DateTimePicker().MinDate;
    private Action refreshFunc;

    public SchuelerverwaltungController(Action aRefreshFunc)
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
        derSchueler.WechsleKlasse(targetKlasse);
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

    public static void InitDateTimePicker(DateTimePicker picker)
    {
      picker.Format = DateTimePickerFormat.Custom;
      picker.ValueChanged += (s, eventArgs) => { picker.CustomFormat = (picker.Checked && picker.Value != picker.MinDate) ? "dd.MM.yyyy" : " "; };
    }  

  }
}
