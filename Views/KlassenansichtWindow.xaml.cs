using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using diNo.ViewModels;

namespace diNo.Views
{
  // WPF-Nachfolger von Klassenansicht.cs (WinForms), Phase 2 der WPF-Umstellung: die 7
  // bestehenden WinForms-UserControls und die TreeListView (ObjectListView) werden vorerst
  // unverändert per WindowsFormsHost eingebunden (siehe Claude\UmstellungWPF.md.txt), Toolbar/
  // Suche/Statusleiste/Tab-Sichtbarkeit sind natives WPF/MVVM (KlassenansichtViewModel).
  public partial class KlassenansichtWindow : Window
  {
    private readonly KlassenansichtViewModel vm;
    private readonly BrightIdeasSoftware.TreeListView treeListView1;

    private readonly UserControlNotenbogen userControlNotenbogen1 = new UserControlNotenbogen();
    private readonly UserControlFPAundSeminar userControlFPAundSeminar1 = new UserControlFPAundSeminar();
    private readonly UserControlVorkommnisse userControlVorkommnisse1 = new UserControlVorkommnisse();
    private readonly UserControlKurszuordnungen userControlKurszuordnungen1 = new UserControlKurszuordnungen();
    private readonly UserControlSekretariat userControlSekretariat1 = new UserControlSekretariat();
    private readonly UserControlAdministration userControlAdministration1 = new UserControlAdministration();

    public KlassenansichtWindow()
    {
      InitializeComponent();

      treeListView1 = BuildTreeListView();
      TreeHost.Child = treeListView1;

      NotenbogenHost.Child = userControlNotenbogen1;
      FPAundSeminarHost.Child = userControlFPAundSeminar1;
      VorkommnisseHost.Child = userControlVorkommnisse1;
      KurszuordnungenHost.Child = userControlKurszuordnungen1;
      SekretariatHost.Child = userControlSekretariat1;
      AdministrationHost.Child = userControlAdministration1;

      vm = new KlassenansichtViewModel(
        roots => treeListView1.Roots = roots,
        () => treeListView1.SelectedObjects,
        PushSchuelerToUserControls,
        () => userControlVorkommnisse1.RefreshVorkommnisse());
      DataContext = vm;

      userControlAdministration1.SelectedObjectsProvider = vm.SelectedObjects;
      userControlAdministration1.SchuelerChangedNotifier = vm.RefreshSchuelerAnzeige;

      if (vm.VerwaltungController != null)
      {
        treeListView1.IsSimpleDragSource = true;
        treeListView1.IsSimpleDropSink = true;
        treeListView1.ModelCanDrop += vm.VerwaltungController.treeListView1_ModelCanDrop;
        treeListView1.ModelDropped += vm.VerwaltungController.treeListView1_ModelDropped;
      }

      BuildPrintContextMenu();
    }

    private BrightIdeasSoftware.TreeListView BuildTreeListView()
    {
      var olvColumnBezeichnung = new BrightIdeasSoftware.OLVColumn
      {
        Text = "Bezeichnung",
        Width = 191,
        Hideable = false,
        IsEditable = false,
        AspectGetter = KlassenTreeViewController.SelectValueCol1
      };

      var tree = new BrightIdeasSoftware.TreeListView
      {
        Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
        HideSelection = false,
        OwnerDraw = true,
        ShowGroups = false,
        UseCompatibleStateImageBehavior = false,
        View = System.Windows.Forms.View.Details,
        VirtualMode = true,
        CanExpandGetter = x => x is Klasse,
        ChildrenGetter = x => ((Klasse)x).Schueler
      };
      tree.AllColumns.Add(olvColumnBezeichnung);
      tree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { olvColumnBezeichnung });
      tree.SelectedIndexChanged += (s, e) => vm.OnTreeSelectionChanged(tree.SelectedObject);
      return tree;
    }

    private void PushSchuelerToUserControls(Schueler schueler)
    {
      SchueleransichtView.SetSchueler(schueler);
      userControlVorkommnisse1.Schueler = schueler;
      userControlFPAundSeminar1.Schueler = schueler;
      userControlNotenbogen1.Schueler = schueler;

      if (Zugriff.Instance.HatVerwaltungsrechte)
      {
        userControlKurszuordnungen1.Schueler = schueler;
        userControlAdministration1.Schueler = schueler;
        userControlSekretariat1.Schueler = schueler;
      }
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
