using Xunit;

namespace Codi.Cli.Tests;

public class CodeWriterTests
{
    [Fact]
    public void AllWriterMethods_ShouldNotThrow()
    {
        var sw = new StringWriter();
        var writer = new CodeWriter(sw, 0);
        writer.WriteLineWithComma("test");
        writer.WriteLineWithSemicolon("test");
        writer.StartCollection();
        writer.EndCollection();
        writer.EndCollectionWithComma();
        writer.EndCollectionWithSemicolon();
        writer.StartBlock();
        writer.EndBlock();
        writer.EndBlockWithComma();
        writer.EndBlockWithSemicolon();
        writer.InitializeIndent();
        writer.WriteWithComma("test");
        Assert.NotNull(sw.ToString());
    }
    [Fact]
    public void InitializeIndent_ShouldWriteTabs_WhenIndentIsGreaterThanZero()
    {
        var sw = new StringWriter();
        var writer = new CodeWriter(sw, 3); // Indent > 0
        writer.InitializeIndent();
        var result = sw.ToString();
        Assert.Contains(CodeWriter.DefaultTabString, result);
    }
}
