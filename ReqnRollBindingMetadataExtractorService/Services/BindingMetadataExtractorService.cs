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

    public BindingMetadata LoadMetadata()
    {
        var result = new BindingMetadata
        {
            BindingClasses = new List<BindingClassMetadata>(),
            StepDefinitions = new List<StepDefinitionMetadata>(),
        };

        var stepDefinitionsMetadata = new List<StepDefinitionMetadata>();
        var assembly = Assembly.LoadFrom(_dllPath);

        AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
        {
            var name = new AssemblyName(eventArgs.Name).Name + ".dll";
            var dllDir = Path.GetDirectoryName(_dllPath)!;

            // Probe next to the target DLL first
            var candidate = Path.Combine(dllDir, name);
            if (File.Exists(candidate)) return Assembly.LoadFrom(candidate);

            // Fallback to the app base directory
            candidate = Path.Combine(AppContext.BaseDirectory, name);
            return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
        };

        var bindingAttr = typeof(BindingAttribute);
        var typesInAssembly = assembly.GetTypes();
        foreach (var type in typesInAssembly)
        {
            try { if (!type.GetCustomAttributes(bindingAttr, inherit: true).Any()) continue; } catch { continue; }

            if (type.GetCustomAttribute<BindingAttribute>() is null) continue;

            MethodInfo[] methods;
            try { methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic); } catch { continue; }

            result.BindingClasses.Add(GetBindingClassMetadata(type));

            foreach (var method in methods)
            {
                object[] attrs;
                try { attrs = method.GetCustomAttributes(typeof(StepDefinitionBaseAttribute), true); } catch { continue; }

                foreach (StepDefinitionBaseAttribute stepBindingAttribute in attrs)
                {
                    stepDefinitionsMetadata.Add(new StepDefinitionMetadata
                    {
                        Source = new StepDefinitionSourceMetadata
                        {
                            Assembly = assembly.FullName!.Split(",").First(),
                            ClassName = type.Name,
                            ClassFullName = type.FullName!,
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

        result.StepDefinitions = stepDefinitionsMetadata;

        return result;
    }

    private BindingClassMetadata GetBindingClassMetadata(Type type)
    {
        return new BindingClassMetadata
        {
            Assembly = type.Assembly.FullName?.Split(",")?.First() ?? "unknown",
            ClassName = type.Name ?? "unknown",
            ClassFullName = type.FullName ?? "unknown",
            Description = _xmlDocumentationProvider.GetClassComment(type),
        };
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

    private List<StepDefinitionParameterInfo> GetStepDefinitionParameters(MethodInfo method)
    {
        var result = new List<StepDefinitionParameterInfo>();
        foreach (var parameter in method.GetParameters())
        {
            result.Add(new StepDefinitionParameterInfo
            {
                Name = parameter.Name!,
                Description = _xmlDocumentationProvider.GetParameterComment(method, parameter),
                ParameterType = parameter.ParameterType.Name
            });
        }

        return result;
    }
}