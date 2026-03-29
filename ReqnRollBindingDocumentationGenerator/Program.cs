using ReqnRollBindingDocumentationGenerator;
using Spectre.Console.Cli;

var app = new CommandApp<DocumentationGeneratorCommand>();
return app.Run(args);