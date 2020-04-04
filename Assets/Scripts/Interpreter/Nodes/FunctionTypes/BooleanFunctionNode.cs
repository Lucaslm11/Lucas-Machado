using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.FunctionTypes
{
    public class BooleanFunctionNode : BooleanNode
    {
        public BooleanFunctionNode(HighlightableButton highlightableBlox, bool value = false):base(highlightableBlox,value) { 
             
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
