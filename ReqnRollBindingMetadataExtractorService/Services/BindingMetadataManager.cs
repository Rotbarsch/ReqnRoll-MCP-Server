using System.Text.Json;
using ReqnRollBindingMetadataExtractorService.Model;

namespace ReqnRollBindingMetadataExtractorService.Services;

public static class BindingMetadataManager
{
    private static List<BindingMetadata> _metadata= new List<BindingMetadata>();

    public static void Initialize()
    {
        _metadata = new List<BindingMetadata>();

        var assemblyLocation = typeof(BindingMetadataManager).Assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        
        var inputFileName = Path.Join(assemblyDirectory, "inputs.json");
        var inputJson = File.ReadAllText(inputFileName);

        var inputs = JsonSerializer.Deserialize<List<BindingAssemblyInput>>(inputJson);

        foreach (var input in inputs!)
        {
            var service = new BindingMetadataExtractorService(input.Dll,input.Xml);
            var metadata = service.Go();
            _metadata.AddRange(metadata);
        }
    }

    public static List<BindingMetadata> GetAll()
    {
        return _metadata;
    }

    public static List<BindingMetadata> GetBindingsInAssembly(string assemblyName)
    {
        return _metadata.Where(x => x.Source.Assembly == assemblyName).ToList();
    }

    public static List<BindingMetadata> GetBindingsByStepDefinitionType(string type)
    {
        return _metadata.Where(x => x.StepType == type).ToList();
    }
}