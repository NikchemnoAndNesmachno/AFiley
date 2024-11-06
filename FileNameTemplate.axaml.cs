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
    public Control? Build(object? param) => 
        param is IFileyItem { IsDirectory: false } ? 
            Templates["Directory"].Build(param) :
            null;

    public bool Match(object? data) => data is IFileyItem;
}