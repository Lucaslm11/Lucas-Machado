using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Types
{
    public class StringNode : CodeNode
    {
        public String Value { get; set; }
        public override object ReturnValue { get { return Value; } set { } }
        public StringNode(HighlightableButton highlightableBlox,string value):base(highlightableBlox)
        {
            Value = value; 
        }
        public override bool OnBeforeChildNodesExecuteAction()
        { 
            return false;
        } 

    }
}
