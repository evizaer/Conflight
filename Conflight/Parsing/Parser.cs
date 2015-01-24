using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    public class Parser
    {
        public static ParseNode Parse(List<Token> tokens)
        {
            int i = 0;
            return Process(tokens, ref i);
        }

        delegate ParseNode TokenProcessor(List<Token> tokens, ref int i);

        static Dictionary<TokenType, TokenProcessor> processors = new Dictionary<TokenType, TokenProcessor> {
            {TokenType.ListStart, ProcessList},
            {TokenType.DictStart, ProcessDict},
            {TokenType.Text, ProcessText}
        };

        public static ParseNode Process(List<Token> tokens, ref int i) 
        {
            if (i >= tokens.Count)
            {
                return null;
            }

            ParseNode result = null;

            Token cur = tokens[i];

            result = processors[cur.Type](tokens, ref i);

            return result;
        }

        public static ParseNode ProcessText(List<Token> tokens, ref int i)
        {
            if (tokens[i].Type == TokenType.Text)
            {
                return new TextNode { LiteralContents = tokens[i].Contents, Value = tokens[i++].Contents };
            }
            else
            {
                throw new Exception("Expected text node but got " + tokens[i].Contents);
            }
        }

        public static ParseNode ProcessList(List<Token> tokens, ref int i)
        {
            ListNode result = new ListNode();

            ++i;
            while (tokens[i].Type == TokenType.ListDelimiter) ++i;

            // ++i because we want to increment past the list opener AND leave i at the index right AFTER the list ends.
            while (i < tokens.Count && tokens[i].Type != TokenType.ListEnd)
            {
                ParseNode candidate = Process(tokens, ref i);

                if (candidate != null)
                {
                    result.Children.Add(candidate);
                }
                else
                {
                    throw new Exception("Found an unexpected token in a list while processing! " + tokens[i].Contents);
                }

                while (tokens[i].Type == TokenType.ListDelimiter) ++i;
            }

            foreach (ParseNode p in result.Children)
            {
                result.Value.Add(p);
            }

            // Advance one past the end of the list.
            ++i;

            return result;
        }

        public static ParseNode ProcessDict(List<Token> tokens, ref int i)
        {
            DictNode result = new DictNode();

            List<MappingNode> mappings = new List<MappingNode>();

            ++i;
            while (tokens[i].Type == TokenType.ListDelimiter) ++i;

            // ++i because we want to increment past the list opener AND leave i at the index right AFTER the list ends.
            while (i < tokens.Count && tokens[i].Type != TokenType.DictEnd)
            {
                mappings.Add(ProcessMapping(tokens, ref i));

                while (tokens[i].Type == TokenType.ListDelimiter) ++i;
            }

            foreach (MappingNode node in mappings)
            {
                result.Value.Add(node.Key.LiteralContents, node.Value);
            }

            ++i;
            return result;
        }

        private static MappingNode ProcessMapping(List<Token> tokens, ref int i)
        {
            MappingNode result = null;

            ParseNode keyNode = Process(tokens, ref i);

            if (keyNode != null && keyNode is TextNode)
            {
                if (tokens[i].Type == TokenType.MappingDelimiter)
                {
                    ++i;
                    ParseNode valueNode = Process(tokens, ref i);

                    if (valueNode != null)
                    {
                        result = new MappingNode { Key = keyNode as TextNode, Value = valueNode };
                    }
                    else
                    {
                        throw new Exception("Could not parse value node!");
                    }
                }
                else
                {
                    throw new Exception("Mapping delimiter must follow key node.");
                }
            }
            else
            {
                throw new Exception("Found an unexpected token in a dict key! " + tokens[i].Contents);
            }

            return result;
        }
    }
}
