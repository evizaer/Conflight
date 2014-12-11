using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    public abstract class ParseNode
    {
        public string LiteralContents;
        public List<ParseNode> Children = new List<ParseNode>();
    }
}
