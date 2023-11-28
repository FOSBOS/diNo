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
          textBoxStrasse.Text = schueler.Data.AnschriftStrasse;
          textBoxPLZ.Text = schueler.Data.AnschriftPLZ;
          textBoxOrt.Text = schueler.Data.AnschriftOrt;
          textBoxTelefonnummer.Text = schueler.Data.AnschriftTelefonnummer;
          textBoxNotfalltelefonnummer.Text = schueler.Data.Notfalltelefonnummer;

          textBoxGeburtsdatum.Text = schueler.Data.IsGeburtsdatumNull() ? "" : schueler.Data.Geburtsdatum.ToString("dd.MM.yyyy");
          textBoxGeburtsort.Text = schueler.Data.Geburtsort;
          textBoxWiederholungen.Text = schueler.getWiederholungen();
          textBoxBeruflicheVorbildung.Text = schueler.Data.BeruflicheVorbildung;
          textBoxVorigeSchule.Text = schueler.EintrittAusSchulname;

          textBoxJahrgangsstufe.Text = schueler.EintrittInJahrgangsstufe;
          textBoxEintrittAm.Text = schueler.EintrittAm == null ? "" : schueler.EintrittAm.Value.ToString("dd.MM.yyyy");
          string kontaktEltern = schueler.Data.VornameEltern1 + " " + schueler.Data.NachnameEltern1;
          kontaktEltern += string.IsNullOrEmpty(schueler.Data.VornameEltern2) ? "" : "\n" + schueler.Data.VornameEltern2 + " " + schueler.Data.NachnameEltern2;
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
      schueler.Data.AnschriftStrasse = textBoxStrasse.Text;
      schueler.Data.AnschriftPLZ = textBoxPLZ.Text;
      schueler.Data.AnschriftOrt = textBoxOrt.Text;
      schueler.Data.AnschriftTelefonnummer = textBoxTelefonnummer.Text;
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
