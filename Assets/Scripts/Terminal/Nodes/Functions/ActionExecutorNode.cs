using Assets.Scripts.Terminal.Nodes.FunctionTypes;
using Assets.Scripts.Terminal.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Terminal.Nodes.Functions
{
    public class ActionExecutorNode : VoidFunctionNode
    {
        Func<bool> WaitWhileCondition;
        Action TaskToExecute;

        /// <summary>
        /// Constructor for ActionExecutorNode
        /// waitWhileCondition will be called "endlessly" until it returns false
        /// </summary>
        /// /// <param name="taskToExecute"> A task that is called to execute </param>
        /// <param name="waitWhileCondition"> A function that is called to check if it is executed </param>
        public ActionExecutorNode(Action taskToExecute,Func<bool> waitWhileCondition)
        {
            TaskToExecute = taskToExecute;
            WaitWhileCondition = waitWhileCondition;
        }

        public override void Function()
        {
            TaskToExecute();
            TaskHelper.WaitWhile(WaitWhileCondition,25, 5000).Wait();
        }

    }
}
