using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FileyCore.Interfaces;

namespace AFiley;

public partial class AFileyDirectoryPicker : UserControl
{
    private FlatTreeDataGridSource<DirectoryItem> _flatSource;
    private HierarchicalTreeDataGridSource<DirectoryItem> _treeSource;
    
    public IList<string> SelectedDirectories
    {
        get => SelectedFileList.SelectedFiles;
        set => SelectedFileList.SelectedFiles = value;
    }
    
    public AFileyDirectoryPicker()
    {
        InitializeComponent();
    }

    private void FileTable_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedTableItem = _flatSource.RowSelection?.SelectedItem;
        if (selectedTableItem is null) return;
        var selectedTableItemIndex = _flatSource.RowSelection?.SelectedIndex;
        var selectedTreeItemIndex = _treeSource.RowSelection?.SelectedIndex;
        if(selectedTableItemIndex is null || selectedTreeItemIndex is null) return;
        var treeIndex = selectedTreeItemIndex.Value.Append(selectedTableItemIndex.Value[0]);
        _treeSource.RowSelection?.Select(treeIndex);
    }

    private void LoadDrives(object? sender, RoutedEventArgs e)
    {
        var drives = DriveInfo.GetDrives();
        var items = drives.Select(x => new DirectoryItem(x.RootDirectory));
        InitTreeSource(new ObservableCollection<DirectoryItem>(items));
    }
    
    private void InitTreeSource(IEnumerable<DirectoryItem> items)
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
    
    private void InitTableSource(IEnumerable<DirectoryItem> items)
    {
        
        _flatSource = new FlatTreeDataGridSource<DirectoryItem>(items)
        {
            Columns =
            {
                new TextColumn<DirectoryItem,string>("Name", x=> x.DirectoryInfo.Name),
                new TextColumn<DirectoryItem,DateTime>("Creation date", x=>x.DirectoryInfo.CreationTime)
            }
        };
        _flatSource.RowSelection.SelectionChanged += OnFlatSelectionChanged;
        FileTable.Source = _flatSource;
    }

    private void OnFlatSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<DirectoryItem> e)
    {
        if(e.SelectedItems.Count == 0) return;
        var item = e.SelectedItems[0];
        if(item is null) return;
        CurrentPathBox.Text = item.DirectoryInfo.FullName;
    }

    private void OnTreeSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<DirectoryItem> e)
    {
        if(e.SelectedIndexes.Count == 0) return;
        
        var item = e.SelectedItems[0];
        if(item is null) return;
        CurrentPathBox.Text = item.DirectoryInfo.FullName;
        if (item.Directories.Count == 0)
        {
            item.Read();
        }

        InitTableSource(item.Directories);
        
        var index = e.SelectedIndexes[0];
        var rowIndex = FileTree.Rows.ModelIndexToRowIndex(index);
        FileTree.RowsPresenter?.BringIntoView(rowIndex);
        _treeSource.Expand(index);
    }
    
    private (int, DirectoryItem?) IndexOf(IEnumerable<DirectoryItem> items, string name)
    {
        var k = -1;
        foreach (var filey in items)
        {
            k++;
            if (filey.DirectoryInfo.Name == name) return (k, filey);
        }
        return (-1, null);
    }

    private void GoTo(object? sender, RoutedEventArgs e)
    {
        if(!Path.Exists(CurrentPathBox.Text)) return;
        var segments = new Uri(CurrentPathBox.Text).Segments;
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
    

    private void AddToSelected(object? sender, RoutedEventArgs e)
    {
        if(string.IsNullOrEmpty(CurrentPathBox.Text)) return;
        SelectedDirectories.Add(CurrentPathBox.Text);
    }

    private void FileTree_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedTreeItem = _treeSource.RowSelection?.SelectedItem;
        var selectedTreeItemIndex = _treeSource.RowSelection?.SelectedIndex;
        if (selectedTreeItem is null || selectedTreeItemIndex is null) return;
        selectedTreeItem.Read();
    }
}