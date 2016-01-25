using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using System.Runtime.InteropServices;

namespace diNo.OmnisDB
{
  public class OmnisConnection
  {
    private Dictionary<string, string[]> faecherspiegelFOS;
    private Dictionary<string, string[]> faecherspiegelBOS;

    public OmnisConnection()
    {
      faecherspiegelFOS = new Dictionary<string, string[]>();
      faecherspiegelBOS = new Dictionary<string, string[]>();

      OdbcConnection conn = new OdbcConnection("DSN=sd");
      conn.Open();

      var command = conn.CreateCommand();
      command.CommandText = "SELECT FSP_NUM, FSP_SCHULART, FSP_Z_F01, FSP_Z_F02, FSP_Z_F03, FSP_Z_F04, FSP_Z_F05, FSP_Z_F06, FSP_Z_F07, FSP_Z_F08, FSP_Z_F09, FSP_Z_F10, FSP_Z_F11, FSP_Z_F12, FSP_Z_F13, FSP_Z_F14, FSP_Z_F15, FSP_Z_F16, FSP_Z_F17, FSP_Z_F18, FSP_Z_F19, FSP_Z_F20, FSP_Z_F21, FSP_Z_F22, FSP_Z_F23, FSP_Z_F24, FSP_Z_F25, FSP_Z_F26, FSP_Z_F27, FSP_Z_F28, FSP_Z_F29, FSP_Z_F30 FROM DZeugnisFspSt WHERE FSP_Schulart='FOS' OR FSP_Schulart = 'BOS'";
      var cmdResult = command.ExecuteReader();
      while (cmdResult.Read())
      {
        string code = cmdResult.GetString(0);
        string schulart = cmdResult.GetString(1);
        if ("FOS".Equals(schulart, StringComparison.OrdinalIgnoreCase))
        {
          if (!faecherspiegelFOS.ContainsKey(code))
          {
            faecherspiegelFOS.Add(code, new string[30]);
          }

          for (int i = 0; i < 30; i++)
          {
            faecherspiegelFOS[code][i] = cmdResult.GetString(i + 2);
          }
        }
        else if ("BOS".Equals(schulart, StringComparison.OrdinalIgnoreCase))
        {
          if (!faecherspiegelBOS.ContainsKey(code))
          {
            faecherspiegelBOS.Add(code, new string[30]);
          }

          for (int i = 0; i < 30; i++)
          {
            faecherspiegelBOS[code][i] = cmdResult.GetString(i + 1);
          }
        }
      }
    }

    public string SucheFach(string faecherspiegelCode, int index, Schulart schulart)
    {
      var faecherspiegel = schulart == Schulart.BOS ? this.faecherspiegelBOS : this.faecherspiegelFOS;

      if (!faecherspiegel.ContainsKey(faecherspiegelCode))
      {
        throw new InvalidOperationException("Unbekannter Fächerspiegel "+faecherspiegelCode);
      }

      string fach = faecherspiegel[faecherspiegelCode][index];
      return PasseFachKuerzelAn(fach);
    }

    private string PasseFachKuerzelAn(string fach)
    {
      switch (fach)
      {
        case "BWL": return "BwR";
        case "PPs": return "PP";
        case "Sp": return "Smw";
        case "WI": return "WIn";
        case "FF": return "F-Wi";
        case "RL": return "Rl";
        case "WL": return "Wl";
        case "TI": return "TeIn";
        default: return fach;
      }
    }
  }
}
