using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    /// <summary>
    /// Sets gameObject active state to false
    /// </summary>
    /// <param name="gameObject"></param>
    public void Close(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
