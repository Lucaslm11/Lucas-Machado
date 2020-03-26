﻿using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForBlox : ABlox, IBloxVariable, ICompilableBlox
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

    #region ICompilable region
    public List<BloxValidationError> Validate()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();
         
        // if counter has no name 
        if (string.IsNullOrWhiteSpace(GetName()))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.FOR_BLOX_NO_NAME,
                TargetBlox = this
            });
        }

        // if ToField or FromField not filled
        if(string.IsNullOrWhiteSpace(FromField.text) || string.IsNullOrWhiteSpace(ToField.text))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.FOR_BLOX_NO_VALUES,
                TargetBlox = this
            });
        }
         
        // Validate children
        ValidateBloxList(ChildBloxes);
        return errors;
    }

    public void ToNodes(ICodeNode parentNode)
    {
        //Creates an integer node for the counter
        IntegerNode counter = new IntegerNode(int.Parse(GetValue()));
        counter.NodeName = this.GetName();
        //Adds it to parent
        parentNode.AddChildNode(counter);

        //Creates the for node
        ForNode forNode = new ForNode(counter, int.Parse(FromField.text), int.Parse(ToField.text));
        parentNode.AddChildNode(forNode);

        //This node has child nodes. Ensures that child nodes are created from child bloxes
        CompileChildrenToNodes(forNode);
    }
    #endregion
}
