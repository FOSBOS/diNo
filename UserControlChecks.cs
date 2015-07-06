using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace diNo
{
  public partial class UserControlChecks : UserControl
  {
    public UserControlChecks()
    {
      InitializeComponent();
    }

    public void Print(string klasse, string[] meldungen)
    {
      PrintDocument printDocument1 = new PrintDocument();
      printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);

      this.textBoxKlasse.Text = klasse;
      this.textBoxDatum.Text = DateTime.Today.ToShortDateString();

      foreach (string meldung in meldungen)
      {
        var item = new ListViewItem();
        if (meldung.StartsWith("Schüler"))
        {
          item.Font = new Font(item.Font, FontStyle.Bold);
          item.Text = meldung;
        }
        else
        {
          item.Text = "   " + meldung;
        }

        this.listViewChecks.Items.Add(item);
      }

      CaptureScreen();
      List<string> Printers = new List<string>();
      foreach (string p in PrinterSettings.InstalledPrinters)
        Printers.Add(p);

      // Alle verfügbaren Drucker sind in Printers gespeichert, 
      // TODO: man könnte den Benutzer daraus auswählen lassen.
      // In diesem Beispiel wird einfach immer Drucker 0 gewählt.
      printDocument1.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[0];
      printDocument1.Print();
    }

    private Bitmap memoryImage;

    private void CaptureScreen()
    {
      Graphics myGraphics = this.CreateGraphics();
      Size s = this.Size;
      memoryImage = new Bitmap(s.Width, s.Height, myGraphics);
      Graphics memoryGraphics = Graphics.FromImage(memoryImage);
      this.DrawToBitmap(memoryImage, new Rectangle(0, 0, memoryImage.Width, memoryImage.Height));
      // memoryGraphics.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, s);
    }

    private void printDocument1_PrintPage(System.Object sender,
           System.Drawing.Printing.PrintPageEventArgs e)
    {
      e.Graphics.DrawImage(memoryImage, 0, 0);
    }
  }
}
