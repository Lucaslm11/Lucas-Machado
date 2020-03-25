using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoolBlox : ABlox, IBloxVariable
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
 

    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        // Only allows nesting a component to the side of this if it is an ArithmeticOperator
        return GameObjectHelper.HasComponent<ArithmeticOperatorBlox>(objectToNest) && BloxParams.Count == 0;
    }

    VariableType IBloxVariable.GetType()
    {
        return VariableType.BOOL;
    }
}
