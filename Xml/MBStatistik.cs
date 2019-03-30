using System;
using System.IO;
using System.Xml.Serialization;
using diNo.Xml.Mbstatistik;

namespace diNo.Xml
{
  public class MBStatistik
  {
    public static void Serialize(String fileName)
    {
      abschlusspruefungsstatistik ap = new abschlusspruefungsstatistik();

      // hier wird die Abschlusspruefungsstatistik zusammengebaut







      using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        XmlSerializer ser = new XmlSerializer(typeof(abschlusspruefungsstatistik));
        ser.Serialize(stream, ap);
      }
    }
  }
}
