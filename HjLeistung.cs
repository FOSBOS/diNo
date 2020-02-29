using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using diNo.diNoDataSetTableAdapters;

namespace diNo
{
  public class HjLeistung
  {
    private diNoDataSet.HjLeistungRow data = null; // für Update
    private Fach fach=null;
    private int schuelerId;    
    public HjArt Art { get; private set; }
    public byte Punkte;
    public HjStatus Status=HjStatus.None;
    public Jahrgangsstufe JgStufe;
    public decimal? Punkte2Dez=null;
    public decimal? SchnittMdl=null;
    public static HjLeistungTableAdapter ta = new HjLeistungTableAdapter();

    public HjLeistung(int SchuelerId,Fach afach,HjArt aart, Jahrgangsstufe jg)
    {
      schuelerId=SchuelerId;
      Art=aart;
      fach=afach;
      JgStufe = jg;
    }

    public HjLeistung(diNoDataSet.HjLeistungRow r)
    {
      data = r;
      Art = (HjArt)r.Art;
      Punkte = r.Punkte;
      Status = (HjStatus) r.Status;
      JgStufe = (Jahrgangsstufe) r.JgStufe;
      if (!r.IsPunkte2DezNull()) Punkte2Dez =  r.Punkte2Dez;
      if (!r.IsSchnittMdlNull()) SchnittMdl =  r.SchnittMdl;      
    }

    public Fach getFach
    {
      get
      {
        if (fach == null)
          fach = Zugriff.Instance.FachRep.Find(data.FachId);
        return fach;
      }
      set // das passiert eigentlich nie. Nur bei Fachreferaten kann es sein, dass man das Fach überschreiben muss.
      {
        fach = value;
      }
    }

    public void WriteToDB()
    {    
      if (data==null) // neue HjLeistung -->INSERT
      {        
        ta.Insert(schuelerId,getFach.Id,(byte)Art,Punkte,Punkte2Dez,SchnittMdl,(int)JgStufe,(byte)Status);
      }
      else // vorhandene HjLeistung anpassen
      {
        data.Punkte=Punkte;
        data.Status = (byte)Status;
        data.FachId = getFach.Id;
        if (Punkte2Dez==null) data.SetPunkte2DezNull(); else data.Punkte2Dez = Punkte2Dez.GetValueOrDefault();
        if (SchnittMdl==null) data.SetSchnittMdlNull(); else data.SchnittMdl = SchnittMdl.GetValueOrDefault();
        ta.Update(data);
      }
    }
    public void Delete()
    {
      if (data != null)
      {              
        ta.Delete1(data.Id);
      }        
    }

    public void SetStatus(HjStatus s)
    {
      Status = s;
      WriteToDB();
    }

    public Color GetBackgroundColor()
    {
      if (Status == HjStatus.Ungueltig) return Color.Gray;
      if (Status == HjStatus.NichtEinbringen) return Color.LightGray;
      if (Status == HjStatus.AlternativeEinbr) return Color.LightBlue;
      if (Punkte < 1) return Color.Coral;
      if (Punkte < 3.5) return Color.Khaki;
      return Color.White;
    }

    public static void CreateOrUpdate(HjLeistung hjl, int sid, HjArt art, Fach fach, Jahrgangsstufe jg, byte? punkte, decimal? punkte2Dez = null, decimal? schnittMdl = null)
    {      
      if (hjl == null && punkte != null) // neu anlegen (nicht gefunden)
      {
        hjl = new HjLeistung(sid, fach, art, jg);
      }
      Update(hjl, punkte, punkte2Dez, schnittMdl);
    }

    public static void Update(HjLeistung hjl, byte? punkte, decimal? punkte2Dez = null, decimal? schnittMdl = null)
    { 
      if (punkte != null) // überschreiben
      {
        hjl.Punkte = (byte)punkte;
        hjl.Punkte2Dez = punkte2Dez;
        hjl.SchnittMdl = schnittMdl;
        hjl.WriteToDB();
      }
      else if (hjl != null) // HjLeistung wurde in dieser Exceldatei gelöscht
      {
        hjl.Delete();
      }
    }

  }

  public enum HjArt
  {
    Hj1 = 0, // aktuelles Sj./1
    Hj2 = 1, // aktuelles Sj./2
    AP = 2,  // Abschlussprüfung-Gesamt
    FR = 3,
    GesErg = 4, // Gesamtergebnis 
    JN = 5, // Jahresnote (stammt aus Excel und gibt unabhängig von den eingebrachten Leistungen den Durchschnitt von Hj1/2 an)    
    GesErgSprache = 6, // Gesamtergebnis für das Sprachniveau (unabhängig von der Einbringung)
    Sprachenniveau = 7 // keine echte HjLeistung, aber die Datenstruktur passt ganz gut
  }

  public enum HjStatus
  {
    None = 0,
    Einbringen = 1,
    NichtEinbringen = 2,
    Ungueltig = 3,        // z.B. Note steht zwar drin, aber es gibt nur eine Ex --> nicht bewertbar
    AlternativeEinbr = 4  // diese HjLeistung wird in der 13. Klasse alternativ zu einem der beiden Französisch-Hj eingebracht (fachgeb. HSR)
  }

}
