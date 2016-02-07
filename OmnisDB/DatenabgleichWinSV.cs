using diNo.diNoDataSetTableAdapters;
using System.Data.Odbc;

namespace diNo
{
  public class DatenabgleichWinSV
  {
    private OdbcConnection conn;

    public DatenabgleichWinSV()
    {
      this.conn = new OdbcConnection("DSN=sd");
      conn.Open();
    }

    public void CheckSchuelerdaten()
    {
      var command = conn.CreateCommand();
      command.CommandText = "SELECT * FROM DSchueler";
      var cmdResult = command.ExecuteReader();
      var schuelerTable = new SchuelerTableAdapter();
      while (cmdResult.Read())
      {
        int schuelerId = cmdResult.GetInt32(WinSVSchuelerReader.schuelerIdSpalte);

        if (schuelerTable.GetDataById(schuelerId).Count == 0)
        {
          continue; // Entferne die Schülerdatensätze aus dem Vorjahr oder sonstige Wartelisten- und Spaßdatensätze
        }

        Schueler schueler = new Schueler(schuelerId);
        schueler.Data.Name = cmdResult.GetString(WinSVSchuelerReader.nachnameSpalte);
        schueler.Data.Vorname = cmdResult.GetString(WinSVSchuelerReader.vornameSpalte);
        schueler.Data.Rufname = cmdResult.GetString(WinSVSchuelerReader.rufnameSpalte);
        schueler.Data.Geschlecht = cmdResult.GetString(WinSVSchuelerReader.geschlechtSpalte);
        schueler.Data.Geburtsdatum = cmdResult.GetDate(WinSVSchuelerReader.geburtsdatumSpalte);
        schueler.Data.Geburtsort = cmdResult.GetString(WinSVSchuelerReader.geburtsortSpalte);
        schueler.Data.Bekenntnis = cmdResult.GetString(WinSVSchuelerReader.bekenntnisSpalte);
        schueler.Data.NachnameEltern1 = cmdResult.GetString(WinSVSchuelerReader.nachnameEltern1Spalte);
        schueler.Data.VornameEltern1 = cmdResult.GetString(WinSVSchuelerReader.vornameEltern1Spalte);
        schueler.Data.AnredeEltern1 = cmdResult.GetString(WinSVSchuelerReader.anredeEltern1Spalte);
        schueler.Data.VerwandtschaftsbezeichnungEltern1 = cmdResult.GetString(WinSVSchuelerReader.verwandtschaftsbezeichnungEltern1Spalte);
        schueler.Data.NachnameEltern2 = cmdResult.GetString(WinSVSchuelerReader.nachnameEltern2Spalte);
        schueler.Data.VornameEltern2 = cmdResult.GetString(WinSVSchuelerReader.vornameEltern2Spalte);
        schueler.Data.AnredeEltern2 = cmdResult.GetString(WinSVSchuelerReader.anredeEltern2Spalte);
        schueler.Data.VerwandtschaftsbezeichnungEltern2 = cmdResult.GetString(WinSVSchuelerReader.verwandtschaftsbezeichnungEltern2Spalte);
        schueler.Data.AnschriftPLZ = cmdResult.GetString(WinSVSchuelerReader.anschr1PlzSpalte);
        schueler.Data.AnschriftOrt = cmdResult.GetString(WinSVSchuelerReader.anschr1OrtSpalte);
        schueler.Data.AnschriftStrasse = cmdResult.GetString(WinSVSchuelerReader.anschr1StrasseSpalte);
        schueler.Data.AnschriftTelefonnummer = cmdResult.GetString(WinSVSchuelerReader.anschr1TelefonSpalte);

        // TODO: Klasse und Klassenwechsel: Wollen wir das automatisch? Eher nicht, oder...
        // genauso: Wechsel von Jahrgangsstufen (Rücktritt) oder Ausbildungsrichtungen
        // public const int klasseSpalte = 52;

        // TODO: Auswerten, d.h. aus Kursen An-/Abmelden
        schueler.Data.Fremdsprache2 = cmdResult.GetString(WinSVSchuelerReader.fremdsprache2Spalte);
        schueler.Data.ReligionOderEthik = cmdResult.GetString(WinSVSchuelerReader.reliOderEthikSpalte);
        schueler.Data.Wahlpflichtfach = cmdResult.GetString(WinSVSchuelerReader.wahlpflichtfachSpalte);
        schueler.Data.Wahlfach1 = cmdResult.GetString(WinSVSchuelerReader.wahlfach1Spalte);
        schueler.Data.Wahlfach2 = cmdResult.GetString(WinSVSchuelerReader.wahlfach2Spalte);
        schueler.Data.Wahlfach3 = cmdResult.GetString(WinSVSchuelerReader.wahlfach3Spalte);
        schueler.Data.Wahlfach4 = cmdResult.GetString(WinSVSchuelerReader.wahlfach4Spalte);

        // TODO: Gleichen wir diese ganzen Daten überhaupt ab? Die ändern sich doch nach dem Eintritt nicht (oder gibt es auch hier Korrekturen?)
        // Andererseits wäre es wohl unvollständig, wenn wir manche Daten abgleichen, andere aber nicht.
        /*
    public const int wdh1JahrgangsstufeSpalte = 86;
    public const int wdh2JahrgangsstufeSpalte = 87;
    public const int wdh1GrundSpalte = 91;
    public const int wdh2GrundSpalte = 92;
    public const int probezeitBisSpalte = 98;
    public const int eintrittDatumSpalte = 115;
    public const int eintrittJgstSpalte = 117;
    public const int eintrittVonSchulnummerSpalte = 125;
    public const int austrittsdatumSpalte = 122;
    public const int schulischeVorbildungSpalte = 128;
    public const int beruflicheVorbildungSpalte = 129;
    */

        // TODO: Ab hier stürzt er immer ab (evtl. kann der Treiber nicht so viele Spalten?)
        var schwaeche = cmdResult.GetString(WinSVSchuelerReader.lrsSchwaecheSpalte);
        schueler.Data.LRSSchwaeche = bool.Parse(cmdResult.GetString(WinSVSchuelerReader.lrsSchwaecheSpalte));
        schueler.Data.LRSStoerung = cmdResult.GetBoolean(WinSVSchuelerReader.lrsStoerungSpalte);
        schueler.Data.LRSBisDatum = cmdResult.GetDate(WinSVSchuelerReader.lrsBisDatumSpalte);
        schueler.Data.Email = cmdResult.GetString(WinSVSchuelerReader.emailSpalte);
        schueler.Data.Notfalltelefonnummer = cmdResult.GetString(WinSVSchuelerReader.notfallrufnummerSpalte);

        schueler.Save();
      }
    }
  }
}
