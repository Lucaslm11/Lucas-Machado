using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IfBlox : Blox
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
        return GameObjectHelper.HasComponent<LogicalOperatorBlox>(objectToNest);
    }

    private void UpdateVariableList()
    {
        List<IBloxVariable> varList = GetVariablesInBloxScope(this).Where(v => v.GetType() == VariableType.BOOL).ToList();
        GameObjectHelper.PushVariablesIntoDropdown(booleanVariablesDropdown, varList);
    }
}
