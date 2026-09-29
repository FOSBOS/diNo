using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von GlobalesForm.cs (WinForms).
  public partial class GlobalesWindow : System.Windows.Window
  {
    public GlobalesWindow()
    {
      InitializeComponent();
      DataContext = new GlobalesViewModel();
    }
  }
}
