using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace diNo
{
  public partial class Brief : Form
  {
    private Schueler s;
    private Klassenansicht frmKlasse;
    private BriefDaten b;
    public Brief(Klassenansicht aufrufendesFormular)
    {
      frmKlasse = aufrufendesFormular;
      InitializeComponent();
      ControlBox = false;
      radioButton_CheckedChanged(this, null);
      datVersaeumtAm.Value = DateTime.Today;
      datTermin.Value = DateTime.Today;
      foreach (var f in Zugriff.Instance.eigeneFaecher)
      {
        cbFach.Items.Add(f.Bezeichnung);
      }
      //cbFach.SelectedIndex = 0;
      //opVerschVerweis.Enabled = Zugriff.Instance.HatRolle(Rolle.Schulleitung);      
    }

    public void Anzeigen(Schueler schueler)
    {
      s = schueler;
      opAttestpflicht.Enabled = (s.getKlasse.KlassenleiterId == Zugriff.Instance.lehrer.Id) || Zugriff.Instance.HatVerwaltungsrechte;
      if (!opAttestpflicht.Enabled && opAttestpflicht.Checked) opSA.Checked = true;
      Show();
    }

    private void btnEsc_Click(object sender, EventArgs e)
    {
      Hide();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      BriefTyp typ=BriefTyp.Standard;
      if (opVerweis.Checked || opVerschVerweis.Checked) typ = BriefTyp.Verweis;
      else if (opMEP.Checked || opSEP.Checked) typ = BriefTyp.Ersatzpruefung;
      else if (opAttestpflicht.Checked) typ = BriefTyp.Attestpflicht;
      b = new BriefDaten(s, typ);
      if (typ == BriefTyp.Verweis) VerweisText(opVerschVerweis.Checked);
      else if (opSA.Checked || opKA.Checked) NachterminText();
      else if (typ == BriefTyp.Ersatzpruefung) ErsatzprText();
      else if (typ == BriefTyp.Attestpflicht) AttestpflichtText();
      else NacharbeitText();

      Hide();
      new ReportBrief(b).Show();

      if ((opVerweis.Checked || opVerschVerweis.Checked) && MessageBox.Show("Soll der Verweis auch in den Notenbogen eingetragen werden?", "diNo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        if (opVerschVerweis.Checked) s.AddVorkommnis(Vorkommnisart.verschaerfterVerweis, edInhalt.Text, true);
        else s.AddVorkommnis(Vorkommnisart.Verweis, edInhalt.Text, true);
        frmKlasse.RefreshVorkommnisse();
      }
      if (opNacharbeit.Checked && MessageBox.Show("Soll die Nacharbeit auch in den Notenbogen eingetragen werden?", "diNo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        s.AddVorkommnis(Vorkommnisart.Nacharbeit, edInhalt.Text, true);
        frmKlasse.RefreshVorkommnisse();
      }
    }


    private void radioButton_CheckedChanged(object sender, EventArgs e)
    {
      pnlVersaeumtAm.Enabled = opSA.Checked || opKA.Checked;
      pnlNachterminAm.Enabled = !(opVerweis.Checked || opVerschVerweis.Checked || opAttestpflicht.Checked);
      pnlInhalt.Enabled = opVerweis.Checked || opVerschVerweis.Checked || opNacharbeit.Checked || opSEP.Checked || opMEP.Checked;
      labelInhalt.Text = (opSEP.Checked || opMEP.Checked) ? "Prüfungsstoff" : "Grund";
    }

    public string erzeugeRaum()
    {
      if (edRaum.Text == "") return "";
      else return " in Raum " + edRaum.Text;
    }

    public void VerweisText(bool verschaerft)
    {
      if (verschaerft)
      {
        b.Betreff = "Verschärfter Verweis";
        b.Unterschrift = Zugriff.Instance.getString(GlobaleStrings.Schulleiter) + "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulleiterText);
      }
      else
      {
        b.Betreff = "Verweis";
        b.Unterschrift2 = Zugriff.Instance.getString(GlobaleStrings.Schulleiter) + ", " + Zugriff.Instance.getString(GlobaleStrings.SchulleiterText);
      }
      b.Inhalt = "Hiermit wird " + s.getHerrnFrau() + s.VornameName + " gemäß Art. 86 (2) BayEUG ein ";
      b.Inhalt += (verschaerft ? "verschärfter Verweis durch die Schulleitung" : "Verweis") + " erteilt.<br><br>";
      b.Inhalt += "Begründung der Ordnungsmaßnahme:<br>" + edInhalt.Text + "<br><br>";
    }

    public void NachterminText()
    {
      string lnwart = opSA.Checked ? "Schulaufgabe" : "Kurzarbeit";
      b.Betreff = "Versäumnis einer " + lnwart;
      b.Inhalt += "Sie haben die " + lnwart + " im Fach " + cbFach.Text + " am " + datVersaeumtAm.Text + " versäumt.<br>";

      b.Inhalt += "Nach § 20 (1) FOBOSO wird Ihnen ein Nachtermin eingeräumt.<br><br>";
      b.Inhalt += "Der Nachtermin findet statt am " + datTermin.Text + " um " + datZeit.Text + " Uhr" + erzeugeRaum() + ".<br><br>";
      b.Inhalt += "Wird dieser Termin ohne ausreichende Entschuldigung versäumt, wird die Note 6 (0 Punkte) erteilt.<br><br>";
      b.Inhalt += "Freundliche Grüße";
    }
    public void ErsatzprText()
    {
      string lnwart = opSEP.Checked ? "schriftliche" : "mündliche";
      b.Betreff = "Nachholung von Leistungsnachweisen";
      if (b.IstU18) b.Inhalt += (s.Data.Geschlecht == "M" ? "Ihr Sohn " : "Ihre Tochter ") + s.benutzterVorname
        + " konnte in diesem Schuljahr im Fach " + cbFach.Text + " wegen " + (s.Data.Geschlecht == "M" ? "seiner " : "ihrer ");
      else 
        b.Inhalt += "Sie konnten in diesem Schuljahr im Fach " + cbFach.Text + " wegen Ihrer ";
      b.Inhalt += "Versäumnisse nicht hinreichend geprüft werden.<br><br>Gemäß § 20 (2) FOBOSO wird hiermit eine " + lnwart + " Ersatzprüfung angesetzt.<br><br>";
      b.Inhalt += "Prüfungsstoff wird sein: <br>" + edInhalt.Text + "<br><br>";
      b.Inhalt += "Die " + lnwart + " Ersatzprüfung findet statt am " + datTermin.Text + " um " + datZeit.Text + " Uhr" + erzeugeRaum() + ".<br><br>";
      b.Inhalt += "Wird an der Ersatzprüfung wegen Erkrankung nicht teilgenommen, so muss die Erkrankung durch ärztliches Attest nachgewiesen werden. " +
        "In diesem Fall gilt nach § 21 (1) FOBOSO die Halbjahresleistung als nicht erbracht und mindert eines Ihrer Streichergebnisse. " +
        "Ohne ausreichende Entschuldigung wird die Note 6 (0 Punkte) erteilt.<br><br>";
      b.Inhalt += "Freundliche Grüße";
    }

    public void NacharbeitText()
    {
      b.Betreff = "Nacharbeit";
      b.Inhalt += "hiermit werden Sie gemäß Art. 86 (1) BayEUG zur Nacharbeit verpflichtet.<br><br>";
      b.Inhalt += "Begründung:<br>" + edInhalt.Text;
      b.Inhalt += "<br><br>Die Nacharbeit findet statt am " + datTermin.Text + " um " + datZeit.Text + " Uhr" + erzeugeRaum() + ".<br><br>";
      b.Inhalt += "Wird die Nacharbeit wegen Erkrankung nicht ausgeführt, so muss die Erkrankung durch ärztliches Attest nachgewiesen werden.<br><br>";
      b.Inhalt += "Freundliche Grüße";
    }

    public void AttestpflichtText()
    {      
      b.Betreff = "Attestpflicht";
      b.Inhalt += "da sich im laufenden Schuljahr bei ";
      if (b.IstU18) b.Inhalt += (s.Data.Geschlecht == "M" ? "Ihrem Sohn " : "Ihrer Tochter ") + s.VornameName;
      else b.Inhalt += "Ihnen";
      b.Inhalt += " die krankheitsbedingten Schulversäumnisse häufen, werden Sie gemäß § 20 (2) BaySchO dazu verpflichtet, künftig jede weitere krankheitsbedingte Abwesenheit ";
      b.Inhalt += "durch ein aktuelles ärztliches Zeugnis (Schulunfähigkeitsbescheinigung) zu belegen.<br><br>";
      b.Inhalt += "Wird das Zeugnis nicht unverzüglich vorgelegt, so gilt das Fernbleiben als unentschuldigt.";
      //b.Unterschrift2 = Zugriff.Instance.getString(GlobaleStrings.Schulleiter) + "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulleiterText);

      s.AddVorkommnis(Vorkommnisart.Attestpflicht, "", false);
    }
  }

  public class BriefDaten
  {
    public int Id { get; private set; }
    public string Absender { get; private set; }
    public string Absenderzeile { get; private set; }
    public string Telefon { get; private set; }
    public string Adressfeld { get; set; }
    public string Name { get; set; }
    public string VornameName { get; set; }
    public string Klasse { get; set; }
    public string Betreff { get; set; }
    public string Inhalt { get; set; }
    public string Inhalt2 { get; set; }
    public string Unterschrift { get; set; }
    public string Unterschrift2 { get; set; }
    public string UnterschriftsText { get; set; }
    public bool IstU18 { get; set; }
    public string OrtDatum { get; set; }
    public string Logo { get; private set; }

    public BriefDaten(Schueler s, BriefTyp typ)
    {
      Lehrer lehrer;
      IstU18 = s.Alter() < 18 && (typ != BriefTyp.Standard); // Schüleradresse bei normalen Nachterminen
      bool UnterschriftKL = typ == BriefTyp.Gefaehrdung || typ == BriefTyp.Attestpflicht; // hier nicht der angemeldete Benutzer

      Id = s.Id;
      Absenderzeile = Zugriff.Instance.getString(GlobaleStrings.SchulAbsenderzeile);
      Absender = Zugriff.Instance.getString(GlobaleStrings.SchulName);
      if (Zugriff.Instance.getString(GlobaleStrings.SchulNameZusatz) != "") Absender += "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulNameZusatz);
      Absender += "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulStrasse) + "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulPLZ) + " " + Zugriff.Instance.getString(GlobaleStrings.SchulOrt);
      Telefon = "Telefon: " + Zugriff.Instance.getString(GlobaleStrings.SchulTel) + "\nTelefax: " + Zugriff.Instance.getString(GlobaleStrings.SchulFax);
      Telefon += "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulWeb) + "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulMail);

      Adressfeld = s.ErzeugeAdresse(IstU18); 
      Name = s.Name;
      VornameName = s.VornameName;
      Klasse = s.getKlasse.Bezeichnung;
      OrtDatum = Zugriff.Instance.getString(GlobaleStrings.SchulOrt) + ", den " + DateTime.Today.ToString("dd.MM.yyyy");

      if (UnterschriftKL)
      {
        lehrer = s.getKlasse.Klassenleiter;        
      }
      else
        lehrer = Zugriff.Instance.lehrer;

      Unterschrift = lehrer.NameDienstbezeichnung;
      if (UnterschriftKL)
        Unterschrift += "\n" + lehrer.KLString;

      if (IstU18)
        UnterschriftsText = "Unterschrift eines Erziehungsberechtigten";
      else
        UnterschriftsText = "Unterschrift";

      Inhalt = s.ErzeugeAnrede(IstU18);
      try
      {
        string verz = Directory.GetCurrentDirectory() + "\\Logo\\Logo.png";
        Logo = ConvertImageToBase64(Image.FromFile(verz), ImageFormat.Png);
      }
      catch
      {
        Logo = "";
      }
    }

    private string ConvertImageToBase64(Image image, ImageFormat format)
    {
      byte[] imageArray;

      using (System.IO.MemoryStream imageStream = new System.IO.MemoryStream())
      {
        image.Save(imageStream, format);
        imageArray = new byte[imageStream.Length];
        imageStream.Seek(0, System.IO.SeekOrigin.Begin);
        imageStream.Read(imageArray, 0, (int)imageStream.Length);
      }

      return Convert.ToBase64String(imageArray);
    }
  }

  public enum BriefTyp 
  {
    Standard = 0,
    Verweis = 1,
    Ersatzpruefung = 2,
    Gefaehrdung = 3,
    Attestpflicht = 4
  }

}
