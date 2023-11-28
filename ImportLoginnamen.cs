using diNo.diNoDataSetTableAdapters;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace diNo
{
  public class ImportLoginnamen
  {    

    public ImportLoginnamen()
    {
      MessageBox.Show("Benötigt wird eine Textdatei (CSV mit Trennzeichen ; ), bei der in Spalte 1 die Schüler-ID und in Spalte 2 die schulische Mailadresse des Schülers steht.\nDieser Import ist optional und wird nur für interne Automatisierungsprozesse verwendet (Kurswahl).", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Textdateien|*.*";

      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        using (FileStream stream = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new StreamReader(stream))
        {
          LeseLoginnamen(reader);
        }        
      }
    }

    private void LeseLoginnamen(StreamReader reader)
    {      
      int c = 0;
      while (!reader.EndOfStream)
      {
        string orignal = reader.ReadLine();        
        string[] line = orignal.Split(new string[] { ";" }, StringSplitOptions.None);
        string mail;
        int sid;
        
        Schueler s;

        sid = int.Parse(line[0]);
        mail = line[1];
        if (sid == 0)
          continue;
        try
        {
          s = Zugriff.Instance.SchuelerRep.Find(sid);
          if (s != null)
          {
            s.Data.MailSchule = mail;
            s.Save();
            c++;
          }
        }
        catch
        {
          // Schüler wahrscheinlich schon ausgetreten...?
        }
      }
      MessageBox.Show("Import beendet.\n"+ c +" Datensätze aktualisiert.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }
  }
}
