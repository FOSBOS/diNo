using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von Klassenansicht.cs (WinForms), Phase 2 der WPF-Umstellung. Der Klassenbaum
  // ist inzwischen eine native, flach gehaltene WPF-ListBox (KlassenBaumViewModel/TreeRow) statt
  // der WinForms-TreeListView (BrightIdeasSoftware) - siehe Claude\UmstellungWPF.md.txt. Die 4
  // verbleibenden WinForms-UserControls (Notenbogen, FPAundSeminar, Sekretariat, Administration)
  // werden vorerst weiter unverändert per WindowsFormsHost eingebunden.
  public partial class KlassenansichtWindow : Window
  {
    private readonly KlassenansichtViewModel vm;
    private readonly KlassenBaumViewModel baumVm = new KlassenBaumViewModel();

    private readonly UserControlNotenbogen userControlNotenbogen1 = new UserControlNotenbogen();
    private readonly UserControlFPAundSeminar userControlFPAundSeminar1 = new UserControlFPAundSeminar();
    private readonly UserControlSekretariat userControlSekretariat1 = new UserControlSekretariat();
    private readonly UserControlAdministration userControlAdministration1 = new UserControlAdministration();

    private Point dragStartPoint;
    private Schueler dragSchueler;

    public KlassenansichtWindow()
    {
      InitializeComponent();

      BaumListBox.DataContext = baumVm;

      NotenbogenHost.Child = userControlNotenbogen1;
      FPAundSeminarHost.Child = userControlFPAundSeminar1;
      SekretariatHost.Child = userControlSekretariat1;
      AdministrationHost.Child = userControlAdministration1;

      vm = new KlassenansichtViewModel(
        roots => baumVm.SetKlassen(roots),
        GetTreeSelectedObjects,
        PushSchuelerToUserControls,
        () => VorkommnisseView.RefreshVorkommnisse());
      DataContext = vm;

      userControlAdministration1.SelectedObjectsProvider = vm.SelectedObjects;
      userControlAdministration1.SchuelerChangedNotifier = vm.RefreshSchuelerAnzeige;

      BuildPrintContextMenu();
    }

    private void PushSchuelerToUserControls(Schueler schueler)
    {
      SchueleransichtView.SetSchueler(schueler);
      VorkommnisseView.SetSchueler(schueler);
      userControlFPAundSeminar1.Schueler = schueler;
      userControlNotenbogen1.Schueler = schueler;

      if (Zugriff.Instance.HatVerwaltungsrechte)
      {
        KurszuordnungenView.SetSchueler(schueler);
        userControlAdministration1.Schueler = schueler;
        userControlSekretariat1.Schueler = schueler;
      }
    }

    private IList GetTreeSelectedObjects()
    {
      var result = new ArrayList();
      foreach (TreeRow row in BaumListBox.SelectedItems)
        result.Add(row.Model);
      return result;
    }

    private void BaumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var primary = BaumListBox.SelectedItem as TreeRow;
      vm.OnTreeSelectionChanged(primary?.Model);
    }

    private void BaumListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      dragStartPoint = e.GetPosition(null);
      dragSchueler = GetRowAt(e.GetPosition(BaumListBox))?.Schueler;
    }

    private void BaumListBox_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (dragSchueler == null || e.LeftButton != MouseButtonState.Pressed || !vm.HatVerwaltungsrechte) return;

      var pos = e.GetPosition(null);
      if (Math.Abs(pos.X - dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
          Math.Abs(pos.Y - dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
        return;

      var schueler = dragSchueler;
      dragSchueler = null;
      DragDrop.DoDragDrop(BaumListBox, schueler, DragDropEffects.Move);
    }

    private void BaumListBox_DragOver(object sender, DragEventArgs e)
    {
      e.Effects = vm.HatVerwaltungsrechte && e.Data.GetDataPresent(typeof(Schueler)) ? DragDropEffects.Move : DragDropEffects.None;
      e.Handled = true;
    }

    private void BaumListBox_Drop(object sender, DragEventArgs e)
    {
      if (!vm.HatVerwaltungsrechte || !(e.Data.GetData(typeof(Schueler)) is Schueler derSchueler)) return;

      var targetRow = GetRowAt(e.GetPosition(BaumListBox));
      var targetKlasse = targetRow?.Klasse ?? targetRow?.Schueler?.getKlasse;
      if (targetKlasse == null) return;

      if (MessageBox.Show("Soll der Schüler " + derSchueler.NameVorname + " von der " + derSchueler.getKlasse.Bezeichnung +
          " in die " + targetKlasse.Bezeichnung + " verschoben werden?", "Nachfrage", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        derSchueler.WechsleKlasse(targetKlasse);
        vm.RefreshCommand.Execute(null);
      }
    }

    private TreeRow GetRowAt(Point position)
    {
      var element = BaumListBox.InputHitTest(position) as DependencyObject;
      while (element != null && !(element is ListBoxItem))
        element = VisualTreeHelper.GetParent(element);
      return (element as ListBoxItem)?.DataContext as TreeRow;
    }

    private void BuildPrintContextMenu()
    {
      PrintContextMenu.Items.Add(new MenuItem { Header = "Klassenliste", Command = vm.DruKlassenlisteCommand });
      PrintContextMenu.Items.Add(new MenuItem { Header = "Notenbogen", Command = vm.DruNotenbogenCommand });
      PrintContextMenu.Items.Add(new MenuItem { Header = "Übersicht Legastheniker", Command = vm.DruLegasthenikerCommand });

      if (vm.KursDruckItems.Count > 0)
      {
        PrintContextMenu.Items.Add(new Separator());
        foreach (var item in vm.KursDruckItems)
          PrintContextMenu.Items.Add(new MenuItem { Header = item.Bezeichnung, Command = vm.DruKurslisteCommand, CommandParameter = item });
      }
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e) => PrintContextMenu.IsOpen = true;

    private void SuchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter && vm.SuchenCommand.CanExecute(null))
        vm.SuchenCommand.Execute(null);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => vm.OnLoaded();
  }
}
