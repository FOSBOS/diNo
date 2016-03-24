using diNo.diNoDataSetTableAdapters;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace diNo
{
  /// <summary>
  /// Liest die Lehrerdatei ein.
  /// </summary>
  public class LehrerFileReader
  {
    private const int kuerzelSpalte = 0;
    private const int nachnameSpalte = 1;
    private const int vornameSpalte = 2;
    private const int dienstbezeichnungSpalte = 3;

    /// <summary>
    /// Eigentliche Lese-Methode.
    /// </summary>
    /// <param name="fileName">Der dateiname.</param>
    public static void Read(string fileName)
    {
      using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("iso-8859-1")))
      {
        while (!reader.EndOfStream)
        {
          string line = reader.ReadLine();
          if (string.IsNullOrEmpty(line))
          {
            continue;
          }

          string[] array = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
          string[] cleanArray = array.Select(aString => aString.Trim(new char[] { '\"', ' ', '\n' })).ToArray();

          using (LehrerTableAdapter adapter = new LehrerTableAdapter())
          {
            if (adapter.GetDataByKuerzel(cleanArray[kuerzelSpalte]).Count == 0)
            {
              string mailAdresse = cleanArray[vornameSpalte] + "." + cleanArray[nachnameSpalte] + "@fosbos-kempten.de";
              mailAdresse = ReplaceUmlaute(mailAdresse);
              string windowsname = cleanArray[vornameSpalte].Substring(0, 1) + ReplaceUmlaute(cleanArray[nachnameSpalte]);
              adapter.Insert(cleanArray[kuerzelSpalte], cleanArray[vornameSpalte] + " " + cleanArray[nachnameSpalte], cleanArray[vornameSpalte], cleanArray[nachnameSpalte], cleanArray[dienstbezeichnungSpalte], mailAdresse, windowsname);
            }
          }
        }
      }
    }

    private static string ReplaceUmlaute(string mailAdresse)
    {
      mailAdresse = mailAdresse.Replace("ä", "ae");
      mailAdresse = mailAdresse.Replace("ö", "oe");
      mailAdresse = mailAdresse.Replace("ü", "ue");
      mailAdresse = mailAdresse.Replace("ß", "ss");
      return mailAdresse;
    }
  }
}
