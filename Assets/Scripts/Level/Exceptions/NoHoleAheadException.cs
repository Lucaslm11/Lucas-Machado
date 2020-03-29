using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This exception is thrown when attempting to move forward
/// to a place that as no tile at a lower level than the current
/// </summary>
public class NoHoleAheadException : CodeBloxException
{
    public NoHoleAheadException() : base(BloxValidationErrorMessages.NO_HOLE_AHEAD_EXCEPTION)
    {

    }
}