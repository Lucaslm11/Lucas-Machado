using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericActionButton : MonoBehaviour
{
    /// <summary>
    /// Sets gameObject active state to false
    /// </summary>
    /// <param name="gameObject"></param>
    public void Close(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets gameObject active state to true
    /// </summary>
    /// <param name="gameObject"></param>
    public void Open(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Sets gameObject active state to true
    /// </summary>
    /// <param name="gameObject"></param>
    public void Switch(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
