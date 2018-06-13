using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class Notenbogen : UserControl
  {
    private Schueler schueler;
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// leerer Konstruktor ist Pflicht für UserControls
    /// </summary>
    public Notenbogen()
    {
      InitializeComponent();
      dataGridNoten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // sollte das Neuzeichnen schneller machen
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

      dataGridNoten.Rows.Clear();
      dataGridNoten.RowsDefaultCellStyle.BackColor = Color.White;
      log.Debug("Öffne Notenbogen SchülerId=" + this.schueler.Id);

      var dieNoten = schueler.getNoten.alleKurse;
      if (schueler.Status == Schuelerstatus.Abgemeldet)
      {
        //dieNoten = schueler.getNoten.SucheAlteNoten();
        dataGridNoten.RowsDefaultCellStyle.BackColor = Color.LightGray;
      }

      SetzeSichtbarkeitDerSpalten(dieNoten);

      // Row[lineCount] für schriftliche und Row[lineCount+1] für mündliche Noten
      int lineCount = 0;
      foreach (var kursNoten in dieNoten)
      {
        dataGridNoten.Rows.Add(2);
        dataGridNoten.Rows[lineCount + 1].Height += 2;
        dataGridNoten.Rows[lineCount + 1].DividerHeight = 2;
        dataGridNoten.Rows[lineCount].Cells[0].Value = kursNoten.getFach.Bezeichnung;

        InsertNoten(1, lineCount, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Schulaufgabe), false);
        InsertNoten(1, lineCount + 1, kursNoten.sonstigeLeistungen(Halbjahr.Erstes));

        InsertNoten(10, lineCount, kursNoten.getNoten(Halbjahr.Zweites, Notentyp.Schulaufgabe), false);
        InsertNoten(10, lineCount + 1, kursNoten.sonstigeLeistungen(Halbjahr.Zweites));

        InsertSchnitt(8, lineCount, kursNoten.getSchnitt(Halbjahr.Erstes));
        BerechneteNote zeugnis = kursNoten.getSchnitt(Halbjahr.Zweites);
        InsertSchnitt(18, lineCount, zeugnis);

        InsertNoten(20, lineCount, kursNoten.getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich), Zugriff.Instance.aktZeitpunkt == 4); // AP Einzelergebnisse nur zur zweiten PA hervorheben
        InsertNoten(20, lineCount + 1, kursNoten.getNoten(Halbjahr.Zweites, Notentyp.APMuendlich), Zugriff.Instance.aktZeitpunkt == 4);

        if (zeugnis != null)
        {
          if (zeugnis.PruefungGesamt != null)
          {
            dataGridNoten.Rows[lineCount].Cells[21].Value = zeugnis.PruefungGesamt;
            dataGridNoten.Rows[lineCount].Cells[22].Value = zeugnis.SchnittFortgangUndPruefung;
            dataGridNoten.Rows[lineCount].Cells[23].Value = zeugnis.Abschlusszeugnis;
            if (Zugriff.Instance.aktZeitpunkt > 3) // ab der 2.PA werden die Prüfungsnoten auch farblich hervorgehoben
            {
              SetBackgroundColor(Convert.ToDouble(zeugnis.PruefungGesamt), dataGridNoten.Rows[lineCount].Cells[21]);
              SetBackgroundColor(Convert.ToDouble(zeugnis.SchnittFortgangUndPruefung), dataGridNoten.Rows[lineCount].Cells[22]);
              SetBackgroundColor(Convert.ToDouble(zeugnis.Abschlusszeugnis), dataGridNoten.Rows[lineCount].Cells[23]);
            }
          }
          else
          {
            dataGridNoten.Rows[lineCount].Cells[23].Value = zeugnis.JahresfortgangGanzzahlig;
            if (Zugriff.Instance.aktZeitpunkt > 3 && zeugnis.JahresfortgangGanzzahlig != null) // ab der 2.PA werden die Prüfungsnoten auch farblich hervorgehoben
            {
              SetBackgroundColor(Convert.ToDouble(zeugnis.JahresfortgangGanzzahlig), dataGridNoten.Rows[lineCount].Cells[23]);
            }
          }
        }

        lineCount = lineCount + 2;
      }
    }

    private void SetzeSichtbarkeitDerSpalten(List<FachSchuelerNoten> dieNoten)
    {
      int maxAnzahlNotenHJ1 = 3; //zeige minimal 3 Spalten pro Halbjahr an
      int maxAnzahlNotenHJ2 = 3;
      foreach (var kursNoten in dieNoten)
      {
        var benoetigteSpaltenHJ1 = getAnzahlBenoetigterSpalten(Halbjahr.Erstes, kursNoten);
        maxAnzahlNotenHJ1 = maxAnzahlNotenHJ1 <= benoetigteSpaltenHJ1 ? benoetigteSpaltenHJ1 : maxAnzahlNotenHJ1;
        var benoetigteSpaltenHJ2 = getAnzahlBenoetigterSpalten(Halbjahr.Zweites, kursNoten);
        maxAnzahlNotenHJ2 = maxAnzahlNotenHJ2 <= benoetigteSpaltenHJ2 ? benoetigteSpaltenHJ2 : maxAnzahlNotenHJ2;
      }

      for (int i = 1; i < 8; i++)
      {
        dataGridNoten.Columns[i].Visible = i - 1 < maxAnzahlNotenHJ1;
        dataGridNoten.Columns[i].DividerWidth = (i == maxAnzahlNotenHJ1) ? 10 : 0;
        dataGridNoten.Columns[i].Width = (i == maxAnzahlNotenHJ1) ? 45 : 35;
      }

      // ab dem Vorlauf der 1. PA interessiert das Zwischenzeugnis nicht mehr
      dataGridNoten.Columns[8].Visible = (Zugriff.Instance.aktZeitpunkt <= 2);
      dataGridNoten.Columns[9].Visible = (Zugriff.Instance.aktZeitpunkt <= 2);

      for (int i = 10; i < 18; i++)
      {
        dataGridNoten.Columns[i].Visible = i - 10 < maxAnzahlNotenHJ2;
      }

    }

    private int getAnzahlBenoetigterSpalten(Halbjahr hj, FachSchuelerNoten noten)
    {
      return noten.getNoten(hj, Notentyp.EchteMuendliche).Count +
        noten.getNoten(hj, Notentyp.Ersatzprüfung).Count +
        noten.getNoten(hj, Notentyp.Ex).Count +
        noten.getNoten(hj, Notentyp.Fachreferat).Count +
        noten.getNoten(hj, Notentyp.Kurzarbeit).Count;
    }

    // schreibt eine Notenliste (z.B. alle SA in Englisch aus dem 1. Hj. ins Grid), bez wird als Text an jede Note angefügt    
    private void InsertNoten(int startCol, int startRow, IList<string> noten)
    {
      foreach (var note in noten)
      {
        var cell = dataGridNoten.Rows[startRow].Cells[startCol];
        cell.Value = note;
      //  SetBackgroundColor(note, cell);
        startCol++;
      }
    }

    private void InsertNoten(int startCol, int startRow, IList<int> noten, bool setzeFarbe)
    {
      foreach (var note in noten)
      {
        var cell = dataGridNoten.Rows[startRow].Cells[startCol];
        cell.Value = note;
        if (setzeFarbe)
        {
          SetBackgroundColor(note, cell);
        }
        startCol++;
      }
    }

    // schreibt eine Schnittkonstellation ins Grid    
    private void InsertSchnitt(int startCol, int startRow, BerechneteNote b)
    {
      if (b != null)
      {
        if (b.SchnittSchulaufgaben != null)
        {
          var cell = dataGridNoten.Rows[startRow].Cells[startCol];
          cell.Value = b.SchnittSchulaufgaben;
        }

        if (b.SchnittMuendlich != null)
        {
          dataGridNoten.Rows[startRow + 1].Cells[startCol].Value = b.SchnittMuendlich;
        }

        if (b.JahresfortgangMitKomma != null)
        {
          dataGridNoten.Rows[startRow + 1].Cells[startCol + 1].Value = b.JahresfortgangMitKomma;
          if ((b.ErstesHalbjahr && Zugriff.Instance.aktZeitpunkt <= 2) || (!b.ErstesHalbjahr && Zugriff.Instance.aktZeitpunkt == 3))
          {
            SetBackgroundColor((double)b.JahresfortgangMitKomma, dataGridNoten.Rows[startRow + 1].Cells[startCol + 1]);
          }
        }

        if (b.JahresfortgangGanzzahlig != null)
        {
          dataGridNoten.Rows[startRow].Cells[startCol + 1].Value = b.JahresfortgangGanzzahlig;
          if ((b.ErstesHalbjahr && Zugriff.Instance.aktZeitpunkt <= 2) || (!b.ErstesHalbjahr && Zugriff.Instance.aktZeitpunkt == 3))
          {
            SetBackgroundColor((double)b.JahresfortgangGanzzahlig, dataGridNoten.Rows[startRow].Cells[startCol + 1]);
          }
        }
      }
    }

    private void SetBackgroundColor(string note, DataGridViewCell cell)
    {
      
      double notenwert = double.MaxValue;
      if (!double.TryParse(note, out notenwert))
      {
        return; // nur zur Sicherheit: Wenn das parsen nicht klappt, muss man ja nicht gleich abstürzen
      }

      SetBackgroundColor(notenwert, cell);
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
      // Ab der zweiten PA interessieren Vierer niemanden mehr
      if (aktuellerZeitpunkt < (int)Zeitpunkt.ZweitePA)
      {
        if (notenwert < 4.5) return Color.Yellow;
        if (notenwert < 5.5) return Color.LightYellow;
      }

      if (notenwert > 13.5) return Color.LimeGreen;
      if (notenwert > 11.5) return Color.PaleGreen;
      return dataGridNoten.BackgroundColor;
    }

  }
}
