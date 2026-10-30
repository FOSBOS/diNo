using diNo.ViewModels;

namespace diNo.Views
{
  public partial class BuecherWindow : System.Windows.Window
  {
    public BuecherWindow()
    {
      InitializeComponent();
      DataContext = new BuecherViewModel();
    }
  }
}
