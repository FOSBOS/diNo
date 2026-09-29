using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von Datenauswahl.cs (WinForms).
  public partial class DatenauswahlWindow : System.Windows.Window
  {
    public DatenauswahlWindow()
    {
      InitializeComponent();
      var vm = new DatenauswahlViewModel();
      vm.RequestClose += (s, e) => DialogResult = true;
      DataContext = vm;
    }
  }
}
