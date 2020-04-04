using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Types
{
    public class BooleanNode : CodeNode
    {
        public bool Value { get; set; }
        public override object ReturnValue { get { return Value; } set { } }
        public BooleanNode(HighlightableButton highlightableBlox,bool value):base(highlightableBlox)
        {
            Value = value; 
        }
        public override bool OnBeforeChildNodesExecuteAction()
        { 
            return false;
        } 

    }
}
