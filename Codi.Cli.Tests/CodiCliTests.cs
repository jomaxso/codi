using System.Text.Json.Nodes;
using Xunit;

namespace Codi.Cli.Tests;

public class CodiCliTests
{

    [Fact]
    public void ArrayWithNullElement_ShouldHitNullItemBranch()
    {
        // Arrange
        var arr = new JsonArray(JsonValue.Create(1), null, JsonValue.Create(2));
        var obj = new JsonObject();
        obj["arr"] = arr;

        // Act
        var result = obj.ToCSharpInitializationString();

        // Assert: Die Ausgabe enthält beide Zahlen, aber kein 'null,' für das mittlere Element
        Assert.Contains("1,", result);
        Assert.Contains("2,", result);
        Assert.DoesNotContain("null,", result); // null-Elemente werden übersprungen
    }

    [Fact]
    public void RootArrayJsonToCSharp_ShouldHandleNullAndMixedTypes()
    {
        // Arrange
        var json = "[\"a\", null, 42, true, {\"x\":1}, []]";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("[", result); // Array start
        Assert.Contains("\"a\",", result);
        // Null-Elemente werden übersprungen, daher nicht enthalten
        Assert.DoesNotContain("null,", result);
        Assert.Contains("42,", result);
        Assert.Contains("true,", result);
        Assert.Contains("x = 1", result); // Objekt im Array
        Assert.Contains("[]", result); // Leeres Array im Array
    }
    [Fact]
    public void RootArrayJsonToCSharp_ShouldHandleRootArrays()
    {
        // Arrange
        var json = "[\"x\", \"y\", \"z\"]";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("[", result); // Array start
        Assert.Contains("\"x\",", result);
        Assert.Contains("\"y\",", result);
        Assert.Contains("\"z\",", result);
    }

    [Fact]
    public void JsonWithMetaProperties_ShouldSkipMetaProperties()
    {
        // Arrange
        var json = """{"$meta": "shouldBeSkipped", "name": "Test"}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.DoesNotContain("$meta", result);
        Assert.Contains("name = \"Test\",", result);
    }

    [Fact]
    public void JsonWithUndefined_ShouldHandleUnsupportedType()
    {
        // Arrange
        var obj = new JsonObject();
        // Simuliere einen "unsupported" Wert, indem ein Wert null gesetzt wird
        obj["unsupported"] = null;
        // Act
        var result = obj.ToCSharpInitializationString();
        // Assert
        Assert.Contains("unsupported = null,", result);
        // Für explizit unsupported types müsste man JsonValueKind.Undefined setzen, was mit JsonNode nicht direkt geht
        // Daher ist dies ein Grenzfall, der im Code als default /* unsupported type */ behandelt wird, wenn ein unbekannter Typ kommt
    }
    [Fact]
    public void SimpleJsonToCSharp_ShouldGenerateValidCode()
    {
        // Arrange
        var json = """{"name": "Test", "value": 123}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("name = \"Test\",", result);
        Assert.Contains("value = 123,", result);
        Assert.Contains("new()", result);
    }

    [Fact]
    public void NestedJsonToCSharp_ShouldHandleNestedObjects()
    {
        // Arrange
        var json = """{"user": {"name": "John", "age": 30}}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("user = new()", result);
        Assert.Contains("name = \"John\",", result);
        Assert.Contains("age = 30,", result);
    }

    [Fact]
    public void ArrayJsonToCSharp_ShouldHandleArrays()
    {
        // Arrange
        var json = """{"items": ["a", "b", "c"]}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("items =", result);
        Assert.Contains($"[{Environment.NewLine}", result); // Array start with newline
        Assert.Contains("\"a\",", result);
        Assert.Contains("\"b\",", result);
        Assert.Contains("\"c\",", result);
    }

    [Fact]
    public void EmptyArrayJsonToCSharp_ShouldHandleEmptyArrays()
    {
        // Arrange
        var json = """{"items": []}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("items = [],", result);
    }

    [Fact]
    public void ComplexJsonToCSharp_ShouldHandleComplexStructures()
    {
        // Arrange
        var json = """
    {
        "users": [
            {"name": "John", "age": 30},
            {"name": "Jane", "age": 25}
        ],
        "config": {
            "debug": true,
            "timeout": 5000
        }
    }
    """;
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("users =", result);
        Assert.Contains("config = new()", result);
        Assert.Contains("debug = true,", result);
        Assert.Contains("timeout = 5000,", result);
        Assert.Contains("name = \"John\",", result);
        Assert.Contains("age = 30,", result);
    }

    [Fact]
    public void JsonWithNullValue_ShouldHandleNull()
    {
        // Arrange
        var json = """{"name": null, "value": 123}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("name = null,", result);
        Assert.Contains("value = 123,", result);
    }

    [Fact]
    public void JsonWithBooleanValues_ShouldHandleBooleans()
    {
        // Arrange
        var json = """{"enabled": true, "visible": false}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("enabled = true,", result);
        Assert.Contains("visible = false,", result);
    }

    [Fact]
    public void JsonWithNumbers_ShouldHandleIntegers()
    {
        // Arrange
        var json = """{"count": 42, "percentage": 85.5}""";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("count = 42,", result);

        Assert.Contains("percentage = 85.5,", result);
    }

    [Fact]
    public void RootEmptyArrayJsonToCSharp_ShouldHandleEmptyArray()
    {
        // Arrange
        var json = "[]";
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Equal("MyObject instance = [];", result.Trim());
    }

    [Fact]
    public void MultipleEmptyArrays_ShouldHandleAllEmptyArrays()
    {
        // Arrange
        var json = """
            {
                "array1": [],
                "nested": {
                    "array2": []
                },
                "arrays": [[], [], []]
            }
            """;
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();
    
        // Assert
        Assert.Contains("array1 = [],", result);
        Assert.Contains("array2 = [],", result);
        // Find the arrays property robustly
        var arraysStart = result.IndexOf("arrays =");
        Assert.True(arraysStart >= 0, $"arrays property not found. Output: {result}");
        var bracketStart = result.IndexOf('[', arraysStart);
        Assert.True(bracketStart > arraysStart, $"arrays block not found. Output: {result}");
        var blockLines = result[bracketStart..].Split('\n');
        int emptyArrayCount = 0;
        foreach (var line in blockLines)
        {
            if (line.Trim() == "[]" || line.Trim() == "[],")
                emptyArrayCount++;
            if (line.Trim().StartsWith("]"))
                break;
        }

        Assert.Equal(3, emptyArrayCount);
    }

    [Fact]
    public void DeeplyNestedEmptyArrays_ShouldHandleNestedEmptyArrays()
    {
        // Arrange
        var json = """
            {
                "nested": {
                    "data": [
                        {
                            "items": []
                        }
                    ]
                }
            }
            """;
        var jsonNode = JsonNode.Parse(json);

        // Act
        var result = jsonNode!.ToCSharpInitializationString();

        // Assert
        Assert.Contains("items = [],", result);
        Assert.Contains("data =", result);
        Assert.Contains("nested = new()", result);
    }
}
