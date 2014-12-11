using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    class MappingNode : ParseNode
    {
        public TextNode Key;
        public ParseNode Value;
    }
}
