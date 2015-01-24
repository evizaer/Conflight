using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Conflight
{
    public enum TokenType { Text, DictStart, DictEnd, ListStart, ListEnd, MappingDelimiter, ListDelimiter }

    public class Lexer
    {
        private static Dictionary<char, TokenType> CharTokenTypes = new Dictionary<char, TokenType> {
            {'[', TokenType.ListStart},
            {']', TokenType.ListEnd},
            {'{', TokenType.DictStart},
            {'}', TokenType.DictEnd},
            {':', TokenType.MappingDelimiter},
            {',', TokenType.ListDelimiter},
            {'\n', TokenType.ListDelimiter}
        };

        public static List<Token> Lex(string input)
        {
            var result = new List<Token>();

            int i = 0;

            while (i < input.Length)
            {
                char c = input[i];

                var t = new Token();

                bool shouldAddToken = true;
                if (CharTokenTypes.ContainsKey(c))
                {
                    t.Type = CharTokenTypes[c];
                    t.Contents = c.ToString();

                    ++i;
                }
                else if (Regex.IsMatch(c.ToString(), @"\s"))
                {
                    ++i;
                    shouldAddToken = false;
                }
                else if (Regex.IsMatch(c.ToString(), @"-|\w|\.")) 
                {
                    t = ReadText(input, ref i);
                }
                else if (c == '"')
                {
                    t = ReadQuotedText(input, ref i);
                }
                else
                {
                    ++i;
                    shouldAddToken = false;
                    Console.WriteLine("Couldn't handle character " + c);
                }

                if (shouldAddToken)
                {
                    result.Add(t);
                }
            }

            Token prev = null;
            foreach (Token t in result)
            {
                t.Previous = prev;

                if (prev != null)
                {
                    prev.Next = t;
                }

                prev = t;
            }

            return result;
        }

        private static Token ReadQuotedText(string input, ref int i)
        {
            Match m = Regex.Match(input.Substring(i), "\"[^\"]+\"");

            if (m.Success && m.Index == 0)
            {
                i += m.Length;
                return new Token { Contents = m.Value.Substring(1, m.Value.Length - 2) };
            }
            else
            {
                throw new Exception("Couldn't find end of quoted string!");
            }
        }

        private static Token ReadText(string input, ref int i)
        {
            Match m = Regex.Match(input.Substring(i), @"^-?(\w|\.| )+");

            if (m.Success && m.Index == 0)
            {
                i += m.Length;
                return new Token { Contents = m.Value, Type = TokenType.Text };
            }
            else
            {
                throw new Exception("ReadText called on bogus index " + i);
            }
        }
    }

}
