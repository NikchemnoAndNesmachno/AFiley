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
    
    public OFiley CreateFile(FileInfo fileInfo, IFileyItem parent)=>
        new(fileInfo)
        {
            FileSystemInfo = fileInfo,
            Parent = parent,
            Path = fileInfo.FullName,
            IsDirectory = false,
            Name = GetFileName(fileInfo.FullName),
            Files = [],
            Size = fileInfo.Length,
            DateModified = fileInfo.LastWriteTime
        };

    public OFiley CreateDirectory(DirectoryInfo dirInfo, IFileyItem parent)=>
        new(dirInfo)
        {
            Parent = parent,
            Path = dirInfo.FullName,
            IsDirectory = true,
            Name = GetFileName(dirInfo.FullName),
            Files = new ObservableCollection<IFileyItem>(),
            DateModified = dirInfo.LastWriteTime
        };

    public OFiley CreateDrive(DriveInfo dirInfo)=>
        new(dirInfo.RootDirectory)
        {
            Parent = null,
            Path = dirInfo.RootDirectory.FullName,
            IsDirectory = true,
            Name = GetFileName(dirInfo.RootDirectory.FullName),
            Files = new ObservableCollection<IFileyItem>(),
        };
}