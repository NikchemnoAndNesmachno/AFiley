using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
namespace AFiley;


public class FileSizeTemplate : IDataTemplate
{
    public FileSizeTemplate()
    {
        AvaloniaXamlLoader.Load(this);
    }
    [Content] public IDataTemplate SizeTemplate { get; set; }
    public Control? Build(object? param)
    {
        if (param is not FileItem file) return null;
        return SizeTemplate.Build(param);
    }

    public bool Match(object? data) => data is IFileItem;
}