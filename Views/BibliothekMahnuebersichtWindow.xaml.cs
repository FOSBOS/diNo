using diNo.ViewModels;

namespace diNo.Views
{
  public partial class BibliothekMahnuebersichtWindow : System.Windows.Window
  {
    public BibliothekMahnuebersichtWindow()
    {
      InitializeComponent();
      DataContext = new BibliothekMahnuebersichtViewModel();
    }
  }
}
