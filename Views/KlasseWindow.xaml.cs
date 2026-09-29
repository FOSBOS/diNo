using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von KlasseForm.cs (WinForms).
  public partial class KlasseWindow : System.Windows.Window
  {
    public KlasseWindow()
    {
      InitializeComponent();
      DataContext = new KlasseViewModel();
    }
  }
}
