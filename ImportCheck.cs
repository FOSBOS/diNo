using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        else if (jg == Jahrgangsstufe.Vorklasse) notw = 6;
        else if (jg == Jahrgangsstufe.IntVk) notw = 5;
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
      }

      if (err.list.Count == 0) MessageBox.Show("Alles in Ordnung.", "diNo", MessageBoxButtons.OK);
      else new ReportNotencheck(err,false).Show();
    }
  }
}
