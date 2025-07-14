
# Codi Project – AI Coding Instructions

## Project Overview
Codi is a CLI tool that converts JSON data models into C# object initialization code. It is designed for single-file architecture, clean separation of concerns, and modern .NET workflows.

## Architecture & Major Components
- **CLI Entry Point**: `Codi.Cli/Program.cs` – System.CommandLine setup, argument parsing, JSON loading, and code generation logic.
- **Code Generation**: `CSharpCode` public static class with extension method `ToCSharpInitializationString(JsonNode, className)` for recursive C# code generation from JSON.
- **CodeWriter**: `Codi.Cli/CodeWriter.cs` – sealed class extending `IndentedTextWriter` with custom methods for C# syntax (commas, semicolons, blocks, indentation).
- **Test Suite**: `Codi.Cli.Tests/` – XUnit-based tests for all supported JSON scenarios, referencing the CLI project directly.

## Developer Workflows
- **Build**: `dotnet build` (all projects)
- **Run CLI**: `dotnet run --project Codi.Cli -- --from <input.json> [--to <output-dir>]`
- **Test**: `dotnet test` (runs XUnit tests in `Codi.Cli.Tests`)
- **Release**: `dotnet publish -c Release` (self-contained executable)
- **NuGet Tool Packaging**: `dotnet pack Codi.Cli/Codi.Cli.csproj --configuration Release --output ./nupkg`

## Code Generation Flow
1. CLI parses arguments (`--from`, `--to`) using System.CommandLine.
2. Loads and parses JSON file to `JsonNode`.
3. Calls `ToCSharpInitializationString()` extension method.
4. Recursively traverses JSON via `ComputeInitializationFromSchema()`.
5. Writes output to `GeneratedInitialization.cs` in the target directory (creates directory if needed).

## Project-Specific Conventions
- **Meta-properties**: Properties starting with `$` are skipped (`if (prop.Key.StartsWith("$")) continue;`).
- **Trailing Commas**: All object properties and array elements end with a comma.
- **Indentation**: Managed via `CodeWriter.Indent++/--` for readable output.
- **Empty Arrays**: Always rendered as `[]`.
- **Error Handling**: Validates file existence and required parameters; unsupported JSON types emit `/* unsupported type */` comments.
- **Debugging**: Uses `Debug.WriteLine()` for development tracing.

## Testing & Quality
- **Test Project**: `Codi.Cli.Tests` (XUnit 2.9.3, .NET 10.0)
- **Test Coverage**: Simple, nested, array, empty array, complex, null, boolean, and number JSON scenarios.
- **Formatting**: Enforced via `dotnet format` in CI.

## Build & CI/CD
- **Solution File**: `Codi.slnx` (modern format, includes CLI and test projects)
- **CI Pipeline**: `.github/workflows/build-and-release.yml` – builds, formats, tests, packages, and (optionally) publishes NuGet tool.
- **Release Flow**: Tag-based release triggers NuGet packaging and asset upload.

## Integration Points
- **Input**: Any valid JSON file
- **Output**: Single C# file with object initialization code
- **No external services or databases** – pure file-based transformation

## Extending & Modifying
- **Add new JSON type support**: Extend the `JsonValueKind` switch in `ComputeInitializationFromSchema()` in `Program.cs`.
- **Change output format**: Update methods in `CodeWriter.cs` or code generation logic in `Program.cs`.
- **CLI changes**: Adjust `Option<string>` declarations and root command handler in `Program.cs`.

## Example: Code Generation
Input JSON:
```json
{
  "name": "John Doe",
  "skills": ["C#", "Python"]
}
```
Output C#:
```csharp
MyObject instance = new()
{
    name = "John Doe",
    skills =
    [
        "C#",
        "Python",
    ],
};
```
