using System.Windows.Controls;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von UserControlSchueleransicht.cs (WinForms).
  public partial class SchueleransichtView : UserControl
  {
    private readonly SchueleransichtViewModel vm = new SchueleransichtViewModel();

    public SchueleransichtView()
    {
      InitializeComponent();
      DataContext = vm;
    }

    public void SetSchueler(Schueler schueler) => vm.SetSchueler(schueler);
  }
}
