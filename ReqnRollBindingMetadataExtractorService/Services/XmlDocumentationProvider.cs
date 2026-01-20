using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReqnRollBindingMetadataExtractorService.Services;

public class XmlDocumentationProvider
{
    private readonly XDocument _xDoc;

    public XmlDocumentationProvider(string xmlPath)
    {
        _xDoc = XDocument.Load(xmlPath);
    }

    public string GetMethodComment(MethodInfo method)
    {
        var summary =
            _xDoc.XPathSelectElement($".//member[@name=\"{ConstructXPathMethodIdentifier(method)}\"]/summary")?.Value ??
            string.Empty;

        return summary.Trim();
    }

    private string ConstructXPathMethodIdentifier(MethodInfo method)
    {
        string parametersString = string.Empty;
        var methodParams = method.GetParameters();
        if (methodParams.Any())
        {
            parametersString += "(";
            parametersString += string.Join(",", methodParams.Select(x => x.ParameterType.FullName));
            parametersString += ")";
        }

        return $"M:{method.DeclaringType!.FullName}.{method.Name}{parametersString}";
    }

    public string GetParameterComment(MethodInfo method, ParameterInfo parameter)
    {
        var paramComment = _xDoc.XPathSelectElement($".//member[@name=\"{ConstructXPathMethodIdentifier(method)}\"]/param[@name=\"{parameter.Name}\"]")?.Value ?? string.Empty;

        return paramComment.Trim();
    }

    public string GetClassComment(Type type)
    {
        return (_xDoc.XPathSelectElement($".//member[@name=\"T:{type.FullName}\"]/summary")?.Value ?? string.Empty)
            .Trim();
    }
}