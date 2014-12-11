using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Contents;

        public Token Next, Previous;

        public override string ToString()
        {
            return string.Format("['{0}' : {1}]", Contents, Type);
        }
    }
}
