using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ModelContextProtocol.Server;
using ReqnRollBindingMetadataExtractorService.Services;

namespace ReqnRollMcpServer.Tools;

[McpServerToolType]
public static class BindingsTool
{
    [McpServerTool, Description("Returns information about all ReqnRoll bindings available. Provide the current working directory as the 'currentWorkingDirectory' parameter to scan for assemblies.")]
    public static string GetAvailableBindings(string currentWorkingDirectory)
    {
        return JsonSerializer.Serialize(BindingMetadataManager.GetAll(currentWorkingDirectory));
    }
}
