namespace Lab_01;

static public partial class Program
{
    static object SelectHead(MyList list)
    {
        if (list.IsEmpty)
        {
            throw new InvalidOperationException("Head is undefined for empty list.");
        }

        return list.Head;
    }

    static MyList SelectTail(MyList list)
    {
        if (list == null || list.IsEmpty)
        {
            throw new InvalidOperationException("Tail is undefined for empty list.");
        }

        return list.Tail;
    }

    static MyList Cons(object Head, MyList Tail)
    {
        if (Head is null)
        {
            throw new ArgumentNullException(nameof(Head));
        }

        return new MyList(Head, Tail ?? MyList.Empty);
    }
}
