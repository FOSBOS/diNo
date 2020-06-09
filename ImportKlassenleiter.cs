using diNo.diNoDataSetTableAdapters;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Windows.Forms;

// erwartet in Spalte C das Lehrerkürzel und in Spalte D einen Klassennamen, falls dieser Lehrer eine KL hat.
// die erste Zeile wird ignoriert!

namespace diNo
{
  public class ImportKlassenleiter
  {
    private OpenExcel xls;
    private Dictionary<string, int> LehrerListe = new Dictionary<string, int>(); // verwaltet Kürzel, LehrerId

    public ImportKlassenleiter()
    {
      MessageBox.Show("Benötigt wird eine Excelliste, bei der in Spalte 3 das Kürzel und in Spalte 4 die Klasse steht.\nAb Zeile 2 müssen Daten enthalten sein.\nDie Klassen müssen schon angelegt worden sein.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Excel Files|*.xls*";

      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        xls = new OpenExcel(fileDialog.FileName);
        LoadLehrer();
        LeseKlassenleiter();
        MessageBox.Show("Bitte in der Datenbank prüfen, ob alle Klassen richtig angelegt wurden.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        xls.Dispose();
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

    private void LeseKlassenleiter()
    {
      KlasseTableAdapter ta = new KlasseTableAdapter();
      Worksheet s = xls.workbook.Worksheets[1];
      //Worksheet s = wb.ActiveSheet;
      int zeile = 2;
      string kuerzel, klasse;
      int lehrerid;

      kuerzel = (string)(s.Cells[zeile, 3].Value);
      klasse = (string)(s.Cells[zeile, 4].Value);
      while (kuerzel != null)
      {
        if (klasse != null)
        {
          LehrerListe.TryGetValue(kuerzel, out lehrerid);
          var dt = ta.GetDataByBezeichnung(klasse);
          if (dt.Count > 0)
          {
            var klassenRow = dt[0];
            klassenRow.KlassenleiterId = lehrerid;
            ta.Update(klassenRow);
          }
        }
        zeile++;
        kuerzel = (string)(s.Cells[zeile, 3].Value);
        klasse = (string)(s.Cells[zeile, 4].Value);
      }
    }
  }
}
