using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace diNo
{
  public partial class AdminKursLehrerForm : Form
  {
    /// <summary>
    /// Konstuktor.
    /// </summary>
    public AdminKursLehrerForm()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Der Nutzer hat auf Suche geklickt. Sucht nach Kursbezeichnung.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die Event Args.</param>
    private void btnSucheKurse_Click(object sender, EventArgs e)
    {
      FillKursliste(new KursTableAdapter().SucheByBezeichnung("%"+textBoxSuche.Text+"%"));
    }

    /// <summary>
    /// Beim Laden des Formulars werden alle Kurse schon mal eingefügt.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die Event Args.</param>
    private void AdminKursLehrer_Load(object sender, EventArgs e)
    {
      List<Lehrer> alleLehrer = new List<Lehrer>();
      foreach (var aLehrer in new LehrerTableAdapter().GetData())
      {
        alleLehrer.Add(new Lehrer(aLehrer));
      }

      alleLehrer.Sort((x, y) => x.KompletterName.CompareTo(y.KompletterName));
      this.comboBoxLehrer.DataSource = alleLehrer;

      FillKursliste(new KursTableAdapter().GetData());
    }

    /// <summary>
    /// Füllt die Kurs-ListView mit den übergebenen Kursen.
    /// </summary>
    /// <param name="table">Data Table mit den Kursen.</param>
    private void FillKursliste(diNoDataSet.KursDataTable table)
    {
      List<Kurs> kurse = new List<Kurs>();

      foreach (var aKurs in table)
      {
        kurse.Add(new Kurs(aKurs));
      }

      listBoxKurse.DataSource = kurse;
    }

    /// <summary>
    /// Ein neuer Kurs wurde per Klick ausgewählt. Zeigt den Lehrer in der ComboBox an.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die Event Args.</param>
    private void listBoxKurse_SelectedValueChanged(object sender, EventArgs e)
    {
      Kurs kurs = this.listBoxKurse.SelectedItem as Kurs;
      if (kurs == null)
      {
        this.comboBoxLehrer.SelectedItem = null;
      }
      else
      {
        // markiere den Lehrer auf der rechten Seite in der ComboBox
        if (kurs.getLehrer != null)
        {
          // dazu muss man ihn dummerweise von Hand suchen, weil SelectedItem ein anderes Lehrer-Objekt liefert
          int index = 0;
          foreach (Lehrer lehrer in this.comboBoxLehrer.Items)
          {
            if (lehrer.Id == kurs.getLehrer.Id)
            {
              this.comboBoxLehrer.SelectedIndex = index;
            }

            index++;
          }
          
        }
      }
    }

    /// <summary>
    /// Ein neuer Lehrer wurde ausgewählt.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die Event Args.</param>
    private void comboBoxLehrer_SelectedValueChanged(object sender, EventArgs e)
    {
      Lehrer lehrer = this.comboBoxLehrer.SelectedItem as Lehrer;
      Kurs kurs = this.listBoxKurse.SelectedItem as Kurs;

      if (lehrer != null && kurs != null)
      {
        if (lehrer.Id != kurs.getLehrer.Id)
        {
          kurs.SetzeNeuenLehrer(lehrer);
        }
      }
    }
  }
}
