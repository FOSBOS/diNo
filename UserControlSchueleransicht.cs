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
      SchuelerverwaltungController.InitDateTimePicker(this.dateTimeProbezeit);
      SchuelerverwaltungController.InitDateTimePicker(this.dateTimeAustritt);
      btnSave.Visible = Zugriff.Instance.HatVerwaltungsrechte;
      panelSekretariat.Visible = btnSave.Visible;
      btnResetProbezeit.Visible = btnSave.Visible;
      labelAustrittHinweis.Visible = btnSave.Visible;
      pnlAdmin.Visible = Zugriff.Instance.HatRolle(Rolle.Admin);
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
          checkBoxLegasthenie.Checked = schueler.IsLegastheniker;
          
          //this.dateTimePicker1.Value = !schueler.Data.IsAustrittsdatumNull() ? schueler.Data.Austrittsdatum : this.dateTimePicker1.MinDate;
          dateTimeProbezeit.Value = schueler.Data.IsProbezeitBisNull() ? dateTimeProbezeit.MinDate : schueler.Data.ProbezeitBis;          
          dateTimeAustritt.Value = schueler.Data.IsAustrittsdatumNull() ? dateTimeAustritt.MinDate : schueler.Data.Austrittsdatum;
          textBoxEmail.Text = schueler.Data.Email;
          cbStatus.SelectedIndex = schueler.Data.Status;
          textBoxZeugnisbemerkung.Text = schueler.Data.IsZeugnisbemerkungNull() ? "" : schueler.Data.Zeugnisbemerkung;
          textBoxDNote.Text = schueler.Data.IsDNoteNull() ? "" : string.Format("{0:F1}", schueler.Data.DNote);
          textBoxDNoteAllg.Text = schueler.Data.IsDNoteAllgNull() ? "" : string.Format("{0:F1}", schueler.Data.DNoteAllg);
          numAndereFremdspr2Note.Value = schueler.Data.IsAndereFremdspr2NoteNull() ? null : (decimal?) schueler.Data.AndereFremdspr2Note;
          textBoxAndereFremdspr2Text.Text = schueler.Data.IsAndereFremdspr2TextNull() ? "" : schueler.Data.AndereFremdspr2Text;

          textBoxNachname.Text = schueler.Data.Name;
          textBoxVorname.Text = schueler.Data.Vorname;
          textBoxRufname.Text = schueler.Data.Rufname;
          textBoxAR.Text = schueler.Data.Ausbildungsrichtung;
        }
        /*
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
          dateTimeProbezeit.Text = "";
          textBoxEmail.Text = "";
          //pictureBoxImage.Image = null;
        }*/
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

      schueler.IsLegastheniker = checkBoxLegasthenie.Checked;

      if (dateTimeProbezeit.Value==dateTimeProbezeit.MinDate) schueler.Data.SetProbezeitBisNull();
        else schueler.Data.ProbezeitBis = dateTimeProbezeit.Value;
      if (dateTimeAustritt.Value==dateTimeAustritt.MinDate) schueler.Data.SetAustrittsdatumNull();
        else schueler.Data.Austrittsdatum = dateTimeAustritt.Value;

      schueler.Data.Email = textBoxEmail.Text;
      schueler.Data.Status = cbStatus.SelectedIndex;
      if (numAndereFremdspr2Note.Value==null) schueler.Data.SetAndereFremdspr2NoteNull();
        else schueler.Data.AndereFremdspr2Note = (int) numAndereFremdspr2Note.Value.GetValueOrDefault();            
      if (textBoxAndereFremdspr2Text.Text=="") schueler.Data.SetAndereFremdspr2TextNull();
        else schueler.Data.AndereFremdspr2Text = textBoxAndereFremdspr2Text.Text;

      if (textBoxZeugnisbemerkung.Text == "") schueler.Data.SetZeugnisbemerkungNull();
      else schueler.Data.Zeugnisbemerkung = textBoxZeugnisbemerkung.Text;
    
      schueler.Data.Name = textBoxNachname.Text;
      schueler.Data.Vorname = textBoxVorname.Text;
      schueler.Data.Rufname = textBoxRufname.Text;
      schueler.Data.Ausbildungsrichtung = textBoxAR.Text;

      schueler.Save();
    }

    private void dateTimeAustritt_ValueChanged(object sender, EventArgs e)
    {
      if (dateTimeAustritt.Value!=dateTimeAustritt.MinDate) cbStatus.SelectedIndex=1; // abgemeldet
        else cbStatus.SelectedIndex=0; // aktiv

    }
    
    private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbStatus.SelectedIndex==0)
        dateTimeAustritt.Value = dateTimeAustritt.MinDate;
      else if (cbStatus.SelectedIndex==1 && dateTimeAustritt.Value == dateTimeAustritt.MinDate) // nur setzen wenn nicht eh schon ein Datum drinsteht
        dateTimeAustritt.Value = DateTime.Today;
    }

    private void buttonResetProbezeit_Click(object sender, EventArgs e)
    {
      dateTimeProbezeit.Value = dateTimeProbezeit.MinDate;
    }
  }
}
