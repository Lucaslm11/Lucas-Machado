using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringBlox : ABlox, IBloxVariable, ICompilableBlox
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

    public void ToNodes(ICodeNode parentNode)
    {
        StringNode stringNode = new StringNode(GetValue());
        stringNode.NodeName = GetName();
        parentNode.AddChildNode(parentNode);
    }

    public List<BloxValidationError> Validate()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();

        // If STRINGBlox has no name
        if (string.IsNullOrWhiteSpace(GetName()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.STRING_BLOX_NO_NAME,
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

        // If STRINGBloxHas no value
        if (string.IsNullOrWhiteSpace(GetValue()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.STRING_BLOX_NO_VALUE,
                TargetBlox = this
            });
        }

        return errors;
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
