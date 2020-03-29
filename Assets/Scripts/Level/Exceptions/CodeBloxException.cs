using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This is a base class for exceptions that occur in the context 
/// of Blox execution. For example, character will fall on a next step
/// it shall throw an exception.
/// </summary>
public class CodeBloxException : Exception
{
    public ABlox blox { get; set; }
    public CodeBloxException(string Message) : base(Message)
    {

    }
}