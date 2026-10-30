using System.Windows;
using System.Windows.Controls;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von UserControlNotenbogen.cs (WinForms). Spaltensichtbarkeit bleibt Code-Behind
  // (DataGridColumn ist nicht Teil des Visual Trees, daher nicht direkt an die ViewModel-Properties
  // bindbar) - war im WinForms-Original ohnehin schon imperativer Code (ShowCols/ShowFixedCols).
  public partial class NotenbogenView : UserControl
  {
    private readonly NotenbogenViewModel vm = new NotenbogenViewModel();

    public NotenbogenView()
    {
      InitializeComponent();
      DataContext = vm;
      Zeitpunkt z = (Zeitpunkt) Zugriff.Instance.aktZeitpunkt;
      ChkShowHj1.IsChecked = z == Zeitpunkt.HalbjahrUndProbezeitFOS;
      ChkShowHj2.IsChecked = z > Zeitpunkt.HalbjahrUndProbezeitFOS;
      ChkShowAbi.IsChecked = z >= Zeitpunkt.ErstePA && z <= Zeitpunkt.DrittePA;
      UpdateColumnVisibility();
    }

    public void SetSchueler(Schueler schueler)
    {
      vm.SetSchueler(schueler);
      UpdateColumnVisibility();
    }

    private void ShowChanged(object sender, RoutedEventArgs e) => UpdateColumnVisibility();

    private void UpdateColumnVisibility()
    {
      if (ColGE == null) return; // Checked-Events feuern schon während InitializeComponent, bevor alle Spalten existieren

      SetVisible(vm.Show11, Col11_1, Col11_2);
      SetVisible(ChkShowHj1.IsChecked == true, ColSlHj1, ColSchnittMdl1, ColSaHj1, ColPunkte2DezHj1);
      SetVisible(ChkShowHj2.IsChecked == true, ColSlHj2, ColSchnittMdl2, ColSaHj2, ColPunkte2DezHj2);
      SetVisible(ChkShowAbi.IsChecked == true && vm.ShowAbiCols, ColSAP, ColMAP, ColAPG);

      bool fixedVisible = ChkShowHj2.IsChecked == true || ChkShowAbi.IsChecked == true ||
        (ChkShowHj1.IsChecked != true && ChkShowHj2.IsChecked != true && ChkShowAbi.IsChecked != true);
      SetVisible(fixedVisible, ColHj2, ColJN, ColGE);
    }

    private static void SetVisible(bool visible, params DataGridColumn[] columns)
    {
      foreach (var c in columns)
        c.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
