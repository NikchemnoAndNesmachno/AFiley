using Avalonia.Data.Converters;

namespace AFiley;

public static class Converters
{
    public static FuncValueConverter<long?, string> SizeConverter { get; } = new(size =>
    {
        return size switch
        {
            < Sizes.Kb => $"{size} B",
            < Sizes.Mb => $"{size / Sizes.Kb} KB",
            < Sizes.Gb => $"{size / Sizes.Mb} MB",
            < Sizes.Tb => $"{size / Sizes.Gb} GB",
            _ => ""
        };
    });

}