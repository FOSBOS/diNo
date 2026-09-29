using System.Collections.Generic;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von NotenCheckForm.cs (WinForms).
  public partial class NotenCheckWindow : System.Windows.Window
  {
    public NotenCheckWindow(List<Klasse> selObj)
    {
      InitializeComponent();
      var vm = new NotenCheckViewModel(selObj);
      vm.RequestClose += (s, e) => Close();
      DataContext = vm;
    }
  }
}
