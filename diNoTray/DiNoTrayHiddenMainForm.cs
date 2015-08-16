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

/*
    /// <summary>
    /// Konstruktor.
    /// </summary>
    public diNoTrayHiddenMainForm()
    {
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
    }

    /// <summary>
    /// Wenn das Programm startet, versteckt diese Methode das Hauptformular, so dass die Applikation nur als Tray Icon sichtbar ist.
    /// </summary>
    /// <param name="e">Die event arguments.</param>
    protected override void OnLoad(EventArgs e)
    {
      Visible = false; // Hide form window.
      ShowInTaskbar = false; // Remove from taskbar.

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
          var notenReader = new NotenAusExcelReader();
          notenReader.OnStatusChange += notenReader_OnStatusChange;
          notenReader.Synchronize(fileName);
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
          // TODO: Evtl. nur neu eingesteckte filtern (Änderungen zum letzten Mal?)
          string path = d.Name+"Noten";
          if (Directory.Exists(path))
          {
            foreach (var fileName in Directory.GetFiles(path, "*.xlsx"))
            {
              var notenReader = new NotenAusExcelReader();
              notenReader.OnStatusChange += notenReader_OnStatusChange;
              notenReader.Synchronize(fileName);
            }
          }
        }
      }
    }

    /// <summary>
    /// Event Handler für Statusmeldungen vom Notenleser.
    /// </summary>
    /// <param name="e">Event Args mit dem neuen Status.</param>
    /// <param name="sender">Der Sender des Events.</param>
    void notenReader_OnStatusChange(Object sender, NotenAusExcelReader.StatusChangedEventArgs e)
    {
      trayIcon.BalloonTipText = e.Status;
      trayIcon.BalloonTipTitle = "diNo Status";
      trayIcon.ShowBalloonTip(3);
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
*/
  }
}
