using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace diNo.Zeugnisprogramm
{
  public class ExportSchueler
  {
    /// <summary>
    /// Schreibt eine Schülerdatei für das neue Zeugnisprogramm
    /// </summary>
    /// <param name="fileName">Dateiname.</param>
    public static void Write(string fileName)
    {
      using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding("iso-8859-1")))
      {
        foreach (var dbSchueler in new SchuelerTableAdapter().GetData())
        {
          Schueler schueler = new Schueler(dbSchueler);
          Jahrgangsstufe jgstufe = schueler.getKlasse.Jahrgangsstufe;
          if (jgstufe == Jahrgangsstufe.Elf || jgstufe == Jahrgangsstufe.None || jgstufe == Jahrgangsstufe.Vorklasse || jgstufe == Jahrgangsstufe.Vorkurs)
          {
            Schuelerzeile zeile = new Schuelerzeile();
            zeile.SchuelerId = schueler.Id;
            zeile.FosOderBos = schueler.Data.Schulart == "F" ? "FOS" : "BOS";
            zeile.Klasse = schueler.getKlasse.Bezeichnung;
            zeile.Familienname = schueler.Name;
            zeile.Rufname = schueler.benutzterVorname;
            zeile.Vorname = schueler.Vorname;
            zeile.Geburtsdatum = schueler.Data.IsGeburtsdatumNull() ? "" : schueler.Data.Geburtsdatum.ToString("dd.MM.yyyy");
            zeile.Geburtsort = schueler.Data.Geburtsort;
            zeile.Geburtsland = ""; // wir haben kein Geburtsland in diNo
            zeile.Bekenntnis = schueler.Data.Bekenntnis;
            if (zeile.Bekenntnis == "")
            {
              zeile.Bekenntnis = "BL"; // für Bekenntnislos
            }
            zeile.ReliOderEthik = schueler.ReliOderEthik;
            zeile.Geschlecht = schueler.Data.Geschlecht;
            switch (jgstufe)
            {
              case Jahrgangsstufe.Vorklasse: zeile.Jahrgangsstufe = schueler.Data.Schulart == "F" ? "10" : "11"; break;
              case Jahrgangsstufe.Vorkurs: zeile.Jahrgangsstufe = "11"; break; // BOS Vorkurs
              case Jahrgangsstufe.Elf: zeile.Jahrgangsstufe = "11"; break;
              case Jahrgangsstufe.None: zeile.Jahrgangsstufe = "10"; break; // IV
            }
            zeile.Ausbildungsrichtung = schueler.Data.Ausbildungsrichtung;
            if (zeile.Ausbildungsrichtung == "V")
            {
              zeile.Ausbildungsrichtung = "T"; // Hack für IV
            }
            zeile.Eintrittsdatum = schueler.Data.IsEintrittAmNull() ? "" : schueler.Data.EintrittAm.ToString("dd.MM.yyyy");
            zeile.ProbezeitBis = schueler.Data.IsProbezeitBisNull() ? "" : schueler.Data.ProbezeitBis.ToString("dd.MM.yyyy");

            writer.WriteLine(zeile.ToString());
          }
        }
      }
    }

    /// <summary>
    /// Hält die Daten zur leichteren Verarbeitung. Kann später auch mal diverse Umwandlungen von Abkürzungen o.Ä. vornehmen.
    /// </summary>
    private class Schuelerzeile
    {
      private static string Separator = ";";

      public int SchuelerId { get; set; }
      public string FosOderBos { get; set; }
      public string Klasse { get; set; }
      public string Familienname { get; set; }
      public string Rufname { get; set; }
      public string Vorname { get; set; }
      public string Geburtsdatum { get; set; }
      public string Geburtsort { get; set; }
      public string Geburtsland { get; set; }
      public string Bekenntnis { get; set; }
      public string ReliOderEthik { get; set; }
      public string Geschlecht { get; set; }
      public string Jahrgangsstufe { get; set; } //10 = FOS Vk, 11 = BOS Vk oder FOS11 usw.
      public string Ausbildungsrichtung { get; set; } // ABU, S, T, W
      public string Eintrittsdatum { get; set; }
      public string ProbezeitBis { get; set; }

      public override string ToString()
      {
        return
          SchuelerId + Separator +
          FosOderBos + Separator +
          Klasse + Separator +
          Familienname + Separator +
          Rufname + Separator +
          Vorname + Separator +
          Geburtsdatum + Separator +
          Geburtsort + Separator +
          Geburtsland + Separator +
          Bekenntnis + Separator +
          ReliOderEthik + Separator +
          Geschlecht + Separator +
          Jahrgangsstufe + Separator +
          Ausbildungsrichtung + Separator +
          Eintrittsdatum + Separator +
          ProbezeitBis;
      }
    }
  }

  public class ExportLehrer
  {
    /// <summary>
    /// Schreibt eine Schülerdatei für das neue Zeugnisprogramm
    /// </summary>
    /// <param name="fileName">Dateiname.</param>
    public static void Write(string fileName)
    {
      string Separator = ","; // fragt bitte nicht warum in der Lehrerdatei ein Komma stehen soll - aber so sagt es die Doku
      using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding("iso-8859-1")))
      {
        foreach (var dbLehrer in new LehrerTableAdapter().GetData())
        {
          string geschlecht = dbLehrer.Dienstbezeichnung.EndsWith("in") ? "w" : "m";
          writer.WriteLine(dbLehrer.Kuerzel + Separator + dbLehrer.Nachname + Separator + dbLehrer.Vorname + Separator + dbLehrer.Dienstbezeichnung + Separator + Separator + geschlecht + Separator + Separator);
        }
      }
    }
  }
  }
