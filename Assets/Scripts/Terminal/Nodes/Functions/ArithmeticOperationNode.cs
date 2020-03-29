using Assets.Scripts.Terminal.Nodes.FunctionTypes;
using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Functions
{
    public class ArithmeticOperationNode : IntegerFunctionNode
    {
        public enum ArithmeticOperation
        {
            SUM,
            SUBTRACT,
            MULTIPLY,
            DIVIDE,

        }

        IntegerNode Val1 { get; set; }
        IntegerNode Val2 { get; set; }
        ArithmeticOperation Operation { get; set; }


        public ArithmeticOperationNode(HighlightableButton highlightableBlox, IntegerNode val1, IntegerNode val2, ArithmeticOperation operation):base(highlightableBlox)
        {
            Val1 = val1;
            Val2 = val2;
            Operation = operation;
        }

        public override void Function()
        {
            switch (Operation)
            {
                case ArithmeticOperation.SUM:
                    Value = Val1 + Val2;
                    break;
                case ArithmeticOperation.SUBTRACT:
                    Value = Val1 - Val2;
                    break;
                case ArithmeticOperation.MULTIPLY:
                    Value = Val1 * Val2;
                    break;
                case ArithmeticOperation.DIVIDE:
                    Value = Val1 / Val2;
                    break;
            }
        }

    }
}
