﻿using diNo.diNoDataSetTableAdapters;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Excel Files|*.xls*";
            
      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        xls = new OpenExcel(fileDialog.FileName);
        LoadLehrer();
        LeseKlassenleiter();
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
      KlasseTableAdapter ta  = new KlasseTableAdapter();
      Workbook wb = xls.workbook;
      Worksheet s = wb.ActiveSheet;
      int zeile=2;
      string kuerzel, klasse;
      int lehrerid;

      kuerzel = s.Cells[zeile,3];
      klasse = s.Cells[zeile,4];
      while (kuerzel!="")
      {
        if (klasse!="")
        {
          LehrerListe.TryGetValue(kuerzel,out lehrerid);
          var dt = ta.GetDataByBezeichnung(klasse);
          if (dt.Count>0)
          {
            var klassenRow = dt[0];
            klassenRow.KlassenleiterId = lehrerid;
            ta.Update(klassenRow);
          }
        }
      }
    }
  }
}
