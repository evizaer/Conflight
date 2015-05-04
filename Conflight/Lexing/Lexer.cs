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

            int lineNumber = 1;
            int lineStartCharIndex = 0;
            int charIndex = 0;

            while (charIndex < input.Length)
            {
                char c = input[charIndex];

                var t = new Token() { LineNumber = lineNumber, ColumnNumber = charIndex - lineStartCharIndex };

                bool shouldAddToken = true;
                if (CharTokenTypes.ContainsKey(c))
                {
                    t.Type = CharTokenTypes[c];
                    t.Contents = c.ToString();

                    ++charIndex;
                }
                else if (Regex.IsMatch(c.ToString(), @"\s"))
                {
                    ++charIndex;
                    shouldAddToken = false;
                }
                else if (Regex.IsMatch(c.ToString(), @"-|\w|\.")) 
                {
                    ReadText(input, ref charIndex, ref t);
                }
                else if (c == '"')
                {
                    ReadQuotedText(input, ref charIndex, ref t);
                }
                else
                {
                    ++charIndex;
                    shouldAddToken = false;
                    Console.WriteLine("Couldn't handle character " + c);
                }

                if (c == '\n')
                {
                    ++lineNumber;
                    lineStartCharIndex = charIndex;
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

        private static void ReadQuotedText(string input, ref int i, ref Token t)
        {
            Match m = Regex.Match(input.Substring(i), "\"[^\"]+\"");

            if (m.Success && m.Index == 0)
            {
                i += m.Length;
                t.Type = TokenType.Text;
                t.Contents = m.Value.Substring(1, m.Value.Length - 2);
            }
            else
            {
                throw new Exception("Couldn't find end of quoted string!");
            }
        }

        private static void ReadText(string input, ref int i, ref Token t)
        {
            Match m = Regex.Match(input.Substring(i), @"^-?(\w|\.| )+");

            if (m.Success && m.Index == 0)
            {
                i += m.Length;
                t.Contents = m.Value; 
                t.Type = TokenType.Text;
            }
            else
            {
                throw new Exception();
            }
        }
    }

}
