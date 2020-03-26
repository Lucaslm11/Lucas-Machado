using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes
{
    public class RootNode : CodeNode
    {
        private bool execute = true;
        public override bool CanHaveChildren { get { return true; } }

        public override bool OnBeforeChildNodesExecuteAction()
        {
            bool executeThisTime = execute;
            execute = false; //Root node is only intented to execute once
            return executeThisTime;
        }
    }
}
