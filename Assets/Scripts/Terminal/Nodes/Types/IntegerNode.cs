using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Types
{
    public class IntegerNode : CodeNode
    {
        public int Value { get; set; }
        public IntegerNode()
        {
            Value = 0;
        }
        public override void OnBeforeChildNodesExecuteAction()
        {
            ReturnValue = Value;
        }
        public override void OnAfterChildNodesExecuteAction()
        {
        }

    }
}
