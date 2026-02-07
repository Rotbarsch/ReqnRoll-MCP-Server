using System.Xml.Linq;
using System.Xml.XPath;

namespace ReqnRollBindingMetadataExtractorService.Services;

public class XmlDocumentationProvider
{
    private readonly XDocument? _xDoc;

    public XmlDocumentationProvider(string xmlPath)
    {
        if(File.Exists(xmlPath))
        {
            _xDoc = XDocument.Load(xmlPath);
        }
    }

    public string GetMethodComment(string ns, string methodName,
        IEnumerable<string> paramMapValues)
    {
        if (_xDoc is null) return string.Empty;

        var summary =
            _xDoc.XPathSelectElement($".//member[@name=\"{ConstructXPathMethodIdentifier(ns, methodName, paramMapValues)}\"]/summary")?.Value ??
            string.Empty;

        return summary.Trim();
    }

    public string GetParameterComment(string ns, string methodName, IEnumerable<string> parameterTypes, string parameterName)
    {
        if (_xDoc is null) return string.Empty;

        var paramComment = _xDoc.XPathSelectElement($".//member[@name=\"{ConstructXPathMethodIdentifier(ns, methodName, parameterTypes)}\"]/param[@name=\"{parameterName}\"]")?.Value ?? string.Empty;

        return paramComment.Trim();
    }

    public string GetClassComment(string typeName)
    {
        if (_xDoc is null) return string.Empty;

        var classComment= _xDoc.XPathSelectElement($".//member[@name=\"T:{typeName}\"]/summary")?.Value ?? string.Empty;
        return classComment.Trim();
    }

    private string ConstructXPathMethodIdentifier(string ns, string methodName, IEnumerable<string> parameterTypes)
    {
        //M:NatLaRestTest.Bindings.Actions.BasicVariableBindings.#ctor(NatLaRestTest.Logic.Interfaces.IBasicVariableLogic)

        string parametersString = string.Empty;
        var methodParams = parameterTypes.ToArray();
        if (methodParams.Any())
        {
            parametersString += "(";
            parametersString += string.Join(",", methodParams);
            parametersString += ")";
        }

        return $"M:{ns}.{methodName}{parametersString}";
    }
}