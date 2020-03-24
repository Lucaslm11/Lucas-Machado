using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrueFalseTextSwitcher : MonoBehaviour
{
    private const string FALSE_TEXT = "false";
    private const string TRUE_TEXT = "true";
    
    [SerializeField] bool DefaultState = false;
    private bool state;
    [SerializeField] Text TextComponent;

    private void Start()
    {
        state = !DefaultState; //The Default is inverted because switch also inverts. Its a strange way to stay in the same value on start
        Switch();
    }

    public void Switch()
    {
        state = !state;
        TextComponent.text = state ? TRUE_TEXT : FALSE_TEXT;
    }

}
