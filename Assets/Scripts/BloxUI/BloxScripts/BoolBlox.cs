using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Functions;
using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoolBlox : ABlox, IBloxVariable, ICompilableBlox
{
    [SerializeField] InputField VarNameField;
   // [SerializeField] Button ValueField;
    [SerializeField] TrueFalseTextSwitcher ValueField;

    public string GetName()
    {
        return VarNameField.text;
    }

    public string GetValue()
    {
        return ValueField.state.ToString();
    }
    
    public bool GetValueAsBoolean()
    {
        return bool.Parse(ValueField.state.ToString());
    }

    public void ToNodes(ICodeNode parentNode)
    {
        RootNode rootNode = parentNode.GetRootNode();
        //Checks if this blox has a param if it has creates an LogicalOperationNode instead of an Integer node
        if (this.BloxParams.Count > 0)
        {
            LogicalOperatorBlox logicOpBlox = BloxParams[0].GetComponent<LogicalOperatorBlox>();
            LogicalOperationNode logicOpNode = logicOpBlox.CreateNode(rootNode);
            logicOpNode.NodeName = this.GetName();
            parentNode.AddChildNode(logicOpNode);
        }
        else
        {
            BooleanNode boolNode = new BooleanNode(GetValueAsBoolean());
            boolNode.NodeName = this.GetName();
            parentNode.AddChildNode(boolNode);
        }
    }

    public List<BloxValidationError> Validate()
    {
        throw new System.NotImplementedException();
    }

    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        // Only allows nesting a component to the side of this if it is an LogicalOperator
        return GameObjectHelper.HasComponent<LogicalOperatorBlox>(objectToNest) && BloxParams.Count == 0;
    }

    VariableType IBloxVariable.GetType()
    {
        return VariableType.BOOL;
    }
}
