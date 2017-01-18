﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public bool Einbringen=false;
    decimal? Punkte2Dez=null;
    decimal? SchnittMdl=null;

    public HjLeistung(int SchuelerId,Fach afach,HjArt aart)
    {
      schuelerId=SchuelerId;
      Art=aart;
      fach=afach;      
    }

    public HjLeistung(diNoDataSet.HjLeistungRow r)
    {
      data = r;
      Art = (HjArt)r.Art;
      Punkte = r.Punkte;
      Einbringen = r.Einbringen;
      Punkte2Dez = r.Punkte2Dez;
      SchnittMdl = r.SchnittMdl;
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
        ta.Insert(schuelerId,getFach.Id,(byte)Art,Punkte,Einbringen,Punkte2Dez,SchnittMdl);
      }
      else // vorhandene HjLeistung anpassen
      {
        data.Punkte=Punkte;
        data.Einbringen = Einbringen;
        if (Punkte2Dez==null) data.SetPunkte2DezNull(); else data.Punkte2Dez = Punkte2Dez.GetValueOrDefault();
        if (SchnittMdl==null) data.SetSchnittMdlNull(); else data.SchnittMdl = SchnittMdl.GetValueOrDefault();
        ta.Update(data);
      }
    }
  }

  public enum HjArt
  {
    Hj1 = 0, // aktuelles Sj./1
    Hj2 = 1, // aktuelles Sj./2
    FR = 2, // Fachreferat
    AP = 3,  // Abschlussprüfung-Schnitt
    Fachnote = 4,
    VorHj1 = 5, // ggf. für 11/1
    VorHj2 = 6 // ggf. für 11/2
  }
}
