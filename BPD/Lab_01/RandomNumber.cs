using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_01
{
    public class RandomNumber
    {
        public int a {  get; set; }
        public int c { get; set; }
        public int m { get; set; }
        public RandomNumber(int a, int c, int m)
        {
            if (m < 0)
            {
                throw new ArgumentException("m should be > 0");
            }
            if (a < 0 || a >= m)
            {
                throw new ArgumentOutOfRangeException(nameof(a), "a should be 0 ≤ a < m ");
            }
            if (c < 0 || c >= m)
            {
                throw new ArgumentOutOfRangeException(nameof(c), "c should be 0 ≤ c < m ");
            }

            this.a = a;
            this.c = c;
            this.m = m;
        }


        public List<int> GenerateNumbers(int x0, int N)
        {
            if (x0 < 0 || x0 >= m)
            {
                throw new ArgumentOutOfRangeException(nameof(x0), "x0 should be 0 ≤ x0 < m ");
            }
            if (N <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(N), "N should be > 0");
            }

            int xn = x0;
            var res = new List<int>();
            for (int i = 0; i < N; i++)
            {
                xn = LCG(xn);
                res.Add(xn);
            }

            return res;
        }
        public int GetPeriod(int x0)
        {
            int period = 0;
            var numbers = new List<int>();
            int xn = x0;
            do
            {
                numbers.Add(xn);
                xn = LCG(xn);
                period++;
            } while (x0 != xn);
            Console.WriteLine("\nPeriod numbers:");
            Printer.Print(numbers);
            return period;
        }

        private int LCG(int x0)
        {
            return (this.a * x0 + c) % m;
        }
    }
}
