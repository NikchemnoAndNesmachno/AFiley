namespace AFiley;

public class FileItem(FileInfo fileInfo): IFileItem
{
    public FileInfo FileInfo { get; set; } = fileInfo;

    public bool IsDirectory => false;

    public string Name { get; set; } = fileInfo.Name;

    public string FullName => FileInfo.FullName;

    public DateTime CreationTime { get; set; } = fileInfo.CreationTime;
}