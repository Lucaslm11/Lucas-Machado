using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// This class helps to describe a level.
/// Each level will have a terrain/tile plot (similar to LightBot), described by a 2.5D scene,
/// that will consist in group of cubes/tiles. Each cube has a discrete coordinate inside this plot.
///   
/// 2 _ _ _ (2,2)
///  |_|_|_|
///  |_|_|_|
/// 0|_|_|_|
///  0      2
///  
/// This coordinates can be translated to coordinates in scene
/// </summary>
/// 

//  TODO: REMOVE THE PUBLIC STATEMENTS, AND MAKE GET FUNCTIONS
[System.Serializable]
public class LevelDescriptor
{
    public enum CharacterOrientation
    {
        /// <summary>
        /// Z positive. RotY is 0
        /// </summary>
        NORTH = 0, 
        /// <summary>
        /// Z negative. RotY is 180 degrees
        /// </summary>
        SOUTH = 180,
        /// <summary>
        /// X positive. RotY is 90 degrees
        /// </summary>
        EAST = 90,
        /// <summary>
        /// X negative. RotY is -90 degrees
        /// </summary>
        WEST = -90   
    }

    /// <summary>
    /// Character controller object. We are assuming that the default orientation is to North (pointing to Z positive)
    /// </summary>
    [SerializeField] public CubotController Character;

    /// <summary>
    /// Defines the first tile in plot, from which others will be drawn
    /// </summary>
    [SerializeField] public PlotTile FirstTile;

    /// <summary>
    /// Defines the plot width
    /// </summary>
    [SerializeField] public int PlotWidth;
    /// <summary>
    /// Defines the plot height
    /// </summary>
    [SerializeField] public int PlotHeight;

    [SerializeField] public float CharacterSpeed;


    /// <summary>
    /// Defines the Character starting point
    /// </summary>
    [SerializeField] public Vector3Int CharacterStartPointInPlot;
    /// <summary>
    /// Defines the initial orientation of the character
    /// </summary>
    [SerializeField] public CharacterOrientation CharacterInitialOrientation;

    /// <summary>
    /// Overrides a coordinate height. The PlotWidth and PlotHeight fields will be used
    /// to define the plot, but defaulting hight to 0. 
    /// If the PlotWidth and PlotHeight are 3 (it's a 3x3), a SpecialCoordinate of (4,4,1), wont be valid,
    /// but a (1,1,1) will be.
    /// </summary>
    [SerializeField] public List<Vector3Int> SpecialCoordinates;

    /// <summary>
    /// This is only for type check. Defines which bloxes are mandatory for use
    /// </summary>
    [SerializeField] public List<ExpectedBlox> MandatoryBloxes;

    /// <summary>
    /// This is only for type check. Defines which bloxes are expected to be used
    /// but are not mandatory
    /// </summary>
    [SerializeField] public List<ExpectedBlox> ExpectedBloxes;

    /// <summary>
    /// Describes the cubes in plot the player must step into
    /// </summary>
    [SerializeField] public List<ObjectiveStep> MandatorySteps;


    // Note: this variables won't limit the player in game. 
    //       They will only define the maximum for evaluation purposes

    /// <summary>
    /// Defines the expected maximum code lines (we are not including params on this count!)
    /// </summary>
    [SerializeField] public int MaxCodeLinesExpected;

    /// <summary>
    /// Defines the expected maximum time
    /// </summary>
    [SerializeField] public int MaxTimeInMinutes;

    /// <summary>
    /// Defines the expected maximum attempts
    /// </summary>
    [SerializeField] public int MaxAttempts;

    /// <summary>
    /// Defines the minimum stars for success
    /// </summary>
    [SerializeField] public int MinimumStarsForSuccess;


    public void Configure()
    {
        // The lists with ExpectedBlox class receive a Blox as a configuration parameter
        // but we will only need its type, and since they can get destroyed later, because they are game objects
        // we save their type

        ExpectedBloxes.ForEach(item => item.BloxType = item.Blox.GetType());
        MandatoryBloxes.ForEach(item => item.BloxType = item.Blox.GetType());
    }
}