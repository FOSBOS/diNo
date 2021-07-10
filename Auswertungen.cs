using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//TODO: Achtung vorher alle Kurse mal reinladen (aktuell manuell durch Notenprüfung)

namespace diNo
{
  [Serializable]
  public class AbiErgebnis
  {
    public string Kursbez;
    public string Lehrerkuerzel;
    public string Anzahl;
    public string Hj1;
    public string Hj2;
    public string Abi;

    public void Schreibe(StreamWriter sw)
    {
      sw.WriteLine(Kursbez + ";" + Lehrerkuerzel + ";" + Anzahl + ";" + Hj1 + ";" + Hj2 + ";" + Abi);
    }
  }

  public static class Auswertungen
  {
  
    public static void CopyToClipboard(List<AbiErgebnis> erg)
    {
    //??
      System.IO.MemoryStream mem = new System.IO.MemoryStream();
      BinaryFormatter bin = new BinaryFormatter();
      bin.Serialize(mem, erg);
      
      
      // Construct data format for Slide collection
      DataFormats.Format dataFormat = DataFormats.GetFormat(typeof(List<AbiErgebnis>).FullName);

      // Construct data object from selected slides
      IDataObject dataObject = new DataObject();

      //List<AbiErgebnis> dataToCopy = erg.ToList();
      dataObject.SetData(dataFormat.Name, false, mem);

      // Put data into clipboard
      Clipboard.SetDataObject(dataObject, false);
    }

    public static void AbiSchnitte()
    {
      StreamWriter sw = new StreamWriter(@"C:\tmp\AbiErgebnis.txt");
      //List<AbiErgebnis> ergListe = new List<AbiErgebnis>();
      {
        var erg = new AbiErgebnis();
        erg.Kursbez = "Kurs";
        erg.Lehrerkuerzel = "Lehrer";
        erg.Anzahl = "Anzahl";
        erg.Hj1 = "Hj1";
        erg.Hj2 = "Hj2";
        erg.Abi = "SAP";
        erg.Schreibe(sw);
        //ergListe.Add(erg);
      }
      List<Kurs> kurse = Zugriff.Instance.KursRep.getList();
      foreach (Kurs k in kurse)
      {
        if (k.IstSAPKurs)
        {
          int hj1=0, hj2=0, abi=0, anz=0;
          foreach (Schueler s in k.Schueler)
          {
            var f = s.getNoten.FindeFach(k.getFach.Id);
            int? ap = f.getNote(Halbjahr.Zweites, Notentyp.APSchriftlich);
            if (ap!=null)
            {
              hj1 += f.getHjLeistung(HjArt.Hj1).Punkte;
              hj2 += f.getHjLeistung(HjArt.Hj2).Punkte;
              abi += (int)ap;
              anz++;
            }
          }
          AbiErgebnis erg = new AbiErgebnis();
          erg.Kursbez = k.Data.Bezeichnung;
          erg.Lehrerkuerzel = k.getLehrer.Kuerzel;
          erg.Anzahl = anz.ToString();
          erg.Hj1 = String.Format("{0:0.00}", hj1 / (double)anz);
          erg.Hj2 = String.Format("{0:0.00}", hj2 / (double)anz);
          erg.Abi = String.Format("{0:0.00}", abi / (double)anz);

          //ergListe.Add(erg);
          erg.Schreibe(sw);
        }
      }
      sw.Close();
      //CopyToClipboard(ergListe);
    }
  }
}
