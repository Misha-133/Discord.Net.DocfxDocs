using System.Text;

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