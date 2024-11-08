using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;

namespace AFiley;

public static class Converters
{
    public static FuncValueConverter<long?, string> SizeConverter { get; } = new(size => 
        size switch
    {
        < Sizes.Kb => $"{size} B",
        < Sizes.Mb => $"{size / Sizes.Kb} KB",
        < Sizes.Gb => $"{size / Sizes.Mb} MB",
        < Sizes.Tb => $"{size / Sizes.Gb} GB",
        _ => ""
    });
    
    public static FuncValueConverter<TreeDataGridRow?, bool> IsDirectoryDataContextConverter { get; } = 
        new(textBlock => textBlock?.DataContext is DirectoryItem);
    
    public static FuncValueConverter<TextBlock?, bool> IsFileDataContextConverter { get; } = 
        new(textBlock => textBlock?.DataContext is FileItem);
}