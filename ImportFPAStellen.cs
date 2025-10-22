using diNo.diNoDataSetTableAdapters;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace diNo
{
  public class ImportFPAStellen
  {

    public ImportFPAStellen()
    {
      MessageBox.Show("Benötigt wird eine Textdatei (CSV mit Trennzeichen ; ), bei der in Spalte 1 die Schüler-ID, in Spalte 2 das Halbjahr, in Spalte 3 der Name des FPA-Betrieb.\nDieser Import ist optional.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Textdateien|*.*";

      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        using (FileStream stream = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new StreamReader(stream))
        {
          LeseFPA(reader);
        }
      }
    }

    private void LeseFPA(StreamReader reader)
    {
      int c = 0;
      var ta = new FpaTableAdapter();
      while (!reader.EndOfStream)
      {
        string orignal = reader.ReadLine();
        string[] line = orignal.Split(new string[] { ";" }, StringSplitOptions.None);
        Schueler s;

        int sid = int.Parse(line[0]);
        int hj = int.Parse(line[1]);
        string stelle = line[2];
        if (sid == 0)
          continue;
        try
        {
          s = Zugriff.Instance.SchuelerRep.Find(sid);
          if (s != null)
          {
            var f = s.FPANoten[hj - 1];
            if (f.IsStelleNull())
            {
              f.Stelle = stelle;
              ta.Update(s.FPANoten);
              c++;
            }
          }
        }
        catch
        {
          // Schüler wahrscheinlich schon ausgetreten...?
        }
      }
      MessageBox.Show("Import beendet.\n" + c + " Datensätze aktualisiert.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }
  }
}
