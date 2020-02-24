﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CodeNode : ICodeNode
{
    protected bool Loop { get; set; }

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
    public List<ICodeNode> NodesInScope {
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
    public object ReturnValue { get; set; }
    public List<object> Parameters { get; set; }

    public CodeNode()
    {
        ChildNodes = new List<ICodeNode>();
        NodesInScope = new List<ICodeNode>();
        ParentNode = new CodeNode();
        Loop = false;
        Parameters = new List<object>();
    }

    public object Execute()
    {
        OnBeforeExecuteAction();
        while (Loop)
        {
            OnBeforeChildNodesExecuteAction();
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
        if(currentIdx > 0 && NodesInLevel.Count > 1)
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
        ChildNodes.Add(childNode);
    }

    public virtual void OnBeforeChildNodesExecuteAction()
    {
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
}
