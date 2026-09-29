using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace diNo.ViewModels
{
  // WPF-Nachfolger von CopyLNW.cs (WinForms). Geschäftslogik (Pfadaufbau, Kopieren
  // über UserImpersonation) unverändert übernommen, nur die Steuerelement-Zugriffe
  // sind durch Properties/Commands ersetzt.
  public partial class CopyLNWViewModel : ObservableObject
  {
    private string verz, dat, kursBez;
    private bool serverValid = true;

    public List<KeyValuePair<int, string>> Kurse { get; }
    public List<string> ArtOptions { get; } = new List<string> { "SA", "KA", "Ex" };
    public List<string> NummerOptions { get; } = new List<string> { "1", "2", "3" };
    public List<string> HalbjahrOptions { get; } = new List<string> { "1", "2" };

    public ObservableCollection<string> AbgegebeneDateien { get; } = new ObservableCollection<string>();

    [ObservableProperty]
    private int selectedKursId;

    [ObservableProperty]
    private string selectedArt = "KA";

    [ObservableProperty]
    private string selectedNummer = "1";

    [ObservableProperty]
    private string selectedHalbjahr;

    [ObservableProperty]
    private bool isKoordiniert;

    [ObservableProperty]
    private bool isNachtermin;

    [ObservableProperty]
    private string selectedAbgegebeneDatei;

    public CopyLNWViewModel()
    {
      Kurse = Zugriff.Instance.eigeneKurse.Select(k => new KeyValuePair<int, string>(k.Id, k.Kursbezeichnung)).ToList();
      if (Kurse.Count > 0) SelectedKursId = Kurse[0].Key;
      SelectedHalbjahr = HalbjahrOptions[(byte)Zugriff.Instance.aktHalbjahr - 1];
      AktualisiereAbgabeListe();
    }

    partial void OnSelectedKursIdChanged(int value) => AktualisiereAbgabeListe();
    partial void OnSelectedHalbjahrChanged(string value) => AktualisiereAbgabeListe();

    private UserImpersonation GetCopyUser()
    {
      var copyUser = new UserImpersonation(
        Zugriff.Instance.getString(GlobaleStrings.CopyUserLoginname),
        Zugriff.Instance.getString(GlobaleStrings.CopyUserDomain),
        Zugriff.Instance.getString(GlobaleStrings.CopyUserPwd));
      if (!copyUser.ImpersonateValidUser() && serverValid)
      {
        MessageBox.Show("Der Server steht nicht zur Verfügung.", "diNo", MessageBoxButton.OK, MessageBoxImage.Error);
        serverValid = false;
      }
      return copyUser;
    }

    private void SetzePfade(string datei, string art, bool delete)
    {
      Kurs k = Zugriff.Instance.KursRep.Find(SelectedKursId);
      kursBez = k.Kursbezeichnung.Replace("/", "");
      string typ = Path.GetExtension(datei);

      verz = Zugriff.Instance.getString(GlobaleStrings.LNWAblagePfad) + @"\" + k.getFach.Fachschaft
        + @"\Hj" + SelectedHalbjahr + @"\" + k.FachBezeichnung.Replace("/", "") + @"\";
      if (k.getFach.Typ != FachTyp.WPF)
        verz += "Jg" + k.JgStufe + @"\";

      dat = Zugriff.Instance.getString(GlobaleStrings.SchulnummerFOS) + "_" + kursBez + "_Hj" + SelectedHalbjahr + "_";
      if (delete)
        dat += datei;
      else
      {
        dat += SelectedArt + SelectedNummer;
        if (IsKoordiniert) dat += "_koordiniert";
        if (IsNachtermin) dat += "_Nachtermin";
        dat += "_" + art + typ;
      }
    }

    private void Kopiere(string datei, string art)
    {
      string tmp = @"C:\tmpCopyLNW\";
      if (!Directory.Exists(tmp))
        Directory.CreateDirectory(tmp);

      tmp += Path.GetFileName(datei);
      File.Copy(datei, tmp, true);
      SetzePfade(datei, art, false);

      using (GetCopyUser())
        try
        {
          if (!Directory.Exists(verz))
            Directory.CreateDirectory(verz);
          if (File.Exists(verz + dat))
          {
            if (MessageBox.Show("Die " + art + " wurde bereits archiviert.\nSoll die Datei ersetzt werden?\n(Sind alle Einstellungen richtig?)",
                  "diNo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
              return;
          }
          File.Copy(tmp, verz + dat, true);
          MessageBox.Show("Die " + art + " wurde archiviert.", "diNo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
          MessageBox.Show("Die " + art + " konnte nicht archiviert werden.\n" + e.Message, "diNo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      AktualisiereAbgabeListe();
    }

    private void AktualisiereAbgabeListe()
    {
      if (Kurse.Count == 0) return;
      SetzePfade("dummy.pdf", "", false);
      AbgegebeneDateien.Clear();

      using (GetCopyUser())
        try
        {
          foreach (var f in Directory.GetFiles(verz, "*" + kursBez + "*", SearchOption.TopDirectoryOnly))
            AbgegebeneDateien.Add(f.Substring(5 + f.IndexOf("_Hj")));
        }
        catch
        {
          // keine Fehlerausgabe
        }
    }

    private void Abgabe(string art)
    {
      var fileDialog = new OpenFileDialog { Filter = "PDF|*.pdf|MP3|*.mp3" };
      if (fileDialog.ShowDialog() == true)
      {
        Mouse.OverrideCursor = Cursors.Wait;
        try { Kopiere(fileDialog.FileName, art); }
        finally { Mouse.OverrideCursor = null; }
      }
    }

    [RelayCommand]
    private void Angabe() => Abgabe("Angabe");

    [RelayCommand]
    private void Lsg() => Abgabe("Lösung");

    [RelayCommand]
    private void Delete()
    {
      if (SelectedAbgegebeneDatei == null)
      {
        MessageBox.Show("Bitte erst eine Datei auswählen.", "diNo", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        return;
      }
      SetzePfade(SelectedAbgegebeneDatei, "", true);
      using (GetCopyUser())
        try { File.Delete(verz + dat); }
        catch (Exception ex)
        {
          MessageBox.Show("Die Datei " + verz + dat + " konnte nicht gelöscht werden.\n" + ex.Message, "diNo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      AktualisiereAbgabeListe();
    }
  }
}
