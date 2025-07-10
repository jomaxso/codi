using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Codi.Cli.Tests;

public class CodiCliTests
{
    [Fact]
    public void SimpleJsonToCShar_ShouldGenerateValidCode()
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
        Assert.Contains("[\r\n", result); // Array start with newline
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
}
