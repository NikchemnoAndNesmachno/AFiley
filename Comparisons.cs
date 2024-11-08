namespace AFiley;


public static class Comparisons
{
    public static readonly Comparison<IFileItem?> DirToFileCompare = (x, y) => 
        x is DirectoryItem ? 
            y is DirectoryItem ? 
                0 : 
                1 : 
            y is DirectoryItem ? 
                -1 :
                0;

    public static readonly Comparison<IFileItem?> DirToFileCompareDesc = (x, y) => 
        -DirToFileCompare(x, y);

    public static readonly Comparison<FileItem> FileSizeCompare = (x, y) =>
    {
        var (lenX, lenY) = (x.FileInfo.Length, y.FileInfo.Length);
        if (lenX > lenY) return 1;
        if (lenY > lenX) return -1;
        return 0;
    };
    
    public static readonly Comparison<IFileItem?> SameFileItemNameCompare = (x, y) =>
        string.CompareOrdinal(x?.Name, y?.Name);

    public static readonly Comparison<IFileItem?> FileItemSizeCompare = (x, y) => 
        x is FileItem xInfo && y is FileItem yInfo ?
            FileSizeCompare(xInfo, yInfo) : 0;
    
    public static readonly Comparison<IFileItem?> FileItemSizeCompareDesc = (x, y) =>
        FileItemSizeCompare(y, x);
    
    public static readonly Comparison<IFileItem?> FileItemNameCompare = (x, y) =>
    {
        if (x is DirectoryItem xDir)
        {
            if (y is DirectoryItem yDir) return SameFileItemNameCompare(xDir, yDir);
            return 1;
        }
        if (y is DirectoryItem) return -1;

        if (x is FileItem xFile && y is FileItem yFile) return SameFileItemNameCompare(xFile, yFile);
        return 0;
    };

    public static readonly Comparison<IFileItem?> FileItemNameCompareDesc = (x, y) =>
        FileItemNameCompare(y, x);
}