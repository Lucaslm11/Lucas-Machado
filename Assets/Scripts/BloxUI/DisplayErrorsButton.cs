using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayErrorsButton : MonoBehaviour
{
    private bool DisplayWindow = false;
    [SerializeField] ErrorPopup ErrorPopup;
    // Start is called before the first frame update
    void Start()
    {
        ErrorPopup.gameObject.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchDisplayState()
    {
        DisplayWindow = !DisplayWindow;
        ErrorPopup.gameObject.SetActive(DisplayWindow);
    }


}
