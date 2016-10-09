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
    OpenExcel xls;
    public ImportKlassenleiter()
    {      
      var fileDialog = new OpenFileDialog();
      fileDialog.Filter = "Excel Files|*.xls*";
            
      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        xls = new OpenExcel(fileDialog.FileName);
        

      }



    }

  }
}
