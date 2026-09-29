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

    public BriefDaten(Schueler s, BriefTyp typ)
    {
      Lehrer lehrer;
      IstU18 = s.Alter() < 18 && (typ != BriefTyp.Standard) || typ == BriefTyp.Mitteilung; // Schüleradresse bei normalen Nachterminen
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
      try
      {
        string verz = Directory.GetCurrentDirectory() + "\\Logo\\Logo.png";
        Logo = ConvertImageToBase64(Image.FromFile(verz), ImageFormat.Png);
      }
      catch
      {
        Logo = "";
      }
    }

    private string ConvertImageToBase64(Image image, ImageFormat format)
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
    Mitteilung = 5
  }
}
