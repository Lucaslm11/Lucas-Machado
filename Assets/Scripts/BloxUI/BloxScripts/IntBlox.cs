using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IntBlox : ABlox, IBloxVariable
{
    [SerializeField] InputField VarNameField;
    [SerializeField] InputField ValueField;
     
    public string GetName()
    {
        return VarNameField.text;
    }

    public string GetValue()
    {
        return ValueField.text;
    }
    VariableType IBloxVariable.GetType()
    {
        return VariableType.INT;
    }

    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        // Only allows nesting a component to the side of this if it is an ArithmeticOperator
        return GameObjectHelper.HasComponent<ArithmeticOperatorBlox>(objectToNest) && BloxParams.Count == 0;
    }


}
