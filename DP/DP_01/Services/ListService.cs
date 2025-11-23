using Lab_01.Models;

namespace Lab_01.Services;

public interface IListService
{
    MyList ListModification(MyList list);
    int CountZeros(MyList list);
    MyList MapTransform(MyList list);
    MyList AppendLast(MyList list, object value);

    object SelectHead(MyList list);
    MyList SelectTail(MyList list);
    MyList Cons(object head, MyList tail);
}

public class ListService : IListService
{
    public object SelectHead(MyList list)
    {
        if (list is null || list.IsEmpty)
            throw new InvalidOperationException("Head is undefined for empty list.");
        return list.Head;
    }

    public MyList SelectTail(MyList list)
    {
        if (list is null || list.IsEmpty)
            throw new InvalidOperationException("Tail is undefined for empty list.");
        return list.Tail!;
    }

    public MyList Cons(object head, MyList tail)
    {
        if (head is null) throw new ArgumentNullException(nameof(head));
        return MyList.Cons(head, tail ?? MyList.Empty);
    }

    public MyList ListModification(MyList list)
    {
        return AppendLast(MapTransform(list), CountZeros(list));
    }

    public int CountZeros(MyList list)
    {
        if (list == null || list.IsEmpty) return 0;

        var h = SelectHead(list);
        var t = SelectTail(list);

        int count = 0;

        if (h is int iv && iv == 0)
            count++;
        else if (h is MyList subList)
            count += CountZeros(subList);

        return count + CountZeros(t);
    }

    public MyList MapTransform(MyList list)
    {
        if (list == null || list.IsEmpty) return MyList.Empty;

        var h = SelectHead(list);
        object newHead;

        if (h is int value)
        {
            if (value < 0) newHead = value + 1;
            else if (value > 0) newHead = "plus";
            else newHead = 0;
        }
        else if (h is MyList subList)
        {
            newHead = MapTransform(subList);
        }
        else
        {
            newHead = h;
        }

        return Cons(newHead, MapTransform(SelectTail(list)));
    }

    public MyList AppendLast(MyList list, object value)
    {
        if (list == null || list.IsEmpty) return Cons(value, MyList.Empty);
        return MyList.Cons(SelectHead(list), AppendLast(SelectTail(list), value));
    }
}
