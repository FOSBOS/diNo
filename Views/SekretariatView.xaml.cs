using System.Windows.Controls;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von UserControlSekretariat.cs (WinForms).
  public partial class SekretariatView : UserControl
  {
    private readonly SekretariatViewModel vm = new SekretariatViewModel();

    public SekretariatView()
    {
      InitializeComponent();
      DataContext = vm;
    }

    public void SetSchueler(Schueler schueler) => vm.SetSchueler(schueler);
  }
}
