using diNo.diNoDataSetTableAdapters;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

// erwartet in Spalte C das Lehrerkürzel und in Spalte D einen Klassennamen, falls dieser Lehrer eine KL hat.
// die erste Zeile wird ignoriert!

namespace diNo
{
  public class ImportKlassenleiter
  {
    private Dictionary<string, int> LehrerListe = new Dictionary<string, int>(); // verwaltet Kürzel, LehrerId

    public ImportKlassenleiter()
    {
      MessageBox.Show("Benötigt wird eine Textdatei, bei der in Spalte 2 das Lehrerkürzel und in Spalte 3 die Klasse steht.\nDie Klassen müssen schon angelegt worden sein.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Textdateien|*.*";

      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        using (FileStream stream = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new StreamReader(stream))
        {
          LoadLehrer();
          LeseKlassenleiter(reader);
        }
        MessageBox.Show("Bitte im Klassendialog prüfen, ob alle Klassen richtig angelegt wurden.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        
      }
    }


    private void LoadLehrer()
    {
      diNoDataSet.LehrerDataTable dt;
      var ta = new LehrerTableAdapter();
      dt = ta.GetData();

      foreach (var r in dt)
      {
        LehrerListe.Add(r.Kuerzel, r.Id);
      }
    }

    private void LeseKlassenleiter(StreamReader reader)
    {
      KlasseTableAdapter ta = new KlasseTableAdapter();
      while (!reader.EndOfStream)
      {
        string orignal = reader.ReadLine();        
        string[] line = orignal.Split(new string[] { "\t" }, StringSplitOptions.None);
        string kuerzel, klasse;
        int lehrerid;

        kuerzel = line[1];
        klasse = line[2];
        if (klasse == "")
          continue;
        
        LehrerListe.TryGetValue(kuerzel, out lehrerid);
        var dt = ta.GetDataByBezeichnung(klasse);
        if (dt.Count > 0)
        {
          var klassenRow = dt[0];
          klassenRow.KlassenleiterId = lehrerid;
          ta.Update(klassenRow);
        }
      }
    }
  }
}
