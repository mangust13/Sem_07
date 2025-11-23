namespace Lab_01.Models;

public class MyList
{
    public object Head { get; }
    public MyList? Tail { get; }

    private MyList(object head, MyList? tail)
    {
        Head = head;
        Tail = tail;
    }

    public bool IsEmpty => this == Empty;

    public static readonly MyList Empty = new MyList(new object(), null);

    public static MyList Cons(object head, MyList? tail)
    {
        if (head is null) throw new ArgumentNullException(nameof(head));
        return new MyList(head, tail ?? Empty);
    }

    public static MyList FromArray(object[] items) => FromArray(items, 0);

    private static MyList FromArray(object[] items, int i)
    {
        if (items == null || i >= items.Length) return Empty;
        return Cons(items[i], FromArray(items, i + 1));
    }

    public static MyList CreateList(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Empty;

        var tokens = Tokenize(input).ToList();
        int pos = 0;
        return (MyList)ParseList(tokens, ref pos);
    }

    private static object ParseList(List<string> tokens, ref int pos)
    {
        if (tokens[pos] == "(")
        {
            pos++;
            var elements = new List<object>();
            while (pos < tokens.Count && tokens[pos] != ")")
            {
                elements.Add(ParseList(tokens, ref pos));
            }
            pos++;
            return FromArray(elements.ToArray());
        }
        else if (tokens[pos] == ")")
        {
            throw new InvalidOperationException("Unexpected ')'");
        }
        else
        {
            string t = tokens[pos++];
            if (int.TryParse(t, out int num)) return num;
            if (t.Length == 1) return t[0];
            return t;
        }
    }

    private static IEnumerable<string> Tokenize(string input)
    {
        var spaced = input.Replace("(", " ( ").Replace(")", " ) ");
        return spaced.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    public override string ToString()
    {
        if (IsEmpty) return "()";
        return "(" + ToStringElements(this) + ")";
    }

    private static string ToStringElements(MyList list)
    {
        if (list.IsEmpty) return "";
        if (list.Tail is null || list.Tail.IsEmpty) return list.Head.ToString()!;
        return list.Head + " " + ToStringElements(list.Tail);
    }
}
