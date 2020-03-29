using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Functions;
using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicalOperatorBlox : OperatorBlox, ICompilableBlox
{


    #region Compiler helpers
    public List<BloxValidationError> Validate()
    {
        return ValidateOperator();
    }

    public LogicalOperationNode.LogicalOperation GetOperation(string op)
    {
        LogicalOperationNode.LogicalOperation operation = LogicalOperationNode.LogicalOperation.BIGGER_THAN;
        switch (op)
        {
            case ">":
                operation = LogicalOperationNode.LogicalOperation.BIGGER_THAN;
                break;
            case "<":
                operation = LogicalOperationNode.LogicalOperation.LESSER_THAN;
                break;
            case "==":
                operation = LogicalOperationNode.LogicalOperation.EQUALS;
                break;
            case "!=":
                operation = LogicalOperationNode.LogicalOperation.DIFFERENT;
                break;
            default:
                throw new System.Exception("Invalid operator " + op);
                break;
        }
        return operation;
    }



    public void ToNodes(ICodeNode parentNode)
    {
        RootNode rootNode = parentNode.GetRootNode();
        LogicalOperationNode logicOpNode = CreateNode(rootNode);
        parentNode.AddChildNode(logicOpNode);
    }

    public LogicalOperationNode CreateNode(RootNode rootNode)
    {
        HighlightableButton highlightableButton = (GameObjectHelper.HasComponent<HighlightableButton>(this.gameObject)) ? this.GetComponent<HighlightableButton>() : null;
        IntegerNode field1 = GetField1Node(rootNode);
        IntegerNode field2 = GetField2Node(rootNode);
        // Converts the Logical operator choosen to the proper enum
        LogicalOperationNode.LogicalOperation operation = GetOperation(GetOperator());
        LogicalOperationNode logicOpNode = new LogicalOperationNode(highlightableButton, field1, field2, operation);
        return logicOpNode;
    }

    #endregion
}
