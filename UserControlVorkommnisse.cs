using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlVorkommnisse : UserControl
  {
    private Schueler schueler;
    private Dictionary<Vorkommnisart, string> vorkommnisarten;

    public UserControlVorkommnisse()
    {
      InitializeComponent();

      InitVorkommnisse();
      this.olvColumnArt.AspectGetter = GetDisplayValueVorkommnisart;
    }

    public Schueler Schueler
    {
      get
      {
        return this.schueler;
      }
      set
      {
        this.schueler = value;
        this.lblSchuelername.Text = (value == null) ? "" : this.schueler.NameVorname;
        if (value != null)
        {
          this.objectListViewVorkommnisse.SetObjects(this.schueler.Vorkommnisse);
        }
      }
    }

    /// <summary>
    /// Gibt den Anzeige-string der Art eines Vorkommnisses zurück.
    /// </summary>
    /// <param name="aObject">Das anzuzeigende Vorkommnis.</param>
    /// <returns>Der Display value.</returns>
    private object GetDisplayValueVorkommnisart(object aObject)
    {
      Vorkommnis vorkommnis = aObject as Vorkommnis;
      if (vorkommnis == null)
      {
        throw new InvalidOperationException("Übergebenes Objekt kann nicht als Vorkommnis angezeigt werden");
      }

      if (!this.vorkommnisarten.ContainsKey(vorkommnis.Art))
      {
        throw new InvalidOperationException("Unbekannte Vorkommnisart "+vorkommnis.Art);
      }

      return this.vorkommnisarten[vorkommnis.Art];
    }

    private void InitVorkommnisse()
    {
      this.vorkommnisarten = new Dictionary<Vorkommnisart, string>();
      this.vorkommnisarten.Add(Vorkommnisart.NotSet, "");
      this.vorkommnisarten.Add(Vorkommnisart.Gefaehrdungsmitteilung, "Gefährdungsmitteilung");
      this.vorkommnisarten.Add(Vorkommnisart.JahrgangsstufeNichtBestanden, "Jahrgangsstufe nicht bestanden");
      this.vorkommnisarten.Add(Vorkommnisart.NichtZurPruefungZugelassen, "Nicht zur Prüfung zugelassen");
      this.vorkommnisarten.Add(Vorkommnisart.Notenausgleich, "Notenausgleich gewährt");
      this.vorkommnisarten.Add(Vorkommnisart.ProbezeitNichtBestanden, "Probezeit nicht bestanden");
      this.vorkommnisarten.Add(Vorkommnisart.PruefungInsgesamtNichtBestanden, "Prüfung im Endergebnis nicht bestanden");
      this.vorkommnisarten.Add(Vorkommnisart.PruefungSchriftlichNichtBestanden, "Prüfung nach dem schriftlichen Teil noch nicht bestanden");
      this.vorkommnisarten.Add(Vorkommnisart.SonstigeOrdnungsmaßnahme, "sonstige Ordnungsmaßnahme");
      this.vorkommnisarten.Add(Vorkommnisart.Verweis, "Verweis");
      this.vorkommnisarten.Add(Vorkommnisart.VorrueckenAufProbe, "Vorrücken auf Probe");

      this.comboBoxArt.BeginUpdate();
      this.comboBoxArt.DataSource = vorkommnisarten.ToList();
      this.comboBoxArt.DisplayMember = "Value";
      this.comboBoxArt.ValueMember = "Key";
      this.comboBoxArt.EndUpdate();
    }

    private void btnNeuesVorkommnis_Click(object sender, EventArgs e)
    {
      if ((Vorkommnisart)this.comboBoxArt.SelectedValue == Vorkommnisart.NotSet)
      {
        MessageBox.Show("Sie müssen erst auswählen, von welcher Art das Vorkommnis ist");
        return;
      }

      if (this.schueler != null)
      {
        this.schueler.AddVorkommnis((Vorkommnisart)this.comboBoxArt.SelectedValue, dateTimePicker1.Value, textBox1.Text);
        this.comboBoxArt.SelectedValue = Vorkommnisart.NotSet;
        this.textBox1.Text = "";
        this.objectListViewVorkommnisse.SetObjects(this.schueler.Vorkommnisse);
      }
    }
  }
}
