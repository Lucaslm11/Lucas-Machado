using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts.Terminal.Nodes.Types;
using Assets.Scripts.Terminal.Nodes;

/// <summary>
/// Handles an operator blox. 
/// We are assuming an operator blox has 3 components:
/// Value1, Value2 and Operator
/// Value1and Value2 can be either an input number or a dropdown choice
/// </summary>
public class OperatorBlox : ABlox
{
    protected override bool IsParam { get { return true; } }

    /// <summary>
    /// Describes a choice of elements in a FieldSwitcher
    /// If user inputs a choice in the dropdown, isVariable will be true.
    /// If user inputs a value in inputField, isVariable will be false.
    /// </summary>
    public struct FieldChoice
    {
        public bool isVariable;
        public string value;
    }

    [SerializeField] protected FieldSwitcher Value1Switcher;
    [SerializeField] protected FieldSwitcher Value2Switcher;

    private const string OPERATOR_DROPDOWN_NAME = "Operator";

    // Update is called once per frame
    void Update()
    {
        FillFieldDropdownWithVariables();
    }

    public string GetOperator()
    {
        GameObject childObject = GameObjectHelper.GetChildByName(this.gameObject, OPERATOR_DROPDOWN_NAME);
        Dropdown operatorDropdown = childObject.GetComponent<Dropdown>();
        return GameObjectHelper.GetDropdownSelectedTextValue(operatorDropdown);
    }

    public FieldChoice getField1Value()
    {
        return getSwitcherValue(Value1Switcher);
    }


    public FieldChoice getField2Value()
    {
        return getSwitcherValue(Value2Switcher);
    }



    /// <summary>
    /// Gets a value from the active field of a field switcher
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    protected FieldChoice getSwitcherValue(FieldSwitcher fs)
    {
        GameObject activeField = fs.GetActiveField();
        FieldChoice fieldChoice;
        fieldChoice.value = string.Empty;
        fieldChoice.isVariable = false;
        if (GameObjectHelper.HasComponent<InputField>(activeField))
        {
            fieldChoice.isVariable = false;
            fieldChoice.value = activeField.GetComponent<InputField>().text;
        }
        else if (GameObjectHelper.HasComponent<Dropdown>(activeField))
        {
            fieldChoice.isVariable = true;
            fieldChoice.value = GameObjectHelper.GetDropdownSelectedTextValue(activeField.GetComponent<Dropdown>()); ;
        }
        return fieldChoice;
    }

    private void FillFieldDropdownWithVariables()
    {
        List<IBloxVariable> varList = GetVariablesInBloxScope(this).Where(v=>v.GetType()==VariableType.INT).ToList(); //Operator bloxes only accepts numbers
        Dropdown field1 = Value1Switcher.getField2().GetComponent<Dropdown>();
        Dropdown field2 = Value2Switcher.getField2().GetComponent<Dropdown>();
        GameObjectHelper.PushVariablesIntoDropdown(field1, varList);
        GameObjectHelper.PushVariablesIntoDropdown(field2, varList);
    }

    #region Blox Compiler helpers
    
    protected IntegerNode GetField1Node(RootNode rootNode)
    {
        return FieldToNode(getSwitcherValue(Value1Switcher), rootNode);
    }

    protected IntegerNode GetField2Node(RootNode rootNode)
    {
        return FieldToNode(getSwitcherValue(Value2Switcher), rootNode);
    }

    /// <summary>
    /// Converts a field choice to a node.
    /// If field is variable, instead of creating a new node, returns an existing
    /// </summary>
    /// <param name="field"></param>
    /// <param name="rootNode"></param>
    /// <returns></returns>
    protected IntegerNode FieldToNode(FieldChoice field, RootNode rootNode)
    {
        HighlightableButton highlightableButton = (GameObjectHelper.HasComponent<HighlightableButton>(this.gameObject)) ? this.GetComponent<HighlightableButton>() : null;
        IntegerNode fieldNode = null;
        // If it is a variable, the variable has already been created
        // so we are going to search for it
        if (field.isVariable)
        {
            // When it is a variable, value will have its name
            ICodeNode variable = rootNode.SearchChildByName(field.value);
            if(variable!=null && GameObjectHelper.CanBeCastedAs<IntegerNode>(variable))
            {
                fieldNode =(IntegerNode)variable;
            }
            else
            {
                throw new System.Exception("Expected " + field.value + " to be an integer node");
            }
        }
        // User has input a numeric value
        else
        {
            int value = int.Parse(field.value);
            fieldNode = new IntegerNode(null, value);
        }
        return fieldNode;
    }

    public List<BloxValidationError> ValidateOperator()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();
        FieldChoice field1 = getSwitcherValue(Value1Switcher);
        FieldChoice field2 = getSwitcherValue(Value2Switcher);

        // if one of the fields is empty
        if (string.IsNullOrWhiteSpace(field1.value) || string.IsNullOrWhiteSpace(field2.value))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.OPERATOR_BLOX_NO_VALUE,
                TargetBlox = this
            });
        }

        return errors;
    }
    #endregion

    public override bool ValidateNestToBottom(GameObject objectToNest)
    {
        return false;
    }


}
