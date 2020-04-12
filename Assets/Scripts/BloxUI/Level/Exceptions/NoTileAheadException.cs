using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// This exception is thrown when attempting to move forward
/// to a place that as no tile
/// </summary>
public class NoTileAheadException : CodeBloxException
{
    public NoTileAheadException() : base(BloxValidationErrorMessages.NO_TILE_AHEAD_EXCEPTION)
    {

    }
}