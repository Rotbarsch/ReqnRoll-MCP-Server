using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using ReqnRollBindingMetadataExtractorService.Services;

namespace ReqnRollMcpServer.Tools;

[McpServerToolType]
[Description("Provides tools for cataloguing ReqnRoll bindings.")]
public static class BindingsTool
{
    [McpServerTool(Name="get_bindings",Title="Get Reqnroll bindings available in workspace.")]
    [Description("Returns information about all ReqnRoll bindings available by inspecting all available assemblies.")]
    public static string GetAvailableBindings(
        [Description("Current working directory containing the assemblies to inspect (they don't need to be in root; they can be anywhere in the working directory).")]
        string currentWorkingDirectory)
    {
        return JsonSerializer.Serialize(BindingMetadataManager.GetAll(currentWorkingDirectory));
    }
}
