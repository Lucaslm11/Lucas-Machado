﻿using Assets.Scripts.Terminal.Nodes.FunctionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes
{ 
    public class IfNode : CodeNode
    {
        private bool execute = true;

        BooleanFunctionNode Condition { get; set; }

        public override bool CanHaveChildren { get { return true; } }

        public override bool OnBeforeChildNodesExecuteAction()
        {
            bool evalResult = Condition.Value && execute;
            execute = !execute; //This helps to prevent the continuation of the loop in CodeNode, and to execute this verification again when passing over this a next time
            return evalResult;
        }

    }
}
