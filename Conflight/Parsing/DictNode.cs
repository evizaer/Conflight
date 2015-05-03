using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conflight
{
    public class DictNode : ParseNode
    {
        public Dictionary<string, ParseNode> Mappings = new Dictionary<string,ParseNode>();

        public double GetDoubleValue(string key)
        {
            if (Mappings[key] is TextNode)
            {
                return Convert.ToDouble((Mappings[key] as TextNode).Value);
            }
            else
            {
                throw new Exception(string.Format("Dict value at key [{0}] was not a text node and we attempted to parse it as a double.", key));
            }
        }

        public string GetStringValue(string key)
        {
            if (Mappings[key] is TextNode)
            {
                return (Mappings[key] as TextNode).Value;
            }
            else
            {
                throw new Exception(string.Format("Dict value at key [{0}] was not a text node and we attempted to parse it as a string.", key));
            }
        }

        public int GetIntValue(string key)
        {
            if (Mappings[key] is TextNode)
            {
                return Convert.ToInt32((Mappings[key] as TextNode).Value);
            }
            else
            {
                throw new Exception(string.Format("Dict value at key [{0}] was not a text node and we attempted to parse it as a integer.", key));
            }
        }

        public List<string> GetStringListValue(string key)
        {
            List<string> result = new List<string>();

            if (Mappings[key] is ListNode)
            {
                foreach (ParseNode p in (Mappings[key] as ListNode).Value)
                {
                    if (p is TextNode)
                    {
                        result.Add((p as TextNode).Value);
                    }
                    else
                    {
                        throw new Exception(string.Format("Found a non-text node in a the list at key [{0}] so we can't return it as a list.", key));
                    }
                }
            }

            return result;
        }
    }
}
