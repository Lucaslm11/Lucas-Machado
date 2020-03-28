using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.FunctionTypes
{
    public class VoidFunctionNode : CodeNode
    {
        public VoidFunctionNode( ) { 
             
        }


        public override bool OnBeforeChildNodesExecuteAction()
        {
            Function();
            return false;
        }

        /// <summary>
        /// The different overrides of this will implement what is needed to execute a functions
        /// </summary>
        public virtual void Function()
        {

        }
    }
}
