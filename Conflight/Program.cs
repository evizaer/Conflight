using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    class Program
    {
        private class T
        {
            public int A { get; set; }
            public string B { get; set; }
            public double? C { get; set; }
            public List<int> D { get; set; }
            public T2 E { get; set; }
            public E0 F { get; set; }
            public Dictionary<E0, int> E0L { get; set; } 
        }

        private class T2
        {
            public string A { get; set; }
            public Dictionary<string, int> D { get; set; }
            public bool F { get; set; }
        }

        private enum E0 { V1, V2, V3 }

        static void Main(string[] args)
        {
            //string test1 = "[1, [123, 12, 3, 46 ,3], 000]";
            //string test2 = "{test: 1, test2: [2, 1, 3]}";
            //string test3 = "[a, b, {test: [1, 2], test2: \"other 123, super!\"}, [1, 2]]";
            //string test4 = "{D: [1, 10, -3, 1], A: 10, E: {A: dog, D: {test: 2, test3: 3}, meta: 10}, B: \"very test\"}";
            //string test5 = "[{E0L: {V1: 10, V3: -1}, A: 10, B: base dog, E: {F: true}}, {A: 1, C: 10.231}, {F: V3, D: [1, 0, 0, 1]}]";
            string test = "[{\n\tA: 10\n\tB: dog\n}\n{\n\tA: 5\n\tF: V3, C: 1.1}]";


            /*
            Lexer l = new Lexer();
            var tokens = l.Lex(test5);
            var root = (new Parser()).Parse(tokens);

            Console.WriteLine(string.Join(", ", tokens.Select(t => t.ToString())));
            */

            var o = ObjectLoader.LoadInstanceList<T>(test);

            Console.ReadLine();
        }
    }
}
