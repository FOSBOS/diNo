using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//TODO: Achtung vorher alle Kurse mal reinladen (aktuell manuell durch Notenprüfung)

namespace diNo
{
  /*
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
  */
  public static class Auswertungen
  {
    /*
    public static void CopyToClipboard(List<AbiErgebnis> erg)
    {
    // funktioniert nicht??
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
    */
    public static void AbiSchnitte()
    {
      string erg;      
      erg =  "Kurs\tLehrer\tAnzahl\tHj1\tHj2\tSAP\r\n";

      // alle Kurse laden
      var ta = new KursTableAdapter();
      diNoDataSet.KursDataTable dtFach = ta.GetData();
      foreach (var kRow in dtFach)
      {
        Kurs k = new Kurs(kRow);
        Zugriff.Instance.KursRep.Add(k);
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
            if (ap != null)
            {
              hj1 += f.getHjLeistung(HjArt.Hj1).Punkte;
              hj2 += f.getHjLeistung(HjArt.Hj2).Punkte;
              abi += (int)ap;
              anz++;
            }
          }
          if (anz > 0)
          {
            erg += k.Data.Bezeichnung + "\t";
            erg += k.getLehrer.Kuerzel + "\t";
            erg += anz.ToString() + "\t";
            erg += String.Format("{0:0.00}", hj1 / (double)anz) + "\t";
            erg += String.Format("{0:0.00}", hj2 / (double)anz) + "\t";
            erg += String.Format("{0:0.00}", abi / (double)anz) + "\r\n";
          }
        }
      }
      Clipboard.SetText(erg);
      MessageBox.Show("Auswertung liegt in der Zwischenablage.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Übersicht über alle 12. und 13. Klassen: Anzahl Schüler, zur Prüfung Zugelassene
    /// (= alle außer denen mit Vorkommnis "nicht zur Prüfung zugelassen") und Bestandene
    /// (Fachhochschulreife, fachgebundene oder allgemeine Hochschulreife), gruppiert nach
    /// Zweig, Jahrgangsstufe und Schulart. Tab-separiert für Excel in der Zwischenablage.
    /// </summary>
    public static void StatistikBestanden()
    {
      var gruppen = new SortedDictionary<string, BestandenGruppe>();
      foreach (Klasse k in Zugriff.Instance.KlassenRep.getList())
      {
        if (k.Jahrgangsstufe < Jahrgangsstufe.Zwoelf)
          continue;

        foreach (Schueler s in k.Schueler)
        {
          // Schulart und Zweig kommen bewusst vom Schüler (nicht von der Klasse), da Mischklassen
          // (z.B. FB- oder Zweig-Mischklassen) pro Schüler unterschiedliche Werte haben können.
          string schulart = SchulartText(s);
          string zweig = Faecherkanon.GetZweigString(s.Zweig);
          string key = schulart + "\t" + (int)k.Jahrgangsstufe + "\t" + zweig; // bestimmt auch die Sortierung
          BestandenGruppe g;
          if (!gruppen.TryGetValue(key, out g))
          {
            g = new BestandenGruppe { Zweig = zweig, Jahrgangsstufe = (int)k.Jahrgangsstufe, Schulart = schulart };
            gruppen.Add(key, g);
          }

          g.Anzahl++;
          if (!s.hatVorkommnis(Vorkommnisart.NichtZurPruefungZugelassen))
            g.Zugelassen++;
          bool allgHSR = s.hatVorkommnis(Vorkommnisart.allgemeineHochschulreife);
          if (allgHSR)
            g.AllgHSR++;
          if (allgHSR || s.hatVorkommnis(Vorkommnisart.Fachabiturzeugnis) || s.hatVorkommnis(Vorkommnisart.fachgebundeneHochschulreife))
            g.Bestanden++;
        }
      }

      if (gruppen.Count == 0)
      {
        MessageBox.Show("Es sind keine 12. oder 13. Klassen geladen.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      string erg = "Schulart\tJahrgangsstufe\tZweig\tSchüler\tzugelassen\tbestanden\tdarunter allg. HSR\r\n";
      foreach (var g in gruppen.Values)
      {
        erg += g.Schulart + "\t";
        erg += g.Jahrgangsstufe + "\t";
        erg += g.Zweig + "\t";
        erg += g.Anzahl + "\t";
        erg += g.Zugelassen + "\t";
        erg += g.Bestanden + "\t";
        erg += g.AllgHSR + "\r\n";
      }
      Clipboard.SetText(erg);
      MessageBox.Show("Auswertung liegt in der Zwischenablage.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Schueler.Data.Schulart ist (anders als Klasse.Data.Schulart) ein rohes DB-Kürzel ("F"/"B").
    private static string SchulartText(Schueler s)
    {
      if (s.Data.IsSchulartNull()) return "ohne";
      switch (s.Data.Schulart)
      {
        case "F": return Schulart.FOS.ToString();
        case "B": return Schulart.BOS.ToString();
        default: return s.Data.Schulart;
      }
    }

    private class BestandenGruppe
    {
      public string Zweig;
      public int Jahrgangsstufe;
      public string Schulart;
      public int Anzahl;
      public int Zugelassen;
      public int Bestanden;
      public int AllgHSR;
    }

    public static void AbiBesten()
    {            
      List<Schueler> best = new List<Schueler>();
      List<Klasse> klassen = Zugriff.Instance.KlassenRep.getList();
      foreach (var k in klassen)
      {
        if (k.Jahrgangsstufe < Jahrgangsstufe.Zwoelf)
          continue;
        Schueler bester=null;
        foreach (Schueler s in k.Schueler)
        {
          if (bester == null || bester.punktesumme.Summe(PunktesummeArt.Gesamt) < s.punktesumme.Summe(PunktesummeArt.Gesamt))
            bester = s;
          if (!s.Data.IsDNoteNull() && (double)s.Data.DNote < 2.0)
            best.Add(s);
        }
        if (!bester.Data.IsDNoteNull() && (double)bester.Data.DNote > 1.9)
          best.Add(bester); // zumindest den Klassenbester nehmen wir noch auf.
      }
      if (best.Count > 0)
        new ReportSchuelerdruck(best, Bericht.EinserAbi).Show();
      else
        MessageBox.Show("Die benötigten Daten werden erst zur 3. PA berechnet.");
    }
  }
}
