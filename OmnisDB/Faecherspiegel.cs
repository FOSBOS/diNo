using log4net;
using System;

namespace diNo.OmnisDB
{
  public class Faecherspiegel
  {
    // da bei Reli und Ethik sowieso immer nachgeschaut werden muss, ob der Schüler dieses Fach auch wirklich belegt
    // müssen diese beiden (Index=0 und Index=1) sowieso immer gesondert betrachtet werden

    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
      if (string.IsNullOrEmpty(faecherKuerzel))
      {
        return ""; // Wenn kein sinnvolles Fach mehr kommt, bleibt das Notenfeld leer
      }

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
        log.Warn(schueler.NameVorname + " sollte in " + faecherKuerzel + " gehen, aber diese Zuordnung findet diNo nicht!");
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
  }
}
