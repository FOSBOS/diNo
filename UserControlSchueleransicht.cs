using System;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlSchueleransicht : UserControl
  {
    private Schueler schueler;

    public UserControlSchueleransicht()
    {
      InitializeComponent();
      SchuelerverwaltungController.InitDateTimePicker(this.dateTimeProbezeit);
      SchuelerverwaltungController.InitDateTimePicker(this.dateTimeAustritt);
      btnSave.Visible = Zugriff.Instance.HatVerwaltungsrechte;
      btnResetProbezeit.Visible = btnSave.Visible;
      labelAustrittHinweis.Visible = btnSave.Visible;
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
          var eigeneAnschrift = schueler.getEigeneAnschrift();
          string strasse = eigeneAnschrift == null || eigeneAnschrift.IsStrasseNull() ? "" : eigeneAnschrift.Strasse;
          if (eigeneAnschrift != null && !eigeneAnschrift.IsHausnummerNull() && eigeneAnschrift.Hausnummer != "")
            strasse += " " + eigeneAnschrift.Hausnummer;
          textBoxStrasse.Text = strasse;
          textBoxPLZ.Text = eigeneAnschrift == null || eigeneAnschrift.IsPLZNull() ? "" : eigeneAnschrift.PLZ;
          textBoxOrt.Text = eigeneAnschrift == null || eigeneAnschrift.IsOrtNull() ? "" : eigeneAnschrift.Ort;
          textBoxTelefonnummer.Text = eigeneAnschrift == null || eigeneAnschrift.IsTelefonnummerNull() ? "" : eigeneAnschrift.Telefonnummer;
          textBoxNotfalltelefonnummer.Text = schueler.Data.Notfalltelefonnummer;

          textBoxGeburtsdatum.Text = schueler.Data.IsGeburtsdatumNull() ? "" : schueler.Data.Geburtsdatum.ToString("dd.MM.yyyy");
          textBoxGeburtsort.Text = schueler.Data.Geburtsort;
          textBoxWiederholungen.Text = schueler.getWiederholungen();
          textBoxBeruflicheVorbildung.Text = schueler.Data.BeruflicheVorbildung;
          textBoxVorigeSchule.Text = schueler.EintrittAusSchulname;

          textBoxJahrgangsstufe.Text = schueler.EintrittInJahrgangsstufe;
          textBoxEintrittAm.Text = schueler.EintrittAm == null ? "" : schueler.EintrittAm.Value.ToString("dd.MM.yyyy");
          var eltern1 = schueler.getErziehungsberechtigter("1");
          var eltern2 = schueler.getErziehungsberechtigter("2");
          string kontaktEltern = eltern1 == null ? "" : eltern1.VornamePerson + " " + eltern1.NachnamePerson;
          kontaktEltern += (eltern2 == null || string.IsNullOrEmpty(eltern2.VornamePerson)) ? "" : "\n" + eltern2.VornamePerson + " " + eltern2.NachnamePerson;
          textBoxAdresseEltern.Lines = kontaktEltern.Split('\n');
          textBoxBekenntnis.Text = schueler.Data.Bekenntnis;

          dateTimeProbezeit.Value = schueler.Data.IsProbezeitBisNull() ? dateTimeProbezeit.MinDate : schueler.Data.ProbezeitBis;
          dateTimeAustritt.Value = schueler.Data.IsAustrittsdatumNull() ? dateTimeAustritt.MinDate : schueler.Data.Austrittsdatum;
          textBoxEmail.Text = schueler.Data.Email;
          textBoxMailSchule.Text = schueler.Data.IsMailSchuleNull() ? "" : schueler.Data.MailSchule;
          cbStatus.SelectedIndex = schueler.Data.Status;
        }        
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      schueler.SaveEigeneAnschrift(textBoxStrasse.Text, textBoxPLZ.Text, textBoxOrt.Text, textBoxTelefonnummer.Text);
      schueler.Data.Notfalltelefonnummer = textBoxNotfalltelefonnummer.Text;

      schueler.Data.Bekenntnis = textBoxBekenntnis.Text;
      // ReliUnterricht via Kurszuordnung wird automatisch gesetzt!       

      if (dateTimeProbezeit.Value == dateTimeProbezeit.MinDate) schueler.Data.SetProbezeitBisNull();
      else schueler.Data.ProbezeitBis = dateTimeProbezeit.Value;
      if (dateTimeAustritt.Value == dateTimeAustritt.MinDate) schueler.Data.SetAustrittsdatumNull();
      else schueler.Data.Austrittsdatum = dateTimeAustritt.Value;

      schueler.Data.Email = textBoxEmail.Text;
      schueler.Data.Geburtsort = textBoxGeburtsort.Text;
      schueler.Data.MailSchule = textBoxMailSchule.Text;
      schueler.Data.Status = cbStatus.SelectedIndex;

      schueler.Save();
    }

    private void dateTimeAustritt_ValueChanged(object sender, EventArgs e)
    {
      if (dateTimeAustritt.Value != dateTimeAustritt.MinDate) cbStatus.SelectedIndex = 1; // abgemeldet
      else cbStatus.SelectedIndex = 0; // aktiv

    }

    private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbStatus.SelectedIndex == 0)
        dateTimeAustritt.Value = dateTimeAustritt.MinDate;
      else if (cbStatus.SelectedIndex == 1 && dateTimeAustritt.Value == dateTimeAustritt.MinDate) // nur setzen wenn nicht eh schon ein Datum drinsteht
        dateTimeAustritt.Value = DateTime.Today;
    }

    private void buttonResetProbezeit_Click(object sender, EventArgs e)
    {
      dateTimeProbezeit.Value = dateTimeProbezeit.MinDate;
    }
  }
}
