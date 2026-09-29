namespace diNo.ViewModels
{
  // Eine Fach-Zeile im Notenbogen (Ersatz für die dynamisch befüllten Zeilen von dataGridNoten
  // in UserControlNotenbogen.cs).
  public class NotenRow
  {
    public string FachText { get; set; } = "";

    public NotenZelle Zelle11_1 { get; set; }
    public NotenZelle Zelle11_2 { get; set; }

    public string SlHj1 { get; set; } = "";
    public string SchnittMdl1 { get; set; } = "";
    public string SaHj1 { get; set; } = "";
    public string Punkte2DezHj1 { get; set; } = "";
    public NotenZelle ZelleHj1 { get; set; }

    public string SlHj2 { get; set; } = "";
    public string SchnittMdl2 { get; set; } = "";
    public string SaHj2 { get; set; } = "";
    public string Punkte2DezHj2 { get; set; } = "";
    public NotenZelle ZelleHj2 { get; set; }

    public string SAP { get; set; } = "";
    public string MAP { get; set; } = "";
    public NotenZelle ZelleAPG { get; set; }
    public NotenZelle ZelleJN { get; set; }
    public NotenZelle ZelleGE { get; set; }
  }

  // Eine Zeile in der Punktesumme-Tabelle (dataGridPunktesumme).
  public class PunktesummeRow
  {
    public string Text { get; set; }
    public int Summe { get; set; }
    public int Anzahl { get; set; }
  }
}
