using ReqnRollBindingMetadataExtractorService.Model;
using Mono.Cecil;
using Mono.Collections.Generic;
using CustomAttribute = Mono.Cecil.CustomAttribute;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace ReqnRollBindingMetadataExtractorService.Services;

public class BindingMetadataExtractorService : IDisposable
{
    private readonly XmlDocumentationProvider _xmlDocumentationProvider;
    private ModuleDefinition _module;
    private readonly string _dllPath;

    public BindingMetadataExtractorService(string dllPath, string? xmlPath = null)
    {
        var defaultDocPath = Path.ChangeExtension(dllPath, "xml");
        _dllPath = dllPath;

        _xmlDocumentationProvider = !string.IsNullOrEmpty(xmlPath) ? new XmlDocumentationProvider(xmlPath) : new XmlDocumentationProvider(defaultDocPath);
    }
    
    public List<BindingMetadata> LoadMetadata()
    {
        try
        {
            // Unmanaged dlls are not readable by Mono.Cecil and throw exceptions.
            // We catch this and return an empty list, as it means there are no bindings to extract.
            _module = ModuleDefinition.ReadModule(_dllPath);
        }
        catch (BadImageFormatException)
        {
            return new List<BindingMetadata>();
        }

        var metadata = new List<BindingMetadata>();
        string[] stepDefinitionAttributeNames = ["Reqnroll.GivenAttribute","Reqnroll.WhenAttribute", "Reqnroll.ThenAttribute"];

        foreach (var type in _module.Types.OrderBy(x=>x.Name))
        {
            if (!type.CustomAttributes.Select(x => x.AttributeType.FullName)
                .Any(x => x == "Reqnroll.BindingAttribute")) continue;

            var typeComment = GetTypeDescription(type.FullName);

            foreach (var method in type.Methods.OrderBy(x=>x.Name))
            {
                var stepDefinitionAttributes = method.CustomAttributes
                    .Where(x => stepDefinitionAttributeNames.Contains(x.AttributeType.FullName))
                    .ToList();

                if (!stepDefinitionAttributes.Any()) continue;

                foreach (var stepDefinitionAttribute in stepDefinitionAttributes)
                {
                    metadata.Add(new BindingMetadata
                    {
                        Source = new BindingSourceMetadata
                        {
                            Assembly = _module.Assembly.Name.Name,
                            ClassName = type.Name,
                            ClassFullName=type.FullName,
                            MethodName = method.Name,
                            ClassDescription = typeComment,
                        },
                        StepType = GetStepType(stepDefinitionAttribute),
                        Expression = GetExpressionProperty(stepDefinitionAttribute),
                        ExpressionType = GetExpressionTypeProperty(stepDefinitionAttribute),
                        Description = GetStepDefinitionDescription(type.FullName, method.Name, method.Parameters),
                        Parameters = GetStepDefinitionParameters(type.FullName,method.Name,method.Parameters),
                    });
                }
            }
        }

        return metadata;
    }

    private string GetTypeDescription(string typeName)
    {
        return _xmlDocumentationProvider.GetClassComment(typeName);
    }

    private string GetExpressionProperty(CustomAttribute stepDefinitionAttribute)
    {
        return stepDefinitionAttribute.ConstructorArguments.FirstOrDefault().Value.ToString() ?? "unknown";
    }

    private string GetExpressionTypeProperty(CustomAttribute stepDefinitionAttribute)
    {
        var prop = stepDefinitionAttribute.Properties
            .FirstOrDefault(p => p.Name == "ExpressionType");
        return prop.Argument.Value?.ToString()  ?? "Unspecified";
    }

    private static string GetStepType(CustomAttribute stepBindingAttribute)
    {
        if (stepBindingAttribute.AttributeType.FullName == "Reqnroll.GivenAttribute") return "Given";
        if (stepBindingAttribute.AttributeType.FullName == "Reqnroll.WhenAttribute") return "When";
        if (stepBindingAttribute.AttributeType.FullName == "Reqnroll.ThenAttribute") return "Then";
        throw new NotImplementedException(
            $"{stepBindingAttribute.GetType().FullName} is not a valid StepDefinition Type.");
    }

    private string GetStepDefinitionDescription(string ns, string methodName, Collection<ParameterDefinition> paramMap)
    {
        return _xmlDocumentationProvider.GetMethodComment(ns, methodName, paramMap.Select(x=>x.ParameterType.FullName));
    }

    private List<BindingSourceParameterInfo> GetStepDefinitionParameters(string ns, string methodName, Collection<ParameterDefinition> parameters)
    {
        var result = new List<BindingSourceParameterInfo>();
        foreach (var parameter in parameters)
        {
            result.Add(new BindingSourceParameterInfo
            {
                Name = parameter.Name,
                Description = _xmlDocumentationProvider.GetParameterComment(ns, methodName, parameters.Select(x=>x.ParameterType.FullName), parameter.Name),
                ParameterType = parameter.ParameterType.FullName
            });
        }

        return result;
    }

    public void Dispose()
    {
        _module?.Dispose();
    }
}