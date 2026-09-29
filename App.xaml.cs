namespace diNo
{
  // Wird manuell in Program.Main() instanziiert (kein StartupUri), da Main() weiterhin
  // Application.EnableVisualStyles()/SetCompatibleTextRenderingDefault() für die per
  // WindowsFormsHost eingebetteten WinForms-Controls aufrufen muss, bevor das Hauptfenster
  // (KlassenansichtWindow) erzeugt und über app.Run(window) gestartet wird.
  public partial class App
  {
    public App()
    {
      InitializeComponent();
    }
  }
}
