using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AFiley;

public class AFileyDirectoryPicker : AFileyPickerBase
{
    protected override void ReadItems(DirectoryItem item)
    {
        item.ReadDirectories();
    }

    protected override FlatTreeDataGridSource<IFileItem> CreateSource(DirectoryItem item) =>
        new(item.Directories)
        {
            Columns =
            {
                new TemplateColumn<IFileItem>("Name",
                    cellTemplate: DataTemplates[0],
                    options: new TemplateColumnOptions<IFileItem> 
                    {
                        MaxWidth = new GridLength(400, GridUnitType.Pixel),
                        CompareAscending = Comparisons.SameFileItemNameCompare,
                        CompareDescending = Comparisons.SameFileItemNameCompareDesc
                    }),
                new TextColumn<IFileItem,DateTime>("Creation date", x=>x.CreationTime)
            }
        };
}