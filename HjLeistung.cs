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
    private diNoDataSet.HjLeistungRow data; // für Update
    private Fach fach=null;
    private int schuelerId;    
    public HjArt Art { get; private set; }
    public byte Punkte;
    public HjStatus Status=HjStatus.None;
    public Jahrgangsstufe JgStufe;
    public decimal? Punkte2Dez=null;
    public decimal? SchnittMdl=null;

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

    public Fach getFach { get
    {
      if (fach==null)
          fach=Zugriff.Instance.FachRep.Find(data.FachId);
      return fach;
    }}

    public void WriteToDB()
    {
      var ta = new HjLeistungTableAdapter();
      if (data==null) // neue HjLeistung -->INSERT
      {        
        ta.Insert(schuelerId,getFach.Id,(byte)Art,Punkte,Punkte2Dez,SchnittMdl,(int)JgStufe,(byte)Status);
      }
      else // vorhandene HjLeistung anpassen
      {
        data.Punkte=Punkte;
        data.Status = (byte)Status;
        if (Punkte2Dez==null) data.SetPunkte2DezNull(); else data.Punkte2Dez = Punkte2Dez.GetValueOrDefault();
        if (SchnittMdl==null) data.SetSchnittMdlNull(); else data.SchnittMdl = SchnittMdl.GetValueOrDefault();
        ta.Update(data);
      }
    }
    public void Delete()
    {
      if (data != null)
      {
        var ta = new HjLeistungTableAdapter();        
        ta.Delete1(data.Id);
      }        
    }

    public Color GetBackgroundColor()
    {
      if (Status == HjStatus.Ungueltig) return Color.Gray;
      if (Status == HjStatus.NichtEinbringen) return Color.LightGray;
      if (Punkte < 1) return Color.Coral;
      if (Punkte < 3.5) return Color.Khaki;
      return Color.White;
    }
  }


  public enum HjArt
  {
    Hj1 = 0, // aktuelles Sj./1
    Hj2 = 1, // aktuelles Sj./2
    JN = 2, // Jahresnote (stammt aus Excel und gibt unabhängig von den eingebrachten Leistungen den Durchschnitt von Hj1/2 an)    
    AP = 3,  // Abschlussprüfung-Gesamt
    GesErg = 4, // Gesamtergebnis 
    FR = 5
  }

  public enum HjStatus
  {
    None = 0,
    Einbringen = 1,
    NichtEinbringen = 2,
    Ungueltig = 3 // z.B. Note steht zwar drin, aber es gibt nur eine Ex --> nicht bewertbar
  }
}
