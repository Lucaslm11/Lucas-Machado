using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringBlox : ABlox, IBloxVariable
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
        return VariableType.STRING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
