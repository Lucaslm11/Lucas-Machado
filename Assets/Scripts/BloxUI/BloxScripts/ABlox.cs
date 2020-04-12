using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameObjectHelper;

/// <summary>
/// Based on https://docs.unity3d.com/2018.1/Documentation/ScriptReference/EventSystems.IDragHandler.html
/// </summary>
public abstract class ABlox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public static ABlox LastClickedBlox { get; private set; }
    /// <summary>
    /// Contains the text to be shown in the BloxHelpPanel
    /// </summary>
    [SerializeField] public String HelpText;
    /// <summary>
    /// Contains the image to be shown in the BloxHelpPanel
    /// </summary>
    [SerializeField] public Texture HelpExampleTexture; 

    public enum NestingType
    {
        NONE,
        SIDE,
        BOTTOM,
        BOTTOM_IDENTED
    }

    // Saves a list of bloxes that are highlighted
    List<HighlightableButton> hightlightableBloxes = new List<HighlightableButton>();

    //Indicates if the blox will be only intended to be used as a PARAM
    protected virtual bool IsParam { get { return false; } }

    [NonSerialized] public bool InsideBloxBag = false;
    [NonSerialized] public bool IsBeingDragged = false;
    protected RootBlox rootBlox = null;

    public struct BloxIdent
    {
        public ABlox blox;
        public int ident;
    }

    protected bool NestingActive = true; // Disables any nesting if false

    protected ABlox ParentBlox;
    protected List<ABlox> ChildBloxes = new List<ABlox>();
    protected List<ABlox> BloxParams = new List<ABlox>();

    protected List<Collider2D> collidedObjects = new List<Collider2D>();
    private RectTransform bloxTransform;

    #region start and update
    void Start()
    {
        InsideBloxBag = CheckIfInsideBloxBag();
        OnStart();
    }

    //Classes that use ABlox as base, shall use OnStart instead of Start()
    protected virtual void OnStart()
    {
        
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

    #region


    public void OnPointerDown(PointerEventData eventData)
    {
        ABlox.LastClickedBlox = this;
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
            ABlox newInstance = Instantiate(this, this.transform.parent);
            this.transform.SetParent(this.transform.parent.parent.parent.parent); //Content, then Viewport then BloxBag then Canvas
            this.InsideBloxBag = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        IsBeingDragged = true;
        bloxTransform.position = eventData.position;


        // If this blox has child bloxes and params, sets their positions in 
        // order to drag them also
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

        // Verifies if there are collided objects, and if
        // this object can be potentially nested to those
        if (collidedObjects.Count > 0)
        {
            // Although we are saving all the simultaneous collisions 
            // We only want this object to nest with the highest ones
            float maxY = collidedObjects.Max(c => c.transform.position.y);
            List<GameObject> highestObjects = collidedObjects.Where(c => c.transform.position.y == maxY).Select(a => a.gameObject).ToList();
            ResetHightlight();
            foreach (GameObject collidedObject in highestObjects)
            {
                //If the collided object is a blox, checks if this is nestable to it, and hightlights if it is
                if (GameObjectHelper.HasComponent<ABlox>(collidedObject))
                {
                    NestingType nestingType = collidedObject.GetComponent<ABlox>().DetermineNestingType(this.gameObject);
                    if (nestingType != NestingType.NONE && GameObjectHelper.HasComponent<HighlightableButton>(collidedObject.gameObject))
                    {
                        HighlightableButton hB = collidedObject.gameObject.GetComponent<HighlightableButton>();
                        hB.HighlightButton(HighlightableButton.ButtonHighlight.Info);
                        hightlightableBloxes.Add(hB);
                    }
                }

            }

        }
    }

    /// <summary>
    /// Grabs the list of the highlighted object, and resets it
    /// </summary>
    private void ResetHightlight()
    {
        hightlightableBloxes.ForEach(h => { h.HighlightButton(HighlightableButton.ButtonHighlight.None); });
        hightlightableBloxes = new List<HighlightableButton>();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        IsBeingDragged = false;
        print("End dragging");

        ResetHightlight();

        if (collidedObjects.Count > 0)
        {
            // Although we are saving all the simultaneous collisions 
            // We only want this object to nest with the highest ones
            
            float maxY = collidedObjects.Max(c => c.transform.position.y);
            List<GameObject> highestObjects = collidedObjects.Where(c => c.transform.position.y == maxY).Select(a => a.gameObject).ToList();


            foreach (GameObject collidedObject in highestObjects)
            {
                //If the collided object is a blox, attempts nesting
                if (GameObjectHelper.HasComponent<ABlox>(collidedObject))
                {
                    ABlox blox = collidedObject.GetComponent<ABlox>();
                    blox.NestObject(this.gameObject);
                }
                //If the collied object is the trashbin, destroys this blox
                else if (GameObjectHelper.HasComponent<Trashbin>(collidedObject))
                {
                    RemoveFromParent(this);
                    Destroy(this.gameObject);
                }
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
        // We are only going to add to the list of collided objects, bloxes that are not params
        // and trashbin
        if ((GameObjectHelper.HasComponent<ABlox>(collision.gameObject) && !collision.gameObject.GetComponent<ABlox>().IsParam) || GameObjectHelper.HasComponent<Trashbin>(collision.gameObject))
        { 
                collidedObjects.Add(collision);
 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ResetHightlight();
        collidedObjects.Remove(collision); 
    }
    #endregion

    #region Nesting Handling

    /// <summary>
    /// Checks if object passed can be nested to this one, and returns nesting type
    /// </summary>
    /// <param name="secondObject"></param>
    /// <returns></returns>
    public NestingType DetermineNestingType(GameObject secondObject)
    {
        NestingType nestingType = NestingType.NONE;
        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null && NestingActive)
        {
            if (secondObject.GetComponent<ABlox>() != null && secondObject != null && ValidateNesting(secondObject))
            {
                ABlox secondObjectBlox = secondObject.GetComponent<ABlox>();
                Vector2 thisObjectPosition = this.gameObject.transform.position;
                BoundingBox2D thisBBox = GameObjectHelper.getBoundingBoxInWorld(this.gameObject);
                BoundingBox2D secondObjBBox = GameObjectHelper.getBoundingBoxInWorld(secondObject);

                float thisObjectWidth = GameObjectHelper.getWidthFromBBox(thisBBox);
                float thisObjectHeight = GameObjectHelper.getHeightFromBBox(thisBBox);

                //checks if second object top is bellow this object center
                if (secondObjBBox.top.y < thisObjectPosition.y)
                {
                    if (!secondObjectBlox.IsParam && ValidateNestToBottom(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.left.x, thisObjectWidth / 4))
                    {
                        nestingType = NestingType.BOTTOM;
                    }
                    else if (!secondObjectBlox.IsParam && ValidateNestToBottomIdented(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.bottom.x, thisObjectWidth / 4))
                    {
                        nestingType = NestingType.BOTTOM_IDENTED;
                    }

                }
                else //if it is above
                {
                    // Checks if the left parth of the second object is near the right part of the first, and verifies if they are kind of aligned
                    if (ValidateNestToTheSide(secondObject) && MathHelper.IsNearby(secondObjBBox.left.y, thisBBox.right.y, thisObjectHeight / 4)
                    && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.right.x, thisObjectWidth / 4))
                    {
                        // Nest side to side 
                        nestingType = NestingType.SIDE;
                    }
                }

            }
        }
        return nestingType;
    }

    public void NestObject(GameObject secondObject)
    {
        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null && NestingActive)
        {
            if (secondObject.GetComponent<ABlox>() != null && secondObject != null && ValidateNesting(secondObject))
            {
                ABlox secondObjectBlox = secondObject.GetComponent<ABlox>();
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
                    if (!secondObjectBlox.IsParam && ValidateNestToBottom(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.left.x, thisObjectWidth / 4))
                    {
                        AddToBottom(secondObjectBlox);
                        OnNestToBottom();
                    }
                    else if (!secondObjectBlox.IsParam && ValidateNestToBottomIdented(secondObject) && MathHelper.IsNearby(secondObjBBox.left.x, thisBBox.bottom.x, thisObjectWidth / 4))
                    {
                        AddToBottomIdented(secondObjectBlox); 
                        OnNestToBottomIdented();
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
                        OnNestToSide();
                    }
                }
                OnNest();
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

    public virtual void OnNest() { }
    public virtual void OnNestToBottom() { }
    public virtual void OnNestToBottomIdented() { }
    public virtual void OnNestToSide() { }

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
    protected void SetChildBloxesPositionOnScreen(ABlox parentBlox, float bloxVerticalSpacing, float identSpacing, float paramSpacing, bool nestingActive = true)
    {
        BoundingBox2D parentBBox = GameObjectHelper.getBoundingBoxInWorld(parentBlox.gameObject);
        Vector2 parentBloxLeft = parentBBox.left;

        // Gets all the nodes (except the ones nested to the side and parent)
        List<BloxIdent> bloxIdentList = parentBlox.GetChildBloxListInVerticalOrder();
        ABlox previousBlox = parentBlox;

        // Sets the parent blox params
        SetBloxParamsPositionOnScreen(parentBlox, paramSpacing, nestingActive);


        foreach (BloxIdent bloxIdent in bloxIdentList)
        {
            ABlox blox = bloxIdent.blox;
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
            ABlox previousBloxInLine = blox;

            previousBlox = blox;
        }
    }

    /// <summary>
    /// Positions the blox params on screen
    /// </summary>
    /// <param name="blox"></param>
    protected void SetBloxParamsPositionOnScreen(ABlox blox, float paramSpacing, bool nestingActive = true)
    {
        ABlox previousBloxInLine = blox;
        BoundingBox2D bloxBBox = GameObjectHelper.getBoundingBoxInWorld(blox.gameObject);
        float bloxWidth = GameObjectHelper.getWidthFromBBox(bloxBBox);
        foreach (ABlox param in blox.BloxParams)
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
    protected void AddToBottom(ABlox blox)
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
    protected void AddToBottomIdented(ABlox blox)
    {
        RemoveFromParent(blox);
        this.ChildBloxes.Insert(0, blox);
        blox.ParentBlox = this;
    }

    protected void AddParam(ABlox param)
    {
        RemoveFromParent(param);
        BloxParams.Add(param);
        param.ParentBlox = this;
    }

    /// <summary>
    /// Takes a blox and removes any reference inside it's parent
    /// </summary>
    /// <param name="blox"></param>
    protected void RemoveFromParent(ABlox blox)
    {
        ABlox originalParent = blox.ParentBlox;
        if (originalParent != null)
        {
            originalParent.OnBeforeChildRemove(blox);
            originalParent.ChildBloxes.Remove(blox);
            originalParent.BloxParams.Remove(blox);
        }
        blox.ParentBlox = null;
    }

    /// <summary>
    /// This method is called when a blox is going to be eliminated from parent
    /// </summary>
    public virtual void OnBeforeChildRemove(ABlox blox)
    {

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
    public List<BloxIdent> GetChildBloxListInVerticalOrder(int ident = 1)
    {
        List<BloxIdent> bloxList = new List<BloxIdent>();
        foreach (ABlox child in ChildBloxes)
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
    /// Gets all the bloxes bellow this one, not matter if they are child or param
    /// This list won't express parental relations.
    /// </summary>
    /// <returns></returns>
    public List<ABlox> GetAllBloxesBellow()
    {
        List<ABlox> bloxList = new List<ABlox>();
        foreach(ABlox child in ChildBloxes)
        {
            bloxList.Add(child);
            child.BloxParams.ForEach(p => bloxList.Add(p));
            bloxList.AddRange(child.GetAllBloxesBellow());
        }
        return bloxList;
    }

    
    /// <summary>
    /// From the current blox, gets all the bloxes in scope, in vertical order
    /// The first one in list will be the closest to this one, and the last the root
    /// Does not consider param bloxes
    /// Example:
    /// What are the bloxes in scope of c.2.2?
    /// root
    /// -a
    /// --a.1
    /// ---a.1.1
    /// -b
    /// --b.1
    /// --b.2
    /// -c
    /// --c.1
    /// ---c.1.1
    /// --c.2
    /// ---c.2.1
    /// ---c.2.2 [c.2.2.p1]
    /// ---c.2.3
    /// 
    /// Answer: [c.2.1, c.2, c.1, c, b, a, root]
    ///          The answer shall be the same for c.2.2.p1 (which is a param for c.2.2)
    /// </summary>
    /// <param name="blox"></param>
    /// <returns></returns>
    protected List<ABlox> GetBloxesInScope(ABlox blox)
    {
        // This is the recursion stop condition
        if (blox == null || GameObjectHelper.CanBeCastedAs<RootBlox>(blox))
            return new List<ABlox>() { blox };

        ABlox auxBlox = blox.IsParam ? blox.ParentBlox : blox; //if blox is a param,considers its parent

        List<ABlox> scope = new List<ABlox>();
        ABlox parentBlox = auxBlox.ParentBlox;
        int indexOfBloxInParent = parentBlox.ChildBloxes.IndexOf(auxBlox);
        for (int i = indexOfBloxInParent - 1; i >= 0; i--)
        {
            scope.Add(parentBlox.ChildBloxes[i]);
        }
        scope.Add(parentBlox);
        scope.AddRange(GetBloxesInScope(parentBlox));

        return scope;
    }

    /// <summary>
    /// Gets all the variable bloxes on the scope of a blox. They are presented in the following order: 
    /// the one closest to blox, to the one closest to root.
    /// </summary>
    /// <param name="blox"></param>
    /// <returns></returns>

    public List<IBloxVariable> GetVariablesInBloxScope(ABlox blox)
    {
        List<IBloxVariable> variablesInScope = new List<IBloxVariable>();

        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null)
        {
            List<ABlox> bloxesInScope = blox.GetBloxesInScope(blox);
            variablesInScope = bloxesInScope.Where(b => GameObjectHelper.CanBeCastedAs<IBloxVariable>(b)).Select(b => (IBloxVariable)b).ToList();
        }
        return variablesInScope;
    }

    [Obsolete]
    public List<IBloxVariable> GetVariablesInBloxScope2(ABlox blox)
    {
        List<IBloxVariable> variablesInScope = new List<IBloxVariable>();


        RootBlox rootBlox = GetRootBlox();
        if (rootBlox != null)
        {
            List<BloxIdent> bloxList = rootBlox.GetChildBloxListInVerticalOrder();
            //Params are not loaded in GetChildBloxListInVerticalOrder() method, so we evaluate according to the parent index
            //on that case
            BloxIdent bloxIdent = bloxList.Find(b => b.blox == (blox.IsParam ? blox.ParentBlox : blox));
            BloxIdent bloxIdent2 = bloxList.Where(b => b.blox == (blox.IsParam ? blox.ParentBlox : blox)).FirstOrDefault();

            int indexOfBlox = -1;
            try
            {
                indexOfBlox = bloxList.IndexOf(bloxIdent);
            }
            catch (Exception ex) { }

            if (indexOfBlox >= 0)
            {
                // Gets all the bloxes above this one, that are variables
                // Although pure Blox objects cannot be cast as IBloxVariable,
                // objects of classes that inherit from Blox, and implement IBloxVariable, can
                variablesInScope = bloxList.GetRange(0, indexOfBlox).Where(b => GameObjectHelper.CanBeCastedAs<IBloxVariable>(b.blox)).Select(b => (IBloxVariable)b.blox).ToList();
            }

        }
        return variablesInScope;
    }

    /// <summary>
    /// Verifies if in a blox scope there is already a variable with a given name.
    /// Ignores null or whitespace (returns false when name is null or empty)
    /// </summary>
    /// <param name="blox"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool VariableExistsInBloxScope(ABlox blox, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        // From the list of variables available in scope, gets the ones that are not null or empty,
        // and counts the number of corresponding cases, and returns true if it is bigger than zero
        return GetVariablesInBloxScope(blox).Where(a => !String.IsNullOrWhiteSpace(a.GetName())
                                                        && a.GetName().ToLower() == name.ToLower()).Count() > 0;
    }


    #endregion

    #region Blox Compilation helpers

    protected void CompileChildrenToNodes(ICodeNode parentNode)
    {
        foreach (ABlox child in ChildBloxes)
        {
            if (GameObjectHelper.CanBeCastedAs<ICompilableBlox>(child))
            {
                //Creates nodes for each child. Each child will add parentNode as a parent
                ((ICompilableBlox)child).ToNodes(parentNode);
            }
        }
    }

    protected List<BloxValidationError> ValidateBloxList(List<ABlox> bloxList)
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();
        foreach (ABlox child in bloxList)
        {
            if (GameObjectHelper.CanBeCastedAs<ICompilableBlox>(child))
            {
                errors.AddRange(((ICompilableBlox)child).Validate());
            }

        }
        return errors;
    }


    #endregion
}
