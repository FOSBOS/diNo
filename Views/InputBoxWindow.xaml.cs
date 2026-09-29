using System.Windows;

namespace diNo.Views
{
  // WPF-Nachfolger von InputBox.cs (WinForms). Gleiche statische Show()-API
  // wie das Original, damit Aufrufer unverändert bleiben können.
  public partial class InputBoxWindow : Window
  {
    public string Label
    {
      get => PromptLabel.Content?.ToString() ?? "";
      set => PromptLabel.Content = value;
    }

    public string Value
    {
      get => ValueTextBox.Text;
      set => ValueTextBox.Text = value;
    }

    public InputBoxWindow()
    {
      InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    public static string Show(string label, string wert)
    {
      var window = new InputBoxWindow { Label = label, Value = wert };
      return window.ShowDialog() == true ? window.Value : "";
    }
  }
}
