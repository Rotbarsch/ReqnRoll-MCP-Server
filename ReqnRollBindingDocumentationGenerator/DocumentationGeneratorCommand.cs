using ReqnRollBindingMetadataExtractorService.Model;
using ReqnRollBindingMetadataExtractorService.Services;
using Spectre.Console.Cli;

namespace ReqnRollBindingDocumentationGenerator;

public class DocumentationGeneratorCommand : Command<DocumentationGeneratorSettings>
{
    public override int Execute(CommandContext context, DocumentationGeneratorSettings settings, CancellationToken cancellationToken)
    {
        var metaData = new List<BindingMetadata>();

        var inputs = settings.Input.Select(s => s.Split(";"))
            .Select(x=>new BindingAssemblyInput
            {
                Dll = x.First(),
                Xml = x.Skip(1).FirstOrDefault(),
            });

        foreach (var input in inputs)
        {
            using var metadataService = new BindingMetadataExtractorService(input.Dll, input.Xml);
            metaData.AddRange(metadataService.LoadMetadata());
        }

        var markdown = MarkdownGenerator.GenerateMarkdown(metaData);

        File.WriteAllText(settings.OutputFileName, markdown);
        return 0;
    }
}