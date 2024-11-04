using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using FileyCore;
using FileyCore.Interfaces;

namespace AFiley;
public class AFiley : TemplatedControl
{
    private TreeDataGrid? _tree;
    private HierarchicalTreeDataGridSource<IFileyItem> _fileSource;

    public static readonly DirectProperty<AFiley, HierarchicalTreeDataGridSource<IFileyItem>> FileSourceProperty =
        AvaloniaProperty.RegisterDirect<AFiley, HierarchicalTreeDataGridSource<IFileyItem>>(nameof(FileSource),
            o => o.FileSource, 
            (o, v) => o.FileSource = v);

    public HierarchicalTreeDataGridSource<IFileyItem> FileSource
    {
        get => _fileSource;
        set => SetAndRaise(FileSourceProperty, ref _fileSource, value);
    }

    public static readonly StyledProperty<IFileyItem> SelectedItemProperty =
        AvaloniaProperty.Register<AFiley, IFileyItem>(nameof(SelectedItem));

    public static readonly StyledProperty<string> SelectedIndexProperty =
        AvaloniaProperty.Register<AFiley, string>(nameof(SelectedIndex));

    public string SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public IFileyItem SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public void InitFileSource(IEnumerable<IFileyItem> items)
    {
        FileSource = new HierarchicalTreeDataGridSource<IFileyItem>(items)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<IFileyItem>(
                    new TemplateColumn<IFileyItem>("Name", new FileDataTemplate())
                    {
                        Options  =
                        {
                            IsTextSearchEnabled = true,
                            CompareAscending = (x, y) => string.CompareOrdinal(x?.Name, y?.Name),
                            CompareDescending = (x, y)=>string.CompareOrdinal(y?.Name, x?.Name)
                        }
                    },
                    x=>x.Files),
                new TextColumn<IFileyItem,bool>("IsDirectory", x=>x.IsDirectory)
            }
        };
        FileSource.RowSelection.SelectionChanged += OnSelectionChanged;
    }
    public AFiley()
    {
        InitFileSource([]);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _tree = e.NameScope.Find<TreeDataGrid>("PART_tree");
        if (_tree is not null)
        {
            _tree.DoubleTapped += OnDoubleTapped;
        }
        base.OnApplyTemplate(e);
    }

    private void OnSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs e)
    {
        if(e.SelectedIndexes.Count == 0) return;
        var item = e.SelectedItems[0];
        var index = e.SelectedIndexes[0];
        if (item is not IFileyItem selectedFile) return;
        SelectedItem = selectedFile;
        SelectedIndex = string.Join(" ", index.ToArray());
        CurrentPath = SelectedItem.Path;
    }

    public void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var item = FileSource.RowSelection?.SelectedItem;
        if(item is null) return;
        try
        {
            FileMethods.LoadFull(item);
        }
        catch (UnauthorizedAccessException exception)
        {
            Console.WriteLine(exception);
        }
        
    }

    public FileMethod FileMethods { get; set; } = new()
    {
        FileyFabric = new OFileyFabric()
    };
    public static readonly StyledProperty<string> CurrentPathProperty =
        AvaloniaProperty.Register<AFiley, string>(nameof(CurrentPath));

    public string CurrentPath
    {
        get => GetValue(CurrentPathProperty);
        set => SetValue(CurrentPathProperty, value);
    }
    
    public void Load()
    {
        InitFileSource(new ObservableCollection<IFileyItem>(FileMethods.GetDrives()));
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
    
    public void Browse()
    {
        if(!Path.Exists(CurrentPath)) return;
        var segments = new Uri(CurrentPath).Segments;
        var indexes = new IndexPath();
        var collectionToFind = FileSource.Items.ToArray();
        for (int i = 0; i < segments.Length; i++)
        {
            var name = Uri.UnescapeDataString(i == 0 ? segments[i] : segments[i].TrimEnd('/'));
            var (index, item) = IndexOf(collectionToFind, name);
            if (index == -1 || item is null) return;
            indexes = indexes.Append(index);
            FileMethods.LoadFull(item);
            collectionToFind = item.Files.ToArray();
        }
        FileSource.RowSelection?.Select(indexes);
        FileSource.Expand(indexes);

    }
    
}