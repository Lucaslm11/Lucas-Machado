using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This exception is thrown when attempting to climb up
/// to a tile that is not elevated
/// </summary>
public class NoObstacleAheadException : CodeBloxException
{
    public NoObstacleAheadException() : base(BloxValidationErrorMessages.NO_OBSTACLE_AHEAD)
    {

    }
}