namespace Snap2HTMLMerger;

public class SnapHtmlInfo
{
    public required string FileName { get; set; }
    public required string Title { get; set; }
    public required long NumberOfFiles { get; set; }
    public required bool LinkFiles { get; set; }
    public required string LinkProtocol { get; set; }
    public required string LinkRoot { get; set; }
    public required string SourceRoot { get; set; }
    public long TotalSize => SnapInfos.Sum(x => x.TotalSize);
    public List<SnapInfo> SnapInfos { get; set; } = [];
}

public class SnapInfo
{
    public required string Path { get; set; }
    public required string ModifiedDate { get; set; }
    public List<SnapFileInfo> Files { get; set; } = [];
    public required long TotalSize { get; set; }
    public required int[] AssociatedSnapFileInfoIndexes { get; set; }
    
    public override string ToString()
    {
        var pathInfo = $"{Path}*0*{ModifiedDate}";
        var filesInfo = Files.Select(x => x.ToString());
        List<object> array =
            [pathInfo, ..filesInfo, TotalSize, string.Join('*', AssociatedSnapFileInfoIndexes)]; 
        return string.Join(',', array.Select(x => x is string ? $"\"{x}\"" : x.ToString()));
    }
}

public class SnapFileInfo
{
    public required string FileName { get; set; }
    public required long Size { get; set; }
    public required string ModifiedDate { get; set; }

    public override string ToString()
    {
        return $"{FileName}*{Size}*{ModifiedDate}";
    }
}