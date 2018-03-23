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

        dataGridNoten.Rows[lineCount].Cells[3].Value = fach.SA(Halbjahr.Erstes);
        dataGridNoten.Rows[lineCount].Cells[1].Value = fach.sL(Halbjahr.Erstes);
        HjLeistung hjl = fach.getHjLeistung(HjArt.Hj1);
        if (hjl!=null)
        {
          dataGridNoten.Rows[lineCount].Cells[2].Value = hjl.SchnittMdl.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[4].Value = hjl.Punkte2Dez.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[5].Value = hjl.Punkte;
          if (Zugriff.Instance.aktZeitpunkt <= 2)
            SetBackgroundColor(hjl.Punkte, dataGridNoten.Rows[lineCount].Cells[5]);
        }

        dataGridNoten.Rows[lineCount].Cells[8].Value = fach.SA(Halbjahr.Zweites);
        dataGridNoten.Rows[lineCount].Cells[6].Value = fach.sL(Halbjahr.Zweites);
        hjl = fach.getHjLeistung(HjArt.Hj2);
        if (hjl!=null)
        {
          dataGridNoten.Rows[lineCount].Cells[7].Value = hjl.SchnittMdl.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[9].Value = hjl.Punkte2Dez.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[10].Value = hjl.Punkte;
          if (Zugriff.Instance.aktZeitpunkt > 2)
            SetBackgroundColor(hjl.Punkte, dataGridNoten.Rows[lineCount].Cells[10]);
        }

        if ((int)schueler.getKlasse.Jahrgangsstufe < 12)
          hjl = fach.getHjLeistung(HjArt.JN);
        else
          hjl = fach.getHjLeistung(HjArt.GesErg);
        if (hjl != null)
        {         
          dataGridNoten.Rows[lineCount].Cells[14].Value = hjl.Punkte;
          if (Zugriff.Instance.aktZeitpunkt > 2)
            SetBackgroundColor(hjl.Punkte, dataGridNoten.Rows[lineCount].Cells[14]);
        }
        lineCount++;
      }
    }

    private void SetBackgroundColor(double notenwert, DataGridViewCell cell)
    {
      Color color = GetBackgroundColor(notenwert);
      if (color == dataGridNoten.BackgroundColor)
      {
        return; // nothing to do
      }
      else
      {
        cell.Style.BackColor = color;
      }
    }

    private Color GetBackgroundColor(double notenwert)
    {
      int aktuellerZeitpunkt = Zugriff.Instance.aktZeitpunkt;

      if (notenwert < 1) return Color.Crimson;
      if (notenwert < 1.5) return Color.Coral;
      if (notenwert < 2.5) return Color.Orange;
      if (notenwert < 3.5) return Color.Gold;
      if (notenwert > 11.5) return Color.PaleGreen;
      return Color.LightYellow;
  }

    private void FillCell(DataGridViewCell c, HjLeistung hjl)
    {
      if (hjl!=null)
      {
        c.Value = hjl.Punkte;
      }
    }

    private void ShowCols(int vonCol,int bisCol, bool visible)
    {
      int i;
      for (i=vonCol;i<=bisCol;i++)
        dataGridNoten.Columns[i].Visible = visible;
    }

    private void chkShowHj1_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(1,5,chkShowHj1.Checked);
    }

    private void chkShowHj2_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(6,10,chkShowHj2.Checked);
      ShowCols(14, 14, chkShowHj2.Checked|| chkShowAbi.Checked);
    }

    private void chkShowAbi_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(11,13,chkShowAbi.Checked);
      ShowCols(14, 14, chkShowHj2.Checked || chkShowAbi.Checked);
    }
  }
}
