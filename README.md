# ReqnRollMcpServer

This is a Proof-of-Concept (PoC) implementation of a MCP (Model Context Protocol) server for [ReqnRoll](https://github.com/reqnroll/Reqnroll) bindings, to enable AI agents to get information on the available bindings.

## Mission Statement
This project aims to simplify the creation of ReqnRoll feature files by allowing AI agents to query available ReqnRoll bindings and their documentation directly in a format suited for AI agents, minimizing AI hallucinations and providing necessary context to the agent.

## Setting up the server

1. Clone the repository and ensure it builds (.NET 10 required). *Currently, it is not published to any repositories.*
2. Prepare the [inputs.json](./ReqnRollMcpServer/inputs.json) by setting up the paths to DLL files containing ReqnRoll bindings and, if available, their XML documentation.
If no XML documentation is provided, the server will attempt to find the corresponding XML next to the provided DLL.
If still no XML file is found, the server will proceed without documentation for that binding, decreasing its value considerably.
Example:
```
[
	{
		"dll": "FirstAssemblyWithBindings.dll",
		"xml": "FirstAssemblyWithBindings.xml"		
	},
	{
		"dll": "AnotherAssemblyWithBindings.dll",
		"xml": "AnotherAssemblyWithBindings.xml"		
	}
]
```
Only bindings visible to the server through the configured `inputs.json` will be available when chatting with the AI agent of choice.

## Making the MCP server available to AI agents

### Visual Studio Code with GitHub Copilot extension

Assuming you have the GitHub Copilot extension installed in Visual Studio Code, you can configure it to use the MCP server as follows:

1. If not open already, open the GitHub Copilot chat, e.g., by pressing `CTRL+ALT+I` or whatever shortcut is configured for your environment.
2. Make sure GitHub Copilot is set to "Agent" on the bottom left of the chat window.
3. Click the small icon depicting a wrench and screwdriver on the bottom of the chat, next to the selection of the model. At the top of the Visual Studio Code window, a list of available MCP servers opens.
4. Click the small icon labeled "Add MCP Server..." at the top of the list of available MCP servers.
5. Next, select "Command (stdio)".
6. Enter the following command in the prompt and confirm:
```
dotnet run --project PATH_TO_YOUR_CLONED_REPO/ReqnRollMcpServer/ReqnRollMcpServer.csproj
```
Replace `PATH_TO_YOUR_CLONED_REPO` with your local path to the cloned ReqnRollMcpServer repository.

7. Enter a unique and informative name for your configuration, e.g., "ReqnRoll MCP Server".
8. A file named `mcp.json` located in your `%APPDATA%/Code/User` directory will open. It should look something like this:
```
{
	"servers": {
		"ReqnRollMcp": {
			"type": "stdio",
			"command": "dotnet",
			"args": [
				"run",
				"--project",
				"C:\\...\\ReqnRollMcpServer\\ReqnRollMcpServer.csproj"
			]
		}
	},
	"inputs": []
}
```
9. Save the file. Notice the small `Start` prompt on top of the JSON node describing your newly added MCP server. Click it to start the server. If anything goes wrong, Visual Studio Code will display the console output of the server with a detailed stack trace.
10. Clicking the wrench and screwdriver icon again will now show your MCP server. If the checkbox next to it is unchecked, check it.
11. GitHub Copilot is now ready for use. Try prompting it with:
```
List all available ReqnRoll bindings.
```
12. Before using a specific functionality (named "Tool" in the MCP world) for the first time, the chat will ask for your permission via a prompt.
After confirming that prompt, you should get an answer listing all available ReqnRoll bindings in the defined assemblies.

### Visual Studio with GitHub Copilot
1. Open the window "GitHub Copilot Chat".
2. Click the small wrench icon on the bottom right of the chat window labeled "Select tools". A list of available MCP servers opens.
3. Click the small green plus on the top right of the tool list. A new dialog opens.
4. Fill out the dialog:
- Destination: Select whether you want the server to be available globally or in the current solution only.
- Server ID: Enter a unique and informative name for your configuration, e.g., "ReqnRoll MCP Server".
- Type: Select "stdio".
- Command: Enter `dotnet run --project PATH_TO_YOUR_CLONED_REPO/ReqnRollMcpServer/ReqnRollMcpServer.csproj`, replacing `PATH_TO_YOUR_CLONED_REPO` with your local file path.
5. Click "Save". The MCP server is now added to your list of available tools.
6. GitHub Copilot is now ready for use. Try prompting it with:
```
List all available ReqnRoll bindings.
```
7. Before using a specific functionality (named "Tool" in the MCP world) for the first time, the chat will ask for your permission via a prompt.
After confirming that prompt, you should get an answer listing all available ReqnRoll bindings in the defined assemblies.

## Bonus: Markdown Documentation Generator
The repository also includes a simple console application that generates markdown documentation for all available ReqnRoll bindings based on the same `inputs.json` file used by the MCP server. Simply start the console app with an argument providing the desired output file path (eg. `C:/source/Bindings.md`) and prepared `inputs.json`.