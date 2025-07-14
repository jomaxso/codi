using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Codi.Cli;

public static class CSharpCode
{
    public static string ToCSharpInitializationString(this JsonNode json, string className = "MyObject")
    {
        using var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
        using var codeWriter = new CodeWriter(stringWriter, 0);

        codeWriter.Write($"{className} instance = ");

        codeWriter.ComputeInitializationFromSchema(json);

        return stringWriter.ToString();
    }

    private static CodeWriter ComputeInitializationFromSchema(this CodeWriter codeWriter, JsonNode json, bool isRoot = true)
    {
        switch (json.GetValueKind())
        {
            case JsonValueKind.Object:
                WriteObjectInitialization(codeWriter, json, isRoot);
                break;
            case JsonValueKind.Array:
                WriteArrayInitialization(codeWriter, json, isRoot);
                break;
            default:
                HandleJsonValue(codeWriter, json);
                break;
        }
    
        return codeWriter;
    }

    // Extrahierte Methode für Objekt-Initialisierung
    private static void WriteObjectInitialization(CodeWriter codeWriter, JsonNode json, bool isRoot)
    {
        codeWriter.WriteLine("new()");
        codeWriter.StartBlock();

        foreach (var prop in json.AsObject())
        {
            if (prop.Key.StartsWith('$')) continue; // Meta-Properties überspringen
            codeWriter.Write($"{prop.Key} = ");
            codeWriter.HandleJsonValue(prop.Value);
        }

        if (isRoot)
            codeWriter.EndBlockWithSemicolon();
        else
            codeWriter.EndBlockWithComma();
    }

    // Extrahierte Methode für Array-Initialisierung
    private static void WriteArrayInitialization(CodeWriter codeWriter, JsonNode json, bool isRoot)
    {
        var items = json.AsArray();
    
        if (items.Count == 0)
        {
            if (isRoot)
                codeWriter.WriteLine("[];");
            else
                codeWriter.WriteLineWithComma("[]");
            return;
        }
    
        codeWriter.StartCollection();
    
        foreach (var item in items)
        {
            if (item is null) continue;
            ComputeInitializationFromSchema(codeWriter, item, false);
        }
    
        if (isRoot)
            codeWriter.EndCollectionWithSemicolon();
        else
            codeWriter.EndCollectionWithComma();
    }

    private static void HandleJsonValue(this CodeWriter codeWriter, JsonNode? propertyValue)
    {
        if (propertyValue is null)
        {
            codeWriter.WriteLineWithComma("null");
            return;
        }
        
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
                codeWriter.WriteLineWithComma("false");
                break;
            case JsonValueKind.True:
                codeWriter.WriteLineWithComma("true");
                break;

            case JsonValueKind.Array:
                var items = propertyValue.AsArray();
                if (items.Count != 0)
                {
                    codeWriter.StartCollection();

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];

                        Debug.WriteLine("Processing array item {0}", item?.ToJsonString() ?? "null");

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
                break;

            default:
                codeWriter.WriteLineWithComma("default /* unsupported type */");
                break;
        }
    }
}