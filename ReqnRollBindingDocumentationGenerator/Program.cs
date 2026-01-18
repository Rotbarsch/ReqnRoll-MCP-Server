using System.Text.Json;
using ReqnRollBindingDocumentationGenerator;
using ReqnRollBindingMetadataExtractorService.Model;
using ReqnRollBindingMetadataExtractorService.Services;

if (!args.Any())
{
    throw new InvalidOperationException("Please set output path argument.");
}

var metaData = new List<BindingMetadata>();

var inputsJson = File.ReadAllText("inputs.json");
var inputs = JsonSerializer.Deserialize<List<BindingAssemblyInput>>(inputsJson)!;

foreach(var input in inputs)
{
    var metadataService = new BindingMetadataExtractorService(input.Dll, input.Xml);
    metaData.AddRange(metadataService.LoadMetadata());
}

var markdown = MarkdownGenerator.GenerateMarkdown(metaData);

File.WriteAllText(args[0],markdown);