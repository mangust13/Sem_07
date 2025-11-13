using System.Text;

namespace Lab_01;

public class DualWriter : TextWriter
{
    private readonly TextWriter console;
    private readonly StreamWriter file;
    public DualWriter(string filePath)
    {
        console = Console.Out;
        file = new StreamWriter(filePath);
        file.AutoFlush = false;
    }
    public override Encoding Encoding => console.Encoding;

    public override void Write(char value)
    {
        console.Write(value);
        file.Write(value);
    }
    public override void WriteLine(string? value)
    {
        console.WriteLine(value);
        file.WriteLine(value);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            file.Dispose();
        }
        base.Dispose(disposing);
    }
}
