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
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      log4net.Config.XmlConfigurator.Configure();
      SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
      Zugriff.Instance.ToString(); // Instantiierung vor dem Formularaufruf
      Application.Run(new Klassenansicht());
    }
  }
}
