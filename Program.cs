using System;
using System.Collections.Generic;
using System.Linq;
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
      Zugriff.Instance.ToString(); // Instantiierung vor dem Formularaufruf
      Application.Run(new Klassenansicht());
		}
	}
}
