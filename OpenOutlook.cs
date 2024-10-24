using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook; // Verweis hinzufügen -> COM -> Microsoft Outlook Object Library

namespace diNo
{
  public static class OpenOutlook
  {

    // es fehlt der Test, ob Outlook offen ist:
    // https://learn.microsoft.com/de-de/office/client-developer/outlook/pia/how-to-get-and-log-on-to-an-instance-of-outlook

    public static void NewMail(List<Kurs> kurse, string subject)
    {
      Outlook.Application outlookApp = new Outlook.Application();
      Outlook._MailItem oMailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
      Outlook.Inspector oInspector = oMailItem.GetInspector;
      
      // Recipient
      Outlook.Recipients oRecips = oMailItem.Recipients;
      foreach (Kurs k in kurse)
      {
        Outlook.Recipient oRecip = oRecips.Add(k.getLehrer.Data.EMail);
        oRecip.Resolve();
      }
      
      oMailItem.Subject = subject;
      
      oMailItem.Display(false);     
    }

    public static void NewMail(Klasse klasse)
    {
      List<Kurs> kurse = new List<Kurs>();
      foreach (Kurs k in klasse.Kurse)
      {
        FachTyp typ = k.getFach.Typ;
        string kue = k.getFach.Kuerzel;
        if (typ==FachTyp.Allgemein && kue!="K" && kue != "Ev" && kue != "Eth" || typ == FachTyp.Profilfach)
        {
          kurse.Add(k);
        }
      }
      NewMail(kurse, "Klasse " + klasse.Bezeichnung);
    }
  }
}
