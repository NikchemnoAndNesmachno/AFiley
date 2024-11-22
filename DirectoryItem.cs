using System.Collections.ObjectModel;
using CombinedCollections;

namespace AFiley;

public class DirectoryItem: IFileItem
{
    public DirectoryInfo DirectoryInfo { get; set; }
    public ObservableCollection<DirectoryItem> Directories { get; set; }
    public ObservableCollection<FileItem> Files { get; set; }
    public CombinedNotifyingList<IFileItem> AllItems { get; set; }

    public DirectoryItem(DirectoryInfo info)
    {
        DirectoryInfo = info;
        Directories = [];
        Files = [];
        AllItems = new CombinedNotifyingList<IFileItem>(Directories, Files);
        AllItems.SetForNotifying(Directories, Files);
        Name = info.Name;
        CreationTime = info.CreationTime;
    }
    public void ReadDirectories()
    {
        try
        {
            var dirs = DirectoryInfo.GetDirectories();
            Directories.Clear();
            foreach (var dir in dirs)
            {
                Directories.Add(new DirectoryItem(dir));
            }
            
        }
        catch (Exception e)
        {
            Directories.Clear();
        }
    }

    public void ReadFiles()
    {
        try
        {
            var files = DirectoryInfo.GetFiles();
            Files.Clear();
            foreach (var file in files)
            {
                Files.Add(new FileItem(file));
            }
        }
        catch (Exception e)
        {
            Files.Clear();
        }
    }

    public bool IsDirectory => true;

    public string Name { get; set; }

    public string FullName => DirectoryInfo.FullName;

    public DateTime CreationTime { get; set; }
}