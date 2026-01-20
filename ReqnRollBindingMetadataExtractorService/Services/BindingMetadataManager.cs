using System.Text.Json;
using ReqnRollBindingMetadataExtractorService.Model;

namespace ReqnRollBindingMetadataExtractorService.Services;

public static class BindingMetadataManager
{
    private static BindingMetadata _metadata= new BindingMetadata()
    {
        StepDefinitions = new List<StepDefinitionMetadata>(),
        BindingClasses = new List<BindingClassMetadata>(),
    };

    public static void Initialize()
    {
        var assemblyLocation = typeof(BindingMetadataManager).Assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        
        var inputFileName = Path.Join(assemblyDirectory, "inputs.json");
        var inputJson = File.ReadAllText(inputFileName);

        var inputs = JsonSerializer.Deserialize<List<BindingAssemblyInput>>(inputJson);

        var result = new BindingMetadata()
        {
            StepDefinitions = new List<StepDefinitionMetadata>(),
            BindingClasses = new List<BindingClassMetadata>(),
        };

        foreach (var input in inputs!)
        {
            var service = new BindingMetadataExtractorService(input.Dll,input.Xml);
            var inputMetadata = service.LoadMetadata();
            result.StepDefinitions.AddRange(inputMetadata.StepDefinitions);
            result.BindingClasses.AddRange(inputMetadata.BindingClasses);
        }

        _metadata = result;
    }

    public static BindingMetadata GetAll()
    {
        return _metadata;
    }

    public static BindingMetadata GetBindingsInAssembly(string assemblyName)
    {
        return new BindingMetadata
        {
            StepDefinitions = _metadata.StepDefinitions.Where(x=>x.Source.Assembly==assemblyName).ToList(),
            BindingClasses = _metadata.BindingClasses.Where(x=>x.Assembly==assemblyName).ToList(),
        };
    }

    public static BindingMetadata GetBindingsByStepDefinitionType(string type)
    {
        var relevantSteps = _metadata.StepDefinitions.Where(x => x.StepType == type).ToList();
        return new BindingMetadata
        {
            StepDefinitions = relevantSteps.ToList(),
            BindingClasses = _metadata.BindingClasses
                .Where(x => relevantSteps.Any(step => step.Source.ClassFullName == x.ClassFullName))
                .ToList(),
        };
    }
}