using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von KurseForm.cs (WinForms).
  public partial class KurseWindow : System.Windows.Window
  {
    public KurseWindow()
    {
      InitializeComponent();
      DataContext = new KurseViewModel();
    }
  }
}
