using Assets.Scripts.Terminal.Nodes.FunctionTypes;
using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Functions
{
    public class LogicalOperationNode : BooleanFunctionNode
    {
        public enum LogicalOperation
        {
            LESSER_THAN,
            BIGGER_THAN,
            EQUALS,
            DIFFERENT,

        }

        IntegerNode Val1 { get; set; }
        IntegerNode Val2 { get; set; }
        LogicalOperation Operation { get; set; }


        public LogicalOperationNode(HighlightableButton highlightableBlox, IntegerNode val1, IntegerNode val2, LogicalOperation operation) : base(highlightableBlox)
        {
            Val1 = val1;
            Val2 = val2;
            Operation = operation;
        }

        public override void Function()
        {
            switch (Operation)
            {
                case LogicalOperation.BIGGER_THAN:
                    Value = Val1 > Val2;
                    break;
                case LogicalOperation.LESSER_THAN:
                    Value = Val1 < Val2;
                    break;
                case LogicalOperation.EQUALS:
                    Value = Val1 == Val2;
                    break;
                case LogicalOperation.DIFFERENT:
                    Value = Val1 != Val2;
                    break;
            }
        }

    }
}
