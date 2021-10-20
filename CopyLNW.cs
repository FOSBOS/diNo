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

/* ToDo: 
    Liste der Kurse inkl. Schlüssel einlesen
    Verzeichnisaufbau, ggf. nicht vorhandenes Verz. erstellen      
    globale Strings für User und Passwort ggf. Domäne
*/

namespace diNo
{
  public partial class CopyLNW : Form
  {
    public CopyLNW()
    {
      InitializeComponent();
      foreach (var f in Zugriff.Instance.eigeneFaecher)
      {
        cbKurs.Items.Add(f.Bezeichnung);
      }
      cbKurs.SelectedIndex = 0;
      cbArt.SelectedIndex = 0;
      cbNummer.SelectedIndex = 0;
    }

    private void Kopiere(string datei, string art)
    {
      using (UserImpersonation user = new UserImpersonation("CopyUser", "FOSBOS", "NdiNo87§"))
      {
        if (user.ImpersonateValidUser())
        {
          string verz = @"\\srvfosbos\AblageLNW\" + "Hj" + (byte)Zugriff.Instance.aktHalbjahr;
          string dat = Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS) + "_" + cbKurs.Text + "_Hj" + (byte)Zugriff.Instance.aktHalbjahr + "_" 
            + cbArt.Text + cbNummer.Text + "_" + art + ".pdf";

         if (!Directory.Exists(verz))
            Directory.CreateDirectory(verz);
         File.Copy(datei, verz + dat);
          MessageBox.Show("Die " + art + " wurde archiviert.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          MessageBox.Show("Die Datei konnte nicht archiviert werden.","diNo",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
      }
    }

    private void btnAngabe_Click(object sender, EventArgs e)
    {
      Abgabe("Angabe");
    }

    private void Abgabe(string art)
    {
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "PDF|*.pdf";

      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        Cursor.Current = Cursors.WaitCursor;
        Kopiere(fileDialog.FileName, art);
        Cursor.Current = Cursors.Default;        
      }
    }
  }

}
