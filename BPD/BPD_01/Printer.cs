namespace Lab_01;

public static class Printer
{
    public static void Print(List<uint> nums)
    {
        string text = "{" + String.Join(", ", nums) + "}";
        Console.WriteLine(text);
    }
}
