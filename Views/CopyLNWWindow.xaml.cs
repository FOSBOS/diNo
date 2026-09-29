using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von CopyLNW.cs (WinForms). Anders als das WinForms-Original
  // (das sich im eigenen Konstruktor selbst per ShowDialog() anzeigt) zeigt der
  // Aufrufer dieses Fenster explizit an (new CopyLNWWindow().ShowDialog()).
  public partial class CopyLNWWindow : System.Windows.Window
  {
    public CopyLNWWindow()
    {
      InitializeComponent();
      DataContext = new CopyLNWViewModel();
    }
  }
}
