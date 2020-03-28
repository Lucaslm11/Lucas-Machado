using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This exception is thrown when attempting to move forward
/// to a place that as no tile at the same level as the current
/// </summary>
public class HoleAheadException : Exception
{
    public HoleAheadException() : base()
    {

    }
}