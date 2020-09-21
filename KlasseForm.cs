using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace diNo
{
  // war mal als abstrakte Basisklasse für Stammdatenformulare gedacht, aber der Designer macht da riesen Ärger
  // jetzt Kopiervorlage

  public partial class KlasseForm : BasisForm
  {
    private List<Klasse> t;
    private Klasse q;

    public KlasseForm()
    {
      InitializeComponent();
      Init();
    }

    private void Init()
    {
      List<Lehrer> alleLehrer;

      t = Zugriff.Instance.KlassenRep.getList();
      t.Sort((x, y) => x.Bezeichnung.CompareTo(y.Bezeichnung));
      liste.DataSource = t;

      alleLehrer = Zugriff.Instance.LehrerRep.getList();
      alleLehrer.Sort((x, y) => x.KompletterName.CompareTo(y.KompletterName));
      cbKlassenleiter.DataSource = alleLehrer;
    }

    private void liste_SelectedValueChanged(object sender, EventArgs e)
    {
      q = liste.SelectedItem as Klasse;
      if (q != null)
      {
        edBezeichnung.Text = q.Bezeichnung;
        edJgStufe.Text = ((byte)q.Jahrgangsstufe).ToString();
        edZweig.Text = q.Data.IsZweigNull() ? "" : q.Data.Zweig;
        cbSchulart.SelectedIndex = (int)q.Schulart;
        cbKlassenleiter.SelectedValue = q.KlassenleiterId;
      }        
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      q = null;
      edBezeichnung.Text = "";      
      edZweig.Text = "";
      edJgStufe.Text = "";
      cbSchulart.SelectedIndex = 1;
      cbKlassenleiter.SelectedValue = 0;
    }

    private void btnDel_Click(object sender, EventArgs e)
    {
      if (q != null)
      {
        if (MessageBox.Show("Soll die Klasse " + q.Data.Bezeichnung + " gelöscht werden?", "Löschen?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
          try
          {
            Zugriff.Instance.KlassenRep.Remove(q.Data.Id);
            var ta = new KlasseTableAdapter();
            ta.DeleteById(q.Data.Id);
            Init();
          }
          catch
          {
            MessageBox.Show("Diese Klasse konnte nicht gelöscht werden, weil sie Beziehungen zu Klassen, Lehrern oder Schülern besitzt.", "diNo", MessageBoxButtons.OK);
          }
        }

      }      
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      q = liste.SelectedItem as Klasse;
      q.Data.Bezeichnung = edBezeichnung.Text;
      q.Data.JgStufe = byte.Parse(edJgStufe.Text);
      q.Data.Schulart = (byte) cbSchulart.SelectedIndex;
      q.Data.Zweig = edZweig.Text;
      q.Data.KlassenleiterId = (int)cbKlassenleiter.SelectedValue;
      q.Save();
    }
  }
}
