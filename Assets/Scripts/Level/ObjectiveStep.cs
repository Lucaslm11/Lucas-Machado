using UnityEngine;
using UnityEditor;

/// <summary>
/// Describes an objective to complete inside the terrain plot.
/// 
/// CoordinateInPlot describes which cube the character must step in
/// and SpecialAction defines if in this cube the character is supposed to do
/// a SpecialAction
/// </summary>
[System.Serializable]
public class ObjectiveStep 
{
    /// <summary>
    /// Cube in which the character is supposed to step into
    /// </summary>
    [SerializeField] Vector3Int CoordinateInPlot;

    /// <summary>
    /// Is the character supposed to do a special action
    /// </summary>
    [SerializeField] bool SpecialAction;

    
}