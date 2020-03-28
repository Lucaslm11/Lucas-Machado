using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This exception is thrown when attempting to climb up
/// to a tile that is not elevated
/// </summary>
public class NoObstacleAheadException : Exception
{
    public NoObstacleAheadException() : base()
    {

    }
}