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
      chkShowAbi_CheckedChanged(this,null);
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
          if (hjl.SchnittMdl!=null) // der Rest sollte befüllt sein!
            dataGridNoten.Rows[lineCount].Cells[2].Value = hjl.SchnittMdl.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[4].Value = hjl.Punkte2Dez.GetValueOrDefault();
          dataGridNoten.Rows[lineCount].Cells[5].Tag = hjl;
          dataGridNoten.Rows[lineCount].Cells[5].Value = hjl.Punkte;
          //if (Zugriff.Instance.aktHalbjahr == Halbjahr.Erstes)
            SetBackgroundColor(hjl, dataGridNoten.Rows[lineCount].Cells[5]);
        }

        dataGridNoten.Rows[lineCount].Cells[8].Value = fach.SA(Halbjahr.Zweites);
        dataGridNoten.Rows[lineCount].Cells[6].Value = fach.sL(Halbjahr.Zweites);
        hjl = fach.getHjLeistung(HjArt.Hj2);
        if (hjl!=null)
        {
          if (hjl.SchnittMdl != null)
            dataGridNoten.Rows[lineCount].Cells[7].Value = hjl.SchnittMdl.GetValueOrDefault(); 
          dataGridNoten.Rows[lineCount].Cells[9].Value = hjl.Punkte2Dez.GetValueOrDefault();
          dataGridNoten.Rows[lineCount].Cells[10].Tag = hjl;
          dataGridNoten.Rows[lineCount].Cells[10].Value = hjl.Punkte;
          //if (Zugriff.Instance.aktHalbjahr == Halbjahr.Zweites)
            SetBackgroundColor(hjl, dataGridNoten.Rows[lineCount].Cells[10]);
        }

        hjl = fach.getHjLeistung(HjArt.JN); // Jahresnote (unabhängig von Einbringung)
        // hjl = fach.getHjLeistung(HjArt.GesErg); --> kommt im Reiter Hj-Leistung
        if (hjl != null)
        {
          dataGridNoten.Rows[lineCount].Cells[14].Tag = hjl;
          dataGridNoten.Rows[lineCount].Cells[14].Value = hjl.Punkte;
          //if (Zugriff.Instance.aktHalbjahr == Halbjahr.Zweites)
            SetBackgroundColor(hjl, dataGridNoten.Rows[lineCount].Cells[14]);
        }
        lineCount++;
      }
    }

    private void SetBackgroundColor(HjLeistung hj, DataGridViewCell cell)
    {
      Color color = GetBackgroundColor(hj);
      if (color == dataGridNoten.BackgroundColor)
      {
        return; // nothing to do
      }
      else
      {
        cell.Style.BackColor = color;
      }
    }

    private Color GetBackgroundColor(HjLeistung hj)
    {
      if (hj.Status == HjStatus.Ungueltig) return Color.Violet;
      if (hj.Status == HjStatus.NichtEinbringen) return Color.LightGray;
      if (hj.Punkte < 1) return Color.Coral;
      if (hj.Punkte < 3.5) return Color.Khaki;
      return dataGridNoten.BackgroundColor;
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
      ShowCols(1,4,chkShowHj1.Checked);
      ShowFixedCols();
    }

    private void chkShowHj2_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(6,9,chkShowHj2.Checked);
      ShowFixedCols();
    }

    private void chkShowAbi_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(11,13,chkShowAbi.Checked);
      ShowFixedCols();
    }

    private void ShowFixedCols() // bestimmte Gesamtwert-Spalten bleiben (fast) immer sichtbar
    {
      dataGridNoten.Columns[10].Visible = chkShowHj2.Checked || chkShowAbi.Checked; // 2. Hj
      dataGridNoten.Columns[14].Visible = chkShowHj2.Checked || chkShowAbi.Checked; // Jahresnote
    }

    private void setStatus(HjStatus status)
    {
      HjLeistung hj = (HjLeistung) dataGridNoten.SelectedCells[0].Tag;
      hj.Status = status;
      hj.WriteToDB();
      SetBackgroundColor(hj, dataGridNoten.SelectedCells[0]);
    }

    private void contextMenu_Opening(object sender, CancelEventArgs e)
    {
      var c = dataGridNoten.SelectedCells[0];
      if (c.Tag == null)
      {
        e.Cancel = true;
        return;
      }

      byte s = (byte)((HjLeistung)c.Tag).Status; // aktueller Status anhaken
      for (int i = 0; i < 4; i++)
        ((ToolStripMenuItem)contextMenu.Items[i]).Checked = i == s;
    }

    private void undefiniertToolStripMenuItem_Click(object sender, EventArgs e)
    {
      setStatus(HjStatus.None);
    }

    private void einbringenToolStripMenuItem_Click(object sender, EventArgs e)
    {
      setStatus(HjStatus.Einbringen);
    }

    private void nichtEinbringenToolStripMenuItem_Click(object sender, EventArgs e)
    {
      setStatus(HjStatus.NichtEinbringen);
    }

    private void ungueltigToolStripMenuItem_Click(object sender, EventArgs e)
    {
      setStatus(HjStatus.Ungueltig);
    }

    private void dataGridNoten_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
      {
        dataGridNoten.CurrentCell = dataGridNoten.Rows[e.RowIndex].Cells[e.ColumnIndex];
      }
    }
  }
}
