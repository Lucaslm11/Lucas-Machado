using Assets.Scripts.Terminal.Nodes.FunctionTypes;
using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Functions
{
    public class IsLesserThan : BooleanFunctionNode
    {
        IntegerNode Val1 { get; set; }
        IntegerNode Val2 { get; set; }

        public override void Function()
        {
            Value = Val1 < Val2;
        }

    }
}
