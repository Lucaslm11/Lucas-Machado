using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FilterChar helps filter keyboard input on InputFields. Only characters matching the regular expression 
/// are accepted.
/// </summary>
public class FilterChar : MonoBehaviour
{
    InputField InputField;
    Regex regex;
    [SerializeField] string RemoveMatchingRegex = @"[^a-zA-Z\d:]";
    // Start is called before the first frame update
    void Start()
    {
        InputField = GetComponent<InputField>();
        
        regex = new Regex(RemoveMatchingRegex);
    }

    // Update is called once per frame
    void Update()
    {
        // If anykey was pressed, filters the text
        if(Input.anyKey || Input.anyKeyDown)
            InputField.text = Regex.Replace(InputField.text, RemoveMatchingRegex, "");
    }

  
}
