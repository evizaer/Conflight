using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conflight
{
    public class ParseException : Exception
    {
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }

        public ParseException(int line, int col, string message) : base(message)
        {
            LineNumber = line;
            ColumnNumber = col;
        }

        public ParseException(Token t, string message) : base(message)
        {
            LineNumber = t.LineNumber;
            ColumnNumber = t.ColumnNumber;
        }

        public override string ToString()
        {
            return string.Join(": ", new[] 
            { 
                string.Format("Line {0}, Col {1}", LineNumber, ColumnNumber),
                Message
            });
        }
    }
}
