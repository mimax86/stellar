using System;
using System.Collections.Generic;
using System.Linq;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Zip(new[] {1, 2, 3}, new[] {4, 5, 6});
            Console.WriteLine("-------------------");
            Zip(new[] {1, 2, 3}, new[] {1, 2, 3});
            Console.WriteLine("-------------------");
            Zip(new[] {1, 2, 3, 4, 5, 6}, new[] {1, 2, 3});
            Console.WriteLine("-------------------");
            Console.ReadLine();
        }

        public static int[] Zip(int[] a, int[] b)
        {
            WriteArray("a", a);
            WriteArray("b", b);

            var items = new List<int>();
            for (int i = 0, j = 0; i < a.Length || j < b.Length;)
            {
                if (i == a.Length)
                {
                    items.Add(b[j]);
                    j++;
                }
                else if (j == b.Length)
                {
                    items.Add(a[i]);
                    i++;
                }
                else if (a[i] > b[j])
                {
                    items.Add(b[j]);
                    j++;
                }
                else if (a[i] < b[j])
                {
                    items.Add(a[i]);
                    i++;
                }
                else
                {
                    items.Add(a[i]);
                    i++;
                    j++;
                }
            }
            var result = items.ToArray();
            WriteArray("result", result);
            return result;
        }

        private static void WriteArray(string name, int[] array)
        {
            Console.Write($"{name}:");
            foreach (var item in array)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine();
        }
    }
}