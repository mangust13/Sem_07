namespace Lab_01
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(1 << 2);

            using var writer = new DualWriter("output.txt");
            Console.SetOut(writer);

            int a = (int)Math.Pow(9, 3), c = 233, m = 1 << 22 - 1, x0 = 5;
            Console.Write("Enter N: ");
            if (!int.TryParse(Console.ReadLine(), out int N))
            {
                Console.WriteLine("N should be a number");
                return;
            }

            try
            {
                var random = new RandomNumber(a, c, m);
                var nums = random.GenerateNumbers(x0, N);
                Printer.Print(nums);

                int period = random.GetPeriod(x0);
                Console.WriteLine($"Period: {period}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
        }

        

        
    }
}
