using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicalOperatorBlox : Blox
{
    private const string OPERATOR_DROPDOWN_NAME = "Operator";
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
