using System.Collections.Generic;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von MailDialog.cs (WinForms).
  public partial class MailDialogWindow : System.Windows.Window
  {
    public MailDialogWindow(List<Schueler> selectedObjects)
    {
      InitializeComponent();
      DataContext = new MailDialogViewModel(selectedObjects);
    }
  }
}
