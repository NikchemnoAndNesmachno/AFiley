using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FileyCore;
using FileyCore.Interfaces;

namespace AFiley;

public partial class AFiley : UserControl
{
    private FlatTreeDataGridSource<IFileyItem> FlatSource;
    private HierarchicalTreeDataGridSource<DirectoryInfo> TreeSource;
    
    public static readonly StyledProperty<string> CurrentPathProperty =
        AvaloniaProperty.Register<AFiley, string>(nameof(CurrentPath));

    public string CurrentPath
    {
        get => GetValue(CurrentPathProperty);
        set => SetValue(CurrentPathProperty, value);
    }
    
    public FileMethod FileMethods { get; set; } = new()
    {
        FileyFabric = new OFileyFabric()
    };
    public AFiley()
    {
        InitializeComponent();
    }

    private void InitTableSource(IEnumerable<IFileyItem> items)
    {
        
        FlatSource = new FlatTreeDataGridSource<IFileyItem>(items)
        {
            Columns =
            {
                new TextColumn<IFileyItem,string>("Name", x=> x.Name)
                {
                  Options  =
                  {
                      CompareAscending = (x, y) =>
                      {
                          if (x is null || y is null) return 0;
                          return x.IsDirectory && !y.IsDirectory ? -1 : string.CompareOrdinal(x.Name, y.Name);
                      },
                      CompareDescending= (x, y) =>
                      {
                          if (x is null || y is null) return 0;
                          return y.IsDirectory && !x.IsDirectory ? -1 : string.CompareOrdinal(y.Name, x.Name);
                      }
                  }
                },
                new TemplateColumn<IFileyItem>("Size", new FileSizeTemplate())
                {
                    Options =
                    {
                        CompareAscending = (x, y)=>x?.Size.CompareTo(y?.Size) ?? 0,
                        CompareDescending = (x, y)=>y?.Size.CompareTo(x?.Size) ?? 0,
                    }
                },
                new TextColumn<IFileyItem,DateTime>("Date modified", x=>x.DateModified)
            }
        };
        FileTable.Source = FlatSource;
    }
    private void InitTreeSource(IEnumerable<DirectoryInfo> items)
    {
        TreeSource = new HierarchicalTreeDataGridSource<DirectoryInfo>(items)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<DirectoryInfo>(
                    new TextColumn<DirectoryInfo,string>
                        ("Directory", x=> x.Name),
                    x =>
                    {
                        try
                        {
                            return x.GetDirectories();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return [];
                        }
                    })
                
            }
        };
        TreeSource.RowSelection.SelectionChanged += OnTreeSelectionChanged;
        FileTree.Source = TreeSource;
    }

    private void OnTreeSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs e)
    {
        /*if(e.SelectedIndexes.Count == 0) return;
        var item = e.SelectedItems[0];
        var index = e.SelectedIndexes[0];
        if (item is not DirectoryInfo selectedDir) return;
        CurrentPath = selectedDir.FullName;
        try
        {
            FileMethods.LoadFull(selectedDir);
            InitTableSource(selectedDir.Files);
            var rowIndex = FileTree.Rows.ModelIndexToRowIndex(index);
            FileTree.RowsPresenter?.BringIntoView(rowIndex);
            TreeSource.Expand(index);
        }
        catch (UnauthorizedAccessException exception)
        {
            Console.WriteLine(exception);
        }
        */
    }

    public void LoadDrives()
    {
        var drives = DriveInfo.GetDrives();

        var items = drives.Select(x => x.RootDirectory);
        InitTreeSource(items);
    }
    

    private (int, IFileyItem?) IndexOf(IEnumerable<IFileyItem> items, string name)
    {
        var k = -1;
        foreach (var filey in items)
        {
            k++;
            if (filey.Name == name)
            {
                return (k, filey);
            }
        }
        return (-1, null);
    }
    
    public void Browse(object? sender, RoutedEventArgs routedEventArgs)
    {
        /*if(!Path.Exists(CurrentPath)) return;
        var segments = new Uri(CurrentPath).Segments;
        var indexes = new IndexPath();
        var collectionToFind = TreeSource.Items.ToArray();
        for (int i = 0; i < segments.Length; i++)
        {
            var name = Uri.UnescapeDataString(i == 0 ? segments[i] : segments[i].TrimEnd('/'));
            var (index, item) = IndexOf(collectionToFind, name);
            if (index == -1 || item is null) return;
            indexes = indexes.Append(index);
            FileMethods.LoadFull(item);
            //collectionToFind = item.Files.ToArray();
        }
        TreeSource.Expand(indexes);
        TreeSource.RowSelection?.Select(indexes);*/
    }

    private void Load(object? sender, RoutedEventArgs e)
    {
        LoadDrives();
    }

    private void FileTable_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedTableItem = FlatSource.RowSelection?.SelectedItem;
        if (selectedTableItem is null || !selectedTableItem.IsDirectory) return;
        var selectedTableItemIndex = FlatSource.RowSelection?.SelectedIndex;
        var selectedTreeItemIndex = TreeSource.RowSelection?.SelectedIndex;
        if(selectedTableItemIndex is null || selectedTreeItemIndex is null) return;
        var treeIndex = selectedTreeItemIndex.Value.Append(selectedTableItemIndex.Value[0]);
        TreeSource.RowSelection?.Select(treeIndex);

    }
}