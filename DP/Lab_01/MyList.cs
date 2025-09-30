using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_01;

public class MyList
{
    public object Head { get; set; }
    public MyList Tail { get; private set; }
    public MyList(object head, MyList tail)
    {
        Head = head;
        Tail = tail;
    }

    public bool IsEmpty => Head == null && Tail == null;
    public static MyList Empty => new MyList(null!, null!);
    public override string ToString()
    {
        if (this.IsEmpty)
        {
            return "[]";
        }

        return "[" + ToStringElements(this) + "]";
    }

    private static string ToStringElements(MyList list)
    {
        if (list.IsEmpty)
        {
            return "";
        }
        if (list.Tail.IsEmpty)
        {
            return list.Head.ToString()!;
        }
        return list.Head.ToString() + ", " + ToStringElements(list.Tail);
    }

    public static MyList CreateList(string input)
    {
        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        object[] parsed = tokens.Select(t =>
        {
            if (int.TryParse(t, out int num))
            {
                return (object)num;
            }
            if (t.Length == 1)
            {
                return (object)t[0];
            }
            return (object)t;
        }).ToArray();

        return FromArray(parsed);
    }

    private static MyList FromArray(object[] items) => FromArray(items, 0);

    private static MyList FromArray(object[] items, int index)
    {
        if (items is null || index >= items.Length)
        {
            return Empty;
        }
        return new MyList(items[index], FromArray(items, index + 1));
    }
}
