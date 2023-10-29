using System.Text;

using Json.Schema;

namespace Discord.Net.DocfxDocs;

public class ConsoleOutputCapture : TextWriter
{
    private TextWriter stdOutWriter;
    public override Encoding Encoding => Encoding.UTF8;

    private StringWriter? pendingLine;

    public ConsoleOutputCapture()
    {
        stdOutWriter = Console.Out;
        Console.SetOut(this);
    }

    public event ConsoleCaptureEventHandler OnWriteLine;

    public override void Write(char value)
        => Write(value.ToString());

    public override void Write(string? value)
    {
        pendingLine ??= new ();
        pendingLine?.Write(value);

        if (value == "\n")
        {
            OnWriteLine(this, new ConsoleCaptureArgs(pendingLine?.ToString() ?? string.Empty));
            pendingLine = null;
        }

        stdOutWriter.Write(value ?? string.Empty);
    }

    public override void Write(object? value)
    {
        pendingLine ??= new();
        pendingLine?.Write(value?.ToString());

        if (value is "\n")
        {
            OnWriteLine(this, new ConsoleCaptureArgs(pendingLine?.ToString() ?? string.Empty));
            pendingLine = null;
        }

        stdOutWriter.Write(value?.ToString() ?? string.Empty);
    }

    public override void Write(string format, params object?[] arg)
    {
        pendingLine ??= new();
        pendingLine?.Write(format, arg);

        if (format == "\n")
        {
            OnWriteLine(this, new ConsoleCaptureArgs(pendingLine?.ToString() ?? string.Empty));
            pendingLine = null;
        }

        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg)));
        stdOutWriter.Write(format, arg);
    }

    public override void Write(string format, object? arg)
    {
        pendingLine ??= new();
        pendingLine?.Write(format, arg);

        if (format == "\n")
        {
            OnWriteLine(this, new ConsoleCaptureArgs(pendingLine?.ToString() ?? string.Empty));
            pendingLine = null;
        }

        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg)));
        stdOutWriter.Write(format, arg);
    }

    public override void Write(string format, object? arg1, object? arg2)
    {
        pendingLine ??= new();
        pendingLine?.Write(format, arg1, arg2);

        if (format == "\n")
        {
            OnWriteLine(this, new ConsoleCaptureArgs(pendingLine?.ToString() ?? string.Empty));
            pendingLine = null;
        }

        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg1, arg2)));
        stdOutWriter.Write(format, arg1, arg2);
    }

    public override void Write(string format, object? arg1, object? arg2, object? arg3)
    {
        pendingLine ??= new();
        pendingLine?.Write(format, arg1, arg2, arg3);

        if (format == "\n")
        {
            OnWriteLine(this, new ConsoleCaptureArgs(pendingLine?.ToString() ?? string.Empty));
            pendingLine = null;
        }

        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg1, arg2, arg3)));
        stdOutWriter.Write(format, arg1, arg2, arg3);
    }

    public override void WriteLine(string? output)
    {
        if (pendingLine is not null)
        { 
            OnWriteLine(this, new ConsoleCaptureArgs(pendingLine.ToString()));
            pendingLine = null;
        }

        OnWriteLine(this, new ConsoleCaptureArgs(output ?? string.Empty));
        stdOutWriter.WriteLine(output ?? string.Empty);
    }

    public override void WriteLine(string format, params object?[] arg)
    {
        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg)));
        stdOutWriter.WriteLine(format, arg);
    }

    public override void WriteLine(string format, object? arg)
    {
        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg)));
        stdOutWriter.WriteLine(format, arg);
    }

    public override void WriteLine(string format, object? arg1, object? arg2)
    {
        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg1, arg2)));
        stdOutWriter.WriteLine(format, arg1, arg2);
    }

    public override void WriteLine(string format, object? arg1, object? arg2, object? arg3)
    {
        OnWriteLine(this, new ConsoleCaptureArgs(string.Format(format, arg1, arg2, arg3)));
        stdOutWriter.WriteLine(format, arg1, arg2, arg3);
    }

    public override void WriteLine(StringBuilder? value)
    {
        OnWriteLine(this, new ConsoleCaptureArgs(value?.ToString() ?? string.Empty));
        stdOutWriter.WriteLine(value?.ToString() ?? string.Empty);
    }

    public override void WriteLine(object? value)
    {
        OnWriteLine(this, new ConsoleCaptureArgs(value?.ToString() ?? string.Empty));
        stdOutWriter.WriteLine(value?.ToString() ?? string.Empty);
    }
}

public class ConsoleCaptureArgs : EventArgs
{
    public string Line;

    public ConsoleCaptureArgs(string line)
    {
        Line = line;
    }
}

public delegate void ConsoleCaptureEventHandler(object source, ConsoleCaptureArgs e);