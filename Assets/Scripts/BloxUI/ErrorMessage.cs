using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static HighlightableButton;

public class ErrorMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text TextMessageOutput;
    public BloxValidationError Error { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void SetTextMessage(string message)
    {
        TextMessageOutput.text = message;
    }

    public void SetErrorMessage(BloxValidationError error)
    {
        Error = error;
        SetTextMessage(error.ErrorMessage);


    }

    public void HighlightTarget(ButtonHighlight highlight)
    {
        if (Error.TargetBlox != null && GameObjectHelper.HasComponent<HighlightableButton>(Error.TargetBlox.gameObject))
        {
            HighlightableButton hBtn = Error.TargetBlox.GetComponent<HighlightableButton>();
            hBtn.HighlightButton(highlight);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightTarget(ButtonHighlight.Error);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightTarget(ButtonHighlight.None);
    }
}
