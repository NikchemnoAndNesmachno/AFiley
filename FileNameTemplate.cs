using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace AFiley;

public class FileNameTemplate: IDataTemplate
{
    [Content] public DataTemplates Templates { get; set; } = [];
    public Control? Build(object? param)
    {
        if (param is FileItem) return Templates[0].Build(param);
        if (param is DirectoryItem) return Templates[1].Build(param);
        return null;
    }

    public bool Match(object? data) => data is IFileItem;
}