using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von NotenCheckForm.cs (WinForms).
  public partial class NotenCheckViewModel : ObservableObject
  {
    private readonly List<Klasse> selObj;

    public List<KeyValuePair<NotenCheckModus, string>> ModusOptions { get; }

    public List<string> ZeitpunktOptions { get; } = new List<string>
    {
      "Probezeit BOS", "Halbjahr", "Zulassung Abitur", "SAP", "MAP", "Jahresende"
    };

    [ObservableProperty] private NotenCheckModus selectedModus;
    [ObservableProperty] private int selectedZeitpunktIndex;
    [ObservableProperty] private bool isZeitpunktEnabled;
    [ObservableProperty] private bool isKurzfassung;
    [ObservableProperty] private string statusText = "";
    [ObservableProperty] private int progressMax;
    [ObservableProperty] private int progressValue;

    public event EventHandler RequestClose;

    public NotenCheckViewModel(List<Klasse> obj)
    {
      selObj = obj;

      var dict = new Dictionary<NotenCheckModus, string>();
      dict.Add(NotenCheckModus.EigeneNotenVollstaendigkeit, "eigene Noten vollständig?");
      if (Zugriff.Instance.lehrer.KlassenleiterVon != null)
        dict.Add(NotenCheckModus.EigeneKlasse, "eigene Klasse prüfen");

      dict.Add(NotenCheckModus.Gesamtpruefung, "Gesamtprüfung");
      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin))
      {
        dict.Add(NotenCheckModus.KonferenzVorbereiten, "Konferenz vorbereiten");
        dict.Add(NotenCheckModus.Protokolle, "Protokolle Klassenkonferenz");
      }
      ModusOptions = dict.ToList();
      if (ModusOptions.Count > 0) SelectedModus = ModusOptions[0].Key;

      SelectedZeitpunktIndex = Zugriff.Instance.aktZeitpunkt - 1;
      IsZeitpunktEnabled = Zugriff.Instance.HatVerwaltungsrechte || Zugriff.Instance.HatRolle(Rolle.Schulleitung);
    }

    private Zeitpunkt GetZeitpunkt() => (Zeitpunkt)(SelectedZeitpunktIndex + 1);

    // WPF-Ersatz für das blockierende Form.Refresh() im Original: pumpt die
    // Dispatcher-Queue, damit der Statustext während der Prüf-Schleife sichtbar aktualisiert wird.
    private static void PumpUi()
    {
      Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
    }

    [RelayCommand]
    private void Start()
    {
      ProgressValue = 0;
      var contr = new NotenCheckController(GetZeitpunkt(), SelectedModus, IsKurzfassung, () => ProgressValue++, selObj);
      ProgressMax = contr.AnzahlSchueler;
      if (contr.zuPruefendeKlassen.Count == 0)
      {
        MessageBox.Show("Diese Klasse muss zu diesem Zeitpunkt nicht geprüft werden.", "diNo", MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }

      foreach (var k in contr.zuPruefendeKlassen)
      {
        StatusText = "Prüfe Klasse " + k.Bezeichnung;
        PumpUi();
        contr.CheckKlasse(k);
      }

      RequestClose?.Invoke(this, EventArgs.Empty);
      contr.ShowResults();

      if (SelectedModus == NotenCheckModus.KonferenzVorbereiten)
        Zugriff.Instance.Refresh();
    }
  }
}
