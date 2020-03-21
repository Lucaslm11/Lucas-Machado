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
    protected Blox ParentBlox;
    protected List<Blox> ChildBloxes = new List<Blox>();
    protected List<Blox> BloxParams = new List<Blox>();

    protected Collider2D lastCollisionInfo;
    private RectTransform bloxTransform;

    #region Dragging events
    public void OnBeginDrag(PointerEventData eventData)
    {
        bloxTransform = GetComponent<RectTransform>(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        bloxTransform.position = eventData.position;
    }
    
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        print("End dragging");
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

    #endregion

    #region Collision events
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        print(gameObject.name + " collided with " + collision.name);
        lastCollisionInfo = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        lastCollisionInfo = null;
    }
    #endregion


    #region Nesting Handling

    public void NestObject(GameObject secondObject)
    {
        if (secondObject.GetComponent<Blox>()!=null && secondObject!=null && ValidateNesting(secondObject))
        {
            Blox secondObjectBlox = secondObject.GetComponent<Blox>();
            Vector2 thisObjectPosition = this.gameObject.transform.position;
            Vector2 secondObjectPosition = secondObject.transform.position;
            RectTransform gameObjectTransform = this.gameObject.GetComponent<RectTransform>();
            RectTransform collidedObjectTransform = secondObject.GetComponent<RectTransform>();
            //TODO VER UMA FORMA DE OBTER AS DIMENSÕES SEM SER PELO RECT
            BoundingBox2D thisBBox = GameObjectHelper.getBoundingBoxInWorld(this.gameObject);
            BoundingBox2D secondObjBBox = GameObjectHelper.getBoundingBoxInWorld(secondObject);

            // TODO : substitute this for Rect obtention of width and height
            float thisObjectWidth = GameObjectHelper.getWidthFromBBox(thisBBox);
            float secondObjectWidth = GameObjectHelper.getWidthFromBBox(secondObjBBox);

            float thisObjectHeight = GameObjectHelper.getHeightFromBBox(thisBBox);
            float secondObjectHeight = GameObjectHelper.getHeightFromBBox(secondObjBBox);

            Vector3 collidedObjectNewPosition = gameObjectTransform.position;

            // Checks if it is to nest the secondObject to the bottom of this one
            // The condition will be true when the second object is bellow this one
            // and if the left boundaries are near each other
            if (ValidateNestToBottom(secondObject) && secondObjectPosition.y < thisObjectPosition.y
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.left.x, thisObjectWidth / 4))
            {
                // Nest to the bottom 
                collidedObjectNewPosition.y = collidedObjectNewPosition.y - (thisObjectHeight) / 2 - secondObjectHeight;
                // Both objects are aligned by their centers, but we want to align them by their lefts
                float alignmentFactor = (secondObjectWidth - thisObjectWidth) /2;
                collidedObjectNewPosition.x += alignmentFactor;
                AddToBottom(secondObjectBlox);
            }
            // Checks if the left parth of the second object is near the right part of the first, and verifies if they are kind of aligned
            else if (ValidateNestToTheSide(secondObject) && MathHelper.IsNearby(secondObjBBox.left.y, thisBBox.right.y, thisObjectHeight/4) 
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.right.x, thisObjectWidth/4))
            {
                // Nest side to side 
                collidedObjectNewPosition.x += (thisObjectWidth) / 2 + secondObjectWidth / 2; 
            }
            else if (ValidateNestToBottomIdented(secondObject) && secondObjectPosition.y < thisObjectPosition.y
                && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.bottom.x, thisObjectWidth / 4))
            {
                //Nest to the bottom but idented 
                collidedObjectNewPosition.y = collidedObjectNewPosition.y - (thisObjectHeight) / 2 - secondObjectHeight;
                // Both objects are aligned by their centers, but we want to ident
                float alignmentFactor = secondObjectWidth/4;
                collidedObjectNewPosition.x += alignmentFactor;
                AddToBottomIdented(secondObjectBlox);
            }
            else
            {
                collidedObjectNewPosition = collidedObjectTransform.position;
            }
            collidedObjectTransform.position = collidedObjectNewPosition;


        }
    }

    /// <summary>
    /// Called before attempting to nest at all. 
    /// Nesting is not possible when this returns false
    /// </summary>
    /// <param name="objectToNest"></param>
    /// <returns></returns>
    public virtual bool ValidateNesting(GameObject objectToNest)
    {
        return true;
    }

    /// <summary>
    /// Called before attempting to nest a blox to the bottom of the current blox.
    /// Nesting to the bottom is not possible when this returns false
    /// </summary>
    /// <param name="objectToNest"></param>
    /// <returns></returns>
    public virtual bool ValidateNestToBottom(GameObject objectToNest)
    {
        return true;
    }

    /// <summary>
    /// Called before attempting to nest a blox to the bottom, but idented, of the current blox.
    /// Nesting to the buttom (idented) is not possible when this returns false
    /// </summary>
    /// <param name="objectToNest"></param>
    /// <returns></returns>
    public virtual bool ValidateNestToBottomIdented(GameObject objectToNest)
    {
        return false;
    }

    /// <summary>
    /// Called before attempting to nest a blox to the side of the current blox.
    /// Nesting to the side is not possible when this returns false
    /// </summary>
    /// <param name="objectToNest"></param>
    /// <returns></returns>
    public virtual bool ValidateNestToTheSide(GameObject objectToNest)
    {
        return false;
    }


    // Reoders the nodes
    public void OrderNodes()
    {

    }
    #endregion


    #region ChildrenHandling
        
    /// <summary>
    /// Adds a blox next to this one in parent children
    /// </summary>
    /// <param name="blox">Blox to add</param>
    protected void AddToBottom(Blox blox)
    {
        RemoveFromParent(blox);
        // Gets the index of this blox in parent
        int thisBloxIndex = this.ParentBlox.ChildBloxes.IndexOf(this);
        this.ParentBlox.ChildBloxes.Insert(thisBloxIndex + 1, blox);
        blox.ParentBlox = this.ParentBlox;



    }

    protected void AddToBottomIdented(Blox blox)
    {
        RemoveFromParent(blox);
        this.ChildBloxes.Insert(0, blox);
        blox.ParentBlox = this;
    }


    protected void RemoveFromParent(Blox blox)
    {
        Blox originalParent = blox.ParentBlox;
        if (originalParent != null)
        {
            originalParent.ChildBloxes.Remove(blox);
            originalParent.BloxParams.Remove(blox);
        }
    }
    #endregion

}
