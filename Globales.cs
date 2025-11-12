using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
      try
      {        
        dic.Clear();
        // Erster Datenbankzugriff beim Programmstart: 1. Globale Daten, dann Logindaten
        var ta = new GlobaleStringsTableAdapter();
        var dt = ta.GetData();

        foreach (var d in dt)
        {
          dic.Add(d.ID, d.Wert);
        }
      }
      catch (Exception e)
      {
        Cursor.Current = Cursors.Default;
        MessageBox.Show("Keine Verbindung zur Datenbank!\nBitte wenden Sie sich an einen Administrator.\n\n" + e.Message, "diNo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Application.ExitThread();
        Environment.Exit(1);
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
    MailPasswort,
    VerzeichnisExceldateien,
    SchulnummerFOS,
    SchulnummerBOS,
    CopyUserLoginname,
    CopyUserPwd,
    CopyUserDomain,
    LNWAblagePfad,
    MailAdresseTest
  }

}
