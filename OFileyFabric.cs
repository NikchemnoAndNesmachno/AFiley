using System.Collections.ObjectModel;
using FileyCore.Interfaces;

namespace AFiley;

public class OFileyFabric: IFileyFabric<OFiley>
{
    public static string GetFileName(string path)
    {
        var segment = new Uri(path).Segments[^1];
        return Uri.UnescapeDataString(segment.Length == 1 ? segment : segment.TrimEnd('/'));
    }
        
    public OFiley CreateFile(string path, IFileyItem? parent = default) => new()
    {
        Parent = parent,
        Path = path,
        IsDirectory = false,
        Name = GetFileName(path),
        Files = []
    };

    public OFiley CreateDirectory(string path, IFileyItem? parent = default) => new()
    {
        Parent = parent,
        Path = path,
        Name = GetFileName(path),
        IsDirectory = true,
        Files = new ObservableCollection<IFileyItem>()
    };

}