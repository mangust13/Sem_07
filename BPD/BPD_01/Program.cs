namespace Lab_01;

public static class Program
{
    static void Main(string[] args)
    {
        using var writer = new DualWriter("output.txt");
        Console.SetOut(writer);

        //uint a = ReadUInt("Enter a: ");
        //uint c = ReadUInt("Enter c: ");
        //uint m = ReadUInt("Enter m: ");
        //uint x0 = ReadUInt("Enter x0: ");
        uint N = ReadUInt("Enter N: ");

        //uint a = (uint)Math.Pow(9, 3), c = 233, m = (1 << 22) - 1, x0 = 5;
        //uint a = (uint)Math.Pow(2, 16), c = 28657, m = (uint)Math.Pow(2, 31), x0 = 33;
        uint a = (uint)Math.Pow(7, 7), c = 0, m = (uint)Math.Pow(2, 31), x0 = 1;


        try
        {
            var random = new RandomNumber(a, c, m);
            var nums = random.GenerateNumbers(x0, N);
            //Printer.Print(nums);

            int period = random.GetPeriod(x0);
            Console.WriteLine($"Period: {period}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static public uint ReadUInt(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            input = input.Replace(" ", "");

            if (input.Contains('^'))
            {
                var parts = input.Split('^');
                if (parts.Length == 2 &&
                    uint.TryParse(parts[0], out uint baseNum))
                {
                    string expPart = parts[1];
                    if (expPart.Contains('-'))
                    {
                        var expParts = expPart.Split('-');
                        if (expParts.Length == 2 &&
                            uint.TryParse(expParts[0], out uint power) &&
                            uint.TryParse(expParts[1], out uint sub))
                        {
                            return (uint)Math.Pow(baseNum, power) - sub;
                        }
                    }
                    else if (uint.TryParse(expPart, out uint power))
                    {
                        return (uint)Math.Pow(baseNum, power);
                    }
                }
            }

            if (uint.TryParse(input, out uint value))
                return value;
        }
    }

}
