using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes
{
    public class LoopRangeNode : CodeNode
    {
        IntegerNode counter { get; set; }
        public int min { get; set; }
        public int max { get; set; }

        public LoopRangeNode()
        {
            counter = new IntegerNode();
        }
 
        public override void OnAfterChildNodesExecuteAction()
        {
            counter.Value = counter.Value + 1;
        }

        public override void OnBeforeChildNodesExecuteAction()
        {

        }

       
    }
}
