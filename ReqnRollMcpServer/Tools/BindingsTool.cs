using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using ReqnRollBindingMetadataExtractorService.Services;

namespace ReqnRollMcpServer.Tools;

[McpServerToolType]
public static class BindingsTool
{
    [McpServerTool, Description("Returns information about all ReqnRoll bindings available.")]
    public static string GetAvailableBindings()
    {
        return JsonSerializer.Serialize(BindingMetadataManager.GetAll());
    }

    [McpServerTool, Description("Returns information about ReqnRoll bindings in a specific assembly.")]
    public static string GetBindingsFromAssembly(string assemblyName)
    {
        return JsonSerializer.Serialize(BindingMetadataManager.GetBindingsInAssembly(assemblyName));
    }

    [McpServerTool, Description("Returns information about ReqnRoll bindings of a specific StepDefinition Type (Given, When or Then).")]
    public static string GetBindingsOfStepDefinitionType(string type)
    {
        return JsonSerializer.Serialize(BindingMetadataManager.GetBindingsByStepDefinitionType(type));
    }
}