using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlNotenbogen : UserControl
  {
    private Schueler schueler;

    public UserControlNotenbogen()
    {
      int z = Zugriff.Instance.aktZeitpunkt;
      InitializeComponent();
      chkShowHj1.Checked = z <= (int)Zeitpunkt.HalbjahrUndProbezeitFOS;
      chkShowHj2.Checked = z == (int)Zeitpunkt.ErstePA || z == (int)Zeitpunkt.Jahresende;
      chkShowAbi.Checked = z == (int)Zeitpunkt.ZweitePA || z == (int)Zeitpunkt.DrittePA;
      chkShowHj1_CheckedChanged(this, null);
      chkShowHj2_CheckedChanged(this, null);
      chkShowAbi_CheckedChanged(this, null);

      if (Zugriff.Instance.HatRolle(Rolle.Admin)) // bearbeiten von Punktewerten
      {
        dataGridNoten.CellDoubleClick += dataGridNoten_CellDoubleClick;
        lbBerechnungsstatus.Visible = true;
        comboBoxBerechnungsstatus.Visible = true;
      }
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

      // Hj-Leistungen aus 11/1 und 11/2 anzeigen?
      bool show11 = schueler.hatVorHj;
      dataGridNoten.Columns[1].Visible = show11;
      dataGridNoten.Columns[2].Visible = show11;

      int lineCount = 0;
      foreach (var fach in schueler.getNoten.alleFaecher)
      {
        dataGridNoten.Rows.Add();
        dataGridNoten.Rows[lineCount].Height += 2;
        dataGridNoten.Rows[lineCount].Cells[0].Value = fach.getFach.Bezeichnung;
        dataGridNoten.Rows[lineCount].Tag = fach.getFach;

        if (show11)
        {
          FillCell(dataGridNoten.Rows[lineCount].Cells[1], fach.getVorHjLeistung(HjArt.Hj1));
          FillCell(dataGridNoten.Rows[lineCount].Cells[2], fach.getVorHjLeistung(HjArt.Hj2));
        }

        dataGridNoten.Rows[lineCount].Cells[5].Value = fach.SA(Halbjahr.Erstes);
        dataGridNoten.Rows[lineCount].Cells[3].Value = fach.sL(Halbjahr.Erstes);

        HjLeistung hjl = fach.getHjLeistung(HjArt.Hj1);
        if (hjl != null)
        {
          if (hjl.SchnittMdl != null) // der Rest sollte befüllt sein!
            dataGridNoten.Rows[lineCount].Cells[4].Value = hjl.SchnittMdl.GetValueOrDefault();
          dataGridNoten.Rows[lineCount].Cells[6].Value = hjl.Punkte2Dez.GetValueOrDefault();
          FillCell(dataGridNoten.Rows[lineCount].Cells[7], hjl);
        }

        dataGridNoten.Rows[lineCount].Cells[10].Value = fach.SA(Halbjahr.Zweites);
        dataGridNoten.Rows[lineCount].Cells[8].Value = fach.sL(Halbjahr.Zweites);

        hjl = fach.getHjLeistung(HjArt.Hj2);
        if (hjl != null)
        {
          if (hjl.SchnittMdl != null)
            dataGridNoten.Rows[lineCount].Cells[9].Value = hjl.SchnittMdl.GetValueOrDefault();
          dataGridNoten.Rows[lineCount].Cells[11].Value = hjl.Punkte2Dez.GetValueOrDefault();
          FillCell(dataGridNoten.Rows[lineCount].Cells[12], hjl);
        }

        dataGridNoten.Rows[lineCount].Cells[13].Value = fach.ToString(Halbjahr.Zweites, Notentyp.APSchriftlich);
        dataGridNoten.Rows[lineCount].Cells[14].Value = fach.ToString(Halbjahr.Zweites, Notentyp.APMuendlich);
        FillCell(dataGridNoten.Rows[lineCount].Cells[15], fach.getHjLeistung(HjArt.AP));

        FillCell(dataGridNoten.Rows[lineCount].Cells[16], fach.getHjLeistung(HjArt.JN));
        FillCell(dataGridNoten.Rows[lineCount].Cells[17], fach.getHjLeistung(HjArt.GesErg));
        lineCount++;
      }
      foreach (HjLeistung fr in schueler.Fachreferat)
      {
        dataGridNoten.Rows.Add();
        dataGridNoten.Rows[lineCount].Height += 2;
        dataGridNoten.Rows[lineCount].Cells[0].Value = "Fachreferat in " + fr.getFach.Bezeichnung;
        FillCell(dataGridNoten.Rows[lineCount].Cells[12], fr);
        FillCell(dataGridNoten.Rows[lineCount].Cells[17], fr);
        lineCount++;
      }

      if (dataGridPunktesumme.Visible = schueler.getKlasse.Jahrgangsstufe > Jahrgangsstufe.Elf && (Zugriff.Instance.aktZeitpunkt >= (int)Zeitpunkt.ErstePA || Zugriff.Instance.HatRolle(Rolle.Admin)))
      {
        lineCount = 0;
        Punktesumme p = schueler.punktesumme;
        dataGridPunktesumme.Rows.Clear();
        foreach (PunktesummeArt a in Enum.GetValues(typeof(PunktesummeArt)))
        {
          if (p.Anzahl(a) > 0)
          {
            dataGridPunktesumme.Rows.Add();
            dataGridPunktesumme.Rows[lineCount].Cells[0].Value = Punktesumme.ArtToText(a);
            dataGridPunktesumme.Rows[lineCount].Cells[1].Value = p.Summe(a);
            dataGridPunktesumme.Rows[lineCount].Cells[2].Value = p.Anzahl(a);
            //if (a=PunktesummeArt.Gesamt) dataGridPunktesumme.Rows[lineCount].Cells[1]. Bold??
            lineCount++;
          }
        }
      }

      lbHinweise.Visible = schueler.Data.Berechungsstatus > (byte)Berechnungsstatus.Unberechnet
        && schueler.punktesumme.Anzahl(PunktesummeArt.HjLeistungen) != schueler.GetAnzahlEinbringung();
      comboBoxBerechnungsstatus.SelectedIndex = schueler.Data.Berechungsstatus;
      textBoxDNote.Text = schueler.Data.IsDNoteNull() ? "" : string.Format("{0:F1}", schueler.Data.DNote);
      textBoxDNoteFachgebHSR.Text = schueler.Data.IsDNoteFachgebHSRNull() ? "" : string.Format("{0:F1}", schueler.Data.DNoteFachgebHSR);
    }

    private void SetBackgroundColor(HjLeistung hj, DataGridViewCell cell)
    {
      cell.Style.BackColor = hj.GetBackgroundColor(); ;
    }

    private void FillCell(DataGridViewCell c, HjLeistung hjl)
    {
      if (hjl != null)
      {
        c.Tag = hjl;
        c.Value = hjl.Punkte;
        SetBackgroundColor(hjl, c);
      }
    }

    private void EditHjLeistung(HjLeistung hj)
    {
      string input = InputBox.Show("Neue Notenpunkte:", hj.Punkte.ToString());
      if (input != "")
      {
        hj.Punkte = Convert.ToByte(input, CultureInfo.CurrentUICulture);
        hj.WriteToDB();
      }
    }

    private void ShowCols(int vonCol, int bisCol, bool visible)
    {
      int i;
      for (i = vonCol; i <= bisCol; i++)
        dataGridNoten.Columns[i].Visible = visible;
    }

    private void chkShowHj1_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(3, 6, chkShowHj1.Checked);
      ShowFixedCols();
    }

    private void chkShowHj2_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(8, 11, chkShowHj2.Checked);
      ShowFixedCols();
    }

    private void chkShowAbi_CheckedChanged(object sender, EventArgs e)
    {
      ShowCols(13, 15, chkShowAbi.Checked);
      ShowFixedCols();
    }

    private void ShowFixedCols() // bestimmte Gesamtwert-Spalten bleiben (fast) immer sichtbar
    {
      bool c = chkShowHj2.Checked || chkShowAbi.Checked || !chkShowHj1.Checked && !chkShowHj2.Checked & !chkShowAbi.Checked;
      dataGridNoten.Columns[12].Visible = c; // 2. Hj
      dataGridNoten.Columns[16].Visible = c; // Jahresnote
      dataGridNoten.Columns[17].Visible = c; // GE
    }

    private void setStatus(HjStatus status)
    {
      HjLeistung hj = (HjLeistung)dataGridNoten.SelectedCells[0].Tag;
      hj.SetStatus(status);
      //SetBackgroundColor(hj, dataGridNoten.SelectedCells[0]);
      var berechnungen = new Berechnungen();
      berechnungen.AktualisiereGE(schueler);
      Init();
    }

    private void contextMenu_Opening(object sender, CancelEventArgs e)
    {
      var c = dataGridNoten.SelectedCells[0];
      if (c.Tag == null || !Zugriff.Instance.HatVerwaltungsrechte)
      {
        e.Cancel = true;
        return;
      }

      byte s = (byte)((HjLeistung)c.Tag).Status; // aktueller Status anhaken
      for (int i = 0; i < 5; i++)
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

    private void alternatEinbrToolStripMenuItem_Click(object sender, EventArgs e)
    {
      setStatus(HjStatus.AlternativeEinbr);
    }

    private void dataGridNoten_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
      {
        dataGridNoten.CurrentCell = dataGridNoten.Rows[e.RowIndex].Cells[e.ColumnIndex];
      }
    }

    private void dataGridNoten_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      var c = dataGridNoten.Rows[e.RowIndex].Cells[e.ColumnIndex];
      if (c.Tag == null)
      {
        HjArt art;
        switch (e.ColumnIndex)
        {
          case 1:
          case 7: art = HjArt.Hj1; break;
          case 2:
          case 12: art = HjArt.Hj2; break;
          case 15: art = HjArt.AP; break;
          case 16: art = HjArt.JN; break;
          default: return;
        }
        Jahrgangsstufe jg = (e.ColumnIndex < 3 ? Jahrgangsstufe.Elf : schueler.getKlasse.Jahrgangsstufe);
        HjLeistung hj = new HjLeistung(schueler.Id, (Fach)dataGridNoten.Rows[e.RowIndex].Tag, art, jg);
        EditHjLeistung(hj);
        schueler.ReloadNoten();
      }
      else
        EditHjLeistung((HjLeistung)c.Tag);
      Init();
    }
  }
}
