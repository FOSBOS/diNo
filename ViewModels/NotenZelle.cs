using System;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;

namespace diNo.ViewModels
{
  // Eine editierbare Notenpunkte-Zelle im Notenbogen (Ersatz für die WinForms-DataGridViewCell
  // mit Tag=HjLeistung in UserControlNotenbogen.cs). HjLeistung==null + NeuanlageArt!=null heißt:
  // Doppelklick legt eine neue HjLeistung an (leere Zelle); NeuanlageArt==null heißt: nicht anlegbar.
  public class NotenZelle
  {
    public string Text { get; set; } = "";
    public Brush Background { get; set; } = Brushes.White;
    public HjLeistung HjLeistung { get; set; }
    public Fach Fach { get; set; }
    public HjArt? NeuanlageArt { get; set; }
    public Jahrgangsstufe Jahrgangsstufe { get; set; }

    public ICommand DoubleClickCommand { get; }
    public ICommand SetStatusCommand { get; }

    public NotenZelle(Action<NotenZelle> onDoubleClick, Action<NotenZelle, HjStatus> onSetStatus)
    {
      DoubleClickCommand = new RelayCommand(() => onDoubleClick(this));
      SetStatusCommand = new RelayCommand<HjStatus>(status => onSetStatus(this, status));
    }

    public static Brush ToBrush(System.Drawing.Color c) => new SolidColorBrush(Color.FromRgb(c.R, c.G, c.B));
  }
}
