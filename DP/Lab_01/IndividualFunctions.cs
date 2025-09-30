namespace Lab_01;

static public partial class Program
{
    static MyList ListModification(MyList list)
    {
        return AppendLast(MapTransform(list), CountZeros(list));
    }

    static int CountZeros(MyList list)
    {
        if (list == null || list.IsEmpty)
        {
            return 0;
        }

        if (SelectHead(list) is int && (int)SelectHead(list) == 0)
        {
            return 1 + CountZeros(SelectTail(list));
        }
        else
        {
            return CountZeros(SelectTail(list));
        }
    }

    static MyList MapTransform(MyList list)
    {
        if (list == null || list.IsEmpty)
        {
            return MyList.Empty;
        }

        object newHead;
        if (SelectHead(list) is int value)
        {
            if (value < 0)
            {
                newHead = value + 1;
            }
            else if (value > 0)
            {
                newHead = "plus";
            }
            else
            {
                newHead = 0;
            }
        }
        else
        {
            newHead = SelectHead(list);
        }

        return Cons(newHead, MapTransform(SelectTail(list)));
    }

    static MyList AppendLast(MyList list, object value)
    {
        if (list == null || list.IsEmpty)
        {
            return Cons(value, MyList.Empty);
        }

        return new MyList(SelectHead(list), AppendLast(SelectTail(list), value));
    }

}

// (a (b c . d) ((m)))