using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using FileyCore.Interfaces;
namespace AFiley;


public class FileNameTemplate : IDataTemplate
{
    public FileNameTemplate()
    {
        AvaloniaXamlLoader.Load(this);
    }
    [Content] public Dictionary<string, IDataTemplate> Templates { get; set; } = [];
    public Control? Build(object? param)
    {
        if (param is not IFileyItem tagValue) return null;
        if (tagValue.IsDirectory) return Templates["Directory"].Build(param);
        return Templates["File"].Build(param);
    }

    public bool Match(object? data) => data is IFileyItem;
}