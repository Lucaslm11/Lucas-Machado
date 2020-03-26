using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public void HighlightTarget(BloxHighlight highlight)
    {
        if (Error.TargetBlox != null && GameObjectHelper.CanBeCastedAs<IHighlightableBlox>(Error.TargetBlox))
        {
            IHighlightableBlox hBlox = ((IHighlightableBlox)Error.TargetBlox);
            hBlox.Highlight(BloxHighlight.Error);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightTarget(BloxHighlight.Error);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightTarget(BloxHighlight.None);
    }
}
