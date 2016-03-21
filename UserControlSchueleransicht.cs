using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

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
          textBoxStrasse.Text = schueler.Data.AnschriftStrasse;
          textBoxPLZ.Text = schueler.Data.AnschriftPLZ;
          textBoxOrt.Text = schueler.Data.AnschriftOrt;
          textBoxTelefonnummer.Text = schueler.Data.AnschriftTelefonnummer;
          textBoxNotfalltelefonnummer.Text = schueler.Data.Notfalltelefonnummer;

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
          textBoxAdresseEltern.Lines = kontaktEltern.Split('\n');
          textBoxBekenntnis.Text = schueler.Data.Bekenntnis;
          textBoxReliUnterricht.Text = schueler.Data.ReligionOderEthik;

          textBoxProbezeit.Text = schueler.Data.IsProbezeitBisNull() ? "" : schueler.Data.ProbezeitBis.ToString("dd.MM.yyyy");
          textBoxAustritt.Text = schueler.Data.IsAustrittsdatumNull() ? "" : schueler.Data.Austrittsdatum.ToString("dd.MM.yyyy");
          textBoxEmail.Text = schueler.Data.Email;
          cbStatus.SelectedIndex = schueler.Data.Status;
          textBoxDNote.Text = schueler.Data.IsDNoteNull() ? "" : string.Format("{0:F1}", schueler.Data.DNote);
        }
        else
        {
          //nameLabel.Text = "";
          textBoxStrasse.Text = "";
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
          //pictureBoxImage.Image = null;
        }
      }
    }

    public void DatenUebernehmen()
    {
      schueler.Data.AnschriftStrasse = textBoxStrasse.Text;
      schueler.Data.AnschriftPLZ = textBoxPLZ.Text;
      schueler.Data.AnschriftOrt = textBoxOrt.Text;
      schueler.Data.AnschriftTelefonnummer = textBoxTelefonnummer.Text;
      schueler.Data.Notfalltelefonnummer = textBoxNotfalltelefonnummer.Text;
          
      schueler.Data.Bekenntnis = textBoxBekenntnis.Text;
      // ReliUnterricht via Kurszuordnung          

      if (textBoxProbezeit.Text=="") schueler.Data.SetProbezeitBisNull();
        else schueler.Data.ProbezeitBis = DateTime.Parse(textBoxProbezeit.Text, CultureInfo.CurrentCulture);      
      if (textBoxAustritt.Text=="") schueler.Data.SetAustrittsdatumNull();
        else schueler.Data.Austrittsdatum = DateTime.Parse(textBoxAustritt.Text, CultureInfo.CurrentCulture);            

      schueler.Data.Email = textBoxEmail.Text;
      schueler.Data.Status = cbStatus.SelectedIndex;
    }

  }
}
