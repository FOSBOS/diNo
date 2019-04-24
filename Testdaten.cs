using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diNo
{

  // erzeugt zufällige HjLeistungen für die ausgewählte Klasse für Testzwecke
  public class Testdaten
  {
    private Random r;
    public Testdaten()
    {
      r = new Random();
    }

    public void ZufallHjLeistung(Schueler s)
    {
      bool FR = s.Fachreferat.Count > 0; // FR vorhanden

      foreach (var f in s.getNoten.alleKurse)
      {
        byte sum;
        sum = Wuerfeln(f,HjArt.Hj1);
        sum +=Wuerfeln(f,HjArt.Hj2);
        sum /= 2;
        
        HjLeistung jn  = f.getHjLeistung(HjArt.JN);
        if (jn==null)
           jn = new HjLeistung(f.schueler.Id, f.getFach, HjArt.JN, f.schueler.getKlasse.Jahrgangsstufe);
        jn.Punkte = sum;
        jn.SchnittMdl = 20;
        jn.WriteToDB();

        if (f.getFach.IstSAPFach(s.Zweig))
          Wuerfeln(f, HjArt.AP);

        if (!FR && r.Next(10)==9)
        {
          Wuerfeln(f, HjArt.FR);
          FR = true;
        }       
      }
    }

    private byte Wuerfeln(FachSchuelerNoten f, HjArt a)
    {
      HjLeistung hj = f.getHjLeistung(a);
      if (hj == null)
      {
        hj = new HjLeistung(f.schueler.Id, f.getFach, a, f.schueler.getKlasse.Jahrgangsstufe);
        hj.Punkte = (byte)r.Next(16);
        hj.SchnittMdl = 20; // sinnloser Wert, um gewürfelte Noten zu erkennen.
        hj.WriteToDB();
        return hj.Punkte;
      }
      else
        return f.getHjLeistung(a).Punkte;
    }

  }
}
