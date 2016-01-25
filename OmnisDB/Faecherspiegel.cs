using System;

namespace diNo.OmnisDB
{
  public class Faecherspiegel
  {
    // da bei Reli und Ethik sowieso immer nachgeschaut werden muss, ob der Schüler dieses Fach auch wirklich belegt
    // müssen diese beiden (Index=0 und Index=1) sowieso immer gesondert betrachtet werden

    #region Faecherspiegel BOS
    private static string[] faecherKuerzelVORBOS = new[] { "D", "E", "M" }; // Vorkurs

    private static string[] faecherKuerzelAVSBOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Ph", "C", "B"};
    private static string[] faecherKuerzelA12BOS = new[] { "Reli", "Eth", "D", "E", "G", "Sk", "M", "Ph", "C", "B", "TeIn", "Wl" };
    private static string[] faecherKuerzelA13BOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "Ph", "C", "B", "TeIn", "Wl" };

    private static string[] faecherKuerzelSVSBOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "C", "B" };
    private static string[] faecherKuerzelS12BOS = new[] { "Reli", "Eth", "D", "E", "G", "Sk", "M", "C", "B", "PP", "Wl" };
    private static string[] faecherKuerzelS13BOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "C", "B", "PP", "Wl", "Rl" };

    private static string[] faecherKuerzelTVSBOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Ph", "C" };
    private static string[] faecherKuerzelT12BOS = new[] { "Reli", "Eth", "D", "E", "G", "Sk", "M", "Ph", "C", "TeIn" };
    private static string[] faecherKuerzelT13BOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "Ph", "C", "TeIn" };

    private static string[] faecherKuerzelWVSBOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Te", "BwR" };
    private static string[] faecherKuerzelW12BOS = new[] { "Reli", "Eth", "D", "E", "G", "Sk", "M", "Te", "BwR", "VWL", "WIn" };
    private static string[] faecherKuerzelW13BOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "Te", "BwR", "VWL", "WIn" };
    #endregion

    #region Faecherspiegel FOS
    private static string[] faecherKuerzelVORFOS = new[] { "D", "E", "M", "BwR" }; // zumindest dieses Jahr kriegt der Vorkus FOS BwR

    private static string[] faecherKuerzelAVSFOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Ph", "C", "B" };
    private static string[] faecherKuerzelA11FOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Ph", "C", "B", "TeIn", "Wl" };
    private static string[] faecherKuerzelA12FOS = new[] { "Reli", "Eth", "D", "E", "Sk", "M", "Ph", "C", "B", "TeIn", "Wl", "Smw"};
    private static string[] faecherKuerzelA13FOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "Ph", "C", "B", "TeIn", "Wl" };

    private static string[] faecherKuerzelSVSFOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "C", "B" };
    private static string[] faecherKuerzelS11FOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "C", "PP", "Wl", "Mu", "Ku" };
    private static string[] faecherKuerzelS12FOS = new[] { "Reli", "Eth", "D", "E", "Sk", "M", "B", "PP", "Rl", "Wl", "Mu", "Ku", "Smw" };
    private static string[] faecherKuerzelS13FOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "C", "B", "PP", "Wl", "Inf" };

    private static string[] faecherKuerzelTVSFOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Ph", "C" };
    private static string[] faecherKuerzelT11FOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Ph", "C", "TeIn", "TZ" };
    private static string[] faecherKuerzelT12FOS = new[] { "Reli", "Eth", "D", "E", "Sk", "M", "Ph", "C", "TeIn", "Smw" };
    private static string[] faecherKuerzelT13FOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "Ph", "C", "TeIn" };

    private static string[] faecherKuerzelWVSFOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "Te", "BwR" };
    private static string[] faecherKuerzelW11FOS = new[] { "Reli", "Eth", "D", "E", "G", "M", "BwR", "WIn", "F-Wi", "Rl" };
    private static string[] faecherKuerzelW12FOS = new[] { "Reli", "Eth", "D", "E", "Sk", "M", "Te", "BwR", "VWL", "WIn", "Smw" };
    private static string[] faecherKuerzelW13FOS = new[] { "Reli", "Eth", "D", "E", "GSk", "M", "Te", "BwR", "VWL", "WIn", "F-Wi" };
    #endregion

    private OmnisConnection omnis;

    public Faecherspiegel()
    {
      this.omnis = new OmnisConnection();
    }

    /// <summary>
    /// Sucht das Fach am angegebenen Index aus dem angegebenen Fächerspiegel.
    /// </summary>
    /// <param name="faecherspiegel">Welcher Fächerspiegel, z. B. W11.</param>
    /// <param name="index">Der Index des Faches.</param>
    /// <param name="schulart">FOS oder BOS.</param>
    /// <param name="schueler">Den Schüler brauchen wir auch, um zu ermitteln ob er katholisch oder evangelisch ist.</param>
    /// <param name="zeitpunkt">Der Zeitpunkt, für welchen wir die Note ermitteln müssen.</param>
    /// <returns>Das Fach oder null, wenn kein weiteres Fach mehr im Fächerspiegel vorhanden ist.</returns>
    public string GetFachNoteString(string faecherspiegel, int index, Schulart schulart, Schueler schueler, Zeitpunkt zeitpunkt)
    {
      string faecherKuerzel = omnis.SucheFach(faecherspiegel, index, schulart);
      /* var faecher = FindeDenRichtigenFaecherspiegel(faecherspiegel, schulart);
       if (faecher.Length <= index)
       {
         return string.Empty;
       }

       string faecherKuerzel = faecher[index];
       */
      if (faecherKuerzel.Equals("Rel", StringComparison.OrdinalIgnoreCase))
      {
        // Relinote - je nachdem, ob Schüler Evangelisch oder Katholisch ist. Geht er in Ethik: "-"
        var kath = schueler.getNoten.FindeFach("K", false);
        if (kath != null)
          return GetNotenString(kath, zeitpunkt);

        var ev = schueler.getNoten.FindeFach("Ev", false);
        if (ev != null)
          return GetNotenString(ev, zeitpunkt);

        return "-"; //offenbar weder kath. noch ev.
      }
      if (faecherKuerzel.Equals("Eth", StringComparison.OrdinalIgnoreCase))
      {
        //Ethiknote (wenn der Schüler in Ethik geht, sonst "-")
        var ethik = schueler.getNoten.FindeFach("Eth", false);
        return ethik != null ? GetNotenString(ethik, zeitpunkt) : "-";
      }

      var dieRichtigeNote = schueler.getNoten.FindeFach(faecherKuerzel, false);
      if (dieRichtigeNote == null)
      {
        //Todo: log schreiben oder Exception werfen?
        return "-";
      }
      else
      {
        return GetNotenString(dieRichtigeNote, zeitpunkt);
      }
    }

    public string GetNotenString(FachSchuelerNoten note, Zeitpunkt zeitpunkt)
    {
      byte? relevanteNote = note.getRelevanteNote(zeitpunkt); 
      return relevanteNote == null ? "-" : string.Format("{0:00}", (byte)relevanteNote); //wichtig: Im Feld muss 08 stehen, nicht 8 (d.h. 2 Ziffern)
    }

    private string[] FindeDenRichtigenFaecherspiegel (string faecherspiegel, Schulart schulart)
    {
      if (schulart == Schulart.FOS)
      {
        switch (faecherspiegel)
        {
          case "VOR": return faecherKuerzelVORFOS;
          case "AVS": return faecherKuerzelAVSFOS;
          case "A11": return faecherKuerzelA11FOS;
          case "A12": return faecherKuerzelA12FOS;
          case "A13": return faecherKuerzelA13FOS;
          case "SVS": return faecherKuerzelSVSFOS;
          case "S11": return faecherKuerzelS11FOS;
          case "S12": return faecherKuerzelS12FOS;
          case "S13": return faecherKuerzelS13FOS;
          case "TVS": return faecherKuerzelTVSFOS;
          case "T11": return faecherKuerzelT11FOS;
          case "T12": return faecherKuerzelT12FOS;
          case "T13": return faecherKuerzelT13FOS;
          case "WVS": return faecherKuerzelWVSFOS;
          case "W11": return faecherKuerzelW11FOS;
          case "W12": return faecherKuerzelW12FOS;
          case "W13": return faecherKuerzelW13FOS;
          default: throw new InvalidOperationException("unbekannter Fächerspiegel für die FOS : " + faecherspiegel);
        }
      }

      if (schulart == Schulart.BOS)
      {
        switch (faecherspiegel)
        {
          case "VOR": return faecherKuerzelVORBOS;
          case "AVS": return faecherKuerzelAVSBOS;
          case "A12": return faecherKuerzelA12BOS;
          case "A13": return faecherKuerzelA13BOS;
          case "SVS": return faecherKuerzelSVSBOS;
          case "S12": return faecherKuerzelS12BOS;
          case "S13": return faecherKuerzelS13BOS;
          case "TVS": return faecherKuerzelTVSBOS;
          case "T12": return faecherKuerzelT12BOS;
          case "T13": return faecherKuerzelT13BOS;
          case "WVS": return faecherKuerzelWVSBOS;
          case "W12": return faecherKuerzelW12BOS;
          case "W13": return faecherKuerzelW13BOS;
          default: throw new InvalidOperationException("unbekannter Fächerspiegel für die BOS : " + faecherspiegel);
        }
      }

      throw new InvalidOperationException("ungültige Schulart zum Suchen eines Fächerspiegels "+schulart);
    }
  }
}
