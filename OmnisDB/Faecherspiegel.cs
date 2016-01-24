using System;

namespace diNo.OmnisDB
{
  public class Faecherspiegel
  {
    private static string[] faecherKuerzelW11FOS = new[] { "rK", "Eth", "D" }; // TODO: Im Programm angucken
    private static string[] faecherKuerzelW11BOS = new[] { "rK", "Eth", "D" }; // TODO: Im Programm angucken

    /// <summary>
    /// Sucht das Fach am angegebenen Index aus dem angegebenen Fächerspiegel.
    /// </summary>
    /// <param name="faecherspiegel">Welcher Fächerspiegel, z. B. W11.</param>
    /// <param name="index">Der Index des Faches.</param>
    /// <param name="schulart">FOS oder BOS.</param>
    /// <returns>Das Fach oder null, wenn kein weiteres Fach mehr im Fächerspiegel vorhanden ist.</returns>
    public Fach GetFach(string faecherspiegel, int index, Schulart schulart)
    {
      var faecher = FindeDenRichtigenFaecherspiegel(faecherspiegel, schulart);
      if (faecher.Length <= index)
      {
        return null;
      }

      return FindeFachByKuerzel(faecher[index]);
    }

    private string[] FindeDenRichtigenFaecherspiegel (string faecherspiegel, Schulart schulart)
    {
      if (schulart == Schulart.FOS)
      {
        switch (faecherspiegel)
        {
          case "W11": return faecherKuerzelW11FOS;
          default: throw new InvalidOperationException("unbekannter Fächerspiegel für die FOS : " + faecherspiegel);
        }
      }

      if (schulart == Schulart.BOS)
      {
        switch (faecherspiegel)
        {
          case "W11": return faecherKuerzelW11BOS;
          default: throw new InvalidOperationException("unbekannter Fächerspiegel für die BOS : " + faecherspiegel);
        }
      }

      throw new InvalidOperationException("ungültige Schulart zum Suchen eines Fächerspiegels "+schulart);
    }

    private Fach FindeFachByKuerzel(string kuerzel)
    {
      throw new NotImplementedException("diese Methode fehlt noch");
    }

  }
}
