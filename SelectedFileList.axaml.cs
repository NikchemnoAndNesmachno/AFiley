using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AFiley;

public partial class SelectedFileList : UserControl
{
    public static readonly StyledProperty<IList<string>> SelectedFilesProperty = 
        AvaloniaProperty.Register<SelectedFileList, IList<string>>(nameof(SelectedFiles));

    public IList<string> SelectedFiles
    {
        get => GetValue(SelectedFilesProperty);
        set => SetValue(SelectedFilesProperty, value);
    }
    public SelectedFileList()
    {
        InitializeComponent();
        SelectedFiles = new ObservableCollection<string>();
    }

    public void Remove(object? obj)
    {
        SelectedFiles.Remove(obj?.ToString()??"");
    }
}