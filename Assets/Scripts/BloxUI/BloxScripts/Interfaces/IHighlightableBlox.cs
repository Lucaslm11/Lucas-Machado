using Assets.Scripts.Terminal.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines the contract for a blox that can be compiled into a node
/// </summary>
public interface IHighlightableBlox
{
    void Highlight(BloxHighlight blox);
}
 
public enum BloxHighlight
{
    // No highlight shall be applied
    None,
    // A success highlight shall be applied
    Success,
    // A warning highlight shall be applied
    Warning,
    // An error highlight shall be applied
    Error,
    // An info highlight shall be applied
    Info
}
