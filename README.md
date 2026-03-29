# ReqnRollMcpServer

This is a Proof-of-Concept (PoC) implementation of a MCP (Model Context Protocol) server for [Reqnroll](https://github.com/reqnroll/Reqnroll) bindings, to enable AI agents to get information on the available bindings.

## Mission Statement
This project aims to simplify the creation of ReqnRoll feature files by allowing AI agents to query available ReqnRoll bindings and their documentation directly in a format suited for AI agents, minimizing AI hallucinations and providing necessary context to the agent.

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
dnx Rotbarsch.ReqnrollMcpServer --yes
```
7. Enter a unique and informative name for your configuration, e.g., "ReqnRoll MCP Server".
8. A file named `mcp.json` located in your `%APPDATA%/Code/User` directory will open. It should look something like this:
```
{
	"servers": {
		"ReqnRollMcp": {
			"type": "stdio",
			"command": "dnx",
			"args": [
				"Rotbarsch-ReqnrollMcpServer",
				"--yes",
				""
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
- Command: Enter `dnx Rotbarsch.ReqnrollMcpServer --yes`.
5. Click "Save". The MCP server is now added to your list of available tools.
6. GitHub Copilot is now ready for use. Try prompting it with:
```
List all available ReqnRoll bindings.
```
7. Before using a specific functionality (named "Tool" in the MCP world) for the first time, the chat will ask for your permission via a prompt.
After confirming that prompt, you should get an answer listing all available ReqnRoll bindings in the defined assemblies.

## "Unable to load type" and similar messages
Make sure all dependencies of the assemblies lie next to the assembly to inspect. The easieest way to achieve this is by setting the paths of a runnable, buildable ReqnRoll project referencing and using those bindings instead of the bindings project itself.

## The documentation XML is nowhere to be found
Depending on the configuration of the bindings csproj file, XML documentation is not always copied to the output directory. 
In that case, either adjust the bindings project to copy the XML documentation to the output directory or provide the path to the XML documentation manually in `inputs.json`. 
In case of nuget packages, check `%HOMEPATH%/.nuget/packages` for the XML documentation files.

## Bonus: Markdown Documentation Generator
The functionality to extract markdown documentation of Reqnroll bindings from an assembly is also available as a standalone command line tool.

Use via the following:

```
dnx Rotbarsch.ReqnrollDocumentationGenerator -i "<pathToAssembly>;<pathToXml>" -i "<pathToAssembly2>;<pathToXml2>" -o "<outputpath>"
```

Example:

```
dnx Rotbarsch.ReqnrollDocumentationGenerator -i "..\TestProject\MyBindings.dll;..\TestProject\MyBindings.xml" -o "Bindings.md"
```

You can get further information by calling:
```
dnx Rotbarsch.ReqnrollDocumentationGenerator --help
```

## License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.

### Third-Party Dependencies
Information about the licenses of dependencies can be found in the [THIRD-PARTY_NOTICES](./THIRD-PARTY_NOTICES.txt) file.
