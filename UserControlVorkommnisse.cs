﻿using diNo.Properties;
using System;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlVorkommnisse : UserControl
  {
    private Schueler schueler;
    private Vorkommnisart[] notenRelevanteVorkommnisse = new[] { Vorkommnisart.Gefaehrdungsmitteilung, Vorkommnisart.starkeGefaehrdungsmitteilung, Vorkommnisart.BeiWeiteremAbsinken, Vorkommnisart.KeineVorrueckungserlaubnis, Vorkommnisart.NichtZurPruefungZugelassen, Vorkommnisart.Notenausgleich, Vorkommnisart.ProbezeitNichtBestanden, Vorkommnisart.NichtBestanden, Vorkommnisart.nichtBestandenMAPnichtZugelassen, Vorkommnisart.VorrueckenAufProbe };

    public UserControlVorkommnisse()
    {
      InitializeComponent();

      InitVorkommnisse();
      this.olvColumnArt.AspectGetter = GetDisplayValueVorkommnisart;
      this.olvColumnLoeschen.ImageGetter = delegate { return Resources.muell; };
      this.olvColumnLoeschen.AspectGetter = delegate { return "Delete"; };
      this.olvColumnLoeschen.AspectToStringConverter = delegate { return string.Empty; };
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
          VorbelegungVorkommnistext(); // auch beim Schülerwechsel vorbelegen
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

      if (!Vorkommnisse.Instance.Liste.ContainsKey(vorkommnis.Art))
      {
        throw new InvalidOperationException("Unbekannte Vorkommnisart " + vorkommnis.Art);
      }

      return Vorkommnisse.Instance.Liste[vorkommnis.Art];
    }

    private void InitVorkommnisse()
    {
      this.comboBoxArt.BeginUpdate();
      this.comboBoxArt.DataSource = Vorkommnisse.Instance.Liste.ToList();
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
        /*if (art == Vorkommnisart.ProbezeitNichtBestanden)
        {
          new ReportNotenbogen(schueler); // drucke den Notenbogen als Abschluss aus; geht auch nachher noch.
        }
        */

        this.schueler.AddVorkommnis(art, datVorkommnis.Value, edVorkommnisBemerkung.Text, true);
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
      if (schueler == null) return;
      VorbelegungVorkommnistext();
    }

    private void VorbelegungVorkommnistext()
    {
      var art = (Vorkommnisart)this.comboBoxArt.SelectedValue;

      if (notenRelevanteVorkommnisse.Contains(art))
      {
        edVorkommnisBemerkung.Text = schueler.getNoten.Unterpunktungen;
      }
      else
      {
        this.edVorkommnisBemerkung.Text = "";
      }
    }

    private void objectListViewVorkommnisse_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
    {      // special cell edit handling for our delete-row
      if (e.Column == olvColumnLoeschen)
      {
        e.Cancel = true;        // we don't want to edit anything

        Vorkommnis geklicktesVorkommnis = e.RowObject as Vorkommnis;
        if (geklicktesVorkommnis != null && this.schueler != null)
        {
          DialogResult ergebnis = MessageBox.Show("Soll das Vorkommnis gelöscht werden?", "Löschen?", MessageBoxButtons.YesNo);
          if (ergebnis == DialogResult.Yes)
          {
            this.schueler.RemoveVorkommnis(geklicktesVorkommnis.Id);
            this.objectListViewVorkommnisse.RemoveObject(e.RowObject); // remove object
          }
        }
      }
    }
  }
}
