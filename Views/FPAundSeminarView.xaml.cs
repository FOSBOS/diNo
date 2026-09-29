using System.Windows.Controls;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von UserControlFPAundSeminar.cs (WinForms).
  public partial class FPAundSeminarView : UserControl
  {
    private readonly FPAundSeminarViewModel vm = new FPAundSeminarViewModel();

    public FPAundSeminarView()
    {
      InitializeComponent();
      DataContext = vm;
    }

    public void SetSchueler(Schueler schueler) => vm.SetSchueler(schueler);
  }
}
