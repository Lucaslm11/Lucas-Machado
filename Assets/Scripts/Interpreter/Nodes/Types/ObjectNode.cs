using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Types
{
    public class ObjectNode : CodeNode
    {
        public ICodeNode Value { get; set; }
        public override object ReturnValue { get { return Value; } set { } }
        public ObjectNode(ICodeNode node, HighlightableButton highlightableBlox):base(highlightableBlox)
        {
            Value = node; 
        }
        public override bool OnBeforeChildNodesExecuteAction()
        { 
            return false;
        } 

    }
}
