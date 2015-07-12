using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class Notenbogen : Form
  {
    private diNo.diNoDataSet.SchuelerRow schueler = null;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="schuelerId">Die Id des anzuzeigenden Schülers.</param>
    public Notenbogen(int schuelerId)
    {
      InitializeComponent();

      SchuelerTableAdapter schuelerAdapter = new SchuelerTableAdapter();
      var schuelerRows = schuelerAdapter.GetDataById(schuelerId);
      if (schuelerRows.Count == 1)
      {
        schueler = schuelerRows[0];
        nameTextBox.Text = schueler.Name + ", " + schueler.Rufname;

        KlasseTableAdapter klasseAdapter = new KlasseTableAdapter();
        var klassenRows = klasseAdapter.GetDataById(schueler.KlasseId);
        klasseTextBox.Text = klassenRows.Count == 1 ? klassenRows[0].Bezeichnung : "";

        textBoxAdresse.Text = schueler.AnschriftStrasse + "\n" + schueler.AnschriftPLZ + " " + schueler.AnschriftOrt + "\n Tel.:" + schueler.AnschriftTelefonnummer;

        SchuelerKursTableAdapter skAdapter = new SchuelerKursTableAdapter();
        NoteTableAdapter noteAdapter = new NoteTableAdapter();
        FachTableAdapter fachAdapter = new FachTableAdapter();
        KursTableAdapter kursAdapter = new KursTableAdapter();

        // Row[lineCount] für schriftliche und Row[lineCount+1] für mündliche Noten
        int lineCount = 0;
        foreach(var kurs in skAdapter.GetDataBySchuelerId(schuelerId))
        {
          dataGridNoten.Rows.Add(2);
          dataGridNoten.Rows[lineCount + 1].Height += 2; 
          dataGridNoten.Rows[lineCount + 1].DividerHeight = 2;
          
          var kursRow = kursAdapter.GetDataById(kurs.KursId)[0];
          var fachRow = fachAdapter.GetDataById(kursRow.FachId)[0];
          dataGridNoten.Rows[lineCount].Cells[0].Value = fachRow.Bezeichnung;

          IList<diNo.diNoDataSet.NoteRow> noten = new List<diNo.diNoDataSet.NoteRow>();
          foreach (var note in noteAdapter.GetDataBySchuelerAndKurs(kurs.SchuelerId, kurs.KursId))
          {
            noten.Add(note);
          }

          InsertSchulaufgaben(1, lineCount, noten, Halbjahr.Erstes);
          InsertExen(1, lineCount + 1, noten, Halbjahr.Erstes);
          InsertMuendliche(6, lineCount + 1, noten, Halbjahr.Erstes);
          InsertSingleNote(9, lineCount, noten, Notentyp.SchnittSA, Halbjahr.Erstes, true);
          InsertSingleNote(9, lineCount + 1, noten, Notentyp.Schnittmuendlich, Halbjahr.Erstes, true);
          InsertSingleNote(10, lineCount, noten, Notentyp.Jahresfortgang, Halbjahr.Erstes, false);
          InsertSingleNote(10, lineCount + 1, noten, Notentyp.JahresfortgangMitNKS, Halbjahr.Erstes, true);

          InsertSchulaufgaben(11, lineCount, noten, Halbjahr.Zweites);
          InsertExen(11, lineCount + 1, noten, Halbjahr.Zweites);
          InsertMuendliche(16, lineCount + 1, noten, Halbjahr.Zweites);
          InsertSingleNote(19, lineCount + 1, noten, Notentyp.Fachreferat, Halbjahr.Ohne, false);
          InsertSingleNote(20, lineCount, noten, Notentyp.SchnittSA, Halbjahr.Zweites, true);
          InsertSingleNote(20, lineCount + 1, noten, Notentyp.Schnittmuendlich, Halbjahr.Zweites, true);
          InsertSingleNote(21, lineCount, noten, Notentyp.Jahresfortgang, Halbjahr.Zweites, false);
          InsertSingleNote(21, lineCount + 1, noten, Notentyp.JahresfortgangMitNKS, Halbjahr.Zweites, true);

          InsertSingleNote(22, lineCount, noten, Notentyp.APSchriftlich, Halbjahr.Ohne, false);
          InsertSingleNote(23, lineCount, noten, Notentyp.APMuendlich, Halbjahr.Ohne, false);
          InsertSingleNote(24, lineCount, noten, Notentyp.APGesamt, Halbjahr.Ohne, true);
          InsertSingleNote(25, lineCount, noten, Notentyp.Abschlusszeugnis, Halbjahr.Ohne, false);
          lineCount = lineCount + 2;
        }

        if (schueler.Jahrgangsstufe == "11")
        {
          FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
          var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
          if (fpANoten.Count == 1)
          {
            textBoxFpABemerkung.Text = fpANoten[0].Bemerkung;
            listBoxFpA.SelectedIndex = fpANoten[0].Note;
          }
        }

        if (schueler.Jahrgangsstufe == "13")
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
      if (schueler != null && schueler.Jahrgangsstufe == "11")
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

      if (schueler != null && schueler.Jahrgangsstufe == "13")
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
