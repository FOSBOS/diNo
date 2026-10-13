using System;
using System.ComponentModel;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von Brief.cs (WinForms). Wird wie das Original einmal erzeugt
  // und über Anzeigen(Schueler) für verschiedene Schüler wiederverwendet (ShowDialog/Hide
  // statt Neuerzeugung).
  public partial class BriefWindow : System.Windows.Window
  {
    private readonly BriefViewModel vm;

    public BriefWindow(Action onRefreshVorkommnisse)
    {
      InitializeComponent();
      vm = new BriefViewModel(onRefreshVorkommnisse);
      // ShowDialog() statt Show(): das Fenster wird dadurch vom Fenstermanager zuverlässig
      // aktiviert und in den Vordergrund geholt. Mit reinem Show() (+ manuellem Activate())
      // griff Windows' Schutz gegen Fokus-Diebstahl, wodurch das Fenster hinter dem
      // maximierten Hauptfenster verborgen blieb und erst ein zweiter Klick es sichtbar machte.
      //
      // ShowDialog() wird außerdem erst verzögert aufgerufen (statt noch synchron im
      // Click-Handler des Brief-Buttons): Startet die verschachtelte Dialog-Message-Loop
      // direkt während die Maus-Ereignisse des auslösenden Klicks noch verarbeitet werden,
      // bekommt das neue Fenster trotz ShowDialog() keinen Eingabefokus und wirkt dann so,
      // als hätte der erste Klick gar nichts bewirkt - erst ein zweiter Klick (auf irgendein
      // Steuerelement) stößt die überfällige Aktivierung an.
      vm.RequestShow += (s, e) => Dispatcher.BeginInvoke(new Action(() => ShowDialog()),
        System.Windows.Threading.DispatcherPriority.ContextIdle);
      vm.RequestHide += (s, e) => Close();
      DataContext = vm;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      // Fenster wird wiederverwendet statt bei jedem Aufruf neu erzeugt (siehe Anzeigen):
      // echtes Schließen verhindern und stattdessen nur verstecken, damit Eingaben erhalten
      // bleiben und ShowDialog() beim nächsten Mal wieder aufgerufen werden kann.
      e.Cancel = true;
      Hide();
      // Beim Verstecken (statt echtem Schließen) holt WPF den Eingabefokus nicht automatisch
      // zum Owner zurück. Ohne das bleibt das Hauptfenster zwar im Vordergrund, aber nicht
      // fokussiert, wodurch der nächste Klick auf den Brief-Button nur das Fenster aktiviert,
      // statt den Button auszulösen - man müsste also wieder zweimal klicken.
      Owner?.Activate();
    }

    public void Anzeigen(Schueler schueler) => vm.Anzeigen(schueler);
  }
}
