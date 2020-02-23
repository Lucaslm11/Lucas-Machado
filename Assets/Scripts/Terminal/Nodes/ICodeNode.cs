using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface ICodeNode 
{
    ICodeNode ParentNode { get; set; }
    List<ICodeNode> ChildNodes { get; set; }
    Object ReturnValue { get; set; } // value to return on execute
    Object Execute(params object[] parameters);
    List<ICodeNode> NodesInScope { get; set; }
    List<ICodeNode> NodesInLevel { get; set; }
    void moveUp();
    void moveDown();
    void moveToParent();
    void moveToChild();

    void OnExecuteAction(params object[] parameters);
    void OnAfterExecuteAction(params object[] parameters);

}