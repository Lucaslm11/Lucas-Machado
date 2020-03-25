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
        throw new System.NotImplementedException();
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
