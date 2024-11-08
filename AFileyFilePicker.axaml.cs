using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AFiley;

public partial class AFileyFilePicker : UserControl
{
    private FlatTreeDataGridSource<IFileItem> _flatSource;
    private HierarchicalTreeDataGridSource<DirectoryItem> _treeSource;
    
    public IList<string> SelectedFiles
    {
        get => SelectedFileList.SelectedFiles;
        set => SelectedFileList.SelectedFiles = value;
    }
    
    public AFileyFilePicker()
    {
        InitializeComponent();
    }

    private void FileTable_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedTableItem = _flatSource.RowSelection?.SelectedItem;
        if (selectedTableItem is FileItem) return;
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
    
    private void InitTableSource(IEnumerable<IFileItem> items)
    {
        
        _flatSource = new FlatTreeDataGridSource<IFileItem>(items)
        {
            Columns =
            {
                new TextColumn<IFileItem,string>("Name", x =>x.Name,
                    options: new TextColumnOptions<IFileItem>
                {
                    CompareAscending = Comparisons.FileItemNameCompare,
                    CompareDescending = Comparisons.FileItemNameCompareDesc
                }),
                new TemplateColumn<IFileItem>("Size", new FileSizeTemplate(),
                    options: new TemplateColumnOptions<IFileItem>
                    {
                        CompareAscending = Comparisons.FileItemSizeCompare,
                        CompareDescending = Comparisons.FileItemSizeCompareDesc
                    }),
                new TextColumn<IFileItem,DateTime>("Creation date", x=>x.CreationTime)
            }
        };
        _flatSource.RowSelection.SelectionChanged += OnFlatSelectionChanged;
        FileTable.Source = _flatSource;
    }

    private void OnFlatSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<IFileItem> e)
    {
        if(e.SelectedItems.Count == 0) return;
        var item = e.SelectedItems[0];
        CurrentPathBox.Text = item switch
        {
            FileItem file => file.FileInfo.FullName,
            DirectoryItem dir => dir.DirectoryInfo.FullName,
            _ => CurrentPathBox.Text
        };
    }

    private void OnTreeSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<DirectoryItem> e)
    {
        if(e.SelectedIndexes.Count == 0) return;
        
        var item = e.SelectedItems[0];
        if(item is null) return;
        CurrentPathBox.Text = item.DirectoryInfo.FullName;
        if (item.Directories.Count == 0)
        {
            item.ReadDirectories();
            item.ReadFiles();
        }

        InitTableSource(item.AllItems);
        
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
        SelectedFiles.Add(CurrentPathBox.Text);
    }

    private void FileTree_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedTreeItem = _treeSource.RowSelection?.SelectedItem;
        var selectedTreeItemIndex = _treeSource.RowSelection?.SelectedIndex;
        if (selectedTreeItem is null || selectedTreeItemIndex is null) return;
        selectedTreeItem.ReadDirectories();
    }
}