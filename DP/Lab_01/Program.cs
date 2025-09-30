namespace Lab_01;

static public partial class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Enter elements of list: ");
        string? input = Console.ReadLine();
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(input));
        var list = MyList.CreateList(input!);

        Console.WriteLine("Head of list: " + SelectHead(list));
        Console.WriteLine("Tail of list: " + SelectTail(list));
        Console.WriteLine("Added head 'a' to current list: " + Cons('a', list));

        Console.WriteLine("Modified list: " + ListModification(list));
    }
}