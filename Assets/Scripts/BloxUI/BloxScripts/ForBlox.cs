using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForBlox : ABlox, IBloxVariable
{
    [SerializeField] InputField CounterNameField;
    [SerializeField] InputField FromField;
    [SerializeField] InputField ToField;

    public string GetName()
    {
        return CounterNameField.text;
    }

    /// <summary>
    /// Only returns the starting value
    /// </summary>
    /// <returns></returns>
    public string GetValue()
    {
        return FromField.text;
    }

    VariableType IBloxVariable.GetType()
    {
        return VariableType.INT;
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool ValidateNestToBottomIdented(GameObject objectToNest)
    {
        return true;
    }
}
