using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von UserControlNotenbogen.cs (WinForms). Die Spalten-Sichtbarkeits-Umschaltung
  // (chkShowHj1/2/Abi) bleibt aus WPF-DataGrid-Gründen imperativer Code-Behind in NotenbogenView
  // (DataGridColumn ist nicht Teil des Visual Trees und daher nicht direkt bindbar) - wie im
  // WinForms-Original war das ohnehin schon Code-Behind-Logik (ShowCols/ShowFixedCols).
  public partial class NotenbogenViewModel : ObservableObject
  {
    private Schueler schueler;

    public bool IsAdminEdit => Zugriff.Instance.HatRolle(Rolle.Admin);
    public bool HatVerwaltungsrechte => Zugriff.Instance.HatVerwaltungsrechte;

    public ObservableCollection<NotenRow> Rows { get; } = new ObservableCollection<NotenRow>();
    public ObservableCollection<PunktesummeRow> PunktesummeRows { get; } = new ObservableCollection<PunktesummeRow>();

    [ObservableProperty] private System.Windows.Media.Brush gridBackground = System.Windows.Media.Brushes.White;
    [ObservableProperty] private bool showPunktesumme;
    [ObservableProperty] private bool showHinweise;
    [ObservableProperty] private int berechnungsstatusIndex;
    [ObservableProperty] private string dNoteText = "";
    [ObservableProperty] private string dNoteFachgebHSRText = "";

    public void SetSchueler(Schueler value)
    {
      schueler = value;
      Init();
    }

    public void Init()
    {
      if (schueler == null) return;

      Rows.Clear();
      GridBackground = schueler.Status == Schuelerstatus.Abgemeldet
        ? System.Windows.Media.Brushes.LightGray
        : System.Windows.Media.Brushes.White;

      bool show11 = schueler.hatVorHj;
      Jahrgangsstufe jg = schueler.getKlasse.Jahrgangsstufe;

      foreach (var fach1 in schueler.getNoten.alleFaecher)
      {
        var row = new NotenRow { FachText = fach1.getFach.Bezeichnung };

        if (show11)
        {
          row.Zelle11_1 = MakeZelle(fach1.getVorHjLeistung(HjArt.Hj1), fach1.getFach, HjArt.Hj1, Jahrgangsstufe.Elf);
          row.Zelle11_2 = MakeZelle(fach1.getVorHjLeistung(HjArt.Hj2), fach1.getFach, HjArt.Hj2, Jahrgangsstufe.Elf);
        }

        row.SaHj1 = fach1.SA(Halbjahr.Erstes);
        row.SlHj1 = fach1.sL(Halbjahr.Erstes);

        HjLeistung hjl = fach1.getHjLeistung(HjArt.Hj1);
        if (hjl != null)
        {
          if (hjl.SchnittMdl != null) row.SchnittMdl1 = hjl.SchnittMdl.GetValueOrDefault().ToString(CultureInfo.CurrentCulture);
          row.Punkte2DezHj1 = hjl.Punkte2Dez.GetValueOrDefault().ToString(CultureInfo.CurrentCulture);
        }
        row.ZelleHj1 = MakeZelle(hjl, fach1.getFach, HjArt.Hj1, jg);

        row.SaHj2 = fach1.SA(Halbjahr.Zweites);
        row.SlHj2 = fach1.sL(Halbjahr.Zweites);

        hjl = fach1.getHjLeistung(HjArt.Hj2);
        if (hjl != null)
        {
          if (hjl.SchnittMdl != null) row.SchnittMdl2 = hjl.SchnittMdl.GetValueOrDefault().ToString(CultureInfo.CurrentCulture);
          row.Punkte2DezHj2 = hjl.Punkte2Dez.GetValueOrDefault().ToString(CultureInfo.CurrentCulture);
        }
        row.ZelleHj2 = MakeZelle(hjl, fach1.getFach, HjArt.Hj2, jg);

        row.SAP = fach1.ToString(Halbjahr.Zweites, Notentyp.APSchriftlich);
        row.MAP = fach1.ToString(Halbjahr.Zweites, Notentyp.APMuendlich);
        row.ZelleAPG = MakeZelle(fach1.getHjLeistung(HjArt.AP), fach1.getFach, HjArt.AP, jg);

        row.ZelleJN = MakeZelle(fach1.getHjLeistung(HjArt.JN), fach1.getFach, HjArt.JN, jg);
        row.ZelleGE = MakeZelle(fach1.getHjLeistung(HjArt.GesErg), fach1.getFach, null, jg);

        Rows.Add(row);
      }

      ShowPunktesumme = schueler.getKlasse.Jahrgangsstufe > Jahrgangsstufe.Elf &&
        (Zugriff.Instance.aktZeitpunkt >= (int)Zeitpunkt.ErstePA || Zugriff.Instance.HatRolle(Rolle.Admin));
      PunktesummeRows.Clear();
      if (ShowPunktesumme)
      {
        Punktesumme p = schueler.punktesumme;
        foreach (PunktesummeArt a in Enum.GetValues(typeof(PunktesummeArt)))
        {
          if (p.Anzahl(a) > 0)
            PunktesummeRows.Add(new PunktesummeRow { Text = Punktesumme.ArtToText(a), Summe = p.Summe(a), Anzahl = p.Anzahl(a) });
        }
      }

      ShowHinweise = schueler.Data.Berechungsstatus > (byte)Berechnungsstatus.Unberechnet
        && schueler.punktesumme.Anzahl(PunktesummeArt.HjLeistungen) != schueler.GetAnzahlEinbringung();
      BerechnungsstatusIndex = schueler.Data.Berechungsstatus;
      DNoteText = schueler.Data.IsDNoteNull() ? "" : string.Format("{0:F1}", schueler.Data.DNote);
      DNoteFachgebHSRText = schueler.Data.IsDNoteFachgebHSRNull() ? "" : string.Format("{0:F1}", schueler.Data.DNoteFachgebHSR);
    }

    private NotenZelle MakeZelle(HjLeistung hjl, Fach fach, HjArt? neuanlageArt, Jahrgangsstufe jg)
    {
      var zelle = new NotenZelle(OnZelleDoubleClick, OnSetStatus)
      {
        Fach = fach,
        NeuanlageArt = neuanlageArt,
        Jahrgangsstufe = jg,
        HjLeistung = hjl
      };
      if (hjl != null)
      {
        zelle.Text = hjl.Punkte.ToString();
        zelle.Background = NotenZelle.ToBrush(hjl.GetBackgroundColor());
      }
      return zelle;
    }

    private void OnZelleDoubleClick(NotenZelle zelle)
    {
      if (!IsAdminEdit) return;

      if (zelle.HjLeistung == null)
      {
        if (zelle.NeuanlageArt == null) return;
        var hj = new HjLeistung(schueler.Id, zelle.Fach, zelle.NeuanlageArt.Value, zelle.Jahrgangsstufe);
        EditHjLeistung(hj);
        schueler.ReloadNoten();
      }
      else
        EditHjLeistung(zelle.HjLeistung);

      Init();
    }

    private static void EditHjLeistung(HjLeistung hj)
    {
      string input = diNo.Views.InputBoxWindow.Show("Neue Notenpunkte:", hj.Punkte.ToString());
      if (input != "")
      {
        hj.Punkte = Convert.ToByte(input, CultureInfo.CurrentUICulture);
        hj.WriteToDB();
      }
    }

    private void OnSetStatus(NotenZelle zelle, HjStatus status)
    {
      if (zelle.HjLeistung == null || !HatVerwaltungsrechte) return;

      zelle.HjLeistung.SetStatus(status);
      var berechnungen = new Berechnungen();
      berechnungen.AktualisiereGE(schueler);
      Init();
    }
  }
}
