
namespace diNo
{
  /// <summary>
  /// Enthält die Konstanten, in welcher Zelle welche Noten stehen.
  /// </summary>
  internal static class CellConstant
  {
    public static string Klassenbezeichnung = "B1";
    public static string Wertungsart = "C1";
    public static string Fachbezeichnung = "F1";
    public static string Lehrer = "M1";
    public static string Schuljahr = "S1";
    public static string DatumStand = "Z1";
    public static int ZeileErsterSchueler = 5;
    
    #region Konstanten geben das Feld für den ersten Schüler an.
    
    public static string Nachname = "B";
    public static string[] SchulaufgabenErstesHJ = new[] { "D", "E", "F" };
    public static string ZeugnisnoteErstesHJ = "O";
    public static string[] SchulaufgabenZweitesHJ = new[] { "P", "Q", "R" };
    public static string LegasthenieVermerk = "Y";
    public static string LegasthenieEintragung = "Legasthenie";
    public static string ZeugnisnoteZweitesHJ = "AA";

    #endregion

    #region Ab hier die untere Zeile der beiden Zeilen eines Schuelers.

    public static string Vorname = "B";
    public static string[] ExenErstesHJ = new[] { "D", "E", "F", "G" };
    public static string[] MuendlicheErstesHJ = new[] { "H", "I", "J" };
    public static string EPErstesHJ = "K";
    public static string FachreferatErstesHJ = "L";
    public static string SchnittSchulaufgabenErstesHJ = "M";
    public static string SchnittMuendlicheUndExenErstesHJ = "N";
    public static string SchnittGesamtErstesHJ = "O";
    public static string[] ExenZweitesHJ = new[] { "P", "Q", "R", "S" };
    public static string[] MuendlicheZweitesHJ = new[] { "T", "U", "V" };
    public static string EPZweitesHJ = "W";
    public static string FachreferatZweitesHJ = "X";
    public static string SchnittSchulaufgabenZweitesHJ = "Y";
    public static string SchnittMuendlicheUndExenZweitesHJ = "Z";
    public static string SchnittGesamtZweitesHJ = "AA";

    #endregion

    public static int GewichteSchulaufgaben = 75;
    public static int GewichteExen = 76;

    #region AP

    public static string APschriftlichSpalte = "E";
    public static string APmuendlichSpalte = "F";
    public static string APgesamtSpalte = "G";
    public static string APZeugnisnote = "I";
    public static int APZeileErsterSchueler = 6;

    #endregion

    #region diNo
    /// <summary>
    /// Achtung diese Konstanten liegen auf einem anderen Datenblatt "diNo".
    /// </summary>
    public static string SId = "C";
    public static int zeileSIdErsterSchueler = 4;
    public static string KursId = "F2";

    #endregion

    #region Notenschluessel
    /// <summary>
    /// M oder E zulässig
    /// </summary>
    public static string SchluesselArt ="H32";

    public static string ProzentFuenfUntergrenze = "H34";
    public static string ProzentFuenfObergrenze = "H35";

    #endregion
  }
}
