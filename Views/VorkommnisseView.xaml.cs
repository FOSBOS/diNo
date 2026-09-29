using System.Windows.Controls;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von UserControlVorkommnisse.cs (WinForms).
  public partial class VorkommnisseView : UserControl
  {
    private readonly VorkommnisseViewModel vm = new VorkommnisseViewModel();

    public VorkommnisseView()
    {
      InitializeComponent();
      DataContext = vm;
    }

    public void SetSchueler(Schueler schueler) => vm.SetSchueler(schueler);

    public void RefreshVorkommnisse() => vm.RefreshVorkommnisse();
  }
}
