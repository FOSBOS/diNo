using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public class GlobaleStringsContainer
  {
    private Dictionary<int, string> dic;

    public GlobaleStringsContainer()
    {
      Refresh();
    }

    public void Refresh()
    {
      dic = new Dictionary<int, string>();
      var ta = new GlobaleStringsTableAdapter();
      var dt = ta.GetData();
      foreach (var d in dt)
      {
        dic.Add(d.ID, d.Wert);
      }
    }

    public string getString(GlobaleStrings g)
    {
      string s;
      dic.TryGetValue((int)g, out s);
      return s;
    }   
  }
  
  public enum GlobaleStrings
  {
    Backuppfad = 1,
    Schulleiter,
    SchulleiterText,
    Stellvertreter,
    StellvertreterText,
    FOSName,
    BOSName,
    SchulName,
    SchulNameZusatz,
    SchulStrasse,
    SchulPLZ,
    SchulOrt,
    SchulAbsenderzeile,
    SchulTel,
    SchulFax,
    SchulWeb,
    SchulMail,
    SMTP,
    Port,
    SendExcelViaMail,
    MailPasswort
  }
  
}
