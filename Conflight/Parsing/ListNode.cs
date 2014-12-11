using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    public class ListNode : ParseNode
    {
        public List<ParseNode> Value = new List<ParseNode>();
    }
}
