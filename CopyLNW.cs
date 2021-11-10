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

*/

namespace diNo
{
  public partial class CopyLNW : Form
  {
    public CopyLNW()
    {
      InitializeComponent();
      Dictionary<int, string> kurse = new Dictionary<int, string>();      
      foreach (var k in Zugriff.Instance.eigeneKurse)
      {
        kurse.Add(k.Id,k.Kursbezeichnung);
      }
      cbKurs.BeginUpdate();
      cbKurs.DataSource = kurse.ToList();
      cbKurs.DisplayMember = "Value";
      cbKurs.ValueMember = "Key";
      cbKurs.EndUpdate();

      cbKurs.SelectedIndex = 0;
      cbArt.SelectedIndex = 1;
      cbNummer.SelectedIndex = 0;
    }

    private void Kopiere(string datei, string art)
    {
      using (UserImpersonation user = new UserImpersonation(
        Zugriff.Instance.getString(GlobaleStrings.CopyUserLoginname),
        Zugriff.Instance.getString(GlobaleStrings.CopyUserDomain),
        Zugriff.Instance.getString(GlobaleStrings.CopyUserPwd)))
      {
        if (user.ImpersonateValidUser())
        {
          try
          {
            Kurs k = Zugriff.Instance.KursRep.Find((int)cbKurs.SelectedValue);
            string kursBez = cbKurs.Text;
            kursBez.Replace("/", "");

            string verz = Zugriff.Instance.getString(GlobaleStrings.LNWAblagePfad) + @"\" + k.getFach.Fachschaft
              + @"\Hj" + (byte)Zugriff.Instance.aktHalbjahr + @"\" + k.FachBezeichnung + @"\";
            string dat = Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS) + "_" + kursBez + "_Hj" + (byte)Zugriff.Instance.aktHalbjahr + "_"
              + cbArt.Text + cbNummer.Text + "_" + art + ".pdf";

            if (!Directory.Exists(verz))
              Directory.CreateDirectory(verz);
            if (File.Exists(verz + dat))
            {
              if (MessageBox.Show("Die " + art + " wurde bereits archiviert.\nSoll die Datei ersetzt werden?\n(Sind alle Einstellungen richtig?)", "diNo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            }
            File.Copy(datei, verz + dat, true);
            MessageBox.Show("Die " + art + " wurde archiviert.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
          catch
          {
            MessageBox.Show("Die " + art + " konnte nicht archiviert werden.\nFehler beim Kopieren.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
        else
        {
          MessageBox.Show("Die " + art + " konnte nicht archiviert werden.\nDer Server steht nicht zur Verfügung.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void btnAngabe_Click(object sender, EventArgs e)
    {
      Abgabe("Angabe");
    }

    private void btnLsg_Click(object sender, EventArgs e)
    {
      Abgabe("Lösung");
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
