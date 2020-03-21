using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBlox : Blox
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool ValidateNestToBottom(GameObject objectToNest)
    {
        return false;
    }

    public override bool ValidateNestToBottomIdented(GameObject objectToNest)
    {
        return true;
    }
}
