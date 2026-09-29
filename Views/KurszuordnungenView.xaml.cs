using System.Windows.Controls;
using System.Windows.Input;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von UserControlKurszuordnungen.cs (WinForms).
  public partial class KurszuordnungenView : UserControl
  {
    private readonly KurszuordnungenViewModel vm = new KurszuordnungenViewModel();

    public KurszuordnungenView()
    {
      InitializeComponent();
      DataContext = vm;
    }

    public void SetSchueler(Schueler schueler) => vm.SetSchueler(schueler);

    private void AktuelleKurse_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (((ListView)sender).SelectedItem is Kurs kurs && vm.MeldeAbCommand.CanExecute(kurs))
        vm.MeldeAbCommand.Execute(kurs);
    }

    private void MoeglicheKurse_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (((ListView)sender).SelectedItem is Kurs kurs && vm.MeldeAnCommand.CanExecute(kurs))
        vm.MeldeAnCommand.Execute(kurs);
    }
  }
}
