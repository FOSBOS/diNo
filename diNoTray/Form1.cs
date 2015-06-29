using diNo;
using diNoTray.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diNoTray
{
  public partial class Form1 : Form
  {
    private NotifyIcon trayIcon;
    private ContextMenu trayMenu;

    public Form1()
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

    protected override void OnLoad(EventArgs e)
    {
      Visible = false; // Hide form window.
      ShowInTaskbar = false; // Remove from taskbar.

      base.OnLoad(e);
    }

    private void OnExit(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void OnSynchronizeClick(object sender, EventArgs e)
    {
      // ToDo: Datei/Verzeichnisauswahl ermöglichen
      CheckDrives();
    }

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
              NotenAusExcelReader.Synchronize(fileName);
            }
          }
        }
      }
    }

    const int WM_DEVICECHANGE = 0x219;
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == WM_DEVICECHANGE)
      {
        CheckDrives();
      }

      base.WndProc(ref m);
    }

  }
}
