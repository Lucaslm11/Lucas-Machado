using Assets.Scripts.Terminal.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines the contract for a blox that can be compiled into a node
/// </summary>
public interface ICompilableBlox
{
    /// <summary>
    /// Generate a list of validation errors
    /// </summary>
    /// <returns></returns>
    List<BloxValidationError> Validate();

    /// <summary>
    /// Generates the necessary nodes. Each ToNodes call will add new created node to the parentNode
    /// </summary>
    /// <returns></returns> 
    void ToNodes(ICodeNode parentNode);
}
 
public struct BloxValidationError
{
    public string BloxName;
    public string ErrorMessage;
    public GameObject TargetObject;
}
