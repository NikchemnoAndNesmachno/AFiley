namespace AFiley;

public class FileItem(FileInfo fileInfo): IFileItem
{
    public FileInfo FileInfo { get; set; } = fileInfo;
    public string Name { get; set; } = fileInfo.Name;
    public DateTime CreationTime { get; set; } = fileInfo.CreationTime;
}