using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableChoiceBlox : ABlox, ICompilableBlox
{
    [SerializeField] Dropdown VariableDropdown;
    protected override bool IsParam { get { return true; } }

    public void ToNodes(ICodeNode parentNode)
    {
        // This blox won't compile to a node.
        // Instead use ToObjectNode for it to be used inside other node context
       // throw new System.NotImplementedException();
    }


    public ObjectNode ToObjectNode(RootNode rootNode)
    {
        // Grabs the selected variable and searchs for it
        string selectedValue = GameObjectHelper.GetDropdownSelectedTextValue(VariableDropdown);
        ICodeNode variable = rootNode.SearchChildByName(selectedValue);
        // Then creates an object node
        ObjectNode objectNode = new ObjectNode(variable, this.GetComponent<HighlightableButton>());
        return objectNode;
    }

    public List<BloxValidationError> Validate()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();
        string selectedValue = GameObjectHelper.GetDropdownSelectedTextValue(VariableDropdown);

        if (string.IsNullOrWhiteSpace(selectedValue))
        {
            errors.Add(new BloxValidationError()
            {
                TargetBlox = this,
                ErrorMessage = BloxValidationErrorMessages.VARIABLE_CHOICE_BLOX_NO_VALUE

            });
        }
        return errors;
    }
 
    // Update is called once per frame
    void Update()
    {
        List<IBloxVariable> varList = GetVariablesInBloxScope(this); //Operator bloxes only accepts numbers
        GameObjectHelper.PushVariablesIntoDropdown(VariableDropdown, varList);
    }
}
