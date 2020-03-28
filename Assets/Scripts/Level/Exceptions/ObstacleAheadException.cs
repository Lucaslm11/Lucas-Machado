using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This exception is thrown when attempting to move forward
/// to a tile that is elevated
/// </summary>
public class ObstacleAheadException : Exception
{
    public ObstacleAheadException() : base()
    {

    }
}