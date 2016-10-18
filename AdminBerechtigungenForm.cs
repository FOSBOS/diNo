using diNo.diNoDataSetTableAdapters;
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
  public partial class AdminBerechtigungenForm : Form
  {
    public AdminBerechtigungenForm()
    {
      InitializeComponent();
    }

    private void AdminBerechtigungenForm_Load(object sender, EventArgs e)
    {
      LehrerTableAdapter ada = new LehrerTableAdapter();
      List<Lehrer> alleLehrer = new List<Lehrer>();
      foreach (diNoDataSet.LehrerRow row in ada.GetData())
      {
        alleLehrer.Add(new Lehrer(row));
      }

      alleLehrer.Sort((x, y) => x.Name.CompareTo(y.Name));
      comboBoxLehrer.DataSource = alleLehrer;

      List<KeyValuePair<Rolle, string>> alleRollen = new List<KeyValuePair<Rolle, string>>();
      foreach (Rolle aRolle in Enum.GetValues(typeof(Rolle)))
      {
        alleRollen.Add(new KeyValuePair<Rolle, string>(aRolle, Enum.GetName(typeof(Rolle), aRolle)));
      }

      comboBoxBerechtigungen.DataSource = alleRollen;
    }

    private void comboBoxLehrer_SelectedValueChanged(object sender, EventArgs e)
    {
      Lehrer selectedLehrer = comboBoxLehrer.SelectedItem as Lehrer;
      if (selectedLehrer != null)
      {
        checkBoxIsAdmin.Checked = selectedLehrer.HatRolle(Rolle.Admin);
        checkBoxIsSchulleitung.Checked = selectedLehrer.HatRolle(Rolle.Schulleitung);
        checkBoxIsSekretariat.Checked = selectedLehrer.HatRolle(Rolle.Sekretariat);
        checkBoxIsSeminarfach.Checked = selectedLehrer.HatRolle(Rolle.Seminarfach);
        checkBoxIsFpAWirtschaft.Checked = selectedLehrer.HatRolle(Rolle.FpAWirtschaft);
        checkBoxIsFpASozial.Checked = selectedLehrer.HatRolle(Rolle.FpASozial);
        checkBoxIsFpATechnik.Checked = selectedLehrer.HatRolle(Rolle.FpATechnik);
        checkBoxIsFpAAgrar.Checked = selectedLehrer.HatRolle(Rolle.FpAAgrar);
      }
      else
      {
        checkBoxIsAdmin.Checked = false;
        checkBoxIsSchulleitung.Checked = false;
        checkBoxIsSekretariat.Checked = false;
        checkBoxIsSeminarfach.Checked = false;
        checkBoxIsFpAWirtschaft.Checked = false;
        checkBoxIsFpASozial.Checked = false;
        checkBoxIsFpATechnik.Checked = false;
        checkBoxIsFpAAgrar.Checked = false;
      }
    }

    private void SetBerechtigung(Rolle rolle, bool newValue)
    {
      Lehrer selectedLehrer = comboBoxLehrer.SelectedItem as Lehrer;
      if (selectedLehrer != null)
      {
        if (newValue && !selectedLehrer.HatRolle(rolle))
        {
          selectedLehrer.AddRolle(rolle);
        }

        if (!newValue && selectedLehrer.HatRolle(rolle))
        {
          selectedLehrer.RemoveRolle(rolle);
        }
      }
    }

    private void checkBoxIsAdmin_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.Admin, checkBoxIsAdmin.Checked);
    }

    private void checkBoxIsSchulleitung_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.Schulleitung, checkBoxIsSchulleitung.Checked);
    }

    private void checkBoxIsSekretariat_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.Sekretariat, checkBoxIsSekretariat.Checked);
    }

    private void checkBoxIsSeminarfach_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.Seminarfach, checkBoxIsSeminarfach.Checked);
    }

    private void checkBoxIsFpAWirtschaft_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.FpAWirtschaft, checkBoxIsFpAWirtschaft.Checked);
    }

    private void checkBoxIsFpASozial_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.FpASozial, checkBoxIsFpASozial.Checked);
    }

    private void checkBoxIsFpATechnik_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.FpATechnik, checkBoxIsFpATechnik.Checked);
    }

    private void checkBoxIsFpAAgrar_CheckedChanged(object sender, EventArgs e)
    {
      SetBerechtigung(Rolle.FpAAgrar, checkBoxIsFpAAgrar.Checked);
    }

    private void comboBoxBerechtigungen_SelectedValueChanged(object sender, EventArgs e)
    {
      if (comboBoxBerechtigungen.SelectedItem == null)
        return;

      KeyValuePair<Rolle, string> kvp = (KeyValuePair<Rolle, string>)comboBoxBerechtigungen.SelectedItem;
      IList<Lehrer> alleLehrer = (IList<Lehrer>)comboBoxLehrer.DataSource;
      var betroffeneLehrer = alleLehrer.Where(x => x.HatRolle(kvp.Key));
      textBoxLehrerNamen.Lines = betroffeneLehrer.Select(x => x.KompletterName).ToArray();
    }
  }
}
