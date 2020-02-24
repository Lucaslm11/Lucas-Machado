using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface ICodeNode 
{
    List<object> Parameters { get; set; }
    ICodeNode ParentNode { get; set; }
    List<ICodeNode> ChildNodes { get; set; }
    object ReturnValue { get; set; } // value to return on execute
    object Execute();
    List<ICodeNode> NodesInScope { get; set; }
    List<ICodeNode> NodesInLevel { get; set; }
    void moveUp();
    void moveDown();
    void moveToParent();
    void moveToChild();

    void OnBeforeExecuteAction();
    void OnAfterExecuteAction();

    void OnBeforeChildNodesExecuteAction();
    void OnAfterChildNodesExecuteAction();

    void AddChildNode(ICodeNode childNode);

}