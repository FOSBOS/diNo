using diNo;
using diNoTray.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace diNoTray
{
  public partial class diNoTrayHiddenMainForm : Form
  {
    private NotifyIcon trayIcon;
    private ContextMenu trayMenu;
    private string path;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    public diNoTrayHiddenMainForm(): base()
    {
      InitializeComponent();

      // Create a simple tray menu with only one item.
      trayMenu = new ContextMenu();
      trayMenu.MenuItems.Add("Exit", OnExit);
      trayMenu.MenuItems.Add("Synchronisieren", OnSynchronizeClick);

      // Create a tray icon. In this example we use a
      // standard system icon for simplicity, but you
      // can of course use your own custom icon too.
      trayIcon = new NotifyIcon();
      trayIcon.Text = "diNo";
      Bitmap bmp = new Bitmap(Resources.stegosaurus_304011__180);
      IntPtr Hicon = bmp.GetHicon();
      trayIcon.Icon = Icon.FromHandle(Hicon);

      // Add menu to tray icon and show it.
      trayIcon.ContextMenu = trayMenu;
      trayIcon.Visible = true;
      trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
    }

    /// <summary>
    /// Wenn das Programm startet, versteckt diese Methode das Hauptformular, so dass die Applikation nur als Tray Icon sichtbar ist.
    /// </summary>
    /// <param name="e">Die event arguments.</param>
    protected override void OnLoad(EventArgs e)
    {
      // Visible = false; // Hide form window.
      // ShowInTaskbar = false; // Remove from taskbar.

      base.OnLoad(e);
    }

    /// <summary>
    /// Methode wird aufgerufen, wenn ein Nutzer "exit" klickt.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die event arguments.</param>
    private void OnExit(object sender, EventArgs e)
    {
      Application.Exit();
    }

    /// <summary>
    /// Methode wird aufgerufen, wenn ein Nutzer "synchronisieren" klickt.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die event arguments.</param>
    private void OnSynchronizeClick(object sender, EventArgs e)
    {
      WaehleDateiUndTrageNotenEin();
    }

    private void WaehleDateiUndTrageNotenEin()
    {
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Excel Files|*.xls*";
      fileDialog.Multiselect = true;
      // Call the ShowDialog method to show the dialog box.
      bool userClickedOK = fileDialog.ShowDialog() == DialogResult.OK;

      // Process input if the user clicked OK.
      if (userClickedOK == true)
      {
        foreach (string fileName in fileDialog.FileNames)
        {
          Synchronisiere(fileName);
        }
      }
    }

    /// <summary>
    /// Prüft alle eingesteckten Removable Drives (i.d.R. USB-Sticks) ob Notendateien drauf sind.
    /// </summary>
    private void CheckDrives()
    {
     foreach (System.IO.DriveInfo d in System.IO.DriveInfo.GetDrives())
      {
        if (d.DriveType == System.IO.DriveType.Removable)
        {
          this.path = d.Name+"Noten";
          if (Directory.Exists(path))
          {
            trayIcon.BalloonTipText = "Zum Synchronisieren bitte hier klicken.";
            trayIcon.BalloonTipTitle = "Im Verzeichnis " + path + " wurden Notendateien gefunden";
            trayIcon.ShowBalloonTip(3);
          }
        }
      }
    }
    /// <summary>
    /// Event Handler für Click Ereignis auf dem Balloon.
    /// </summary>
    /// <param name="sender">Der Sender.</param>
    /// <param name="e">Die Event Arguments.</param>
    private void TrayIcon_BalloonTipClicked(object sender, EventArgs e)
    {
      if (Directory.Exists(this.path))
      {
        foreach (var fileName in Directory.GetFiles(this.path, "*.xlsx"))
        {
          Synchronisiere(fileName);
        }
      }
    }

    /// <summary>
    /// Übernimmt die Synchronisation.
    /// </summary>
    /// <param name="fileName">Name der Excel-Datei, die die Noten enthält.</param>
    private void Synchronisiere(string fileName)
    {
      var notenReader = new LeseNotenAusExcel(fileName, notenReader_OnStatusChange);
    }

    
    /// <summary>
    /// Event Handler für Statusmeldungen vom Notenleser.
    /// </summary>
    /// <param name="e">Event Args mit dem neuen Status.</param>
    /// <param name="sender">Der Sender des Events.</param>
    void notenReader_OnStatusChange(Object sender, StatusChangedEventArgs e)
    {
      lblStatus.Text = e.Meldung;
    }
    

    const int WM_DEVICECHANGE = 0x219;
    /// <summary>
    /// Fängt eine windows message ab, wenn ein neuer USB-Stick eingesteckt wurde.
    /// </summary>
    /// <param name="m">Die windows message.</param>
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == WM_DEVICECHANGE)
      {
        CheckDrives();
      }

      base.WndProc(ref m);
    }

    private void btnNotenSync_Click(object sender, EventArgs e)
    {
      WaehleDateiUndTrageNotenEin();
    }

    private void btnNotenbogen_Click(object sender, EventArgs e)
    {
      new Klassenansicht().Show();
    }
  }
}
