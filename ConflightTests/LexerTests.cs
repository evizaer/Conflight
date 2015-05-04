using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Conflight;
using System.Collections.Generic;
using System.Linq;

namespace ConflightTests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void LexEmptyInput()
        {
            Assert.IsTrue(Lexer.Lex("").Count == 0);
        }

        [TestMethod]
        public void LexEmptyDict()
        {
            string message;
            Assert.IsTrue(AreTokenListsEqual(Lexer.Lex("{}"), new List<Token>
            {
                new Token { ColumnNumber = 0, LineNumber = 1, Contents = "{", Type = TokenType.DictStart },
                new Token { ColumnNumber = 1, LineNumber = 1, Contents = "}", Type = TokenType.DictEnd }
            }, out message), message);
        }

        [TestMethod]
        public void LexDict()
        {
            string message;
            Assert.IsTrue(AreTokenListsEqual(Lexer.Lex("{Test: 10\n\"OtherKey\":   Free, Dog:true}"), new List<Token>
            {
                new Token { ColumnNumber = 0, LineNumber = 1, Contents = "{", Type = TokenType.DictStart },
                new Token { ColumnNumber = 1, LineNumber = 1, Contents = "Test", Type = TokenType.Text },
                new Token { ColumnNumber = 5, LineNumber = 1, Contents = ":", Type = TokenType.MappingDelimiter },
                new Token { ColumnNumber = 7, LineNumber = 1, Contents = "10", Type = TokenType.Text },
                new Token { ColumnNumber = 9, LineNumber = 1, Contents = "\n", Type = TokenType.ListDelimiter },
                new Token { ColumnNumber = 0, LineNumber = 2, Contents = "OtherKey", Type = TokenType.Text },
                new Token { ColumnNumber = 10, LineNumber = 2, Contents = ":", Type = TokenType.MappingDelimiter },
                new Token { ColumnNumber = 14, LineNumber = 2, Contents = "Free", Type = TokenType.Text },
                new Token { ColumnNumber = 18, LineNumber = 2, Contents = ",", Type = TokenType.ListDelimiter },
                new Token { ColumnNumber = 20, LineNumber = 2, Contents = "Dog", Type = TokenType.Text },
                new Token { ColumnNumber = 23, LineNumber = 2, Contents = ":", Type = TokenType.MappingDelimiter },
                new Token { ColumnNumber = 24, LineNumber = 2, Contents = "true", Type = TokenType.Text },
                new Token { ColumnNumber = 28, LineNumber = 2, Contents = "}", Type = TokenType.DictEnd }
            }, out message), message);
        }

        [TestMethod]
        public void LexEmptyList()
        {
            string message;
            Assert.IsTrue(AreTokenListsEqual(Lexer.Lex("[]"), new List<Token>
            {
                new Token { ColumnNumber = 0, LineNumber = 1, Contents = "[", Type = TokenType.ListStart },
                new Token { ColumnNumber = 1, LineNumber = 1, Contents = "]", Type = TokenType.ListEnd }
            }, out message), message);
        }

        [TestMethod]
        public void LexList()
        {
            string message;
            Assert.IsTrue(AreTokenListsEqual(Lexer.Lex("[Test, \"Test2\"\n30,11.1]"), new List<Token>
            {
                new Token { ColumnNumber = 0, LineNumber = 1, Contents = "[", Type = TokenType.ListStart },
                new Token { ColumnNumber = 1, LineNumber = 1, Contents = "Test", Type = TokenType.Text },
                new Token { ColumnNumber = 5, LineNumber = 1, Contents = ",", Type = TokenType.ListDelimiter },
                new Token { ColumnNumber = 7, LineNumber = 1, Contents = "Test2", Type = TokenType.Text },
                new Token { ColumnNumber = 14, LineNumber = 1, Contents = "\n", Type = TokenType.ListDelimiter },
                new Token { ColumnNumber = 0, LineNumber = 2, Contents = "30", Type = TokenType.Text },
                new Token { ColumnNumber = 2, LineNumber = 2, Contents = ",", Type = TokenType.ListDelimiter },
                new Token { ColumnNumber = 3, LineNumber = 2, Contents = "11.1", Type = TokenType.Text },
                new Token { ColumnNumber = 7, LineNumber = 2, Contents = "]", Type = TokenType.ListEnd }
            }, out message), message);
        }

        public bool AreTokenListsEqual(List<Token> tokens1, List<Token> tokens2, out string message)
        {
            message = "";

            if (tokens1.Count != tokens2.Count) 
            {
                message = "Lists have unequal token count.";
                return false;
            }

            var firstMismatchedTokens = tokens1.Zip(tokens2, (t1, t2) => Tuple.Create(t1, t2)).SkipWhile(t => AreTokensEqual(t.Item1, t.Item2)).FirstOrDefault();

            if (firstMismatchedTokens != null)
            {
                message = string.Format("Token mismatch: [{0}] is not [{1}]", firstMismatchedTokens.Item1, firstMismatchedTokens.Item2);
                return false;
            }

            return true;
        }

        public bool AreTokensEqual(Token t1, Token t2)
        {
            return t1.ColumnNumber == t2.ColumnNumber
                && t1.LineNumber == t2.LineNumber
                && t1.Contents == t2.Contents;
        }
    }
}
