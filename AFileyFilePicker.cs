using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace AFiley;

public class AFileyFilePicker : AFileyPickerBase
{
    protected override void ReadItems(DirectoryItem item)
    {
        item.ReadDirectories();
        item.ReadFiles();
    }

    protected override FlatTreeDataGridSource<IFileItem> CreateSource(DirectoryItem item) =>
        new(item.AllItems)
        {
            Columns =
            {
                new TemplateColumn<IFileItem>("Name",
                    cellTemplate: DataTemplates[0],
                    options: new TemplateColumnOptions<IFileItem> 
                    {
                        MaxWidth = new GridLength(400, GridUnitType.Pixel),
                        CompareAscending = Comparisons.FileItemNameCompare,
                        CompareDescending = Comparisons.FileItemNameCompareDesc
                    }),
                new TemplateColumn<IFileItem>("Size", new FileSizeTemplate(),
                    options: new TemplateColumnOptions<IFileItem>
                    {
                        CompareAscending = Comparisons.FileItemSizeCompare,
                        CompareDescending = Comparisons.FileItemSizeCompareDesc
                    }),
                new TextColumn<IFileItem,DateTime>("Creation date", x=>x.CreationTime)
            }
        };
    
}