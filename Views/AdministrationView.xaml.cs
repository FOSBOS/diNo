using System.Windows.Controls;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von UserControlAdministration.cs (WinForms).
  public partial class AdministrationView : UserControl
  {
    public AdministrationViewModel Vm { get; } = new AdministrationViewModel();

    public AdministrationView()
    {
      InitializeComponent();
      DataContext = Vm;
    }
  }
}
