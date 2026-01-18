// See https://aka.ms/new-console-template for more information

using ReqnRollBindingDocumentationGenerator;
using ReqnRollBindingMetadataExtractorService.Model;
using ReqnRollBindingMetadataExtractorService.Services;

Console.WriteLine("Hello, World!");


var metaData = new List<BindingMetadata>();

// TODO: Load inputs.json

var markdown = MarkdownGenerator.GenerateMarkdown(metaData);

Console.WriteLine(markdown);