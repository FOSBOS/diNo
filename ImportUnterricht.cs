using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using System.IO;

namespace diNo
{
  public class ImportUnterricht
  {
    string FileName;
    private char SepChar = ';';

    public ImportUnterricht(string Datei)
    {
      FileName = Datei;
    }


    public void Import()
    {
      var kursTa = new KursTableAdapter();
      var klasseKursTa = new KlasseKursTableAdapter();
      int neueUNr = 5001; // Vergabe von automatischen Kursnummern, wenn anderes Fach zur selben Zeit (z.B. K/Ev/Eth)
      int vorigeUNr = 0;
      List<Kurs> GleicheKursnr = new List<Kurs>();

      using (FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(stream))
      using (StreamWriter writer = new StreamWriter(new FileStream(FileName + "_err.txt", FileMode.Create, FileAccess.ReadWrite)))
      {

        while (!reader.EndOfStream)
        {
          string orignal = reader.ReadLine();
          string[] line = orignal.Split(SepChar);
          bool weiter = false;

          int UNr = int.Parse(line[0]);
          string kl = line[4].Trim(new char[] { '"', ' ' });
          string le = line[5].Trim(new char[] { '"', ' ' }); // Kürzel
          string fachOrg = line[6].Trim(new char[] { '"', ' ' });
          string f = line[6].Trim(new char[] { '"', ' ' , '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' }); // Kursnummern weg
          if (string.IsNullOrEmpty(kl) || string.IsNullOrEmpty(le) || string.IsNullOrEmpty(f)) continue;

          foreach (var substr in new string[] { "FP", "FBB", "-Fö", "GK_", "GK-", "_Ü", "-Ü", "BK-", "CHÜ", "PRAK" , "KL", "SF", "AWU", "PROJ", "SOZP" })
          { // diese Fächer werden ohne Noten unterrichtet
            if (f.ToUpper().Contains(substr))
            {
              writer.WriteLine("Ignoriere " + orignal);
              weiter = true;
              break;
            }
          }
          if (weiter) continue;

          Fach fach = Zugriff.Instance.FachRep.Find(f);
          if (fach == null)
          {
            writer.WriteLine("Fach " + f + " wird angelegt.");
            new FachTableAdapter().Insert(f, f, 999, 0, null, false, 0, null, null,null);
            Zugriff.Instance.FachRep.Clear();
            Zugriff.Instance.LoadFaecher();
            fach = Zugriff.Instance.FachRep.Find(f);
          }
          if (fach.Typ == FachTyp.OhneNoten) // nur für in der DB bereits registrierte Fächer 
          {
            writer.WriteLine("Ignoriere Fach ohne Noten " + f);
            continue;
          }

          string zweig = null;
          if (kl.Contains("_")) // Mischklasse: Zweig extrahieren
          {
            string[] teilstrings = kl.Split('_'); 
            kl = teilstrings[0];
            if (fach.Typ == FachTyp.Profilfach ||  // nur Profilfächer und Mathe (T/NT) außer Vorklasse werden i.d.R. getrennt
              fach.Kuerzel=="M" && kl.Contains("T") && !kl.Contains("V"))
              zweig = teilstrings[1].Trim();
            if (zweig == "") zweig = null;
          }

          Klasse klasse = Zugriff.Instance.KlassenRep.Find(kl);
          if (klasse==null)
          {
            writer.WriteLine("Klasse " + kl + " nicht vorhanden.");
            continue;
          }
          Lehrer lehrer = Zugriff.Instance.LehrerRep.Find(le);
          if (lehrer == null)
          {
            writer.WriteLine("Lehrer " + le + " nicht vorhanden.");
            continue;
          }

          // Tandems/anderer Raum sind kein neuer Unterricht (geht teils quer über UNr): gleiche Klasse, Fach und Zweig
          if (fach.Typ != FachTyp.WPF)
          {
            diNoDataSet.KlasseKursDataTable dt;
            if (zweig==null)
              dt = klasseKursTa.GetDataByKlasseAndFach(klasse.GetId(), fach.Id);
            else
              dt = klasseKursTa.GetDataByKlasseFachAndZweig(klasse.GetId(), fach.Id, zweig);
            if (dt.Count > 0)
              continue;
          }

          // Existiert diese Kursnummer schon? Suche in GleicheKursnr, ob Fach und Lehrer-Kombi nur in einer anderen Klasse auftreten
          if (vorigeUNr != UNr)
          {
            GleicheKursnr.Clear();
            vorigeUNr = UNr;            
          }          
          else
          {
            foreach (var k in GleicheKursnr)
            {
              // Kurs auch mit anderer Klasse (d.h. gleicher Lehrer, gleiches Fach)? --> ggf. Klasse in Kurs aufnehmen.
              if (k.getLehrer.Id == lehrer.Id && k.getFach.Id == fach.Id)
              {
                if (!k.Klassen.Contains(klasse)) // klappt das?
                {
                  k.Klassen.Add(klasse);
                  klasseKursTa.Insert(klasse.Data.Id, k.Id);
                  if (fach.Typ != FachTyp.WPF)
                  {
                    k.Data.Bezeichnung += ", " + klasse.Bezeichnung;
                    kursTa.Update(k.Data);
                  }
                } // sonst vielleicht nur anderer Raum
                weiter = true;
                break;
              }

              // gleiches Fach, gleiche Klasse ==> Lehrer-Tandem (nicht aufnehmen)
              else if (fach.Typ==FachTyp.WPF || k.getFach.Id == fach.Id && k.Klassen.Count == 1 && k.Klassen[0].GetId() == klasse.GetId())
              {
                weiter = true;
                break;
              } 
            }
            
            if (weiter) continue;

            // es liegt eigentlich ein neuer Kurs vor:
            UNr = neueUNr;
            neueUNr++;
          }

          // neuen Kurs anlegen
          string KursBezeichung = fach.Typ == FachTyp.WPF ? fach.Bezeichnung + " " + fachOrg : fach.Bezeichnung.Trim() + " " + klasse.Bezeichnung + (zweig!=null ? "_"+zweig :"");
          string geschlecht = null;
          if (fach.Kuerzel == "Sw") geschlecht = "W";
          if (fach.Kuerzel == "Sm") geschlecht = "M";
          kursTa.Insert(UNr, KursBezeichung, lehrer.Id, fach.Id, zweig, geschlecht, (fach.Typ == FachTyp.WPF ? fachOrg : fach.Kuerzel) + " (" + lehrer.Kuerzel + ")");
          klasseKursTa.Insert(klasse.Data.Id, UNr);

          Kurs kurs = Zugriff.Instance.KursRep.Find(UNr); // das Repository aktualisieren
          GleicheKursnr.Add(kurs);                            
        }
      }

      SchuelerZuweisen();
    }

    // Meldet alle Schüler in den Kursen an (außer WPF)
    public void SchuelerZuweisen()
    {
      
      // nochmal alle Klasse mit ihren Schülern durchgehen: Die Kurse werden nun zugewiesen.
      foreach (Klasse k in Zugriff.Instance.Klassen)
      {
        k.RefreshKurse();
        foreach (Schueler s in k.Schueler)
        {
          var kurse = s.AlleNotwendigenKurse();
          foreach (var kurs in kurse)
          {
            s.MeldeAn(kurs);
          }
        }
      }
    }

  }
}
