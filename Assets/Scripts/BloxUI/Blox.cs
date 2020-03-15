using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Based on https://docs.unity3d.com/2018.1/Documentation/ScriptReference/EventSystems.IDragHandler.html
/// </summary>
public abstract class Blox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    private RectTransform bloxTransform;


   
    public void OnBeginDrag(PointerEventData eventData)
    {
        bloxTransform = GetComponent<RectTransform>();
        print("Begin Dragging"); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        bloxTransform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        print("End draggin");
    }


}
