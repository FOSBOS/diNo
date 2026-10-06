using System;
using System.Windows.Forms;

namespace diNo
{
  static class Program
  {
    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles(); // weiterhin nötig für die per WindowsFormsHost eingebetteten WinForms-Controls
      Application.SetCompatibleTextRenderingDefault(false);
      Zugriff.Instance.ToString(); // Instantiierung vor dem Fensteraufruf
      var app = new App();
      app.Run(new Views.KlassenansichtWindow());
    }
  }
}
