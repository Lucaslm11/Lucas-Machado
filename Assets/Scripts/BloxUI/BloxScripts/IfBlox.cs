using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Functions;
using Assets.Scripts.Terminal.Nodes.Types;

public class IfBlox : ABlox, ICompilableBlox
{
    [SerializeField] Dropdown booleanVariablesDropdown;

    private void Update()
    {
        UpdateVariableList();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {

        // ESTE PEDAÇO DE CÓDIGO NÃO FUNCIONA ATUALMENTE, PORQUE O QUE SE ESTÁ A ARRASTAR É O INT E NÃO O IF. SÓ QUANDO O IF É ARRASTADO
        base.OnEndDrag(eventData);

    }

    public override bool ValidateNesting(GameObject objectToNest)
    {
        // For now accepts, but must validate if object is nestable
        return true;
    }

    public override bool ValidateNestToBottom(GameObject objectToNest)
    {
        return true;
    }
    public override bool ValidateNestToBottomIdented(GameObject objectToNest)
    {
        return true;
    }

    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        return GameObjectHelper.HasComponent<LogicalOperatorBlox>(objectToNest) && BloxParams.Count == 0; //Only allows one
    }

    private void UpdateVariableList()
    {
        List<IBloxVariable> varList = GetVariablesInBloxScope(this).Where(v => v.GetType() == VariableType.BOOL).ToList();
        GameObjectHelper.PushVariablesIntoDropdown(booleanVariablesDropdown, varList);
    }

    public List<BloxValidationError> Validate()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();

        // Validations:  
        // --Must validate also child nodes
        // --Must validate params
        string selectedBoolVarName = GameObjectHelper.GetDropdownSelectedTextValue(this.booleanVariablesDropdown);

        // If no params and no var selected
        if (BloxParams.Count == 0 && string.IsNullOrWhiteSpace(selectedBoolVarName))
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.IF_BLOX_NO_VAR,
                TargetBlox = this
            });
        }

        // If no children
        if(ChildBloxes.Count == 0)
        {
            errors.Add(new BloxValidationError()
            {
                ErrorMessage = BloxValidationErrorMessages.IF_BLOX_NO_CHILDREN,
                TargetBlox = this
            });
        }        
        
        // validates existing children 
        errors.AddRange(ValidateBloxList(ChildBloxes));

        // validates params
        errors.AddRange(ValidateBloxList(BloxParams));


        return errors;
    }

    public void ToNodes(ICodeNode parentNode)
    {
        RootNode rootNode = parentNode.GetRootNode();
        IfNode ifNode = null;
        HighlightableButton highlightableButton = (GameObjectHelper.HasComponent<HighlightableButton>(this.gameObject)) ? this.GetComponent<HighlightableButton>() : null;
        // IfNode can receive either a bool variable (chosen in dropdown) or a LogicalOperatorBlox as param
        // If it has received a param (LogicalOperatorBlox), creates a LogicalOperationNode
        // and instantiates ifNode with it
        if (BloxParams.Count > 0)
        { 
            LogicalOperatorBlox blox = BloxParams[0].GetComponent<LogicalOperatorBlox>();
            LogicalOperationNode logicOpNode = blox.CreateNode(rootNode);
            ifNode = new IfNode(highlightableButton,logicOpNode);
        }
        // If a variable was chosen, will search for an existing BooleanNode with that same name
        else
        {
            // Gets the selected boolean variable
            string selectedBoolVarName = GameObjectHelper.GetDropdownSelectedTextValue(this.booleanVariablesDropdown);
            ICodeNode foundNode = rootNode.SearchChildByName(selectedBoolVarName);
            if(foundNode!= null && GameObjectHelper.CanBeCastedAs<BooleanNode>(foundNode))
            {
                BooleanNode boolNode = (BooleanNode)foundNode;
                ifNode = new IfNode(highlightableButton,boolNode);
            }
            else
            {
                throw new System.Exception("Expected " + selectedBoolVarName + " to be a boolean node");
            }
        }
        
        parentNode.AddChildNode(ifNode);

        CompileChildrenToNodes(ifNode);

    }
}
