using System.CodeDom.Compiler;

namespace Codi.Cli;

public sealed class CodeWriter : IndentedTextWriter
{
    public CodeWriter(StringWriter stringWriter, int baseIndent) : base(stringWriter)
    {
        Indent = baseIndent;
    }

    public void WriteWithComma<T>(T value)
    {
        base.Write(value);
        base.Write(",");
    }

    public void WriteLineWithComma<T>(T value)
    {
        base.Write(value);
        base.WriteLine(",");
    }

    public void WriteLineWithSemicolon<T>(T value)
    {
        base.Write(value);
        base.WriteLine(";");
    }

    public void StartCollection()
    {
        this.WriteLine();
        this.WriteLine("[");
        this.Indent++;
    }

    public void EndCollection()
    {
        this.Indent--;
        this.WriteLine("]");
    }

    public void EndCollectionWithComma()
    {
        this.Indent--;
        this.WriteLine("],");
    }
    public void EndCollectionWithSemicolon()
    {
        this.Indent--;
        this.WriteLine("];");
    }

    public void StartBlock()
    {
        this.WriteLine("{");
        this.Indent++;
    }

    public void EndBlock()
    {
        this.Indent--;
        this.WriteLine("}");
    }

    public void EndBlockWithComma()
    {
        this.Indent--;
        this.WriteLine("},");
    }

    public void EndBlockWithSemicolon()
    {
        this.Indent--;
        this.WriteLine("};");
    }

    // The IndentedTextWriter adds the indentation
    // _after_ writing the first line of text. This
    // method can be used ot initialize indentation
    // when an emit method might only emit one line
    // of code or when the code writer is emitting
    // indented code as part of a larger string.
    public void InitializeIndent()
    {
        for (var i = 0; i < Indent; i++)
        {
            Write(DefaultTabString);
        }
    }
}