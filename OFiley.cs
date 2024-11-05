using System.Collections.ObjectModel;
using FileyCore.Interfaces;

namespace AFiley;

public class OFiley(FileSystemInfo fileSystemInfo): IFileyItem
{
    public FileSystemInfo FileSystemInfo { get; set; } = fileSystemInfo;
    public IFileyItem? Parent { get; set; }
    public string Path { get; set; } = "";

    public string Name { get; set; } = "";
    public bool IsDirectory { get; set; }
    public IList<IFileyItem> Files { get; set; } = new ObservableCollection<IFileyItem>();
    public long Size { get; set; }
    public DateTime DateModified { get; set; }
}