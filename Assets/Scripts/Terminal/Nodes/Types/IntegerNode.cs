using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Types
{
    public class IntegerNode : CodeNode
    {
        
        public int Value { get; set; }
        public override object ReturnValue { get { return Value; } set { } }
        public IntegerNode(HighlightableButton highlightableBlox,int value=0):base(highlightableBlox)
        {
            Value = value;
        }
        public override bool OnBeforeChildNodesExecuteAction()
        { 
            return false;
        }


        public static int operator +(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value + val2.Value;
        }

        public static int operator -(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value - val2.Value;
        }
        public static int operator *(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value * val2.Value;
        }
        public static int operator /(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value / val2.Value;
        }
        public static bool operator>=(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value >= val2.Value;
        }

        public static bool operator<=(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value <= val2.Value;
        }

        public static bool operator>(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value > val2.Value;
        }
        public static bool operator<(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value < val2.Value;
        }

        public static bool operator==(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value == val2.Value;
        }

        public static bool operator !=(IntegerNode val1, IntegerNode val2)
        {
            return val1.Value != val2.Value;
        }


    }
}
