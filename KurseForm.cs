using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class KurseForm : BasisForm
  {
    private Kurs q;
    private List<Kurs> t;
    private KursTableAdapter ta = new KursTableAdapter();

    public KurseForm()
    {
      InitializeComponent();
      Init();
      SetValues();
    }

    private void Init()
    {
      List<Lehrer> alleLehrer;
      List<Fach> alleFaecher;

      t = new List<Kurs>();
      var dt = ta.GetData();
      foreach (var d in dt)
        t.Add(new Kurs(d));

      t.Sort((x, y) => x.Kursbezeichnung.CompareTo(y.Kursbezeichnung));
      liste.DataSource = t;

      alleLehrer = Zugriff.Instance.LehrerRep.getList();
      alleLehrer.Sort((x, y) => x.KompletterName.CompareTo(y.KompletterName));
      cbLehrer.DataSource = alleLehrer;

      alleFaecher = Zugriff.Instance.FachRep.getList();
      alleFaecher.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
      cbFach.DataSource = alleFaecher;

      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        checkedListBoxKlassen.Items.Add(k, false);
      }
    }

    private void listLehrer_SelectedValueChanged(object sender, EventArgs e)
    {
      SetValues();
    }

    private void SetValues()
    {
      q = liste.SelectedItem as Kurs;
      edBezeichnung.Text = q.Data.Bezeichnung;
      edKurzbez.Text = q.Data.IsKurzbezNull() ? "" : q.Data.Kurzbez;
      edId.Text = q.Data.Id.ToString();
      edZweig.Text = (q.Data.IsZweigNull() ? "" : q.Data.Zweig);
      opUndef.Checked = q.Data.IsGeschlechtNull();
      opMaennlich.Checked = !q.Data.IsGeschlechtNull() && q.Data.Geschlecht == "M";
      opWeiblich.Checked = !q.Data.IsGeschlechtNull() && q.Data.Geschlecht == "W";

      cbLehrer.SelectedValue = q.getLehrer.Id; // in der ComboBox muss als ValueMember Id stehen!!
      cbFach.SelectedValue = q.getFach.Id;

      for (int i = 0; i < checkedListBoxKlassen.Items.Count; i++)
      {
        checkedListBoxKlassen.SetItemChecked(i, q.Klassen.Contains(checkedListBoxKlassen.Items[i]));
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
        q.Data.Bezeichnung = edBezeichnung.Text;
        // = edKurzbez.Text;        
        if (edZweig.Text == "") q.Data.SetZweigNull(); else q.Data.Zweig = edZweig.Text;
        if (opUndef.Checked) q.Data.SetGeschlechtNull();
        else if (opMaennlich.Checked) q.Data.Geschlecht = "M";
        else q.Data.Geschlecht = "W";

        q.Data.Id = int.Parse(edId.Text);
        q.Data.LehrerId = (int)cbLehrer.SelectedValue;
        q.Data.FachId = (int)cbFach.SelectedValue;
        q.SetLehrerNull();
        q.SetFachNull();
        ta.Update(q.Data);

        q.Klassen.Clear();
        foreach (Klasse klasse in checkedListBoxKlassen.CheckedItems)
        {
          q.Klassen.Add(klasse);
        }
        q.SaveKlassenzuordnung();
      }
      else
      {
        try
        {
          ta.Insert(int.Parse(edId.Text), F(edBezeichnung), (int)cbLehrer.SelectedValue, (int)cbFach.SelectedValue, F(edZweig), (opUndef.Checked ? null : (opMaennlich.Checked ? "M" : "W")), (edKurzbez.Text == "" ? null : edKurzbez.Text));
          Init();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "diNo", MessageBoxButtons.OK);
        }
        //groupBoxBerechtigungen.Enabled = true;
      }
    }

    private void btnDel_Click(object sender, EventArgs e)
    {
      if (q != null)
      {
        if (MessageBox.Show("Soll der Kurs " + q.Data.Bezeichnung + " gelöscht werden?", "Löschen?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
          try
          {
            Zugriff.Instance.KursRep.Remove(q.Data.Id);
            ta.DeleteById(q.Data.Id);
            Init();
          }
          catch
          {
            MessageBox.Show("Dieser Kurs konnte nicht gelöscht werden, weil er Beziehungen zu Klassen, Lehrern oder Schülern besitzt.", "diNo", MessageBoxButtons.OK);
          }
        }

      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      q = null;
      edBezeichnung.Text = "";
      edKurzbez.Text = "";
      edZweig.Text = "";
      opUndef.Checked = true;
      opMaennlich.Checked = false;
      opWeiblich.Checked = false;
      edId.Text = "";
    }

    private void btnErzeugeExcel_Click(object sender, EventArgs e)
    {
      var datei = new ErzeugeNeueExcelDatei(q.Data);
      datei.Dispose();
    }
  }
}
