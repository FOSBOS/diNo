using diNo.ViewModels;

namespace diNo.Views
{
  public partial class BuchSammelEinbuchenWindow : System.Windows.Window
  {
    public BuchSammelEinbuchenWindow()
    {
      InitializeComponent();
      DataContext = new BuchSammelEinbuchenViewModel();
    }
  }
}
