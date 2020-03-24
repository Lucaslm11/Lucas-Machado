﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameObjectHelper;

/// <summary>
/// Based on https://docs.unity3d.com/2018.1/Documentation/ScriptReference/EventSystems.IDragHandler.html
/// </summary>
public abstract class Blox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Indicates if the blox will be only intended to be used as a PARAM
    protected virtual bool IsParam { get { return false; } }

    [NonSerialized] public bool InsideBloxBag = false;
    [NonSerialized]public bool IsBeingDragged = false;
    protected RootBlox rootBlox = null;

    public struct BloxIdent
    {
        public Blox blox;
        public int ident;
    }

    protected bool NestingActive = true; // Disables any nesting if false

    protected Blox ParentBlox;
    protected List<Blox> ChildBloxes = new List<Blox>();
    protected List<Blox> BloxParams = new List<Blox>();

    protected List<Collider2D> collidedObjects = new List<Collider2D>();
    private RectTransform bloxTransform;

    #region start and update
    protected virtual void Start()
    {
        InsideBloxBag = CheckIfInsideBloxBag();
    }

    bool CheckIfInsideBloxBag()
    {
        bool validation = false;
        try
        {
            Transform parent = this.transform.parent.parent.parent; //Content, then Viewport then BloxBag
            validation = GameObjectHelper.HasComponent<BloxBag>(parent.gameObject);
        }
        catch (Exception ex) { }
        return validation;
    }

    #endregion


    #region Dragging events
    public void OnBeginDrag(PointerEventData eventData)
    {
        bloxTransform = GetComponent<RectTransform>();

        // When attempting to drag a Blox that is inside the blox bag, 
        // instead of just dragging we create a new instance and set
        // the current one as not inside
        if (InsideBloxBag)
        {
            Blox newInstance = Instantiate(this, this.transform.parent);
            this.transform.parent = this.transform.parent.parent.parent.parent; //Content, then Viewport then BloxBag then Canvas
            this.InsideBloxBag = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        IsBeingDragged = true;
        bloxTransform.position = eventData.position;

        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null)
        {
            float bloxVerticalSpacing = GetVerticalSpacing(rootBlox);
            float identSpacing = GetIdentSpacing(rootBlox);
            float paramSpacing = GetParamSpacing(rootBlox);
            // Sets the child blox and params position while dragging, for them to follow this blox
            // Those bloxes nesting is temporarily disabled 
            SetChildBloxesPositionOnScreen(this, bloxVerticalSpacing, identSpacing, paramSpacing, false);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        IsBeingDragged = false;
        print("End dragging");

        if (collidedObjects.Count > 0)
        {
            // Although we are saving all the simultaneous collisions 
            // We only want this object to nest with the highest one
            float maxY = collidedObjects.Max(c => c.transform.position.y);
            GameObject collidedObject = collidedObjects.Where(c => c.transform.position.y == maxY).FirstOrDefault().gameObject;

            //If the collided object is a blox, attempts nesting
            if (GameObjectHelper.HasComponent<Blox>(collidedObject))
            {
                Blox blox = collidedObject.GetComponent<Blox>();
                blox.NestObject(this.gameObject);
            }
            //If the collied object is the trashbin, destroys it
            else if (GameObjectHelper.HasComponent<Trashbin>(collidedObject))
            {
                RemoveFromParent(this);
                Destroy(this.gameObject);
            }

        }
        else
            SetAllBloxesPositionsOnScreen();
    }

    #endregion

    #region Collision events
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(gameObject.name + " collided with " + collision.name);
        if (GameObjectHelper.HasComponent<Blox>(collision.gameObject) || GameObjectHelper.HasComponent<Trashbin>(collision.gameObject))
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
        if (rootBlox != null && NestingActive)
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

                //checks if second object top is bellow this object center
                if (secondObjBBox.top.y < thisObjectPosition.y)
                {
                    if (ValidateNestToBottom(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.left.x, thisObjectWidth / 4))
                    {
                        AddToBottom(secondObjectBlox);
                    }
                    else if (ValidateNestToBottomIdented(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.bottom.x, thisObjectWidth / 4))
                    {
                        AddToBottomIdented(secondObjectBlox);
                    }

                }
                else //if it is above
                {
                    // Checks if the left parth of the second object is near the right part of the first, and verifies if they are kind of aligned
                    if (ValidateNestToTheSide(secondObject) && MathHelper.IsNearby(secondObjBBox.left.y, thisBBox.right.y, thisObjectHeight / 4)
                    && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.right.x, thisObjectWidth / 4))
                    {
                        // Nest side to side 
                        AddParam(secondObjectBlox);
                    }
                }

                SetAllBloxesPositionsOnScreen();
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


    public void SetAllBloxesPositionsOnScreen()
    {
        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null)
        {
            float bloxVerticalSpacing = GetVerticalSpacing(rootBlox);
            float identSpacing = GetIdentSpacing(rootBlox);
            float paramSpacing = GetParamSpacing(rootBlox);
            SetChildBloxesPositionOnScreen(rootBlox, bloxVerticalSpacing, identSpacing, paramSpacing);
        }
    }


    /// <summary>
    /// Sets the position of child bloxes and param bloxes of parent 
    /// </summary>
    protected void SetChildBloxesPositionOnScreen(Blox parentBlox, float bloxVerticalSpacing, float identSpacing, float paramSpacing, bool nestingActive = true)
    {
        BoundingBox2D parentBBox = GameObjectHelper.getBoundingBoxInWorld(parentBlox.gameObject);
        Vector2 parentBloxLeft = parentBBox.left;

        // Gets all the nodes (except the ones nested to the side and parent)
        List<BloxIdent> bloxIdentList = parentBlox.GetChildBloxListInVerticalOrder();
        Blox previousBlox = parentBlox;

        // Sets the parent blox params
        SetBloxParamsPositionOnScreen(parentBlox, paramSpacing, nestingActive);


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
            newPosition.x = parentBloxLeft.x + bloxWidth / 2 + ident * identSpacing;
            blox.transform.position = newPosition;

            // Sets the params bloxes positions

            SetBloxParamsPositionOnScreen(blox, paramSpacing, nestingActive);
            Blox previousBloxInLine = blox;

            previousBlox = blox;
        }
    }

    /// <summary>
    /// Positions the blox params on screen
    /// </summary>
    /// <param name="blox"></param>
    protected void SetBloxParamsPositionOnScreen(Blox blox, float paramSpacing, bool nestingActive = true)
    {
        Blox previousBloxInLine = blox;
        BoundingBox2D bloxBBox = GameObjectHelper.getBoundingBoxInWorld(blox.gameObject);
        float bloxWidth = GameObjectHelper.getWidthFromBBox(bloxBBox);
        foreach (Blox param in blox.BloxParams)
        {
            param.SetNestingState(nestingActive);
            BoundingBox2D paramBBox = GameObjectHelper.getBoundingBoxInWorld(param.gameObject);
            float paramWidth = GameObjectHelper.getWidthFromBBox(paramBBox);
            Vector2 paramPosition = previousBloxInLine.transform.position;
            paramPosition.x += (bloxWidth + paramWidth) / 2 + paramSpacing;
            param.transform.position = paramPosition;
            previousBloxInLine = param;
        }
    }


    protected void SetNestingState(bool active)
    {
        NestingActive = active;
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

    /// <summary>
    /// Adds blox as a child of this blox
    /// </summary>
    /// <param name="blox"></param>
    protected void AddToBottomIdented(Blox blox)
    {
        RemoveFromParent(blox);
        this.ChildBloxes.Insert(0, blox);
        blox.ParentBlox = this;
    }

    protected void AddParam(Blox param)
    {
        RemoveFromParent(param);
        BloxParams.Add(param);
        param.ParentBlox = this;
    }

    /// <summary>
    /// Takes a blox and removes any reference inside it's parent
    /// </summary>
    /// <param name="blox"></param>
    protected void RemoveFromParent(Blox blox)
    {
        Blox originalParent = blox.ParentBlox;
        if (originalParent != null)
        {
            originalParent.ChildBloxes.Remove(blox);
            originalParent.BloxParams.Remove(blox);
        }
        blox.ParentBlox = null;
    }
    #endregion

    #region Info getting

    private float GetVerticalSpacing(RootBlox rootBlox)
    {
        BoundingBox2D rootBBox = GameObjectHelper.getBoundingBoxInWorld(rootBlox.gameObject);
        float rootBloxHeight = GameObjectHelper.getHeightFromBBox(rootBBox);
        float bloxVerticalSpacing = 2 * rootBloxHeight / 3;
        return bloxVerticalSpacing;
    }

    private float GetIdentSpacing(RootBlox rootBlox)
    {
        BoundingBox2D rootBBox = GameObjectHelper.getBoundingBoxInWorld(rootBlox.gameObject);
        float rootBloxWidth = GameObjectHelper.getWidthFromBBox(rootBBox);
        float bloxIdentSpacing = rootBloxWidth / 4;
        return bloxIdentSpacing;
    }

    private float GetParamSpacing(RootBlox rootBlox)
    {
        BoundingBox2D rootBBox = GameObjectHelper.getBoundingBoxInWorld(rootBlox.gameObject);
        float rootBloxWidth = GameObjectHelper.getWidthFromBBox(rootBBox);
        float bloxParamSpacing = rootBloxWidth / 10;
        return bloxParamSpacing;
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
    /// Grabs all the child and n-grand child bloxes, and puts them in a list, ordered by their vertical order
    /// and associates each one with an indent value.
    /// Does not contain param bloxes. 
    /// Does not include this blox.
    /// Call this method from root blox to have the whole list
    /// </summary>
    /// <returns></returns>
    protected List<BloxIdent> GetChildBloxListInVerticalOrder(int ident = 1)
    {
        List<BloxIdent> bloxList = new List<BloxIdent>();
        foreach (Blox child in ChildBloxes)
        {
            BloxIdent childIdent;
            childIdent.blox = child;
            childIdent.ident = ident;


            bloxList.Add(childIdent);
            bloxList.AddRange(child.GetChildBloxListInVerticalOrder(ident + 1));
        }
        return bloxList;
    }


    /// <summary>
    /// Gets all the variable bloxes on the scope of a blox
    /// </summary>
    /// <param name="blox"></param>
    /// <returns></returns>
    public List<IBloxVariable> GetVariablesInBloxScope(Blox blox)
    {
        List<IBloxVariable> variablesInScope = new List<IBloxVariable>(); 
        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null)
        {
            List<BloxIdent> bloxList = rootBlox.GetChildBloxListInVerticalOrder();
            //Params are not loaded in GetChildBloxListInVerticalOrder() method, so we evaluate according to the parent index
            //on that case
            BloxIdent bloxIdent = bloxList.Find(b=>b.blox == (blox.IsParam ? blox.ParentBlox : blox)); 
            BloxIdent bloxIdent2 = bloxList.Where(b=>b.blox == (blox.IsParam ? blox.ParentBlox : blox)).FirstOrDefault(); 

            int indexOfBlox = -1;
            try
            {
                indexOfBlox = bloxList.IndexOf(bloxIdent);
            }
            catch(Exception ex) { }

            if(indexOfBlox >= 0)
            {
                // Gets all the bloxes above this one, that are variables
                // Although pure Blox objects cannot be cast as IBloxVariable,
                // objects of classes that inherit from Blox, and implement IBloxVariable, can
                variablesInScope = bloxList.GetRange(0, indexOfBlox).Where(b=>((IBloxVariable)b.blox) !=null).Select(b=>(IBloxVariable)b.blox).ToList();                 
            }

        }
        return variablesInScope;
    }




    #endregion

}
