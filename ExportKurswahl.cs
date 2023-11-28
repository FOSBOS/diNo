using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace diNo
{
  public static class Tools
  {
    public static string ErsetzeUmlaute(string t)
    {
      t = t.Replace("ä", "ae");
      t = t.Replace("ö", "oe");
      t = t.Replace("ü", "ue");
      t = t.Replace("Ä", "Ae");
      t = t.Replace("Ö", "Oe");
      t = t.Replace("Ü", "Ue");
      t = t.Replace("ß", "ss");
      t = t.Replace(" ", "-");
      t = t.Replace("'", "");
      t = t.Replace("à", "a");
      t = t.Replace("é", "e");
      t = t.Replace("è", "e");
      t = t.Replace("ó", "o");
      t = t.Replace("ñ", "n");
      return t;
    }

  }

  public class ExportKurswahl : IDisposable
  {
    private StreamWriter writer;
    private List<Schueler> list;
    string sep = ",";

    private string qt(string t)
    {
      return "'" + t + "'";
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
        /*
        string pref = "FB" + (Zugriff.Instance.Schuljahr - 2000 - Math.Max((sbyte)s.getKlasse.Jahrgangsstufe - 11 ,0)) + "_";
        string name = s.Name.Substring(1 + s.Name.LastIndexOfAny(new[] { ' ', '-' })) ; 
        string username = pref + Tools.ErsetzeUmlaute(s.Data.Rufname).Substring(0, 2) + Tools.ErsetzeUmlaute(name);
        if (username.Length > 20)
          username = username.Substring(0, 20); // maximale Länge
        */
        string username = s.getLoginname();
        string pwd = "FB-" + s.Data.Geburtsdatum.ToString("yyyyMMdd");

        int jgstufe = (int)s.getKlasse.Jahrgangsstufe;
        if (jgstufe < 11) jgstufe = 12; // BOS-Vorklasse
        else if (Zugriff.Instance.aktHalbjahr == Halbjahr.Zweites && jgstufe < 13) jgstufe++; // Wahl idR für das nächste Schuljahr
        writer.WriteLine(s.Id + sep + qt(username) + sep + qt(pwd) + sep + qt(s.Name.Replace("'", " ")) + sep + qt(s.Data.Rufname) + sep
          + qt(s.getKlasse.Bezeichnung) + sep + jgstufe + sep + qt(s.Data.Ausbildungsrichtung) + sep + qt(s.Data.Schulart) + sep + qt(s.Data.SchulischeVorbildung)
          + sep + qt(getHash(s)));
      }
      writer.Close();
    }


    public void ExportAlteWPF(string datei)
    {
      FileStream stream = new FileStream(datei, FileMode.Create, FileAccess.Write);
      writer = new StreamWriter(stream);
      foreach (Schueler s in list)
      {
        if (s.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Zwoelf)
        {
          foreach (Kurs k in s.Kurse)
          {
            if (k.getFach.WPFid != null)
              writer.WriteLine(s.Id + sep + k.getFach.WPFid + sep + Zugriff.Instance.Schuljahr);
          }
        }
      }
      writer.Close();
    }

    public void ExportSchuelerID(string datei)
    {
      FileStream stream = new FileStream(datei, FileMode.Create, FileAccess.Write);
      writer = new StreamWriter(stream);
      sep = ";";
      writer.WriteLine("ID;Nachname;Vorname;Klasse;JgStufe;Zweig;Schulart;SchulischeVorbildung");
      foreach (Schueler s in list)
      {
        int jgstufe = (int)s.getKlasse.Jahrgangsstufe;
        if (jgstufe < 11) jgstufe = 12; // BOS-Vorklasse
        else if (Zugriff.Instance.aktHalbjahr == Halbjahr.Zweites && jgstufe < 13) jgstufe++; // Wahl idR für das nächste Schuljahr

        writer.WriteLine(s.Id + sep + s.Name + sep + s.Data.Rufname + sep + s.getKlasse.Bezeichnung + sep + jgstufe + sep +
          s.Data.Ausbildungsrichtung + sep + s.Data.Schulart + sep +s.Data.SchulischeVorbildung);

      }
      writer.Close();
    }

    public string getHash(Schueler s)
    {
      return sha256(s.Id + s.Data.MailSchule + s.Data.KlasseId + s.Data.Rufname);
    }

    private string sha256(string randomString)
    {
      var crypt = new System.Security.Cryptography.SHA256Managed();
      var hash = new System.Text.StringBuilder();
      byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
      foreach (byte theByte in crypto)
      {
        hash.Append(theByte.ToString("x2"));
      }
      return hash.ToString();
    }

    #region IDisposable Support
    private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          writer.Dispose();
          writer = null;
        }
        disposedValue = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }
    #endregion
  }
}
