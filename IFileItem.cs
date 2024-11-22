namespace AFiley;

public interface IFileItem
{
    bool IsDirectory { get; }
    string Name { get; set; }
    string FullName { get; }
    DateTime CreationTime { get; set; }
}