using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.Scripts.Terminal.Nodes;

public interface ICodeNode 
{
    bool CanHaveChildren { get; }
    string NodeName { get; set; }
    List<object> Parameters { get; set; }
    ICodeNode ParentNode { get; set; }
    List<ICodeNode> ChildNodes { get; set; }
    object ReturnValue { get; set; } // value to return on execute
    object Execute();
    List<ICodeNode> NodesInScope { get; set; }
    List<ICodeNode> NodesInLevel { get; set; }
 
 

    void OnBeforeExecuteAction();
    void OnAfterExecuteAction();

    bool OnBeforeChildNodesExecuteAction();
    void OnAfterChildNodesExecuteAction();

    void AddChildNode(ICodeNode childNode); 

    ICodeNode SearchChildByName(string NodeName);

    RootNode GetRootNode(); 
}