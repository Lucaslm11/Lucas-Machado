using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Functions;
using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArithmeticOperatorBlox : OperatorBlox, ICompilableBlox
{


    #region Compiler helpers
    public List<BloxValidationError> Validate()
    {
        return ValidateOperator();
    }

    public ArithmeticOperationNode.ArithmeticOperation GetOperation(string op)
    {
        ArithmeticOperationNode.ArithmeticOperation operation = ArithmeticOperationNode.ArithmeticOperation.SUM;
        switch (op)
        {
            case "+":
                operation = ArithmeticOperationNode.ArithmeticOperation.SUM;
                break;
            case "-":
                operation = ArithmeticOperationNode.ArithmeticOperation.SUBTRACT;
                break;
            case "*":
                operation = ArithmeticOperationNode.ArithmeticOperation.MULTIPLY;
                break;
            case "/":
                operation = ArithmeticOperationNode.ArithmeticOperation.DIVIDE;
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
        ArithmeticOperationNode arithOpNode = CreateNode(rootNode);
        parentNode.AddChildNode(arithOpNode);
    }

    public ArithmeticOperationNode CreateNode(RootNode rootNode)
    {
        IntegerNode field1 = GetField1Node(rootNode);
        IntegerNode field2 = GetField2Node(rootNode);
        // Converts the arithmetic operator choosen to the proper enum
        ArithmeticOperationNode.ArithmeticOperation operation = GetOperation(GetOperator());
        ArithmeticOperationNode arithOpNode = new ArithmeticOperationNode(field1, field2, operation);
        return arithOpNode;
    }

    #endregion
}
