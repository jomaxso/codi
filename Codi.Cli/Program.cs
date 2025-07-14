using System.CommandLine;
using System.Text.Json.Nodes;
using Codi.Cli;

var fromOption = new Option<string>("--from", "-f")
{
    Required = true,
    Description = "The path to the domain model file to generate code from."
};

var toOption = new Option<string>("--to", "-t")
{
    Required = false,
    Description = "The path to the output directory for the generated code."
};

var rootCommand = new RootCommand("Codi - A command line interface for WebOfMe")
{
    fromOption,
    toOption
};

rootCommand.SetAction(parseResult =>
{
    string parsedFile = parseResult.GetRequiredValue(fromOption);
    string? outputDirectory = parseResult.GetValue(toOption);

    if (string.IsNullOrWhiteSpace(parsedFile))
    {
        Console.WriteLine("The '--from' option is required.");
        return 1;
    }

    if (!File.Exists(parsedFile))
    {
        Console.WriteLine($"The file '{parsedFile}' does not exist.");
        return 1;
    }

    var jsonFile = File.ReadAllText(parsedFile);
    var jsonNode = JsonNode.Parse(jsonFile)!;
    var code = jsonNode.ToCSharpInitializationString();

    Console.WriteLine("Generating C# code for initialization...");

    var outputFile = Path.Combine(outputDirectory ?? Path.GetDirectoryName(parsedFile)!, "GeneratedInitialization.cs");

    // Stelle sicher, dass das Ausgabeverzeichnis existiert
    var outputDir = Path.GetDirectoryName(outputFile)!;
    Directory.CreateDirectory(outputDir);

    File.WriteAllText(outputFile, code);

    Console.WriteLine($"Generated code written to: {outputFile}");

    return 0;
});

return rootCommand.Parse(args).Invoke();
