using System.Collections.ObjectModel;

namespace AFiley;

public class DirectoryItem(DirectoryInfo info)
{
    public DirectoryInfo DirectoryInfo { get; set; } = info;
    public ObservableCollection<DirectoryItem> Directories { get; set; } = [];

    public void Read()
    {
        try
        {
            var dirs = DirectoryInfo.GetDirectories();
            var dirItems = dirs.Select(x => new DirectoryItem(x));
            Directories = new ObservableCollection<DirectoryItem>(dirItems);
        }
        catch (Exception e)
        {
            Directories = [];
        }

        
    }
}