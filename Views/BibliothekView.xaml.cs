using System.Windows.Controls;
using diNo.ViewModels;

namespace diNo.Views
{
  public partial class BibliothekView : UserControl
  {
    private readonly BibliothekViewModel vm = new BibliothekViewModel();

    public BibliothekView()
    {
      InitializeComponent();
      DataContext = vm;
    }

    public void SetSchueler(Schueler schueler) => vm.SetSchueler(schueler);

    public void RefreshAusleihen() => vm.RefreshAusleihen();
  }
}
