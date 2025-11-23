namespace Lab_01;

public class RandomNumber
{
    public uint a {  get; set; }
    public uint c { get; set; }
    public uint m { get; set; }
    public RandomNumber(uint a, uint c, uint m)
    {
        if (m < 0)
        {
            throw new ArgumentException("m should be > 0");
        }
        if (a >= m)
        {
            throw new ArgumentOutOfRangeException(nameof(a), "a should be correct");
        }
        if (c >= m)
        {
            throw new ArgumentOutOfRangeException(nameof(c), "c should be correct");
        }

        this.a = a;
        this.c = c;
        this.m = m;
    }


    public List<uint> GenerateNumbers(uint x0, uint N)
    {
        if (x0 < 0 || x0 >= m)
        {
            throw new ArgumentOutOfRangeException(nameof(x0), "x0 should be 0 ≤ x0 < m ");
        }
        if (N <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(N), "N should be > 0");
        }

        uint xn = x0;
        var res = new List<uint>();
        for (uint i = 0; i < N; i++)
        {
            xn = LCG(xn);
            res.Add(xn);
        }

        return res;
    }
    public int GetPeriod(uint x0)
    {
        var seen = new HashSet<uint>();
        uint xn = x0;
        int period = 0;
        List<uint> list = new List<uint>();

        while (true)
        {
            uint xprev = xn;
            uint xnext = LCG(xn);

            if (xprev == xn && xnext == xn)
            {
                return 1;
            }
            else if (xprev != xn && xnext == xn)
            {
                return 0;
            }

            seen.Add(xn);
            xn = LCG(xn);
            period++;
        }
    }


    private uint LCG(uint x0)
    {
        return (uint)(((ulong)a * x0 + c) % m);
    }
}
