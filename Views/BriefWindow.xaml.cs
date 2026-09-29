using System;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von Brief.cs (WinForms). Wird wie das Original einmal erzeugt
  // und über Anzeigen(Schueler) für verschiedene Schüler wiederverwendet (Show/Hide
  // statt Neuerzeugung).
  public partial class BriefWindow : System.Windows.Window
  {
    private readonly BriefViewModel vm;

    public BriefWindow(Action onRefreshVorkommnisse)
    {
      InitializeComponent();
      vm = new BriefViewModel(onRefreshVorkommnisse);
      vm.RequestShow += (s, e) =>
      {
        Show();
        // Direkt nach dem ersten Show() existiert das Fensterhandle noch nicht vollständig,
        // ein sofortiges Activate() geht dann ins Leere und das Fenster bleibt hinter dem
        // maximierten Hauptfenster verborgen. Daher erst nach Abschluss der Initialisierung aktivieren.
        Dispatcher.BeginInvoke(new Action(() => Activate()), System.Windows.Threading.DispatcherPriority.ContextIdle);
      };
      vm.RequestHide += (s, e) => Hide();
      DataContext = vm;
    }

    public void Anzeigen(Schueler schueler) => vm.Anzeigen(schueler);
  }
}
