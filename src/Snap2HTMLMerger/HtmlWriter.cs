using System.Text;

namespace Snap2HTMLMerger;

public class HtmlWriter(string templateHtml)
{
    private const string Root = "Z:/";

    private long _allFilesCount = 0;
    private long _allDirCount = 0;
    private long _totalSize = 0;
    private readonly List<SnapInfo> _allSnapInfos = [];
    private int _indexOffset = 1;
    private List<int> _snapHtmlIndexes = [];

    public void AddHtml(SnapHtmlInfo htmlInfo)
    {
        _totalSize += htmlInfo.TotalSize;
        _allDirCount += htmlInfo.SnapInfos.Count;
        _allFilesCount += htmlInfo.SnapInfos.Select(x => x.Files.Count).Sum();
        
        var snapInfos = htmlInfo.SnapInfos;
        if (snapInfos.Count > 0)
        {
            _snapHtmlIndexes.Add(_indexOffset);
        }
        foreach (var snapInfo in snapInfos)
        {
            var path = snapInfo.Path == htmlInfo.SourceRoot
                ? snapInfo.Path.Replace(htmlInfo.SourceRoot, $"{Root}{htmlInfo.FileName}")
                : snapInfo.Path.Replace(htmlInfo.SourceRoot, $"{Root}{htmlInfo.FileName}/");
            var indexes = new int[snapInfo.AssociatedSnapFileInfoIndexes.Length];
            for (var i = 0; i < snapInfo.AssociatedSnapFileInfoIndexes.Length; i++)
            {
                indexes[i] = snapInfo.AssociatedSnapFileInfoIndexes[i] + _indexOffset;
            }
            _allSnapInfos.Add(new SnapInfo
            {
                Path = path,
                ModifiedDate = snapInfo.ModifiedDate,
                Files = snapInfo.Files,
                TotalSize = snapInfo.TotalSize,
                AssociatedSnapFileInfoIndexes = indexes,
            });
        }

        _indexOffset += htmlInfo.SnapInfos.Count;
    }

    public void WriteToFile(string outputPath, string? title = null)
    {
        if (_allSnapInfos.Count == 0)
            throw new Exception("No snap info available");
        
        var htmlTitle = title ?? "MergedSnaps";
        var allSnapInfoLines = _allSnapInfos.Select(x => $"D.p([{x}])").ToList();
        // 补一个新的文件夹 管理每个盘符
        var topModifiedDate = _allSnapInfos.Select(x => x.ModifiedDate).Max();
        var topIndexes = string.Join('*', _snapHtmlIndexes);
        allSnapInfoLines.Insert(0, $"D.p([\"{Root}*0*{topModifiedDate}\",0,\"{topIndexes}\"])");
        var dirData = string.Join(Environment.NewLine, allSnapInfoLines);
        var html = templateHtml
            .Replace("[APP LINK]", "")
            .Replace("[APP NAME]", "")
            .Replace("[APP VER]", "")
            .Replace("[TITLE]", htmlTitle)
            .Replace("[GEN DATE]", DateTime.Now.ToString("yyyy/M/d"))
            .Replace("[GEN TIME]", DateTime.Now.ToString("t"))
            .Replace("[SOURCE ROOT]", Root)
            .Replace("[TOT SIZE]", _totalSize.ToString())
            .Replace("[NUM FILES]", _allFilesCount.ToString())
            .Replace("[NUM DIRS]", _allDirCount.ToString())
            .Replace("[LINK ROOT]", "")
            .Replace("[LINK FILES]", "false")
            .Replace("[LINK PROTOCOL]", "")
            .Replace("[DIR DATA]", dirData);
        File.WriteAllText(outputPath, html, new UTF8Encoding(false));
    }
}