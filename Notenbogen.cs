using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class Notenbogen : Form
  {
    private Schueler schueler;
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="schuelerId">Die Id des anzuzeigenden Schülers.</param>
    public Notenbogen(int schuelerId)
    {
        log.Debug("Öffne Notenbogen SchülerId=" + schuelerId);
        InitializeComponent();

        schueler = new Schueler(schuelerId);
        nameTextBox.Text = schueler.Name;
        klasseTextBox.Text = schueler.Klasse;
        textBoxAdresse.Text = schueler.Data.AnschriftStrasse + "\n" + schueler.Data.AnschriftPLZ + " " + schueler.Data.AnschriftOrt + "\n Tel.:" + schueler.Data.AnschriftTelefonnummer;

        SchuelerKursTableAdapter skAdapter = new SchuelerKursTableAdapter();
        NoteTableAdapter noteAdapter = new NoteTableAdapter();
        FachTableAdapter fachAdapter = new FachTableAdapter();
        KursTableAdapter kursAdapter = new KursTableAdapter();
        BerechneteNoteTableAdapter berechneteNoteAdapter = new BerechneteNoteTableAdapter();

        // Row[lineCount] für schriftliche und Row[lineCount+1] für mündliche Noten
        int lineCount = 0;
        foreach(var kurs in skAdapter.GetDataBySchuelerId(schuelerId))
        {
          var kursRow = kursAdapter.GetDataById(kurs.KursId)[0];
          var fachRow = fachAdapter.GetDataById(kursRow.FachId)[0];
          
          IList<diNo.diNoDataSet.NoteRow> noten = new List<diNo.diNoDataSet.NoteRow>();
          foreach (var note in noteAdapter.GetDataBySchuelerAndKurs(kurs.SchuelerId, kurs.KursId))
          {
            noten.Add(note);
          }

          var berechneteNoten = berechneteNoteAdapter.GetDataBySchuelerAndKurs(kurs.KursId, kurs.SchuelerId);

          if (noten.Count == 0 || berechneteNoten.Count == 0)
          {
            continue;
          }

          dataGridNoten.Rows.Add(2);
          dataGridNoten.Rows[lineCount + 1].Height += 2;
          dataGridNoten.Rows[lineCount + 1].DividerHeight = 2;
          dataGridNoten.Rows[lineCount].Cells[0].Value = fachRow.Bezeichnung;
          var berechneteErstesHJ = berechneteNoten.First(x => x.ErstesHalbjahr);
          var berechneteZweitesHJ = berechneteNoten.First(x => !x.ErstesHalbjahr);

          InsertSchulaufgaben(1, lineCount, noten, Halbjahr.Erstes);
          InsertExen(1, lineCount + 1, noten, Halbjahr.Erstes);
          InsertMuendliche(6, lineCount + 1, noten, Halbjahr.Erstes);

          dataGridNoten.Rows[lineCount].Cells[9].Value = berechneteErstesHJ.SchnittSchulaufgaben.ToString();
          dataGridNoten.Rows[lineCount + 1].Cells[9].Value = berechneteErstesHJ.SchnittMuendlich.ToString();
          dataGridNoten.Rows[lineCount].Cells[10].Value = ((int)berechneteErstesHJ.JahresfortgangGanzzahlig).ToString();
          dataGridNoten.Rows[lineCount + 1].Cells[10].Value = berechneteErstesHJ.JahresfortgangMitKomma.ToString();

          InsertSchulaufgaben(11, lineCount, noten, Halbjahr.Zweites);
          InsertExen(11, lineCount + 1, noten, Halbjahr.Zweites);
          InsertMuendliche(16, lineCount + 1, noten, Halbjahr.Zweites);
          InsertSingleNote(19, lineCount + 1, noten, Notentyp.Fachreferat, Halbjahr.Ohne, false);

          dataGridNoten.Rows[lineCount].Cells[20].Value = berechneteZweitesHJ.SchnittSchulaufgaben.ToString();
          dataGridNoten.Rows[lineCount + 1].Cells[20].Value = berechneteZweitesHJ.SchnittMuendlich.ToString();
          dataGridNoten.Rows[lineCount].Cells[21].Value = ((int)berechneteZweitesHJ.JahresfortgangGanzzahlig).ToString();
          dataGridNoten.Rows[lineCount + 1].Cells[21].Value = berechneteZweitesHJ.JahresfortgangMitKomma.ToString();

          InsertSingleNote(22, lineCount, noten, Notentyp.APSchriftlich, Halbjahr.Ohne, false);
          InsertSingleNote(23, lineCount, noten, Notentyp.APMuendlich, Halbjahr.Ohne, false);

          dataGridNoten.Rows[lineCount].Cells[24].Value = berechneteZweitesHJ.PruefungGesamt.ToString();
          dataGridNoten.Rows[lineCount].Cells[25].Value = berechneteZweitesHJ.Abschlusszeugnis.ToString();

          lineCount = lineCount + 2;
        }

        if (schueler.Data.Jahrgangsstufe == "11")
        {
          FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
          var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
          if (fpANoten.Count == 1)
          {
            textBoxFpABemerkung.Text = fpANoten[0].Bemerkung;
            listBoxFpA.SelectedIndex = fpANoten[0].Note;
          }
        }

        if (schueler.Data.Jahrgangsstufe == "13")
        {
          SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
          var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
          if (seminarfachnoten.Count == 1)
          {
            numericUpDownSeminarfach.Value = seminarfachnoten[0].Gesamtnote;
            textBoxSeminarfachthemaKurz.Text = seminarfachnoten[0].ThemaKurz;
            textBoxSeminarfachthemaLang.Text = seminarfachnoten[0].ThemaLang;
          }
        }      
    }

    /// <summary>
    /// Fügt eine Einzelnote in den Notenbogen ein.
    /// </summary>
    /// <param name="col">Die Spalte.</param>
    /// <param name="row">Die Zeile.</param>
    /// <param name="noten">Die Noten (zum Filtern).</param>
    /// <param name="typ">Der Typ der gesuchten Note.</param>
    /// <param name="halbjahr">Das Halbjahr.</param>
    /// <param name="isCommaValue">true wenn die Note als Kommazahl eingetragen werden soll.</param>
    private void InsertSingleNote(int col, int row, IList<diNo.diNoDataSet.NoteRow> noten, Notentyp typ, Halbjahr halbjahr, bool isCommaValue)
    {
      var note = GetSingleNote(noten, typ, halbjahr);
      if (note != null)
      {
        dataGridNoten.Rows[row].Cells[col].Value = isCommaValue ? note.Punktwert.ToString() : ((int)note.Punktwert).ToString();
      }
      else if (halbjahr == Halbjahr.Ohne)
      {
        // bei manchen Noten (z. B. Fachreferat) muss in beiden Halbjahren gesucht werden.
        note = GetSingleNote(noten, typ, Halbjahr.Erstes);
        if (note == null)
        {
          note = GetSingleNote(noten, typ, Halbjahr.Zweites);
        }
        if (note != null)
        {
          dataGridNoten.Rows[row].Cells[col].Value = isCommaValue ? note.Punktwert.ToString() : ((int)note.Punktwert).ToString();
        }
      }
    }

    private diNo.diNoDataSet.NoteRow GetSingleNote(IList<diNo.diNoDataSet.NoteRow> noten, Notentyp typ, Halbjahr halbjahr)
    {
      var result = noten.Where(x => x.Halbjahr == (int)halbjahr && (Notentyp)x.Notenart == typ);
      if (result.Count() > 1)
      {
        throw new InvalidOperationException("Mehr als eine Note gefunden, obwohl die Note nur einmal vorhanden sein sollte.");
      }

      foreach (var aNote in result)
      {
        return aNote;
      }

      return null;
    }

    private void InsertMuendliche(int startCol, int startRow, IList<diNo.diNoDataSet.NoteRow> noten, Halbjahr halbjahr)
    {
      foreach (var note in noten.Where(x => x.Halbjahr == (int)halbjahr && (Notentyp)x.Notenart == Notentyp.EchteMuendliche).OrderBy(x => x.Zelle))
      {
        dataGridNoten.Rows[startRow].Cells[startCol].Value = (int)note.Punktwert;
        startCol++;
      }
    }

    private void InsertExen(int startCol, int startRow, IList<diNo.diNoDataSet.NoteRow> noten, Halbjahr halbjahr)
    {
      foreach (var note in noten.Where(x => x.Halbjahr == (int)halbjahr && ((Notentyp)x.Notenart == Notentyp.Kurzarbeit || (Notentyp)x.Notenart == Notentyp.Ex)).OrderBy(x => x.Zelle))
      {
        dataGridNoten.Rows[startRow].Cells[startCol].Value = (int)note.Punktwert + ((Notentyp)note.Notenart == Notentyp.Kurzarbeit ? "(K)" : "");
        startCol++;
      }
    }

    private void InsertSchulaufgaben(int startCol, int startRow, IList<diNo.diNoDataSet.NoteRow> noten, Halbjahr halbjahr)
    {
      foreach (var note in noten.Where(x => x.Halbjahr == (int)halbjahr && (Notentyp)x.Notenart == Notentyp.Schulaufgabe).OrderBy(x => x.Zelle))
      {
        dataGridNoten.Rows[startRow].Cells[startCol].Value = (int)note.Punktwert;
        startCol++;
      }
    }

    private void buttonSpeichern_Click(object sender, EventArgs e)
    {
      if (schueler != null && schueler.Data.Jahrgangsstufe == "11")
      {
        FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
        var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
        if (fpANoten.Count == 1)
        {
          fpAAdapter.Update(listBoxFpA.SelectedIndex, textBoxFpABemerkung.Text, schueler.Id);
        }
        else
        {
          fpAAdapter.Insert(schueler.Id, listBoxFpA.SelectedIndex, textBoxFpABemerkung.Text);
        }
      }

      if (schueler != null && schueler.Data.Jahrgangsstufe == "13")
      {
        SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
        var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
        if (seminarfachnoten.Count == 1)
        {
          seminarfachAdapter.Update((int)numericUpDownSeminarfach.Value, textBoxSeminarfachthemaLang.Text, textBoxSeminarfachthemaKurz.Text, schueler.Id);
        }
        else
        {
          seminarfachAdapter.Insert(schueler.Id, (int)numericUpDownSeminarfach.Value, textBoxSeminarfachthemaLang.Text, textBoxSeminarfachthemaKurz.Text);
        }
      }
    }

    private void btnSchliessen_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
