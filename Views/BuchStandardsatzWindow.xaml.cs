using diNo.ViewModels;

namespace diNo.Views
{
  public partial class BuchStandardsatzWindow : System.Windows.Window
  {
    public BuchStandardsatzWindow()
    {
      InitializeComponent();
      DataContext = new BuchStandardsatzViewModel();
    }
  }
}
