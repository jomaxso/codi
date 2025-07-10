# Codi Project - AI Coding Instructions

## Project Overview
Codi is a CLI tool that converts JSON data models into C# object initialization code. It uses a single-file architecture with extension methods for clean separation of concerns.

## Architecture Patterns

### Core Components
- **CLI Entry Point**: `Program.cs` contains the System.CommandLine setup with `--from` (required) and `--to` (optional) parameters
- **Code Generation**: `CSharpCode` static class with extension method `ToCSharpInitializationString()` that operates on `JsonNode`
- **Code Writing**: `CodeWriter` sealed class extending `IndentedTextWriter` for formatted output with custom methods like `WriteLineWithComma()`, `StartBlock()`, `EndBlockWithSemicolon()`

### Key Design Decisions
- Uses `file static class` for the `CSharpCode` class (file-scoped visibility)
- Recursive pattern for handling nested JSON structures via `ComputeInitializationFromSchema()`
- Extension method pattern: `JsonNode.ToCSharpInitializationString()` for clean API
- Custom `CodeWriter` with specialized methods for C# syntax (commas, semicolons, blocks)

## Development Workflows

### Building & Running
```bash
dotnet build                    # Standard build
dotnet run -- -f input.json    # Run with sample JSON file
dotnet publish -c Release      # Create release build
```

### Code Generation Flow
1. Parse CLI arguments using System.CommandLine
2. Load JSON file and parse to `JsonNode`
3. Call `ToCSharpInitializationString()` extension method
4. Recursive traversal through `ComputeInitializationFromSchema()`
5. Output to `GeneratedInitialization.cs` in target directory

## Project-Specific Conventions

### JSON Processing Patterns
- Skip properties starting with `$` (meta-properties): `if (prop.Key.StartsWith("$")) continue;`
- Handle all `JsonValueKind` cases explicitly in switch statements
- Use `Debug.WriteLine()` for tracing during development
- Arrays: Empty arrays become `[]`, populated arrays use collection syntax with trailing commas

### Code Generation Patterns
- Default class name is "MyObject" but parameterizable
- Use `new()` syntax for object initialization (C# 9+ target type inference)
- Trailing commas on all object properties and array elements
- Proper indentation management through `CodeWriter.Indent++/--`

### Error Handling
- File existence validation before processing
- Required parameter validation with descriptive error messages
- Graceful handling of unsupported JSON types with `/* unsupported type */` comments

## Dependencies & Framework
- **Target**: .NET 10.0 (cutting edge, note the preview nature)
- **Key Package**: `System.CommandLine.Hosting` v0.4.0-alpha (not the main System.CommandLine package)
- **No testing framework** currently configured

## File Structure Conventions
```
Codi.Cli/Program.cs     # Everything in one file: CLI setup, JSON processing, code generation
Codi.slnx              # Modern .slnx format (not .sln)
```

## Integration Points
- Input: JSON files (any valid JSON structure)
- Output: Single C# file with object initialization code
- No external services or databases - purely file-based transformation tool

## Common Tasks
- **Adding new JSON type support**: Extend the `JsonValueKind` switch in `ComputeInitializationFromSchema()`
- **Modifying output format**: Update `CodeWriter` methods or the generation logic
- **CLI changes**: Modify the `Option<string>` declarations and `SetAction` handler
