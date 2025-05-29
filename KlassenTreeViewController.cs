using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;

namespace diNo
{
  public class KlassenTreeViewController
  {    
    public static object SelectValueCol1(Object rowObject)
    {
      if (rowObject is Klasse)
      {
        return ((Klasse)rowObject).Bezeichnung;
      }

      if (rowObject is Schueler)
      {
        return ((Schueler)rowObject).NameVorname;
      }

      throw new InvalidOperationException("no aspect getter for this object given");
    }
  }
}
