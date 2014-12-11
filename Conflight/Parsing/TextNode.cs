using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    public class TextNode : ParseNode
    {
        public string Value;

        public override string ToString()
        {
            return LiteralContents;
        }
    }
}
