using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Reflection;

namespace diNo
{
  public class DatenabgleichWinSV
  {
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private OdbcConnection conn;
    private OdbcCommand command; // wird immer wieder verwendet, um RAM zu sparen (Freigabe schwierig bei OMNISDb-Objekten)

    public DatenabgleichWinSV()
    {
      this.conn = new OdbcConnection("DSN=sd");
      conn.Open();
      command = conn.CreateCommand();
    }

    public void CheckSchuelerdaten()
    {
      foreach (int schuelerId in GetAlleGueltigenSchuelerIds())
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);

        foreach (KeyValuePair<string, string> kvp in GetStringPropertyMapping())
        {
          var aValue = ReadStringValue(schuelerId, kvp.Key);
          if (kvp.Value == "Ausbildungsrichtung" && aValue == "WVR")
          {
            aValue = "W";
          }

          var propValue = (string)GetPropValue(schueler.Data, kvp.Value);
          if (!string.Equals(propValue, aValue, System.StringComparison.OrdinalIgnoreCase))
          {
            log.Info(schueler.Name+", "+schueler.Vorname+"("+schueler.getKlasse.Bezeichnung+") - "+ kvp.Value.ToUpper() + ": " + propValue + " (alt) vs. " + aValue + " (neu)");
           
            if (kvp.Value == "Fremdsprache2")
            {
              schueler.Fremdsprache2 = aValue;
              // Klappt das? Der Schüler müsste sich eigentlich dann selbst an- bzw. abmelden...
            }

            if (kvp.Value == "ReligionOderEthik")
            {
              schueler.ReliOderEthik = aValue;
              // Klappt das? Der Schüler müsste sich eigentlich dann selbst an- bzw. abmelden...
            }

            if (kvp.Value == "Wahlpflichtfach")
            {
              schueler.Wahlpflichtfach = aValue;
              // Klappt das? Der Schüler müsste sich eigentlich dann selbst an- bzw. abmelden...
            }

            if (kvp.Value == "Ausbildungsrichtung")
            {
              log.Warn(schueler.Name + ", " + schueler.Vorname + "(" + schueler.getKlasse.Bezeichnung + ") - " + kvp.Value.ToUpper() + ": " + propValue + " (alt) vs. " + aValue + " (neu)");
              // Ausbildungsrichtungen gewechselt: Manuelle Änderung nötig!
            }

            SetPropValue(schueler.Data, kvp.Value, aValue);
          }
        }

        CheckKlasse(schuelerId, schueler);
        CheckLRS(schuelerId, schueler);

        schueler.Save();
      }
    }

    private IList<int> GetAlleGueltigenSchuelerIds()
    {
      var result = new List<int>();
      command.CommandText = "SELECT _SCHUELER_ID FROM DSchueler";
      using (var cmdResult = command.ExecuteReader())
      {
        var schuelerTable = new SchuelerTableAdapter();
        while (cmdResult.Read())
        {
          int schuelerId = cmdResult.GetInt32(0);

          if (schuelerTable.GetDataById(schuelerId).Count > 0) // Entferne die Schülerdatensätze aus dem Vorjahr oder sonstige Wartelisten- und Spaßdatensätze
          {
            result.Add(schuelerId);
          }
        }
      }

      return result;
    }

    private string ReadStringValue(int schuelerId, string colName)
    {
      command.CommandText = "SELECT " + colName + " FROM DSchueler WHERE _SCHUELER_ID=" + schuelerId;
      using (var einzelresult = command.ExecuteReader())
      {  //ExecuteScalar unterstützt der doofe Treiber nicht
        if (einzelresult.Read())
        {
          return einzelresult.GetString(0);
        }
      }

      return string.Empty;
    }

    private void CheckLRS(int schuelerId, Schueler schueler)
    {
      command.CommandText = "SELECT LRS_Schwaeche, LRS_Stoerung, LRS_BIS FROM DSchueler WHERE _SCHUELER_ID =" + schuelerId;
      using (var lrsresult = command.ExecuteReader())
      {
        if (lrsresult.Read())
        {          
          bool istLegasthenikerNeu = lrsresult.GetString(1) != "0" && lrsresult.GetString(1) != "";                    
          if (schueler.IsLegastheniker != istLegasthenikerNeu)
          {
            schueler.IsLegastheniker = istLegasthenikerNeu;
            log.Info(schueler.Name + ", " + schueler.Vorname + "(" + schueler.getKlasse.Bezeichnung + ") - " + "LRS: " + schueler.IsLegastheniker + " (alt) vs. " + istLegasthenikerNeu + " (neu)");
          }
        }
      }
    }

    private void CheckKlasse(int schuelerId, Schueler schueler)
    {
      command.CommandText = "SELECT KLASSE FROM DSchueler WHERE _SCHUELER_ID =" + schuelerId;
      using (var einzelresult = command.ExecuteReader())
      {
        if (einzelresult.Read())
        {
          if (!string.Equals(schueler.getKlasse.Bezeichnung, einzelresult.GetString(0), System.StringComparison.OrdinalIgnoreCase))
          {
            log.Warn(schueler.Name + ", " + schueler.Vorname + "(" + schueler.getKlasse.Bezeichnung + ") - " + "KLASSE: " + schueler.getKlasse.Bezeichnung + " (alt) vs. " + einzelresult.GetString(0) + " (neu)");
            // Klasse geändert: Manuelle Änderung nötig!
          }
        }
      }
    }

    private static IDictionary<string, string> GetStringPropertyMapping()
    {
      Dictionary<string, string> result = new Dictionary<string, string>();
      result.Add("Familienname", "Name");
      result.Add("Vornamen", "Vorname");
      result.Add("Rufname", "Rufname");
      result.Add("Geschlecht", "Geschlecht");
      result.Add("Geburtsort", "Geburtsort");
      result.Add("Bekenntnis", "Bekenntnis");
      result.Add("ERZB1_Famname", "NachnameEltern1");
      result.Add("ERZB1_Rufname", "VornameEltern1");
      result.Add("ERZB1_Anrede", "AnredeEltern1");
      result.Add("ERZB1_Art", "VerwandtschaftsbezeichnungEltern1");
      result.Add("ERZB2_Famname", "NachnameEltern2");
      result.Add("ERZB2_Rufname", "VornameEltern2");
      result.Add("ERZB2_Anrede", "AnredeEltern2");
      result.Add("ERZB2_Art", "VerwandtschaftsbezeichnungEltern2");
      result.Add("ANSCHR1_PLZ", "AnschriftPLZ");
      result.Add("ANSCHR1_Ort", "AnschriftOrt");
      result.Add("ANSCHR1_Str", "AnschriftStrasse");
      result.Add("ANSCHR1_Tel", "AnschriftTelefonnummer");
      result.Add("E_Mail1", "Email");
      result.Add("FREMDSPRACHE2", "Fremdsprache2");
      result.Add("RELIGION_ETHIK", "ReligionOderEthik");
      result.Add("WAHLPFLICHTF1", "Wahlpflichtfach");
      result.Add("WAHLFACH1", "Wahlfach1");
      // result.Add("WAHLFACH2", "Wahlfach2"); gibt es bei uns eh nicht
      // result.Add("WAHLFACH3", "Wahlfach3");
      // result.Add("WAHLFACH4", "Wahlfach4");
      result.Add("AUSBILDUNGSR", "Ausbildungsrichtung");

      return result;
    }

    private static object GetPropValue(object src, string propName)
    {
      try
      {
        return src.GetType().GetProperty(propName).GetValue(src, null);
      }
      catch (TargetInvocationException exp)
      {
        // die kommt evtl. ab und zu, weil bei den manuell neu eingetragenen Schülern wohl DBNull in der Datenbank an einigen Stellen steht
        log.Error(exp.Message, exp);
        return null;
      }
    }

    private static void SetPropValue(object src, string propName, object value)
    {
      src.GetType().GetProperty(propName).SetValue(src, value, null);
    }
  }
}
