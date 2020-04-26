using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputPanel : MonoBehaviour
{
    [SerializeField] Text LogText;
    private List<string> MessageBuffer = new List<string>();
    private const string LOG_FORMAT = "@{0} => {1}\n";
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(MessageBuffer.Count > 0)
        {
            //Picks the first message, adds it, and removes from buffer
            string message = MessageBuffer[0];
            string formatedMessage = string.Format(LOG_FORMAT, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message);
            LogText.text += formatedMessage;
            MessageBuffer.RemoveAt(0);    
        }
         
    }

    public void AddMessage(string message)
    {
        message = message ?? string.Empty;
        MessageBuffer.Add(message);
    }       

}
