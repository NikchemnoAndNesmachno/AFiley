using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AFiley;

public partial class AFileyPickerBase: UserControl
{
    public static readonly StyledProperty<string> CurrentPathProperty =
        AvaloniaProperty.Register<AFileyPickerBase, string>(nameof(CurrentPath));

    public string CurrentPath
    {
        get => GetValue(CurrentPathProperty);
        set => SetValue(CurrentPathProperty, value);
    }
    public AFileyPickerBase()
    {
        InitializeComponent();
    }
    protected HierarchicalTreeDataGridSource<DirectoryItem> _treeSource;
    protected FlatTreeDataGridSource<IFileItem> _flatSource;

    public IList<string> SelectedItems
    {
        get => SelectedFileList.SelectedFiles;
        set => SelectedFileList.SelectedFiles = value;
    }
    protected void LoadDrives(object? sender, RoutedEventArgs e)
    {
        var drives = DriveInfo.GetDrives();
        var items = drives.Select(x => new DirectoryItem(x.RootDirectory));
        InitTreeSource(new ObservableCollection<DirectoryItem>(items));
    }
    
    protected (int, DirectoryItem?) IndexOf(IEnumerable<DirectoryItem> items, string name)
    {
        var k = -1;
        foreach (var filey in items)
        {
            k++;
            if (filey.DirectoryInfo.Name == name) return (k, filey);
        }
        return (-1, null);
    }
    protected void InitTreeSource(IEnumerable<DirectoryItem> items)
    {
        _treeSource = new HierarchicalTreeDataGridSource<DirectoryItem>(items)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<DirectoryItem>(
                    new TextColumn<DirectoryItem,string>
                        ("Directory", x=> x.DirectoryInfo.Name),
                    x => x.Directories)
                
            }
        };
        _treeSource.RowSelection.SelectionChanged += OnTreeSelectionChanged;
        FileTree.Source = _treeSource;
    }
    
    protected void GoTo(object? sender, RoutedEventArgs e)
    {
        if(!Path.Exists(CurrentPath)) return;
        var segments = new Uri(CurrentPath).Segments;
        var indexes = new IndexPath();
        IList<DirectoryItem> collectionToFind = _treeSource.Items.ToArray();
        for (int i = 0; i < segments.Length; i++)
        {
            var name = Uri.UnescapeDataString(i == 0 ? segments[i] : segments[i].TrimEnd('/'));
            var (index, item) = IndexOf(collectionToFind, name);
            if (index == -1 || item is null) return;
            indexes = indexes.Append(index);
            _treeSource.RowSelection?.Select(indexes);
            collectionToFind = item.Directories;
        }
    }
    
    protected void FileTree_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedTreeItem = _treeSource.RowSelection?.SelectedItem;
        var selectedTreeItemIndex = _treeSource.RowSelection?.SelectedIndex;
        if (selectedTreeItem is null || selectedTreeItemIndex is null) return;
        selectedTreeItem.ReadDirectories();
    }
    
    
    protected void AddToSelected(object? sender, RoutedEventArgs e)
    {
        if(string.IsNullOrEmpty(CurrentPath)) return;
        SelectedItems.Add(CurrentPath);
    }

    protected virtual void ReadItems(DirectoryItem item) => item.ReadDirectories();

    protected virtual FlatTreeDataGridSource<IFileItem> CreateSource(DirectoryItem dir) => new(dir.AllItems);
    protected bool IsDoubleTapped(IFileItem? fileItem) => fileItem?.IsDirectory ?? false;
    
    private void OnTreeSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<DirectoryItem> e)
    {
        if(e.SelectedIndexes.Count == 0) return;
        
        var item = e.SelectedItems[0];
        if(item is null) return;
        CurrentPath = item.DirectoryInfo.FullName;
        if (item.Directories.Count == 0)
        {
            ReadItems(item);
        }

        InitTableSource(item);
        
        var index = e.SelectedIndexes[0];
        var rowIndex = FileTree.Rows.ModelIndexToRowIndex(index);
        FileTree.RowsPresenter?.BringIntoView(rowIndex);
        _treeSource.Expand(index);
    }
    
    protected void InitTableSource(DirectoryItem dir)
    {
        _flatSource = CreateSource(dir);
        _flatSource.RowSelection.SelectionChanged += OnFlatSelectionChanged;
        FileTable.Source = _flatSource;
    }
    protected void OnFlatSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<IFileItem> e)
    {
        if(e.SelectedItems.Count == 0) return;
        var item = e.SelectedItems[0];
        if(item is null) return;
        CurrentPath = item.FullName;
    }
    
    private void FileTable_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedTableItem = _flatSource.RowSelection?.SelectedItem;
        if(!IsDoubleTapped(selectedTableItem)) return;
        var selectedTableItemIndex = _flatSource.RowSelection?.SelectedIndex;
        var selectedTreeItemIndex = _treeSource.RowSelection?.SelectedIndex;
        if(selectedTableItemIndex is null || selectedTreeItemIndex is null) return;
        var treeIndex = selectedTreeItemIndex.Value.Append(selectedTableItemIndex.Value[0]);
        _treeSource.RowSelection?.Select(treeIndex);
    }
}