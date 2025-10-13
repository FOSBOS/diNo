using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diNo
{
  public class ImportCheck
  {
    public ImportCheck()
    {
      List<Schueler> alle = Zugriff.Instance.SchuelerRep.getList();
      NotenCheckResults err = new NotenCheckResults();
      foreach (var s in alle)
      {
        Jahrgangsstufe jg = s.getKlasse.Jahrgangsstufe;
        
        int notw; // Anzahl Kurse
        if (jg == Jahrgangsstufe.Zwoelf && s.Data.Schulart == "F") notw = 12; // FOS 12
        else if (jg == Jahrgangsstufe.Zwoelf) notw = 10;
        else if (jg == Jahrgangsstufe.Dreizehn) notw = 9;
        else if (jg == Jahrgangsstufe.Vorklasse) notw = 7; // 6 ohne Reli
        else if (jg == Jahrgangsstufe.IntVk) notw = 8;
        else notw = 7;
        if (s.Kurse.Count != notw) err.list.Add(new NotenCheckResult(s, null, s.Kurse.Count + " statt " + notw + " Kurse"));
        else
        {
          if (jg == Jahrgangsstufe.Zwoelf && s.Data.Schulart == "F")
          {            
            notw = (s.Data.Ausbildungsrichtung=="S" || s.Data.Ausbildungsrichtung == "W") ? 15 : 14;
          }
          if (s.getNoten.alleFaecher.Count != notw) err.list.Add(new NotenCheckResult(s, null, s.getNoten.alleFaecher.Count + " statt " + notw + " Fächer")); ;
        }
        Regex regex = new Regex(@"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$");
        //@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        Match match = regex.Match(s.Data.Email);
        if (!match.Success)
          err.list.Add(new NotenCheckResult(s, null,"Ungültige Mailadresse: "+ s.Data.Email));

        if (!(s.Data.IsNotfalltelefonnummerNull() || s.Data.Notfalltelefonnummer==""))
        {
          string elternmail = s.Data.Notfalltelefonnummer.Split(new string[] { ",", ";", " " }, StringSplitOptions.RemoveEmptyEntries).First();
          match = regex.Match(elternmail);
          if (!match.Success)
            err.list.Add(new NotenCheckResult(s, null, "Ungültige Mailadresse: " + elternmail));
        }
      }


      if (err.list.Count == 0) MessageBox.Show("Alles in Ordnung.", "diNo", MessageBoxButtons.OK);
      else new ReportNotencheck(err,false).Show();
    }
  }
}
