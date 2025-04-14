using System.Text.RegularExpressions;

namespace Snap2HTMLMerger;

public static partial class Util
{
    public static string ConvertDateTime(string text)
    {
        if (!TimestampRegex().IsMatch(text)) return text;
        
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(text));
        var localDateTime = dateTimeOffset.DateTime.ToLocalTime();
        return $"{localDateTime:yyyy/M/d} {localDateTime:HH:mm:ss}";
    }

    [GeneratedRegex(@"^\d{10}$")]
    private static partial Regex TimestampRegex();
}