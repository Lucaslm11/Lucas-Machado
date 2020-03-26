using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Terminal.Nodes;

public class CodeNode : ICodeNode
{
    public string NodeName { get; set; }
    public virtual bool CanHaveChildren { get { return false; } }
    /// <summary>
    /// Represents the node that the current one is child of
    /// </summary>
    public ICodeNode ParentNode { get; set; }
    /// <summary>
    /// Contains the nodes that are child of the current one
    /// </summary>
    public List<ICodeNode> ChildNodes { get; set; }
    /// <summary>
    /// Contains the nodes that belong to the same scope as the current
    /// </summary>
    public List<ICodeNode> NodesInScope
    {
        get; set;
    }
    /// <summary>
    /// Contains the nodes that belong to the same level as the current
    /// </summary>
    public List<ICodeNode> NodesInLevel
    {
        get
        {
            return ParentNode.ChildNodes;
        }
        set { }
    }
    /// <summary>
    /// Contains the value to be returned on Execute method
    /// </summary>
    public virtual object ReturnValue { get; set; }
    public List<ICodeNode> Parameters { get; set; }
    List<object> ICodeNode.Parameters { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public CodeNode()
    {
        ChildNodes = new List<ICodeNode>();
        NodesInScope = new List<ICodeNode>();
        //Parent node default is null. Don't do a new on this one, or will enter in infinite recursion
        ParentNode = null; 
        Parameters = new List<ICodeNode>();
        NodeName = UnityEditor.GUID.Generate().ToString();
    }

    public object Execute()
    {
        OnBeforeExecuteAction();
        while (OnBeforeChildNodesExecuteAction())
        {
            if (CanHaveChildren)
                foreach (ICodeNode childNode in ChildNodes)
                {
                    childNode.Execute();
                }
            OnAfterChildNodesExecuteAction();
        }
        OnAfterExecuteAction();

        return ReturnValue;
    }

    /// <summary>
    /// Moves the node up, in the same level
    /// </summary>
    public void moveUp()
    {
        // Finds the index of the current node
        int currentIdx = NodesInLevel.IndexOf(this);
        // Swaps the current element with the previous, if the current one is not already the first
        if (currentIdx > 0 && NodesInLevel.Count > 1)
        {
            ListHelper.Swap<ICodeNode>(NodesInLevel, currentIdx - 1, currentIdx);
        }
    }

    public void moveDown()
    {
        // Finds the index of the current node
        int currentIdx = NodesInLevel.IndexOf(this);
        // Swaps the current element with the next, if the current one is not already the last
        if (currentIdx < (NodesInLevel.Count - 1) && NodesInLevel.Count > 1)
        {
            ListHelper.Swap<ICodeNode>(NodesInLevel, currentIdx + 1, currentIdx);
        }
    }

    public void moveToChild()
    {
        throw new System.NotImplementedException();
    }

    public void moveToParent()
    {
        throw new System.NotImplementedException();
    }


    public void AddChildNode(ICodeNode childNode)
    {
        if (CanHaveChildren)
        {
            childNode.ParentNode = this;
            ChildNodes.Add(childNode);
        }
    }

    public void AddChildNodes(List<ICodeNode> childNodes)
    {
        if (CanHaveChildren)
        {
            ChildNodes.ForEach(child =>
            {
                AddChildNode(child);
            }); 
        }
    }


    public virtual bool OnBeforeChildNodesExecuteAction()
    {
        return false;
    }

    public virtual void OnAfterChildNodesExecuteAction()
    {
    }

    public virtual void OnBeforeExecuteAction()
    {
    }

    public virtual void OnAfterExecuteAction()
    {
    }


    public ICodeNode SearchChildByName(string NodeName)
    {
        if (this.NodeName == NodeName)
            return this;
        else
        {
            foreach(ICodeNode codeNode in ChildNodes)
            {
                ICodeNode foundNode = codeNode.SearchChildByName(NodeName);
                if (foundNode != null)
                    return foundNode;
            }
        }

        return null;
    }

 
    public RootNode GetRootNode()
    {
        if (GameObjectHelper.CanBeCastedAs<RootNode>(this))
            return (RootNode)this;
        else
            return ParentNode.GetRootNode();
    }
}
