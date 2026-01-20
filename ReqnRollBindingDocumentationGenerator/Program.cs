using System.Text.Json;
using ReqnRollBindingDocumentationGenerator;
using ReqnRollBindingMetadataExtractorService.Model;
using ReqnRollBindingMetadataExtractorService.Services;

if (!args.Any())
{
    throw new InvalidOperationException("Please set output path argument.");
}

var inputsJson = File.ReadAllText("inputs.json");
var inputs = JsonSerializer.Deserialize<List<BindingAssemblyInput>>(inputsJson)!;

var result = new BindingMetadata()
{
    StepDefinitions = new List<StepDefinitionMetadata>(),
    BindingClasses = new List<BindingClassMetadata>(),
};

foreach(var input in inputs)
{
    var metadataService = new BindingMetadataExtractorService(input.Dll, input.Xml);
    var inputMetadata = metadataService.LoadMetadata();
    result.StepDefinitions.AddRange(inputMetadata.StepDefinitions);
    result.BindingClasses.AddRange(inputMetadata.BindingClasses);
}

var markdown = MarkdownGenerator.GenerateMarkdown(result);

File.WriteAllText(args[0],markdown);