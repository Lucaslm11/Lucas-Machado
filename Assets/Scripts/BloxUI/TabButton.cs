using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TabButton : Button
{
    public void SetListener(UnityEngine.Events.UnityAction call)
    {
        onClick.AddListener(call);
    }
}