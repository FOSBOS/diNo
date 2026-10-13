using System;
using System.Collections.Generic;
using System.ComponentModel;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von MailDialog.cs (WinForms). Wird wie BriefWindow einmal erzeugt und
  // über Anzeigen(selectedObjects) wiederverwendet (statt bei jedem Öffnen neu erzeugt zu
  // werden), damit Betreff/Bodytext/Optionen beim erneuten Öffnen erhalten bleiben.
  public partial class MailDialogWindow : System.Windows.Window
  {
    private readonly MailDialogViewModel vm;

    public MailDialogWindow()
    {
      InitializeComponent();
      vm = new MailDialogViewModel();
      DataContext = vm;
    }

    public void Anzeigen(List<Schueler> selectedObjects)
    {
      vm.Anzeigen(selectedObjects);
      // Siehe BriefWindow: ShowDialog() erst verzögert aufrufen, damit das Fenster nicht noch
      // während der Mausverarbeitung des auslösenden Klicks (ohne Eingabefokus) geöffnet wird.
      Dispatcher.BeginInvoke(new Action(() => ShowDialog()), System.Windows.Threading.DispatcherPriority.ContextIdle);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      // Fenster wird wiederverwendet statt bei jedem Aufruf neu erzeugt: echtes Schließen
      // verhindern und stattdessen nur verstecken, damit Eingaben erhalten bleiben und
      // ShowDialog() beim nächsten Mal wieder aufgerufen werden kann.
      e.Cancel = true;
      Hide();
      Owner?.Activate();
    }
  }
}
