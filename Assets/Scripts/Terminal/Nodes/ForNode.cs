using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes
{
    public class ForNode : CodeNode
    {
        public override bool CanHaveChildren { get { return true; } }
        IntegerNode Counter { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public ForNode(HighlightableButton highlightableBlox, IntegerNode counter, int min, int max) : base(highlightableBlox)
        {
            Counter = counter;
            Min = min;
            Max = max;
        }
 
        public override void OnAfterChildNodesExecuteAction()
        {
            Counter.Value = Counter.Value + 1;
        }

        public override bool OnBeforeChildNodesExecuteAction()
        {
            return Counter.Value >= Min && Counter.Value <= Max;
        }

       
    }
}
