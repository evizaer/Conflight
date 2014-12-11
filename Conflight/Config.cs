using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conflight
{
    public class Config
    {
        public static ParseNode Parse(string input)
        {
            Parser p = new Parser();
            Lexer l = new Lexer();

            return p.Parse(l.Lex(input));
        }

        public static ParseNode ParseFile(string filename)
        {
            string input = File.ReadAllText(filename);

            Parser p = new Parser();
            Lexer l = new Lexer();

            return p.Parse(l.Lex(input));
        }

    }
}
