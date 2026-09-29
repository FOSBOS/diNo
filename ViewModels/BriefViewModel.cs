using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von Brief.cs (WinForms). Statt einer Referenz auf das Hauptfenster
  // bekommt dieses ViewModel einen Refresh-Callback übergeben (siehe KlassenansichtViewModel).
  public partial class BriefViewModel : ObservableObject
  {
    private Schueler s;
    private BriefDaten b;
    private readonly Action onRefreshVorkommnisse;

    public List<string> FachOptions { get; }

    [ObservableProperty] private bool opSA = true;
    [ObservableProperty] private bool opKA;
    [ObservableProperty] private bool opSEP;
    [ObservableProperty] private bool opMEP;
    [ObservableProperty] private bool opVerweis;
    [ObservableProperty] private bool opNacharbeit;
    [ObservableProperty] private bool opVerschVerweis;
    [ObservableProperty] private bool opAttestpflicht;
    [ObservableProperty] private bool opMitteilung;

    [ObservableProperty] private bool isAttestpflichtEnabled = true;
    [ObservableProperty] private bool isVersaeumtAmEnabled;
    [ObservableProperty] private bool isNachterminAmEnabled;
    [ObservableProperty] private bool isInhaltEnabled;
    [ObservableProperty] private string inhaltLabel = "Grund";

    [ObservableProperty] private DateTime versaeumtAm = DateTime.Today;
    [ObservableProperty] private DateTime termin = DateTime.Today;
    [ObservableProperty] private string zeitText = "13:30";
    [ObservableProperty] private string raum = "";
    [ObservableProperty] private string selectedFach;
    [ObservableProperty] private string inhalt = "";

    public event EventHandler RequestShow;
    public event EventHandler RequestHide;

    public BriefViewModel(Action onRefreshVorkommnisse)
    {
      this.onRefreshVorkommnisse = onRefreshVorkommnisse;
      FachOptions = Zugriff.Instance.eigeneFaecher.Select(f => f.Bezeichnung).ToList();
      RecomputePanels();
    }

    partial void OnOpSAChanged(bool value) => RecomputePanels();
    partial void OnOpKAChanged(bool value) => RecomputePanels();
    partial void OnOpSEPChanged(bool value) => RecomputePanels();
    partial void OnOpMEPChanged(bool value) => RecomputePanels();
    partial void OnOpVerweisChanged(bool value) => RecomputePanels();
    partial void OnOpNacharbeitChanged(bool value) => RecomputePanels();
    partial void OnOpVerschVerweisChanged(bool value) => RecomputePanels();
    partial void OnOpAttestpflichtChanged(bool value) => RecomputePanels();
    partial void OnOpMitteilungChanged(bool value) => RecomputePanels();

    private void RecomputePanels()
    {
      IsVersaeumtAmEnabled = OpSA || OpKA;
      IsNachterminAmEnabled = !(OpVerweis || OpVerschVerweis || OpAttestpflicht || OpMitteilung);
      IsInhaltEnabled = OpVerweis || OpVerschVerweis || OpNacharbeit || OpSEP || OpMEP || OpMitteilung;
      InhaltLabel = (OpSEP || OpMEP) ? "Prüfungsstoff" : "Grund";
    }

    public void Anzeigen(Schueler schueler)
    {
      s = schueler;
      IsAttestpflichtEnabled = (s.getKlasse.KlassenleiterId == Zugriff.Instance.lehrer.Id) || Zugriff.Instance.HatVerwaltungsrechte;
      if (!IsAttestpflichtEnabled && OpAttestpflicht) OpSA = true;
      RequestShow?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Esc() => RequestHide?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void Ok()
    {
      BriefTyp typ = BriefTyp.Standard;
      if (OpVerweis || OpVerschVerweis) typ = BriefTyp.Verweis;
      else if (OpMEP || OpSEP) typ = BriefTyp.Ersatzpruefung;
      else if (OpAttestpflicht) typ = BriefTyp.Attestpflicht;
      else if (OpMitteilung) typ = BriefTyp.Mitteilung;
      b = new BriefDaten(s, typ);
      if (typ == BriefTyp.Verweis) VerweisText(OpVerschVerweis);
      else if (OpSA || OpKA) NachterminText();
      else if (typ == BriefTyp.Ersatzpruefung) ErsatzprText();
      else if (typ == BriefTyp.Attestpflicht) AttestpflichtText();
      else if (typ == BriefTyp.Mitteilung) MitteilungText();
      else NacharbeitText();

      RequestHide?.Invoke(this, EventArgs.Empty);
      new ReportBrief(b).Show();

      if ((OpVerweis || OpVerschVerweis) && MessageBox.Show("Soll der Verweis auch in den Notenbogen eingetragen werden?", "diNo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
      {
        if (OpVerschVerweis) s.AddVorkommnis(Vorkommnisart.verschaerfterVerweis, Inhalt, true);
        else s.AddVorkommnis(Vorkommnisart.Verweis, Inhalt, true);
        onRefreshVorkommnisse?.Invoke();
      }
      if (OpNacharbeit && MessageBox.Show("Soll die Nacharbeit auch in den Notenbogen eingetragen werden?", "diNo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
      {
        s.AddVorkommnis(Vorkommnisart.Nacharbeit, Inhalt, true);
        onRefreshVorkommnisse?.Invoke();
      }
    }

    private string erzeugeRaum() => Raum == "" ? "" : " in Raum " + Raum;
    private string TerminText() => Termin.ToString("dd.MM.yyyy");
    private string VersaeumtAmText() => VersaeumtAm.ToString("dd.MM.yyyy");

    private void VerweisText(bool verschaerft)
    {
      if (verschaerft)
      {
        b.Betreff = "Verschärfter Verweis";
        b.Unterschrift = Zugriff.Instance.getString(GlobaleStrings.Schulleiter) + "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulleiterText);
      }
      else
      {
        b.Betreff = "Verweis";
        if (b.Unterschrift.Contains("OStD"))
          b.Unterschrift += "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulleiterText);
        else
          b.Unterschrift2 = Zugriff.Instance.getString(GlobaleStrings.Schulleiter) + ", " + Zugriff.Instance.getString(GlobaleStrings.SchulleiterText);
      }
      b.Inhalt = "Hiermit wird " + s.getHerrnFrau() + s.VornameName + " gemäß Art. 86 (2) BayEUG ein ";
      b.Inhalt += (verschaerft ? "verschärfter Verweis durch die Schulleitung" : "Verweis") + " erteilt.<br><br>";
      b.Inhalt += "Begründung der Ordnungsmaßnahme:<br>" + Inhalt + "<br><br>";
    }

    private void NachterminText()
    {
      string lnwart = OpSA ? "Schulaufgabe" : "Kurzarbeit";
      b.Betreff = "Versäumnis einer " + lnwart;
      b.Inhalt += "Sie haben die " + lnwart + " im Fach " + SelectedFach + " am " + VersaeumtAmText() + " versäumt.<br>";

      b.Inhalt += "Nach § 20 (1) FOBOSO wird Ihnen ein Nachtermin eingeräumt.<br><br>";
      b.Inhalt += "Der Nachtermin findet statt am " + TerminText() + " um " + ZeitText + " Uhr" + erzeugeRaum() + ".<br><br>";
      b.Inhalt += "Wird dieser Termin ohne ausreichende Entschuldigung versäumt, wird die Note 6 (0 Punkte) erteilt.<br><br>";
      b.Inhalt += "Freundliche Grüße";
    }

    private void ErsatzprText()
    {
      string lnwart = OpSEP ? "schriftliche" : "mündliche";
      b.Betreff = "Nachholung von Leistungsnachweisen";
      if (b.IstU18) b.Inhalt += s.getIhrSohn() + " " + s.benutzterVorname
        + " konnte in diesem Schuljahr im Fach " + SelectedFach + " wegen " + (s.Data.Geschlecht != "W" ? "seiner " : "ihrer ");
      else
        b.Inhalt += "Sie konnten in diesem Schuljahr im Fach " + SelectedFach + " wegen Ihrer ";
      b.Inhalt += "Versäumnisse nicht hinreichend geprüft werden.<br><br>Gemäß § 20 (2) FOBOSO wird hiermit eine " + lnwart + " Ersatzprüfung angesetzt.<br><br>";
      b.Inhalt += "Prüfungsstoff wird sein: <br>" + Inhalt + "<br><br>";
      b.Inhalt += "Die " + lnwart + " Ersatzprüfung findet statt am " + TerminText() + " um " + ZeitText + " Uhr" + erzeugeRaum() + ".<br><br>";
      b.Inhalt += "Wird an der Ersatzprüfung wegen Erkrankung nicht teilgenommen, so muss die Erkrankung durch ärztliches Attest nachgewiesen werden. " +
        "In diesem Fall gilt nach § 21 (1) FOBOSO die Halbjahresleistung als nicht erbracht und mindert eines Ihrer Streichergebnisse. " +
        "Ohne ausreichende Entschuldigung wird die Note 6 (0 Punkte) erteilt.<br><br>";
      b.Inhalt += "Freundliche Grüße";
    }

    private void NacharbeitText()
    {
      b.Betreff = "Nacharbeit";
      b.Inhalt += "hiermit werden Sie gemäß Art. 86 (1) BayEUG zur Nacharbeit verpflichtet.<br><br>";
      b.Inhalt += "Begründung:<br>" + Inhalt;
      b.Inhalt += "<br><br>Die Nacharbeit findet statt am " + TerminText() + " um " + ZeitText + " Uhr" + erzeugeRaum() + ".<br><br>";
      b.Inhalt += "Wird die Nacharbeit wegen Erkrankung nicht ausgeführt, so muss die Erkrankung durch ärztliches Attest nachgewiesen werden.<br><br>";
      b.Inhalt += "Freundliche Grüße";
    }

    private void AttestpflichtText()
    {
      b.Betreff = "Attestpflicht";
      b.Inhalt += "da sich im laufenden Schuljahr bei ";
      if (b.IstU18) b.Inhalt += s.getIhrSohn(3) + " " + s.VornameName;
      else b.Inhalt += "Ihnen";
      b.Inhalt += " die krankheitsbedingten Schulversäumnisse häufen, werden Sie gemäß § 20 (2) BaySchO dazu verpflichtet, künftig jede weitere krankheitsbedingte Abwesenheit ";
      b.Inhalt += "durch ein aktuelles ärztliches Zeugnis (Schulunfähigkeitsbescheinigung) zu belegen.<br><br>";
      b.Inhalt += "Wird das Zeugnis nicht unverzüglich vorgelegt, so gilt das Fernbleiben als unentschuldigt.";

      s.AddVorkommnis(Vorkommnisart.Attestpflicht, "", false);
    }

    private void MitteilungText()
    {
      b.Betreff = s.getIhrSohn() + " " + s.VornameName;
      b.Inhalt += Inhalt + "<br><br>Freundliche Grüße";
    }
  }
}
