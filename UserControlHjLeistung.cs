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
  public partial class UserControlHjLeistung : UserControl
  {
    private Schueler schueler;

    public UserControlHjLeistung()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Der anzuzeigende Schüler.
    /// </summary>
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

    /// <summary>
    /// Füllt die Felder mit den Daten des Schülers.
    /// </summary>
    public void Init()
    {
      if (this.schueler == null)
      {
        return;
      }

      dataGridHjLeistung.Rows.Clear();
      dataGridHjLeistung.RowsDefaultCellStyle.BackColor = Color.White;

      // keine Abi-Klasse: HjLeistung werden nicht angezeigt
      if (schueler.getKlasse.Jahrgangsstufe<Jahrgangsstufe.Zwoelf)
        return; // TODO: Tab gar nicht anzeigen?!

      // nur FOS 12. hat Leistungen aus der 11. Klasse:
      bool hatVorHj = (schueler.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Zwoelf) && (schueler.Data.Schulart == "F");
      dataGridHjLeistung.Columns[1].Visible = hatVorHj;
      dataGridHjLeistung.Columns[2].Visible = hatVorHj;


      if (schueler.Status == Schuelerstatus.Abgemeldet)
      {        
        dataGridHjLeistung.RowsDefaultCellStyle.BackColor = Color.LightGray;
      }

      int lineCount = 0;
      foreach (var fach in schueler.getNoten.alleFaecher)
      {
        dataGridHjLeistung.Rows.Add();
        dataGridHjLeistung.Rows[lineCount].Height += 2;        
        dataGridHjLeistung.Rows[lineCount].Cells[0].Value = fach.getFach.Bezeichnung;

        FillCell(dataGridHjLeistung.Rows[lineCount].Cells[1], fach.getHjLeistung(HjArt.VorHj1));   
        FillCell(dataGridHjLeistung.Rows[lineCount].Cells[2], fach.getHjLeistung(HjArt.VorHj2));   
        FillCell(dataGridHjLeistung.Rows[lineCount].Cells[3], fach.getHjLeistung(HjArt.Hj1));   
        FillCell(dataGridHjLeistung.Rows[lineCount].Cells[4], fach.getHjLeistung(HjArt.Hj2));   
        FillCell(dataGridHjLeistung.Rows[lineCount].Cells[5], fach.getHjLeistung(HjArt.AP));           
        FillCell(dataGridHjLeistung.Rows[lineCount].Cells[6], fach.getHjLeistung(HjArt.GesErg));

        //FillCell(dataGridHjLeistung.Rows[lineCount].Cells[6], fach.getHjLeistung(HjArt.FR)); // FR als eigene Zeile darstellen

        /*
        if (Zugriff.Instance.aktZeitpunkt > 3 && zeugnis.JahresfortgangGanzzahlig != null) // ab der 2.PA werden die Prüfungsnoten auch farblich hervorgehoben
            {
              SetBackgroundColor((double)zeugnis.JahresfortgangGanzzahlig, dataGridNoten.Rows[lineCount].Cells[23]);
            }
          }        
        */

        lineCount++;
      }
    }

    private void FillCell(DataGridViewCell c, HjLeistung hjl)
    {
      if (hjl!=null)
      {
        c.Value = hjl.Punkte;

        //if (!hjl.Einbringen)
        //  c.DataGridView.BackgroundColor = Color.LightGray;
      }
    }

  }
}
