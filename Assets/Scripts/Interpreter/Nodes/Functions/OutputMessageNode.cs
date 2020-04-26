using Assets.Scripts.Terminal.Nodes.FunctionTypes;
using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Functions
{
    public class OutputMessageNode : VoidFunctionNode
    {
        Action<string> MessageInput;
        List<ObjectNode> VariableNodes = new List<ObjectNode>();


        /// <summary>
        /// Constructor for ActionExecutorNode
        /// waitWhileCondition will be called "endlessly" until it returns false
        /// </summary>
        /// /// <param name="taskToExecute"> A task that is called to execute </param> 
        public OutputMessageNode(HighlightableButton highlightableBlox, Action<string> messageInput, List<ObjectNode> variables ) : base(highlightableBlox)
        {
            MessageInput = messageInput;
            VariableNodes = variables ?? new List<ObjectNode>();
        }

        public override void Function()
        {
            try
            {
                string message = string.Join(" ",VariableNodes.Select(v => GetValueFromVariable(v)));
                MessageInput(message); 
            }catch(CodeBloxException ex)
            {
                ex.blox = this.NodeBlox;
                throw ex;
            }
        }
        string GetValueFromVariable(ObjectNode variable)
        {
            return variable == null ? string.Empty : variable.Value.ReturnValue.ToString();
        }

    }
}
