namespace AtlasPacker;

internal sealed class AtlasInfo
{
    public required string Name { get; init; }
    public required string FilePath { get; init; }
    public required int CellWidth { get; set; }
    public required int CellHeight { get; set; }
}
