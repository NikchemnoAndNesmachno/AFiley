using System.Collections.ObjectModel;
using FileyCore.Interfaces;

namespace AFiley;

public class OFiley: IFileyItem
{
    public IFileyItem? Parent { get; set; }
    public string Path { get; set; } = "";

    public string Name { get; set; } = "";
    public bool IsDirectory { get; set; }
    public IList<IFileyItem> Files { get; set; } = new ObservableCollection<IFileyItem>();
}