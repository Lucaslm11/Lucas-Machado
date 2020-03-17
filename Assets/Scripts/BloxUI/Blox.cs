using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameObjectHelper;

/// <summary>
/// Based on https://docs.unity3d.com/2018.1/Documentation/ScriptReference/EventSystems.IDragHandler.html
/// </summary>
public abstract class Blox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    protected Collider2D lastCollisionInfo;
    private RectTransform bloxTransform;

       
    public void OnBeginDrag(PointerEventData eventData)
    {
        bloxTransform = GetComponent<RectTransform>();
        print("Begin Dragging"); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        bloxTransform.position = eventData.position;
    }


    public virtual void OnEndDrag(PointerEventData eventData)
    {
        print("End dragging");
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        print(gameObject.name + " collided with " + collision.name);
        lastCollisionInfo = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        lastCollisionInfo = null;
    }

    public void NestObject(GameObject secondObject)
    {
        if (secondObject.GetComponent<Blox>()!=null && secondObject!=null && ValidateNesting(secondObject))
        {
            Vector2 thisObjectPosition = this.gameObject.transform.localPosition;
            Vector2 secondObjectPosition = secondObject.transform.localPosition;
            RectTransform gameObjectTransform = this.gameObject.GetComponent<RectTransform>();
            RectTransform collidedObjectTransform = secondObject.GetComponent<RectTransform>();
 
            BoundingBox2D thisBBox = GameObjectHelper.getBoundingBox(this.gameObject);
            BoundingBox2D secondObjBBox = GameObjectHelper.getBoundingBox(secondObject);


            float thisObjectWidth = GameObjectHelper.getWidthFromBBox(thisBBox);
            float secondObjectWidth = GameObjectHelper.getWidthFromBBox(secondObjBBox);

            float thisObjectHeight = GameObjectHelper.getHeightFromBBox(thisBBox);
            float secondObjectHeight = GameObjectHelper.getHeightFromBBox(secondObjBBox); 

            // Checks if it is to nest the secondObject to the bottom of this one
            // The condition will be true when the second object is bellow this one
            // and if the left boundaries are near each other
            if (secondObjectPosition.y < thisObjectPosition.y
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.left.x, thisObjectWidth / 4))
            {
                // Nest to the bottom
                Vector3 collidedObjectNewPosition = gameObjectTransform.localPosition;
                collidedObjectNewPosition.y = collidedObjectNewPosition.y - (gameObjectTransform.rect.height) / 2 - collidedObjectTransform.rect.height;
                // Both objects are aligned by their centers, but we want to align them by their lefts
                float alignmentFactor = (collidedObjectTransform.rect.width - gameObjectTransform.rect.width)/2;
                collidedObjectNewPosition.x += alignmentFactor;
                collidedObjectTransform.localPosition = collidedObjectNewPosition;
            }
            // Checks if the left parth of the second object is near the right part of the first, and verifies if they are kind of aligned
            else if (MathHelper.IsNearby(secondObjBBox.left.y, thisBBox.right.y, thisObjectHeight/4) 
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.right.x, thisObjectWidth/4))
            {
                // Nest side to side
                Vector3 collidedObjectNewPosition = gameObjectTransform.localPosition;
                collidedObjectNewPosition.x += (thisObjectWidth) / 2 + secondObjectWidth / 2;
                
                collidedObjectTransform.localPosition = collidedObjectNewPosition;
                print("WIIIIDTH " + collidedObjectTransform.rect.width);
            }
            else if (secondObjectPosition.y < thisObjectPosition.y
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.bottom.x, thisObjectWidth / 4))
            {
                //Nest to the bottom but idented
                // Nest to the bottom
                Vector3 collidedObjectNewPosition = gameObjectTransform.localPosition;
                collidedObjectNewPosition.y = collidedObjectNewPosition.y - (gameObjectTransform.rect.height) / 2 - collidedObjectTransform.rect.height;
                // Both objects are aligned by their centers, but we want to align them by their lefts
                float alignmentFactor = collidedObjectTransform.rect.width/4;
                collidedObjectNewPosition.x += alignmentFactor;
                collidedObjectTransform.localPosition = collidedObjectNewPosition;
            }
          
            
        }
    }


    public void NestObject2(GameObject secondObject)
    {
        if (secondObject.GetComponent<Blox>() != null && secondObject != null && ValidateNesting(secondObject))
        {
            Vector2 thisObjectPosition = this.gameObject.transform.position;
            Vector2 secondObjectPosition = secondObject.transform.position;
            RectTransform gameObjectTransform = this.gameObject.GetComponent<RectTransform>();
            RectTransform collidedObjectTransform = secondObject.GetComponent<RectTransform>();
            //double thisObjectWidth = this.gameObject.GetComponent<Rect>().width;
            double thisObjectWidth = gameObjectTransform.rect.width;
            double secondObjectWidth = collidedObjectTransform.rect.width;
            //double thisObjectHeight = this.gameObject.GetComponent<Rect>().width;
            double thisObjectHeight = gameObjectTransform.rect.width;
            double secondObjectHeight = collidedObjectTransform.rect.width;
            BoundingBox2D thisBBox = GameObjectHelper.getBoundingBox(this.gameObject);
            BoundingBox2D secondObjBBox = GameObjectHelper.getBoundingBox(secondObject);

            // Checks if it is to nest the secondObject to the bottom of this one
            // The condition will be true when the second object is bellow this one
            // and if the left boundaries are near each other
            if (secondObjectPosition.y < thisObjectPosition.y
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.left.x, thisObjectWidth / 4))
            {
                // Nest to the bottom
                Vector3 collidedObjectNewPosition = gameObjectTransform.position;
                collidedObjectNewPosition.y = collidedObjectNewPosition.y - (gameObjectTransform.rect.height) / 2 - collidedObjectTransform.rect.height;
                // Both objects are aligned by their centers, but we want to align them by their lefts
                float alignmentFactor = collidedObjectTransform.rect.width - gameObjectTransform.rect.width;
                collidedObjectNewPosition.x += alignmentFactor;
                collidedObjectTransform.position = collidedObjectNewPosition;
            }
            // Checks if the left parth of the second object is near the right part of the first, and verifies if they are kind of aligned
            else if (MathHelper.IsNearby(secondObjBBox.left.y, thisBBox.right.y, thisObjectHeight / 4)
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.right.x, thisObjectWidth / 4))
            {
                // Nest side to side
                Vector3 collidedObjectNewPosition = gameObjectTransform.position;
                collidedObjectNewPosition.x += (gameObjectTransform.rect.width) / 2 + collidedObjectTransform.rect.width / 2;

                collidedObjectTransform.position = collidedObjectNewPosition;
                print("WIIIIDTH " + collidedObjectTransform.rect.width);
            }
            else if (secondObjectPosition.y < thisObjectPosition.y
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.bottom.x, thisObjectWidth / 4))
            {
                //Nest to the bottom but idented

            }


        }
    }


    /// <summary>
    /// This method must be overriden if the blox acepts nesting
    /// </summary>
    /// <param name="objectToNest"></param>
    /// <returns></returns>
    public virtual bool ValidateNesting(GameObject objectToNest)
    {
        return false;
    }

}
