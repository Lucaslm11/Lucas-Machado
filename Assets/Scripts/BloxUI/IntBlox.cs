﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IntBlox : Blox, IBloxVariable
{
    private const string VAR_INPUT = "Var Input";
    private const string VALUE_INPUT = "Value Input";
    public string GetName()
    {
        return this.transform.Find(VAR_INPUT).gameObject.GetComponent<InputField>().text;
    }

    public string GetValue()
    {
        return this.transform.Find(VALUE_INPUT).gameObject.GetComponent<InputField>().text; 
    }

    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        // Only allows nesting a component to the side of this if it is an ArithmeticOperator
        return GameObjectHelper.HasComponent<ArithmeticOperatorBlox>(objectToNest) && BloxParams.Count == 0;
    }

    VariableType IBloxVariable.GetType()
    {
        return VariableType.INT;
    }
}
