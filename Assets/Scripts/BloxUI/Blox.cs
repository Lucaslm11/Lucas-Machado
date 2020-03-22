using System;
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
    public struct BloxIdent
    {
        public Blox blox;
        public int ident;
    }

    protected Blox ParentBlox;
    protected List<Blox> ChildBloxes = new List<Blox>();
    protected List<Blox> BloxParams = new List<Blox>();

    protected List<Collider2D> collidedObjects = new List<Collider2D>();
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

        foreach(Collider2D collider in collidedObjects)
        {
            GameObject collidedObject = collider.gameObject;
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
        collidedObjects.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collidedObjects.Remove(collision);
    }
    #endregion

    #region Nesting Handling

    public void NestObject(GameObject secondObject)
    {
        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null)
        {
            if (secondObject.GetComponent<Blox>() != null && secondObject != null && ValidateNesting(secondObject))
            {
                Blox secondObjectBlox = secondObject.GetComponent<Blox>();
                Vector2 thisObjectPosition = this.gameObject.transform.position;
                Vector2 secondObjectPosition = secondObject.transform.position;
                RectTransform gameObjectTransform = this.gameObject.GetComponent<RectTransform>();
                RectTransform collidedObjectTransform = secondObject.GetComponent<RectTransform>();
                BoundingBox2D thisBBox = GameObjectHelper.getBoundingBoxInWorld(this.gameObject);
                BoundingBox2D secondObjBBox = GameObjectHelper.getBoundingBoxInWorld(secondObject);

                float thisObjectWidth = GameObjectHelper.getWidthFromBBox(thisBBox);
                float secondObjectWidth = GameObjectHelper.getWidthFromBBox(secondObjBBox);

                float thisObjectHeight = GameObjectHelper.getHeightFromBBox(thisBBox);
                float secondObjectHeight = GameObjectHelper.getHeightFromBBox(secondObjBBox);

                Vector3 collidedObjectNewPosition = gameObjectTransform.position;

                //checks if second object is bellow this
                if (secondObjectPosition.y < thisObjectPosition.y)
                {
                    // Checks if it is to nest the secondObject to the bottom of this one
                    // The condition will be true when the second object is bellow this one
                    // and if the left boundaries are near each other
                    if (ValidateNestToBottom(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.left.x, thisObjectWidth / 4))
                    {
                        // Nest to the bottom 
                        /*collidedObjectNewPosition.y = collidedObjectNewPosition.y - (thisObjectHeight) / 2 - secondObjectHeight;
                        // Both objects are aligned by their centers, but we want to align them by their lefts
                        float alignmentFactor = (secondObjectWidth - thisObjectWidth) / 2;
                        collidedObjectNewPosition.x += alignmentFactor;*/
                        AddToBottom(secondObjectBlox);
                    }
                    else if (ValidateNestToBottomIdented(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.bottom.x, thisObjectWidth / 4))
                    {

                       /* float identSpacing = rootBlox.GetIdentSpacing(rootBlox);

                        //Nest to the bottom but idented 
                        collidedObjectNewPosition.y = collidedObjectNewPosition.y - (thisObjectHeight) / 2 - secondObjectHeight;
                        // Both objects are aligned by their centers, but we want to ident
                        float alignmentFactor = (secondObjectWidth - thisObjectWidth) / 2 + identSpacing;
                        collidedObjectNewPosition.x += alignmentFactor;*/
                        AddToBottomIdented(secondObjectBlox);
                    }
                    else
                    {
                        collidedObjectNewPosition = collidedObjectTransform.position;
                    }
                }
                else //if it is above
                {
                    // Checks if the left parth of the second object is near the right part of the first, and verifies if they are kind of aligned
                    if (ValidateNestToTheSide(secondObject) && MathHelper.IsNearby(secondObjBBox.left.y, thisBBox.right.y, thisObjectHeight / 4)
                    && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.right.x, thisObjectWidth / 4))
                    {
                        // Nest side to side 
                        collidedObjectNewPosition.x += (thisObjectWidth) / 2 + secondObjectWidth / 2;
                    }
                    else
                    {
                        collidedObjectNewPosition = collidedObjectTransform.position;
                    }
                }
                 
                collidedObjectTransform.position = collidedObjectNewPosition;

                SetBloxPositionOnScreen();
            }
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

    /// <summary>
    /// Sets the bloxes positions based on their hierarchy
    /// </summary>
    public void SetBloxPositionOnScreen()
    {
        RootBlox rootBlox = GetRootBlox();
        Vector2 rootBloxPosition = rootBlox.transform.position;
        BoundingBox2D rootBBox = GameObjectHelper.getBoundingBoxInWorld(rootBlox.gameObject);
        float rootBloxWidth = GameObjectHelper.getWidthFromBBox(rootBBox);
        Vector2 rootBloxLeft = rootBBox.left;

        float bloxVerticalSpacing = GetVerticalSpacing(rootBlox);
        float identSpacing = GetIdentSpacing(rootBlox);

        // Gets all the nodes (except the ones nested to the side and root)
        List<BloxIdent> bloxIdentList = rootBlox.GetBloxListInVerticalOrder();
        Blox previousBlox = rootBlox;

        foreach (BloxIdent bloxIdent in bloxIdentList)
        {
            Blox blox = bloxIdent.blox;
            int ident = bloxIdent.ident;

            Vector2 previousBloxPosition = previousBlox.transform.position;
            BoundingBox2D previousBloxBBox = GameObjectHelper.getBoundingBoxInWorld(previousBlox.gameObject);
            BoundingBox2D bloxBBox = GameObjectHelper.getBoundingBoxInWorld(blox.gameObject);
            float bloxWidth = GameObjectHelper.getWidthFromBBox(bloxBBox);

            // Determines the vertical distance of this blox to the previous one
            float previousBloxHeight = GameObjectHelper.getHeightFromBBox(previousBloxBBox);
            float verticalOffset = previousBloxHeight / 2 + bloxVerticalSpacing;
            Vector2 newPosition = blox.transform.position;
            newPosition.y = previousBloxPosition.y - verticalOffset;

            // Determines the horizontal offset of this blox based on its ident
            newPosition.x = (rootBloxPosition.x - rootBloxWidth / 2) + bloxWidth/2 + ident * identSpacing;

            blox.transform.position = newPosition;

            previousBlox = blox;
        }


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

    #region Info getting

    private float GetVerticalSpacing(RootBlox rootBlox)
    {
        BoundingBox2D rootBBox = GameObjectHelper.getBoundingBoxInWorld(rootBlox.gameObject);
        float rootBloxHeight = GameObjectHelper.getHeightFromBBox(rootBBox);
        float bloxVerticalSpacing = rootBloxHeight / 2;
        return bloxVerticalSpacing;
    }

    private float GetIdentSpacing(RootBlox rootBlox)
    {
        BoundingBox2D rootBBox = GameObjectHelper.getBoundingBoxInWorld(rootBlox.gameObject);
        float rootBloxWidth = GameObjectHelper.getWidthFromBBox(rootBBox);
        float bloxIdentSpacing = rootBloxWidth / 4;
        return bloxIdentSpacing;
    }

    protected RootBlox GetRootBlox()
    {
        if (this is RootBlox)
            return this as RootBlox;
        else if (ParentBlox != null)
        {
            return ParentBlox.GetRootBlox();
        }
        return null;
    }

    /// <summary>
    /// Gets a list of bloxes in their vertical order plus their number of idents, no matter parent child relation they have.
    /// Ignores the ones that are nested to the right side of others. 
    /// Does not include root blox
    /// Call this method from root blox to have the whole list
    /// </summary>
    /// <returns></returns>
    protected List<BloxIdent> GetBloxListInVerticalOrder(int ident = 1)
    {
        List<BloxIdent> bloxList = new List<BloxIdent>();
        foreach (Blox child in ChildBloxes)
        {
            BloxIdent childIdent;
            childIdent.blox = child;
            childIdent.ident = ident;
            

            bloxList.Add(childIdent);
            bloxList.AddRange(child.GetBloxListInVerticalOrder(ident+1));
        }
        return bloxList;
    }



    #endregion

}
