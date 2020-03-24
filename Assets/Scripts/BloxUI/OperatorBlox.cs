using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Handles an operator blox. 
/// We are assuming an operator blox has 3 components:
/// Value1, Value2 and Operator
/// Value1and Value2 can be either an input number or a dropdown choice
/// </summary>
public class OperatorBlox : Blox
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

    [SerializeField] FieldSwitcher Value1Switcher;
    [SerializeField] FieldSwitcher Value2Switcher;

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
    private FieldChoice getSwitcherValue(FieldSwitcher fs)
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

}
