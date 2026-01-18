using ReqnRollBindingMetadataExtractorService.Model;
using System.Text;
using System.Xml.Linq;

namespace ReqnRollBindingDocumentationGenerator;

public static class MarkdownGenerator
{
    private static string ToMarkdownAnchor(string header)
    {
        // Lowercase, remove non-alphanumeric except spaces, replace spaces with hyphens
        var sb = new StringBuilder();
        foreach (var c in header.ToLower())
        {
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '-')
                sb.Append(c);
        }
        return sb.ToString().Trim().Replace(" ", "-");
    }

    private static string EscapeAndNormalize(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return EscapeMarkdown(text).Replace("\n", "<br>").Replace("\r", "");
    }

    public static string GenerateMarkdown(List<BindingMetadata> bindingInfos)
    {
        var sb = new StringBuilder();
        
        var byNamespace = bindingInfos.GroupBy(b => b.Source.Assembly).OrderBy(g => g.Key).ToList();
        sb.AppendLine(BuildTableOfContents(byNamespace));
        foreach (var nsGroup in byNamespace)
        {
            var nsHeader = $"Namespace: {nsGroup.Key}";
            var nsAnchor = ToMarkdownAnchor(nsHeader);
            sb.AppendLine($"<a id=\"{nsAnchor}\"></a>");
            sb.AppendLine($"# {nsHeader}");
            var byClass = nsGroup.GroupBy(b => b.Source.ClassName).OrderBy(g => g.Key);
            foreach (var classGroup in byClass)
            {
                var classHeader = $"Class: {classGroup.Key}";
                var classAnchor = ToMarkdownAnchor(classHeader);
                sb.AppendLine($"\n<a id=\"{classAnchor}\"></a>");
                sb.AppendLine($"## {classHeader}");
                sb.AppendLine();
                sb.AppendLine("| MethodName | BindingValue | Comments |");
                sb.AppendLine("|------------|--------------|----------|");
                foreach (var b in classGroup.OrderBy(x => x.Source.MethodName))
                {
                    var methodName = EscapeAndNormalize(b.Source.MethodName);
                    var bindingValue = EscapeAndNormalize($"{b.StepType} {b.Pattern}");
                    var comments = BuildCommentsString(b.Description, b.Parameters);
                    sb.AppendLine($"| {methodName} | {bindingValue} | {comments} |");
                }
                sb.AppendLine();
            }
        }
        return sb.ToString();
    }

    private static string BuildTableOfContents(List<IGrouping<string, BindingMetadata>> byNamespace)
    {
        var toc = new StringBuilder();
        toc.AppendLine("## Table of Contents");
        foreach (var nsGroup in byNamespace)
        {
            var nsHeader = $"Namespace: {nsGroup.Key}";
            var nsAnchor = ToMarkdownAnchor(nsHeader);
            toc.AppendLine($"- [{nsHeader}](#{nsAnchor})");
            var byClass = nsGroup.GroupBy(b => b.Source.ClassName).OrderBy(g => g.Key).ToList();
            foreach (var classGroup in byClass)
            {
                var classHeader = $"Class: {classGroup.Key}";
                var classAnchor = ToMarkdownAnchor(classHeader);
                toc.AppendLine($"  - [{classHeader}](#{classAnchor})");
            }
        }
        return toc.ToString();
    }

    /// <summary>
    /// Escapes the pipe (|), asterisk (*) and dollar ($) characters for safe rendering in markdown tables.
    /// - | is replaced with \\| to avoid breaking markdown tables.
    /// - * is replaced with \* to avoid emphasis parsing (e.g. (.*) stays literal).
    /// - $ is replaced with \\$ to avoid special interpretation in some markdown renderers.
    /// </summary>
    /// <param name="text">The string to escape for markdown table cell usage.</param>
    /// <returns>The escaped string, safe for markdown table cells.</returns>
    private static string EscapeMarkdown(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text
            .Replace("|", @"\\|")   // Escape pipe for markdown tables
            .Replace("*", @"\*")     // Escape asterisk to prevent emphasis
            .Replace("$", @"\\$");    // Escape dollar for markdown/regex safety
    }

    private static string BuildCommentsString(string comments, IEnumerable<BindingSourceParameterInfo> parameters)
    {
        var sb = new StringBuilder();
        bool hasContent = false;
        if (!string.IsNullOrWhiteSpace(comments))
        {
            sb.Append(EscapeAndNormalize(comments.Trim()));
            hasContent = true;
        }

        if (parameters.Any())
        {
            if (hasContent) sb.Append("<br>");
            // Render each parameter on its own line using <br> to avoid breaking markdown tables
            var paramLines = parameters.Select(p => $"*{EscapeAndNormalize(p.Name)}*: {EscapeAndNormalize(p.Description)}");
            sb.Append(string.Join("<br>", paramLines));
        }
        return sb.ToString().Trim();
    }
}