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
        HighlightableButton highlightableButton = (GameObjectHelper.HasComponent<HighlightableButton>(this.gameObject)) ? this.GetComponent<HighlightableButton>() : null;
        //Checks if this blox has a param if it has creates an LogicalOperationNode instead of an BOOLeger node
        if (this.BloxParams.Count > 0)
        {
            LogicalOperatorBlox logicOpBlox = BloxParams[0].GetComponent<LogicalOperatorBlox>();
            LogicalOperationNode logicOpNode = logicOpBlox.CreateNode(rootNode);
            logicOpNode.NodeName = this.GetName();
            parentNode.AddChildNode(logicOpNode);
        }
        else
        {
            BooleanNode boolNode = new BooleanNode(highlightableButton, GetValueAsBoolean());
            boolNode.NodeName = this.GetName();
            parentNode.AddChildNode(boolNode);
        }
    }

    public List<BloxValidationError> Validate()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();

        // If BOOLBlox has no name
        if (string.IsNullOrWhiteSpace(GetName()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.BOOL_BLOX_NO_NAME,
                TargetBlox = this
            });
        }

        // If a variable with the same name exists, no matter the type
        if (VariableExistsInBloxScope(this, GetName()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.BLOX_REPEATED_NAME,
                TargetBlox = this
            });
        }

        // If BOOLBloxHas no value
        if (BloxParams.Count == 0 && string.IsNullOrWhiteSpace(GetValue()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.BOOL_BLOX_NO_VALUE,
                TargetBlox = this
            });
        }
         
        //If it has params, validates the param
        errors.AddRange(ValidateBloxList(BloxParams));

        return errors;
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
