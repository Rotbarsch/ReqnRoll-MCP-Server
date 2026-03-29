using System.ComponentModel;
using Spectre.Console.Cli;

namespace ReqnRollBindingDocumentationGenerator;

public class DocumentationGeneratorSettings : CommandSettings
{
    [CommandOption("-i|--input",isRequired:true)]
    [Description("Pairs of DLL paths and their corresponding XML doc file. Multiple usages possible. Example: -i a.dll;a.xml -i b.dll;b.xml")]
    public string[] Input { get; set; }

    [CommandOption("-o|--output",isRequired:true)]
    [Description("Output file name for the generated markdown documentation.")]
    public string OutputFileName { get; set; }

}