using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von LehrerForm.cs (WinForms).
  public partial class LehrerWindow : System.Windows.Window
  {
    private readonly LehrerViewModel vm;

    public LehrerWindow()
    {
      InitializeComponent();
      vm = new LehrerViewModel();
      DataContext = vm;
    }

    private void VornameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
      vm.VornameLostFocus();
    }
  }
}
