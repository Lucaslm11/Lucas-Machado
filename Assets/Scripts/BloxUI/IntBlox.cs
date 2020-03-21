using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntBlox : Blox
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        // Only allows nesting a component to the side of this if it is an ArithmeticOperator
        return GameObjectHelper.HasComponent<ArithmeticOperatorBlox>(objectToNest);
    }
}
