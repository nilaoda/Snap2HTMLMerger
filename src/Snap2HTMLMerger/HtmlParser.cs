using System.Text.RegularExpressions;

namespace Snap2HTMLMerger;

public partial class HtmlParser(string htmlPath)
{
    private readonly string _html = File.ReadAllText(htmlPath);
    private readonly string _fileName = Path.GetFileNameWithoutExtension(htmlPath);

    public SnapHtmlInfo Parse()
    {
        var title = TitleRegex().Match(_html).Groups[1].Value;
        var linkFiles = Convert.ToBoolean(LinkFilesRegex().Match(_html).Groups[1].Value);
        var linkProtocol = LinkProtocolRegex().Match(_html).Groups[1].Value;
        var linkRoot = LinkRootRegex().Match(_html).Groups[1].Value;
        var sourceRoot = SourceRootRegex().Match(_html).Groups[1].Value;
        var numberOfFiles = Convert.ToInt64(SumberOfFilesRegex().Match(_html).Groups[1].Value);
        
        SnapHtmlInfo result = new()
        {
            FileName = _fileName,
            Title = title,
            LinkFiles = linkFiles,
            LinkProtocol = linkProtocol,
            LinkRoot = linkRoot,
            SourceRoot = sourceRoot,
            NumberOfFiles = numberOfFiles,
        };

        var matches = SnapInfoRegex().Matches(_html);
        foreach (Match match in matches)
        {
            var value = match.Groups[1].Value;
            var array = ParseLines(value);

            var pathInfo = array.First().Split('*'); // 第1个
            var path = pathInfo.First();
            var modifiedDate = pathInfo.Last();
            var size = Convert.ToInt64(array.SkipLast(1).Last()); // 倒数第2个
            var indexes = array.Last().Split('*').Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToArray();
            
            SnapInfo snapInfo = new()
            {
                Path = path,
                ModifiedDate = modifiedDate,
                TotalSize = size,
                AssociatedSnapFileInfoIndexes = indexes,
            };

            var fileArray = array[1..^2];
            foreach (var file in fileArray)
            {
                var fileName = file.Split('*')[0];
                var fileSize = Convert.ToInt64(file.Split('*')[1]);
                var fileModifiedDate = file.Split('*')[2];
                snapInfo.Files.Add(new SnapFileInfo
                {
                    FileName = fileName,
                    Size = fileSize,
                    ModifiedDate = fileModifiedDate,
                });
            }
            result.SnapInfos.Add(snapInfo);
        }
        
        return result;
    }

    private List<string> ParseLines(string input)
    {
        // 按逗号分割，同时考虑转义字符和引号包裹的内容
        List<string> result = [];
        var inQuotes = false;
        var start = 0;
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            switch (c)
            {
                case '\"':
                    inQuotes = !inQuotes; // 切换引号状态
                    break;
                case ',' when !inQuotes:
                    result.Add(input.Substring(start, i - start).Trim().Trim('"'));
                    start = i + 1;
                    break;
            }
        }
        // 添加最后一个元素
        result.Add(input[start..].Trim().Trim('"'));
        return result;
    }

    [GeneratedRegex(@"<title>(.*)</title>")]
    private static partial Regex TitleRegex();
    [GeneratedRegex(@"var linkFiles = (\w+);")]
    private static partial Regex LinkFilesRegex();
    [GeneratedRegex(@"var linkProtocol = ""(.*)"";")]
    private static partial Regex LinkProtocolRegex();
    [GeneratedRegex(@"var linkRoot = ""(.*)"";")]
    private static partial Regex LinkRootRegex();
    [GeneratedRegex(@"var sourceRoot = ""(.*)"";")]
    private static partial Regex SourceRootRegex();
    [GeneratedRegex(@"var numberOfFiles = (\d+);")]
    private static partial Regex SumberOfFilesRegex();
    [GeneratedRegex(@"D.p\(\[(.*)\]\)")]
    private static partial Regex SnapInfoRegex();
}