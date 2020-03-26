using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Functions;
using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IntBlox : ABlox, IBloxVariable, ICompilableBlox
{
    [SerializeField] InputField VarNameField;
    [SerializeField] InputField ValueField;

    #region IBloxVariable region
    public string GetName()
    {
        return VarNameField.text;
    }

    public string GetValue()
    {
        return ValueField.text;
    }

    public int GetValueAsInt()
    {
        return int.Parse(GetValue());
    }

    VariableType IBloxVariable.GetType()
    {
        return VariableType.INT;
    }
    #endregion



    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        // Only allows nesting a component to the side of this if it is an ArithmeticOperator
        return GameObjectHelper.HasComponent<ArithmeticOperatorBlox>(objectToNest) && BloxParams.Count == 0;
    }

    public List<BloxValidationError> Validate()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();
 

        //If it has params, validates the param

        // If IntBlox has no name
        if (string.IsNullOrWhiteSpace(GetName()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxErrors.INT_BLOX_NO_NAME,
                TargetBlox = this
            });
        }

        // If a variable with the same name exists, no matter the type
        if(VariableExistsInBloxScope(this, GetName()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxErrors.BLOX_REPEATED_NAME,
                TargetBlox = this
            });
        }


        // If IntBloxHas no value
        if (BloxParams.Count == 0 && string.IsNullOrWhiteSpace(GetValue())){
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxErrors.INT_BLOX_NO_NAME,
                TargetBlox = this
            });
        }

        if(BloxParams.Count > 0 && GameObjectHelper.CanBeCastedAs<ICompilableBlox>(BloxParams[0]))
        {
            errors.AddRange(((ICompilableBlox)BloxParams[0]).Validate());
        }


        return errors;
    }
     
    public void ToNodes(ICodeNode parentNode)
    {
        RootNode rootNode = parentNode.GetRootNode();
        //Checks if this blox has a param if it has creates an ArithmeticOperationNode instead of an Integer node
        if(this.BloxParams.Count > 0)
        {
            ArithmeticOperatorBlox arithOpBlox = BloxParams[0].GetComponent<ArithmeticOperatorBlox>();
            ArithmeticOperationNode arithOpNode = arithOpBlox.CreateNode(rootNode);
            arithOpNode.NodeName = this.GetName();
            parentNode.AddChildNode(arithOpNode);
        }
        else
        {
            IntegerNode intNode = new IntegerNode(GetValueAsInt());
            intNode.NodeName = this.GetName();
            parentNode.AddChildNode(intNode);
        }
    }
}
