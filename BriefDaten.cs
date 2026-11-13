using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace diNo
{
  // Datencontainer für Briefe/Mitteilungen, ursprünglich in Brief.cs (WinForms) zusammen mit der
  // inzwischen durch BriefWindow/BriefViewModel ersetzten Form definiert. Wird weiterhin von
  // ReportController.cs und BriefViewModel.cs für den Brief-Report gebraucht.
  public class BriefDaten
  {
    public int Id { get; private set; }
    public string Absender { get; private set; }
    public string Absenderzeile { get; private set; }
    public string Telefon { get; private set; }
    public string Adressfeld { get; set; }
    public string Name { get; set; }
    public string VornameName { get; set; }
    public string Klasse { get; set; }
    public string Betreff { get; set; }
    public string Inhalt { get; set; }
    public string Inhalt2 { get; set; }
    public string Unterschrift { get; set; }
    public string Unterschrift2 { get; set; }
    public string UnterschriftsText { get; set; }
    public bool IstU18 { get; set; }
    public string OrtDatum { get; set; }
    public string Logo { get; private set; }
    public bool MitPreis { get; set; }
    public string Gesamtbetrag { get; set; } = "";

    public BriefDaten(Schueler s, BriefTyp typ)
    {
      Lehrer lehrer;
      bool IstMahnungOderRechnung = typ == BriefTyp.Mahnung1 || typ == BriefTyp.Mahnung2 || typ == BriefTyp.Rechnung;
      // Mahnung/Rechnung geht immer an den Schüler selbst, nie an die Eltern.
      IstU18 = !IstMahnungOderRechnung && (s.Alter() < 18 && (typ != BriefTyp.Standard) || typ == BriefTyp.Mitteilung); // Schüleradresse bei normalen Nachterminen
      bool UnterschriftKL = typ == BriefTyp.Gefaehrdung || typ == BriefTyp.Attestpflicht; // hier nicht der angemeldete Benutzer

      Id = s.Id;
      Absenderzeile = Zugriff.Instance.getString(GlobaleStrings.SchulAbsenderzeile);
      Absender = Zugriff.Instance.getString(GlobaleStrings.SchulName);
      if (Zugriff.Instance.getString(GlobaleStrings.SchulNameZusatz) != "") Absender += "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulNameZusatz);
      Absender += "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulStrasse) + "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulPLZ) + " " + Zugriff.Instance.getString(GlobaleStrings.SchulOrt);
      Telefon = "Telefon: " + Zugriff.Instance.getString(GlobaleStrings.SchulTel) + "\nTelefax: " + Zugriff.Instance.getString(GlobaleStrings.SchulFax);
      Telefon += "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulWeb) + "\n" + Zugriff.Instance.getString(GlobaleStrings.SchulMail);

      Adressfeld = s.ErzeugeAdresse(IstU18);
      Name = s.Name;
      VornameName = s.VornameName;
      Klasse = s.getKlasse.Bezeichnung;
      DateTime dat = (typ == BriefTyp.Gefaehrdung ? Zugriff.Instance.Zeugnisdatum : DateTime.Today);
      OrtDatum = Zugriff.Instance.getString(GlobaleStrings.SchulOrt) + ", den " + dat.ToString("dd.MM.yyyy");

      if (UnterschriftKL)
      {
        lehrer = s.getKlasse.Klassenleiter;
      }
      else
        lehrer = Zugriff.Instance.lehrer;

      Unterschrift = lehrer.NameDienstbezeichnung;
      if (UnterschriftKL)
        Unterschrift += "\n" + lehrer.KLString;

      if (IstU18)
        UnterschriftsText = "Unterschrift eines Erziehungsberechtigten";
      else
        UnterschriftsText = "Unterschrift";

      Inhalt = s.ErzeugeAnrede(IstU18);
      Inhalt2 = "";
      Logo = LoadLogoBase64();
    }

    // Lädt das Schullogo frisch von der Festplatte (als Base64-PNG), damit es pro Schule
    // austauschbar bleibt. Wird auch von ReportController für rptMahnung verwendet, da dort
    // das Logo per Report-Parameter statt per Datensatz-Feld gebunden werden muss
    // (Fields!-Bindung im Page Header funktioniert bei mehrzeiligen Tablix-Reports nicht zuverlässig).
    public static string LoadLogoBase64()
    {
      try
      {
        string verz = Directory.GetCurrentDirectory() + "\\Logo\\Logo.png";
        return ConvertImageToBase64(Image.FromFile(verz), ImageFormat.Png);
      }
      catch
      {
        return "";
      }
    }

    // Baut den Mahnung/Rechnung-Brief für die aktuelle Mahnstufe des Schülers.
    // Liefert null bei Mahnstufe.Ungemahnt (nichts zu drucken/versenden) - Aufrufer filtert das.
    public static BriefDaten CreateMahnungRechnung(Schueler s)
    {
      BriefTyp typ;
      switch (s.Mahnstufe)
      {
        case Mahnstufe.ErsteMahnung: typ = BriefTyp.Mahnung1; break;
        case Mahnstufe.ZweiteMahnung: typ = BriefTyp.Mahnung2; break;
        case Mahnstufe.Rechnung: typ = BriefTyp.Rechnung; break;
        default: return null;
      }

      var b = new BriefDaten(s, typ);
      int jahr = Zugriff.Instance.Schuljahr;

      if (typ == BriefTyp.Rechnung)
      {
        b.Betreff = "Rechnung";
        b.MitPreis = true;
        b.Gesamtbetrag = string.Format("{0:0.00} €", BuchDruck.Gesamtbetrag(s));
        b.Inhalt += "Sie haben im Schuljahr " + jahr + "/" + (jahr + 1) + " Schulbücher ausgehändigt bekommen. " +
          "Diese sind bis heute noch nicht zurückgegeben bzw. bei Verlust/Beschädigung ersetzt worden.<br>" +
          "Deshalb sehen wir uns gezwungen, Ihnen diese Bücher im Auftrag unseres Sachaufwandsträgers, des Zweckverbandes Berufliches Schulzentrum, " +
          "Wiesstraße 30, 87435 Kempten, wie folgt in Rechnung zu stellen.";
        b.Inhalt2 = "Bitte überweisen Sie o.g. Betrag auf das Konto DE81 7335 0000 0310 0001 53<br>" +
          "bei der Sparkasse Allgäu, BIC BYLADEM1ALG<br>" +
          "Verwendungszweck: Schulbücher, HHSt. 0.2600.1549";
      }
      else
      {
        bool istZweiteMahnung = typ == BriefTyp.Mahnung2;
        string frist = DateTime.Today.AddDays(7).ToString("dd.MM.yyyy");

        b.Betreff = "Rückforderung lehrmittelfreier Bücher (" + (istZweiteMahnung ? "2." : "1.") + " Mahnung)";
        b.Inhalt += "Sie haben im Schuljahr " + jahr + "/" + ((jahr + 1) % 100).ToString("00") + " lehrmittelfreie Bücher erhalten. " +
          "Die Rückgabe erfolgte " + (istZweiteMahnung ? "trotz erster Mahnung bislang" : "bislang") +
          " unvollständig bzw. zu ersetzende Bücher wurden noch nicht ersetzt. Folgende Bücher wurden nicht abgegeben:";

        b.Inhalt2 = "Sie werden gebeten, die ausstehenden Bücher bis spätestens " + frist + " in der Beruflichen Oberschule abzugeben. " +
          "Sollten Sie nicht mehr im Besitz der Schulbücher oder diese beschädigt sein, so ist Ersatz bei einer Buchhandlung zu beschaffen und bei uns abzugeben.";

        if (istZweiteMahnung)
        {
          b.MitPreis = true;
          b.Gesamtbetrag = string.Format("{0:0.00} €", BuchDruck.Gesamtbetrag(s));
          b.Inhalt2 += "<br><br>Werden die Bücher bis zum o. a. Termin nicht bei uns abgegeben, sehen wir uns leider gezwungen, diese über unseren Sachaufwandsträger, " +
            "dem Zweckverband Berufliches Schulzentrum Kempten (Allgäu), in Rechnung stellen zu lassen. In diesem Fall müssen Sie mit oben aufgeführten Kosten rechnen. " +
            "Wir bitten zu beachten, dass ab dem Tag der Rechnungsstellung keine Rückgabe der Bücher mehr erfolgen kann.";
        }
      }

      return b;
    }

    private static string ConvertImageToBase64(Image image, ImageFormat format)
    {
      byte[] imageArray;

      using (MemoryStream imageStream = new MemoryStream())
      {
        image.Save(imageStream, format);
        imageArray = new byte[imageStream.Length];
        imageStream.Seek(0, SeekOrigin.Begin);
        imageStream.Read(imageArray, 0, (int)imageStream.Length);
      }

      return Convert.ToBase64String(imageArray);
    }
  }

  public enum BriefTyp
  {
    Standard = 0,
    Verweis = 1,
    Ersatzpruefung = 2,
    Gefaehrdung = 3,
    Attestpflicht = 4,
    Mitteilung = 5,
    Mahnung1 = 6,
    Mahnung2 = 7,
    Rechnung = 8
  }
}
