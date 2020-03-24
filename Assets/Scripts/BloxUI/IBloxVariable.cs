using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines the contract for a blox that also is a variable
/// </summary>
public interface IBloxVariable 
{
    string GetName();
    string GetValue();
}
