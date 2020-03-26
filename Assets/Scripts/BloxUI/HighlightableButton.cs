using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This component helps changing the normal color of a button
/// Currently used when user puts it's mouse over an error message
/// </summary>
public class HighlightableButton : MonoBehaviour
{
    Dictionary<ButtonHighlight, Color> ColorDictionary;
    public enum ButtonHighlight
    {
        // No highlight shall be applied
        None,
        // A success highlight shall be applied
        Success,
        // A warning highlight shall be applied
        Warning,
        // An error highlight shall be applied
        Error,
        // An info highlight shall be applied
        Info
    }
        
    [SerializeField] Button Button; 
    [SerializeField] Color None = Color.white; 
    [SerializeField] Color Success = Color.green; 
    [SerializeField] Color Warning = Color.yellow; 
    [SerializeField] Color Error = Color.red; 
    [SerializeField] Color Info = Color.blue; 
     
    // Start is called before the first frame update
    void Start()
    { 
        ColorDictionary = new Dictionary<ButtonHighlight, Color>();
        ColorDictionary[ButtonHighlight.None] = None;
        ColorDictionary[ButtonHighlight.Success] = Success;
        ColorDictionary[ButtonHighlight.Warning] = Warning;
        ColorDictionary[ButtonHighlight.Error] = Error;
        ColorDictionary[ButtonHighlight.Info] = Info;

        //HighlightButton(ButtonHighlight.None);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightButton(ButtonHighlight bh)
    {
        ColorBlock cb = Button.colors;
        cb.normalColor = ColorDictionary[bh];
        Button.colors = cb;
        
    }
}
