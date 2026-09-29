using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using diNo.diNoDataSetTableAdapters;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von UserControlSekretariat.cs (WinForms). Die beiden LRS-Zuschlag-Felder und
  // die Fremdsprachennote (bisher NumericUpDownNullable) sind jetzt normale TextBoxen mit
  // NullableIntConverter (siehe FPAundSeminarViewModel für die Begründung).
  public partial class SekretariatViewModel : ObservableObject
  {
    private Schueler schueler;

    public bool ShowSave => Zugriff.Instance.HatVerwaltungsrechte;
    public List<KeyValuePair<int, string>> FachOptions { get; }
    public List<string> SchulischeVorbildungOptions { get; } = new List<string>
    {
      "", "BFo", "BFS", "BP", "BS", "BSo", "F10", "FAo", "GY0", "GY1", "H", "HSo", "HSq", "M", "QB",
      "R3a", "R3b", "RS", "RS1", "RS2", "RS3", "SoM", "VSo", "WS", "WSH", "WSM"
    };

    [ObservableProperty] private bool isRS = true;
    [ObservableProperty] private bool isErgPr;
    [ObservableProperty] private bool isFFalt;
    public bool NichtFFalt => !IsFFalt;
    partial void OnIsFFaltChanged(bool value) => OnPropertyChanged(nameof(NichtFFalt));
    [ObservableProperty] private int? andereFremdspr2Note;
    [ObservableProperty] private int andereFremdspr2Fach;
    [ObservableProperty] private string ffAltText = "";
    [ObservableProperty] private string zeugnisbemerkung = "";
    [ObservableProperty] private string schulischeVorbildung;
    [ObservableProperty] private bool franzB1;
    [ObservableProperty] private bool spanischB1;

    [ObservableProperty] private string schuelerId = "";
    [ObservableProperty] private bool legasthenie;
    [ObservableProperty] private int? lrsZuschlagMin = 0;
    [ObservableProperty] private int? lrsZuschlagMax = 0;
    [ObservableProperty] private string nachname = "";
    [ObservableProperty] private string vorname = "";
    [ObservableProperty] private string rufname = "";
    [ObservableProperty] private string ausbildungsrichtung = "";
    [ObservableProperty] private string schulart = "";
    [ObservableProperty] private string asvId = "";

    [ObservableProperty] private string eltern1Vorname = "";
    [ObservableProperty] private string eltern1Nachname = "";
    [ObservableProperty] private string eltern1Telefon = "";
    [ObservableProperty] private string eltern1Email = "";
    [ObservableProperty] private bool eltern1Haupt;

    [ObservableProperty] private string eltern2Vorname = "";
    [ObservableProperty] private string eltern2Nachname = "";
    [ObservableProperty] private string eltern2Telefon = "";
    [ObservableProperty] private string eltern2Email = "";
    [ObservableProperty] private bool eltern2Haupt;

    public SekretariatViewModel()
    {
      var ta = new FachTableAdapter();
      var faecher = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "") };
      foreach (var f in ta.GetData().Where(x => x.Kursniveau == (int)Kursniveau.Anfaenger))
        faecher.Add(new KeyValuePair<int, string>(f.Id, f.Bezeichnung));
      FachOptions = faecher;
    }

    public void SetSchueler(Schueler value)
    {
      schueler = value;
      if (schueler == null) return;

      int fs2Art = schueler.getNoten.ZweiteFSalt != null ? 2 : schueler.Data.AndereFremdspr2Art;
      IsRS = fs2Art == 0;
      IsErgPr = fs2Art == 1;
      IsFFalt = fs2Art == 2;
      AndereFremdspr2Note = schueler.Data.IsAndereFremdspr2NoteNull() ? (int?)null : schueler.Data.AndereFremdspr2Note;
      AndereFremdspr2Fach = schueler.Data.IsAndereFremdspr2FachNull() ? 0 : schueler.Data.AndereFremdspr2Fach;

      if (fs2Art == 2)
      {
        var f = schueler.getNoten.ZweiteFSalt;
        try
        {
          FfAltText = "Hj1 = " + f.getHjLeistung(HjArt.Hj1).Punkte + ", Hj2 = " + f.getHjLeistung(HjArt.Hj2).Punkte + " aus "
            + f.getFach.Kuerzel + " der Jgst. " + (int)f.getHjLeistung(HjArt.Hj1).JgStufe;
        }
        catch { FfAltText = ""; }
      }
      else FfAltText = "";

      Zeugnisbemerkung = schueler.Data.IsZeugnisbemerkungNull() ? "" : schueler.Data.Zeugnisbemerkung;
      SchulischeVorbildung = schueler.Data.IsSchulischeVorbildungNull() ? null : schueler.Data.SchulischeVorbildung;
      FranzB1 = !schueler.Data.IsFranzB1Null() && schueler.Data.FranzB1;
      SpanischB1 = !schueler.Data.IsSpanischB1Null() && schueler.Data.SpanischB1;

      SchuelerId = schueler.Id.ToString();
      Legasthenie = schueler.Data.LRSStoerung;
      LrsZuschlagMin = schueler.Data.LRSZuschlagMin;
      LrsZuschlagMax = schueler.Data.LRSZuschlagMax;
      Nachname = schueler.Data.Name;
      Vorname = schueler.Data.Vorname;
      Rufname = schueler.Data.Rufname;
      Ausbildungsrichtung = schueler.Data.Ausbildungsrichtung;
      Schulart = schueler.Data.Schulart;
      AsvId = schueler.AsvId;

      LadeElternteil("1");
      LadeElternteil("2");
    }

    private void LadeElternteil(string anschriftWessen)
    {
      var row = schueler.getAnschriftenRows().FirstOrDefault(a => a.AnschriftWessen == anschriftWessen);
      string vorname = row == null || row.IsVornamePersonNull() ? "" : row.VornamePerson;
      string nachname = row == null || row.IsNachnamePersonNull() ? "" : row.NachnamePerson;
      string telefon = row == null || row.IsTelefonnummerNull() ? "" : row.Telefonnummer;
      string email = row == null || row.IsEmailNull() ? "" : row.Email;
      bool haupt = row != null && !row.IsHauptAnsprechpartnerNull() && row.HauptAnsprechpartner;

      if (anschriftWessen == "1")
      {
        Eltern1Vorname = vorname; Eltern1Nachname = nachname; Eltern1Telefon = telefon; Eltern1Email = email; Eltern1Haupt = haupt;
      }
      else
      {
        Eltern2Vorname = vorname; Eltern2Nachname = nachname; Eltern2Telefon = telefon; Eltern2Email = email; Eltern2Haupt = haupt;
      }
    }

    [RelayCommand]
    private void Save()
    {
      if (AndereFremdspr2Note == null) schueler.Data.SetAndereFremdspr2NoteNull();
      else schueler.Data.AndereFremdspr2Note = AndereFremdspr2Note.Value;
      if (AndereFremdspr2Fach == 0) schueler.Data.SetAndereFremdspr2FachNull();
      else schueler.Data.AndereFremdspr2Fach = AndereFremdspr2Fach;

      schueler.Data.AndereFremdspr2Art = IsErgPr ? 1 : 0;

      if (Zeugnisbemerkung == "") schueler.Data.SetZeugnisbemerkungNull();
      else schueler.Data.Zeugnisbemerkung = Zeugnisbemerkung;

      if (string.IsNullOrEmpty(SchulischeVorbildung)) schueler.Data.SetSchulischeVorbildungNull();
      else schueler.Data.SchulischeVorbildung = SchulischeVorbildung;

      schueler.Data.FranzB1 = FranzB1;
      schueler.Data.SpanischB1 = SpanischB1;

      schueler.Data.LRSStoerung = Legasthenie;
      schueler.Data.LRSZuschlagMin = LrsZuschlagMin ?? 0;
      schueler.Data.LRSZuschlagMax = LrsZuschlagMax ?? 0;

      schueler.Data.Name = Nachname;
      schueler.Data.Vorname = Vorname;
      schueler.Data.Rufname = Rufname;
      schueler.Data.Ausbildungsrichtung = Ausbildungsrichtung;
      schueler.Data.Schulart = Schulart;
      schueler.Data.asv_id = AsvId;

      schueler.SaveErziehungsberechtigter("1", Eltern1Vorname.Trim(), Eltern1Nachname.Trim(), Eltern1Telefon.Trim(), Eltern1Email.Trim(), Eltern1Haupt);
      schueler.SaveErziehungsberechtigter("2", Eltern2Vorname.Trim(), Eltern2Nachname.Trim(), Eltern2Telefon.Trim(), Eltern2Email.Trim(), Eltern2Haupt);

      schueler.Save();
    }
  }
}
