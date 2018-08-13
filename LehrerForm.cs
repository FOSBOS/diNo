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
  public partial class LehrerForm : BasisForm
  {
    private Lehrer q;
    private List<Lehrer> t;
    private LehrerTableAdapter ta = new LehrerTableAdapter();
    
    public LehrerForm()
    {
      InitializeComponent();
      Init();
    }

    private void Init()
    {
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

      checkBoxIsAdmin.Checked = q.HatRolle(Rolle.Admin);
      checkBoxIsSchulleitung.Checked = q.HatRolle(Rolle.Schulleitung);
      checkBoxIsSekretariat.Checked = q.HatRolle(Rolle.Sekretariat);
      checkBoxIsSeminarfach.Checked = q.HatRolle(Rolle.Seminarfach);
      checkBoxIsFpAWirtschaft.Checked = q.HatRolle(Rolle.FpAWirtschaft);
      checkBoxIsFpASozial.Checked = q.HatRolle(Rolle.FpASozial);
      checkBoxIsFpATechnik.Checked = q.HatRolle(Rolle.FpATechnik);
      checkBoxIsFpAUmwelt.Checked = q.HatRolle(Rolle.FpAUmwelt);
    }

    private string F(TextBox t)
    {
      if (t.Text == "") return null;
      else return t.Text;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (q!=null) 
      {
        q.Data.Nachname = edNachname.Text;
        q.Data.Vorname = edVorname.Text;
        q.Data.Kuerzel = edKuerzel.Text;
        q.Data.Dienstbezeichnung = edDienstbez.Text;
        q.Data.Windowsname = edWindowsname.Text;
        if (edMail.Text == "") q.Data.SetEMailNull(); else q.Data.EMail = edMail.Text;
        q.Data.Geschlecht = (opMaennlich.Checked ? "M":"W");
        ta.Update(q.Data);

        SetBerechtigung(Rolle.Admin, checkBoxIsAdmin.Checked);
        SetBerechtigung(Rolle.Schulleitung, checkBoxIsSchulleitung.Checked);
        SetBerechtigung(Rolle.Sekretariat, checkBoxIsSekretariat.Checked);
        SetBerechtigung(Rolle.Seminarfach, checkBoxIsSeminarfach.Checked);
        SetBerechtigung(Rolle.FpAWirtschaft, checkBoxIsFpAWirtschaft.Checked);
        SetBerechtigung(Rolle.FpASozial, checkBoxIsFpASozial.Checked);
        SetBerechtigung(Rolle.FpATechnik, checkBoxIsFpATechnik.Checked);
        SetBerechtigung(Rolle.FpAUmwelt, checkBoxIsFpAUmwelt.Checked);
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
        groupBoxBerechtigungen.Enabled = true;
      }
    }

    private void SetBerechtigung(Rolle rolle, bool newValue)
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
      if (q!=null)
      {        
        if (MessageBox.Show("Soll der Lehrer " + q.KompletterName +" gelöscht werden?", "Löschen?", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

      checkBoxIsAdmin.Checked = false;
      checkBoxIsSchulleitung.Checked = false;
      checkBoxIsSekretariat.Checked = false;
      checkBoxIsSeminarfach.Checked = false;
      checkBoxIsFpAWirtschaft.Checked = false;
      checkBoxIsFpASozial.Checked = false;
      checkBoxIsFpATechnik.Checked = false;
      checkBoxIsFpAUmwelt.Checked = false;

      groupBoxBerechtigungen.Enabled = false;
    }
  }
}
