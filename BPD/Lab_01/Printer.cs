using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_01
{
    public static class Printer
    {
        public static void Print(List<int> nums)
        {
            string text = "{" + String.Join(", ", nums) + "}";
            Console.WriteLine(text);
        }
    }
}
