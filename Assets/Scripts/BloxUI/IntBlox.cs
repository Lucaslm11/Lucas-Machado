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
     


    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (lastCollisionInfo != null)
        {
            GameObject collidedObject = lastCollisionInfo.gameObject;

            Blox blox = collidedObject.GetComponent<Blox>();
            if (blox != null)
            {
                blox.NestObject(this.gameObject);
            }
        }
    }
}
