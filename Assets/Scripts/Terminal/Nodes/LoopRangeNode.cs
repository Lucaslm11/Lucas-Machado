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
        public override bool CanHaveChildren { get { return true; } }
        IntegerNode counter { get; set; }
        public int min { get; set; }
        public int max { get; set; }

        public LoopRangeNode(IntegerNode counter, int min, int max)
        {
            counter = new IntegerNode(min);
        }
 
        public override void OnAfterChildNodesExecuteAction()
        {
            counter.Value = counter.Value + 1;
        }

        public override bool OnBeforeChildNodesExecuteAction()
        {
            return counter.Value >= min && counter.Value <= max;
        }

       
    }
}
