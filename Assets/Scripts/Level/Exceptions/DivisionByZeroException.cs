using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This exception is thrown when attempting to move forward
/// to a place that as no tile
/// </summary>
public class DivisionByZeroException : CodeBloxException
{
    public DivisionByZeroException() : base(BloxValidationErrorMessages.DIVISION_BY_ZERO_EXCEPTION)
    {

    }
}