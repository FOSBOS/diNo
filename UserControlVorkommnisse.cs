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
    private Vorkommnisart[] notenRelevanteVorkommnisse = new[] { Vorkommnisart.Gefaehrdungsmitteilung, Vorkommnisart.JahrgangsstufeNichtBestanden, Vorkommnisart.NichtZurPruefungZugelassen, Vorkommnisart.Notenausgleich, Vorkommnisart.ProbezeitNichtBestanden, Vorkommnisart.PruefungInsgesamtNichtBestanden, Vorkommnisart.PruefungSchriftlichNichtBestanden, Vorkommnisart.VorrueckenAufProbe };

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
        //this.lblSchuelername.Text = (value == null) ? "" : this.schueler.NameVorname;
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
        var art = (Vorkommnisart)this.comboBoxArt.SelectedValue;
        if (art == Vorkommnisart.ProbezeitNichtBestanden)
        {
          new ReportNotenbogen(schueler); // drucke den Notenbogen als Abschluss aus
        }

        this.schueler.AddVorkommnis(art, dateTimePicker1.Value, textBox1.Text);
        // ist doch praktisch, weil meistens gleich mehrere ähnliche Vorkommnisse auftreten
        //this.comboBoxArt.SelectedValue = Vorkommnisart.NotSet; 
        //this.textBox1.Text = "";
        RefreshVorkommnisse();
      }
    }

    public void RefreshVorkommnisse()
    {
      //schueler.Refresh();
      objectListViewVorkommnisse.SetObjects(this.schueler.Vorkommnisse);
    }

    /// <summary>
    /// Methode trägt für manche Vorkommnisse vorsorglich die relevanten Noten als Bemerkung ein.
    /// Funktioniert dies nicht automatisch korrekt, muss von Hand die Bemerkung angepasst werden.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die event args.</param>
    private void comboBoxArt_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!(this.comboBoxArt.SelectedValue is Vorkommnisart)) // beim Init wird das Ereignis einmal mit einem KeyValuePaar ausgelöst - seltsames Verhalten
        return;
      if (schueler==null) return;

      var art = (Vorkommnisart)this.comboBoxArt.SelectedValue;
      
      if (notenRelevanteVorkommnisse.Contains(art))
      {
        this.textBox1.Text = schueler.getNoten.GetUnterpunktungenString(ErrateZeitpunkt(art));
      }
      else
      {
        this.textBox1.Text = "";
      }
    }

    /// <summary>
    /// Methode versucht den korrekten Zeitpunkt zu erraten.
    /// Dient nur dem Komfort, weil dann die relevanten Noten automatisch ermittelt werden.
    /// Funktioniert dies nicht automatisch korrekt, muss von Hand die Bemerkung angepasst werden.
    /// </summary>
    /// <param name="vorkommnis">Das Vorkommnis.</param>
    /// <returns>Möglicher Zeitpunkt, an welchem dieses Vorkommnis meist auftritt.</returns>
    private Zeitpunkt ErrateZeitpunkt(Vorkommnisart vorkommnis)
    {
      switch (vorkommnis)
      {
        case Vorkommnisart.Gefaehrdungsmitteilung: return Zeitpunkt.HalbjahrUndProbezeitFOS;
        case Vorkommnisart.ProbezeitNichtBestanden: return DateTime.Now.Month == 12 ? Zeitpunkt.ProbezeitBOS : Zeitpunkt.HalbjahrUndProbezeitFOS;
        case Vorkommnisart.NichtZurPruefungZugelassen: return Zeitpunkt.ErstePA;
        case Vorkommnisart.JahrgangsstufeNichtBestanden: return Zeitpunkt.Jahresende;
        case Vorkommnisart.PruefungInsgesamtNichtBestanden: return Zeitpunkt.DrittePA;
        case Vorkommnisart.PruefungSchriftlichNichtBestanden: return Zeitpunkt.ZweitePA;
        case Vorkommnisart.VorrueckenAufProbe: return Zeitpunkt.Jahresende;
        default: return Zeitpunkt.Jahresende;
      }
    }

  }
}
