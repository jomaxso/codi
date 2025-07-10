using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
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
        Console.WriteLine("Both 'from' and 'to' options are required.");
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
    File.WriteAllText(outputFile, code);

    Console.WriteLine($"Generated code written to: {outputFile}");

    return 0;
});

return rootCommand.Parse(args).Invoke();

file static class CSharpCode
{
    public static string ToCSharpInitializationString(this JsonNode json, string className = "MyObject")
    {
        using var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
        using var codeWriter = new CodeWriter(stringWriter, 0);

        codeWriter.Write($"{className} instance = ");

        ComputeInitializationFromSchema(codeWriter, json);

        return stringWriter.ToString();
    }

    private static CodeWriter ComputeInitializationFromSchema(CodeWriter codeWriter, JsonNode json, bool isRoot = true)
    {
        codeWriter.WriteLine("new()");
        codeWriter.StartBlock();

        foreach (var prop in json.AsObject())
        {
            // Überspringe Meta-Properties die mit $ beginnen
            if (prop.Key.StartsWith("$"))
            {
                continue;
            }

            var propertyName = prop.Key;
            var propertySchema = prop.Value;

            Debug.WriteLine($"Processing property: {propertyName}");
            Debug.WriteLine($"Property schema: {propertySchema?.ToJsonString()}");

            codeWriter.Write($"{propertyName} = ");

            if (json[prop.Key] is not JsonNode propertyValue)
            {
                codeWriter.WriteLineWithComma("null");
                continue;
            }

            Debug.WriteLine($"Property value: {propertyValue.ToJsonString()}");

            switch (propertyValue.GetValueKind())
            {
                case JsonValueKind.Null:
                    codeWriter.WriteLineWithComma("null");
                    break;
                case JsonValueKind.Undefined:
                    codeWriter.WriteLineWithComma("default");
                    break;
                case JsonValueKind.String:
                    codeWriter.WriteLineWithComma($"\"{propertyValue}\"");
                    break;
                case JsonValueKind.Number:
                    codeWriter.WriteLineWithComma(propertyValue.ToString());
                    break;
                case JsonValueKind.False:
                case JsonValueKind.True:
                    codeWriter.WriteLineWithComma(propertyValue.ToString().ToLower());
                    break;

                case JsonValueKind.Array:
                    var items = propertyValue.AsArray();
                    if (items.Count != 0)
                    {
                        codeWriter.StartCollection();

                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];

                            Debug.WriteLine($"Processing array item {i}: {item?.ToJsonString()}");

                            if (item is null)
                            {
                                continue;
                            }

                            ComputeInitializationFromSchema(codeWriter, item, false);
                        }

                        codeWriter.EndCollectionWithComma();
                    }
                    else
                    {
                        // Wenn das Array leer ist, initialisieren wir es als leeres Array
                        codeWriter.WriteLineWithComma("[]");
                    }
                    break;

                case JsonValueKind.Object:
                    ComputeInitializationFromSchema(codeWriter, propertyValue, false);
                    continue;

                default:
                    codeWriter.WriteLineWithComma("default /* unsupported type */");
                    break;
            }
        }

        if (isRoot)
        {
            codeWriter.EndBlockWithSemicolon();
        }
        else
        {
            codeWriter.EndBlockWithComma();
        }

        return codeWriter;
    }
}
