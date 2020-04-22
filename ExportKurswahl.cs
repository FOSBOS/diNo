﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace diNo
{
  public class ExportKurswahl
  {
    private StreamWriter writer;
    private List<Schueler> list;
    string sep = ",";

    private string qt(string t)
    {
      return "'" + t + "'";
    }

    private string repl(string t)
    {
      t = t.Replace("ä", "ae");
      t = t.Replace("ö", "oe");
      t = t.Replace("ü", "ue");
      t = t.Replace("Ä", "Ae");
      t = t.Replace("Ö", "Oe");
      t = t.Replace("Ü", "Ue");
      t = t.Replace("ß", "ss");
      t = t.Replace(" ", "");
      t = t.Replace("'", "");
      t = t.Replace("à", "a");
      t = t.Replace("é", "e");
      t = t.Replace("ó", "o");
      return t;
    }

    // erstellt eine csv-Datei, die direkt als Sql-String eingelesen werden kann vom Kursmodul->ImportAct.php
    public ExportKurswahl(List<Schueler> aList)
    {
      list = aList;
      
    }

    public void ExportSchueler(string datei)
    {
      FileStream stream = new FileStream(datei, FileMode.Create, FileAccess.Write);
      writer = new StreamWriter(stream);
      foreach (Schueler s in list)
      {
        string klasse = s.getKlasse.Bezeichnung;
        string username = klasse + "_" + repl(s.Data.Vorname).Substring(0,2) + repl(s.Name);
        if (username.Length > 20)
          username = username.Substring(0, 20); // maximale Länge
        

        string pwd = "FB-" + s.Data.Geburtsdatum.ToString("yyyyMMdd");
        int jgstufe = (int) s.getKlasse.Jahrgangsstufe;
        if (Zugriff.Instance.aktHalbjahr == Halbjahr.Zweites && jgstufe<13) jgstufe++; // Wahl idR für das nächste Schuljahr
        writer.WriteLine(s.Id + sep + qt(username) + sep + qt(pwd) + sep + qt(s.Name.Replace("'", " ")) + sep + qt(s.Data.Rufname) + sep
          + qt(klasse) + sep + jgstufe + sep + qt(s.Data.Ausbildungsrichtung) + sep + qt(s.Data.Schulart));
      }
      writer.Close();
    }


    public void ExportAlteWPF(string datei)
    {
      FileStream stream = new FileStream(datei, FileMode.Create, FileAccess.Write);
      writer = new StreamWriter(stream);
      foreach (Schueler s in list)
      {
        if (s.getKlasse.Jahrgangsstufe==Jahrgangsstufe.Zwoelf)
        {
          foreach (Kurs k in s.Kurse)
          {
            if (k.getFach.WPFid!=null)
              writer.WriteLine(s.Id + sep + k.getFach.WPFid + sep + Zugriff.Instance.Schuljahr);
          }
        }
      }
      writer.Close();      
    }
  }
}
