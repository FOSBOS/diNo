using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlNotenbogen : UserControl
  {
    private Schueler schueler;

    public UserControlNotenbogen()
    {
      InitializeComponent();
    }
  
    public Schueler Schueler
    {
      get
      {
        return this.schueler;
      }
      set
      {
        this.schueler = value;
        this.Init();
      }
    }

    public void Init()
    {
      if (this.schueler == null)
      {
        return;
      }

      dataGridNoten.Rows.Clear();
      dataGridNoten.RowsDefaultCellStyle.BackColor = Color.White;

      if (schueler.Status == Schuelerstatus.Abgemeldet)
      {        
        dataGridNoten.RowsDefaultCellStyle.BackColor = Color.LightGray;
      }

      int lineCount = 0;
      foreach (var fach in schueler.getNoten.alleKurse)
      {
        dataGridNoten.Rows.Add();
        dataGridNoten.Rows[lineCount].Height += 2;
        dataGridNoten.Rows[lineCount].Cells[0].Value = fach.getFach.Bezeichnung;

        dataGridNoten.Rows[lineCount].Cells[1].Value = fach.SA(Halbjahr.Erstes);
        dataGridNoten.Rows[lineCount].Cells[2].Value = fach.sL(Halbjahr.Erstes);
        HjLeistung hjl = fach.getHjLeistung(HjArt.Hj1);
        if (hjl!=null)
        {
          dataGridNoten.Rows[lineCount].Cells[3].Value = hjl.SchnittMdl.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[4].Value = hjl.Punkte2Dez.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[5].Value = hjl.Punkte;
        }
        dataGridNoten.Rows[lineCount].Cells[6].Value = fach.SA(Halbjahr.Zweites);
        dataGridNoten.Rows[lineCount].Cells[7].Value = fach.sL(Halbjahr.Zweites);
        hjl = fach.getHjLeistung(HjArt.Hj2);
        if (hjl!=null)
        {
          dataGridNoten.Rows[lineCount].Cells[8].Value = hjl.SchnittMdl.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[9].Value = hjl.Punkte2Dez.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[10].Value = hjl.Punkte;
        }
        lineCount++;
      }
    }

    private void FillCell(DataGridViewCell c, HjLeistung hjl)
    {
      if (hjl!=null)
      {
        c.Value = hjl.Punkte;
      }
    }
  }
}
