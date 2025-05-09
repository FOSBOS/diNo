﻿using diNo.diNoDataSetTableAdapters;
using log4net;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.WinForms;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace diNo
{

  public abstract class ReportController
  {
    protected static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected ReportForm rpt;
    
    public ReportController()
    {
      rpt = new ReportForm();
    }

    public abstract void Init();

    public void Show()
    {
      Init();

      // Rendern über den ReportViewer:
      if (Zugriff.Instance.RptDruck)
      {
        if (rpt == null) return;
        rpt.reportViewer.RefreshReport();
        rpt.reportViewer.SetDisplayMode(DisplayMode.PrintLayout); // Darstellung sofort im Seitenlayout
        rpt.reportViewer.ZoomMode = ZoomMode.Percent;
        rpt.reportViewer.ZoomPercent = 100;
        rpt.Show();
      }
      else // als PDF speichern, dann öffnen, um Skalierungsprobleme zu vermeiden
      {
        string mimeType, encoding, filenameExtension;
        string[] streamids;
        Microsoft.Reporting.WinForms.Warning[] warnings;
        int numPages = rpt.reportViewer.LocalReport.GetTotalPages();
        byte[] bytes = rpt.reportViewer.LocalReport.Render(
           "PDF", null, out mimeType, out encoding, out filenameExtension,
           out streamids, out warnings);

        string file = @"C:\tmp\";
        if (!Directory.Exists(file))
            Directory.CreateDirectory(file);
        
        int k = 1;
        do
        {
          file = @"C:\tmp\dino" + k + ".pdf";
          try
          { // wenn die vorige Datei nicht geschlossen wurde, kann es Probleme geben.
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
              fs.Write(bytes, 0, bytes.Length);
            }
            k = 0; // hat geklappt
          }
          catch
          {
            if (k == 100)
            {
                MessageBox.Show("Fehler beim Erzeugen der PDF-Datei.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            k++;
          }          
        }
        while (k > 0);
        System.Diagnostics.Process.Start(file);
      }
    }
  }

  public class ReportNotencheck : ReportController
  {
    private NotenCheckResults bindingDataSource;
    private bool IsProtokolle;
    private bool IsPA;
    public ReportNotencheck(NotenCheckResults dataSource, bool aProtokolle) : base()
    {
      bindingDataSource = dataSource;
      IsProtokolle = aProtokolle;
      IsPA = Zugriff.Instance.aktZeitpunkt > (int)Zeitpunkt.ErstePA && Zugriff.Instance.aktZeitpunkt <= (int)Zeitpunkt.DrittePA;
    }

    public override void Init()
    {
      rpt.BerichtBindingSource.DataSource = bindingDataSource.list;
      if (IsProtokolle)
      {
        rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptKlassenkonferenz.rdlc";
        rpt.reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(subrptEventHandler);
        // Parameter PA liefert die Nummer des PA --> für Berichtsüberschrift
        rpt.reportViewer.LocalReport.SetParameters(new ReportParameter("PA", IsPA ? "Prüfungsausschuss nach der " +(Zugriff.Instance.aktZeitpunkt == (int)Zeitpunkt.ZweitePA ? "SAP" : "MAP") : "Klassenkonferenz"));
      }
      else
        rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptNotenCheck.rdlc";
    }

    void subrptEventHandler(object sender, SubreportProcessingEventArgs e)
    {
      int klassenId;
      int.TryParse(e.Parameters[0].Values[0], out klassenId);
      
      // keine Lehrerliste bei PA
      IList<LehrerDerKlasseDruck> lehrer = new List<LehrerDerKlasseDruck>();
      if (klassenId > 0 && !IsPA) 
      {
        lehrer = LehrerDerKlasseDruck.CreateLehrerDerKlasseDruck(klassenId);        
      }
      e.DataSources.Add(new ReportDataSource("DataSet1", lehrer));
    }
  }

  // kann diverse Schüler-/Notenberichte drucken, die Grunddaten sind jeweils eine Schülerliste  
  public class ReportSchuelerdruck : ReportController
  {
    private List<SchuelerDruck> bindingDataSource = new List<SchuelerDruck>();
    private string rptName;
    private Bericht rptTyp;

    public ReportSchuelerdruck(List<Schueler> dataSource, Bericht b, UnterschriftZeugnis u = UnterschriftZeugnis.SL) : base()
    {
      foreach (Schueler s in dataSource)
      {
        if (dataSource.Count > 1 && (
              b == Bericht.Zwischenzeugnis && !s.hatVorkommnis(Vorkommnisart.Zwischenzeugnis) ||
              b == Bericht.Jahreszeugnis && !s.hatVorkommnis(Vorkommnisart.Jahreszeugnis) ||
              b == Bericht.ZusatzAllgHSR && !(s.hatVorkommnis(Vorkommnisart.fachgebundeneHochschulreife) && !s.Data.IsAndereFremdspr2NoteNull() && s.getZweiteFSArt() == ZweiteFSArt.RS) ||
              b == Bericht.Abiturzeugnis && !(s.hatVorkommnis(Vorkommnisart.Fachabiturzeugnis) || s.hatVorkommnis(Vorkommnisart.fachgebundeneHochschulreife) || s.hatVorkommnis(Vorkommnisart.allgemeineHochschulreife))
              )) continue;
        bindingDataSource.Add(SchuelerDruck.CreateSchuelerDruck(s, b, u));
      }

      rptTyp = b;
      rptName = "diNo." + SchuelerDruck.GetBerichtsname(b) + ".rdlc";
    }

    public override void Init()
    {
      if (bindingDataSource.Count == 0)
      {
        MessageBox.Show("Keiner der ausgewählten Schüler hat das nötige Vorkommnis, um dieses Zeugnis zu drucken.", "diNo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        rpt = null;
        return;
      }
      rpt.BerichtBindingSource.DataSource = bindingDataSource;
      rpt.reportViewer.LocalReport.ReportEmbeddedResource = rptName;

      // Unterberichte einbinden
      rpt.reportViewer.LocalReport.SubreportProcessing +=
         new SubreportProcessingEventHandler(subrptEventHandler);

      // In einer Klassenliste wird ein Titel ausgegeben, wenn die Schüler über bestimmte Vorkommnisse selektiert wurden
      if ((rptTyp == Bericht.Klassenliste || rptTyp == Bericht.Auswahlliste) && Zugriff.Instance.markierteSchueler.Count > 0)
      {
        rpt.reportViewer.LocalReport.SetParameters(new ReportParameter("Titel", Vorkommnisse.Instance.VorkommnisText(Zugriff.Instance.selectedVorkommnisart)));
      }
    }

    void subrptEventHandler(object sender, SubreportProcessingEventArgs e)
    {
      // ACHTUNG: Der Parameter muss im Haupt- und im Unterbericht definiert werden (mit gleichem Namen)
      string subrpt = e.ReportPath; // jeder Unterbericht ruft diesen EventHandler auf; hier steht drin welcher es ist.
      int schuelerId;
      int.TryParse(e.Parameters[0].Values[0], out schuelerId);
      if (schuelerId > 0)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);


        if (subrpt.Substring(subrpt.Length - 7) == "zeugnis")
        {
          IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenZeugnisDruck(rptTyp);
          e.DataSources.Add(new ReportDataSource("DataSet1", noten));
        }
        else if (subrpt == "subrptZusZweiteFS")
        {
          e.DataSources.Add(new ReportDataSource("DataSet1", ZusZweiteFSDruck.CreateZusZweiteFSDruck(schueler)));
        }
        else if (subrpt == "subrptFPANoten")
        {
          e.DataSources.Add(new ReportDataSource("DataSetFPANoten", schueler.FPANotenDruck()));
        }
        else if (subrpt == "subrptPunktesumme" || subrpt == "subrptPunktesummeNB")
        {
          e.DataSources.Add(new ReportDataSource("DataSet1", PunkteSummeDruck.Create(schueler, rptTyp)));
        }
        else if (subrpt == "subrptFremdsprachen")
        {
          e.DataSources.Add(new ReportDataSource("DataSet1", SprachniveauDruck.Create(schueler)));
        }
        else if (subrpt == "subrptAbiRechteSeite" || subrpt == "subrptAbiLinkeSeite")
        {
          var z = SchuelerDruck.CreateSchuelerDruck(schueler, Bericht.Abiturzeugnis, UnterschriftZeugnis.SL);
          var l = new List<SchuelerDruck>();
          l.Add(z);
          e.DataSources.Add(new ReportDataSource("DataSet1", l));
        }
        else if (subrpt == "subrptVorkommnis" || subrpt == "subrptAbiVorkommnis")
        {
          diNoDataSet.vwVorkommnisDataTable vorkommnisse = new diNoDataSet.vwVorkommnisDataTable();
          vwVorkommnisTableAdapter BerichtTableAdapter;
          BerichtTableAdapter = new vwVorkommnisTableAdapter();
          if (subrpt == "subrptAbiVorkommnis")
            BerichtTableAdapter.FillBySchuelerIdForAbi(vorkommnisse, schuelerId);   //nur bestimmte Vorkommnisse selektieren              
          else
            BerichtTableAdapter.FillBySchuelerId(vorkommnisse, schuelerId);
          e.DataSources.Add(new ReportDataSource("DataSetVorkommnis", (DataTable)vorkommnisse));
        }
        else
        {
          IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenDruck(rptTyp);
          e.DataSources.Add(new ReportDataSource("DataSet1", noten));
        }
      }
    }
  }

  public class ReportBrief : ReportController
  {
    private BriefDaten bindingDataSource;
    public ReportBrief(BriefDaten dataSource) : base() { bindingDataSource = dataSource; }
    public override void Init()
    {
      rpt.BerichtBindingSource.DataSource = bindingDataSource;
      rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptBrief.rdlc";
    }
  }

  public class ReportGefaehrdungen : ReportController
  {
    private List<Schueler> orgdataSource;
    private List<BriefDaten> bindingDataSource;
    public ReportGefaehrdungen(List<Schueler> dataSource) : base()
    {
      orgdataSource = dataSource;
    }
    public override void Init()
    {
      // suche alle Schüler mit Gefährdungen
      bool einzelfall = false;
      bindingDataSource = new List<BriefDaten>();
      if (orgdataSource.Count == 1) // nur einen konkreten Schüler ausgewählt ==> allg. Gefährdungsschreiben vor PZ
      {
        Schueler s = orgdataSource[0];
        if (!s.hatVorkommnis(Vorkommnisart.BeiWeiteremAbsinken) && !s.hatVorkommnis(Vorkommnisart.starkeGefaehrdungsmitteilung))
        {
          einzelfall = true;
          var b = new BriefDaten(s, BriefTyp.Gefaehrdung);
          b.Inhalt = "Das Bestehen der Probezeit ist sehr gefährdet.";
          b.Inhalt2 = s.VornameName + " hat bei einem Punktedurchschnitt von " + String.Format("{0:0.00}", s.getNoten.Punkteschnitt) + " in den folgenden Fächern nur die angeführten Leistungen erzielt:";
          bindingDataSource.Add(b);

        }
      }

      if (!einzelfall)
      foreach (var s in orgdataSource)
      {
        if (s.hatVorkommnis(Vorkommnisart.BeiWeiteremAbsinken) || s.hatVorkommnis(Vorkommnisart.starkeGefaehrdungsmitteilung))
        {
          var b = new BriefDaten(s, BriefTyp.Gefaehrdung);
          if (s.hatVorkommnis(Vorkommnisart.BeiWeiteremAbsinken))
            b.Inhalt = "Bei weiterem Absinken der Leistungen ist das Erreichen des Klassenziels gefährdet.";
          else
            b.Inhalt = "Das Erreichen des Klassenziels ist sehr gefährdet.";
          if (s.hatVorkommnis(Vorkommnisart.GefahrDerAbweisung))
            b.Inhalt += "\nDie Jahrgangsstufe darf nicht mehr wiederholt werden.";

          b.Inhalt2 = s.VornameName + " hat bei einem Punktedurchschnitt von " + String.Format("{0:0.00}", s.getNoten.Punkteschnitt) + " in den folgenden Fächern nur die angeführten Leistungen erzielt:";
          bindingDataSource.Add(b);
        }
      }

      rpt.BerichtBindingSource.DataSource = bindingDataSource;
      rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptGefaehrdungen.rdlc";
      // Unterberichte einbinden
      rpt.reportViewer.LocalReport.SubreportProcessing +=
         new SubreportProcessingEventHandler(subrptEventHandler);
    }

    void subrptEventHandler(object sender, SubreportProcessingEventArgs e)
    {
      int schuelerId;
      int.TryParse(e.Parameters[0].Values[0], out schuelerId);
      if (schuelerId > 0)
      {
        Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
        IList<NotenDruck> noten = schueler.getNoten.SchuelerNotenZeugnisDruck(Bericht.Gefaehrdung);
        e.DataSources.Add(new ReportDataSource("DataSet1", noten));
      }
    }
  }

  public class ReportBerechtigungen : ReportController
  {
    private List<LehrerRolleDruck> bindingDataSource;
    public ReportBerechtigungen(List<LehrerRolleDruck> dataSource) : base() { bindingDataSource = dataSource; }
    public override void Init()
    {
      rpt.BerichtBindingSource.DataSource = bindingDataSource;
      rpt.reportViewer.LocalReport.ReportEmbeddedResource = "diNo.rptLehrerRolle.rdlc";
    }
  }

  public class Dummy
  {
    public int Id { get; private set; }
    public Dummy()
    {
      Id = 0;
    }
  }

}
