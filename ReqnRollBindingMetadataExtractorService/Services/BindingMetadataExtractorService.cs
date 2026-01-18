using System.Reflection;
using Reqnroll;
using ReqnRollBindingMetadataExtractorService.Model;

namespace ReqnRollBindingMetadataExtractorService.Services;

public class BindingMetadataExtractorService
{
    private readonly string _dllPath;
    private readonly XmlDocumentationProvider _xmlDocumentationProvider;

    public BindingMetadataExtractorService(string dllPath, string? xmlPath = null)
    {
        _dllPath = dllPath;

        var defaultDocPath = Path.ChangeExtension(dllPath, "xml");

        _xmlDocumentationProvider = !string.IsNullOrEmpty(xmlPath) ? new XmlDocumentationProvider(xmlPath) : new XmlDocumentationProvider(defaultDocPath);
    }

    public List<BindingMetadata> Go()
    {
        var metadata = new List<BindingMetadata>();
        var assembly = Assembly.LoadFrom(_dllPath);

        AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
        {
            var name = new AssemblyName(eventArgs.Name).Name + ".dll";
            var candidate = Path.Combine(((AppDomain)sender!).BaseDirectory, name);
            return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
        };

        var relevantClasses = assembly.GetTypes().Where(x => x.GetCustomAttributes(true).Any(a => a is BindingAttribute)).ToList();

        foreach (var c in relevantClasses)
        {
            var methods = c.GetMethods().Where(m =>
                    m.GetCustomAttributes(true)
                        .Any(a => a is GivenAttribute or WhenAttribute or ThenAttribute))
                .ToList();

            foreach (var method in methods)
            {
                foreach (StepDefinitionBaseAttribute stepBindingAttribute in method.GetCustomAttributes(true)
                             .Where(a => a is GivenAttribute or WhenAttribute or ThenAttribute))
                {
                    metadata.Add(new BindingMetadata
                    {
                        Source = new BindingSourceMetadata
                        {
                            Assembly = assembly.FullName!.Split(",").First(),
                            ClassName = c.Name,
                            MethodName = method.Name
                        },
                        StepType = GetStepType(stepBindingAttribute),
                        Pattern = GetStepDefinitionAttributeValue(stepBindingAttribute),
                        PatternType = GetStepDefinitionExpressionTypes(stepBindingAttribute),
                        Description = GetStepDefinitionDescription(method),
                        Parameters = GetStepDefinitionParameters(method),
                    });
                }

            }
        }

        return metadata;
    }

    private static string GetStepType(StepDefinitionBaseAttribute stepBindingAttribute)
    {
        if (stepBindingAttribute is GivenAttribute) return "Given";
        if (stepBindingAttribute is WhenAttribute) return "When";
        if (stepBindingAttribute is ThenAttribute) return "Then";
        throw new NotImplementedException(
            $"{stepBindingAttribute.GetType().FullName} is not a valid StepDefinition Type.");
    }

    private static string GetStepDefinitionExpressionTypes(StepDefinitionBaseAttribute stepBindingAttribute)
    {
        return stepBindingAttribute.ExpressionType.ToString();
    }

    private static string GetStepDefinitionAttributeValue(StepDefinitionBaseAttribute stepBindingAttribute)
    {
        return stepBindingAttribute.Expression;
    }

    private string GetStepDefinitionDescription(MethodInfo method)
    {
        return _xmlDocumentationProvider.GetMethodComment(method);
    }

    private List<BindingSourceParameterInfo> GetStepDefinitionParameters(MethodInfo method)
    {
        var result = new List<BindingSourceParameterInfo>();
        foreach (var parameter in method.GetParameters())
        {
            result.Add(new BindingSourceParameterInfo
            {
                Name = parameter.Name!,
                Description = _xmlDocumentationProvider.GetParameterComment(method,parameter),
                ParameterType = parameter.ParameterType.Name
            });
        }

        return result;
    }
}