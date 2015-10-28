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
  public partial class UserControlSchueleransicht : UserControl
  {
    private Schueler schueler;

    public UserControlSchueleransicht()
    {
      InitializeComponent();
    }

    public Schueler Schueler
    {
      get
      {
        return schueler;
      }
      set
      {
        this.schueler = value;
        if (this.schueler != null)
        {
          nameLabel.Text = schueler.NameVorname;
          textBoxAdresse.Lines = schueler.NameUndAdresse.Split('\n');
          textBoxTelefonnummer.Text = schueler.Data.AnschriftTelefonnummer;
          textBoxID.Text = schueler.Id.ToString();
          textBoxGeburtsdatum.Text = schueler.Data.IsGeburtsdatumNull() ? "" : schueler.Data.Geburtsdatum.ToString("dd.MM.yyyy");
          textBoxGeburtsort.Text = schueler.Data.Geburtsort;
          textBoxBeruflicheVorbildung.Text = schueler.Data.BeruflicheVorbildung;
          textBoxSchulischeVorbildung.Text = schueler.Data.SchulischeVorbildung;
          textBoxWiederholungen.Text = schueler.getWiederholungen();
          textBoxJahrgangsstufe.Text = schueler.EintrittInJahrgangsstufe;
          textBoxEintrittAm.Text = schueler.EintrittAm == null ? "" : schueler.EintrittAm.Value.ToString("dd.MM.yyyy");
          textBoxVorigeSchule.Text = schueler.EintrittAusSchulname;
          string kontaktEltern = schueler.Data.VornameEltern1 + " " + schueler.Data.NachnameEltern1;
          kontaktEltern += string.IsNullOrEmpty(schueler.Data.VornameEltern2) ? "" : "\n" + schueler.Data.VornameEltern2 + " " + schueler.Data.NachnameEltern2;
          kontaktEltern += schueler.Data.IsNotfalltelefonnummerNull() ? "" : "\n" + schueler.Data.Notfalltelefonnummer; // da steht oft mehr als nur die Telefonnummer drin
          textBoxAdresseEltern.Lines = kontaktEltern.Split('\n');
          textBoxProbezeit.Text = schueler.Data.IsProbezeitBisNull() ? "" : schueler.Data.ProbezeitBis.ToString("dd.MM.yyyy");
         // TODO: Warum geht diese Zeile nicht?
          // textBoxEmail.Text = schueler.Data.IsEmailNull() ? "" : schueler.Data.EMail;
          Image imageToUse = schueler.Data.Geschlecht == "W" ? global::diNo.Properties.Resources.avatarFrau : global::diNo.Properties.Resources.avatarMann;
          pictureBoxImage.Image = new Bitmap(imageToUse, pictureBoxImage.Size);
        }
        else
        {
          nameLabel.Text = "";
          textBoxAdresse.Text = "";
          textBoxTelefonnummer.Text = "";
          textBoxID.Text = "";
          textBoxGeburtsdatum.Text = "";
          textBoxGeburtsort.Text = "";
          textBoxBeruflicheVorbildung.Text = "";
          textBoxSchulischeVorbildung.Text = "";
          textBoxWiederholungen.Text = "";
          textBoxJahrgangsstufe.Text = "";
          textBoxEintrittAm.Text = ""; 
          textBoxVorigeSchule.Text = "";
          textBoxAdresseEltern.Text = "";
          textBoxProbezeit.Text = "";
          textBoxEmail.Text = "";
          pictureBoxImage.Image = null;
        }
      }
    }
  }
}
