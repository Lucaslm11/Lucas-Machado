using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines the contract for a blox that can be compiled into a node
/// </summary>
public interface ICompilable
{
    /// <summary>
    /// Generate a list of validation errors
    /// </summary>
    /// <returns></returns>
    List<ValidationError> Validate();
    ICodeNode ToNode();
}
 
public struct ValidationError
{
    public string BloxName;
    public string ErrorMessage;
    public GameObject TargetObject;
}
