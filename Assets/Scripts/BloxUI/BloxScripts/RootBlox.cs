using Assets.Scripts.Terminal.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RootBlox : ABlox, ICompilableBlox
{
    public RootNode rootNode { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<Button>().colors.;
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

    public List<BloxValidationError> Validate()
    {
        return ValidateBloxList(ChildBloxes);
    }

    public void ToNodes(ICodeNode parentNode)
    {
        //Root blox is not supposed to have a parent, so parentNode is assumed as being null
        
        //Creates the instance of this node, that will be saved in RootBlox
        rootNode = new RootNode();
        //Compiles the children
        CompileChildrenToNodes(rootNode);
    }
}
