using Snap2HTMLMerger;

if (args.Length < 1)
{
    Console.WriteLine("Usage: Snap2HTMLMerger template.html file1.html [file2.html ...]");
    return;
}

var templateHtml = args[0];

// Ensure the template file exists
if (!File.Exists(templateHtml))
{
    Console.WriteLine($"Error: Template file '{templateHtml}' not found.");
    return;
}

var htmlFiles = args[1..];
foreach (var htmlFile in htmlFiles)
{
    if (!File.Exists(htmlFile))
    {
        Console.WriteLine($"Error: File '{htmlFile}' not found.");
        return;
    }
}

Console.WriteLine($"Starting Merge with {htmlFiles.Length} files.");
var templateHtmlContent = File.ReadAllText(templateHtml);
HtmlWriter htmlWriter = new(templateHtmlContent);
foreach (var htmlFile in htmlFiles)
{
    Console.WriteLine($"Parsing {Path.GetFileName(htmlFile)}");
    var parser = new HtmlParser(htmlFile);
    var data = parser.Parse();
    htmlWriter.AddHtml(data);
}
var outputFileName = $"merged_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.html";
htmlWriter.WriteToFile(outputFileName);
Console.WriteLine($"Merged HTML files successfully. Output saved to '{Path.GetFullPath(outputFileName)}'.");
