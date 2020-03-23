using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArithmeticOperatorBlox : Blox
{
    private const string OPERATOR_DROPDOWN_NAME = "Operator";

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetOperator()
    {
        GameObject childObject = GameObjectHelper.GetChildByName(this.gameObject, OPERATOR_DROPDOWN_NAME);
        Dropdown operatorDropdown = childObject.GetComponent<Dropdown>();
        return GameObjectHelper.GetDropdownSelectedTextValue(operatorDropdown);
    }
}
