using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class LehrerForm : BasisForm
  {
    private Lehrer q;
    private List<Lehrer> t;
    private LehrerTableAdapter ta = new LehrerTableAdapter();
    private List<diNoDataSet.RolleRow> rollen = new List<diNoDataSet.RolleRow>();

    public LehrerForm()
    {
      InitializeComponent();
      Init();
    }

    private void Init()
    {
      RolleTableAdapter rta = new RolleTableAdapter();
      foreach (var row in rta.GetData())
      {
        rollen.Add(row);
      }
      foreach (var rolle in rollen)
      {
        listBoxBerechtigungen.Items.Add(rolle.Bezeichnung, false);
      }

      t = Zugriff.Instance.LehrerRep.getList();
      t.Sort((x, y) => x.KompletterName.CompareTo(y.KompletterName));
      listLehrer.DataSource = t;
    }

    private void listLehrer_SelectedValueChanged(object sender, EventArgs e)
    {
      q = listLehrer.SelectedItem as Lehrer;
      edNachname.Text = q.Data.Nachname;
      edVorname.Text = q.Data.Vorname;
      edKuerzel.Text = q.Data.Kuerzel;
      edDienstbez.Text = q.Data.Dienstbezeichnung;
      edWindowsname.Text = q.Data.Windowsname;
      edMail.Text = (q.Data.IsEMailNull() ? "" : q.Data.EMail);
      opMaennlich.Checked = q.Data.Geschlecht == "M";
      opWeiblich.Checked = q.Data.Geschlecht == "W";

      foreach (var rolle in rollen)
      {
        listBoxBerechtigungen.SetItemChecked(listBoxBerechtigungen.Items.IndexOf(rolle.Bezeichnung), q.HatRolle(rolle.Id));
      }
    }

    private string F(TextBox t)
    {
      if (t.Text == "") return null;
      else return t.Text;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (q != null)
      {
        q.Data.Nachname = edNachname.Text;
        q.Data.Vorname = edVorname.Text;
        q.Data.Kuerzel = edKuerzel.Text;
        q.Data.Dienstbezeichnung = edDienstbez.Text;
        q.Data.Windowsname = edWindowsname.Text;
        if (edMail.Text == "") q.Data.SetEMailNull(); else q.Data.EMail = edMail.Text;
        q.Data.Geschlecht = (opMaennlich.Checked ? "M" : "W");
        ta.Update(q.Data);


        foreach (var rolle in rollen)
        {
          SetBerechtigung(rolle.Id, listBoxBerechtigungen.GetItemChecked(listBoxBerechtigungen.Items.IndexOf(rolle.Bezeichnung)));
        }
        /*
        SetBerechtigung(Rolle.Admin, checkBoxIsAdmin.Checked);
        SetBerechtigung(Rolle.Schulleitung, checkBoxIsSchulleitung.Checked);
        SetBerechtigung(Rolle.Sekretariat, checkBoxIsSekretariat.Checked);
        SetBerechtigung(Rolle.Seminarfach, checkBoxIsSeminarfach.Checked);
        SetBerechtigung(Rolle.FpAWirtschaft, checkBoxIsFpAWirtschaft.Checked);
        SetBerechtigung(Rolle.FpASozial, checkBoxIsFpASozial.Checked);
        SetBerechtigung(Rolle.FpATechnik, checkBoxIsFpATechnik.Checked);
        SetBerechtigung(Rolle.FpAUmwelt, checkBoxIsFpAUmwelt.Checked);
        */
      }
      else
      {
        try
        {
          ta.Insert(F(edKuerzel), F(edDienstbez), F(edMail), F(edWindowsname), F(edVorname), F(edNachname), (opMaennlich.Checked ? "M" : "W"));
          //TODO: ta. sollte irgendwie verraten, welche Id er in der DB vergeben hat, dann nur den neu laden
          Zugriff.Instance.LehrerRep.Clear();
          Zugriff.Instance.LoadLehrer();
          Init();
        }
        catch
        {
          MessageBox.Show("Dieser Lehrer konnte nicht eingefügt werden, weil nicht alle Pflichtfelder ausgefüllt wurden.", "diNo", MessageBoxButtons.OK);
        }

        listBoxBerechtigungen.Enabled = true;
      }
    }

    private void SetBerechtigung(int rolle, bool newValue)
    {
      if (q != null)
      {
        if (newValue && !q.HatRolle(rolle))
        {
          q.AddRolle(rolle);
        }

        if (!newValue && q.HatRolle(rolle))
        {
          q.RemoveRolle(rolle);
        }
      }
    }

    private void btnDel_Click(object sender, EventArgs e)
    {
      if (q != null)
      {
        if (MessageBox.Show("Soll der Lehrer " + q.KompletterName + " gelöscht werden?", "Löschen?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
          try
          {
            Zugriff.Instance.LehrerRep.Remove(q.Data.Id);
            ta.DeleteById(q.Data.Id);
            Init();
          }
          catch
          {
            MessageBox.Show("Dieser Lehrer konnte nicht gelöscht werden, weil er Beziehungen zu Klassen oder Kursen besitzt.", "diNo", MessageBoxButtons.OK);
          }
        }

      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      q = null;
      edNachname.Text = "";
      edVorname.Text = "";
      edKuerzel.Text = "";
      edDienstbez.Text = "";
      edWindowsname.Text = "";
      edMail.Text = "";
      opMaennlich.Checked = true;
      opWeiblich.Checked = false;

      for (int i = 0; i < listBoxBerechtigungen.Items.Count; i++)
      {
        listBoxBerechtigungen.SetItemChecked(i, false);
      }

      listBoxBerechtigungen.Enabled = false;
    }

    private void edVorname_Leave(object sender, EventArgs e)
    {
      if (Zugriff.Instance.IsFBKempten)
      {
        if (String.IsNullOrEmpty(edMail.Text))
          edMail.Text = edVorname.Text.ToLower() + "." + edNachname.Text.ToLower() + "@fosbos-kempten.de";
        if (String.IsNullOrEmpty(edWindowsname.Text))
          edWindowsname.Text = edVorname.Text.ToLower().First() +edNachname.Text.ToLower();
      }
    }
  }
}
